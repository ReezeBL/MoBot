using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MoBot.Core.Actions;
using MoBot.Core.Game;
using MoBot.Protocol;
using MoBot.Protocol.Handlers;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace MoBot.Core
{
    public class NetworkController : IObservable<SysAction>
    {
        private static NetworkController instance;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private NetworkController()
        {
        }

        private IObserver<SysAction> viewer;
        public static string Username { get; private set; }
        public static ModInfo[] ModList { get; } = GetModList();

        private WritingThread threadWrite;
        private ReadingThread threadRead;
        public static Channel MainChannel { get; private set; }
        public static IHandler Handler { get; private set; }      
        public static bool Connected { get; private set; }
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            viewer = observer;
            return null;
        }

        public static NetworkController Instance => instance ?? (instance = new NetworkController());

        public static async void ConnectAsync(string serverIp, int port, string name, int delay = 0)
        {
            await Task.Run(() => { Thread.Sleep(delay); Connect(serverIp, port, name); });
        }

        public static void Connect(string serverIp, int port, string name)
        {
            try
            {
                #region InitVariables
                var response = Ping(serverIp, port);

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

                var client = new TcpClient(serverIp, port);
                MainChannel = new Channel(client.GetStream(), Channel.State.Login);
                Username = name;
                GameController.Clear();
                Handler = new ClientHandler();
                Instance.threadWrite = new WritingThread();
                Instance.threadRead = new ReadingThread();

                #endregion

                #region BeginConnect

                Instance.viewer.OnNext(new ActionConnect {Connected = true});
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
                NotifyViewer("Unable to connect to server!");
            }
        }

        public static void Disconnect()
        {           
            Instance.viewer.OnNext(new ActionConnect { Connected = false});
            Instance.threadWrite?.Stop();
            Instance.threadRead?.Stop();
            Connected = false;
        }

        public static void NotifyViewer(string message)
        {
            Instance.viewer.OnNext(new ActionMessage {Message = message});
        }

        public static void NotifyChatMessage(string message)
        {
            Instance.viewer.OnNext(new ActionChatMessage { JsonMessage = message });
        }

        public static dynamic Ping(string serverIp, int port, bool message = false)
        {
            var client = new TcpClient();
            if (!client.ConnectAsync(serverIp, port).Wait(2000))
            {
                return null;
            }
            try
            {
                var channel = new Channel(client.GetStream());
                channel.SendPacket(new PacketHandshake
                {
                    Hostname = serverIp,
                    Port = (ushort) port,
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
            catch (Exception)
            {
                return null;
            }
        }

        public static void SendPacket(Packet packet)
        {
            lock (Instance.threadWrite.QueueLocker)
            {
                Instance.threadWrite.SendingQueue.Enqueue(packet);
            }
        }

        public class ModInfo
        {
            public string modid;
            public string version;
        }

        private static ModInfo[] GetModList()
        {
            ModInfo[] result;
            using (var file = File.OpenText(Settings.ModsPath))
            using (var reader = new JsonTextReader(file))
            {
                var deserializer = JsonSerializer.Create();
                result = deserializer.Deserialize<ModInfo[]>(reader);
            }
            return result;
        }
    }
}
