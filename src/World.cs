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

        public World(Scene myScene) : base(myScene) { }

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

        public void UpdateHitBox() {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            hitBox = new Rectangle[tiles.Length];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (tiles[x, y].type != null) {
                        hitBox[y * width + x] = new Rectangle((x - 1) * Tile.width, (y - 1) * Tile.height, Tile.width, Tile.height);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Tile tile = tiles[x, y];
                    if (tile.texture != null)
                        batch.Draw(tile.texture, new Vector2((float)(x - 1) * Tile.width, (float)(y - 1) * Tile.height), null, tile.color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
