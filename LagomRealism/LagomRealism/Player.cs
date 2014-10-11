using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    class Player: IFocusable
    {
        private Vector2 pos;
        private Vector2 velocity = Vector2.Zero;
        public float[] heightMap;
        private bool canJump = true;
        private Texture2D texture;
        public int ID;
        public bool NeedUpdate = false;
        private Vector2 prevPos = Vector2.Zero;
        private SpriteEffects effect = SpriteEffects.None;
        private Rectangle collisionRectangle;
        private AnimationState animState;
        private float frameTime = 150;
        private float frameTimer = 0;

        internal AnimationState AnimState
        {
            get { return animState; }
            private set { animState = value; }
        }
        
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }
        
        public Player(float[] hm, int id)
        {
            texture = TextureManager.TextureCache["Texture"]; 
            heightMap = hm;
            ID = id;
        }

        public void Update(GameTime gameTime)
        {
            //Input

            if (velocity.Y < 0.5f)
                velocity.Y += velocity.Y / 1.5f;
            //if (velocity.Y < 0.2f)
             //   velocity.Y = 0f;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                effect = SpriteEffects.FlipHorizontally;
                velocity.X = -0.5f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                effect = SpriteEffects.None;
                velocity.X = 0.5f;
            }
            else
                velocity.X = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && canJump)
            {
                velocity.Y += -20f;
                canJump = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                canJump = true;   
            }
            float locX = Position.X + (texture.Width);
            Vector2 vec = new Vector2(locX, pos.Y + texture.Height);
            if (vec.Y >= heightMap[(int)Math.Floor(pos.X)]+5)
            {
                pos.Y = heightMap[(int)Math.Floor(pos.X)] - texture.Height + 5;
            }
            else
                velocity.Y = 0.5f;

            pos += velocity;
            if (pos != prevPos)
                NeedUpdate = true;

            if (velocity.X != 0)
                Animate(gameTime);
            else
                animState = AnimationState.None;

            prevPos = pos;
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        private void Animate(GameTime gT)
        { 
           if((frameTimer += gT.ElapsedGameTime.Milliseconds) >= frameTime)
           {
               animState++;
               if ((int)animState > 4)
                   animState = (AnimationState)1;

               frameTimer = 0f;
           }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle src = new Rectangle(5 * ((int)animState), 0, 5, 10);
            //sb.Draw(texture,collisionRectangle,null,Color.White,0f,Vector2.Zero,effect,0f);
            sb.Draw(texture, Position, src, Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
            //sb.Draw(texture, Position, Color.White);
            
        }



        Vector2 IFocusable.Position
        {
            get { return Position; }
        }
    }
}
