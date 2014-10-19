using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LagomRealism.Weapons
{
    class Sword:IWeapon
    {
        private Vector2 position;
        private float rotation;
        private Texture2D texture;
        private Vector2 origin;
        private Rectangle? wepRectangle;
        private bool flip;
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public bool Flip
        {
            get { return flip; }
            set { flip = value; }
        }
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get{ return rotation; }
            set{ rotation = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Rectangle((int)position.X,((int)position.Y,texture.Width,texture.Height), wepRectangle,
                Color.White, rotation, origin, (flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
