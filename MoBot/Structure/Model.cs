using MoBot.Structure.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MoBot.Protocol;
using MoBot.Protocol.Packets.Handshake;
using Newtonsoft.Json.Linq;
using MoBot.Protocol.Handlers;
using MoBot.Protocol.Threading;

namespace MoBot.Structure
{
    class Model : IObservable<SysAction>
    {
        public IObserver<SysAction> viewer { get; private set; }
        public Channel mainChannel { get; private set; }
        public IHandler handler { get; private set; }
        public String username { get; private set; }
        public JArray modList { get; private set; }
        private WritingThread threadWrite;
        private ReadingThread threadRead;
        public Game.GameController controller
        {
            get; private set;
        }
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            viewer = observer;
            return null;
        }

        public void Connect(String ServerIP, int port, String name)
        {
            #region InitVariables
            dynamic response = Ping(ServerIP, port);
            modList = response.modinfo.modList;
            TcpClient client = new TcpClient(ServerIP, port);
            mainChannel = new Channel(client.GetStream(), Channel.State.Login);
            username = name;
            handler = new ClientHandler(this);
            threadWrite = new WritingThread(this);
            threadRead = new ReadingThread(this);
            controller = new Game.GameController(this);
            #endregion
            #region BeginConnect
            viewer.OnNext(new ActionConnect { Connected = true });
            SendPacket(new PacketHandshake { hostname = ServerIP, port = (ushort)port, nextState = 2, protocolVersion = (int)response.version.protocol});
            SendPacket(new PacketLoginStart { Name = name });
            #endregion
        }
        public void Disconnect()
        {
            viewer.OnNext(new ActionConnect { Connected = false });
            threadWrite.Stop();
            threadRead.Stop();
        }
        public void Message(String message)
        {
            viewer.OnNext(new ActionMessage { message = message });
        }
        public dynamic Ping(String ServerIP, int port, bool message = false)
        {
            TcpClient client = new TcpClient(ServerIP, port);
            Channel channel = new Channel(client.GetStream());
            channel.SendPacket(new PacketHandshake() { hostname = ServerIP, port = (ushort)port, nextState = 1, protocolVersion = 47 });
            channel.SendPacket(new EmptyPacket());
            PacketResponse result = channel.GetPacket() as PacketResponse;
            dynamic response = JObject.Parse(result.JSONResponse);
            client.Close();
            if(message)
                viewer.OnNext(new ActionMessage { message = String.Format("Name: {1}\nProtocol: {0}\nOnline: {2}", response.version.protocol, response.description, response.players.online) });           
            return response;
        }


        public void SendPacket(Packet packet)
        {
            lock (threadWrite.queueLocker)
            {
                threadWrite.SendingQueue.Enqueue(packet);
            }
        }
    }
}
