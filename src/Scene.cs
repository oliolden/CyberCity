using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Scene {
        private Game1 _game;
        public List<GameObject> gameObjects;
        public Scene(Game1 myGame) {
            _game = myGame;
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
