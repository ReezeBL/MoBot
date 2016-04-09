﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    class PacketPlayerPosLook : Packet
    {
        public double X;
        public double Y;
        public double Z;
        public float pitch;
        public float yaw;
        public bool onGround;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketPlayerPosLook(this);
        }

        public override void ReadPacketData(PacketBuffer buff)
        {
            X = buff.ReadDouble();
            Y = buff.ReadDouble();
            Z = buff.ReadDouble();
            yaw = buff.ReadSingle();
            pitch = buff.ReadSingle();
            onGround = buff.ReadBool();
        }

        public override void WritePacketData(PacketBuffer buff)
        {
            buff.WriteDouble(X);
            buff.WriteDouble(Y);
            buff.WriteDouble(Z);
            buff.WriteSingle(yaw);
            buff.WriteSingle(pitch);
            buff.WriteBool(onGround);
        }
    }
}
