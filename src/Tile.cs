using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Tile {
        public static int tileWidth = 32, tileHeight = 32;

        private World world;
        public string type;
        public Texture2D texture;

        public Tile(World _world, string _type) {
            world = _world;
            type = _type;
        }

        public void UpdateTexture() {

        }
    }
}
