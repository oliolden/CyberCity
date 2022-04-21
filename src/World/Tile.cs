using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class Tile {
        public static int width = 32, height = 32;

        public string id;
        public string variant;
        public string textureName;
        public Color color;

        public string GetPath() { return id + (variant != null ? "\\" + variant : ""); }
        public TileType GetTileType() { return TileType.types[id]; }

        public Tile(string _id, string _variant = null) {
            id = _id;
            variant = _variant;
            color = Color.White;
        }

        public override bool Equals(object obj) {
            Tile tile = obj as Tile;
            if (tile == null) return false;
            return (id == tile.id && variant == tile.variant);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
