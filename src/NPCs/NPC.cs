using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CyberCity {
    internal class NPC : Character {
        protected Vector2 destination;
        protected Random random;

        public NPC(Scene scene) : base(scene) { random = new Random(); }

        public override void Update(GameTime gameTime) {
            Move(gameTime);
            base.Update(gameTime);
        }

        protected virtual void Move(GameTime gameTime) {
            isWalking = false;
            if (destination == null || Math.Abs(scene.objects["Player"].position.X - position.X) > 500) return;
            if (Math.Floor(destination.X / Tile.width) > Math.Floor(position.X / Tile.width)) { Walk(gameTime, false, topSpeed); }
            else if (Math.Floor(destination.X / Tile.width) < Math.Floor(position.X / Tile.width)) { Walk(gameTime, true, topSpeed); }
        }
    }
}
