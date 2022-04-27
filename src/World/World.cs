using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class World : GameObject {
        public Texture2D[] backgrounds;
        public Dictionary<int, Chunk> chunks;
        internal static Point chunkTileSize = new Point(32, 32);
        internal static Point chunkSize = new Point(chunkTileSize.X * Tile.width, chunkTileSize.Y * Tile.height);
        private static Point[] offsets = { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1), };
        int seed;
        Random random;
        private readonly string texturePath = "World\\Tiles\\";

        private int GetCurrentChunk() { return (int)Math.Floor(scene.camera.center.X / chunkSize.X); }


        public World(Scene myScene, int seed = 0) : base(myScene) { this.seed = seed; random = new Random(seed); chunks = new Dictionary<int, Chunk>(); GenerateChunks(-3, 3); layer = 0f; }

        public void GenerateChunks(int start, int end) {
            bool hasGenerated = false;
            for (int i = start; i <= end; i++) {
                if (chunks.ContainsKey(i)) continue;
                chunks[i] = new Chunk(this);
                chunks[i].Generate(seed + i);
                hasGenerated = true;
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        UpdateTileTexture(i, x, y);
                    }
                }
                if (chunks.ContainsKey(i - 1))
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        UpdateTileTexture(i - 1, chunkTileSize.X - 1, y);
                    }
                if (chunks.ContainsKey(i + 1))
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        UpdateTileTexture(i + 1, 0, y);
                    }
            }
            if (hasGenerated) {
                UpdateHitBox();
            }
        }

        public Tile GetTile(float x, float y) {
            int chunk = (int)Math.Floor(x / chunkSize.X);
            if (!chunks.ContainsKey(chunk) || y < 0 || y > chunkSize.Y) return new Tile("air");
            return chunks[chunk].tiles[(int)Math.Floor(x - chunk * chunkSize.X) / Tile.width, (int)Math.Floor(y) / Tile.height];
        }

        public void UpdateTileTexture(int chunkIndex, int x, int y) {
            Tile[,] chunk = chunks[chunkIndex].tiles;
            Tile tile = chunk[x, y];
            if (!TileType.types[tile.id].visible) { tile.textureName = null; return; }
            bool[] check = { false, false, false, false, false, false, false, false, };
            bool[] bools;

            bool CheckTile(Point offset) {
                return CheckTileInfo(offset).id == tile.id;
            }

            Tile CheckTileInfo(Point offset) {
                if (y + offset.Y < 0 || y + offset.Y >= chunkTileSize.Y) return new Tile("air");
                if (x + offset.X >= 0 && x + offset.X < chunkTileSize.X) return chunk[x + offset.X, y + offset.Y];
                else {
                    int testChunk = chunkIndex + (int)Math.Floor((x + offset.X) / (float)chunkTileSize.X);
                    if (chunks.ContainsKey(testChunk)) { return chunks[testChunk].tiles[(x + offset.X) > 0 ? (x + offset.X) - chunkTileSize.X : chunkTileSize.X + (x + offset.X), y + offset.Y]; }
                    else return new Tile("air");
                }
            }

            string textureName = "";

            for (int i = 0; i < offsets.Length; i++) {
                check[i] = CheckTile(offsets[i]);
            }

            bools = check;

            // Corner is false if one adjecent side is false
            if (!check[1] || !check[3]) bools[0] = false;
            if (!check[1] || !check[4]) bools[2] = false;
            if (!check[3] || !check[6]) bools[5] = false;
            if (!check[4] || !check[6]) bools[7] = false;


            // If full corner connection exists then side is false if both adjecent corners are false
            if ((check[0] && check[1] && check[3]) ||
                (check[1] && check[2] && check[4]) ||
                (check[3] && check[5] && check[6]) ||
                (check[4] && check[6] && check[7])) {
                if (!check[0] && !check[2]) bools[1] = false;
                if (!check[0] && !check[5]) bools[3] = false;
                if (!check[2] && !check[7]) bools[4] = false;
                if (!check[5] && !check[7]) bools[6] = false;
            }

            // Convert to string
            foreach (bool bit in bools) {
                textureName += bit ? "1" : "0";
            }

            if (bools[1] && Game1.textures.ContainsKey(texturePath + chunk[x, y - 1].GetPath() + textureName + "_1") && !CheckTile(new Point(0, -2))) {
                chunk[x, y].textureName = chunk[x, y - 1].GetPath() + textureName + "_1";
            }
            else if (bools[3] && CheckTileInfo(offsets[3]).variant != tile.variant && Game1.textures.ContainsKey(texturePath + CheckTileInfo(offsets[4]).GetPath() + textureName + "_R")) {
                chunk[x, y].textureName = CheckTileInfo(offsets[4]).GetPath() + textureName + "_R";
            }
            else if (bools[4] && CheckTileInfo(offsets[4]).variant != tile.variant && Game1.textures.ContainsKey(texturePath + CheckTileInfo(offsets[4]).GetPath() + textureName + "_L")) {
                chunk[x, y].textureName = CheckTileInfo(offsets[4]).GetPath() + textureName + "_L";
            }
            else {
                if (Game1.textures.Keys.Contains(texturePath + tile.GetPath() + textureName)) {
                    chunk[x, y].textureName = tile.GetPath() + textureName;
                }
                else {
                    if (Game1.textures.Keys.Contains(texturePath + tile.id + textureName))
                        chunk[x, y].textureName = tile.id + textureName;
                    else if (Game1.textures.Keys.Contains(texturePath + tile.GetPath() + "\\00000000"))
                        chunk[x, y].textureName = tile.GetPath() + "\\00000000";
                    else
                        chunk[x, y].textureName = tile.id + "\\00000000";
                }
            }
        }

        public void SetTile(int x, int y, Tile tile) {
            int chunk = (int)Math.Floor(x / (float)chunkTileSize.X);
            x = x - chunk * chunkTileSize.X;
            if (!chunks.ContainsKey(chunk) || y < 0 || y >= chunkTileSize.Y) return;
            if (!chunks[chunk].tiles[x, y].Equals(tile)) {
                chunks[chunk].tiles[x, y] = new Tile(tile.id, tile.variant);
                UpdateTileTexture(chunk, x, y);
                foreach (Point offset in offsets) {
                    if (y + offset.Y < 0 || y + offset.Y >= chunkTileSize.Y) continue;
                    if (x + offset.X >= 0 && x + offset.X < chunkTileSize.X) UpdateTileTexture(chunk, x + offset.X, y + offset.Y);
                    else {
                        int nextChunk = chunk + (int)Math.Floor((x + offset.X) / (float)chunkTileSize.X);
                        if (chunks.ContainsKey(nextChunk)) { UpdateTileTexture(nextChunk, (x + offset.X) > 0 ? (x + offset.X) - chunkTileSize.X : chunkTileSize.X + (x + offset.X), y + offset.Y); }
                        else continue;
                    }
                }
                if (y + 2 < chunkTileSize.Y) UpdateTileTexture(chunk, x, y + 2);
                UpdateHitBox();
            }
        }

        public override void Update(GameTime gameTime) {
            int currentChunk = GetCurrentChunk();
            GenerateChunks(currentChunk - 2, currentChunk + 2);
        }

        public void UpdateHitBox() {
            hitBox = new List<Rectangle>();
            foreach (KeyValuePair<int, Chunk> chunk in chunks) {
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        if (chunk.Value.tiles[x, y].id != "air") {
                            hitBox.Add(new Rectangle(x * Tile.width + chunk.Key * chunkSize.X, y * Tile.height, Tile.width, Tile.height));
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            // Draw backgrounds
            float scale = 1.4f;
            float parallaxStrength = 1;
            float bgLayer = layer - 1;
            //foreach (Texture2D background in backgrounds) {
            //    float width = background.Width * scale;
            //    float offset = -scene.camera.center.X * (1 - parallaxStrength);
            //    Vector2 pos = new Vector2(scene.camera.center.X - offset + width * (float)Math.Floor(offset / width), scene.camera.center.Y);
            //    for (int i = -1; i <= 0; i++) {
            //        Vector2 drawPos = new Vector2(pos.X - i * width, pos.Y);
            //        batch.Draw(background, drawPos, null, Color.White, 0f, new Vector2(background.Width / 2, background.Height / 2), scale, SpriteEffects.None, bgLayer);
            //    }
            //    parallaxStrength += 0.2f;
            //    bgLayer += 0.1f;
            //}

            // Draw chunks
            int currentChunk = GetCurrentChunk();
            for (int i = currentChunk - 1; i <= currentChunk + 1; i++) {
                if (!chunks.ContainsKey(i)) continue;
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        // Foreground
                        Tile tile = chunks[i].tiles[x, y];
                        if (tile.background != null)
                            batch.Draw(Game1.textures[texturePath + "background\\" + tile.background], new Vector2((float)x * Tile.width + i * chunkSize.X, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + 0.1f);
                        if (tile.textureName != null)
                            batch.Draw(Game1.textures[texturePath + tile.textureName], new Vector2((float)x * Tile.width + i * chunkSize.X, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + 0.2f);
                    }
                }
            }
            base.Draw(batch, gameTime);
        }
    }
}
