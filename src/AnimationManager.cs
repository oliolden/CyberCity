using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class AnimationManager {
        public Animation animation { get; private set; }
        private double _timer;

        public AnimationManager(Animation _animation) {
            animation = _animation;
        }

        public void Play(Animation _animation) {
            if (animation == _animation) { return; }

            animation = _animation;
            animation.currentFrame = 0;
            _timer = 0f;
        }

        public void Update(GameTime gameTime) {
            _timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > animation.frameTime) {
                animation.currentFrame++; if (animation.isLoop && animation.currentFrame >= animation.frameCount) { animation.currentFrame = 0; }
                _timer = 0;
            }
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects) {
            batch.Draw(animation.texture, position, animation.frameRect, color, rotation, origin, scale, effects, 0f);
        }
    }
}
