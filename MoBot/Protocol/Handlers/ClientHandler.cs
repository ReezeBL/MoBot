using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using MoBot.Protocol.Handshake;
using MoBot.Protocol.Packets;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure;
using MoBot.Structure.Actions;
using MoBot.Structure.Game;
using Newtonsoft.Json.Linq;
using NLog;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MoBot.Protocol.Handlers
{
    internal class ClientHandler : IHandler
    {
        private readonly Model _model;
        private readonly Logger _log = Program.GetLogger();
        private readonly GameController _gameController;

        private string PostUrl(PacketEncriptionRequest packetEncriptionRequest, byte[] secretKey, string id)
        {
            var request =
                WebRequest.Create(
                    $"http://ex-server.ru/joinserver.php?user={_model.Username}&sessionId={id}&serverId={GetServerIdHash(packetEncriptionRequest.ServerID, secretKey, packetEncriptionRequest.Key)}");
            var responseStream = request.GetResponse().GetResponseStream();
            if (responseStream == null) return "";
            var responseString = new StreamReader(responseStream).ReadToEnd();
            return responseString;
        }

        private string GetUserSession()
        {
            var document = new XmlDocument();
            document.Load("Settings/UserIDS.xml");
            var root = document.DocumentElement;
            var id = "";
            Debug.Assert(root != null, "root != null");
            foreach (XmlNode child in root)
            {
                if (child.Attributes?.GetNamedItem("name").Value == _model.Username)
                    id = child.InnerText;
            }

            return id;
        }

        #region ServerHashCalculations

        private static string GetServerIdHash(string serverId, byte[] secretKey, byte[] publicKey)
        {
            var idBytes = Encoding.ASCII.GetBytes(serverId);
            var data = new byte[idBytes.Length + secretKey.Length + publicKey.Length];

            idBytes.CopyTo(data, 0);
            secretKey.CopyTo(data, idBytes.Length);
            publicKey.CopyTo(data, idBytes.Length + secretKey.Length);

            return JavaHexDigest(data);
        }

        private static string JavaHexDigest(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            var negative = (hash[0] & 0x80) == 0x80;
            if (negative) // check for negative hashes
                hash = TwosCompliment(hash);
            // Create the string and trim away the zeroes
            var digest = GetHexString(hash).TrimStart('0');
            if (negative)
                digest = "-" + digest;
            return digest;
        }

        private static string GetHexString(IEnumerable<byte> p)
        {
            return p.Aggregate(string.Empty, (current, t) => current + t.ToString("x2"));
        }

        private static byte[] TwosCompliment(byte[] p) // little endian
        {
            int i;
            var carry = true;
            for (i = p.Length - 1; i >= 0; i--)
            {
                p[i] = (byte) ~p[i];
                if (!carry) continue;
                carry = p[i] == 0xFF;
                p[i]++;
            }
            return p;
        }

        #endregion

        public ClientHandler()
        {
            _model = Model.GetInstance();
            _gameController = GameController.GetInstance();
        }

        public void HandlePacketDisconnect(PacketDisconnect packetDisconnect)
        {
            _model.Message($"You have been disconnected from the server! Reason: {packetDisconnect.reason}");
            _model.Disconnect();
        }

        public void HandlePacketEncriptionRequest(PacketEncriptionRequest packetEncriptionRequest)
        {
            var kp = PublicKeyFactory.CreateKey(packetEncriptionRequest.Key);
            var key = (RsaKeyParameters) kp;
            var cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            cipher.Init(true, key);
            var keygen = new CipherKeyGenerator();
            keygen.Init(new KeyGenerationParameters(new SecureRandom(), 128));
            var secretKey = keygen.GenerateKey();
            var cryptedKey = cipher.DoFinal(secretKey);
            var cryptedToken = cipher.DoFinal(packetEncriptionRequest.Token);

            #region PostURL

            try
            {
                var id = GetUserSession();
                var responseString = PostUrl(packetEncriptionRequest, secretKey, id);
                if (responseString != "OK")
                    _log.Error("Auth failed!\nAuth username: {1}\nAuth ID:{2}\nAuth response : {0}", responseString,
                        _model.Username, id);
            }
            catch (SocketException)
            {
                _log.Error("Unable to connect to login server!");
            }
            catch (FileNotFoundException)
            {
                _log.Error("No UserIDS.xml file in directory!");
            }
            catch (XmlException)
            {
                _log.Error("Unable to proceed XML file");
            }

            #endregion

            _model.MainChannel.SendPacket(new PacketEncriptionResponse
            {
                SharedSecret = cryptedKey,
                SharedSecretLength = cryptedKey.Length,
                Token = cryptedToken,
                TokenLength = cryptedToken.Length
            });
            _model.MainChannel.EncriptChannel(secretKey);
        }

        public void HandlePacketLoginSucess(PacketLoginSuccess packetLoginSuccess)
        {
            _model.MainChannel.ChangeState(Channel.State.Play);
        }

        public void HandlePacketKeepAlive(PacketKeepAlive packetKeepAlive)
        {
            _model.SendPacket(packetKeepAlive);
        }

        public void HandlePacketCustomPayload(PacketCustomPayload packetCustomPayload)
        {
            if (packetCustomPayload.channel != "FML|HS") return;
            var payload = new StreamWrapper(packetCustomPayload.Payload);
            var discriminator = payload.ReadByte();
            switch (discriminator)
            {
                case 0:
                {
                    var answer = new StreamWrapper();
                    var version = payload.ReadByte();
                    answer.WriteByte(1);
                    answer.WriteByte(version);
                    _model.SendPacket(new PacketCustomPayload {channel = "FML|HS", MyPayload = answer.GetBlob()});
                    answer = new StreamWrapper();
                    answer.WriteByte(2);
                    answer.WriteVarInt(_model.ModList.Count);
                    foreach (var jToken in _model.ModList)
                    {
                        var obj = (JObject) jToken;
                        answer.WriteString((string) obj["modid"]);
                        answer.WriteString((string) obj["version"]);
                    }
                    _model.SendPacket(new PacketCustomPayload {channel = "FML|HS", MyPayload = answer.GetBlob()});
                }
                    break;
                case 2:
                {
                    var answer = new StreamWrapper();
                    answer.WriteByte(255);
                    answer.WriteByte(2);
                    _model.SendPacket(new PacketCustomPayload {channel = "FML|HS", MyPayload = answer.GetBlob()});
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
                    _model.SendPacket(new PacketCustomPayload {channel = "FML|HS", MyPayload = answer.GetBlob()});
                }
                    break;
                default:
                    _log.Info($"Unhandled FmlDescriptor : {discriminator}");
                    break;
            }
        }

        public void HandlePacketJoinGame(PacketJoinGame packetJoinGame)
        {
            _gameController.CreateUser(packetJoinGame.EntityID, _model.Username);
        }

        public void HandlePacketPlayerAbliities(PacketPlayerAbilities packetPlayerAbilities)
        {
        }

        public void HandlePacketHeldItemChange(PacketHeldItemChange packetHeldItemChange)
        {
            _gameController.Player.HeldItem = packetHeldItemChange.Slot;
        }

        public void HandlePacketPlayerPosLook(PacketPlayerPosLook packetPlayerPosLook)
        {
            _gameController.Player.X = packetPlayerPosLook.X;
            _gameController.Player.Y = packetPlayerPosLook.Y - 1.62;
            _gameController.Player.Z = packetPlayerPosLook.Z;
            _gameController.Player.Yaw = packetPlayerPosLook.yaw;
            _gameController.Player.Pitch = packetPlayerPosLook.pitch;
            _gameController.Player.OnGround = packetPlayerPosLook.onGround;
            _model.SendPacket(packetPlayerPosLook);
        }

        public void HandlePacketWindowItems(PacketWindowItems packetWindowItems)
        {
            if (packetWindowItems.WindowID == 0)
            {
                packetWindowItems.Items.CopyTo(_gameController.Player.Inventory, 0);
            }
        }

        public void HandlePacketSetSlot(PacketSetSlot packetSetSlot)
        {
            if (packetSetSlot.WindowID == 0)
            {
                _gameController.Player.Inventory[packetSetSlot.Slot] = packetSetSlot.item;
            }
        }

        public void HandlePacketSpawnMoob(PacketSpawnMob packetSpawnMob)
        {
            var mob = _gameController.CreateMob(packetSpawnMob.EntityID, packetSpawnMob.Type);
            mob.X = packetSpawnMob.X;
            mob.Y = packetSpawnMob.Y;
            mob.Z = packetSpawnMob.Z;
        }

        public void HandlePacketChat(PacketChat packetChat)
        {
            _model.Viewer.OnNext(new ActionChatMessage {JSONMessage = packetChat.message});
        }

        public void HandlePacketMapChunk(PacketMapChunk packetMapChunk)
        {
            var mas = new byte[packetMapChunk.DataLength - 2];
            Array.Copy(packetMapChunk.ChunkData, 2, mas, 0, packetMapChunk.DataLength - 2);
            var dc = new Decompressor(mas);
            var dced = dc.Decompress();

            for (var i = 0; i < packetMapChunk.ChunkNumber; i++)
            {
                dced = packetMapChunk.chunks[i].GetData(dced);
                _gameController.World.AddChunk(packetMapChunk.chunks[i]);
            }
        }

        public void HandlePacketChunkData(PacketChunkData packetChunkData)
        {
            if (packetChunkData.RemoveChunk)
            {
                _gameController.World.RemoveChunk(packetChunkData.x, packetChunkData.z);
            }
            else
            {
                var mas = new byte[packetChunkData.Length - 2];
                Array.Copy(packetChunkData.ChunkData, 2, mas, 0, packetChunkData.Length - 2);
                var dc = new Decompressor(mas);
                var dced = dc.Decompress();

                packetChunkData.chunk.GetData(dced);
                _gameController.World.AddChunk(packetChunkData.chunk);
            }
        }

        public void HandlePacketEntity(PacketEntity packetEntity)
        {
            try
            {
                var entity = _gameController.GetEntity(packetEntity.EntityID) as LivingEntity;
                if (entity == null) return;
                entity.X += packetEntity.x;
                entity.Y += packetEntity.y;
                entity.Z += packetEntity.z;
            }
            catch (KeyNotFoundException)
            {
                //log.Warn($"Trying to update positin of entity, that is not presented! EntityID: {packetEntity.EntityID}");
            }
        }

        public void HandlePacketEntityTeleport(PacketEntityTeleport packetEntityTeleport)
        {
            try
            {
                var entity = _gameController.GetEntity(packetEntityTeleport.EntityID) as LivingEntity;
                if (entity == null) return;
                entity.X = packetEntityTeleport.x;
                entity.Y = packetEntityTeleport.y;
                entity.Z = packetEntityTeleport.z;
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public void HandlePacketDestroyEntities(PacketDestroyEntities packetDestroyEntities)
        {
            foreach (var id in packetDestroyEntities.IDList)
            {
                _gameController.RemoveEntity(id);
            }
        }

        public void HandlePacketBlockChange(PacketBlockChange packetBlockChange)
        {
            _gameController.World.UpdateBlock(packetBlockChange.X, packetBlockChange.Y, packetBlockChange.Z,
                packetBlockChange.BlockID);
        }

        public void HandlePacketUpdateHealth(PacketUpdateHelath packetUpdateHelath)
        {
            _gameController.Player.Health = packetUpdateHelath.Health;
            _gameController.Player.Food = packetUpdateHelath.Food;
            _gameController.Player.Saturation = packetUpdateHelath.Saturation;
        }

        public void HandlePacketMultiBlockChange(PacketMultiBlockChange packetMultiBlockChange)
        {
            var chunkX = packetMultiBlockChange.chunkXPosiiton*16;
            var chunkZ = packetMultiBlockChange.chunkZPosition*16;

            if (packetMultiBlockChange.metadata != null)
            {
                var buff = new StreamWrapper(packetMultiBlockChange.metadata);
                for (var i = 0; i < packetMultiBlockChange.size; i++)
                {
                    var short1 = buff.ReadShort();
                    var short2 = buff.ReadShort();
                    var id = short2 >> 4 & 4095;
                    var x = short1 >> 12 & 15;
                    var z = short1 >> 8 & 15;
                    var y = short1 & 255;
                    _gameController.World.UpdateBlock(chunkX + x, y, chunkZ + z, id);
                }
            }
        }

        public void HandlePacketEntityStatus(PacketEntityStatus packetEntityStatus)
        {
            if (packetEntityStatus.EntityStatus == 2)
            {
                _gameController.RemoveEntity(packetEntityStatus.EntityID);
            }
        }

        public void HandlePacketSpawnObject(PacketSpawnObject packetSpawnObject)
        {
            var entity = _gameController.CreateLivingEntity(packetSpawnObject.EntityID, packetSpawnObject.Type);
            entity.X = packetSpawnObject.X;
            entity.Y = packetSpawnObject.Y;
            entity.Z = packetSpawnObject.Z;
        }

        public void HandlePacketSpawnPlayer(PacketSpawnPlayer packetSpawnPlayer)
        {
            var player = _gameController.CreatePlayer(packetSpawnPlayer.EntityID, packetSpawnPlayer.name);
            player.X = packetSpawnPlayer.x;
            player.Y = packetSpawnPlayer.y;
            player.Z = packetSpawnPlayer.z;
        }

        public void HandlePacketConfirmTransaction(PacketConfirmTransaction packetConfirmTransaction)
        {
            if (!packetConfirmTransaction.Accepted)
            {
                packetConfirmTransaction.Accepted = true;
                _model.SendPacket(packetConfirmTransaction);
            }
        }
    }
}
