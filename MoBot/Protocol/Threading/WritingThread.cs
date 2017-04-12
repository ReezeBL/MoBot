using System;
using System.Collections.Generic;
using System.Threading;
using MoBot.Core;

namespace MoBot.Protocol.Threading
{
    public class WritingThread : BaseThread
    {     
        public object QueueLocker { get; } = new object();
        public Queue<Packet> SendingQueue { get; } = new Queue<Packet>();
        private Thread Thread { get; }
        public WritingThread()
        {           
             
            Thread = new Thread(() =>
            {
                while (Process)
                {
                    lock (QueueLocker)
                    {
                        try
                        {
                            while (SendingQueue.Count > 0)
                            {
                                var pack = SendingQueue.Dequeue();
                                if (pack != null)
                                    NetworkController.MainChannel.SendPacket(pack);
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"Writing thread: {exception}");
                        }
                    }
                    Thread.Sleep(10);
                }
            }) {IsBackground = true};
            Thread.Start();
        }
    }
}
