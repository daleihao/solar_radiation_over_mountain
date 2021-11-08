using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地形校正
{
    class topo_factors
    {

        //计算太阳方位角
        static public float[,] solar_azimuth(int xSize, int ySize, int bj_year, int bj_month, int bj_day, int bj_hour, int bj_minute, double lon_LeftUp, double lat_LeftUp, double resolution)
        {
            float[,] solar_azi = new float[ySize, xSize];
            float[,] solar_alt = new float[ySize, xSize];
            double DOY, NO, TimeAngle, sitar, ture_solar_time, ED, Et;//TimeAngle为太阳时角,sitar为日角
            double lon, lat;
            int FebDays;
            if (bj_year % 4 == 0 && (bj_year % 100 != 0 || bj_year % 400 == 0))
            {
                FebDays = 29;
            }
            else
            {
                FebDays = 28;
            }
            DOY = bj_day;
            if (bj_month > 1) DOY = DOY + 31;
            if (bj_month > 2) DOY = DOY + FebDays;
            if (bj_month > 3) DOY = DOY + 31;
            if (bj_month > 4) DOY = DOY + 30;
            if (bj_month > 5) DOY = DOY + 31;
            if (bj_month > 6) DOY = DOY + 30;
            if (bj_month > 7) DOY = DOY + 31;
            if (bj_month > 8) DOY = DOY + 31;
            if (bj_month > 9) DOY = DOY + 30;
            if (bj_month > 10) DOY = DOY + 31;
            if (bj_month > 11) DOY = DOY + 30;
            NO = 79.6764 + 0.2422 * (bj_year - 1985) - (int)((bj_year - 1985) / 4.0);//N0为订正的年度
            sitar = 2 * Math.PI * (DOY - NO) / 365.2422;//sitar为日角(弧度)
            ED = 0.3723 + 23.2567 * Math.Sin(sitar) + 0.1149 * Math.Sin(2 * sitar) - 0.1712 * Math.Sin(3 * sitar) - 0.758 * Math.Cos(sitar) + 0.3656 * Math.Cos(2 * sitar) + 0.0201 * Math.Cos(3 * sitar);
            //ED赤纬角,度
            // Console.WriteLine("DOY={0}, NO={1},sitar={2},ED={3}", DOY, NO, sitar, ED);
            ED = ED * Math.PI / 180;
            Et = 0.0028 - 1.9857 * Math.Sin(sitar) + 9.9059 * Math.Sin(2 * sitar) - 7.0924 * Math.Cos(sitar) - 0.6882 * Math.Cos(2 * sitar);//Et为时差。单位为分
            for (int row = 0; row < ySize; row++)
            {
                for (int col = 0; col < xSize; col++)
                {
                    lon = lon_LeftUp + (0.5 + col) * resolution;
                    lat = lat_LeftUp - (0.5 + row) * resolution;
                    ture_solar_time = bj_hour + (bj_minute - (120 - lon) * 4) / 60.0 + Et / 60.0;//真太阳时
                    TimeAngle = (ture_solar_time - 12) * 15;
                    // Console.WriteLine("Et={0}, ture_solar_time={1},TimeAngle={2}", Et, ture_solar_time, TimeAngle);
                    lon = lon * Math.PI / 180;
                    lat = lat * Math.PI / 180;
                    TimeAngle = TimeAngle * Math.PI / 180;
                    solar_alt[row, col] = (float)Math.Asin(Math.Sin(lat) * Math.Sin(ED) + Math.Cos(lat) * Math.Cos(ED) * Math.Cos(TimeAngle));
                    //solar_alt = (float)(solar_alt * 180 / Math.PI);
                    solar_azi[row, col] = (float)(Math.Acos((Math.Sin(solar_alt[row, col]) * Math.Sin(lat) - Math.Sin(ED)) / (Math.Cos(solar_alt[row, col]) * Math.Cos(lat))));
                    solar_azi[row, col] = (float)(solar_azi[row, col] * 180 / Math.PI);
                    if (TimeAngle > 0)
                    { solar_azi[row, col] = 180 + solar_azi[row, col]; }
                    else if (TimeAngle == 0)
                    { solar_azi[row, col] = 180; }
                    else
                    { solar_azi[row, col] = 180 - solar_azi[row, col]; }
                    //Console.WriteLine("bj_hour={0},solar_azi={1},solar_alt={2}", bj_hour, solar_azi[row,col], (float)(solar_alt[row,col] * 180 / Math.PI));
                    if (float.IsNaN(solar_azi[row, col]))
                    { solar_azi[row, col] = 180; }
                }
            }
            return solar_azi;
        }
        //计算太阳高度角
        static public float[,] solar_altitude(int xSize, int ySize, int bj_year, int bj_month, int bj_day, int bj_hour, int bj_minute, double lon_LeftUp, double lat_LeftUp, double resolution)
        {
            float[,] solar_alt = new float[ySize, xSize];
            double DOY, NO, TimeAngle, sitar, ture_solar_time, ED, Et;//TimeAngle为太阳时角,sitar为日角
            double lon, lat;
            int FebDays;
            if (bj_year % 4 == 0 && (bj_year % 100 != 0 || bj_year % 400 == 0))
            {
                FebDays = 29;
            }
            else
            {
                FebDays = 28;
            }
            DOY = bj_day;
            if (bj_month > 1) DOY = DOY + 31;
            if (bj_month > 2) DOY = DOY + FebDays;
            if (bj_month > 3) DOY = DOY + 31;
            if (bj_month > 4) DOY = DOY + 30;
            if (bj_month > 5) DOY = DOY + 31;
            if (bj_month > 6) DOY = DOY + 30;
            if (bj_month > 7) DOY = DOY + 31;
            if (bj_month > 8) DOY = DOY + 31;
            if (bj_month > 9) DOY = DOY + 30;
            if (bj_month > 10) DOY = DOY + 31;
            if (bj_month > 11) DOY = DOY + 30;
            //*************
            NO = 79.6764 + 0.2422 * (bj_year - 1985) - (int)((bj_year - 1985) / 4.0);//N0为订正的年度
            sitar = 2 * Math.PI * (DOY - NO) / 365.2422;//sitar为日角(弧度)
            ED = 0.3723 + 23.2567 * Math.Sin(sitar) + 0.1149 * Math.Sin(2 * sitar) - 0.1712 * Math.Sin(3 * sitar) - 0.758 * Math.Cos(sitar) + 0.3656 * Math.Cos(2 * sitar) + 0.0201 * Math.Cos(3 * sitar);
            //ED赤纬角,度
            // Console.WriteLine("DOY={0}, NO={1},sitar={2},ED={3}", DOY, NO, sitar, ED);
            ED = ED * Math.PI / 180;
            Et = 0.0028 - 1.9857 * Math.Sin(sitar) + 9.9059 * Math.Sin(2 * sitar) - 7.0924 * Math.Cos(sitar) - 0.6882 * Math.Cos(2 * sitar);//Et为时差。单位为分
            for (int row = 0; row < ySize; row++)
            {
                for (int col = 0; col < xSize; col++)
                {
                    lon = lon_LeftUp + (0.5 + col) * resolution;
                    lat = lat_LeftUp - (0.5 + row) * resolution;
                    ture_solar_time = bj_hour + (bj_minute - (120 - lon) * 4) / 60.0 + Et / 60.0;//真太阳时
                    TimeAngle = (ture_solar_time - 12) * 15;
                    // Console.WriteLine("Et={0}, ture_solar_time={1},TimeAngle={2}", Et, ture_solar_time, TimeAngle);
                    lon = lon * Math.PI / 180;
                    lat = lat * Math.PI / 180;
                    TimeAngle = TimeAngle * Math.PI / 180;
                    solar_alt[row, col] = (float)Math.Asin(Math.Sin(lat) * Math.Sin(ED) + Math.Cos(lat) * Math.Cos(ED) * Math.Cos(TimeAngle));
                    solar_alt[row, col] = (float)(solar_alt[row, col] * 180 / Math.PI);
                }
            }
            return solar_alt;
        }
        //太阳与目标像间见的遮蔽因子     
        static public int[,] shading(int xSize, int ySize, int ratio, float[,] solar_altitude, float[,] solar_azimuth, int[,] DEM, int step1, float cellsize)
        {
            int[,] shade = new int[ySize / ratio, xSize / ratio];
            float max_h;

            for (int row = 0; row < ySize / ratio; row++)
            {
                for (int col = 0; col < xSize / ratio; col++)
                {
                    for (int i = 1; i <= step1; i++)
                    {
                        max_h = (float)(i * cellsize * Math.Tan(solar_altitude[row, col] * Math.PI / 180));
                        if (max_h < 0)
                        {
                            shade[row, col] = 0;//遮蔽
                            break;
                        }
                        else
                        {
                            float x, y;
                            y = (float)Math.Abs(i * cellsize * Math.Cos(solar_azimuth[row, col] * Math.PI / 180));// 南北方向搜索的长度(米)
                            y = (float)Math.Abs(y / cellsize); // 转化为经度方向的栅格数
                            if ((solar_azimuth[row, col] > 90) && (solar_azimuth[row, col] < 270))//
                            {
                                y = (float)(row + y + 0.5);
                            }
                            else
                            {
                                y = (float)(row - y - 0.5);
                            }
                            if (y < 0)
                            { y = 0; }
                            if (y > ySize / ratio - 1)
                            { y = ySize / ratio - 1; }

                            x = (float)Math.Abs(i * cellsize * Math.Sin(solar_azimuth[row, col] * Math.PI / 180));// 东西方向搜索的长度(米)
                            x = (float)Math.Abs(x / cellsize); // 转化为经度方向的栅格数
                            if (solar_azimuth[row, col] < 180)
                            {
                                x = (float)(col + x + 0.5);
                            }
                            else
                            {
                                x = (float)(col - x - 0.5);
                            }
                            if (x < 0)
                            {
                                x = 0;
                            }
                            if (x > xSize / ratio - 1)
                            {
                                x = xSize / ratio - 1;
                            }
                            if ((DEM[(int)(y), (int)(x)] - DEM[row, col]) / (i * cellsize) < Math.Tan(solar_altitude[row, col] * Math.PI / 180))
                            {
                                shade[row, col] = 1;
                            } //no shading  
                            else
                            {
                                shade[row, col] = 0;
                                break;
                            }
                        }
                    }
                    // Console.WriteLine("row:{0}, col:{1}, shadefactor:{2}", row, col, shade[row, col]);
                }
            }

            return shade;
        }
        //计算太阳光线与坡面法线夹角（单位为度）
        static public float[,] Angle_of_Incidence(int xSize, int ySize, int ratio, float[,] solar_altitude, float[,] solar_azimuth, float[,] slope, float[,] aspect)
        {
            float[,] I = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    double a, b, c;
                    a = Math.Sin(solar_altitude[i, j] * Math.PI / 180) * Math.Cos(slope[i, j] * Math.PI / 180);
                    b = Math.Cos(solar_altitude[i, j] * Math.PI / 180) * Math.Sin(slope[i, j] * Math.PI / 180);
                    c = Math.Cos(solar_azimuth[i, j] * Math.PI / 180 - aspect[i, j] * Math.PI / 180);
                    I[i, j] = (float)(Math.Acos(a + b * c) * 180 / Math.PI);
                    //Console.WriteLine("I:{0}", I[i, j]);
                }
            }
            return I;
        }
        //计算各向同性环日可见因子
        static public float[,] VD(int xSize, int ySize, int ratio, int[,] DEM, float cellsize, int step2)
        {
            float[,] Vd = new float[ySize / ratio, xSize / ratio];
            for (int row = 0; row < ySize / ratio; row++)
            {
                for (int col = 0; col < xSize / ratio; col++)
                {
                    float sinH1 = -1, sinH2 = -1, sinH3 = -1, sinH4 = -1;
                    float sinH5 = -1, sinH6 = -1, sinH7 = -1, sinH8 = -1;
                    float max1, max2, max3, max4, max5, max6, max7, max8;
                    for (int k = 1; k <= step2; k++)
                    {
                        //计算正东方向的最大遮蔽角
                        if (col + k > xSize / ratio - 1)
                        {
                            max1 = 0;
                        }
                        else
                        {
                            max1 = (float)((DEM[row, col + k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row, col + k] - DEM[row, col]) * (DEM[row, col + k] - DEM[row, col])));
                        }
                        if (max1 > sinH1)
                        {
                            sinH1 = max1;
                        }
                        //计算东南方向的最大遮蔽角
                        if (col + k > xSize / ratio - 1 || row + k > ySize / ratio - 1)
                        {
                            max2 = 0;
                        }
                        else
                        {
                            max2 = (float)((DEM[row + k, col + k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row + k, col + k] - DEM[row, col]) * (DEM[row + k, col + k] - DEM[row, col])));
                        }
                        if (max2 > sinH2)
                        {
                            sinH2 = max2;
                        }
                        //计算正南方向的最大遮蔽角
                        if (row + k > ySize / ratio - 1)
                        {
                            max3 = 0;
                        }
                        else
                        {
                            max3 = (float)((DEM[row + k, col] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row + k, col] - DEM[row, col]) * (DEM[row + k, col] - DEM[row, col])));
                        }
                        if (max3 > sinH3)
                        {
                            sinH3 = max3;
                        }
                        //计算西南方向的最大遮蔽角
                        if (col - k < 0 || row + k > ySize / ratio - 1)
                        {
                            max4 = 0;
                        }
                        else
                        {
                            max4 = (float)((DEM[row + k, col - k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row + k, col - k] - DEM[row, col]) * (DEM[row + k, col - k] - DEM[row, col])));
                        }
                        if (max4 > sinH4)
                        {
                            sinH4 = max4;
                        }
                        //计算正西方向的最大遮蔽角
                        if (col - k < 0)
                        {
                            max5 = 0;
                        }
                        else
                        {
                            max5 = (float)((DEM[row, col - k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row, col - k] - DEM[row, col]) * (DEM[row, col - k] - DEM[row, col])));
                        }
                        if (max5 > sinH5)
                        {
                            sinH5 = max5;
                        }
                        //计算西北方向的最大遮蔽角
                        if (col - k < 0 || row - k < 0)
                        {
                            max6 = 0;
                        }
                        else
                        {
                            max6 = (float)((DEM[row - k, col - k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row - k, col - k] - DEM[row, col]) * (DEM[row - k, col - k] - DEM[row, col])));
                        }
                        if (max6 > sinH6)
                        {
                            sinH6 = max6;
                        }
                        //计算正北方向的最大遮蔽角
                        if (row - k < 0)
                        {
                            max7 = 0;
                        }
                        else
                        {
                            max7 = (float)((DEM[row - k, col] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row - k, col] - DEM[row, col]) * (DEM[row - k, col] - DEM[row, col])));
                        }
                        if (max7 > sinH7)
                        {
                            sinH7 = max7;
                        }
                        //计算东北方向的最大遮蔽角
                        if (col + k > xSize / ratio - 1 || row - k < 0)
                        {
                            max8 = 0;
                        }
                        else
                        {
                            max8 = (float)((DEM[row - k, col + k] - DEM[row, col]) / Math.Sqrt(cellsize * cellsize * k * k + (DEM[row - k, col + k] - DEM[row, col]) * (DEM[row - k, col + k] - DEM[row, col])));
                        }
                        if (max8 > sinH8)
                        {
                            sinH8 = max8;
                        }
                    }
                    //Console.WriteLine("东={0},东南={1},南={2},西南={3},西={4},西北={5},北={6},东北={7}", sinH1, sinH2, sinH3, sinH4, sinH5, sinH6, sinH7, sinH8);
                    Vd[row, col] = (8 - sinH1 - sinH2 - sinH3 - sinH4 - sinH5 - sinH6 - sinH7 - sinH8) / 8;
                    if (Vd[row, col] > 1)
                    { Vd[row, col] = 1; }
                    //Console.WriteLine("Vd={0}", Vd);
                }
            }
            return Vd;
        }
        //函数意义：以行列号为【row,col】的像元为中心，判断其周边周边（2 * step + 1）乘（2 * step + 1）的像元窗口的遮蔽情况
        static public int[,] HID_judge(int row, int col, int xSize, int ySize, int ratio, int[,] DEM, int step3)
        {
            int[,] hid = new int[2 * step3 + 1, 2 * step3 + 1];//HID=0为遮蔽；
            for (int n = -step3; n <= step3; n++)
            {
                for (int m = -step3; m <= step3; m++)
                {
                    if (row + n < 0 || row + n > ySize / ratio - 1 || col + m < 0 || col + m > xSize / ratio - 1)//超出数组行数范围则返回
                    { hid[n + step3, m + step3] = 0; }
                    else if (n >= -1 && n <= 1 && m >= -1 && m <= 1 && m != 0 && n != 0)
                    { hid[n + step3, m + step3] = 1; }
                    else if (m == 0 && n == 0)
                    { hid[n + step3, m + step3] = 0; }//遮蔽 
                    else
                    {
                        for (int k = 1; k <= (int)(Math.Sqrt(n * n + m * m)); k++)
                        {
                            int x, y;
                            if (n < 0)//判断在（2 * step + 1）乘（2 * step + 1）的像元窗口内部，所遍历的像元所处的行号
                            { y = row - (int)(Math.Abs(k * n / Math.Sqrt(m * m + n * n))); }
                            else if (n > 0)
                            { y = row + (int)(Math.Abs(k * n / Math.Sqrt(m * m + n * n))); }
                            else
                            { y = row; }
                            if (y < 0)
                            { y = 0; }
                            if (y > ySize / ratio - 1)
                            { y = ySize / ratio - 1; }

                            if (m < 0)//判断在（2 * step + 1）乘（2 * step + 1）的像元窗口内部，所遍历的像元所处的列号
                            { x = col - (int)(Math.Abs(k * m / Math.Sqrt(m * m + n * n))); }
                            else if (m > 0)
                            { x = col + (int)(Math.Abs(k * m / Math.Sqrt(m * m + n * n))); }
                            else
                            { x = col; }
                            if (x < 0)
                            { x = 0; }
                            if (x > xSize / ratio - 1)
                            { x = xSize / ratio - 1; }
                            if (k * (DEM[row + n, col + m] - DEM[row, col]) >= (int)(Math.Sqrt(m * m + n * n)) * (DEM[y, x] - DEM[row, col]))
                            {
                                hid[n + step3, m + step3] = 1;//不遮蔽
                                // Console.WriteLine("row={0},col={1},n={2},m={3},hid[{4},{5}]={6}", row, col, n, m, n + step, m + step, hid[n + step, m + step]);
                            }
                            else
                            {
                                hid[n + step3, m + step3] = 0;//遮蔽
                                // Console.WriteLine("row={0},col={1},n={2},m={3},hid[{4},{5}]={6}", row, col, n, m, n + step, m + step, hid[n + step, m + step]);
                                break;
                            }
                        }
                    }
                    //Console.WriteLine("row={0},col={1},n={2},m={3},hid[{4},{5}]={6}", row, col, n, m, n + step, m + step, hid[n + step, m +step]);
                }
            }
            return hid;
        }
        //朝向判断，计算目标像元坡面法线与邻近像元连线的夹角
        static public float[,] T1(int row, int col, int xSize, int ySize, int ratio, int[,] DEM, int step3, float cellsize)
        {
            float[,] cosT1 = new float[2 * step3 + 1, 2 * step3 + 1];
            float tanX1, tanY1;//tanX1和tanY1为目标像元在x(西至东)与y（南至北）方向的坡度角，tanX2和tanY2为邻近像元在x与y方向的坡度角
            if (row == 0 && col == 0)
            {
                tanX1 = (DEM[row, col] - DEM[row, col + 1] + DEM[row + 1, col] - DEM[row + 1, col + 1] + 2 * (DEM[row, col] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col] - DEM[row, col] + DEM[row + 1, col + 1] - DEM[row, col + 1] + 2 * (DEM[row + 1, col] - DEM[row, col])) / (8 * cellsize);
            }
            else if (row == 0 && col >= 1 && col <= xSize / ratio - 2)
            {
                tanX1 = (DEM[row, col - 1] - DEM[row, col + 1] + DEM[row + 1, col - 1] - DEM[row + 1, col + 1] + 2 * (DEM[row, col - 1] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col - 1] - DEM[row, col - 1] + DEM[row + 1, col + 1] - DEM[row, col + 1] + 2 * (DEM[row + 1, col] - DEM[row, col])) / (8 * cellsize);
            }
            else if (row == 0 && col == xSize / ratio - 1)
            {
                tanX1 = (DEM[row, col - 1] - DEM[row, col] + DEM[row + 1, col - 1] - DEM[row + 1, col] + 2 * (DEM[row, col - 1] - DEM[row, col])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col - 1] - DEM[row, col - 1] + DEM[row + 1, col] - DEM[row, col] + 2 * (DEM[row + 1, col] - DEM[row, col])) / (8 * cellsize);
            }
            else if (row >= 1 && row <= ySize / ratio - 2 && col == 0)
            {
                tanX1 = (DEM[row - 1, col] - DEM[row - 1, col + 1] + DEM[row + 1, col] - DEM[row + 1, col + 1] + 2 * (DEM[row, col] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col] - DEM[row - 1, col] + DEM[row + 1, col + 1] - DEM[row - 1, col + 1] + 2 * (DEM[row + 1, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            else if (row >= 1 && row <= ySize / ratio - 2 && col == xSize / ratio - 1)
            {
                tanX1 = (DEM[row - 1, col - 1] - DEM[row - 1, col] + DEM[row + 1, col - 1] - DEM[row + 1, col] + 2 * (DEM[row, col - 1] - DEM[row, col])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col - 1] - DEM[row - 1, col - 1] + DEM[row + 1, col] - DEM[row - 1, col] + 2 * (DEM[row + 1, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            else if (row == ySize / ratio - 1 && col == 0)
            {
                tanX1 = (DEM[row - 1, col] - DEM[row - 1, col + 1] + DEM[row, col] - DEM[row, col + 1] + 2 * (DEM[row, col] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row, col] - DEM[row - 1, col] + DEM[row, col + 1] - DEM[row - 1, col + 1] + 2 * (DEM[row, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            else if (row == ySize / ratio - 1 && col >= 1 && col <= xSize / ratio - 2)
            {
                tanX1 = (DEM[row - 1, col - 1] - DEM[row - 1, col + 1] + DEM[row, col - 1] - DEM[row, col + 1] + 2 * (DEM[row, col - 1] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row, col - 1] - DEM[row - 1, col - 1] + DEM[row, col + 1] - DEM[row - 1, col + 1] + 2 * (DEM[row, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            else if (row == ySize / ratio - 1 && col == xSize / ratio - 1)
            {
                tanX1 = (DEM[row - 1, col - 1] - DEM[row - 1, col] + DEM[row, col - 1] - DEM[row, col] + 2 * (DEM[row, col - 1] - DEM[row, col])) / (8 * cellsize);
                tanY1 = (DEM[row, col - 1] - DEM[row - 1, col - 1] + DEM[row, col] - DEM[row - 1, col] + 2 * (DEM[row, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            else
            {
                tanX1 = (DEM[row - 1, col - 1] - DEM[row - 1, col + 1] + DEM[row + 1, col - 1] - DEM[row + 1, col + 1] + 2 * (DEM[row, col - 1] - DEM[row, col + 1])) / (8 * cellsize);
                tanY1 = (DEM[row + 1, col - 1] - DEM[row - 1, col - 1] + DEM[row + 1, col + 1] - DEM[row - 1, col + 1] + 2 * (DEM[row + 1, col] - DEM[row - 1, col])) / (8 * cellsize);
            }
            for (int n = -step3; n <= step3; n++)
            {
                for (int m = -step3; m <= step3; m++)
                {
                    if (row + n < 0 || col + m < 0)
                    {
                        cosT1[n + step3, m + step3] = 0;
                    }
                    else if (row + n > ySize / ratio - 1 || col + m > xSize / ratio - 1)
                    {
                        cosT1[n + step3, m + step3] = 0;
                    }
                    else
                    {
                        float a, b1, b2, b3, b4;
                        a = tanX1 * m * cellsize + tanY1 * n * cellsize + DEM[row + n, col + m] - DEM[row, col];
                        b1 = tanX1 * tanX1 + tanY1 * tanY1 + 1;
                        b2 = cellsize * cellsize * m * m;
                        b3 = cellsize * cellsize * n * n;
                        b4 = (DEM[row + n, col + m] - DEM[row, col]) * (DEM[row + n, col + m] - DEM[row, col]);
                        cosT1[n + step3, m + step3] = (float)(a / Math.Sqrt(b1 * (b2 + b3 + b4)));
                    }
                    if (n == 0 && m == 0)
                    {
                        cosT1[n + step3, m + step3] = 0;
                    }
                    //Console.WriteLine("row={0},col={1},n={2},m={3},cosT1[{4},{5}]={6}", row, col, n, m, n +step3, m +step3, cosT1[n +step3, m +step3]);
                }
            }
            return cosT1;
        }
        //朝向判断，计算邻近像元坡面法线与邻近像元连线的夹角
        static public float[,] T2(int row, int col, int xSize, int ySize, int ratio, int[,] DEM, int step3, float cellsize)
        {
            float[,] cosT2 = new float[2 * step3 + 1, 2 * step3 + 1];
            float tanX2, tanY2;//tanX1和tanY1为目标像元在x(西至东)与y（南至北）方向的坡度角，tanX2和tanY2为邻近像元在x与y方向的坡度角
            for (int n = -step3; n <= step3; n++)
            {
                for (int m = -step3; m <= step3; m++)
                {
                    if (row + n < 0 || col + m < 0)
                    {
                        cosT2[n + step3, m + step3] = 0;
                    }
                    else if (row + n > ySize / ratio - 1 || col + m > xSize / ratio - 1)
                    {
                        cosT2[n + step3, m + step3] = 0;
                    }
                    else
                    {
                        if (row + n == 0 && col + m == 0)
                        {
                            tanX2 = (DEM[row + n, col + m] - DEM[row + n, col + m + 1] + DEM[row + n + 1, col + m] - DEM[row + n + 1, col + m + 1] + 2 * (DEM[row + n, col + m] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m] - DEM[row + n, col + m] + DEM[row + n + 1, col + m + 1] - DEM[row + n, col + m + 1] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n, col + m])) / (8 * cellsize);
                        }
                        else if (row + n == 0 && col + m >= 1 && col + m <= xSize / ratio - 2)
                        {
                            tanX2 = (DEM[row + n, col + m - 1] - DEM[row + n, col + m + 1] + DEM[row + n + 1, col + m - 1] - DEM[row + n + 1, col + m + 1] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m - 1] - DEM[row + n, col + m - 1] + DEM[row + n + 1, col + m + 1] - DEM[row + n, col + m + 1] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n, col + m])) / (8 * cellsize);
                        }
                        else if (row + n == 0 && col + m == xSize / ratio - 1)
                        {
                            tanX2 = (DEM[row + n, col + m - 1] - DEM[row + n, col + m] + DEM[row + n + 1, col + m - 1] - DEM[row + n + 1, col + m] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m - 1] - DEM[row + n, col + m - 1] + DEM[row + n + 1, col + m] - DEM[row + n, col + m] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n, col + m])) / (8 * cellsize);
                        }
                        else if (row + n >= 1 && row + n <= ySize / ratio - 2 && col + m == 0)
                        {
                            tanX2 = (DEM[row + n - 1, col + m] - DEM[row + n - 1, col + m + 1] + DEM[row + n + 1, col + m] - DEM[row + n + 1, col + m + 1] + 2 * (DEM[row + n, col + m] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m] - DEM[row + n - 1, col + m] + DEM[row + n + 1, col + m + 1] - DEM[row + n - 1, col + m + 1] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }
                        else if (row + n >= 1 && row + n <= ySize / ratio - 2 && col + m == xSize / ratio - 1)
                        {
                            tanX2 = (DEM[row + n - 1, col + m - 1] - DEM[row + n - 1, col + m] + DEM[row + n + 1, col + m - 1] - DEM[row + n + 1, col + m] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m - 1] - DEM[row + n - 1, col + m - 1] + DEM[row + n + 1, col + m] - DEM[row + n - 1, col + m] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }
                        else if (row + n == ySize / ratio - 1 && col + m == 0)
                        {
                            tanX2 = (DEM[row + n - 1, col + m] - DEM[row + n - 1, col + m + 1] + DEM[row + n, col + m] - DEM[row + n, col + m + 1] + 2 * (DEM[row + n, col + m] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n, col + m] - DEM[row + n - 1, col + m] + DEM[row + n, col + m + 1] - DEM[row + n - 1, col + m + 1] + 2 * (DEM[row + n, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }
                        else if (row + n == ySize / ratio - 1 && col + m >= 1 && col + m <= xSize / ratio - 2)
                        {
                            tanX2 = (DEM[row + n - 1, col + m - 1] - DEM[row + n - 1, col + m + 1] + DEM[row + n, col + m - 1] - DEM[row + n, col + m + 1] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n, col + m - 1] - DEM[row + n - 1, col + m - 1] + DEM[row + n, col + m + 1] - DEM[row + n - 1, col + m + 1] + 2 * (DEM[row + n, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }
                        else if (row + n == ySize / ratio - 1 && col + m == xSize / ratio - 1)
                        {
                            tanX2 = (DEM[row + n - 1, col + m - 1] - DEM[row + n - 1, col + m] + DEM[row + n, col + m - 1] - DEM[row + n, col + m] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m])) / (8 * cellsize);
                            tanY2 = (DEM[row + n, col + m - 1] - DEM[row + n - 1, col + m - 1] + DEM[row + n, col + m] - DEM[row + n - 1, col + m] + 2 * (DEM[row + n, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }
                        else
                        {
                            tanX2 = (DEM[row + n - 1, col + m - 1] - DEM[row + n - 1, col + m + 1] + DEM[row + n + 1, col + m - 1] - DEM[row + n + 1, col + m + 1] + 2 * (DEM[row + n, col + m - 1] - DEM[row + n, col + m + 1])) / (8 * cellsize);
                            tanY2 = (DEM[row + n + 1, col + m - 1] - DEM[row + n - 1, col + m - 1] + DEM[row + n + 1, col + m + 1] - DEM[row + n - 1, col + m + 1] + 2 * (DEM[row + n + 1, col + m] - DEM[row + n - 1, col + m])) / (8 * cellsize);
                        }

                        float a, b1, b2, b3, b4;
                        a = tanX2 * (-m) * cellsize + tanY2 * (-n) * cellsize + DEM[row, col] - DEM[row + n, col + m];
                        b1 = tanX2 * tanX2 + tanY2 * tanY2 + 1;
                        b2 = cellsize * cellsize * m * m;
                        b3 = cellsize * cellsize * n * n;
                        b4 = (DEM[row, col] - DEM[row + n, col + m]) * (DEM[row, col] - DEM[row + n, col + m]);
                        cosT2[n + step3, m + step3] = (float)(a / Math.Sqrt(b1 * (b2 + b3 + b4)));
                        if (n == 0 && m == 0)
                        {
                            cosT2[n + step3, m + step3] = 0;
                        }
                    }
                    //Console.WriteLine("row={0},col={1},n={2},m={3},cosT1[{4},{5}]={6}", row, col, n, m, n +step3, m +step3, cosT2[n +step3, m +step3]);
                }
            }
            return cosT2;
        }

        //计算坡度
        static public float[,] Cal_slope(int xSize, int ySize, int ratio, int[,] DEM, float cellsize)
        {
            float[,] slope = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {

                    float fx, fy;
                    if (i == 0 && j == 0)
                    {
                        fx = (DEM[i, j + 1] - DEM[i, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i + 1, j + 1] - DEM[i + 1, j]) / (8 * cellsize);//东西方向高程变化率
                        fy = (DEM[i, j] - DEM[i + 1, j] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);//南北方向高程变化率
                    }
                    else if (i == 0 && j >= 1 && j <= xSize / ratio - 2)
                    {
                        fx = (DEM[i, j + 1] - DEM[i, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i + 1, j + 1] - DEM[i + 1, j - 1]) / (8 * cellsize);//东西方向高程变化率
                        fy = (DEM[i, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);//南北方向高程变化率
                    }
                    else if (i == 0 && j == xSize / ratio - 1)
                    {
                        fx = (DEM[i, j] - DEM[i, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i + 1, j] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = (DEM[i, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j] - DEM[i + 1, j]) / (8 * cellsize);
                    }
                    else if (j == 0 && i >= 1 && i <= ySize / ratio - 2)//********************
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i + 1, j + 1] - DEM[i + 1, j]) / (8 * cellsize);
                        fy = (DEM[i - 1, j] - DEM[i + 1, j] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);
                    }
                    else if (j == xSize / ratio - 1 && i >= 1 && i <= ySize / ratio - 2)
                    {
                        fx = (DEM[i - 1, j] - DEM[i - 1, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i + 1, j] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = (DEM[i - 1, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j] - DEM[i + 1, j]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j == 0)
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i, j + 1] - DEM[i, j]) / (8 * cellsize);
                        fy = (DEM[i - 1, j] - DEM[i, j] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j + 1] - DEM[i, j + 1]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j >= 1 && j <= xSize / ratio - 2)
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i, j + 1] - DEM[i, j - 1]) / (8 * cellsize);
                        fy = (DEM[i - 1, j - 1] - DEM[i, j - 1] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j + 1] - DEM[i, j + 1]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j == xSize / ratio - 1)
                    {
                        fx = (DEM[i - 1, j] - DEM[i - 1, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i, j] - DEM[i, j - 1]) / (8 * cellsize);
                        fy = (DEM[i - 1, j - 1] - DEM[i, j - 1] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j] - DEM[i, j]) / (8 * cellsize);
                    }
                    else
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i + 1, j + 1] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = (DEM[i - 1, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);
                    }
                    slope[i, j] = (float)(Math.Atan(Math.Sqrt(fx * fx + fy * fy)) * 180 / Math.PI);
                }
            }
            return slope;
        }
        //计算坡向
        static public float[,] Cal_aspect(int xSize, int ySize, int ratio, int[,] DEM, float cellsize)
        {
            float[,] aspect = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {

                    float fx, fy;//fx东减西，fy南减北
                    if (i == 0 && j == 0)
                    {
                        fx = (DEM[i, j + 1] - DEM[i, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i + 1, j + 1] - DEM[i + 1, j]) / (8 * cellsize);//东西方向高程变化率
                        fy = -(DEM[i, j] - DEM[i + 1, j] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);//南北方向高程变化率
                    }
                    else if (i == 0 && j >= 1 && j <= xSize / ratio - 2)
                    {
                        fx = (DEM[i, j + 1] - DEM[i, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i + 1, j + 1] - DEM[i + 1, j - 1]) / (8 * cellsize);//东西方向高程变化率
                        fy = -(DEM[i, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);//南北方向高程变化率
                    }
                    else if (i == 0 && j == xSize / ratio - 1)
                    {
                        fx = (DEM[i, j] - DEM[i, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i + 1, j] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = -(DEM[i, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i, j] - DEM[i + 1, j]) + DEM[i, j] - DEM[i + 1, j]) / (8 * cellsize);
                    }
                    else if (j == 0 && i >= 1 && i <= ySize / ratio - 2)//********************
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i + 1, j + 1] - DEM[i + 1, j]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j] - DEM[i + 1, j] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);
                    }
                    else if (j == xSize / ratio - 1 && i >= 1 && i <= ySize / ratio - 2)
                    {
                        fx = (DEM[i - 1, j] - DEM[i - 1, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i + 1, j] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j] - DEM[i + 1, j]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j == 0)
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j] + 2 * (DEM[i, j + 1] - DEM[i, j]) + DEM[i, j + 1] - DEM[i, j]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j] - DEM[i, j] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j + 1] - DEM[i, j + 1]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j >= 1 && j <= xSize / ratio - 2)
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i, j + 1] - DEM[i, j - 1]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j - 1] - DEM[i, j - 1] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j + 1] - DEM[i, j + 1]) / (8 * cellsize);
                    }
                    else if (i == ySize / ratio - 1 && j == xSize / ratio - 1)
                    {
                        fx = (DEM[i - 1, j] - DEM[i - 1, j - 1] + 2 * (DEM[i, j] - DEM[i, j - 1]) + DEM[i, j] - DEM[i, j - 1]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j - 1] - DEM[i, j - 1] + 2 * (DEM[i - 1, j] - DEM[i, j]) + DEM[i - 1, j] - DEM[i, j]) / (8 * cellsize);
                    }
                    else
                    {
                        fx = (DEM[i - 1, j + 1] - DEM[i - 1, j - 1] + 2 * (DEM[i, j + 1] - DEM[i, j - 1]) + DEM[i + 1, j + 1] - DEM[i + 1, j - 1]) / (8 * cellsize);
                        fy = -(DEM[i - 1, j - 1] - DEM[i + 1, j - 1] + 2 * (DEM[i - 1, j] - DEM[i + 1, j]) + DEM[i - 1, j + 1] - DEM[i + 1, j + 1]) / (8 * cellsize);
                    }
                    //aspect[i, j] = (float)(270 + Math.Atan(fx / fy) * 180 / Math.PI - 90 * fy / Math.Abs(fy));
                    //aspect[i, j] = (float)(180 - Math.Atan(fx / fy) * 180 / Math.PI + 90 * fy / Math.Abs(fy)); 
                    aspect[i, j] = (float)(57.29578 * Math.Atan2(fy, -fx));//用这个
                    if (aspect[i, j] < 0)
                    { aspect[i, j] = 90 - aspect[i, j]; }
                    else if (aspect[i, j] > 90)
                    { aspect[i, j] = 450 - aspect[i, j]; }
                    else
                    { aspect[i, j] = 90 - aspect[i, j]; }
                }
            }
            return aspect;
        }
    }
}
