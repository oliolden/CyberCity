using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class PhysObject : GameObject {
        public Vector2 velocity;
        protected bool isGrounded;
        protected bool isBlocked;
        protected bool isStuck;
        protected Point hitBoxSize;

        public PhysObject(Scene scene) : base(scene) { }

        protected void PhysicsUpdate(GameTime gameTime) {
            //if (!isWalking) 
            velocity.X = velocity.X * (float)Math.Pow(TileType.types[((World)scene.objects["World"]).GetTile(position.X, position.Y + 0.1f).id].friction, gameTime.ElapsedGameTime.TotalSeconds);

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

    }
}
