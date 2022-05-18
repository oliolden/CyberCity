using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Scene {
        internal Game1 game;
        public Dictionary<string, GameObject> objects;
        public Color backgroundColor;
        public Camera camera;
        public Scene(Game1 myGame) {
            game = myGame;
            objects = new Dictionary<string, GameObject>();
            camera = new Camera(game);
        }

        public virtual void Update(GameTime gameTime) {
            foreach (GameObject obj in objects.Values) {
                if (obj.enabled)
                    obj.Update(gameTime);
            }
            camera.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach (GameObject obj in objects.Values) {
                if (obj.enabled)
                    obj.Draw(batch, gameTime);
            }
        }
    }
}
