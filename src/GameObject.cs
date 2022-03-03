using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCity {
    internal abstract class GameObject {
        protected Game1 game;
        public Vector2 position;
        public float rotation;
        public Vector2 scale;
        public Vector2 origin;
        public Color color;
        public SpriteEffects spriteEffects;
        public bool enabled;

        public GameObject() {
            position = Vector2.Zero;
            rotation = 0f;
            scale = Vector2.One;
            origin = Vector2.Zero;
            color = Color.White;
            spriteEffects = SpriteEffects.None;
            enabled = true;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch batch, GameTime gameTime) { }
    }
}
