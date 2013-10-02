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
    internal sealed class LineObject: SpriteObject
    {
        public LineObject(DiscoRoboGame game, Vector2 position, Texture2D texture)
            : base(game, position, texture)
        {
        }
    }
}
