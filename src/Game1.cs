using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace CyberCity {
    internal class Game1 : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Dictionary<string, Scene> scenes;
        private Scene currentScene;
        internal static Dictionary<string, Texture2D> textures;
        internal static Dictionary<string, SpriteFont> fonts;
        internal bool devTools;
        private KeyboardState keyboardState;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            base.Initialize();
            // TODO: Add your initialization logic here

            scenes = new Dictionary<string, Scene>();
            scenes.Add("mainMenu", new Scene(this));
            scenes["mainMenu"].objects.Add(
                "playButton", new MenuButton(scenes["mainMenu"],
                () => { currentScene = scenes["game"]; }, "Play", "World\\Tiles\\metal\\00000000")
                { scale = new Vector2(3, 3) }
            );
            scenes["mainMenu"].objects.Add(
                "editorButton", new MenuButton(scenes["mainMenu"],
                () => { currentScene = scenes["editor"]; }, "Editor", "World\\Tiles\\metal\\00000000")
                { scale = new Vector2(3, 3), position = new Vector2(0, 150) }
            );
            scenes["mainMenu"].objects.Add("background", new ParallaxBackgroundObject(scenes["mainMenu"], "industrialZone", 2f));
            scenes["mainMenu"].camera.CenterOn(scenes["mainMenu"].objects["playButton"], Vector2.Zero);

            scenes.Add("game", new Scene(this));
            scenes["game"].objects = new Dictionary<string, GameObject> {
                { "World", new World(scenes["game"]) },
                { "Player", new Player(scenes["game"]) },
                { "Jerry", new Jerry(scenes["game"]) },
            };
            scenes["game"].camera.CenterOn(scenes["game"].objects["Player"], new Vector2(0, -120));

            scenes.Add("editor", new Scene(this));
            scenes["editor"].objects.Add(
                "editor", new Editor(scenes["editor"])
            );
            scenes["editor"].camera.CenterOn(scenes["editor"].objects["editor"], new Vector2(0, 0));

            currentScene = scenes["mainMenu"];
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textures = new Dictionary<string, Texture2D> { { "Blank", new Texture2D(GraphicsDevice, 1, 1) } };
            fonts = new Dictionary<string, SpriteFont>();
            textures["Blank"].SetData(new Color[] { Color.White });
            LoadFilesAtPath("..\\..\\..\\Content\\");
            foreach (string path in Directory.GetDirectories("..\\..\\..\\Content\\")) {
                string directory = path.Substring(17);
                if (directory != "bin" && directory != "obj") {
                    LoadContentAtPath(path);
                }
            }
            foreach (string id in textures.Keys) {
                if (id.StartsWith("World\\Background\\")) {
                    string backgroundId = id.Substring("World\\Background\\".Length).Split('\\')[0];
                    if (!ParallaxBackground.backgrounds.ContainsKey(backgroundId)) ParallaxBackground.backgrounds.Add(backgroundId, new List<Texture2D>());
                    ParallaxBackground.backgrounds[backgroundId].Add(textures[id]);
                }
            }
        }

        private void LoadContentAtPath(string path) {
            LoadFilesAtPath(path);
            foreach (string directory in Directory.GetDirectories(path)) {
                LoadContentAtPath(directory);
            }
        }

        private void LoadFilesAtPath(string path) {
            foreach (string file in Directory.GetFiles(path)) {
                if (file.EndsWith(".png")) {
                    string textureName = file.Substring(0, file.Length - 4);
                    textureName = textureName.Substring(17);
                    textures.Add(textureName, Content.Load<Texture2D>(textureName));
                }
                if (file.EndsWith(".spritefont")) {
                    string fontName = file.Substring(0, file.Length - 11);
                    fontName = fontName.Substring(17);
                    fonts.Add(fontName, Content.Load<SpriteFont>(fontName));
                }
            }
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            KeyboardState prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.F3) && prevKeyboardState.IsKeyUp(Keys.F3)) {
                devTools = !devTools;
            }

            if (keyboardState.IsKeyDown(Keys.Escape) && prevKeyboardState.IsKeyUp(Keys.Escape)) {
                currentScene = scenes["mainMenu"];
                Save.SaveGame(this, "game");
            }
            if (keyboardState.IsKeyDown(Keys.M) && prevKeyboardState.IsKeyUp(Keys.M)) {
                Save.LoadGame(this, "game");
            }

            currentScene.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(currentScene.backgroundColor);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, currentScene.camera.matrix);
            currentScene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawRectangle(SpriteBatch batch, Rectangle rectangle, int thickness, Color color, float layer) {
            batch.Draw(textures["Blank"], new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), null, color, 0, Vector2.Zero, SpriteEffects.None, layer); // Top
            batch.Draw(textures["Blank"], new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, layer); // Left
            batch.Draw(textures["Blank"], new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), null, color, 0, Vector2.Zero, SpriteEffects.None, layer); // Right
            batch.Draw(textures["Blank"], new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), null, color, 0, Vector2.Zero, SpriteEffects.None, layer); // Bottom
        }
    }
}
