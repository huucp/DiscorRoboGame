using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace GameFramework
{
    public abstract class GameObjectBase
    {

        //-------------------------------------------------------------------------------------
        // Class constructors

        /// <summary>
        /// Constructor for the object
        /// </summary>
        /// <param name="game">A reference to the MonoGame Game class inside which the object resides</param>
        public GameObjectBase(GameHost game)
        {
            // Store a reference to the game
            Game = game;
        }

        //-------------------------------------------------------------------------------------
        // Property access

        /// <summary>
        /// A reference back to the game that owns the object
        /// </summary>
        protected GameHost Game { get; set; }

        /// <summary>
        /// The number of calls that have been made to the Update method
        /// </summary>
        public int UpdateCount { get; set; }

        /// <summary>
        /// A string that can be used to tag the object for later identification
        /// </summary>
        public string Tag { get; set; }

        //-------------------------------------------------------------------------------------
        // Game functions


        /// <summary>
        /// Update the object state
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            // Increment the UpdateCount
            UpdateCount += 1;
        }

        /// <summary>
        /// Determine whether the specified position is contained within the object
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool IsPointInObject(Vector2 point);

    }
}
