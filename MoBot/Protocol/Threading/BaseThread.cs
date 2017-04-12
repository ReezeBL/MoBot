using NLog;

namespace MoBot.Protocol.Threading
{
    public abstract class BaseThread
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected bool Process = true;
        public void Stop()
        {
            Process = false;
        }
    }
}
