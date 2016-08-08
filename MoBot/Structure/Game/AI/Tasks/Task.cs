using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Task
    {
        protected Composite _root;
        protected readonly object _awaiter = new object();
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

        protected IEnumerator WaitForSeconds(long milliseconds)
        {
            var time = new Stopwatch();
            time.Start();
            while (time.ElapsedMilliseconds < milliseconds)
            {
                yield return _awaiter;
            }
        }
    }
}
