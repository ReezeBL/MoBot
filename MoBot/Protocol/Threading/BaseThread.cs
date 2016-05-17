namespace MoBot.Protocol.Threading
{
    internal abstract class BaseThread
    {
        protected bool Process = true;
        public void Stop()
        {
            Process = false;
        }
    }
}
