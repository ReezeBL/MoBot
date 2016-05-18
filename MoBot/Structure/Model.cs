using System;
using System.Net.Sockets;
using MoBot.Protocol;
using MoBot.Protocol.Handlers;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Threading;
using MoBot.Structure.Actions;
using MoBot.Structure.Game;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure
{
    class Model : IObservable<SysAction>
    {
        private static Model _instance;
        private Model() { }
        public IObserver<SysAction> Viewer { get; private set; }
        public Channel MainChannel { get; private set; }
        public IHandler Handler { get; private set; }
        public string Username { get; private set; }
        public JArray ModList { get; private set; }
        private WritingThread _threadWrite;
        private ReadingThread _threadRead;        
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            Viewer = observer;
            return null;
        }
        public static Model GetInstance()
        {
            if (_instance == null)
                return _instance = new Model();
            return _instance;
        }
        public void Connect(string serverIp, int port, string name)
        {
            try
            {
                #region InitVariables
                dynamic response = Ping(serverIp, port);
                ModList = response.modinfo.modList;
                TcpClient client = new TcpClient(serverIp, port);
                MainChannel = new Channel(client.GetStream(), Channel.State.Login);
                Username = name;
                GameController.GetInstance().Clear();
                Handler = new ClientHandler();               
                _threadWrite = new WritingThread();
                _threadRead = new ReadingThread();
                #endregion
                #region BeginConnect
                Viewer.OnNext(new ActionConnect { Connected = true });
                SendPacket(new PacketHandshake { hostname = serverIp, port = (ushort)port, nextState = 2, protocolVersion = (int)response.version.protocol });
                SendPacket(new PacketLoginStart { Name = name });
                #endregion
            }
            catch (Exception)
            {
                Viewer.OnNext(new ActionMessage { message = "Unable to connect to server!" });
            }
        }
        public void Disconnect()
        {
            Viewer.OnNext(new ActionConnect { Connected = false });
            _threadWrite.Stop();
            _threadRead.Stop();           
        }
        public void Message(string message)
        {
            Viewer.OnNext(new ActionMessage { message = message });
        }
        public dynamic Ping(string serverIp, int port, bool message = false)
        {
            var client = new TcpClient(serverIp, port);
            var channel = new Channel(client.GetStream());
            channel.SendPacket(new PacketHandshake { hostname = serverIp, port = (ushort)port, nextState = 1, protocolVersion = 47 });
            channel.SendPacket(new EmptyPacket());
            var result = channel.GetPacket() as PacketResponse;
            if (result == null) return null;
            dynamic response = JObject.Parse(result.JSONResponse);
            client.Close();
            if(message)
                Viewer.OnNext(new ActionMessage { message = String.Format("Name: {1}\nProtocol: {0}\nOnline: {2}", response.version.protocol, response.description, response.players.online) });           
            return response;
        }
        public void SendPacket(Packet packet)
        {
            lock (_threadWrite.QueueLocker)
            {
                _threadWrite.SendingQueue.Enqueue(packet);
            }
        }
    }
}
