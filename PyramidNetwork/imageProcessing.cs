using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyramidNetwork
{
    class imageProcessing
    {
        public int[,] neighbor(int[,] window)
        {
            // 정사각형 배열을 받아서 interpolation2.25배
            int[,] interpolation = new int[window.GetLength(0) / 2 * 3, window.GetLength(1) / 2 * 3];
            int i = 0;
            int j = 0;

            for (int y = 0; y < interpolation.GetLength(1); y += 3)
            {
                for (int x = 0; x < interpolation.GetLength(0); x += 3)
                {
                    interpolation[x, y] = window[i, j];
                    interpolation[x + 2, y] = window[i + 1, j];
                    interpolation[x, y + 2] = window[i, j + 1];
                    interpolation[x + 2, y + 2] = window[i + 1, j + 1];
                    interpolation[x + 1, y] = (window[i, j] + window[i + 1, j]) / 2;
                    interpolation[x + 1, y + 2] = (window[i, j + 1] + window[i + 1, j + 1]) / 2;
                    interpolation[x, y + 1] = (window[i, j] + window[i, j + 1]) / 2;
                    interpolation[x + 2, y + 1] = (window[i + 1, j] + window[i + 1, j + 1]) / 2;
                    interpolation[x + 1, y + 1] = (window[i, j] + window[i + 1, j] + window[i, j + 1] + window[i + 1, j + 1]) / 4;
                    i += 2;
                }
                i = 0;
                j += 2;
            }

            return interpolation;
        }

        public int[,] GrayArray(Bitmap bitmap)
        {
            /*
                비트맵을 그레이어레이로
             */
            Color gray;
            int brightness;
            int[,] grayarray = new int[bitmap.Width, bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    gray = bitmap.GetPixel(x, y);
                    brightness = (int)(0.299 * gray.R + 0.587 * gray.G + 0.114 * gray.B);
                    grayarray[x, y] = brightness;
                }

            return grayarray;
        }

        public int[,] FuzzyStretch(int[,] grayarray)
        {
            /*
                gray array를 받아서 퍼지 스트레칭한  array를 다시반환
             */

            int Xmean = 0, Xmin = 255, Xmax = 0, Dmin, Dmax, a, lmax, lmin, lmid, x, y;
            double l_value, r_value, percent = 0.05;

            // 평균 명암도 구함
            for (y = 0; y < grayarray.GetLength(1); y++)
            {
                for (x = 0; x < grayarray.GetLength(0); x++)
                {
                    Xmean += grayarray[x, y];
                    if (Xmin > grayarray[x, y])
                        Xmin = grayarray[x, y]; //gray에서 가장 작은값
                    if (Xmax < grayarray[x, y])
                        Xmax = grayarray[x, y];
                }
            }
            Xmean /= (grayarray.GetLength(1) * grayarray.GetLength(0)); // 평균명암도

            // 어두운 영역과 밝은 영역의 거리값을 구함
            Dmax = Xmax - Xmean;
            Dmin = Xmean - Xmin;// 평균명암-min

            //높은 명암도에 속하면 a에넣고 낮은것도 마찬가지
            if (Xmean > 128) // 절반보다 높으면 전체에서 평균명암도를뺌
                a = 255 - Xmean;
            else if (Xmean <= Dmin) // Dmin이 더높을리없음
                a = Dmin;
            else if (Xmean >= Dmax) // 마찬가지
                a = Dmax;
            else
                a = Xmean; // a=xmean

            lmax = Xmean + a; // 평균 + 평균
            lmin = Xmean - a; // 0
            lmid = (lmax + lmin) / 2; // 소속도의평균

            if (lmin != 0) percent = (double)lmin / (double)lmax;
            l_value = (lmid - lmin) * percent + lmin;
            r_value = -(lmax - lmid) * percent + lmax;
            int alpha = (int)l_value;
            int beta = (int)r_value;
            int[] LUT = new int[256];

            for (x = 0; x < alpha; x++) LUT[x] = 0;
            for (x = 255; x > beta; x--) LUT[x] = 255;
            for (x = alpha; x <= beta; x++)
                LUT[x] = (int)((x - alpha) * 255.0 / (beta - alpha));

            for (y = 0; y < grayarray.GetLength(1); y++)
            {
                for (x = 0; x < grayarray.GetLength(0); x++)
                {
                    grayarray[x, y] = LUT[grayarray[x, y]];
                }
            }

            return grayarray;
        }

        public int[,] smooding(int[,] total)
        {
            // 토탈 이미지를 가우시안 필터 스무딩으로 다운샘플링시킴
            int[,] mask = { { 1, 2, 1 },
                            { 2, 4, 2 },
                            { 1, 2, 1 } };
            int[,] pixtotal = new int[total.GetLength(0) / 2, total.GetLength(1) / 2];
            int sum;
            int r, c;

            for (int y = 2; y < total.GetLength(1) - mask.GetLength(1); y += 2)
                for (int x = 2; x < total.GetLength(0) - mask.GetLength(1); x += 2)
                {
                    sum = 0;
                    // -1은 짝수면 하나남아서 그냥 제거
                    for (r = 0; r < mask.GetLength(0); r++)
                    {
                        for (c = 0; c < mask.GetLength(1); c++)
                        {
                            sum += total[x + c, y + r] * mask[c, r];
                        }
                    }
                    pixtotal[x / 2, y / 2] = sum / 16;
                }

            return pixtotal;
        }
    }
}
