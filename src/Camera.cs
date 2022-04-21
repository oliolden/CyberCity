using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CyberCity {
    internal class Camera {
        private Game1 game;
        public Vector2 center;
        public Vector2 position { get { return new Vector2(-matrix.Translation.X, -matrix.Translation.Y) / zoom; } set { } }
        public float rotation;
        public float zoom;
        public Viewport viewport;
        public Matrix matrix;
        public int width { get { return (int)(viewport.Width / zoom); } set { } }
        public int height { get { return (int)(viewport.Height / zoom); } set { } }
        public Vector2 mousePosition { get { return Mouse.GetState().Position.ToVector2() / zoom + position; } set { } }

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
