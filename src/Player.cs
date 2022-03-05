using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CyberCity {
    internal class Player : GameObject {
        private Dictionary<string, Animation> _animations;
        private AnimationManager _animationManager;

        public Vector2 velocity;
        private float topSpeed;
        private float acceleration;

        public Player(Game1 myGame) : base(myGame){
            _animations = new Dictionary<string, Animation> {
                { "idle", new Animation(game.textures["Cyborg\\Cyborg_idle"], 4, true) },
                { "run", new Animation(game.textures["Cyborg\\Cyborg_run"], 6, true) },
                { "jump", new Animation(game.textures["Cyborg\\Cyborg_run"], 4, false) },
            };
            _animationManager = new AnimationManager(_animations["idle"]);

            origin = new Vector2(_animationManager.animation.frameWidth / 2, _animationManager.animation.frameHeight / 2);
            position = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            velocity = Vector2.Zero;
            topSpeed = 100.0f;
            acceleration = 400.0f;
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.A)) {
                if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                    spriteEffects = SpriteEffects.None;
                    velocity.X += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X > topSpeed) { velocity.X = topSpeed; }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    velocity.X -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X < -topSpeed) { velocity.X = -topSpeed; }
                }
                _animationManager.Play(_animations["run"]);
                _animationManager.animation.frameTime = Math.Abs(8f/velocity.X);
            }
            else {
                velocity.X = velocity.X * (float)Math.Pow(0.05f, gameTime.ElapsedGameTime.TotalSeconds);
                _animationManager.Play(_animations["idle"]);
            }

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _animationManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            _animationManager.Draw(batch, position, color, rotation, origin, scale, spriteEffects);
        }
    }
}
