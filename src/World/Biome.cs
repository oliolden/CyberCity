using System;
using System.Collections.Generic;
using System.Text;

namespace CyberCity {
    internal class Biome {
        public readonly Dictionary<int, Tile> front;
        public readonly Dictionary<int, Tile> back;

        public Biome(Dictionary<int, Tile> front, Dictionary<int, Tile> back) {
            this.front = front;
            this.back = back;
        }
    }
}
