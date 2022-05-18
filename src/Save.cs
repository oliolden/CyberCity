using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CyberCity {
    internal class Save {
        int Seed { get; set; }
        Dictionary<int, Chunk> Chunks { get; set; }
        Dictionary<string, Vector2> gameObjects { get; set; }
        Dictionary<string, Vector2> worldObjects;

        public Save() {
            gameObjects = new Dictionary<string, Vector2>();
            worldObjects = new Dictionary<string, Vector2>();
        }

        public static void SaveGame(Game1 game, string name) {
            Save save = new Save();
            Scene gameScene = game.scenes["game"];
            World world = (World)gameScene.objects["World"];
            save.Seed = world.seed;
            save.Chunks = world.chunks;
            foreach (var obj in gameScene.objects) {
                save.gameObjects[obj.Key] = obj.Value.position;
            }
            File.WriteAllText($"..\\..\\..\\Saves\\{name}.save", JsonSerializer.Serialize(save));

        }
        public static void LoadGame(Game1 game, string name) {
            Save save = JsonSerializer.Deserialize<Save>(File.ReadAllText($"..\\..\\..\\Saves\\{name}.save"));
            Scene gameScene = game.scenes["game"];
            World world = (World)gameScene.objects["World"];
            world.seed = save.Seed;
            world.chunks = save.Chunks;
            foreach (Chunk chunk in world.chunks.Values) {
                chunk.SetWorld(world);
            }

            foreach (var obj in gameScene.objects) {
                obj.Value.position = save.gameObjects[obj.Key];
            }
        }
    }
}
