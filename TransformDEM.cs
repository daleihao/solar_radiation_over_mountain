using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地形校正
{
    class TransformDEM
    {

        //用简单平均法将原始DEM（30米）转换到30×ratio尺度上
        static public int[,] AverageDEM(int xSize, int ySize, int ratio, int[,] InitialDEM)
        {
            int[,] IntermediateDEM_Ave = new int[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < xSize / ratio; j++)
                {
                    int num = 0;
                    for (int g = 0; g < ratio; g++)
                    {
                        for (int h = 0; h < ratio; h++)
                        {
                            num = num + InitialDEM[i * ratio + g, j * ratio + h];
                        }
                    }
                    IntermediateDEM_Ave[i, j] = (int)((num / (ratio * ratio)) + 0.5);
                }
            }
            return IntermediateDEM_Ave;
        }
        //以点扩散法将原始DEM（30米）转换到所需尺度上
        //计算N×N窗口的点扩散函数，并将其归一化
        static public int[,] PSF_NxN(int xSize, int ySize, int ratio, int[,] InitialDEM)
        {
            int ratio1 = (ratio - 1) / 2;
            double[,] PSF = new double[ratio, ratio];
            double sum = 0;
            for (int i = -ratio1; i <= ratio1; i++)
            {
                for (int j = -ratio1; j <= ratio1; j++)
                {
                    PSF[i + ratio1, j + ratio1] = Math.Exp(-1.0 * (i * i + j * j) / (2 * ratio1 * ratio1));//计算某窗口的点扩散函数
                    //Console.Write("{0}     ", PSF[i + ratio1, j + ratio1]);
                }
                // Console.Write("\n");
            }
            double[,] PSF_normalization = new double[ratio, ratio];
            double max = -1, min = 100;
            for (int i = 0; i < ratio; i++)
            {
                for (int j = 0; j < ratio; j++)
                {
                    if (max < PSF[i, j])
                    { max = PSF[i, j]; }
                    if (min > PSF[i, j])
                    { min = PSF[i, j]; }
                }
            }
            // Console.WriteLine("max={0},min={1}", max, min);
            for (int i = 0; i < ratio; i++)
            {
                for (int j = 0; j < ratio; j++)
                {
                    PSF_normalization[i, j] = (PSF[i, j] - min) / (max - min);//归一化点扩散函数
                    sum = sum + PSF_normalization[i, j];
                    //Console.Write("{0}   ", PSF_normalization[i , j ]);
                }
                //Console.Write("\n");
            }
            int[,] PSF_DEM = new int[ySize / ratio, xSize / ratio];
            for (int i = 0; i < ySize / ratio; i++)
            {
                for (int j = 0; j < ySize / ratio; j++)
                {
                    double a = 0;
                    for (int n = 0; n < ratio; n++)
                    {
                        for (int m = 0; m < ratio; m++)
                        {
                            a = a + PSF_normalization[n, m] * InitialDEM[i * ratio + n, j * ratio + m];
                        }
                    }
                    PSF_DEM[i, j] = (int)((a / sum) + 0.5);
                }
            }
            return PSF_DEM;
        }
    }
}
