using MoBot.Structure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MoBot.Protocol.Threading
{
    internal class WritingThread : BaseThread
    {     
        public object QueueLocker { get; } = new object();
        public Queue<Packet> SendingQueue { get; private set; } = new Queue<Packet>();
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
                                Packet pack = SendingQueue.Dequeue();
                                if (pack != null)
                                    NetworkController.MainChannel.SendPacket(pack);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    Thread.Sleep(50);
                }
            }) {IsBackground = true};
            Thread.Start();
        }
    }
}
