using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class Truck : PhysObject {
        private static Chassi[] chassis = new Chassi[] {
            new Chassi(new Animation(Game1.textures["Vehicles\\Truck\\Chassis\\1"], 6, true), new Animation(Game1.textures["Vehicles\\Truck\\Chassis\\1_2"], 6, true), new Point(50, 50)),
            new Chassi(new Animation(Game1.textures["Vehicles\\Truck\\Chassis\\2"], 6, true), new Animation(Game1.textures["Vehicles\\Truck\\Chassis\\2_2"], 6, true), new Point(50, 50)),
        };
        private AnimationManager chassiManagerFront;
        private AnimationManager chassiManagerBack;
        private bool hasTrailer, isOpen;
        private int body, chassi, chassiCount, trailer;
        private List<KeyValuePair<Vector2, string>> decor;
        public Truck(Scene scene, int body, int chassi) : base(scene) {
            this.body = body;
            this.chassi = chassi;
            chassiManagerFront = new AnimationManager(chassis[this.chassi - 1].front);
            chassiManagerBack = new AnimationManager(chassis[this.chassi - 1].back);
            hasTrailer = false;
            isOpen = false;
            collisions = false;
            chassiCount = 2;
            decor = new List<KeyValuePair<Vector2, string>>();
            //origin = new Vector2(132 / 2, 57);
        }

        public void SetTrailer(int id) {
            trailer = id;
        }

        public void AddDecor(float x, float y, string id) {
            decor.Add(new KeyValuePair<Vector2, string>(new Vector2(x, y), id));
        }

        public override void Update(GameTime gameTime) {
            PhysicsUpdate(gameTime);
            if (Math.Abs(velocity.X) > 0) { chassiManagerFront.Play(); chassiManagerBack.Play(); }
            else { chassiManagerFront.ShowFrame(0); chassiManagerBack.ShowFrame(0); }
            chassiManagerFront.Update(gameTime);
            chassiManagerBack.Update(gameTime);
        }

        protected override void UpdateHitBox() {
            base.UpdateHitBox();
            for (int i = 0; i < chassiCount; i++) {
                hitBox.Add(new Rectangle((int)position.X + i * 40 + 35, (int)position.Y + 30, chassis[chassi - 1].size.X, chassis[chassi - 1].size.Y));
                hitBox.Add(new Rectangle((int)position.X + i * 40, (int)position.Y + 30, chassis[chassi - 1].size.X, chassis[chassi - 1].size.Y));
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            // Draw trailer
            if (hasTrailer) batch.Draw(Game1.textures[$"Vehicles\\Truck\\Trailers\\{trailer}"], position, null, color, rotation, origin, scale, spriteEffects, layer - 0.3f);
            // Draw body
            batch.Draw(Game1.textures[$"Vehicles\\Truck\\Bodies\\{body}"], position, null, color, rotation, origin, scale, spriteEffects, layer - 0.2f);
            // Draw decor
            foreach (KeyValuePair<Vector2, string> item in decor) {
                batch.Draw(Game1.textures[$"Vehicles\\Truck\\Decor\\{item.Value}"], position + item.Key, null, color, rotation, origin, scale, spriteEffects, layer - 0.1f);
            }
            // Draw chassi
            for (int i = 0; i < chassiCount; i++)
			{
                chassiManagerBack.Draw(batch, new Vector2(position.X + i * 40 + 35, position.Y + 30), color, rotation, origin, scale, spriteEffects, layer - 0.4f);
                chassiManagerFront.Draw(batch, new Vector2(position.X + i * 40, position.Y + 30), color, rotation, origin, scale, spriteEffects, layer);
			}
            base.Draw(batch, gameTime);
        }
    }
}
