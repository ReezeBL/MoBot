using System;
using System.Net.Sockets;

namespace MoBot.Protocol
{
    public class Channel
    {
        private NetworkStream networkStream;
        private State login;

        public Channel(NetworkStream networkStream, State login)
        {
            this.networkStream = networkStream;
            this.login = login;
        }

        public Channel(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }

        public enum State
        {
            Login,
            Play,
            Ping
        }
        public void ChangeState(object play)
        {
            throw new NotImplementedException();
        }

        public void EncriptChannel(byte[] secretKey)
        {
            throw new NotImplementedException();
        }

        public void SendPacket(Packet packet)
        {
            throw new NotImplementedException();
        }

        public Packet GetPacket()
        {
            throw new NotImplementedException();
        }
    }
}