using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.Items
{
    public class ItemTool : Item
    {
        public float Effectivness = 1.0f;
        public HashSet<string> ToolClasses;
        public int[] ClassLevels;

        public bool IsItemEffective(GameBlock block)
        {
            return ToolClasses.Contains(block.HarvestTool);
        }

        protected override float GetItemEffectivness(GameBlock block)
        {
            return IsItemEffective(block) ? Effectivness : base.GetItemEffectivness(block);
        }
    }
}
