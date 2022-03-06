using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Scene {
        internal Game1 game;
        public List<GameObject> gameObjects;
        public Color backgroundColor;
        public Scene(Game1 myGame) {
            game = myGame;
            gameObjects = new List<GameObject>();
        }

        public void Update(GameTime gameTime) {
            foreach (GameObject obj in gameObjects) {
                if (obj.enabled)
                    obj.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach (GameObject obj in gameObjects) {
                if (obj.enabled)
                    obj.Draw(batch, gameTime);
            }
        }
    }
}
