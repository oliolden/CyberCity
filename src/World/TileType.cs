using System.Collections.Generic;

namespace CyberCity {
    internal class TileType {
        public static Dictionary<string, TileType> types = new Dictionary<string, TileType> {
            { "air", new TileType(false, 0.1f) },
            { "metal", new TileType(true, 0.001f) },
            { "stone", new TileType(true, 0.001f) },
        };

        public bool visible;
        public float friction;

        public TileType(bool visible, float friction) {
            this.visible = visible;
            this.friction = friction;
        }
    }
}
