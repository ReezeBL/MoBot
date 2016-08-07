using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Protector : Task
    {
        private Entity _targetEntity;

        private bool HasTraget(object context)
        {
            return _targetEntity != null;
        }

        private void AttackTarget()
        {
            ActionManager.AttackEntity(_targetEntity.Id);
        }

        public void SetTarget(Entity target)
        {
            _targetEntity = target;
        }

        public Protector()
        {
            _root = new Decorator(HasTraget, new TreeSharp.Action(o => AttackTarget()));
        }

        public override int GetPriority()
        {
            return 0;
        }
    }
}
