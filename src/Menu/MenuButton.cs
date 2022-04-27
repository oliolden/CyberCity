using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CyberCity {
    internal class MenuButton : GameObject {
        public Texture2D texture;
        public string text;
        public Action action;
        public float scaleFactor;
        public Vector2 textScale;
        private MouseState mouseState;

        public MenuButton(Scene scene, Action action, string text, string textureName) : base(scene) {
            this.action = action;
            this.text = text;
            texture = Game1.textures[textureName];
            textScale = Vector2.One;
        }

        public override void Update(GameTime gameTime) {
            MouseState prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            UpdateHitbox();
            if (hitBox[0].Contains(scene.camera.GetMousePos())) {
                scaleFactor = 1.2f;
                if (prevMouseState.LeftButton == ButtonState.Pressed) {
                    color = Color.Gray;
                    if (mouseState.LeftButton == ButtonState.Released) action();
                }
                else color = Color.White;
            }
            else { scaleFactor = 1; color = Color.White; }
        }

        private void UpdateHitbox() {
            hitBox = new List<Rectangle> { new Rectangle((position - texture.Bounds.Size.ToVector2() * scale * scaleFactor / 2).ToPoint(), (texture.Bounds.Size.ToVector2() * scale * scaleFactor).ToPoint()) };
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(texture, position, null, color, rotation, texture.Bounds.Size.ToVector2() / 2, scale * scaleFactor, spriteEffects, layer);
            batch.DrawString(Game1.fonts["Fonts\\Minecraft"], text, position, color, rotation, Game1.fonts["Fonts\\Minecraft"].MeasureString(text) / 2, textScale * scaleFactor, spriteEffects, layer + 0.1f);
            base.Draw(batch, gameTime);
        }
    }
}
