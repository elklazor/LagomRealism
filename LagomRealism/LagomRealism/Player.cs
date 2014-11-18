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
        private Vector2 handPos = new Vector2(19, 20);
        private SpriteEffects effect = SpriteEffects.None;
        private IWeapon playerWeapon = new Sword();
        private float prevWeaponRotation = 0;
        public bool IsLocal = false;
        private bool idleSynced = false;
        public bool IsIdle
        {
            get { return idle; }
            set { idle = value; }
        }
        public IWeapon Weapon
        {
            get { return playerWeapon; }
            set { playerWeapon = value; }
        }
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
            if (IsLocal)
            {
                prevWeaponRotation = playerWeapon.Rotation;
                //Input
                velocity.Y += 0.3f;
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
                    velocity.Y += -5f;
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
                    pos.Y = heightMap[(int)Math.Floor(pos.X)] - (texture.Height / 2) + 5;
                    velocity.Y = 0;
                }
                if (velocity.X == 0f)
                {
                    idle = true;
                    idleSynced = false;
                }

                if (pos != prevPos || !idle || prevWeaponRotation != playerWeapon.Rotation)
                {
                    NeedUpdate = true;
                    frameTime = 700f;
                }
                else
                    frameTime = 500f;

                if (!idleSynced)
                {
                    NeedUpdate = true;
                    idleSynced = true;
                }
                

                Animate(gameTime);

                prevPos = pos; 
            }

            bool flip = (effect == SpriteEffects.FlipHorizontally);
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            playerWeapon.Position = (flip) ? new Vector2(Position.X + 3,Position.Y + handPos.Y) : Position + handPos ;
            //playerWeapon.Position = Position + handPos;
            if (IsLocal)
            {
                playerWeapon.Flip = flip;
            }
            else
                playerWeapon.Flip = flip;
          
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
            playerWeapon.Draw(sb);
            //sb.Draw(texture, Position, Color.White);
            
        }



        Vector2 IFocusable.Position
        {
            get { return Position; }
        }
    }
}
