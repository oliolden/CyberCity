using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal class Biome {
        public static readonly string texturePath = "World\\Background\\";
        public Dictionary<int, Tile> tiles;
        public string[] backgrounds;

        public Biome(Dictionary<int, Tile> tiles, string background) {
            this.tiles = tiles;
            List<string> backgroundList = new List<string>();
            for (int i = 0; i < 5; i++) {
                if (Game1.textures.ContainsKey($"World\\Background\\{background}\\{i}"))
                    backgroundList.Add($"{background}\\{i}");
            }
            backgrounds = backgroundList.ToArray();
        }

        public static Dictionary<string, Biome> all = new Dictionary<string, Biome> {
            { "industrialZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("metal", null, "metal") } },
                "industrialZone"
                )
            },
            { "greenZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("stone", "grass", "stone") }, { 17, new Tile("stone", null, "stone") } },
                "industrialZone"
                )
            },
        };

    }
}
