using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Protocol.Threading
{
    class BaseThread
    {
        protected bool Process = true;
        public void Stop()
        {
            Process = false;
        }
    }
}
