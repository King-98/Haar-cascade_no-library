using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyramidNetwork
{
    static class Feature
    {
        public const int COUNT = 7; // 피처개수
    }
    class create
    {
        public int[][,] feature = new int[Feature.COUNT][,];
        public int[][] XY = new int[Feature.COUNT][];

        public create(int[,] window)
        {
            // 피처 초기화
            feature[0] = edgeB((int)(window.GetLength(0) / 20.0 * 14), (int)(window.GetLength(1) / 20.0 * 5)); // 눈
            feature[1] = eyeLine((int)(window.GetLength(0) / 20.0 * 14), (int)(window.GetLength(1) / 20.0 * 6)); // 눈라인
            feature[2] = nosemouth((int)(window.GetLength(0) / 20.0 * 12), (int)(window.GetLength(1) / 20.0 * 8)); // 코&입
            feature[3] = eyeLine((int)(window.GetLength(0) / 20.0 * 20), (int)(window.GetLength(1) / 20.0 * 4)); // 헤어
            feature[4] = edgeB((int)(window.GetLength(0) / 20.0 * 7), (int)(window.GetLength(1) / 20.0 * 3)); // 입
            feature[5] = edgeA((int)(window.GetLength(0) / 20.0 * 16), (int)(window.GetLength(1) / 20.0 * 4)); // 눈썹
            feature[6] = edgeB((int)(window.GetLength(0) / 20.0 * 4), (int)(window.GetLength(1) / 20.0 * 10)); // 오른쪽볼
            XY[0] = new int[] { trans(3, 14), trans(9, 5) };
            XY[1] = new int[] { trans(3, 14), trans(7, 6) };
            XY[2] = new int[] { trans(4, 12), trans(16, 10) };
            XY[3] = new int[] { 0, 0 };
            XY[4] = new int[] { trans(6, 7), trans(22, 3) };
            XY[5] = new int[] { trans(2, 16), trans(5, 4) };
            XY[6] = new int[] { trans(13, 4), trans(6, 10) };
        }

        public int trans(int start, int WH)
        {
            // 시작점과 윈도우크기
            return (int)(start / 20.0 * WH);
        }

        public float sim(int[,] window, int[,] feature, int[] XY)
        {
            // 전체그레이와 객체그레이을 받아서 유사도(0~1) 반환
            float white = 0, black = 0;
            int count = 0;

            for (int y = 0; y < window.GetLength(1) - (window.GetLength(1) - feature.GetLength(1)); y++)
                for (int x = 0; x < window.GetLength(0) - (window.GetLength(0) - feature.GetLength(0)); x++)
                {
                    if (feature[x, y] == 0) // 검은색일때
                        black += (255 - window[x + XY[0], y + XY[1]]) / (float)255; // 검색 0
                    else
                        white += window[x + XY[0], y + XY[1]] / (float)255; // 흰색 1
                    count++;
                }

            return (black + white) / count;
        }

        public int[,] edgeB(int width, int height)
        {
            // 가로세로길이를 받고 edgeB모양으로 변환
            int[,] grayarray = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (height / 2 > y)
                        grayarray[x, y] = 0;
                    else
                        grayarray[x, y] = 255;
                }

            return grayarray;
        }

        public int[,] edgeA(int width, int height)
        {
            // 가로세로길이를 받고 edgeB모양으로 변환
            int[,] grayarray = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (height / 2 > y)
                        grayarray[x, y] = 255;
                    else
                        grayarray[x, y] = 0;
                }

            return grayarray;
        }

        public int[,] eyeLine(int width, int height)
        {
            // 가로세로길이를 받고 edgeB모양으로 변환
            int[,] grayarray = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (width / 3 >= x || (width - width / 3) <= x)
                        grayarray[x, y] = 0;
                    else
                        grayarray[x, y] = 255;
                }

            return grayarray;
        }

        public int[,] nosemouth(int width, int height)
        {
            // 가로세로길이를 받고 edgeB모양으로 변환
            int[,] grayarray = new int[width, height];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (width / 3 >= x || (width - width / 3) <= x)
                        grayarray[x, y] = 255;
                    else
                        grayarray[x, y] = 0;
                }

            return grayarray;
        }
    }
}
