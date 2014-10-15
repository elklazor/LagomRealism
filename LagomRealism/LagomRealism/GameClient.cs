using System;
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
namespace LagomRealism
{
    class GameClient: IFocusable
    {
        NetClient client;
        private bool worldGenerated = false;
        private string config;
        List<GameEntity> entities = new List<GameEntity>();
        List<Player> players = new List<Player>();
        private int ID;
        World world;
        GraphicsDevice graphics;
        Point worldSize;
        private bool receivedSeed = false;
        public Player thisPlayer;
        SpriteFont sf;

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
                                    int x = msg.ReadInt32();
                                    float y = msg.ReadFloat();
                                    switch ((EntityType)type)
                                    {
                                        case EntityType.Tree:
                                            entities.Add(new Tree(id,new Vector2(x,y)));
                                            break;
                                        case EntityType.Rock:
                                            entities.Add(new Rock(id, new Vector2(x, y)));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    int id = msg.ReadInt32();
                                    entities.First(b => b.ID == id).State = (EntityState)msg.ReadInt32();
                                }
                                break;
                            case MessageType.ClientPosition:
                                int Id = msg.ReadInt32();
                                Vector2 vec = msg.ReadVector2();
                                bool connected = msg.ReadBoolean();
                                AnimationState aS = (AnimationState)msg.ReadInt32();
                                bool flip = msg.ReadBoolean();
                                if (worldGenerated)
                                {
                                    if (!connected)
                                    {
                                        players.Remove(players.First(var => var.ID == Id));
                                        break;
                                    }
                                    try
                                    {
                                        Player p = players.First(a => a.ID == Id);
                                        p.Position = vec;
                                        p.AnimState = aS;
                                        p.Effect = (flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                                    }
                                    catch (Exception)
                                    {
                                        players.Add(new Player(world.HeightMap, Id));
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
                players[0].Update(gameTime);
                if (players[0].NeedUpdate)
                {
                    NetOutgoingMessage message = client.CreateMessage();
                    message.Write((int)MessageType.ClientPosition); 
                    message.Write(ID);                                            //Player ID
                    message.Write(players[0].Position);                           //Position
                    message.Write((int)players[0].AnimState);                     //Animation state
                    message.Write((players[0].Effect == SpriteEffects.None));     //Texture flip?
                    client.SendMessage(message, NetDeliveryMethod.Unreliable);
                    players[0].NeedUpdate = false;
                }
            }
            else if(receivedSeed)
            { 
                world = new World();
                world.StringLoad(config,graphics);
                worldGenerated = true;
                thisPlayer = new Player(world.HeightMap, ID);
                players.Add(thisPlayer);
            }
        }
        
        public void Draw(SpriteBatch SB)
        {
            if (worldGenerated)
            {
                world.Draw(SB);
                
                foreach (GameEntity ent in entities)
                {
                    ent.Draw(SB);
                }
                foreach (Player player in players)
                {
                    player.Draw(SB);
                }
                //SB.DrawString(sf, players[0].Velocity.Y.ToString("f3"), players[0].Position, Color.Tomato);
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
    }
}
