using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyramidNetwork
{
    class faceDetection
    {
        imageProcessing ip = new imageProcessing();

        public Bitmap BB_4(Bitmap totalpicture)
        {
            // 피처여러개를 적용시킨 바운딩박스 비트맵
            int[,] total = ip.FuzzyStretch(ip.GrayArray(totalpicture));
            int[,] window = new int[(int)(total.GetLength(0) / 100.0 * 10), (int)(total.GetLength(1) / 100.0 * 13)];

            create cr = new create(window);

            double[] sim = new double[Feature.COUNT];
            int[] XY = new int[2];
            double maxsim = 0.0;
            int count = 0;
            int pick = 0;
            double sum = 0;
            int i;
            int startX = 0, startY = 0;

            while (total.GetLength(1) > window.GetLength(1) || total.GetLength(0) > window.GetLength(0))
            {
                for (int y = 0; y < total.GetLength(1) - window.GetLength(1); y += 1)
                {
                    for (int x = 0; x < total.GetLength(0) - window.GetLength(0); x += 1)
                    {
                        window = BoundGA(total, window, x, y);
                        for (i = 0; i < Feature.COUNT; i++)
                        {
                            if (cr.XY[i][0] > window.GetLength(0) || cr.XY[i][1] > window.GetLength(1))
                            {
                                sim[i] = 0.5;
                                continue;
                            }
                            sim[i] = cr.sim(window, cr.feature[i], cr.XY[i]);
                            if (sim[i] > 0.7)
                                sim[i] = 0.7;
                        }

                        for (i = 0; i < Feature.COUNT; i++)
                            sum += sim[i];

                        if (sum / Feature.COUNT > maxsim)
                        {
                            maxsim = sum / Feature.COUNT;
                            startX = x; startY = y;
                            pick = count;
                            Console.WriteLine(maxsim);
                            Console.WriteLine("pyramid계층 : " + pick);
                            Console.WriteLine();
                        }
                        sum = 0;
                    }
                }

                count++;
                total = ip.smooding(total);
            }

            total = ip.GrayArray(totalpicture);
            for (i = 0; i < pick; i++)
                total = ip.smooding(total);

            total = ip.GrayArray(totalpicture);
            Bitmap bitmap = BB_2(total, window, pick, startX, startY);
            return bitmap;
        }

        public int[,] ROI_2(Bitmap totalpicture)
        {
            // totalpicture를 받아서 유사도검사,이미지피라미드,바운딩박스
            int[,] total = ip.FuzzyStretch(ip.GrayArray(totalpicture));
            int[,] window = new int[(int)(total.GetLength(0) / 100.0 * 10), (int)(total.GetLength(1) / 100.0 * 13)];
            create cr = new create(window);
            double[] sim = new double[Feature.COUNT];
            double maxsim = 0.0;
            int count = 0;
            int pick = 0;
            double sum = 0;
            int i;
            int startX = 0, startY = 0;

            while (total.GetLength(1) > window.GetLength(1) || total.GetLength(0) > window.GetLength(0))
            {
                for (int y = 0; y < total.GetLength(1) - window.GetLength(1); y += 1)
                    for (int x = 0; x < total.GetLength(0) - window.GetLength(0); x += 1)
                    {
                        window = BoundGA(total, window, x, y);
                        for (i = 0; i < Feature.COUNT; i++)
                        {
                            if (cr.XY[i][0] > window.GetLength(0) || cr.XY[i][1] > window.GetLength(1))
                            {
                                sim[i] = 0.5;
                                continue;
                            }
                            sim[i] = cr.sim(window, cr.feature[i], cr.XY[i]);
                            if (sim[i] > 0.7)
                                sim[i] = 0.7;
                        }

                        for (i = 0; i < Feature.COUNT; i++)
                            sum += sim[i];

                        if (sum / Feature.COUNT > maxsim)
                        {
                            maxsim = sum / Feature.COUNT;
                            startX = x; startY = y;
                            pick = count;
                            Console.WriteLine(maxsim);
                            Console.WriteLine("pyramid계층 : " + pick);
                            Console.WriteLine();
                        }
                        sum = 0;
                    }
                count++;
                total = ip.smooding(total);
            }

            total = ip.GrayArray(totalpicture);
            for (i = 0; i < pick; i++)
                total = ip.smooding(total);

            total = ip.GrayArray(totalpicture);
            // 바운딩박스에 들어갈 정보들 다필요해서 더 코드 못줄임
            return ROI_BB(total, window, pick, startX, startY);
        }

        public Bitmap Convert(int[,] grayarray)
        {
            // 배열을 받아서 비트맵으로
            Bitmap bitmap = new Bitmap(grayarray.GetLength(0), grayarray.GetLength(1));
            Color color;

            for (int y = 0; y < grayarray.GetLength(1); y++)
            {
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    color = Color.FromArgb(grayarray[x, y], grayarray[x, y], grayarray[x, y]);
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public int[,] BoundGA(int[,] totalGA, int[,] face, int startX, int startY)
        {
            // 원래이미지의 얼굴크기만큼 잘라서 grayarray로 바꿔줌
            for (int y = startY; y < (startY + face.GetLength(1)); y++)
                for (int x = startX; x < (startX + face.GetLength(0)); x++)
                {
                    face[x - startX, y - startY] = totalGA[x, y];
                }

            return face;
        }

        public Bitmap BB_2(int[,] totalGA, int[,] objGA, int pick, int startX, int startY)
        {
            // 순정토탈이미지를 받아서 다운샘플링된 만큼 계산해서 바운딩박스
            Bitmap bitmap = new Bitmap(totalGA.GetLength(0), totalGA.GetLength(1));
            Color color;
            int up = (pick * 2 == 0) ? 1 : pick * 2;
            int objwidth = objGA.GetLength(0) * up;
            int objheight = objGA.GetLength(1) * up;
            startX = startX * up;
            startY = startY * up;

            for (int y = 0; y < totalGA.GetLength(1); y++)
            {
                for (int x = 0; x < totalGA.GetLength(0); x++)
                {
                    color = Color.FromArgb(totalGA[x, y], totalGA[x, y], totalGA[x, y]);
                    bitmap.SetPixel(x, y, color);
                    // 중간에 바운딩
                    if ((x >= startX && x <= (startX + objwidth)) && (y >= startY && y <= (startY + objheight)))
                    { // 가장자리만
                        if (x == startX || y == startY || x == (startX + objwidth) || y == (startY + objheight))
                            bitmap.SetPixel(x, y, Color.Red);
                    }
                }
            }

            return bitmap;
        }

        public int[,] ROI_BB(int[,] totalGA, int[,] face, int pick, int startX, int startY)
        {
            // 전체이미지의 ROI영역을 검출
            int up = (pick * 2 == 0) ? 1 : pick * 2;
            int objwidth = face.GetLength(0) * up;
            int objheight = face.GetLength(1) * up;
            startX = startX * up;
            startY = startY * up;

            int[,] roi = new int[(startX + objwidth) - startX, (startY + objheight) - startY];

            for (int y = startY; y < (startY + objheight); y++)
                for (int x = startX; x < (startX + objwidth); x++)
                {
                    roi[x - startX, y - startY] = totalGA[x, y];
                }

            return roi;
        }
    }
}
