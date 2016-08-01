using System;
using MoBot.Protocol.Handlers;
using MoBot.Structure.Game;

namespace MoBot.Protocol
{
    public abstract class Packet
    {
        public abstract void HandlePacket(IHandler handler);
        public abstract void ReadPacketData(StreamWrapper buff);
        public abstract void WritePacketData(StreamWrapper buff);

        public virtual bool ProceedNow()
        {
            return false;
        }

        protected static void WriteItem(StreamWrapper buff, Item itemStack)
        {
            throw new NotImplementedException();
        }

        protected static Item ReadItem(StreamWrapper buff)
        {
            throw new NotImplementedException();
        }
    }
}