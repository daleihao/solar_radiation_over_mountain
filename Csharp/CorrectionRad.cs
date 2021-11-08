using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地形校正
{
    class CorrectionRad
    {
        //校正直接辐射
        static public float[,] DirRadition_correction(int xSize, int ySize, int ratio, float[,] DirRadition_H, int[,] ShadeFactor, float[,] angle_of_incidence1, float[,] solar_altitude)
        {
            float[,] dirradition_t = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    if (angle_of_incidence1[i, j] < 90)
                        dirradition_t[i, j] = (float)(DirRadition_H[i, j] * Math.Cos(angle_of_incidence1[i, j] * Math.PI / 180) * ShadeFactor[i, j] / Math.Sin(solar_altitude[i, j] * Math.PI / 180));
                    else
                        dirradition_t[i, j] = 0;
                    //Console.WriteLine("shade:{0}, I:{1},  cosI:{2}", ShadeFactor[i, j], angle_of_incidence1[i, j], Math.Cos(angle_of_incidence1[i, j] * Math.PI / 180));
                }
            }

            return dirradition_t;
        }

        //校正散射辐射(Hay模型，各向异性校正)
        static public float[,] DifRadition_correction_Hay(int xSize, int ySize, int ratio, float[,] DirRadition_H, float[,] DifRadition_H, float[,] Vd, float[,] angle_of_incidence1, float[,] solar_altitude, int[,] shade)
        {
            float[,] DifRadition_t = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    float a, b, c;
                    a = (float)(DirRadition_H[i, j] / Math.Sin(solar_altitude[i, j] * Math.PI / 180));//太阳方向的直接辐射
                    //b为各向异性指数，表示环日各向异性散射占天空散射的权重，用地面法线方向接受的太阳辐射与水平面的总辐射（大气层顶辐射）之比计算
                    b = (float)(a / 1367);
                    c = (float)((1 - b) * Vd[i, j]);
                    if (angle_of_incidence1[i, j] < 90)
                    {
                        if (shade[i, j] == 1)
                        {
                            DifRadition_t[i, j] = DifRadition_H[i, j] * (b + c);
                        }
                        else
                        {
                            DifRadition_t[i, j] = DifRadition_H[i, j] * c;
                        }
                    }
                    else
                    {
                        DifRadition_t[i, j] = DifRadition_H[i, j] * c;
                    }
                    //Console.WriteLine("shade:{0}, I:{1},  cosI:{2}", ShadeFactor[i, j], angle_of_incidence1[i, j], Math.Cos(angle_of_incidence1[i, j] * Math.PI / 180));
                }
            }
            return DifRadition_t;
        }
        //校正散射辐射（各向同性校正）
        static public float[,] DifRadition_correction_isotropy(int xSize, int ySize, int ratio, float[,] DifRadition_H, float[,] vd)
        {
            float[,] DifRadition_t = new float[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    DifRadition_t[i, j] = DifRadition_H[i, j] * vd[i, j];
                }
            }
            return DifRadition_t;
        }
        //邻近附加辐射计算
        static public float CalculateNeiRad(int row, int col, int xSize, int ySize, int ratio, float[,] cosT1, float[,] cosT2, int[,] hid, float[,] DirRadition_T, float[,] DifRadition_T, float[,] slope, float[,] albedo, int step3, float cellsize)
        {
            float NeiRad = 0;
            for (int n = -step3; n <= step3; n++)
            {
                for (int m = -step3; m <= step3; m++)
                {
                    if (row + n >= 0 && col + m >= 0 && row + n <= ySize / ratio - 1 && col + m <= xSize / ratio - 1)
                    {
                        float area = (float)(cellsize * cellsize / Math.Cos(slope[row + n, col + m] * Math.PI / 180));
                        if (cosT1[n + step3, m + step3] < 0 || cosT2[n + step3, m + step3] < 0)
                        {
                            NeiRad = NeiRad + 0;
                        }
                        else if (n == 0 && m == 0)
                        { NeiRad = NeiRad + 0; }
                        else
                        {
                            NeiRad = NeiRad + (float)((DirRadition_T[row + n, col + m] + DifRadition_T[row + n, col + m]) * albedo[row + n, col + m] * hid[n + step3, m + step3] * cosT1[n + step3, m + step3] * cosT2[n + step3, m + step3] * area / (n * n * cellsize * cellsize + m * m * cellsize * cellsize));
                        }
                        //Console.WriteLine("dir={0},dif={1},hid={2},cost1={3},cost2={4},nei={5}", DirRadition_T[row + n, col + m], DifRadition_T[row + n, col + m], hid[n + step3, m + step3], cosT1[n + step3, m + step3], cosT2[n + step3, m + step3]);
                    }
                }
            }
            return NeiRad;
        }
    }
}
