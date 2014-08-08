using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    abstract class GameEntity
    {
        private int id;
        protected Texture2D texture;
        public Vector2 Position { get; set; }
        protected int numHits = 0;
        public Rectangle CollisionRectangle;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private EntityType type;
        internal EntityType Type
        {
            get { return type; }
        }
                
        private EntityState state;

        internal EntityState State
        {
            get { return state; }
            set { state = value; }
        }
        public virtual GameEntity(EntityType entityType, string textureName, int Id,Vector2 position)
        {
            Position = position;
            type = entityType;
            texture = TextureManager.TextureCache[textureName];
            ID = Id;
            CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public virtual void Update(GameTime gameTime)
        { 
        
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Position, Color.White);
        }

        public virtual void Hit()
        { 
            //Is called by player
            numHits++;
        }
    }
}
