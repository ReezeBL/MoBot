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

namespace MoBot.Structure
{
    class Model : IObservable<SysAction>
    {
        Viewer viewer;
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            viewer = observer as Viewer;
            return null;
        }

        internal void Connect()
        {
            viewer.OnNext(new ActionConnect { Connected = true });
        }

        internal void Ping(String ServerIP, int port)
        {
            TcpClient client = new TcpClient(ServerIP, port);
            Channel channel = new Channel(client.GetStream());
            channel.SendPacket(new PacketHandshake() { hostname = ServerIP, port = (ushort)port, nextState = 1, protocolVersion = 47 });
            channel.SendPacket(new EmptyPacket());
            PacketResponse result = channel.GetPacket() as PacketResponse;
            dynamic response = JObject.Parse(result.JSONResponse);
            viewer.OnNext(new ActionMessage { message = String.Format("Name: {1}\nProtocol: {0}\nOnline: {2}", response.version.protocol, response.description, response.players.online) });           
        }
    }
}
