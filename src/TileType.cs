using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal class TileType {
        public static Dictionary<string, TileType> types = new Dictionary<string, TileType> {
            { "air", new TileType(false, false, 0.1f) }, { "metal", new TileType(true, true, 0.001f) }
        };

        public bool visible;
        public bool collideable;
        public float friction;

        public TileType(bool visible, bool collideable, float friction) {
            this.visible = visible;
            this.collideable = collideable;
            this.friction = friction;
        }
    }
}
