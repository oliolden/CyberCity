using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class World : GameObject {
        public Texture2D[] backgrounds;
        public Dictionary<int, Tile[,]> chunks;
        private static Point chunkTileSize = new Point(32, 32);
        private static Point chunkSize = new Point(chunkTileSize.X * Tile.width, chunkTileSize.Y * Tile.height);
        private static Point[] offsets = { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(-1, 0), new Point(1, 0), new Point(-1, 1), new Point(0, 1), new Point(1, 1), };
        Random random;
        private int currentChunk { get { return (int)Math.Floor(scene.camera.center.X / chunkSize.X); } set { } }

        public World(Scene myScene, int seed = 0) : base(myScene) { random = new Random(seed); chunks = new Dictionary<int, Tile[,]>(); GenerateChunks(-3, 3); layer = 0f; }

        public void GenerateChunks(int start, int end) {
            bool hasGenerated = false;
            for (int i = start; i <= end; i++) {
                if (chunks.ContainsKey(i)) continue;
                Tile[,] chunk = new Tile[chunkTileSize.X, chunkTileSize.Y];
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        if (y >= 16) { chunk[x, y] = new Tile("metal"); }
                        else chunk[x, y] = new Tile("air");
                    }
                }
                chunks[i] = chunk;
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
            if (!chunks.ContainsKey(chunk)) return new Tile("air");
            return chunks[chunk][(int)Math.Floor(x - chunk * chunkSize.X) / Tile.width, (int)Math.Floor(y) / Tile.height];
        }

        public void UpdateTileTexture(int chunkIndex, int x, int y) {
            Tile[,] chunk = chunks[chunkIndex];
            Tile tile = chunk[x, y];
            if (!TileType.types[tile.id].visible) { tile.textureName = null; return; }
            bool[] check = { false, false, false, false, false, false, false, false, };
            bool[] bools;

            bool CheckTile(Point offset) {
                if (y + offset.Y < 0 || y + offset.Y >= chunkTileSize.Y) return false;
                if (x + offset.X >= 0 && x + offset.X < chunkTileSize.X) return chunk[x + offset.X, y + offset.Y].id == tile.id;
                else {
                    int testChunk = chunkIndex + (int)Math.Floor((x + offset.X) / (float)chunkTileSize.X);
                    if (chunks.ContainsKey(testChunk)) { return chunks[testChunk][(x + offset.X) > 0 ? (x + offset.X) - chunkTileSize.X : chunkTileSize.X + (x + offset.X), y + offset.Y].id == tile.id; }
                    else return false;
                }
            }

            string textureName = "";
            
            for (int i = 0; i < offsets.Length; i++) {
                check[i] = CheckTile(offsets[i]);
            }

            bools = check;

            if (!check[1] || !check[3]) bools[0] = false;
            if (!check[1] || !check[4]) bools[2] = false;
            if (!check[3] || !check[6]) bools[5] = false;
            if (!check[4] || !check[6]) bools[7] = false;

            if (!check[0] && !check[2]) bools[1] = false;
            if (!check[0] && !check[5]) bools[3] = false;
            if (!check[2] && !check[7]) bools[4] = false;
            if (!check[5] && !check[7]) bools[6] = false;

            foreach (bool bit in bools) {
                textureName += bit ? "1" : "0";
            }

            if (game.textures.Keys.Contains($"World\\Tiles\\{chunk[x, y].id}\\{textureName}")) chunk[x, y].textureName = $"World\\Tiles\\{chunk[x, y].id}\\{textureName}";
            else chunk[x, y].textureName = $"World\\Tiles\\{chunk[x, y].id}\\00000000";
        }

        public void SetTile(int x, int y, bool fill) {
            int chunk = (int)Math.Floor(x / (float)chunkTileSize.X);
            x = x - chunk * chunkTileSize.X;
            if (!chunks.ContainsKey(chunk) || y < 0 || y >= chunkTileSize.Y) return;
            if (chunks[chunk][x, y].id != (fill ? "metal" : "air")) {
                chunks[chunk][x, y] = fill ? new Tile("metal") : new Tile("air");
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
                UpdateHitBox();
            }
        }

        public override void Update(GameTime gameTime) {
            GenerateChunks(currentChunk - 2, currentChunk + 2);
        }

        public void UpdateHitBox() {
            hitBox = new List<Rectangle>();
            foreach (KeyValuePair<int, Tile[,]> chunk in chunks) {
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        if (TileType.types[chunk.Value[x, y].id].collideable) {
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
            foreach (Texture2D background in backgrounds) {
                float width = background.Width * scale;
                float offset = -scene.camera.center.X * (1 - parallaxStrength);
                Vector2 pos = new Vector2(scene.camera.center.X - offset + width * (float)Math.Floor(offset / width), scene.camera.center.Y);
                for (int i = -1; i <= 0; i++) {
                    Vector2 drawPos = new Vector2(pos.X - i * width, pos.Y);
                    batch.Draw(background, drawPos, null, Color.White, 0f, new Vector2(background.Width / 2, background.Height / 2), scale, SpriteEffects.None, bgLayer);
                }
                parallaxStrength += 0.2f;
                bgLayer += 0.1f;
            }

            // Draw chunks
            for (int i = currentChunk - 1; i <= currentChunk + 1; i++) {
                if (!chunks.ContainsKey(i)) continue;
                for (int x = 0; x < chunkTileSize.X; x++) {
                    for (int y = 0; y < chunkTileSize.Y; y++) {
                        Tile tile = chunks[i][x, y];
                        if (tile.textureName != null)
                            batch.Draw(game.textures[tile.textureName], new Vector2((float)x * Tile.width + i * chunkSize.X, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + 0.1f);
                    }
                }

            }
            base.Draw(batch, gameTime);
        }
    }
}
