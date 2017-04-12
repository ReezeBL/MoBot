using TreeSharp;

namespace MoBot.Core.Game.AI.Tasks
{
    public class Protector : Task
    {
        private Entity targetEntity;

        private bool HasTraget(object context)
        {
            return targetEntity != null;
        }

        private void AttackTarget()
        {
            ActionManager.AttackEntity(targetEntity.Id);
        }

        public void SetTarget(Entity target)
        {
            targetEntity = target;
        }

        public Protector()
        {
            Root = new Decorator(HasTraget, new TreeSharp.Action(o => AttackTarget()));
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
