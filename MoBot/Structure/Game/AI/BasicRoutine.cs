using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    internal class BasicRoutine : IRoutine
    {
        public Task Logic()
        {
            return new Task(action: () => { });
        }
    }
}