using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;

namespace CyberCity {
    internal class Structure {
        public int width { get; set; }
        public int height { get; set; }
        public Tile[,] tiles { get; set; }
        public List<GameObject> objects;

        public Structure(int width, int height) {
            this.width = width;
            this.height = height;
            tiles = new Tile[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tiles[x, y] = new Tile("air");
                }
            }
        }

        public void Save(string name) {
            Debug.WriteLine(JsonSerializer.Serialize(this));
            File.WriteAllText($"..\\..\\..\\Content\\World\\Structures\\{name}.struct", JsonSerializer.Serialize(this));
        }

        public static Structure Load(string name) {
            return JsonSerializer.Deserialize<Structure>(File.ReadAllText($"..\\..\\..\\Content\\World\\Structures\\{name}.struct"));
        }
    }
}
