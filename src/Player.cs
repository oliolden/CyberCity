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
        private bool isGrounded;
        private bool isRunning;
        private bool isAttacking;
        private bool hasDoubleJumped;
        private bool canJump;

        private Point hitBoxSize;

        public Player(Scene myScene) : base(myScene) {
            _animations = new Dictionary<string, Animation> {
                { "idle", new Animation(game.textures["Cyborg\\Cyborg_idle"], 4, true) },
                { "run", new Animation(game.textures["Cyborg\\Cyborg_run"], 6, true) },
                { "jump", new Animation(game.textures["Cyborg\\Cyborg_jump"], 4, false) },
                { "doublejump", new Animation(game.textures["Cyborg\\Cyborg_doublejump"], 5, false, 0.1f) },
            };
            _animationManager = new AnimationManager(_animations["idle"]);

            origin = new Vector2(_animationManager.animation.frameWidth / 4, _animationManager.animation.frameHeight);
            position = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            velocity = Vector2.Zero;
            topSpeed = 100.0f;
            acceleration = 400.0f;
            hitBoxSize = new Point(24, 36); // 24, 36
            isGrounded = false;
            UpdateHitBox();
        }

        public override void Update(GameTime gameTime) {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Movement
            isRunning = false;
            if (isGrounded) {
                hasDoubleJumped = false;
                if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.A)) {
                    if (keyboard.IsKeyDown(Keys.D)) {
                        spriteEffects = SpriteEffects.None;
                        velocity.X += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X > topSpeed) { velocity.X = topSpeed; }
                    }
                    if (keyboard.IsKeyDown(Keys.A)) {
                        spriteEffects = SpriteEffects.FlipHorizontally;
                        velocity.X -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X < -topSpeed) { velocity.X = -topSpeed; }
                    }
                    if (Math.Abs(velocity.X) > 1) {
                        _animationManager.Play(_animations["run"]);
                        _animationManager.animation.frameTime = Math.Abs(8f / velocity.X);
                        isRunning = true;
                    }
                }
                else {
                    velocity.X = velocity.X * (float)Math.Pow(0.001f, gameTime.ElapsedGameTime.TotalSeconds);
                    _animationManager.Play(_animations["idle"]);
                }
            }
            else { velocity.X = velocity.X * (float)Math.Pow(0.8f, gameTime.ElapsedGameTime.TotalSeconds); }

            if (isGrounded || !hasDoubleJumped) {
                if (keyboard.IsKeyDown(Keys.Space)) {
                    if (canJump) {
                        velocity.Y = -500f;
                        if (isGrounded) _animationManager.Play(_animations["jump"]);
                        else { _animationManager.Play(_animations["doublejump"]); hasDoubleJumped = true; }
                    }
                    canJump = false;
                }
                else { canJump = true; }
            }
            velocity.Y += 20.0f;

            // Update X position
            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateHitBox();
            if (CollidesAny()) {
                position.X -= velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.X = 0;
                UpdateHitBox();
            }

            // Update Y position
            isGrounded = false;
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateHitBox();
            if (CollidesAny()) {
                if (velocity.Y > 0) { isGrounded = true; }
                position.Y -= velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y = 0;
                UpdateHitBox();
            }

            // Attacking
            if (!isAttacking && mouse.LeftButton == ButtonState.Pressed) {

            }

            // Animations

            _animationManager.Update(gameTime);
        }

        private void UpdateHitBox() {
            hitBox = new Rectangle[] { new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y + 12), hitBoxSize.X, hitBoxSize.Y) };
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            _animationManager.Draw(batch, position - Vector2.UnitX * (spriteEffects == SpriteEffects.FlipHorizontally ? 24 : 0), color, rotation, origin, scale, spriteEffects);
        }
    }
}
