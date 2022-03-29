using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Truck : GameObject {
        private Animation[] chassiAnimations;
        private AnimationManager chassiManager;

        private bool hasTrailer, isOpen;
        private int body, chassi, trailer;
        private List<KeyValuePair<Point, string>> decor;
        public Truck(Scene scene, int body, int chassi) : base(scene) {
            chassiAnimations = new Animation[] {
                new Animation(game.textures["Vehicles\\Truck\\Chassis\\1"], 6, true),
                new Animation(game.textures["Vehicles\\Truck\\Chassis\\2"], 6, true),
            };
            this.body = body;
            this.chassi = chassi;
            chassiManager = new AnimationManager(chassiAnimations[this.chassi - 1]);
            hasTrailer = false;
            isOpen = false;
            collisions = false;
        }

        public void SetTrailer(int id) {
            trailer = id;
        }

        public void AddDecor(int x, int y, string id) {
            decor.Add(new KeyValuePair<Point, string>(new Point(x, y), id));
        }

        public override void Update(GameTime gameTime) {
            //if ()
            chassiManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            // Draw trailer
            if (hasTrailer) batch.Draw(game.textures[$"Vehicles\\Truck\\Trailers\\{trailer}"], position, null, color, rotation, origin, scale, spriteEffects, layer - 0.3f);
            // Draw body
            batch.Draw(game.textures[$"Vehicles\\Truck\\Bodies\\{body}"], position, null, color, rotation, origin, scale, spriteEffects, layer - 0.2f);
            // Draw decor
            // Draw chassi
            chassiManager.Draw(batch, position, color, rotation, origin, scale, spriteEffects, layer);
            base.Draw(batch, gameTime);
        }
    }
}
