using MoBot.Protocol.Handlers;

namespace MoBot.Protocol.Packets.Play
{
    public class PacketPlayerPosLook : Packet
    {
        public double X;
        public double Y;
        public double Z;
        public float Pitch;
        public float Yaw;
        public bool OnGround;
        public override void HandlePacket(IHandler handler)
        {
            handler.HandlePacketPlayerPosLook(this);
        }

        public override void ReadPacketData(StreamWrapper buff)
        {
            X = buff.ReadDouble();
            Y = buff.ReadDouble();
            Z = buff.ReadDouble();
            Yaw = buff.ReadSingle();
            Pitch = buff.ReadSingle();
            OnGround = buff.ReadBool();
        }

        public override void WritePacketData(StreamWrapper buff)
        {
            buff.WriteDouble(X);
            buff.WriteDouble(Y);
            buff.WriteDouble(Y + 1.62);
            buff.WriteDouble(Z);
            buff.WriteSingle(Yaw);
            buff.WriteSingle(Pitch);
            buff.WriteBool(OnGround);
        }
    }
}
