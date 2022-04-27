using System.Collections.Generic;

namespace CyberCity {
    internal class Biome {
        public Dictionary<int, Tile> tiles;
        public string background;

        public Biome(Dictionary<int, Tile> tiles, string background) {
            this.tiles = tiles;
            this.background = background;
        }

        public static Dictionary<string, Biome> all = new Dictionary<string, Biome> {
            { "industrialZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("metal", null, "metal") } },
                "industrialZone"
                )
            },
            { "greenZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("stone", "grass", "stone") }, { 17, new Tile("stone", null, "stone") } },
                "greenZone"
                )
            },
        };

    }
}
