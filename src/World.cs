using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class World : GameObject {
        //public List<List<Tile>> tiles;
        public Tile[,] tiles;

        public World(Game1 myGame) : base(myGame) { }

        public void UpdateTileTextures() {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Tile tile = tiles[x, y];
                    if (tile.type == null) { continue; }
                    bool[] sides = new bool[4]; // 0: Left, 1: Right, 2: Top, 3: Bottom
                    // Check left
                    if (x > 0 && tiles[x - 1, y].type == tile.type) {
                        sides[0] = false;
                    }
                    else { sides[0] = true; }
                    // Check right
                    if (x < width - 1 && tiles[x + 1, y].type == tile.type) {
                        sides[1] = false;
                    }
                    else { sides[1] = true; }
                    // Check bottom[3]
                    if (y < height - 1 && tiles[x, y + 1].type == tile.type) {
                        sides[3] = false;
                    }
                    else { sides[3] = true; }
                    // Check top[2]
                    if (y > 0 && tiles[x, y - 1].type == tile.type) {
                        sides[2] = false;
                    }
                    else { sides[2] = true; }

                    string textureName = "";
                    if (sides[2]) { textureName += "Top"; }
                    if (sides[3]) { textureName += "Bottom"; }
                    if (sides[0]) { textureName += "Left"; }
                    if (sides[1]) { textureName += "Right"; }
                    if (sides[0] && sides[1] && sides[3] && sides[2]) { textureName = "Single"; }
                    else if (!sides[0] && !sides[1] && !sides[3] && !sides[2]) { textureName = "Center"; }

                    tiles[x, y].texture = game.textures[$"World\\{tiles[x, y].type}\\{textureName}"];
                }
            }
        }
        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (tiles[x, y].texture != null)
                        batch.Draw(tiles[x, y].texture, new Vector2((float)(x - 1) * Tile.width, (float)(y - 1) * Tile.height), Color.White);
                }
            }
        }

        /*
        public void UpdateTileTextures() {
            for (int x = 0; x < tiles.Count; x++) {
                for (int y = 0; y < tiles[x].Count; y++) {
                    if (tiles[x][y].type == null) { continue; }
                    string textureName = "";
                    bool top, bottom, left, right;
                    // Check left
                    if (x > 0 && tiles[x - 1][y].type == tiles[x][y].type) {
                        left = false;
                    } else { left = true; }
                    // Check right
                    if (x < tiles.Count - 1 && tiles[x + 1][y].type == tiles[x][y].type) {
                        right = false;
                    }
                    else { right = true; }
                    // Check bottom
                    if (y < tiles[x].Count - 1 && tiles[x][y + 1].type == tiles[x][y].type) {
                        bottom = false;
                    }
                    else { bottom = true; }
                    // Check top
                    if (y > 0 && tiles[x][y - 1].type == tiles[x][y].type) {
                        top = false;
                    }
                    else { top = true; }

                    if (left && right && bottom && top) { textureName = "Single"; continue; }
                    if (!left && !right && !bottom && !top) { textureName = "Center"; continue; }
                    if (top) { textureName += "Top"; }
                    if (bottom) { textureName += "Bottom"; }
                    if (left) { textureName += "Left"; }
                    if (right) { textureName += "Right"; }

                    tiles[x][y].texture = game.Content.Load<Texture2D>($"World/{tiles[x][y].type}/{textureName}");
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {

            for (int x = 0; x < tiles.Count; x++) {
                for (int y = 0; y < tiles[x].Count; y++) {
                    if (tiles[x][y].type != null)
                        batch.Draw(tiles[x][y].texture, new Vector2((float)x * Tile.width, (float)y * Tile.height), Color.White);
                }
            }
        }
        */
    }
}
