using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using MoBot.Protocol.Packets;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Packets.Play;
using MoBot.Structure;
using MoBot.Structure.Game;
using Newtonsoft.Json.Linq;
using NLog;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MoBot.Protocol.Handlers
{
    public class ClientHandler : IHandler
    {
        private readonly Logger _log = Program.GetLogger();


        public void HandlePacketDisconnect(PacketDisconnect packetDisconnect)
        {
            NetworkController.NotifyViewer(
                $"You have been disconnected from the server! Reason: {packetDisconnect.Reason}");
            NetworkController.Disconnect();
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
                        NetworkController.Username, id);
            }
            catch (WebException)
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

            NetworkController.MainChannel.SendPacket(new PacketEncriptionResponse
            {
                SharedSecret = cryptedKey,
                SharedSecretLength = cryptedKey.Length,
                Token = cryptedToken,
                TokenLength = cryptedToken.Length
            });
            NetworkController.MainChannel.EncriptChannel(secretKey);
        }

        public void HandlePacketLoginSucess(PacketLoginSuccess packetLoginSuccess)
        {
            NetworkController.MainChannel.ChangeState(Channel.State.Play);
        }

        public void HandlePacketKeepAlive(PacketKeepAlive packetKeepAlive)
        {
            NetworkController.SendPacket(packetKeepAlive);
        }

        public void HandlePacketCustomPayload(PacketCustomPayload packetCustomPayload)
        {
            if (packetCustomPayload.Channel != "FML|HS") return;
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
                        var obj = (JObject) jToken;
                        answer.WriteString((string) obj["modid"]);
                        answer.WriteString((string) obj["version"]);
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

        public void HandlePacketJoinGame(PacketJoinGame packetJoinGame)
        {
            GameController.CreateUser(packetJoinGame.EntityId, NetworkController.Username);
        }

        public void HandlePacketPlayerAbliities(PacketPlayerAbilities packetPlayerAbilities)
        {
        }

        public void HandlePacketHeldItemChange(PacketHeldItemChange packetHeldItemChange)
        {
            GameController.Player.HeldItemBar = packetHeldItemChange.Slot;
        }

        public void HandlePacketPlayerPosLook(PacketPlayerPosLook packetPlayerPosLook)
        {
            //Console.WriteLine($"Assigning player position from {{{(int) GameController.Player.X}|{(int) GameController.Player.Y}|{(int) GameController.Player.Z}}} to {{{(int) packetPlayerPosLook.X}|{(int) (packetPlayerPosLook.Y - 1.62)}|{(int) packetPlayerPosLook.Z}}}");
            //Console.WriteLine($"Assigning player position from {{{GameController.Player.X}|{GameController.Player.Y}|{GameController.Player.Z}}} to {{{(float)packetPlayerPosLook.X}|{(float)(packetPlayerPosLook.Y - 1.62)}|{(float)packetPlayerPosLook.Z}}}");
            GameController.Player.SetPosition(packetPlayerPosLook.X, packetPlayerPosLook.Y -= 1.62,
                packetPlayerPosLook.Z);
            GameController.Player.Yaw = packetPlayerPosLook.Yaw;
            GameController.Player.Pitch = packetPlayerPosLook.Pitch;
            GameController.Player.OnGround = packetPlayerPosLook.OnGround;

            NetworkController.SendPacket(packetPlayerPosLook);
        }

        public void HandlePacketWindowItems(PacketWindowItems packetWindowItems)
        {
            var container = GameController.Player.GetContainer(packetWindowItems.WindowId);
            for (var i = 0; i < packetWindowItems.ItemCount; i++)
                container[i] = packetWindowItems.ItemsStack[i];
        }

        public void HandlePacketRespawn(PacketRespawn packetRespawn)
        {
        }

        public void HandlePacketOpenWindow(PacketOpenWindow packetOpenWindow)
        {
            var player = GameController.Player;
            player.CreateContainer(packetOpenWindow.WindowId, packetOpenWindow.SlotNumber);
        }

        public void HandlePacketCloseWindow(PacketCloseWindow packetCloseWindow)
        {
            GameController.Player.CloseContainer(packetCloseWindow.WindowId);
        }

        public void HandlePacketSetSlot(PacketSetSlot packetSetSlot)
        {
            try
            {
                GameController.Player.GetContainer(packetSetSlot.WindowId)[packetSetSlot.Slot] = packetSetSlot.ItemStack;
            }
            catch (Exception e)
            {
                Program.GetLogger()
                    .Warn($"Failed to set slot {packetSetSlot.Slot} in window {packetSetSlot.WindowId}, exception {e}");
            }
        }

        public void HandlePacketSpawnMoob(PacketSpawnMob packetSpawnMob)
        {
            var mob = GameController.CreateMob(packetSpawnMob.EntityId, packetSpawnMob.Type);
            mob?.SetPosition(packetSpawnMob.X, packetSpawnMob.Y, packetSpawnMob.Z);
        }

        public void HandlePacketChat(PacketChat packetChat)
        {
            NetworkController.NotifyChatMessage(packetChat.Message);
        }

        public void HandlePacketMapChunk(PacketMapChunk packetMapChunk)
        {
            var mas = new byte[packetMapChunk.DataLength - 2];
            Array.Copy(packetMapChunk.ChunkData, 2, mas, 0, packetMapChunk.DataLength - 2);
            var dc = new Decompressor(mas);
            var dced = dc.Decompress();

            for (var i = 0; i < packetMapChunk.ChunkNumber; i++)
            {
                dced = packetMapChunk.Chunks[i].GetData(dced);
                GameController.World.AddChunk(packetMapChunk.Chunks[i]);
            }
            GameController.World.Invalidate();
        }

        public void HandlePacketChunkData(PacketChunkData packetChunkData)
        {
            if (packetChunkData.RemoveChunk)
            {
                GameController.World.RemoveChunk(packetChunkData.X, packetChunkData.Z);
            }
            else
            {
                var mas = new byte[packetChunkData.Length - 2];
                Array.Copy(packetChunkData.ChunkData, 2, mas, 0, packetChunkData.Length - 2);
                var dc = new Decompressor(mas);
                var dced = dc.Decompress();

                packetChunkData.Chunk.GetData(dced);
                GameController.World.AddChunk(packetChunkData.Chunk);
            }
            GameController.World.Invalidate();
        }

        public void HandlePacketEntity(PacketEntity packetEntity)
        {
            var entity = GameController.GetEntity(packetEntity.EntityId) as LivingEntity;
            entity?.Move(packetEntity.X, packetEntity.Y, packetEntity.Z);
        }

        public void HandlePacketEntityTeleport(PacketEntityTeleport packetEntityTeleport)
        {
            var entity = GameController.GetEntity(packetEntityTeleport.EntityId) as LivingEntity;
            entity?.SetPosition(packetEntityTeleport.X, packetEntityTeleport.Y, packetEntityTeleport.Z);
        }

        public void HandlePacketDestroyEntities(PacketDestroyEntities packetDestroyEntities)
        {
            foreach (var id in packetDestroyEntities.IdList)
            {
                GameController.RemoveEntity(id);
            }
        }

        public void HandlePacketBlockChange(PacketBlockChange packetBlockChange)
        {
            GameController.World.SetBlock(packetBlockChange.X, packetBlockChange.Y, packetBlockChange.Z,
                packetBlockChange.BlockId);
            GameController.World.Invalidate();
        }

        public void HandlePacketUpdateHealth(PacketUpdateHealth packetUpdateHelath)
        {
            GameController.Player.Health = packetUpdateHelath.Health;
            GameController.Player.Food = packetUpdateHelath.Food;
            GameController.Player.Saturation = packetUpdateHelath.Saturation;
        }

        public void HandlePacketMultiBlockChange(PacketMultiBlockChange packetMultiBlockChange)
        {
            var chunkX = packetMultiBlockChange.ChunkXPosiiton*16;
            var chunkZ = packetMultiBlockChange.ChunkZPosition*16;

            if (packetMultiBlockChange.Metadata != null)
            {
                var buff = new StreamWrapper(packetMultiBlockChange.Metadata);
                for (var i = 0; i < packetMultiBlockChange.Size; i++)
                {
                    var short1 = buff.ReadShort();
                    var short2 = buff.ReadShort();
                    var id = short2 >> 4 & 4095;
                    var x = short1 >> 12 & 15;
                    var z = short1 >> 8 & 15;
                    var y = short1 & 255;
                    GameController.World.SetBlock(chunkX + x, y, chunkZ + z, id);
                }
            }
            GameController.World.Invalidate();
        }

        public void HandlePacketEntityStatus(PacketEntityStatus packetEntityStatus)
        {
            if (packetEntityStatus.EntityStatus == 2)
            {
                GameController.RemoveEntity(packetEntityStatus.EntityId);
            }
        }

        public void HandlePacketSpawnObject(PacketSpawnObject packetSpawnObject)
        {
            var entity = GameController.CreateLivingEntity(packetSpawnObject.EntityId, packetSpawnObject.Type);
            entity?.SetPosition(packetSpawnObject.X, packetSpawnObject.Y, packetSpawnObject.Z);
        }

        public void HandlePacketSpawnPlayer(PacketSpawnPlayer packetSpawnPlayer)
        {
            var player = GameController.CreatePlayer(packetSpawnPlayer.EntityId, packetSpawnPlayer.Name);
            player?.SetPosition(packetSpawnPlayer.X, packetSpawnPlayer.Y, packetSpawnPlayer.Z);
        }

        public void HandlePacketConfirmTransaction(PacketConfirmTransaction packetConfirmTransaction)
        {
            if (packetConfirmTransaction.Accepted) return;
            packetConfirmTransaction.Accepted = true;
            NetworkController.SendPacket(packetConfirmTransaction);
        }

        private static string PostUrl(PacketEncriptionRequest packetEncriptionRequest, byte[] secretKey, string id)
        {
            var request =
                WebRequest.Create(
                    $"http://ex-server.ru/joinserver.php?user={NetworkController.Username}&sessionId={id}&serverId={GetServerIdHash(packetEncriptionRequest.ServerId, secretKey, packetEncriptionRequest.Key)}");
            var responseStream = request.GetResponse().GetResponseStream();
            if (responseStream == null) return "";
            var responseString = new StreamReader(responseStream).ReadToEnd();
            return responseString;
        }

        private static string GetUserSession()
        {
            var document = new XmlDocument();
            document.Load(Settings.UserIdsPath);
            var root = document.DocumentElement;
            var id = "";
            Debug.Assert(root != null, "root != null");
            foreach (XmlNode child in root)
            {
                if (child.Attributes?.GetNamedItem("name").Value == NetworkController.Username)
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
    }
}