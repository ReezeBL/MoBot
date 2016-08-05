using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    class Surviver : Task
    {
        public int MinFoodDanger = 5;
        public double MinHealthDanger = 5;
        public string Teleport;

        private bool DangerHealth(object context)
        {
            return GameController.Player.Health <= MinHealthDanger;
        }

        private bool IsHungry(object context)
        {
            return GameController.Player.Food <= MinFoodDanger;
        }

        private void Feed()
        {
            
        }

        private void TeleportOut()
        {
            
        }

        public Surviver()
        {
            _root = new PrioritySelector(new Decorator(IsHungry, new Action(o =>Feed())));
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
