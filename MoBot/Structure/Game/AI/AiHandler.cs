using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Structure.Game.AI
{
    internal class AiHandler
    {
        private IRoutine _mainRoutine;
        private readonly Thread _aiThread;
        private readonly ConcurrentQueue<Task<bool>> _tasks = new ConcurrentQueue<Task<bool>>();
        private bool _threadContinue = true;

        public AiHandler(IRoutine routine)
        {
            _mainRoutine = routine;
            _aiThread = new Thread(async () =>
            {
                while (_threadContinue)
                {
                    while (_tasks.Count > 0)
                    {
                        Task<bool> result;
                        _tasks.TryDequeue(out result);
                        await result;
                    }
                    Thread.Sleep(50);
                }
                _tasks.Enqueue(routine.Logic());
            }) {IsBackground = true};
        }
        public void HookNewRoutine(IRoutine routine)
        {
            _threadContinue = false;
            _aiThread.Join();
            _mainRoutine = routine;
            _aiThread.Start();
        }

        public void EnqueueTask(Func<bool> func)
        {
            _tasks.Enqueue(new Task<bool>(func));
        }
    }
}