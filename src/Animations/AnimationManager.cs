using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class AnimationManager {
        public Animation animation { get; private set; }
        public bool isPlaying;
        private double _timer;

        public AnimationManager(Animation _animation) {
            animation = _animation;
        }

        public void Play(Animation _animation = null) {
            if (_animation == null) {_animation = animation;}
            if (!isPlaying && animation == _animation) { animation.currentFrame = 0; }
            isPlaying = true;
            if (animation == _animation) { return; }

            animation = _animation;
            animation.currentFrame = 0;
            _timer = 0f;
        }

        public void ShowFrame(int frame) {
            isPlaying = false;
            animation.currentFrame = frame;
        }

        public void Update(GameTime gameTime) {
            if (isPlaying) {
                _timer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer > animation.frameTime) {
                    animation.currentFrame++; if (animation.currentFrame >= animation.frameCount) {
                        if (animation.isLoop) { animation.currentFrame = 0; }
                        else { animation.currentFrame = animation.frameCount - 1; isPlaying = false; }
                    }
                    _timer = 0;
                }
            }
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layer) {
            batch.Draw(animation.texture, position, animation.frameRect, color, rotation, origin, scale, effects, layer);
        }
    }
}
