using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal abstract class GameObject {
        protected Game1 game;
        protected Scene scene;
        public Vector2 position;
        public float rotation;
        public Vector2 scale;
        public Vector2 origin;
        public Color color;
        public SpriteEffects spriteEffects;
        public bool enabled;
        public Rectangle[] hitBox;
        public bool collideable;

        public GameObject(Scene myScene) {
            scene = myScene;
            game = scene.game;
            position = Vector2.Zero;
            rotation = 0f;
            scale = Vector2.One;
            origin = Vector2.Zero;
            color = Color.White;
            spriteEffects = SpriteEffects.None;
            enabled = true;
            collideable = false;
        }

        public bool Collides(GameObject target) {
            if (target == null || !target.enabled || !enabled) return false;
            foreach (Rectangle myRect in hitBox) {
                foreach (Rectangle targetRect in target.hitBox) {
                    if(myRect.Intersects(targetRect)) return true;
                }
            }
            return false;
        }

        public bool CollidesAny() {
            foreach (GameObject gameObject in scene.gameObjects) {
                if (gameObject != this && Collides(gameObject)) {
                    return true;
                }
            }
            return false;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch batch, GameTime gameTime) { }
    }
}
