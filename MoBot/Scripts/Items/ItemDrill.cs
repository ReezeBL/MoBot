using MoBot.Structure.Game.Items;

namespace MoBot.Scripts.Items
{
    public class ItemDrill : ItemTool
    {
        [ImportHandler.PreInit]
        public static void Import()
        {
            Extension.Add("IC2:itemToolDrill", new ItemDrill());
        }
    }
}
