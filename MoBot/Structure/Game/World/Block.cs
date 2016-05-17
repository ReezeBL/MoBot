using System.Collections.Generic;

namespace MoBot.Structure.Game.World
{
    internal class Block
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
           CollidableBlocks = new List<int>();
           CollidableBlocks.Add(0);
           CollidableBlocks.Add(6);       
           CollidableBlocks.Add(30);
           CollidableBlocks.Add(31);
           CollidableBlocks.Add(32);
           CollidableBlocks.Add(50);
           CollidableBlocks.Add(51);
           CollidableBlocks.Add(65);
           CollidableBlocks.Add(66);
           CollidableBlocks.Add(69);
           CollidableBlocks.Add(70);
           CollidableBlocks.Add(72);
           CollidableBlocks.Add(76);
           CollidableBlocks.Add(77);
           CollidableBlocks.Add(106);
           CollidableBlocks.Add(131);
           CollidableBlocks.Add(143);
           CollidableBlocks.Add(147);
           CollidableBlocks.Add(148);
       }
    }
}
