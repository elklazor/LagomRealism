using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    abstract class GameEntity: IGameObject
    {
        private int id;
        protected Texture2D texture;
        public Vector2 Position { get; set; }
        protected int numHits = 0;
        public Rectangle CollisionRectangle;
        public bool NeedUpdate { get; set; }
        public virtual Rectangle HitBox
        {
            get { return new Rectangle((int)Position.X - texture.Width / 2, (int)Position.Y - texture.Height, texture.Width, texture.Height); }
        }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private EntityType type;
        internal EntityType Type
        {
            get { return type; }
            set { type = value; }
        }
                
        private int state;

        internal int State
        {
            get { return state; }
            set
            { state = value; }
        }
        public GameEntity(string textureName, int Id,Vector2 position)
        {
            Position = position;
            
            texture = TextureManager.TextureCache[textureName];
            ID = Id;
            CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public virtual void Update(GameTime gameTime)
        { 
            
        }

        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(texture, new Vector2(Position.X - texture.Width / 2,Position.Y - texture.Height / 2), Color.White);
            sb.DrawString(TextureManager.StandardGameFont, ID.ToString(), Position, Color.Yellow);
        }

        public virtual void Hit()
        {
            NeedUpdate = true;
        }

        
    }
}
