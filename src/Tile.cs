using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Tile {
        public static int width = 32, height = 32;

        public static string[] types = new string[] {
            null, "IndustrialTile01"
        };

        public string type;
        public Texture2D texture;

        public Tile(string _type) {
            type = _type;
        }

        public Tile(int id) {
            type = types[id];
        }

        public static Tile[,] GetTiles(int[,] idArray) {
            Tile[,] tileArray = new Tile[idArray.GetLength(1), idArray.GetLength(0)];
            for (int x = 0; x < idArray.GetLength(0); x++) {
                for (int y = 0; y < idArray.GetLength(1); y++) {
                    // Inverting array
                    tileArray[y, x] = new Tile(idArray[x, y]);
                }
            }
            return tileArray;
        }


    }
}
