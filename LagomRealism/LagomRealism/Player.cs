using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LagomRealism.Weapons;
namespace LagomRealism
{
    class Player: IFocusable
    {
        private Vector2 pos;
        private Vector2 velocity = Vector2.Zero;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public float[] heightMap;
        public bool canJump = true;
        private bool idle;
        private Texture2D texture;
        public int ID;
        public bool NeedUpdate = false;
        private Vector2 prevPos = Vector2.Zero;
        private Vector2 handPos = new Vector2(19, 19);
        private SpriteEffects effect = SpriteEffects.None;
        private IWeapon playerWeapon = new Sword();
        public SpriteEffects Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        private Rectangle collisionRectangle;
        private AnimationState animState;
        private float frameTime = 300;
        private float frameTimer = 0;

        internal AnimationState AnimState
        {
            get { return animState; }
            set { animState = value; }
        }
        
        public Vector2 Position
        {
            get { return pos; }
            set { pos = value; }
        }
        
        public Player(float[] hm, int id)
        {
            texture = TextureManager.TextureCache["Knight"]; 
            heightMap = hm;
            ID = id;
        }

        public void Update(GameTime gameTime)
        {
            //Input
            velocity.Y += 0.7f;
            idle = false;
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
                velocity.Y += -10f;
                canJump = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                canJump = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                playerWeapon.Rotation += 0.05f;

            pos += velocity;
            
            Vector2 vec = new Vector2(pos.X, pos.Y + (texture.Height / 2));

            if (vec.Y >= heightMap[(int)Math.Floor(pos.X)] + 5)
            {
                pos.Y = heightMap[(int)Math.Floor(pos.X)] - (texture.Height / 2) +5;
                velocity.Y = 0;
            }
            if (velocity.X == 0f)
                idle = true;

            if (pos != prevPos || !idle)
            {
                NeedUpdate = true;
                frameTime = 300f;
            }
            else
                frameTime = 500f;

            Animate(gameTime);

            prevPos = pos;
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        private void Animate(GameTime gT)
        { 
           if((frameTimer += gT.ElapsedGameTime.Milliseconds) >= frameTime)
           {
               animState++;
               if ((int)animState > 3)
                   animState = (AnimationState)0;

               frameTimer = 0f;
           }
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle src = new Rectangle(21 * ((int)animState), (idle)? 34 : 0 , 21, 34);
            //sb.Draw(texture,collisionRectangle,null,Color.White,0f,Vector2.Zero,effect,0f);
            sb.Draw(texture, Position, src, Color.White, 0f, Vector2.Zero, 1f, effect, 0f);
            playerWeapon.Position = Position + handPos;
            
             playerWeapon.Draw(sb);
            //sb.Draw(texture, Position, Color.White);
            
        }



        Vector2 IFocusable.Position
        {
            get { return Position; }
        }
    }
}
