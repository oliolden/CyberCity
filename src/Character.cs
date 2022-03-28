using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Character : GameObject {
        protected Dictionary<string, Animation> animations;
        protected AnimationManager animationManager;

        public Vector2 velocity;
        protected float topSpeed;
        protected float acceleration;
        protected float jumpStrength;
        protected bool isGrounded;
        protected bool isBlocked;
        protected bool isStuck;
        protected bool isWalking;

        protected Point hitBoxSize;

        public Character(Scene scene) : base(scene) {
            collisions = false;
            velocity = Vector2.Zero;
            isGrounded = false;
            isBlocked = false;
            isStuck = false;
            layer = 2f;
            UpdateHitBox();
        }

        protected void PhysicsUpdate(GameTime gameTime) {
            if (!isWalking) velocity.X = velocity.X * (float)Math.Pow(TileType.types[((World)scene.objects["World"]).GetTile(position.X, position.Y + 0.1f).id].friction, gameTime.ElapsedGameTime.TotalSeconds);

            if (!isGrounded) velocity.Y += 20.0f;

            // Update X position
            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateHitBox();
            isBlocked = false;
            if (CollidesAny()) {
                if (Math.Abs(velocity.X) > 0) { isBlocked = true; }
                int i = 0;
                while (CollidesAny() && i < Math.Abs(velocity.X) / 0.01f) {
                    position.X -= 0.01f * velocity.X / Math.Abs(velocity.X);
                    UpdateHitBox();
                    i++;
                }
                velocity.X = 0;
                UpdateHitBox();
            }

            // Update Y position
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateHitBox();
            isGrounded = false;
            if (CollidesAny()) {
                if (velocity.Y > 0) { isGrounded = true; }
                int i = 0;
                while (CollidesAny() && i < Math.Abs(velocity.Y) / 0.01f) {
                    position.Y -= 0.01f * velocity.Y / Math.Abs(velocity.Y);
                    UpdateHitBox();
                    i++;
                }
                velocity.Y = 0;
            }
            if (!isGrounded) { position.Y += 1; UpdateHitBox(); if (CollidesAny()) { isGrounded = true; } position.Y -= 1; UpdateHitBox(); }
        }

        protected virtual void UpdateHitBox() {
            hitBox = new List<Rectangle> { new Rectangle((int)(position.X - hitBoxSize.X / 2), (int)(position.Y - hitBoxSize.Y), hitBoxSize.X, hitBoxSize.Y) };
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
