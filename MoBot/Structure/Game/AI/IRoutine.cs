using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    internal interface IRoutine
    {
        Task Logic();
    }
}