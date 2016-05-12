using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using MoBot.Structure.Game.AI.Pathfinding;
using System.Runtime.Serialization;

namespace MoBot.Structure.Game.AI.Modules
{
    class AntiAFK : AIModule
    {
        public List<Point> wayPoints = new List<Point>();
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Point[]));     

        public AntiAFK()
        {
            try {
                using (var file = new FileStream("Settings/AntiAFKWayPoints.json", FileMode.Open))
                {
                    wayPoints = new List<Point>((Point[])ser.ReadObject(file));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public sbyte dir { get; set; } = 1;
        public double delay { get; set; } = 2;     
        public override void tick()
        {
            try {
                double diff = (DateTime.Now - mainAIController.lastMove).TotalSeconds;
                if (diff >= delay)
                {
                    Player player = mainAIController.player;
                    var playerPoint = new Pathfinding.PathPoint { x = (int)player.x, y = (int)player.y, z = (int)player.z };
                    var minDist = wayPoints.Min(e => e.DistanceTo(playerPoint));
                    var pathPoint = wayPoints.Where(e => e.DistanceTo(playerPoint) == minDist).FirstOrDefault();
                    pathPoint = wayPoints[(wayPoints.IndexOf(pathPoint) + 1) % wayPoints.Count];
                    ((Movement)mainAIController.aiHandler.moduleList["Movement"].module).destPoint = pathPoint;
                    mainAIController.model.viewer.OnNext(new Actions.ActionMessage { message = $"Anti-afk move to {pathPoint.x}|{pathPoint.y}|{pathPoint.z}" });
                }
            }
            catch (Exception) { }
        }

        public void Add(int x, int y, int z)
        {
            wayPoints.Add(new Point { x = x, y = y, z = z });
            using (var file = new FileStream("Settings/AntiAFKWayPoints.json", FileMode.OpenOrCreate)) 
                ser.WriteObject(file, wayPoints.ToArray());
        }
    }
}
