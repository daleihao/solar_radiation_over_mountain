using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
namespace 地形校正
{
    class Read_WriteData
    {

        //读取30米分辨的DEM数据
        static public int[,] ReadDEM30(string DEMpath)
        {
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Dataset dataSet = OSGeo.GDAL.Gdal.Open(DEMpath, OSGeo.GDAL.Access.GA_ReadOnly);
            if (dataSet == null)
            {
                Console.WriteLine("fail to open files!");
            }
            int xSize = dataSet.RasterXSize;
            int ySize = dataSet.RasterYSize;
            int[] data = new int[(xSize) * (ySize)];
            Band band1 = dataSet.GetRasterBand(1);
            band1.ReadRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            int[,] DEM = new int[ySize, xSize];
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    DEM[i, j] = data[i * (xSize) + j];
                }
            }
            return DEM;
        }
        //反照率数据读取
        static public float[,] ReadAlbedo(int xSize, int ySize, int ratio)
        {
            float[,] albedo = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    albedo[i, j] = (float)(0.2);
                }
            }
            return albedo;
        }

        //将30米的水平直接辐射数据转换到所在DEM尺度，并读取
        static public float[,] ReadDirRadition_H(string DirRadition_H_path, int ratio)
        {
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Dataset dataSet = OSGeo.GDAL.Gdal.Open(DirRadition_H_path, OSGeo.GDAL.Access.GA_ReadOnly);
            int xSize = dataSet.RasterXSize;
            int ySize = dataSet.RasterYSize;
            float[] data = new float[xSize * ySize];
            dataSet.GetRasterBand(1).ReadRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            float[,] data1 = new float[ySize, xSize];//一维数组转化为二维数组
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    data1[i, j] = data[i * xSize + j];
                }
            }
            float[,] DirRadition_H = new float[ySize / ratio, xSize / ratio];//DirRadition_H[]存储的是DEM尺度上的直接辐射，将原始直接辐射数据平均降尺度而来
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    float a = 0;
                    for (int n = 0; n < ratio; n++)
                    {
                        for (int m = 0; m < ratio; m++)
                        {
                            a = a + data1[i * ratio + n, j * ratio + m];
                        }
                    }
                    DirRadition_H[i, j] = a / (ratio * ratio);
                }
            }
            //Console.WriteLine("水平直接辐射 read over!");
            return DirRadition_H;
        }
        //将30米的水平散射辐射数据转换到所在DEM尺度上，并读取
        static public float[,] ReadDifRadition_H(string DifRadition_H_path, int ratio)
        {
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Dataset dataSet = OSGeo.GDAL.Gdal.Open(DifRadition_H_path, OSGeo.GDAL.Access.GA_ReadOnly);
            int xSize = dataSet.RasterXSize;
            int ySize = dataSet.RasterYSize;
            float[] data = new float[xSize * ySize];
            dataSet.GetRasterBand(1).ReadRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            float[,] data1 = new float[ySize, xSize];//一维数组转化为二维数组
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    data1[i, j] = data[i * xSize + j];
                }
            }
            float[,] DifRadition_H = new float[ySize / ratio, xSize / ratio];//DifRadition_H[]存储的是DEM尺度上的直接辐射，将原始直接辐射数据平均降尺度而来
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    float a = 0;
                    for (int n = 0; n < ratio; n++)
                    {
                        for (int m = 0; m < ratio; m++)
                        {
                            a = a + data1[i * ratio + n, j * ratio + m];
                        }
                    }
                    DifRadition_H[i, j] = a / (ratio * ratio);
                }
            }
            return DifRadition_H;
        }
        //数据输出TIF格式(float类型)
        static public void OutputToTif(string OutputPath, float[,] inData, double[] edGeoTrans, string projection)
        {
            int xSize = inData.GetLength(1);
            int ySize = inData.GetLength(0);
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Driver driver = OSGeo.GDAL.Gdal.GetDriverByName("GTiff");
            OSGeo.GDAL.Dataset Outdata = driver.Create(OutputPath, xSize, ySize, 1, DataType.GDT_Float32, null);
            Outdata.SetGeoTransform(edGeoTrans);
            Outdata.SetProjection(projection);
            float[] data = new float[ySize * xSize];
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    data[i * (xSize) + j] = inData[i, j];
                }
            }
            Outdata.GetRasterBand(1).WriteRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            Outdata.FlushCache();
        }
        //数据输出TIF格式(float类型)
        static public void OutputToTifInt(string OutputPath, int[,] inData, double[] edGeoTrans, string projection)
        {
            int xSize = inData.GetLength(1);
            int ySize = inData.GetLength(0);
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Driver driver = OSGeo.GDAL.Gdal.GetDriverByName("GTiff");
            OSGeo.GDAL.Dataset Outdata = driver.Create(OutputPath, xSize, ySize, 1, DataType.GDT_Int16, null);
            Outdata.SetGeoTransform(edGeoTrans);
            Outdata.SetProjection(projection);
            int[] data = new int[ySize * xSize];
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    data[i * (xSize) + j] = inData[i, j];
                }
            }
            Outdata.GetRasterBand(1).WriteRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            Outdata.FlushCache();
        }
        //数据输出成JPG格式
        static public void OutputToJpg(string tifpath, string jpgpath, byte[,] inData)
        {
            int xSize = inData.GetLength(1);
            int ySize = inData.GetLength(0);
            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.GDAL.Driver JPEGdriver = OSGeo.GDAL.Gdal.GetDriverByName("JPEG");
            OSGeo.GDAL.Driver MEMdriver = OSGeo.GDAL.Gdal.GetDriverByName("MEM");
            OSGeo.GDAL.Dataset MEM = MEMdriver.Create(tifpath, xSize, ySize, 1, DataType.GDT_Byte, null);


            byte[] data = new byte[ySize * xSize];
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    data[i * (xSize) + j] = inData[i, j];
                }
            }
            MEM.GetRasterBand(1).WriteRaster(0, 0, xSize, ySize, data, xSize, ySize, 0, 0);
            //MEN.FlushCache();
            JPEGdriver.CreateCopy(jpgpath, MEM, 1, null, null, null);
        }
    }
}
