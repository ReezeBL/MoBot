using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Task
    {
        protected Composite _root;

        /// <summary>
        /// Определяет очереденость исполнения заданий
        /// </summary>
        /// <returns></returns>
        public virtual int GetPriority()
        {
            return 0;
        }

        public static implicit operator Composite(Task obj)
        {
            return obj._root;
        }
    }
}
