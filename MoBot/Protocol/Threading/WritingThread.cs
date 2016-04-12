﻿using MoBot.Protocol.Packets.Play;
using MoBot.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoBot.Protocol.Threading
{
    class WritingThread : BaseThread
    {
        private Model model;
        public Object queueLocker { get; private set; } = new object();
        public Queue<Packet> SendingQueue { get; private set; } = new Queue<Packet>();
        public Thread thread { get; private set; }
        public WritingThread(Model model)
        {
            this.model = model;
            thread = new Thread(() =>
            {
                while (Process)
                {
                    lock (queueLocker)
                    {
                        while (SendingQueue.Count > 0)
                        {
                            Packet pack = SendingQueue.Dequeue();
                            if (pack != null)
                                model.mainChannel.SendPacket(pack);
                        }
                        //if (model.controller.InGameLoaded)
                        //{
                        //    model.mainChannel.SendPacket(new PacketPlayerPosLook
                        //    {
                        //        X = model.controller.player.x,
                        //        Y = model.controller.player.y,
                        //        Z = model.controller.player.z,
                        //        yaw = model.controller.player.yaw,
                        //        pitch = model.controller.player.pitch,
                        //        onGround = true
                        //    });
                        //}
                    }
                    Thread.Sleep(50);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
