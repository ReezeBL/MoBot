using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    internal class BasicRoutine : IRoutine
    {
        public Task<bool> Logic()
        {
            return new Task<bool>(() => true);
        }
    }
}