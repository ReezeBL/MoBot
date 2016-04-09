using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftEmuPTS.GameData
{
    class Block
    {
       public static List<int> CollidableBlocks;
       public int ID;
       public string Name;
       public int x;
       public int y;
       public int z;
       public int cx;
       public int cz;

       public Block(int id, int X, int Y, int Z,int CX, int CZ)
       {
           ID = id;
           x = X;
           y = Y;
           z = Z;
           cx = CX;
           cz = CZ;

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
