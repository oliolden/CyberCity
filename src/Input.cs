using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CyberCity {
    internal static class Input {
        public static Dictionary<string, Keys> binds = new Dictionary<string, Keys> {
            { "left", Keys.A },
            { "right", Keys.D },
            { "sprint", Keys.LeftShift },
            { "jump", Keys.Space },
            { "attack", Keys.E },
        };
        public static bool left = false, right = false, sprint = false;
        public static event EventHandler Jump;
        public static event EventHandler Attack;

        private static KeyboardState keyboard;

        public static void Update() {
            KeyboardState prevKeyboard = keyboard;
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(binds["left"])) left = true;
            else left = false;
            if (keyboard.IsKeyDown(binds["right"])) right = true;
            else right = false;
            if (keyboard.IsKeyDown(binds["sprint"])) sprint = true;
            else sprint = false;
            if (keyboard.IsKeyDown(binds["jump"]) && prevKeyboard.IsKeyUp(binds["jump"])) Jump?.Invoke(null, null);
            if (keyboard.IsKeyDown(binds["attack"]) && prevKeyboard.IsKeyUp(binds["attack"])) Attack?.Invoke(null, null);
        }
    }
}
