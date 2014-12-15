using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Diagnostics;
using System.Collections.ObjectModel;
namespace LagomRealism
{
    static class World
    {

        static private float[] heightMap;
        static private GraphicsDevice Gd;
        static private Texture2D worldTexture;
        static private int seed;
        static private int jump;
        static private int maxChange;
        static public ObservableCollection<GameEntity> Entities = new ObservableCollection<GameEntity>();
        static public ObservableCollection<Player> Players = new ObservableCollection<Player>();
        static public List<IGameObject> AllWorldEntities = new List<IGameObject>();
        
        public static int MaxChange
        {
            get { return maxChange; }
            set { maxChange = value; }
        }

        public static int Jump
        {
            get { return jump; }
            set { jump = value; }
        }
        public static int Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        public static Texture2D WorldTexture
        {
            get { return worldTexture; }
            set { worldTexture = value; }
        }

        private static  Point[] pointArr;
        private static Point imageSize;

        public static float[] HeightMap
        {
            get { return heightMap; }
            set { heightMap = value; }
        }

        static World()
        {
            Players.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(players_CollectionChanged);
            Entities.CollectionChanged += players_CollectionChanged;
        }

        static void players_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AllWorldEntities.Clear();
            AllWorldEntities.AddRange(Entities);
            AllWorldEntities.AddRange(Players);
        }
        public static void SetSeed(int seed)
        {
            Seed = seed;
        }
        public static void SetSeed()
        {
            Random rnd = new Random();
            Seed = rnd.Next(100, 90000);
        }
        public static void Load(Point imgSize,GraphicsDevice gd)
        {
            imageSize = imgSize;
            Gd = gd;
            Generate();
        }
        /// <summary>
        /// Loads world with config file
        /// </summary>
        /// <param name="configName">Name of config file without extension</param>
        public static void Load(string configName)
        {
            Console.WriteLine("Loading config...");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("./WorldConfig/" + configName + ".xml");
            string[] size = (xDoc.SelectSingleNode("CONFIG//SIZE").InnerText).Split(':');
            imageSize = new Point(Convert.ToInt32(size[0]),Convert.ToInt32(size[1]));
            Seed = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//SEED").InnerText);
            maxChange = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//MAXCHANGE").InnerText);
            Jump = Convert.ToInt32(xDoc.SelectSingleNode("CONFIG//JUMP").InnerText);
            Console.WriteLine(@"/***** Config *****\");
            Console.WriteLine(WorldConfigToString());
            Console.WriteLine(@"\******************/");
            ServerGenerate();
        }
        public static void StringLoad(string config,GraphicsDevice gd)
        {
            Gd = gd;
            string[] conf = config.Split('|');
            string[] sizArr = conf[0].Split(':');
            imageSize = new Point(Convert.ToInt32(sizArr[0]),Convert.ToInt32(sizArr[1]));
            seed = Convert.ToInt32(conf[1]);
            Jump = Convert.ToInt32(conf[2]);
            MaxChange = Convert.ToInt32(conf[3]);
            Debug.WriteLine(WorldConfigToString());
            Generate();

        }
        public static void Load(int x, int y)
        {
            imageSize = new Point(x, y);
            ServerGenerate();
        }

        private static void Generate()
        {
            // Array size: (screen width / jump) + 3
            pointArr = new Point[(imageSize.X / jump) +3];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,jump,maxChange);
            worldTexture = TerrainManager.PolygonToTexture(Gd, pointArr, imageSize);
        }

        private static void ServerGenerate()
        {
            pointArr = new Point[imageSize.X];
            heightMap = TerrainManager.GenerateTerrain(ref pointArr, imageSize.Y / 2,Seed,jump,maxChange);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(worldTexture, Vector2.Zero, Color.White);
            
        }

        public static string WorldConfigToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(imageSize.X + ":" + imageSize.Y);
            sb.Append("|");
            sb.Append(seed.ToString());
            sb.Append("|");
            sb.Append(jump.ToString());
            sb.Append("|");
            sb.Append(maxChange.ToString());
            return sb.ToString();
        }
    }
}
