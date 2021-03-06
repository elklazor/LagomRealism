﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LagomRealism.Enteties;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
namespace LagomRealism
{
    class GameClient: IFocusable, IDisposable
    {
        NetClient client;
        private bool worldGenerated = false;
        private string config;

        Texture2D BorderTexture;
        private int ID;
        GraphicsDevice graphics;
        Point worldSize;
        private bool receivedSeed = false;
        public Player thisPlayer;
        SpriteFont sf;
        private bool drawBoxes;

        public GameClient(GraphicsDevice gd)
        {
            graphics = gd;
            NetPeerConfiguration config = new NetPeerConfiguration("lagom");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            client.Start();
            
            using (StreamReader sr = new StreamReader("./Config/Config.txt",Encoding.Default))
            {
                string[] b = (sr.ReadToEnd()).Split(':');
                
                client.Connect(b[0], 14242);
                
            }
            
           // client.DiscoverLocalPeers(14242);
            BorderTexture = new Texture2D(this.graphics, 1, 1, false,SurfaceFormat.Color);
            BorderTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        client.Connect(msg.SenderEndPoint);
                        break;  

                    case NetIncomingMessageType.Data:

                        switch ((MessageType)msg.ReadInt32())
	                    {
                            case MessageType.WorldSeed:
                                ID = msg.ReadInt32();
                                config = msg.ReadString();
                                receivedSeed = true;
                                break;
                            case MessageType.EntityUpdate:
                                bool fullEntity = msg.ReadBoolean();
                                if (fullEntity)
                                {
                                    int id = msg.ReadInt32();
                                    int type = msg.ReadInt32();
                                    int state = msg.ReadInt32();
                                    int x = msg.ReadInt32();
                                    float y = msg.ReadFloat();
                                    
                                    switch ((EntityType)type)
                                    {
                                        case EntityType.Tree:
                                            World.Entities.Add(new Tree(id,new Vector2(x,y)));
                                            break;
                                        case EntityType.Rock:
                                            World.Entities.Add(new Rock(id, new Vector2(x, y),state));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    int id = msg.ReadInt32();
                                    int state = msg.ReadInt32();
                                    try
                                    {
                                        World.Entities.First(b => b.ID == id).State = state;
                                    }
                                    catch (Exception)
                                    { }
                                }
                                break;
                            case MessageType.ClientPosition:
                                int Id = msg.ReadInt32();
                                Vector2 vec = msg.ReadVector2();
                                bool connected = msg.ReadBoolean();
                                AnimationState aS = (AnimationState)msg.ReadInt32();
                                bool flip = msg.ReadBoolean();
                                Vector2 wepPos = msg.ReadVector2();
                                float wepRot = msg.ReadFloat();
                                bool isIdle = msg.ReadBoolean();

                                if (worldGenerated)
                                {
                                    if (!connected)
                                    {
                                        try
                                        {
                                            World.Players.Remove(World.Players.First(var => var.ID == Id));
                                        }
                                        catch (Exception)
                                        { 
                                        
                                        }
                                            break;
                                    }
                                    try
                                    {
                                        Player p = World.Players.First(a => a.ID == Id);
                                        p.Position = vec;
                                        p.AnimState = aS;
                                        p.Weapon.Rotation = wepRot;
                                        p.Weapon.Position = wepPos;
                                        p.IsIdle = isIdle;
                                        p.Effect = (flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                                    }
                                    catch (Exception)
                                    {
                                        World.Players.Add(new Player(Id));
                                    }
                                    
                                }

                                break;
                            case MessageType.ClientDisconnecting:
                                

                                break;
                            default:
                                break;
	                    }   
                        break;      
                }
            }

            //Input
            //If the world isn't generated, generate it and dont do any update logic
            if (worldGenerated)
            {
                //players[0].Update(gameTime);
                foreach (Player player in World.Players)
                {
                    player.Update(gameTime);
                }
                if (thisPlayer.NeedUpdate)
                {
                    NetOutgoingMessage message = client.CreateMessage();
                    message.Write((int)MessageType.ClientPosition); 
                    message.Write(ID);                                            //Player ID
                    message.Write(thisPlayer.Position);                           //Position
                    message.Write((int)thisPlayer.AnimState);                     //Animation state
                    message.Write((thisPlayer.Effect == SpriteEffects.None));     //Texture flip?
                    message.Write(thisPlayer.Weapon.Position);                    //Weaponposition
                    message.Write(thisPlayer.Weapon.Rotation);                    //Weaponrotation
                    message.Write(thisPlayer.IsIdle);                             //Is player idle?
                    client.SendMessage(message, NetDeliveryMethod.Unreliable);    //Send unreliable
                    thisPlayer.NeedUpdate = false;

                }
               
                List<GameEntity> entitiesNeedUpdate = World.Entities.Where(le => le.NeedUpdate).ToList();
                foreach (var locEnt in entitiesNeedUpdate)
                {
                    NetOutgoingMessage entityMessage = client.CreateMessage();
                    entityMessage.Write((int)MessageType.EntityUpdate);
                    entityMessage.Write(locEnt.ID);
                    entityMessage.Write(locEnt.State);
                    client.SendMessage(entityMessage, NetDeliveryMethod.Unreliable);
                    locEnt.NeedUpdate = false;
                }
            }
            else if(receivedSeed)
            { 
                
                World.StringLoad(config,graphics);
                worldGenerated = true;
                thisPlayer = new Player(ID);
                thisPlayer.IsLocal = true;
                World.Players.Add(thisPlayer);
            }

            ControlInput();
        }

        private void ControlInput()
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H))
            {
                drawBoxes = true;
            }
            else
                drawBoxes = false;
        }
        
        public void Draw(SpriteBatch SB)
        {
            if (worldGenerated)
            {
                World.Draw(SB);

                
                foreach (IGameObject ent in World.AllWorldEntities)
                {
                    ent.Draw(SB);
                    if(drawBoxes)
                        DrawBorder(ent.HitBox, 1, Color.Red, SB);
                } 
                
            }
        }
        public void Load(ContentManager content)
        {
            sf = content.Load<SpriteFont>("sFont");
        
        }
        public void Close()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((int)MessageType.ClientDisconnecting);
            msg.Write(ID);
            client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
        }


        public Vector2 Position
        {
            get 
            {
                if (worldGenerated)
                {
                    return thisPlayer.Position;
                }
                else
                    return Vector2.Zero;
            }
        }

        private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor,SpriteBatch spriteBatch)
        {
            // Draw top line
            spriteBatch.Draw(BorderTexture, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(BorderTexture, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(BorderTexture, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(BorderTexture, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }

        public void Dispose()
        {
            BorderTexture.Dispose();
        }
    }
}
