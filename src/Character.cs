using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Character : PhysObject {
        protected Dictionary<string, Animation> animations;
        protected AnimationManager animationManager;

        protected float topSpeed;
        protected float acceleration;
        protected float jumpStrength;
        protected bool isWalking;

        public Character(Scene scene) : base(scene) {
            collisions = false;
            velocity = Vector2.Zero;
            isGrounded = false;
            isBlocked = false;
            isStuck = false;
            layer = 2f;
            UpdateHitBox();
        }

        protected void PlayAnimation(string animationName) {
            animationManager.Play(animations[animationName]);
        }

        public override void Update(GameTime gameTime) {
            PhysicsUpdate(gameTime);
            animationManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            animationManager.Draw(batch, position - Vector2.UnitX * (spriteEffects == SpriteEffects.FlipHorizontally ? 24 : 0), color, rotation, origin, scale, spriteEffects, layer);
            base.Draw(batch, gameTime);
        }

        protected void Walk(GameTime gameTime, bool left, float topSpeed) {
            isWalking = true;
            if (left) {
                spriteEffects = SpriteEffects.FlipHorizontally;
                velocity.X -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X < -topSpeed) { velocity.X = -topSpeed; }
            }
            else {
                spriteEffects = SpriteEffects.None;
                velocity.X += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X > topSpeed) { velocity.X = topSpeed; }
            }
        }
    }
}
