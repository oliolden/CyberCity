using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class Jerry : NPC {
        public Jerry(Scene scene) : base(scene) {
            animations = new Dictionary<string, Animation> {
                { "idle", new Animation(Game1.textures["NPCs\\Jerry\\Idle"], 4, true) },
                { "walk", new Animation(Game1.textures["NPCs\\Jerry\\Walk"], 6, true) },
            };
            animationManager = new AnimationManager(animations["idle"]);
            origin = new Vector2(animationManager.animation.frameWidth / 4, animationManager.animation.frameHeight);
            position = new Vector2(random.Next(200, 600), 500);
            topSpeed = 100.0f;
            acceleration = 300.0f;
            jumpStrength = 300.0f;
            hitBoxSize = new Point(16, 26);
            destination = position + Vector2.UnitX * random.Next(-400, 400);
        }

        public override void Update(GameTime gameTime) {
            Move(gameTime);
            PhysicsUpdate(gameTime, !isWalking);
            if (isWalking && !isBlocked) { PlayAnimation("walk"); animationManager.animation.frameTime = Math.Abs(8f / velocity.X); }
            else PlayAnimation("idle");
            animationManager.Update(gameTime);
        }

        protected override void NewDestination() {
            destination = position + Vector2.UnitX * random.Next(-300, 300);
        }
    }
}
