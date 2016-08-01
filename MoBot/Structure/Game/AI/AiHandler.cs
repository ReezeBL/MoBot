using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private IRoutine _mainRoutine;
        private readonly Thread _aiThread;
        private readonly ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>();
        private bool _threadContinue = true;

        public AiHandler(IRoutine routine)
        {
            _mainRoutine = routine;
            _aiThread = new Thread(() =>
            {
                while (_threadContinue)
                {
                    if (NetworkController.Connected && GameController.Player != null)
                    {
                        while (_tasks.Count > 0)
                        {
                            Action result;
                            _tasks.TryDequeue(out result);
                            result.Invoke();
                        }
                        _mainRoutine.Logic();
                    }
                    
                    Thread.Sleep(50);
                }               
            }) {IsBackground = true};
            _aiThread.Start();
        }
        public void HookNewRoutine(IRoutine routine)
        {
            _threadContinue = false;
            _aiThread.Join();
            _mainRoutine = routine;
            _threadContinue = true;
            _aiThread.Start();
        }

        public void EnqueueTask(Action func)
        {
            _tasks.Enqueue(func);
        }
    }
}