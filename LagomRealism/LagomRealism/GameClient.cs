using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LagomRealism
{
    class GameClient
    {
        NetClient client;
        private bool worldGenerated = false;
        private int seed;
        List<GameEntity> entities = new List<GameEntity>();
        List<Player> players = new List<Player>();
        private int ID;
        World world;
        GraphicsDevice graphics;
        Point worldSize;
        private bool receivedSeed = false;
        public GameClient(GraphicsDevice gd)
        {
            graphics = gd;
            NetPeerConfiguration config = new NetPeerConfiguration("lagom");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            client.Start();
            //(players)[0] represents the local player
            using (StreamReader sr = new StreamReader("./Config/Config.txt",Encoding.Default))
            {
                string[] b = (sr.ReadToEnd()).Split(':');
                while (client.ConnectionsCount == 0)
                {
                    client.Connect(b[0], 14242);
                }
                
            }
            
           // client.DiscoverLocalPeers(14242);
        
        }

        public void Update()
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
                                seed = msg.ReadInt32();
                                worldSize = new Point(msg.ReadInt32(), msg.ReadInt32());
                                receivedSeed = true;
                                break;
                            case MessageType.EntityUpdate:
                                entities.First(v1 => v1.ID == msg.ReadInt32()).State = (EntityState)msg.ReadInt32();
                                
                                break;
                            case MessageType.ClientPosition:
                                int id = msg.ReadInt32();
                                Vector2 vec = msg.ReadVector2();
                                bool connected = msg.ReadBoolean();
                                if (worldGenerated)
                                {
                                    if (!connected)
                                    {
                                        players.Remove(players.First(var => var.ID == id));
                                        break;
                                    }
                                    try
                                    {
                                        
                                        players.First(a => a.ID == id).Position = vec;
                                    }
                                    catch (Exception)
                                    {
                                        players.Add(new Player(world.HeightMap, id));
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
                players[0].Update();
                if (players[0].NeedUpdate)
                {
                    NetOutgoingMessage message = client.CreateMessage();
                    message.Write((int)MessageType.ClientPosition);
                    message.Write(ID);
                    message.Write(players[0].Position);
                    client.SendMessage(message, NetDeliveryMethod.Unreliable);
                    players[0].NeedUpdate = false;
                }
            }
            else if(receivedSeed)
            { 
                world = new World(seed);
                world.Load(worldSize,graphics);
                worldGenerated = true;
                players.Add(new Player(world.HeightMap, ID));
            }
        }

        public void Draw(SpriteBatch SB)
        {
            if (worldGenerated)
            {
                world.Draw(SB);
                foreach (Player player in players)
                {
                    player.Draw(SB);
                } 
            }
        }

        public void Close()
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((int)MessageType.ClientDisconnecting);
            msg.Write(ID);
            client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
        }
        
    }
}
