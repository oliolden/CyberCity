using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Tile {
        public static int width = 32, height = 32;

        public string id;
        public string textureName;
        public Color color;

        public Tile(string _id) {
            id = _id;
            color = Color.White;
        }
    }
}
