using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism.Enteties
{
    class Rock : GameEntity
    {
        public Rock(int Id, Vector2 Position, string textureName = "Rock")
            : base(textureName, Id, Position)
        {
            base.Type = EntityType.Tree;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            if (base.State != 1)
            {
                sb.Draw(base.texture, new Vector2(base.Position.X - base.texture.Width / 2, base.Position.Y - base.texture.Height + 3), Color.White);
                base.Draw(sb);
            }

        }
        public override void Hit()
        {

            if (++base.numHits >= 3)
            {
                base.State = 1;
            }
        }
    }
}
