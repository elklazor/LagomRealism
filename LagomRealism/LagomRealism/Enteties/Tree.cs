using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism.Enteties
{
    class Tree:GameEntity
    {
        public Tree(EntityType type, int Id, Vector2 Position,string textureName = "Tree")
           : base(type,textureName,Id,Position)
        {        
            
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if(base.State != EntityState.Broken)
            {
                sb.Draw(base.texture, new Vector2(base.Position.X + base.texture.Width / 2, base.Position.Y + base.texture.Height), Color.White);
            }
            
        }
        public override void Hit()
        {
            
            if (++base.numHits >= 3)
            {
                base.State = EntityState.Broken;
            }
        }
    }
}
