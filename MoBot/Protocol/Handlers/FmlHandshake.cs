using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure;
using Newtonsoft.Json.Linq;
using NLog;

namespace MoBot.Protocol.Handlers
{
    class FmlHandshake : CustomHandler
    {
        private readonly Logger _log = Program.GetLogger();

        public override void OnPacketData(byte[] payloadData)
        {
            var payload = new StreamWrapper(payloadData);
            var discriminator = payload.ReadByte();
            switch (discriminator)
            {
                case 0:
                    {
                        var answer = new StreamWrapper();
                        var version = payload.ReadByte();
                        answer.WriteByte(1);
                        answer.WriteByte(version);
                        NetworkController.SendPacket(new PacketCustomPayload
                        {
                            Channel = "FML|HS",
                            MyPayload = answer.GetBlob()
                        });
                        answer = new StreamWrapper();
                        answer.WriteByte(2);
                        answer.WriteVarInt(NetworkController.ModList.Count);
                        foreach (var jToken in NetworkController.ModList)
                        {
                            var obj = (JObject)jToken;
                            answer.WriteString((string)obj["modid"]);
                            answer.WriteString((string)obj["version"]);
                        }
                        NetworkController.SendPacket(new PacketCustomPayload
                        {
                            Channel = "FML|HS",
                            MyPayload = answer.GetBlob()
                        });
                    }
                    break;
                case 2:
                    {
                        var answer = new StreamWrapper();
                        answer.WriteByte(255);
                        answer.WriteByte(2);
                        NetworkController.SendPacket(new PacketCustomPayload
                        {
                            Channel = "FML|HS",
                            MyPayload = answer.GetBlob()
                        });
                    }
                    break;
                case 255:
                    {
                        var stage = payload.ReadByte();
                        var answer = new StreamWrapper();
                        answer.WriteByte(255);
                        switch (stage)
                        {
                            case 3:
                                answer.WriteByte(5);
                                break;
                            case 2:
                                answer.WriteByte(4);
                                break;
                            default:
                                _log.Info($"Unhandled Ack Stage : {stage}");
                                break;
                        }
                        NetworkController.SendPacket(new PacketCustomPayload
                        {
                            Channel = "FML|HS",
                            MyPayload = answer.GetBlob()
                        });
                    }
                    break;
            }
        }
    }
}
