using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CyberCity {
    internal class Player : Character {
        private float sprintBoost;

        // Bools
        private bool isRunning;
        private bool isAttacking;
        private bool hasDoubleJumped;
        private bool noClip;

        private KeyboardState keyboardState;
        private MouseState mouseState;

        private int attack;
        private Task attackTask;
        private bool combo;

        public Player(Scene scene) : base(scene) {
            animations = new Dictionary<string, Animation> {
                { "idle", new Animation(Game1.textures["Cyborg\\Cyborg_idle"], 4, true, 0.3f) },
                { "run", new Animation(Game1.textures["Cyborg\\Cyborg_run"], 6, true) },
                { "jump", new Animation(Game1.textures["Cyborg\\Cyborg_jump"], 4, false) },
                { "doublejump", new Animation(Game1.textures["Cyborg\\Cyborg_doublejump"], 5, false, 0.1f) },
                { "run_attack", new Animation(Game1.textures["Cyborg\\Cyborg_run_attack"], 6, false) },
                { "attack1", new Animation(Game1.textures["Cyborg\\Cyborg_attack1"], 6, false, 0.08f) },
                { "attack2", new Animation(Game1.textures["Cyborg\\Cyborg_attack2"], 8, false, 0.08f) },
                { "attack3", new Animation(Game1.textures["Cyborg\\Cyborg_attack3"], 8, false, 0.12f) },
            };
            animationManager = new AnimationManager(animations["idle"]);

            origin = new Vector2(animationManager.animation.frameWidth / 4, animationManager.animation.frameHeight);
            position = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            topSpeed = 100.0f;
            sprintBoost = 100.0f;
            acceleration = 600.0f;
            jumpStrength = 500.0f;
            hitBoxSize = new Point(16, 32); // 24, 36
            isRunning = false;
            isAttacking = false;
            noClip = false;
            Input.Attack += OnAttack;
            layer = 3f;
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
                PlayAnimation("idle");
                float noClipSpeed = 250;
                color = new Color(100, 100, 100, 100);
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
                UpdateHitBox();
                return;
            }
            else {
                color = Color.White;
            }

            if (isAttacking) { Input.right = false; Input.left = false; }

            if (CollidesAny()) { isStuck = true; }
            else {
                isStuck = false;
                // Movement
                isRunning = false;
                if (isGrounded) hasDoubleJumped = false;
                isWalking = false;
                if (Input.left || Input.right) {
                    if (Input.right) {
                        Walk(gameTime, false, topSpeed + (Input.sprint ? sprintBoost : 0));
                    }
                    if (Input.left) {
                        Walk(gameTime, true, topSpeed + (Input.sprint ? sprintBoost : 0));
                    }
                    if (!isBlocked) {
                        isRunning = true;
                    }
                }

                if (isGrounded || !hasDoubleJumped) {
                    if (keyboardState.IsKeyDown(Keys.Space)) {
                        if (prevKeyboardState.IsKeyUp(Keys.Space)) {
                            velocity.Y = -jumpStrength;
                            if (!isGrounded) hasDoubleJumped = true;
                        }
                    }
                }

                // Physics
                PhysicsUpdate(gameTime, !isWalking);

            }

            //if (attackCooldown > 0) { attackCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds; }
            //if (attackCooldown <= 0 && !nextAttack) { isAttacking = false; attack = 1; }

            // Animations
            if (isStuck) { PlayAnimation("idle"); color = (int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 2 == 0 ? Color.Red : Color.White; }
            else {
                color = Color.White;
                if (isAttacking) { PlayAnimation("attack" + attack); }
                else if (!isGrounded) { if (hasDoubleJumped) PlayAnimation("doublejump"); else PlayAnimation("jump"); }
                else if (isRunning) PlayAnimation("run");
                else { PlayAnimation("idle"); }

                if (isGrounded && isRunning && !isAttacking) animationManager.animation.frameTime = Math.Abs(10f / velocity.X);
            }

            animationManager.Update(gameTime);
        }

        private async Task Attack() {
            combo = false;
            isAttacking = true;
            velocity.X += topSpeed * (spriteEffects == SpriteEffects.FlipHorizontally ? -1 : 1);
            await Task.Delay((int)(animations["attack" + attack].frameTime * animations["attack" + attack].frameCount * 1000));
            isAttacking = false;
        }

        public async void OnAttack(object sender, EventArgs args) {
            if (attackTask == null || attackTask.IsCompleted) { attack = 1; attackTask = Task.Run(Attack); return; }
            else if (!combo) {
                combo = true;
                await attackTask;
                if (attack < 3) attack++;
                attackTask = Task.Run(Attack);
            }
            /*
            if (attackCooldown <= 0) {
                attackCooldown = 0.96f;
                isAttacking = true;
                if (nextAttack && attack < 3) { attack++; }

            }
            else {
                nextAttack = true;
            }
            */
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            base.Draw(batch, gameTime);
            if (game.devTools) {
                string info =
                    $"{(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)} fps\n" +
                    $"XY: {(int)position.X}, {(int)position.Y}\n" +
                    $"Tile: {Math.Floor(position.X / Tile.width)}, {Math.Floor(position.Y / Tile.height)}\n";
                batch.DrawString(Game1.fonts["Fonts\\Minecraft"], info, scene.camera.GetPosition() + (Vector2.One * 4) / scene.camera.zoom, Color.Black, 0f, Vector2.Zero, 1.5f / scene.camera.zoom, SpriteEffects.None, 1f);
            }
        }
    }
}
