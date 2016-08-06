using System.Collections.Generic;

namespace MoBot.Structure.Game.World
{
    public class Block
    {
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

        public static implicit operator GameBlock(Block block)
        {
            return GameBlock.GetBlock(block.Id);
        }
    }
}
