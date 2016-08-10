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

        public override bool CanHarvest(Block block)
        {
            return ToolClasses.Contains(block.HarvestTool);
        }

        public override float GetItemStrength(ItemStack stack, Block block)
        {
            return CanHarvest(block) ? Effectivness : base.GetItemStrength(stack, block);
        }
    }
}
