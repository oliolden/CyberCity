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
        private static int chunkWidth = 32 * 16;
        Random random;
        private int currentChunk { get { return (int)Math.Floor(scene.camera.center.X / chunkWidth); } set { } }

        public World(Scene myScene, int seed = 0) : base(myScene) { random = new Random(seed); chunks = new Dictionary<int, Tile[,]>(); GenerateChunks(-3, 3); }

        public void GenerateChunks(int start, int end) {
            bool hasGenerated = false;
            for (int i = start; i <= end; i++) {
                if (chunks.ContainsKey(i)) continue;
                Tile[,] chunk = new Tile[16, 16];
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        if (y >= 12) { chunk[x, y] = new Tile(1); }
                        else chunk[x, y] = new Tile(0);
                    }
                }
                chunks[i] = chunk;
                hasGenerated = true;
            }
            if (hasGenerated) {
                UpdateTileTextures();
                UpdateHitBox();
            }
        }

        public void UpdateTileTextures() {
            foreach (KeyValuePair<int, Tile[,]> chunk in chunks) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunk.Value[x, y];
                        if (tile.type == null) { tile.texture = null; continue; }
                        Dictionary<char, bool> sides = new Dictionary<char, bool> { { 'N', false }, { 'E', false }, { 'S', false }, { 'W', false } };

                        if (y > 0 && chunk.Value[x, y - 1].type != tile.type) sides['N'] = true;
                        if (y < 15 && chunk.Value[x, y + 1].type != tile.type) sides['S'] = true;
                        if (x < 15) { if (chunk.Value[x + 1, y].type != tile.type) sides['E'] = true; }
                        else if (chunks.ContainsKey(chunk.Key + 1)) { if (chunks[chunk.Key + 1][0, y].type != tile.type) sides['E'] = true; }
                        if (x > 0) { if (chunk.Value[x - 1, y].type != tile.type) sides['W'] = true; }
                        else if (chunks.ContainsKey(chunk.Key - 1)) { if (chunks[chunk.Key - 1][15, y].type != tile.type) sides['W'] = true; }

                        Dictionary<string, bool> corners = new Dictionary<string, bool> { { "NE", false }, { "SE", false }, { "SW", false }, { "NW", false } };
                        if (x < 15) {
                            if (y > 0 && chunk.Value[x + 1, y - 1].type != tile.type) corners["NE"] = true;
                            if ( y < 15 && chunk.Value[x + 1, y + 1].type != tile.type) corners["SE"] = true;
                        }
                        else if (chunks.ContainsKey(chunk.Key + 1)) {
                            if (y > 0 && chunks[chunk.Key + 1][0, y - 1].type != tile.type) corners["NE"] = true;
                            if (y < 15 && chunks[chunk.Key + 1][0, y + 1].type != tile.type) corners["SE"] = true;
                        }
                        if (x > 0) {
                            if (y < 15 && chunk.Value[x - 1, y + 1].type != tile.type) corners["SW"] = true;
                            if (y > 0 && chunk.Value[x - 1, y - 1].type != tile.type) corners["NW"] = true;
                        }
                        else if (chunks.ContainsKey(chunk.Key - 1)) {
                            if (y < 15 && chunks[chunk.Key - 1][15, y + 1].type != tile.type) corners["SW"] = true;
                            if (y > 0 && chunks[chunk.Key - 1][15, y - 1].type != tile.type) corners["NW"] = true;
                        }

                        string textureName = "";
                        if (sides.Values.All(a => !a)) {
                            if (corners.Values.All(a => !a)) textureName = "NESW";
                            else if (corners.Values.Where(a => a).Count() > 1) textureName = "Single";
                            else { textureName = "In" + corners.Where(a => a.Value).First().Key; }
                        }
                        else if (sides.Values.All(b => b)) textureName = "Single";
                        else {
                            foreach (KeyValuePair<char, bool> side in sides.Where(a => a.Value)) { textureName += side.Key.ToString(); }
                            foreach (KeyValuePair<string, bool> corner in corners) {
                                if (corner.Value) {
                                    bool hasSide = false;
                                    foreach (KeyValuePair<char, bool> side in sides) {
                                        if (side.Value) {
                                            if (corner.Key.Contains(side.Key)) hasSide = true;
                                        }
                                    }
                                    if (!hasSide) textureName = "Single";
                                }
                            }
                        }


                        chunk.Value[x, y].texture = game.textures[$"World\\{chunk.Value[x, y].type}\\{textureName}"];
                    }
                }
            }
        }

        public override void Update(GameTime gameTime) {
            GenerateChunks(currentChunk - 2, currentChunk + 2);
        }

        public void UpdateHitBox() {
            hitBox = new List<Rectangle>();
            foreach (KeyValuePair<int, Tile[,]> chunk in chunks) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        if (chunk.Value[x, y].type != null) {
                            hitBox.Add(new Rectangle(x * Tile.width + chunk.Key * chunkWidth, y * Tile.height, Tile.width, Tile.height));
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            // Draw backgrounds
            float scale = 1.4f;
            float parallaxStrength = 1;
            foreach (Texture2D background in backgrounds) {
                float width = background.Width * scale;
                float offset = -scene.camera.center.X * (1 - parallaxStrength);
                Vector2 pos = new Vector2(scene.camera.center.X - offset + width * (float)Math.Floor(offset / width), scene.camera.center.Y);
                for (int i = -1; i <= 0; i++) {
                    Vector2 drawPos = new Vector2(pos.X - i * width, pos.Y);
                    batch.Draw(background, drawPos, null, Color.White, 0f, new Vector2(background.Width / 2, background.Height / 2), scale, SpriteEffects.None, 0f);
                }
                parallaxStrength += 0.2f;
            }

            // Draw chunks
            for (int i = currentChunk - 1; i <= currentChunk + 1; i++) {
                if (!chunks.ContainsKey(i)) continue;
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunks[i][x, y];
                        if (tile.texture != null)
                            batch.Draw(tile.texture, new Vector2((float)x * Tile.width + i * chunkWidth, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                    }
                }

            }
        }
    }
}
