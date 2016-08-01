namespace MoBot.Protocol.Threading
{
    public abstract class BaseThread
    {
        protected bool Process = true;
        public void Stop()
        {
            Process = false;
        }
    }
}
