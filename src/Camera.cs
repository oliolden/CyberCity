using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Camera {
        Game1 game;
        public Vector2 center;
        public Vector2 position { get { return new Vector2(-matrix.Translation.X, -matrix.Translation.Y) / zoom; } set { } }
        public float rotation;
        public float zoom;
        public Viewport viewport;
        public Matrix matrix;

        public Camera(Game1 myGame) {
            game = myGame;
            viewport = game.GraphicsDevice.Viewport;
            center = viewport.Bounds.Center.ToVector2();
            rotation = 0f;
            zoom = 1;
        }

        public void Update() {
            viewport = game.GraphicsDevice.Viewport;
            matrix = Matrix.CreateTranslation(-center.X, -center.Y, 0) * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(zoom, zoom, 0) * Matrix.CreateTranslation(viewport.Width / 2, viewport.Height / 2, 0);
        }
    }
}
