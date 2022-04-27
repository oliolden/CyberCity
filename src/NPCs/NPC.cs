using System;
using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class NPC : Character {
        protected Vector2 destination;
        protected Random random;
        private bool hasJumped;

        public NPC(Scene scene) : base(scene) { random = new Random(); }

        public override void Update(GameTime gameTime) {
            Move(gameTime);
            base.Update(gameTime);
        }

        protected virtual void Move(GameTime gameTime) {
            if (isWalking) {
                if (!isBlocked) hasJumped = false;
                else if (!hasJumped) { Jump(); hasJumped = true; }
                else if (isGrounded) destination = position;
            }
            isWalking = false;
            if (destination == null || Math.Abs(scene.objects["Player"].position.X - position.X) > scene.camera.GetWidth() / 2 + 20) return;
            if (destination.X > position.X + 5) { Walk(gameTime, false, topSpeed); }
            else if (destination.X < position.X - 5) { Walk(gameTime, true, topSpeed); }
            if (!isWalking) { if (random.Next(200) == 0) NewDestination(); }
        }

        protected virtual void NewDestination() { }

        protected void Jump() {
            if (isGrounded) velocity.Y -= jumpStrength;
        }
    }
}
