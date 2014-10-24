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
        Vector2 Position
        { get; set; }

        float Rotation
        { get; set; }

        void Draw(SpriteBatch sb);

        Texture2D Texture
        { get; set; }

        Vector2 Origin
        { get; set; }

        float Damage
        { get; set; }
        bool Flip
        { get; set; }
    }
}
