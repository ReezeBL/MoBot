using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MoBot.Protocol;
using MoBot.Protocol.Handlers;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Threading;
using MoBot.Structure.Actions;
using MoBot.Structure.Game;
using Newtonsoft.Json.Linq;

namespace MoBot.Structure
{
    public class NetworkController : IObservable<SysAction>
    {
        private static NetworkController _instance;

        private NetworkController()
        {
        }

        private IObserver<SysAction> _viewer;
        public static string Username { get; private set; }
        public static JArray ModList { get; private set; }

        private WritingThread _threadWrite;
        private ReadingThread _threadRead;
        public static Channel MainChannel { get; private set; }
        public static IHandler Handler { get; private set; }      
        public static bool Connected { get; private set; }
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            _viewer = observer;
            return null;
        }

        public static NetworkController GetInstance()
        {
            if (_instance == null)
                return _instance = new NetworkController();
            return _instance;
        }

        public static async void ConnectAsync(string serverIp, int port, string name, int delay = 0)
        {
            await Task.Run(() => { Thread.Sleep(delay); Connect(serverIp, port, name); });
        }

        public static void Connect(string serverIp, int port, string name)
        {
            try
            {
                #region InitVariables

                var instance = GetInstance();
                dynamic response = Ping(serverIp, port);

                if (response == null)
                {
                    NotifyViewer("Server is unavailible!");
                    Disconnect();
                    return;
                }

                if (response.players.online >= response.players.max)
                {
                    NotifyViewer("Server is full");
                    Disconnect();
                    return;
                }
                    

                ModList = response.modinfo.modList;
                var client = new TcpClient(serverIp, port);
                MainChannel = new Channel(client.GetStream(), Channel.State.Login);
                Username = name;
                GameController.Clear();
                Handler = new ClientHandler();
                instance._threadWrite = new WritingThread();
                instance._threadRead = new ReadingThread();

                #endregion

                #region BeginConnect

                instance._viewer.OnNext(new ActionConnect {Connected = true});
                SendPacket(new PacketHandshake
                {
                    Hostname = serverIp,
                    Port = (ushort) port,
                    NextState = 2,
                    ProtocolVersion = (int) response.version.protocol
                });
                SendPacket(new PacketLoginStart {Name = name});
                Connected = true;

                #endregion
            }
            catch (Exception exception)
            {
                Disconnect();
                Console.WriteLine(exception);
                NotifyViewer($"Unable to connect to server!");
            }
        }

        public static void Disconnect()
        {           
            var networkController = GetInstance();
            networkController._viewer.OnNext(new ActionConnect { Connected = false});
            networkController._threadWrite?.Stop();
            networkController._threadRead?.Stop();
            Connected = false;
        }

        public static void NotifyViewer(string message)
        {
            GetInstance()._viewer.OnNext(new ActionMessage {Message = message});
        }

        public static void NotifyChatMessage(string message)
        {
            GetInstance()._viewer.OnNext(new ActionChatMessage { JsonMessage = message });
        }

        public static dynamic Ping(string serverIp, int port, bool message = false)
        {
            var client = new TcpClient();
            if (!client.ConnectAsync(serverIp, port).Wait(2000))
            {
                return null;
            }
            var channel = new Channel(client.GetStream());
            channel.SendPacket(new PacketHandshake
            {
                Hostname = serverIp,
                Port = (ushort)port,
                NextState = 1,
                ProtocolVersion = 46
            });
            channel.SendPacket(new EmptyPacket());
            var result = channel.GetPacket() as PacketResponse;
            if (result == null) return null;
            dynamic response = JObject.Parse(result.JsonResponse);
            client.Close();
            if (message)
                NotifyViewer(string.Format("Name: {1}\nProtocol: {0}\nOnline: {2}", response.version.protocol,
                    response.description, response.players.online));
            return response;
        }

        public static void SendPacket(Packet packet)
        {
            var instance = GetInstance();
            lock (instance._threadWrite.QueueLocker)
            {
                instance._threadWrite.SendingQueue.Enqueue(packet);
            }
        }
    }
}
