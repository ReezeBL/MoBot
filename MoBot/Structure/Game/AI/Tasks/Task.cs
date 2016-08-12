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
        protected static readonly object _awaiter = new object();
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

        public static IEnumerator WaitForSeconds(long milliseconds)
        {
            var time = new Stopwatch();
            time.Start();
            while (time.ElapsedMilliseconds < milliseconds)
            {
                yield return _awaiter;
            }
        }

        protected IEnumerator StartRoutine(IEnumerator routine)
        {
            while (routine.MoveNext())
            {
                var current = routine.Current;
                if (current is IEnumerator)
                {
                    var coroutine = StartRoutine((IEnumerator)current);
                    while (coroutine.MoveNext())
                        yield return coroutine.Current;
                }
                else if (current is IEnumerable)
                {
                    var coroutine = StartRoutine(((IEnumerable)current).GetEnumerator());
                    while (coroutine.MoveNext())
                        yield return coroutine.Current;
                }
                else
                {
                    yield return current;
                }
            }
        }
    }
}
