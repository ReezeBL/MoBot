using System;
using System.Collections.Concurrent;
using System.Threading;
using MoBot.Structure.Game.AI.Tasks;
using TreeSharp;
using Action = System.Action;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private bool _threadContinue = true;
        private Composite _root;
        public Protector Protector { get; } = new Protector();
        public Surviver Surviver { get; } = new Surviver();
        public Mover Mover { get;} = new Mover();

        private int _flyingTicks;

        public AiHandler()
        {
            var aiThread = new Thread(() =>
            {
                while (_threadContinue)
                {
                    if (NetworkController.Connected && GameController.Player != null)
                    {
                        if (_root.Tick(null) != RunStatus.Running)
                        {
                            _root.Stop(null);
                            _root.Start(null);
                        }
                    }

                    else
                    {
                        _root.Stop(null);
                        _root.Start(null);
                    }

                    Thread.Sleep(50);
                }
            })
            { IsBackground = true };

            CreateRoot();
            _root.Start(null);

            aiThread.Start();
        }

        private void CreateRoot()
        {
            _root = new PrioritySelector(Protector, Surviver, Mover, new TreeSharp.Action());
        }
    }

    

    
}