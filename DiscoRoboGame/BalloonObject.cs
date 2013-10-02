using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiscoRoboGame
{
    internal class BalloonObject : SpriteObject
    {
        #region Class variable
        // A strongly typed reference to the game
        private DiscoRoboGame _game;

        // The current speed
        private float _speed;

        // Is this balloon active? If nit it will ignore calls to 
        // Update and Draw until reactivate        

        internal bool IsActive { get; set; }
        internal ControlObject.ControlType BalloonType = ControlObject.ControlType.Right;
        #endregion

        internal BalloonObject(DiscoRoboGame game, Texture2D texture, Vector2 position)
            : base(game, position, texture)
        {
            _game = game;
            IsActive = false;
            Scale = new Vector2(0.3f, 0.3f);
            _speed = 5;
            SpriteColor = Color.Blue;
        }


        // Reset properties for another use
        internal void ResetProperties(Texture2D texture, Vector2 position, ControlObject.ControlType type)
        {
            Position = position;
            SpriteTexture = texture;
            BalloonType = type;
        }

        public override void Update(GameTime gameTime)
        {
            // Exit if not currently active
            if (!IsActive) return;


            // Move the balloon
            PositionY += _speed;

            // Check when the balloon pass the bottom
            if (PositionY > DiscoRoboGame.ScoreRowEndPos)
            {
                IsActive = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Exit if not currently active
            if (!IsActive) return;

            base.Draw(gameTime, spriteBatch);
        }
    }
}
