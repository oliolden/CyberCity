﻿using System;
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

        public World(Scene myScene, int seed = 0) : base(myScene) { random = new Random(seed); chunks = new Dictionary<int, Tile[,]>(); GenerateChunks(-3, 3); layer = 0f; }

        public void GenerateChunks(int start, int end) {
            bool hasGenerated = false;
            for (int i = start; i <= end; i++) {
                if (chunks.ContainsKey(i)) continue;
                Tile[,] chunk = new Tile[16, 16];
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        if (y >= 12) { chunk[x, y] = new Tile("IndustrialTile01"); }
                        else chunk[x, y] = new Tile("Air");
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

        public Tile GetTile(float x, float y) {
            int chunk = (int)Math.Floor(x / chunkWidth);
            return chunks[chunk][(int)Math.Floor(x - chunk * chunkWidth) / Tile.width, (int)Math.Floor(y) / Tile.height];
        }

        //public void UpdateTileTextures() {
        //    foreach (KeyValuePair<int, Tile[,]> chunk in chunks) {
        //        for (int x = 0; x < 16; x++) {
        //            for (int y = 0; y < 16; y++) {
        //                Tile tile = chunk.Value[x, y];
        //                if (!TileType.types[tile.id].visible) { tile.textureName = null; continue; }
        //                bool[] corners = { false, false, false, false };

        //                bool CheckTile(int xOffset, int yOffset) {
        //                    if (y + yOffset < 0 || y + yOffset >= 16) return false;
        //                    if (x + xOffset >= 0 && x + xOffset < 16) return chunk.Value[x + xOffset, y + yOffset].id == tile.id;
        //                    else {
        //                        int testChunk = chunk.Key + (x + xOffset) / 16;
        //                        if (chunks.ContainsKey(testChunk)) { return chunks[testChunk][(x + xOffset) % 16, y].id == tile.id; }
        //                        else return false;
        //                    }
        //                }

        //                if (CheckTile(0, -1)) { corners[0] = true; corners[1] = true; }
        //                if (CheckTile(1, 0)) { corners[1] = true; corners[3] = true; }
        //                if (CheckTile(0, 1)) { corners[2] = true; corners[3] = true; }
        //                if (CheckTile(-1, 0)) { corners[2] = true; corners[0] = true; }

        //                if (!corners[0]) { corners[0] = CheckTile(-1, -1); }
        //                if (!corners[1]) { corners[1] = CheckTile(1, -1); }
        //                if (!corners[2]) { corners[2] = CheckTile(-1, 1); }
        //                if (!corners[3]) { corners[3] = CheckTile(1, 1); }

        //                string textureName = "";

        //                foreach (bool corner in corners) {
        //                    textureName += corner ? 1 : 0;
        //                }

        //                chunk.Value[x, y].textureName = $"World\\{chunk.Value[x, y].id}\\{textureName}";
        //            }
        //        }
        //    }
        //}

        public void UpdateTileTextures() {
            foreach (KeyValuePair<int, Tile[,]> chunk in chunks) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunk.Value[x, y];
                        if (!TileType.types[tile.id].visible) { tile.textureName = null; continue; }
                        Dictionary<char, bool> sides = new Dictionary<char, bool> { { 'N', false }, { 'E', false }, { 'S', false }, { 'W', false } };

                        if (y > 0 && chunk.Value[x, y - 1].id != tile.id) sides['N'] = true;
                        if (y < 15 && chunk.Value[x, y + 1].id != tile.id) sides['S'] = true;
                        if (x < 15) { if (chunk.Value[x + 1, y].id != tile.id) sides['E'] = true; }
                        else if (chunks.ContainsKey(chunk.Key + 1)) { if (chunks[chunk.Key + 1][0, y].id != tile.id) sides['E'] = true; }
                        if (x > 0) { if (chunk.Value[x - 1, y].id != tile.id) sides['W'] = true; }
                        else if (chunks.ContainsKey(chunk.Key - 1)) { if (chunks[chunk.Key - 1][15, y].id != tile.id) sides['W'] = true; }

                        Dictionary<string, bool> corners = new Dictionary<string, bool> { { "NE", false }, { "SE", false }, { "SW", false }, { "NW", false } };
                        if (x < 15) {
                            if (y > 0 && chunk.Value[x + 1, y - 1].id != tile.id) corners["NE"] = true;
                            if (y < 15 && chunk.Value[x + 1, y + 1].id != tile.id) corners["SE"] = true;
                        }
                        else if (chunks.ContainsKey(chunk.Key + 1)) {
                            if (y > 0 && chunks[chunk.Key + 1][0, y - 1].id != tile.id) corners["NE"] = true;
                            if (y < 15 && chunks[chunk.Key + 1][0, y + 1].id != tile.id) corners["SE"] = true;
                        }
                        if (x > 0) {
                            if (y < 15 && chunk.Value[x - 1, y + 1].id != tile.id) corners["SW"] = true;
                            if (y > 0 && chunk.Value[x - 1, y - 1].id != tile.id) corners["NW"] = true;
                        }
                        else if (chunks.ContainsKey(chunk.Key - 1)) {
                            if (y < 15 && chunks[chunk.Key - 1][15, y + 1].id != tile.id) corners["SW"] = true;
                            if (y > 0 && chunks[chunk.Key - 1][15, y - 1].id != tile.id) corners["NW"] = true;
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


                        chunk.Value[x, y].textureName = $"World\\{chunk.Value[x, y].id}\\{textureName}";
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
                        if (TileType.types[chunk.Value[x, y].id].collideable) {
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
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunks[i][x, y];
                        if (tile.textureName != null)
                            batch.Draw(game.textures[tile.textureName], new Vector2((float)x * Tile.width + i * chunkWidth, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + 0.1f);
                    }
                }

            }
            base.Draw(batch, gameTime);
        }
    }
}
