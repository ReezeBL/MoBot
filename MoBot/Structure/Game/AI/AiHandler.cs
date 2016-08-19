using System;
using System.Threading;
using MoBot.Structure.Game.AI.Tasks;
using TreeSharp;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private bool _threadContinue = true;
        private Composite _root;
        public Protector Protector { get; } = new Protector();
        public CustomEvents CustomEvents { get; } = new CustomEvents();
        public Mover Mover { get;} = new Mover();
        public Digger Digger { get; } = new Digger();

        private int _flyingTicks;
        private bool _paused = true;

        public AiHandler()
        {
            var aiThread = new Thread(() =>
            {
                while (_threadContinue)
                {
                    try
                    {
                        if (NetworkController.Connected && GameController.Player != null)
                        {
                            if (_paused)
                            {
                                _paused = false;
                                CustomEvents.GenerateStrings();
                                _root.Stop(null);
                                _root.Start(null);
                            }

                            if (_root.Tick(null) != RunStatus.Running)
                            {
                                _root.Stop(null);
                                _root.Start(null);
                            }

                            if (!GameController.Player.OnGround)
                                GameController.Player.OnGround = true;
                        }
                        else
                        {
                            _paused = true;
                            _flyingTicks = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Program.GetLogger().Error($"AI Thread exception: {e}");
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
            _root = new PrioritySelector(CustomEvents, Mover, Digger);
        }
    }

    

    
}