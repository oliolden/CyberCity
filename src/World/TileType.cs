using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal class TileType {
        public static Dictionary<string, TileType> types = new Dictionary<string, TileType> {
            { "air", new TileType(false, false, 0.1f) },
            { "airwall", new TileType(false, true, 0f) },
            { "metal", new TileType(true, false, 0.001f) },
            { "metalwall", new TileType(true, true, 0f) },
            { "stone", new TileType(true, false, 0.001f) },
            { "stonewall", new TileType(true, true, 0f)}
        };

        public bool visible;
        public bool background;
        public float friction;

        public TileType(bool visible, bool background, float friction) {
            this.visible = visible;
            this.background = background;
            this.friction = friction;
        }
    }
}
