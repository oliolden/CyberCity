using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberCity {
    internal class Chunk {
        internal Game1 game;
        internal World world;
        public Tile[,] front;
        public Tile[,] back;
        public List<GameObject> objects;
        string biome;
        private static Dictionary<string, Biome> biomes = new Dictionary<string, Biome> {
            { "industrialZone", new Biome(new Dictionary<int, Tile> { { 16, new Tile("metal") } }, new Dictionary<int, Tile> { { 16, new Tile("metalwall") } }) },
            { "greenZone", new Biome(new Dictionary<int, Tile> { { 16, new Tile("stone", "grass") }, { 17, new Tile("stone") } }, new Dictionary<int, Tile> { { 16, new Tile("stonewall") } }) },
        };

        public Chunk(World world) {
            this.world = world;
            game = world.game;
            front = new Tile[World.chunkTileSize.X, World.chunkTileSize.Y];
            back = new Tile[World.chunkTileSize.X, World.chunkTileSize.Y];
        }

        private void SetTile(int x, int y, Tile tile) {
            TileType type = tile.GetTileType();
            if (type.background) back[x, y] = tile;
            else front[x, y] = tile;
            if (type.visible && type.background && Game1.textures.ContainsKey($"World\\Tiles\\{tile.GetPath()}\\texture"))
                tile.textureName = $"World\\Tiles\\{tile.GetPath()}\\texture";
        }

        public void Generate(int seed) {
            Random random = new Random(seed);
            biome = biomes.ElementAt(random.Next(biomes.Count())).Key;
            for (int x = 0; x < front.GetLength(0); x++) {
                for (int y = 0; y < front.GetLength(1); y++) {
                    int front = biomes[biome].front.Keys.Aggregate(0, (a, next) => next < y && next > a ? next : a);
                    if (front == 0) SetTile(x, y, new Tile("air"));
                    else SetTile(x, y, biomes[biome].front[front].Copy());
                    int back = biomes[biome].back.Keys.Aggregate(0, (a, next) => next < y && next > a ? next : a);
                    if (back == 0) SetTile(x, y, new Tile("airwall"));
                    else SetTile(x, y, biomes[biome].back[back].Copy());
                }
            }

        }


    }
}
