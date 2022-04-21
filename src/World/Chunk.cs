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
        //string biome;

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
            for (int x = 0; x < front.GetLength(0); x++) {
                for (int y = 0; y < front.GetLength(1); y++) {
                    if (y >= 18) SetTile(x, y, new Tile("stone"));
                    else if (y >= 16) SetTile(x, y, new Tile("stone", "grass"));
                    else SetTile(x, y, new Tile("air"));

                    if (y >= 16) SetTile(x, y, new Tile("stonewall"));
                    else SetTile(x, y, new Tile("airwall"));
                }
            }

        }
    }
}
