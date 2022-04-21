using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Animation {
        public Texture2D texture;
        public int currentFrame;
        public int frameCount;
        public float frameTime;
        public int frameWidth { get { return texture.Width / frameCount; } }
        public int frameHeight { get { return texture.Height; } }
        public Rectangle frameRect { get { return new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); } }
        public bool isLoop;

        public Animation(Texture2D _texture, int _frameCount, bool _isLoop, float _frameTime = 0.2f) {
            texture = _texture;
            frameCount = _frameCount;
            frameTime = _frameTime;
            isLoop = _isLoop;
        }
    }
}
