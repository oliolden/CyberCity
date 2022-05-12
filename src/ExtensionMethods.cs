using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal static class ExtensionMethods {
        public static T[][] ToJaggedArray<T>(this T[,] input) {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            T[][] returnArray = new T[width][];
            for (int x = 0; x < width; x++) {
                returnArray[x] = new T[height];
                for (int y = 0; y < height; y++) {
                    returnArray[x][y] = input[x, y];
                }
            }
            return returnArray;
        }

        public static T[,] To2DArray<T>(this T[][] input) {
            int width = input.Length;
            int height = input[0].Length;
            T[,] returnArray = new T[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    returnArray[x, y] = input[x][y];
                }
            }
            return returnArray;
        }
    }
}
