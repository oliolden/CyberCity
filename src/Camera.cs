using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CyberCity {
    internal class Camera {
        private Game1 game;
        public Vector2 center;
        public float rotation;
        public float zoom;
        public Viewport viewport;
        public Matrix matrix;
        private float windowScale;
        private GameObject centerObject;
        public Vector2 GetPosition() { return new Vector2(-matrix.Translation.X, -matrix.Translation.Y) / zoom / windowScale; }
        public int GetWidth() { return (int)(viewport.Width / zoom); }
        public int GetHeight() { return (int)(viewport.Height / zoom); }
        public Vector2 GetMousePos() { return Mouse.GetState().Position.ToVector2() / zoom / windowScale + GetPosition(); }

        public Camera(Game1 myGame) {
            game = myGame;
            viewport = game.GraphicsDevice.Viewport;
            center = viewport.Bounds.Center.ToVector2();
            rotation = 0f;
            zoom = 1;
        }

        public void CenterOn(GameObject gameObject) {
            centerObject = gameObject;
        }

        public void Update(GameTime gameTime) {
            if (centerObject != null) {
                center += (centerObject.position + (Vector2.UnitY * -80) - center) * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            windowScale = MathF.Max(viewport.Width / 800f, viewport.Height / 480f);
            viewport = game.GraphicsDevice.Viewport;
            matrix =
                Matrix.CreateTranslation(-center.X, -center.Y, 0) *
                Matrix.CreateRotationZ(rotation) * 
                Matrix.CreateScale(zoom, zoom, 0) *
                Matrix.CreateScale(windowScale, windowScale, 0) *
                Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 0);
        }
    }
}
