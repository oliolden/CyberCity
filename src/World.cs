using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class World : GameObject {
        public List<List<Tile>> tiles;

        public World(Game1 myGame) {
            game = myGame;
            tiles = new List<List<Tile>>();
        }

        public void UpdateTileTextures() {
            for (int x = 0; x < tiles.Count; x++) {
                for (int y = 0; y < tiles[x].Count; y++) {
                    if (tiles[x][y].type == null) { break; }
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

                    if (left && right && bottom && top) { textureName = "Single"; break; }
                    if (!left && !right && !bottom && !top) { textureName = "Center"; break; }
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
    }
}
