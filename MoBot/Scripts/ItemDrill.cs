using MoBot.Structure.Game.Items;

namespace MoBot.Scripts
{
    public class ItemDrill : ItemTool
    {
        [ImportAttribue.PreInit]
        public static void Import()
        {
            Extension.Add("IC2:itemToolDrill", new ItemDrill());
        }
    }
}
