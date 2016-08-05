using System;
using System.Collections.Concurrent;
using System.Threading;
using TreeSharp;
using Action = System.Action;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private readonly ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>();
        private bool _threadContinue = true;
        private Composite _root;

        public AiHandler()
        {
            var aiThread = new Thread(() =>
            {
                while (_threadContinue)
                {
                    if (NetworkController.Connected && GameController.Player != null)
                    {
                        while (_tasks.Count > 0)
                        {
                            Action result;
                            if(_tasks.TryDequeue(out result))
                                result.Invoke();
                        }

                        _root.Tick(null);

                        if (_root.LastStatus != RunStatus.Running)
                        {
                            _root.Stop(null);
                            _root.Start(null);
                        }
                    }
                    else
                    {
                        _threadContinue = false;
                    }
                    
                    Thread.Sleep(50);
                }               
            }) {IsBackground = true};

            _root.Start(null);
            aiThread.Start();
        }

        public void EnqueueTask(Action func)
        {
            _tasks.Enqueue(func);
        }
    }

    

    
}