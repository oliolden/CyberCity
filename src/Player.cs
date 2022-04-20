using System;
using System.Collections.Generic;
using System.Text;
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
        private bool isSprinting;
        private bool runAttack;
        private bool noClip;

        private KeyboardState keyboardState;
        private MouseState mouseState;

        int equippedTile;
        Tile[] inventory = { new Tile("metal"), new Tile("metalwall"), new Tile("stone"), new Tile("stone", "grass"), new Tile("stonewall") };

        public Player(Scene scene) : base(scene) {
            animations = new Dictionary<string, Animation> {
                { "idle", new Animation(Game1.textures["Cyborg\\Cyborg_idle"], 4, true, 0.3f) },
                { "run", new Animation(Game1.textures["Cyborg\\Cyborg_run"], 6, true) },
                { "jump", new Animation(Game1.textures["Cyborg\\Cyborg_jump"], 4, false) },
                { "doublejump", new Animation(Game1.textures["Cyborg\\Cyborg_doublejump"], 5, false, 0.1f) },
                { "run_attack", new Animation(Game1.textures["Cyborg\\Cyborg_run_attack"], 6, false) },
                { "punch", new Animation(Game1.textures["Cyborg\\Cyborg_attack1"], 6, false, 0.1f) },
                { "hurt", new Animation(Game1.textures["Cyborg\\Cyborg_hurt"], 2, false, 0.2f) },
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
            runAttack = false;
            noClip = false;
            layer = 3f;
        }

        public override void Update(GameTime gameTime) {
            KeyboardState prevKeyboardState = keyboardState;
            MouseState prevMouseState = mouseState;
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up)) { equippedTile++; if (equippedTile >= inventory.Length) equippedTile = 0; }
            if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down)) { equippedTile--; if (equippedTile < 0) equippedTile = inventory.Length - 1; }

            if (mouseState.LeftButton == ButtonState.Pressed) {
                Vector2 pos = scene.camera.mousePosition;
                ((World)scene.objects["World"]).SetTile((int)Math.Floor(pos.X/Tile.width), (int)Math.Floor(pos.Y/Tile.height), inventory[equippedTile]);
            }
            if (mouseState.RightButton == ButtonState.Pressed) {
                Vector2 pos = scene.camera.mousePosition;
                ((World)scene.objects["World"]).SetTile((int)Math.Floor(pos.X/Tile.width), (int)Math.Floor(pos.Y/Tile.height), new Tile("air"));
            }

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

            if (CollidesAny()) { isStuck = true; }
            else {
                isStuck = false;
                // Movement
                isRunning = false;
                if (isGrounded) hasDoubleJumped = false;
                if (keyboardState.IsKeyDown(Keys.LeftShift)) isSprinting = true; else isSprinting = false;
                isWalking = false;
                if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.A)) {
                    if (keyboardState.IsKeyDown(Keys.D)) {
                        Walk(gameTime, false, topSpeed + (isSprinting ? sprintBoost : 0));
                    }
                    if (keyboardState.IsKeyDown(Keys.A)) {
                        Walk(gameTime, true, topSpeed + (isSprinting ? sprintBoost : 0));
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

            // Attacking
            if (!isAttacking && isGrounded && mouseState.LeftButton == ButtonState.Pressed) {
                isAttacking = true;
                runAttack = isRunning;
            }
            else if (isAttacking && animationManager.isPlaying == true) { }
            else { isAttacking = false; }

            // Animations
            if (isStuck) { PlayAnimation("idle"); color = (int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 2 == 0 ? Color.Red : Color.White; }
            else {
                color = Color.White;
                if (!isGrounded) { if (hasDoubleJumped) PlayAnimation("doublejump"); else PlayAnimation("jump"); }
                else if (isAttacking) { if (runAttack) PlayAnimation("run_attack"); else PlayAnimation("punch"); }
                else if (isRunning) PlayAnimation("run");
                else { PlayAnimation("idle"); }

                if (isGrounded && isRunning) animationManager.animation.frameTime = Math.Abs(10f / velocity.X);
                else if (isGrounded && isAttacking && runAttack) animationManager.animation.frameTime = 0.2f;
            }

            animationManager.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime) {
            base.Draw(batch, gameTime);
            if (((GameScene)scene).devTools) {
                string info =
                    $"{(int)(1 / gameTime.ElapsedGameTime.TotalSeconds)} fps\n" +
                    $"XY: {(int)position.X}, {(int)position.Y}\n" +
                    $"Tile: {Math.Floor(position.X / Tile.width)}, {Math.Floor(position.Y / Tile.height)}\n" +
                    $"Selected Tile: {inventory[equippedTile].id}, {(inventory[equippedTile].variant != null ? inventory[equippedTile].variant : "default")}";
                batch.DrawString(Game1.fonts["Fonts\\Minecraft"], info, scene.camera.position + (Vector2.One * 4) / scene.camera.zoom, Color.Black, 0f, Vector2.Zero, 1.5f / scene.camera.zoom, SpriteEffects.None, 1f);
            }
        }
    }
}
