using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace CyberCity {
    public class Game1 : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, Scene> _scenes;
        private Scene _currentScene;
        internal Dictionary<string, Texture2D> textures;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize() {
            Window.AllowUserResizing = true;
            base.Initialize();
            // TODO: Add your initialization logic here

            _scenes = new Dictionary<string, Scene> {
                { "mainMenu", new Scene(this) },
                { "game", new GameScene(this) },
            };
            _currentScene = _scenes["game"];
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textures = new Dictionary<string, Texture2D> { { "Blank", new Texture2D(GraphicsDevice, 1, 1) } };
            textures["Blank"].SetData(new Color[] { Color.White });
            foreach (string file in Directory.GetFiles("..\\..\\..\\Content\\")) {
                if (file.EndsWith(".png")) {
                    string textureName = file.Substring(0, file.Length - 4);
                    textureName = textureName.Substring(17);
                    textures.Add(textureName, Content.Load<Texture2D>(textureName));
                }
            }
            foreach (string path in Directory.GetDirectories("..\\..\\..\\Content\\")) {
                string directory = path.Substring(17);
                if (directory != "bin" && directory != "obj") {
                    LoadContentAtPath(path);
                }
            }
        }

        private void LoadContentAtPath(string path) {
            foreach (string file in Directory.GetFiles(path)) {
                if (file.EndsWith(".png")) {
                    string textureName = file.Substring(0, file.Length - 4);
                    textureName = textureName.Substring(17);
                    textures.Add(textureName, Content.Load<Texture2D>(textureName));
                }
            }
            foreach (string directory in Directory.GetDirectories(path)) {
                LoadContentAtPath(directory);
            }
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            _currentScene.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(_currentScene.backgroundColor);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _currentScene.camera.matrix);
            _currentScene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
