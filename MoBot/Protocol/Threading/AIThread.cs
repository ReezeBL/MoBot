using MoBot.Structure.Game.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Protocol.Threading
{
    class AIThread : BaseThread
    {
        public AIThread(AIModule module)
        {
            Thread thread = new Thread(() =>
            {
                while (Process)
                {
                    module.tick();
                    Thread.Sleep(50);
                }
            })
            {IsBackground = true };
            thread.Start();
        }
    }
}
