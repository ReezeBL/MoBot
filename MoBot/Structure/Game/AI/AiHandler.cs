using System;
using System.Collections.Concurrent;
using System.Threading;
using FluentBehaviourTree;

namespace MoBot.Structure.Game.AI
{
    public class AiHandler
    {
        private readonly ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>();
        private bool _threadContinue = true;
        private IBehaviourTreeNode _tree;

        public readonly Protector Protector = new Protector();

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
                        _tree.Tick(new TimeData(50));
                    }
                    else
                    {
                        _threadContinue = false;
                    }
                    
                    Thread.Sleep(50);
                }               
            }) {IsBackground = true};

            BuildTree();
            aiThread.Start();
        }

        public void EnqueueTask(Action func)
        {
            _tasks.Enqueue(func);
        }

        private void BuildTree()
        {
            var builder = new BehaviourTreeBuilder();
            _tree = builder.Selector("mainLoop").Splice(Protector.GetTreeNode()).End().Build();
        }

        
    }

    public class WaitTask
    {
        private readonly float _waitingTime;

        public WaitTask(float waitingTime)
        {
            _waitingTime = waitingTime;
        }
    }

    public abstract class AiHandlerComponent
    {
        protected delegate BehaviourTreeStatus Task(TimeData data);

        public abstract IBehaviourTreeNode GetTreeNode();
    }

    public class Protector : AiHandlerComponent
    {
        public Entity Target;

        private bool HasTarget(TimeData timeData)
        {
            return Target == null;
        }

        private BehaviourTreeStatus AttackTarget(TimeData timeData)
        {
            ActionManager.AttackEntity(Target.Id);
            return BehaviourTreeStatus.Success;
        }

        public override IBehaviourTreeNode GetTreeNode()
        {
            var builder = new BehaviourTreeBuilder();
            return builder.Sequence("protection").Condition("CheckForTarget", HasTarget).Do("AttackTarget", AttackTarget).End().Build();
        }
    }
}