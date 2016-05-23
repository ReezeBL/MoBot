using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    internal class BasicRoutine : IRoutine
    {
        public void Logic()
        {
            if(GameController.Player.Health <= 0)
                ActionManager.Respawn();
        }
    }
}