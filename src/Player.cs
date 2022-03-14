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
        private float sprintBoost;
        private float acceleration;

        // Bools
        private bool isGrounded;
        private bool isRunning;
        private bool isAttacking;
        private bool hasDoubleJumped;
        private bool canJump;
        private bool isBlocked;
        private bool isSprinting;
        private bool runAttack;
        private bool noClip;

        private Point hitBoxSize;

        KeyboardState keyboardState;
        MouseState mouseState;

        public Player(Scene myScene) : base(myScene) {
            _animations = new Dictionary<string, Animation> {
                { "idle", new Animation(game.textures["Cyborg\\Cyborg_idle"], 4, true) },
                { "run", new Animation(game.textures["Cyborg\\Cyborg_run"], 6, true) },
                { "jump", new Animation(game.textures["Cyborg\\Cyborg_jump"], 4, false) },
                { "doublejump", new Animation(game.textures["Cyborg\\Cyborg_doublejump"], 5, false, 0.1f) },
                { "run_attack", new Animation(game.textures["Cyborg\\Cyborg_run_attack"], 6, false) },
                { "punch", new Animation(game.textures["Cyborg\\Cyborg_punch"], 6, false, 0.1f) },
            };
            _animationManager = new AnimationManager(_animations["idle"]);

            origin = new Vector2(_animationManager.animation.frameWidth / 4, _animationManager.animation.frameHeight);
            position = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            velocity = Vector2.Zero;
            topSpeed = 100.0f;
            sprintBoost = 50.0f;
            acceleration = 400.0f;
            hitBoxSize = new Point(24, 36); // 24, 36
            isGrounded = false;
            isRunning = false;
            isAttacking = false;
            runAttack = false;
            noClip = true;
            UpdateHitBox();
        }

        public override void Update(GameTime gameTime) {
            KeyboardState prevKeyboardState = keyboardState;
            MouseState prevMouseState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.V) && prevKeyboardState.IsKeyUp(Keys.V)) {
                noClip = !noClip;
            }

            if (noClip) {
                float noClipSpeed = 1000;
                color = new Color(255, 255, 255, 200);
                velocity = Vector2.Zero;
                if (keyboardState.IsKeyDown(Keys.W)) {
                    velocity.Y -= noClipSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.A)) {
                    velocity.X -= noClipSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.S)) {
                    velocity.Y += noClipSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.D)) {
                    velocity.X += noClipSpeed;
                }
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }
            else {
                color = Color.White;
            }

            // Movement
            isRunning = false;
            if (isGrounded) hasDoubleJumped = false;
            if (keyboardState.IsKeyDown(Keys.LeftShift)) isSprinting = true; else isSprinting = false;

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.A)) {
                if (keyboardState.IsKeyDown(Keys.D)) {
                    spriteEffects = SpriteEffects.None;
                    velocity.X += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X > topSpeed + (isSprinting ? sprintBoost : 0)) { velocity.X = topSpeed + (isSprinting ? sprintBoost : 0); }
                }
                if (keyboardState.IsKeyDown(Keys.A)) {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                    velocity.X -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; if (velocity.X < -topSpeed - (isSprinting ? sprintBoost : 0)) { velocity.X = -topSpeed - (isSprinting ? sprintBoost : 0); }
                }
                if (!isBlocked) {
                    isRunning = true;
                }
            }
            else {
                velocity.X = velocity.X * (float)Math.Pow(0.001f, gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (isGrounded || !hasDoubleJumped) {
                if (keyboardState.IsKeyDown(Keys.Space)) {
                    if (canJump) {
                        velocity.Y = -500f;
                        if (!isGrounded) hasDoubleJumped = true;
                    }
                    canJump = false;
                }
                else { canJump = true; }
            }
            velocity.Y += 20.0f;

            // Update X position
            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateHitBox();
            isBlocked = false;
            if (CollidesAny()) {
                if (Math.Abs(velocity.X) > 0) { isBlocked = true; }
                int i = 0;
                while (CollidesAny() && i < 1000) {
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
                while (CollidesAny() && i < 1000) {
                    position.Y -= 0.01f * velocity.Y / Math.Abs(velocity.Y);
                    UpdateHitBox();
                    i++;
                }
                velocity.Y = 0;
            }

            // Attacking
            if (!isAttacking && isGrounded && mouseState.LeftButton == ButtonState.Pressed) {
                isAttacking = true;
                runAttack = isRunning;
            }
            else if (isAttacking && _animationManager.animation.currentFrame < 5) { }
            else { isAttacking = false; }

            // Animations
            if(!isGrounded) { if (hasDoubleJumped) playAnimation("doublejump"); else playAnimation("jump"); }
            else if (isAttacking) { if (runAttack) playAnimation("run_attack"); else playAnimation("punch"); }
            else if (isRunning) playAnimation("run");
            else { playAnimation("idle"); }

            if (isGrounded && isRunning) _animationManager.animation.frameTime = Math.Abs(8f / velocity.X);
            else if (isGrounded && isAttacking && runAttack) _animationManager.animation.frameTime = 0.1f;

            _animationManager.Update(gameTime);
        }

        private void playAnimation(string animationName) {
            _animationManager.Play(_animations[animationName]);
        }

        private void UpdateHitBox() {
            hitBox = new List<Rectangle> { new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y + 12), hitBoxSize.X, hitBoxSize.Y) };
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            _animationManager.Draw(batch, position - Vector2.UnitX * (spriteEffects == SpriteEffects.FlipHorizontally ? 24 : 0), color, rotation, origin, scale, spriteEffects);
            batch.DrawString(game.fonts["Fonts\\Arial"], $"XY: {string.Format("{0:#0.00}", position.X)}, {string.Format("{0:#0.00}", position.Y)}", scene.camera.position, Color.Lime, 0f, Vector2.Zero, 1 / scene.camera.zoom, SpriteEffects.None, 0f);
        }
    }
}
