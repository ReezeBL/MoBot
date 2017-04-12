using MoBot.Protocol.Handlers;
using MoBot.Core.Game.Items;

namespace MoBot.Scripts.IndustrialCraft
{
    [MoBotExtension("Industrial Craft", "1.0")]
    public class IndustrialCraft
    {
        [Initialisation]
        public void OnLoad()
        {
            ClientHandler.RegisterCustomHandler("ic2", new IC2Handler());
            Item.RegisterItem("item.advDDrill", new ItemAdvancedDrill());
        }
    }
}
