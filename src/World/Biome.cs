using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal class Biome {
        public Dictionary<int, Tile> tiles;
        public string[] backgrounds;

        public Biome(Dictionary<int, Tile> tiles, string[] backgrounds) {
            this.tiles = tiles;
            this.backgrounds = backgrounds;
        }
    }
}
