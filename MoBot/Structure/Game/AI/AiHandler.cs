using System;
using System.Threading;
using MoBot.Structure.Game.AI.Tasks;
using TreeSharp;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private bool threadContinue = true;
        private Composite root;
        public Protector Protector { get; } = new Protector();
        public CustomEvents CustomEvents { get; } = new CustomEvents();
        public Mover Mover { get;} = new Mover();
        public Digger Digger { get; } = new Digger();

        private bool paused = true;

        public AiHandler()
        {
            var aiThread = new Thread(() =>
            {
                while (threadContinue)
                {
                    try
                    {
                        if (NetworkController.Connected && GameController.Player != null)
                        {
                            if (paused)
                            {
                                paused = false;
                                CustomEvents.GenerateStrings();
                                root.Stop(null);
                                root.Start(null);
                            }

                            if (root.Tick(null) != RunStatus.Running)
                            {
                                root.Stop(null);
                                root.Start(null);
                            }

                            if (!GameController.Player.OnGround)
                                GameController.Player.OnGround = true;
                        }
                        else
                        {
                            paused = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Program.GetLogger().Error($"AI Thread exception: {e}");
                        root.Stop(null);
                        root.Start(null);
                    }
                    Thread.Sleep(60);
                }
            })
            { IsBackground = true };

            CreateRoot();
            root.Start(null);

            aiThread.Start();
        }

        private void CreateRoot()
        {
            root = new PrioritySelector(CustomEvents, Mover, Digger);
        }
    }

    

    
}