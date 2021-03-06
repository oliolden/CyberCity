using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CyberCity {
    internal class Tile {
        public static int width = 32, height = 32;

        public string id { get; set; }
        public string variant { get; set; }
        public Texture2D texture;
        public string background { get; set; }
        public Color color;

        public string GetPath() { return id + (variant != null ? "\\" + variant : "") + "\\"; }
        public TileType GetTileType() { return TileType.types[id]; }

        public Tile(string id, string variant = null, string background = null) {
            this.id = id;
            this.variant = variant;
            this.background = background;
            color = Color.White;
        }

        public Tile() {
            id = "air";
            variant = null;
            background = null;
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

        public Tile Copy() {
            return new Tile(id, variant, background);
        }
    }
}
