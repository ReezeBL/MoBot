using System.Collections.Generic;

namespace MoBot.Structure.Game.World
{
    public class Block
    {
       public static List<int> CollidableBlocks;
       public int Id;
       public string Name;
       public int X;
       public int Y;
       public int Z;
       public int Cx;
       public int Cz;

       public Block(int id, int x, int y, int z,int cx, int cz)
       {
           Id = id;
           X = x;
           Y = y;
           Z = z;
           Cx = cx;
           Cz = cz;

           Name = "";
       }
       public static void Init()
       {
           CollidableBlocks = new List<int>
           {
               0,
               6,
               30,
               31,
               32,
               50,
               51,
               65,
               66,
               69,
               70,
               72,
               76,
               77,
               106,
               131,
               143,
               147,
               148
           };

       }
    }
}
