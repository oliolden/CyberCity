using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class World : GameObject {
        public Tile[,] tiles;
        public Texture2D[] backgrounds;
        private List<Tile[,]> _posChunks;
        private List<Tile[,]> _negChunks;
        private static int chunkWidth = 32 * 16, chunkHeight = 32 * 16;

        private Tile[,] GetChunk(int index) {
            if (index >= 0) return _posChunks[index];
            else return _negChunks[Math.Abs(index) - 1];
        }
        private void AddChunk(int index) {
            if (index >= 0) {
                while (_posChunks.Count <= index) {
                    _posChunks.Add(new Tile[16, 16]);
                }
            }
            else {
                while (_negChunks.Count < Math.Abs(index)) {
                    _negChunks.Add(new Tile[16, 16]);

                }
            }
        }

        private void SetTile(int index, int x, int y, Tile tile) {
            if (index >= 0) _posChunks[index][x, y] = tile;
            else _negChunks[Math.Abs(index) - 1][x, y] = tile;
        }
        private void SetChunk(int index, Tile[,] chunk) {
            if (index >= 0) _posChunks[index] = chunk;
            else _negChunks[Math.Abs(index) - 1] = chunk;
        }

        public World(Scene myScene) : base(myScene) { _posChunks = new List<Tile[,]>(); _negChunks = new List<Tile[,]>(); GenerateChunks(-3, 3); }

        public void GenerateChunks(int start, int end) {
            Random rnd = new Random();
            AddChunk(start);
            AddChunk(end);
            for (int i = start; i <= end; i++) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        if (y >= 14) { SetTile(i, x, y, new Tile(1)); }
                        else SetTile(i, x, y, new Tile(0));
                    }
                }
            }
            UpdateTileTextures();
            UpdateHitBox();
        }

        public void UpdateTileTextures() {
            for (int i = -_negChunks.Count + 1; i < _posChunks.Count; i++) {
                Tile[,] chunk = GetChunk(i);
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunk[x, y];
                        if (tile.type == null) { tile.texture = null; continue; }
                        Dictionary<char, bool> sides = new Dictionary<char, bool> { { 'N', false }, { 'E', false }, { 'S', false }, { 'W', false } };

                        if (y > 0 && chunk[x, y - 1].type != tile.type) sides['N'] = true;
                        if (x < 16 - 1 && chunk[x + 1, y].type != tile.type) sides['E'] = true;
                        if (y < 16 - 1 && chunk[x, y + 1].type != tile.type) sides['S'] = true;
                        if (x > 0 && chunk[x - 1, y].type != tile.type) sides['W'] = true;


                        Dictionary<string, bool> corners = new Dictionary<string, bool> { { "NE", false }, { "SE", false }, { "SW", false }, { "NW", false } };

                        if (x < 16 - 1 && y > 0 && chunk[x + 1, y - 1].type != tile.type) corners["NE"] = true;
                        if (x < 16 - 1 && y < 16 - 1 && chunk[x + 1, y + 1].type != tile.type) corners["SE"] = true;
                        if (x > 0 && y < 16 - 1 && chunk[x - 1, y + 1].type != tile.type) corners["SW"] = true;
                        if (x > 0 && y > 0 && chunk[x - 1, y - 1].type != tile.type) corners["NW"] = true;

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


                        chunk[x, y].texture = game.textures[$"World\\{chunk[x, y].type}\\{textureName}"];
                    }
                }
                SetChunk(i, chunk);
            }
        }
        /*
        public void UpdateTileTextures() {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Tile tile = tiles[x, y];
                    if (tile.type == null) { tile.texture = null; continue; }
                    Dictionary<char, bool> sides = new Dictionary<char, bool> { { 'N', false }, { 'E', false }, { 'S', false }, { 'W', false } };

                    if (y > 0 && tiles[x, y - 1].type != tile.type) sides['N'] = true;
                    if (x < width - 1 && tiles[x + 1, y].type != tile.type) sides['E'] = true;
                    if (y < height - 1 && tiles[x, y + 1].type != tile.type) sides['S'] = true;
                    if (x > 0 && tiles[x - 1, y].type != tile.type) sides['W'] = true;


                    Dictionary<string, bool> corners = new Dictionary<string, bool> { { "NE", false }, { "SE", false }, { "SW", false }, { "NW", false } };

                    if (x < width - 1 && y > 0 &&tiles[x + 1, y - 1].type != tile.type) corners["NE"] = true;
                    if (x < width - 1 && y < height - 1 && tiles[x + 1, y + 1].type != tile.type) corners["SE"] = true;
                    if (x > 0 && y < height - 1 && tiles[x - 1, y + 1].type != tile.type) corners["SW"] = true;
                    if (x > 0 && y > 0 && tiles[x - 1, y - 1].type != tile.type) corners["NW"] = true;

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


                    tiles[x, y].texture = game.textures[$"World\\{tiles[x, y].type}\\{textureName}"];
                }
            }
        }
        */

        public override void Update(GameTime gameTime) {
            int currentChunk = (int)((scene.camera.center.X - chunkWidth / 2) / chunkWidth);
            if (currentChunk + 3 > _posChunks.Count) GenerateChunks(_posChunks.Count, currentChunk + 3);
            if (currentChunk - 3 < -_negChunks.Count) GenerateChunks(-_negChunks.Count - 1, currentChunk - 3);
        }

        public void UpdateHitBox() {
            hitBox = new List<Rectangle>();
            for (int i = -_negChunks.Count + 1; i < _posChunks.Count; i++) {
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        if (GetChunk(i)[x, y].type != null) {
                            hitBox.Add(new Rectangle(x * Tile.width + i * chunkWidth, y * Tile.height, Tile.width, Tile.height));
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            // Draw backgrounds
            float dist = 20;
            foreach (Texture2D background in backgrounds) {
                Point size = new Point((int)(background.Width * 2), (int)(background.Height * 2));
                Vector2 center = scene.camera.center / dist;
                int index = (int)(Math.Floor(scene.camera.center.X / size.X));
                for (int i = index - 1; i <= index + 1; i++) {
                    batch.Draw(background, new Vector2(center.X + i * size.X, center.Y), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                }
                dist = dist / 1.2f;
            }

            // Draw chunks
            int currentChunk = (int)(Math.Floor(scene.camera.center.X / chunkWidth));
            for (int i = currentChunk - 1; i <= currentChunk + 1; i++) {
                Tile[,] chunk = GetChunk(i);
                for (int x = 0; x < 16; x++) {
                    for (int y = 0; y < 16; y++) {
                        Tile tile = chunk[x, y];
                        if (tile.texture != null)
                            batch.Draw(tile.texture, new Vector2((float)x * Tile.width + i * chunkWidth, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                    }
                }

            }

            // Draw tiles
            /*
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Tile tile = tiles[x, y];
                    if (tile.texture != null)
                        batch.Draw(tile.texture, new Vector2((float)(x - 1) * Tile.width, (float)(y - 1) * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                }
            }
            */
        }
    }
}
