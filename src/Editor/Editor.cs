using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace CyberCity {
    internal class Editor : GameObject {
        private KeyboardState keyboardState;
        private MouseState mouseState;

        private Structure structure;

        int equippedTile;
        Tile[] inventory = { new Tile("metal"), new Tile("stone"), new Tile("stone", "grass"), };

        public Editor(Scene scene) : base(scene) { structure = new Structure(5, 5); }

        public override void Update(GameTime gameTime) {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState prevKeyboardState = keyboardState;
            MouseState prevMouseState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up)) { equippedTile++; if (equippedTile >= inventory.Length) equippedTile = 0; }
            if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down)) { equippedTile--; if (equippedTile < 0) equippedTile = inventory.Length - 1; }

            if (mouseState.LeftButton == ButtonState.Pressed) {
                Vector2 pos = scene.camera.GetMousePos();
                SetTile((int)Math.Floor(pos.X / Tile.width), (int)Math.Floor(pos.Y / Tile.height), inventory[equippedTile]);
            }
            if (mouseState.RightButton == ButtonState.Pressed) {
                Vector2 pos = scene.camera.GetMousePos();
                SetTile((int)Math.Floor(pos.X / Tile.width), (int)Math.Floor(pos.Y / Tile.height), new Tile("air"));
            }

            float speed = 100f;

            if (keyboardState.IsKeyDown(Keys.W))
                position.Y -= speed * dt;
            if (keyboardState.IsKeyDown(Keys.A))
                position.X -= speed * dt;
            if (keyboardState.IsKeyDown(Keys.S))
                position.Y += speed * dt;
            if (keyboardState.IsKeyDown(Keys.D))
                position.X += speed * dt;
            if (keyboardState.IsKeyDown(Keys.LeftControl)) {
                if (keyboardState.IsKeyDown(Keys.S))
                    structure.Save("test");
                if (keyboardState.IsKeyDown(Keys.L))
                    structure = Structure.Load("test");
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            for (int x = 0; x < structure.width; x++) {
                for (int y = 0; y < structure.height; y++) {
                    // Foreground
                    Tile tile = structure.tiles[x, y];
                    if (tile.background != null)
                        batch.Draw(Game1.textures["World\\Tiles\\background\\" + tile.background], new Vector2((float)x * Tile.width, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0 + 0.1f);
                    if (tile.texture != null)
                        batch.Draw(tile.texture, new Vector2((float)x * Tile.width, (float)y * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0 + 0.2f);
                }
            }
            game.DrawRectangle(batch, new Rectangle(0, 0, 1, 1), 2, Color.Red, 1f);
        }

        public void UpdateTileTexture(int x, int y) {
            Tile tile = structure.tiles[x, y];
            if (!TileType.types[tile.id].visible) { tile.texture = null; return; }
            bool[] check = { false, false, false, false, false, false, false, false, };
            bool[] bools;

            bool CheckTile(Point offset) {
                return CheckTileInfo(offset).id == tile.id;
            }

            Tile CheckTileInfo(Point offset) {
                if (y + offset.Y < 0 || y + offset.Y >= structure.height) return new Tile("air");
                if (x + offset.X >= 0 && x + offset.X < structure.width) return structure.tiles[x + offset.X, y + offset.Y];
                else return new Tile("air");
            }

            string textureName = "";

            for (int i = 0; i < World.offsets.Length; i++) {
                check[i] = CheckTile(World.offsets[i]);
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

            if (bools[1] && Game1.textures.ContainsKey(World.texturePath + structure.tiles[x, y - 1].GetPath() + textureName + "_1") && !CheckTile(new Point(0, -2))) {
                structure.tiles[x, y].texture = Game1.textures[World.texturePath + structure.tiles[x, y - 1].GetPath() + textureName + "_1"];
            }
            else if (bools[3] && CheckTileInfo(World.offsets[3]).variant != tile.variant && Game1.textures.ContainsKey(World.texturePath + CheckTileInfo(World.offsets[4]).GetPath() + textureName + "_R")) {
                structure.tiles[x, y].texture = Game1.textures[World.texturePath + CheckTileInfo(World.offsets[4]).GetPath() + textureName + "_R"];
            }
            else if (bools[4] && CheckTileInfo(World.offsets[4]).variant != tile.variant && Game1.textures.ContainsKey(World.texturePath + CheckTileInfo(World.offsets[4]).GetPath() + textureName + "_L")) {
                structure.tiles[x, y].texture = Game1.textures[World.texturePath + CheckTileInfo(World.offsets[4]).GetPath() + textureName + "_L"];
            }
            else {
                if (Game1.textures.Keys.Contains(World.texturePath + tile.GetPath() + textureName)) {
                    structure.tiles[x, y].texture = Game1.textures[World.texturePath + tile.GetPath() + textureName];
                }
                else {
                    if (Game1.textures.Keys.Contains(World.texturePath + tile.id + textureName))
                        structure.tiles[x, y].texture = Game1.textures[World.texturePath + tile.id + textureName];
                    else if (Game1.textures.Keys.Contains(World.texturePath + tile.GetPath() + "\\00000000"))
                        structure.tiles[x, y].texture = Game1.textures[World.texturePath + tile.GetPath() + "\\00000000"];
                    else
                        structure.tiles[x, y].texture = Game1.textures[World.texturePath + tile.id + "\\00000000"];
                }
            }
        }

        public void SetTile(int x, int y, Tile tile) {
            if (y < 0 || y >= structure.height || x < 0 || x >= structure.width) return;
            if (!structure.tiles[x, y].Equals(tile)) {
                structure.tiles[x, y] = new Tile(tile.id, tile.variant);
                UpdateTileTexture(x, y);
                foreach (Point offset in World.offsets) {
                    if (y + offset.Y < 0 || y + offset.Y >= structure.height) continue;
                    if (x + offset.X >= 0 && x + offset.X < structure.width) UpdateTileTexture(x + offset.X, y + offset.Y);
                }
                if (y + 2 < structure.height) UpdateTileTexture(x, y + 2);
            }
        }
    }
}
