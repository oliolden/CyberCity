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
    }
}
