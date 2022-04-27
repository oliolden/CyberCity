using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;

namespace CyberCity {
    internal class MenuButton : GameObject {
        public string textureName;
        public string text;
        public Action action;
        public MenuButton(Scene scene, Action action, string text, string textureName) : base(scene) {
            this.action = action;
            this.text = text;
            this.textureName = textureName;
        }

        public override void Update(GameTime gameTime) {
            if ((scene.camera.GetMousePos() - position).Length() < 20) {
                scale = Vector2.One * 1.5f;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed) action();
            }
            else scale = Vector2.One;
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            Texture2D texture = Game1.textures[textureName];
            batch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, layer);
            base.Draw(batch, gameTime);
        }
    }
}
