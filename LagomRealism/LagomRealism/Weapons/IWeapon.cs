using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LagomRealism.Weapons
{
    interface IWeapon
    {
        public Vector2 Position
        { get; set; }

        public float Rotation
        { get; set; }

        public void Draw(SpriteBatch sb);

        public Texture2D Texture
        { get; set; }

        public Vector2 Origin
        { get; set; }

        public float Damage
        { get; set; }
    }
}
