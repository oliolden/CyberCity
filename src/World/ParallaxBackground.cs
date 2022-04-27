using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CyberCity {
    internal class ParallaxBackground {
        public static Dictionary<string, List<Texture2D>> backgrounds = new Dictionary<string, List<Texture2D>>();
        Camera camera;
        public ParallaxBackground(Camera camera) { this.camera = camera; }
        public void Draw(SpriteBatch batch, string backgroundId) {
            float scale = 1.5f;
            float parallaxStrength = 1;
            float bgLayer = -5f;
            foreach (Texture2D background in backgrounds[backgroundId]) {
                float width = background.Width * scale;
                float offset = -camera.center.X * (1 - parallaxStrength);
                Vector2 pos = new Vector2(camera.center.X - offset + width * (float)Math.Floor(offset / width), camera.center.Y);
                for (int i = -1; i <= 0; i++) {
                    Vector2 drawPos = new Vector2(pos.X - i * width, pos.Y);
                    batch.Draw(background, drawPos, null, Color.White, 0f, new Vector2(background.Width / 2, background.Height / 2), scale, SpriteEffects.None, bgLayer);
                }
                parallaxStrength += 0.2f;
                bgLayer += 0.1f;
            }
        }
        public void Draw(SpriteBatch batch, string backgroundId, Vector2 position) {
            float scale = 1.5f;
            float parallaxStrength = 1;
            float bgLayer = -5f;
            foreach (Texture2D background in backgrounds[backgroundId]) {
                float width = background.Width * scale;
                float offset = -position.X * (1 - parallaxStrength);
                Vector2 pos = new Vector2(position.X - offset + width * (float)Math.Floor(offset / width), camera.center.Y);
                for (int i = -1; i <= 0; i++) {
                    Vector2 drawPos = new Vector2(pos.X - i * width, pos.Y) - Vector2.UnitX * position.X;
                    batch.Draw(background, drawPos, null, Color.White, 0f, new Vector2(background.Width / 2, background.Height / 2), scale, SpriteEffects.None, bgLayer);
                }
                parallaxStrength += 0.2f;
                bgLayer += 0.1f;
            }
        }
    }

    internal class ParallaxBackgroundObject : GameObject {
        private ParallaxBackground background;
        public string backgroundId;
        float speed;
        public ParallaxBackgroundObject(Scene scene, string backgroundId, float speed) : base(scene) {
            background = new ParallaxBackground(scene.camera);
            this.backgroundId = backgroundId;
            this.speed = speed;
        }

        public override void Update(GameTime gameTime) {
            position.X += speed + (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            background.Draw(batch, backgroundId, position);
        }
    }
}
