using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace CyberCity {
    internal class Structure {
        public Tile[,] tiles;
        public List<GameObject> objects;
        public int width, height;

        public Structure(int width, int height) {
            this.width = width;
            this.height = height;
            tiles = new Tile[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tiles[x, y] = new Tile("air");
                }
            }
        }
    }
}
