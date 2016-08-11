using System;
using System.Collections.Concurrent;
using System.Threading;
using AForge.Math;
using MoBot.Structure.Game.AI.Pathfinding;
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
        public CustomEvents CustomEvents { get; } = new CustomEvents();
        public Mover Mover { get;} = new Mover();
        public Digger Digger { get; } = new Digger();

        private int _flyingTicks;
        private bool _paused = true;
        private Vector3 lastPos;

        public AiHandler()
        {
            var aiThread = new Thread(() =>
            {
                while (_threadContinue)
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
                        //Vector3 floor = GameController.Player.Position;
                        //Vector3 downSide = floor + new Vector3(0f, -0.033f, 0f);

                        //if (!GameController.World.IsBlockFree(downSide))
                        //{
                        //    GameController.Player.OnGround = true;
                        //    _flyingTicks = 0;
                        //}
                        //else
                        //{
                        //    GameController.Player.OnGround = true;
                        //    if ((floor - lastPos).Y >= -0.03125)
                        //    {
                        //        _flyingTicks ++;
                        //        Console.WriteLine($"Flying ticks: {_flyingTicks}");
                        //    }
                        //    ActionManager.SetPlayerPos(downSide);
                        //}

                        //lastPos = floor;
                        //ActionManager.UpdatePosition();


                    }
                    else
                    {
                        _paused = true;
                        _flyingTicks = 0;
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