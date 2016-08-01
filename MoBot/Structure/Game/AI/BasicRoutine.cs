namespace MoBot.Structure.Game.AI
{
    public class BasicRoutine : IRoutine
    {
        public void Logic()
        {
            if(GameController.Player.Health <= 0)
                ActionManager.Respawn();
        }
    }
}