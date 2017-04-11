using MoBot.Protocol.Handlers;
using MoBot.Structure.Game.Items;

namespace MoBot.Scripts.IndustrialCraft
{
    [MoBotExtension("Industrial Craft", "1.0")]
    public class IndustrialCraft
    {
        [Initialisation]
        public void OnLoad()
        {
            ClientHandler.CustomHandlers.Add("ic2", new IC2Handler());
            Item.Extension.Add("item.advDDrill", new ItemAdvancedDrill());
        }
    }
}
