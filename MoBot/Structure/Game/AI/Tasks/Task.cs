using System.Collections;
using System.Diagnostics;
using TreeSharp;

namespace MoBot.Structure.Game.AI.Tasks
{
    public class Task
    {
        protected Composite Root;
        protected static readonly object Awaiter = new object();
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
            return obj.Root;
        }

        public static IEnumerator WaitForSeconds(long milliseconds)
        {
            var time = new Stopwatch();
            time.Start();
            while (time.ElapsedMilliseconds < milliseconds)
            {
                yield return Awaiter;
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
