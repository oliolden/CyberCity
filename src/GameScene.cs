using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal class GameScene : Scene {
        internal bool devTools;

        private KeyboardState keyboardState;

        public GameScene(Game1 myGame) : base(myGame) { }

        public override void Update(GameTime gameTime) {
            KeyboardState prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.F3) && prevKeyboardState.IsKeyUp(Keys.F3)) {
                devTools = !devTools;
            }

            base.Update(gameTime);
        }
    }
}
