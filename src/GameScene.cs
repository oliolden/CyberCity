using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CyberCity {
    internal class GameScene : Scene {

        public GameScene(Game1 myGame) : base(myGame) {
            backgroundColor = new Color(0xB3AFBB);
            objects.Add("World", new World(this));
            ((World)objects["World"]).backgrounds = new Texture2D[] {
                game.textures["World\\Background\\0"], game.textures["World\\Background\\1"], game.textures["World\\Background\\2"], game.textures["World\\Background\\3"], game.textures["World\\Background\\4"],
            };
            objects.Add("Player", new Player(this));
        }

        public override void Update(GameTime gameTime) {

            camera.zoom += ((camera.viewport.Height/240 * (float)Math.Pow(0.9, ((Player)objects["Player"]).velocity.Length()/100)) - camera.zoom) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //camera.zoom = camera.viewport.Height / (480f * 4f);
            Vector2 camDist = (objects["Player"].position + (Vector2.UnitY * -48) - camera.center);
            camera.center += camDist * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
