using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberCity {
    internal class Chunk {
        internal Game1 game;
        internal World world;
        public Tile[,] tiles;
        public Tile[][] Tiles { get { return tiles.ToJaggedArray(); } set { tiles = value.To2DArray(); } }
        public string biome { get; set; }

        public Chunk(World world) {
            this.world = world;
            game = world.game;
            tiles = new Tile[World.chunkTileSize.X, World.chunkTileSize.Y];
        }

        public Chunk() { }

        public void SetWorld(World world) {
            this.world = world;
            game = world.game;
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

        public void PlaceStructure(Structure structure, int X, int Y) {
            if (structure == null) return;
            for (int x = 0; x < structure.width; x++) {
                for (int y = 0; y < structure.height; y++) {
                    SetTile(X + x, Y + y, structure.tiles[x, y]);
                }
            }
        }

        public void Generate(int seed) {
            Random random = new Random(seed);
            biome = Biome.all.ElementAt(random.Next(Biome.all.Count())).Key;
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    int key = Biome.all[biome].tiles.Keys.Aggregate(0, (a, next) => next < y && next > a ? next : a);
                    if (key == 0) SetTile(x, y, new Tile("air"));
                    else SetTile(x, y, Biome.all[biome].tiles[key].Copy());
                }
            }
            if (biome == "industrialZone") {
                Structure structure = Structure.Load("testbuild");
                PlaceStructure(structure, World.chunkTileSize.X / 2 - structure.width / 2, 22 - structure.height);
            }
        }
    }
}
