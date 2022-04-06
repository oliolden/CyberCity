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
        public List<Rectangle> hitBox;
        public bool collisions;
        public float layer;

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
            collisions = true;
            layer = 1f;
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
            foreach (GameObject gameObject in scene.objects.Values) {
                if (gameObject != this && gameObject.collisions && Collides(gameObject)) {
                    return true;
                }
            }
            return false;
        }

        public void DrawHitBox(SpriteBatch batch) {
            if (hitBox == null) return;
            Color color = collisions ? new Color(0, 0, 255, 1) : new Color(0, 255, 0, 1);
            foreach (Rectangle box in hitBox) {
                game.DrawRectangle(batch, box, 1, color, 999f);
                //batch.Draw(Game1.textures["Blank"], box, null, color, 0, Vector2.Zero, SpriteEffects.None, 999f);
            }
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch batch, GameTime gameTime) { if (((GameScene)scene).devTools) DrawHitBox(batch); }
    }
}
