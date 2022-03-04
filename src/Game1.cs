﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CyberCity {
    public class Game1 : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, Scene> _scenes;
        private Scene _currentScene;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize() {
            // TODO: Add your initialization logic here
            _scenes = new Dictionary<string, Scene> {
                { "mainMenu", new Scene(this) },
                { "game", new Scene(this) },
            };
            _currentScene = _scenes["game"];

            _scenes["game"].gameObjects.Add(new World(this));
            ((World)_scenes["game"].gameObjects[0]).tiles = new List<List<Tile>> {
                new List<Tile>{ new Tile(1), new Tile(1), new Tile(1), new Tile(1), },
                new List<Tile>{ new Tile(1), new Tile(0), new Tile(0), new Tile(1), },
                new List<Tile>{ new Tile(1), new Tile(0), new Tile(0), new Tile(1), },
                new List<Tile>{ new Tile(1), new Tile(1), new Tile(1), new Tile(1), },
            };
            ((World)_scenes["game"].gameObjects[0]).UpdateTileTextures();

            _scenes["game"].gameObjects.Add(new Player(this));

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            // TODO: Add your update logic here
            _currentScene.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _currentScene.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
