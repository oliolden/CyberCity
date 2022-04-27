using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberCity {
    internal class Chunk {
        internal Game1 game;
        internal World world;
        public Tile[,] tiles;
        public List<GameObject> objects;
        string biome;
        //private static Dictionary<string, Dictionary<int, Tile>> biomes = new Dictionary<string, Dictionary<int, Tile>> {
        //    { "industrialZone", new Dictionary<int, Tile> { { 16, new Tile("metal", null, "metal") } } },
        //    { "greenZone", new Dictionary<int, Tile> { { 16, new Tile("stone", "grass", "stone") }, { 17, new Tile("stone", null, "stone") } } },
        //};
        private static Dictionary<string, Biome> biomes = new Dictionary<string, Biome> {
            { "industrialZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("metal", null, "metal") } },
                new string[] { }
                ) 
            },
            { "greenZone", new Biome(
                new Dictionary<int, Tile> { { 16, new Tile("stone", "grass", "stone") }, { 17, new Tile("stone", null, "stone") } },
                new string[] { }
                )
            },
        };

        public Chunk(World world) {
            this.world = world;
            game = world.game;
            tiles = new Tile[World.chunkTileSize.X, World.chunkTileSize.Y];
        }

        private void SetTile(int x, int y, Tile tile) {
            tiles[x, y] = tile.Copy();
        }

        private void SetBackground(int x, int y, string background) {
            tiles[x, y].background = background;
        }
        private void SetId(int x, int y, string id) {
            tiles[x, y].id = id;
        }
        private void SetVariant(int x, int y, string variant) {
            tiles[x, y].variant = variant;
        }

        public void Generate(int seed) {
            Random random = new Random(seed);
            biome = biomes.ElementAt(random.Next(biomes.Count())).Key;
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    int key = biomes[biome].tiles.Keys.Aggregate(0, (a, next) => next < y && next > a ? next : a);
                    if (key == 0) SetTile(x, y, new Tile("air"));
                    else SetTile(x, y, biomes[biome].tiles[key].Copy());
                }
            }

        }


    }
}
