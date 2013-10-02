using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace DiscoRoboGame
{
    internal class ControlObject : SpriteObject
    {
        internal enum ControlType
        {
            Up,
            Down,
            Left,
            Right
        }
        #region Class variable
        // A strongly typed reference to the game
        private DiscoRoboGame _game;

        // Type of control object
        internal ControlType InputType;
        
        // Tap happened?        
        internal int TapUpdateCount = 0;

        #endregion

        internal ControlObject(DiscoRoboGame game, Vector2 position, Texture2D texture)
            : base(game, position, texture)
        {
            _game = game;

            ScaleX = 0.33f;
            ScaleY = 0.33f;
        }

        internal ControlObject(DiscoRoboGame game, Vector2 position, Texture2D texture, ControlType type)
            : this(game, position, texture)
        {
            InputType = type;
        }

        public override void Update(GameTime gameTime)
        {
            if (TapUpdateCount > 0)
            {
                SpriteColor = Color.Red;
                TapUpdateCount--;
            } else
            {
                SpriteColor = Color.White;
            }            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

    }
}
