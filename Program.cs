using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System.IO;
namespace 地形校正
{
    class Program
    {
        static void Main(string[] args)
        {
            //可供修改的参数，以便满足不同的需要
            int ratio=1;
            string StationType="QX";
            string TranformType="AVE";//选择"PSF"或"AVE"
            int R1 = 1500;//太阳光线遮蔽的搜索范围
            int R2 = 1000;//天空视野遮蔽的搜索范围
            int R3 = 600;//相邻像元间遮蔽的搜索范围
            string CorrectType = "Hay";//选择"Hay"和"Iso"的校正模式
            string[] time = new string[4] { "20120608","20120704","20120803","20110612"};
            //********************************************************************
            for (int StationNumber = 3; StationNumber <= 3; StationNumber++)
            {
                string DEMPath = "E:\\dem\\" + "QXStationNumber" + StationNumber + "_size.tif";
                OSGeo.GDAL.Gdal.AllRegister();
                OSGeo.GDAL.Dataset dataSet = OSGeo.GDAL.Gdal.Open(DEMPath, OSGeo.GDAL.Access.GA_ReadOnly);
                int xSize = dataSet.RasterXSize;//30米分辨率原始影像的行列数
                int ySize = dataSet.RasterYSize;
                int[,] DEM30 = new int[ySize, xSize];
                DEM30 = Read_WriteData.ReadDEM30(DEMPath);
                double[] edGeo_before = new double[6];
                dataSet.GetGeoTransform(edGeo_before);
                double[] edGeo_now = new double[6];//尺度转换后的坐标信息
                edGeo_now[0] = edGeo_before[0];
                edGeo_now[1] = edGeo_before[1] * ratio;
                edGeo_now[2] = edGeo_before[2];
                edGeo_now[3] = edGeo_before[3];
                edGeo_now[4] = edGeo_before[4];
                edGeo_now[5] = edGeo_before[5] * ratio;
                float cellsize;//尺度转换后的分辨率（米）
                cellsize = (float)(111120 * edGeo_before[1]);
                int step1 = (int)(R1 / cellsize);
                int step2 = (int)(R2 / cellsize);
                int step3 = (int)(R3 / cellsize);
                string projection;
                projection = dataSet.GetProjection();

                int[,] DEM_now = new int[ySize / ratio, xSize / ratio];
                if (TranformType == "PSF")
                {
                    DEM_now = TransformDEM.PSF_NxN(xSize, ySize, ratio, DEM30);
                }
                if (TranformType == "AVE")
                {
                    DEM_now = TransformDEM.AverageDEM(xSize, ySize, ratio, DEM30);
                }
                float[,] slope = new float[ySize / ratio, xSize / ratio];
                float[,] aspect = new float[ySize / ratio, xSize / ratio];
                float[,] vd = new float[ySize / ratio, xSize / ratio];
                slope = topo_factors.Cal_slope(xSize, ySize, ratio, DEM_now, cellsize);
                aspect = topo_factors.Cal_aspect(xSize, ySize, ratio, DEM_now, cellsize);
                vd = topo_factors.VD(xSize, ySize, ratio, DEM_now, cellsize, step2);

                for (int tim = 0; tim < time.Length; tim++)
                {
                    Console.WriteLine("{0},{1},{2}", time[tim].Substring(0, 4), time[tim].Substring(4, 2), time[tim].Substring(6, 2));
                    var DirradFiles = Directory.GetFiles("E:\\simrad_t\\" + StationType + StationNumber + "\\insta", "*" + "" + time[tim] + "" + "*dirrad*");  //遍历文件
                    string[] DirradFilesPath = DirradFiles;//获取水平直接辐射文件目录
                    string[] DirradFilesName = new string[DirradFiles.Length];
                    for (int i = 0; i < DirradFiles.Length; i++)
                    {
                        DirradFilesName[i] = Path.GetFileNameWithoutExtension(DirradFilesPath[i]);//获取水平直接辐射文件名
                    }
                    string[] DifradFilesPath = new string[DirradFiles.Length];
                    for (int i = 0; i < DirradFiles.Length; i++)
                    {
                        //根据水平直接辐射文件名获取散射辐射的文件目录
                        DifradFilesPath[i] = "E:\\simrad_t\\" + StationType + StationNumber + "\\insta\\" + DirradFilesName[i].Substring(0, 10) + "difrad_" + StationType + StationNumber + "_simulate.tif";
                    }

                    float[,] DailyDir = new float[ySize / ratio, xSize / ratio];
                    float[,] DailyDif = new float[ySize / ratio, xSize / ratio];
                    float[,] DailyAll = new float[ySize / ratio, xSize / ratio];
                    for (int N = 0; N < DirradFiles.Length; N++)
                    {
                        int bj_year, bj_month, bj_day, bj_hour;
                        string nian, yue, ri, shi;
                        nian = DirradFilesName[N].Substring(0, 4);
                        yue = DirradFilesName[N].Substring(4, 2);
                        ri = DirradFilesName[N].Substring(6, 2);
                        shi = DirradFilesName[N].Substring(8, 2);
                        bj_year = Convert.ToInt32(nian);
                        bj_month = Convert.ToInt32(yue);
                        bj_day = Convert.ToInt32(ri);
                        bj_hour = Convert.ToInt32(shi);

                        //校正直接辐射
                        float[,] dir_H_now = new float[ySize / ratio, xSize / ratio];
                        float[,] dir_T_now = new float[ySize / ratio, xSize / ratio];
                        float[,] solar_alt = new float[ySize / ratio, xSize / ratio];
                        float[,] solar_azi = new float[ySize / ratio, xSize / ratio];
                        int[,] shade = new int[ySize / ratio, xSize / ratio];
                        float[,] IncidenceAngle = new float[ySize / ratio, xSize / ratio];
                        dir_H_now = Read_WriteData.ReadDirRadition_H(DirradFilesPath[N], ratio);
                        solar_alt = topo_factors.solar_altitude(xSize / ratio, ySize / ratio, bj_year, bj_month, bj_day, bj_hour, 0, edGeo_before[0], edGeo_before[3], edGeo_before[1] * ratio);
                        solar_azi = topo_factors.solar_azimuth(xSize / ratio, ySize / ratio, bj_year, bj_month, bj_day, bj_hour, 0, edGeo_before[0], edGeo_before[3], edGeo_before[1] * ratio);
                        IncidenceAngle = topo_factors.Angle_of_Incidence(xSize, ySize, ratio, solar_alt, solar_azi, slope, aspect);
                        shade = topo_factors.shading(xSize, ySize, ratio, solar_alt, solar_azi, DEM_now, step1, cellsize);
                        dir_T_now = CorrectionRad.DirRadition_correction(xSize, ySize, ratio, dir_H_now, shade, IncidenceAngle, solar_alt);
                        string PathInsta = "E:\\CorRad_t\\" + StationType + StationNumber + "\\insta\\";
                        Directory.CreateDirectory(PathInsta);
                        string OutDirrad_h_path = PathInsta + nian + yue + ri + shi + "dirrad_" + StationType + StationNumber + "_corrected" + "" + 30 * ratio + "" + ".tif";
                        Read_WriteData.OutputToTif(OutDirrad_h_path, dir_T_now, edGeo_now, projection);
                        //校正散射辐射
                        float[,] dif_H_now = new float[ySize / ratio, xSize / ratio];
                        float[,] dif_T_now = new float[ySize / ratio, xSize / ratio];
                        dif_H_now = Read_WriteData.ReadDifRadition_H(DifradFilesPath[N], ratio);
                        if (CorrectType == "Hay")
                        {
                            dif_T_now = CorrectionRad.DifRadition_correction_Hay(xSize, ySize, ratio, dir_H_now, dif_H_now, vd, IncidenceAngle, solar_alt, shade);
                        }
                        if (CorrectType == "Iso")
                        {
                            dif_T_now = CorrectionRad.DifRadition_correction_isotropy(xSize, ySize, ratio, dif_H_now, vd);
                        }
                        string OutDifrad_h_path = PathInsta + nian + yue + ri + shi + "difrad_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                        Read_WriteData.OutputToTif(OutDifrad_h_path, dif_T_now, edGeo_now, projection);
                        Console.WriteLine("{0},{1}", OutDifrad_h_path, DateTime.Now);


                        ////***************************************************************
                        //输出地形因子
                        //string slopepath = @"I:\地形因子\QX56地形因子\QX56_Slope.tif";
                        //string aspectpath = @"I:\地形因子\QX56地形因子\QX56_Aspect.tif";
                        //Read_WriteData.OutputToTif(slopepath, slope, edGeo_now, projection);
                        //Read_WriteData.OutputToTif(aspectpath, aspect, edGeo_now, projection);
                        //string VDpath = @"I:\TopoFactors\QX56TopoFactors\QX56_Vd" + "" + R2 + "" + ".tif";
                        //Read_WriteData.OutputToTif(VDpath, vd, edGeo_now, projection);
                        //double min, max, mean, stddev;
                        //dataSet.GetRasterBand(1).ComputeStatistics(true,out min,out max,out mean,out stddev,null,null);
                        //Console.WriteLine("{0},{1},{2}",min,max,mean);
                        //Console.WriteLine("{0}", R2);
                        //Console.ReadKey();
                        ////*********************************************************************


                        ////计算邻近附加辐射
                        //float[,] albedo = new float[ySize / ratio, xSize / ratio];
                        //float[,] neirad = new float[ySize / ratio, xSize / ratio];
                        float[,] allrad = new float[ySize / ratio, xSize / ratio];
                        //int[,] hid = new int[2 * step3 + 1, 2 * step3 + 1];
                        //float[,] cosT1 = new float[2 * step3 + 1, 2 * step3 + 1];
                        //float[,] cosT2 = new float[2 * step3 + 1, 2 * step3 + 1];
                        //albedo = Read_WriteData.ReadAlbedo(xSize, ySize, ratio);
                        for (int row = 0; row < ySize / ratio; row++)
                        {
                            for (int col = 0; col < xSize / ratio; col++)
                            {
                                //hid = topo_factors.HID_judge(row, col, xSize, ySize, ratio, DEM_now, step3);
                                //cosT1 = topo_factors.T1(row, col, xSize, ySize, ratio, DEM_now, step3, cellsize);
                                //cosT2 = topo_factors.T2(row, col, xSize, ySize, ratio, DEM_now, step3, cellsize);
                                //*********************************************************************
                                //float[,] NeiFactor = new float[2 * step3 + 1, 2 * step3 + 1];
                                //for (int i = 0; i < 2 * step3 + 1; i++)
                                //{
                                //    for (int j = 0; j < 2 * step3 + 1; j++)
                                //    {
                                //        if (hid[i, j] == 1 && cosT1[i, j] > 0 && cosT2[i, j] > 0)
                                //        {
                                //            NeiFactor[i, j] = 1;
                                //        }
                                //        else
                                //        {
                                //            NeiFactor[i, j] = 0;
                                //        }
                                //        if (i == step3 && j == step3)
                                //        { NeiFactor[i, j] = 3; }
                                //    }
                                //}
                                //string NeiFactorpath = @"I:\TopoFactors\QX56TopoFactors\NeiFactor" + "" + R3 + "" + "NO5.tif";                  
                                //Read_WriteData.OutputToTif(NeiFactorpath, NeiFactor, edGeo_now, projection);
                                //*********************************************************************************************************
                                //neirad[row, col] = CorrectionRad.CalculateNeiRad(row, col, xSize, ySize, ratio, cosT1, cosT2, hid, dir_T_now, dif_T_now, slope, albedo, step3, cellsize);
                                //allrad[row, col] = dir_T_now[row, col] + dif_T_now[row, col] + neirad[row, col];
                                allrad[row, col] = dir_T_now[row, col] + dif_T_now[row, col];
                                DailyDir[row, col] = DailyDir[row, col] + dir_T_now[row, col] * 3600;
                                DailyDif[row, col] = DailyDif[row, col] + dif_T_now[row, col] * 3600;
                                DailyAll[row, col] = DailyAll[row, col] + allrad[row, col] * 3600;
                            }
                        }
                        //string OutNeirad_h_path = PathInsta + nian + yue + ri + shi + "neirad_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                        //Read_WriteData.OutputToTif(OutNeirad_h_path, neirad, edGeo_now, projection);
                        //Console.WriteLine("{0},{1}", OutNeirad_h_path, DateTime.Now);
                        string OutAllrad_h_path = PathInsta + nian + yue + ri + shi + "allrad_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                        Read_WriteData.OutputToTif(OutAllrad_h_path, allrad, edGeo_now, projection);
                        Console.WriteLine("{0},{1}", OutAllrad_h_path, DateTime.Now);
                    }
                    for (int row = 0; row < ySize; row++)
                    {
                        for (int col = 0; col < xSize; col++)
                        {
                            DailyDir[row, col] = DailyDir[row, col] / 1000000;
                            DailyDif[row, col] = DailyDif[row, col] / 1000000;
                            DailyAll[row, col] = DailyDir[row, col] + DailyDif[row, col];
                        }
                    }
                    string PathDaily = "E:\\CorRad_t\\" + StationType + StationNumber + "\\daily\\";
                    Directory.CreateDirectory(PathDaily);
                    string DailyDirPath = PathDaily + time[tim].Substring(0, 4) + time[tim].Substring(4, 2) + time[tim].Substring(6, 2) + "DailyDir_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                    string DailyDifPath = PathDaily + time[tim].Substring(0, 4) + time[tim].Substring(4, 2) + time[tim].Substring(6, 2) + "DailyDif_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                    string DailyAllPath = PathDaily + time[tim].Substring(0, 4) + time[tim].Substring(4, 2) + time[tim].Substring(6, 2) + "DailyAll_" + StationType + StationNumber + CorrectType + "_corrected" + "" + 30 * ratio + "" + ".tif";
                    Read_WriteData.OutputToTif(DailyDirPath, DailyDir, edGeo_now, projection);
                    Console.WriteLine("{0},{1}", DailyDirPath, DateTime.Now);

                    Read_WriteData.OutputToTif(DailyDifPath, DailyDif, edGeo_now, projection);
                    Console.WriteLine("{0},{1}", DailyDifPath, DateTime.Now);

                    Read_WriteData.OutputToTif(DailyAllPath, DailyAll, edGeo_now, projection);
                    Console.WriteLine("{0},{1}", DailyAllPath, DateTime.Now);
                }
          }
               
            
           
        
          Console.WriteLine("over");
          Console.ReadKey();
        }
    }
}
