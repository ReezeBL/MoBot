using System;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Surviver : Task
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

        private bool IsDead(object context)
        {
            return Math.Abs(GameController.Player.Health) < 1;
        }

        private void Ressurect()
        {
            ActionManager.Respawn();
        }

        public Surviver()
        {
            _root = new PrioritySelector(new Decorator(IsDead, new Action(o => Ressurect())), new Decorator(IsHungry, new Action(o =>Feed())));
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
