using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoBot.Protocol.Packets;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Structure;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using MoBot.Protocol.Handshake;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using System.Xml;
using System.IO;
using System.Net.Sockets;
using MoBot.Protocol.Packets.Play;
using Newtonsoft.Json.Linq;

namespace MoBot.Protocol.Handlers
{
    class ClientHandler : IHandler
    {
        Model model;
        private NLog.Logger log = Program.getLogger();

        #region ServerHashCalculations
        private String GetServerIdHash(String ServerId, byte[] SecretKey, byte[] PublicKey)
        {
            byte[] IdBytes = System.Text.Encoding.ASCII.GetBytes(ServerId);
            byte[] data = new byte[IdBytes.Length + SecretKey.Length + PublicKey.Length];

            IdBytes.CopyTo(data, 0);
            SecretKey.CopyTo(data, IdBytes.Length);
            PublicKey.CopyTo(data, IdBytes.Length + SecretKey.Length);

            return JavaHexDigest(data);
        }
        private string JavaHexDigest(byte[] data)
        {
            var sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);
            bool negative = (hash[0] & 0x80) == 0x80;
            if (negative) // check for negative hashes
                hash = TwosCompliment(hash);
            // Create the string and trim away the zeroes
            string digest = GetHexString(hash).TrimStart('0');
            if (negative)
                digest = "-" + digest;
            return digest;
        }
        private string GetHexString(byte[] p)
        {
            string result = string.Empty;
            for (int i = 0; i < p.Length; i++)
                result += p[i].ToString("x2"); // Converts to hex string
            return result;
        }
        private byte[] TwosCompliment(byte[] p) // little endian
        {
            int i;
            bool carry = true;
            for (i = p.Length - 1; i >= 0; i--)
            {
                p[i] = (byte)~p[i];
                if (carry)
                {
                    carry = p[i] == 0xFF;
                    p[i]++;
                }
            }
            return p;
        }
        #endregion

        public ClientHandler(Model model)
        {
            this.model = model;
        }
        public void HandlePacketDisconnect(PacketDisconnect packetDisconnect)
        {
            model.Message(String.Format("You have been disconnected from the server! Reason: {0}", packetDisconnect.reason));
            model.Disconnect();
        }
        public void HandlePacketEncriptionRequest(PacketEncriptionRequest packetEncriptionRequest)
        {
            AsymmetricKeyParameter kp = PublicKeyFactory.CreateKey(packetEncriptionRequest.Key);
            var Key = (RsaKeyParameters)kp;
            IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            cipher.Init(true, Key);
            CipherKeyGenerator keygen = new CipherKeyGenerator();
            keygen.Init(new KeyGenerationParameters(new SecureRandom(), 128));
            var SecretKey = keygen.GenerateKey();
            var CryptedKey = cipher.DoFinal(SecretKey);
            var CryptedToken = cipher.DoFinal(packetEncriptionRequest.Token);
            #region PostURL
            Thread post = new Thread(() =>
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load("UserIDS.xml");
                    var Root = document.DocumentElement;
                    string ID = "";
                    foreach (XmlNode Child in Root)
                    {
                        if (Child.Attributes.GetNamedItem("name").Value == model.username)
                            ID = Child.InnerText;
                    }
                    WebRequest request = WebRequest.Create(String.Format("http://ex-server.ru/joinserver.php?user={0}&sessionId={1}&serverId={2}", model.username, ID, GetServerIdHash(packetEncriptionRequest.ServerID, SecretKey, packetEncriptionRequest.Key)));
                    var response = request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseString != "OK")
                        log.Error("Auth failed!\nAuth username: {1}\nAuth ID:{2}\nAuth response : {0}", responseString, model.username, ID);
                }
                catch (SocketException e)
                {
                    log.Error("Unable to connect to login server!");
                }
                catch (FileNotFoundException)
                {
                    log.Error("No UserIDS.xml file in directory!");
                }
                catch (XmlException e)
                {
                    log.Error("Unable to proceed XML file");
                }
            })
            { IsBackground = true };
            post.Start();
            #endregion
            model.mainChannel.SendPacket(new PacketEncriptionResponse { SharedSecret = CryptedKey, SharedSecretLength = CryptedKey.Length, Token = CryptedToken, TokenLength = CryptedToken.Length });
            model.mainChannel.EncriptChannel(SecretKey);
        }
        public void HandlePacketLoginSucess(PacketLoginSuccess packetLoginSuccess)
        {
            model.mainChannel.ChangeState(Channel.State.Play);
        }

        public void HandlePacketKeepAlive(PacketKeepAlive packetKeepAlive)
        {
            model.SendPacket(packetKeepAlive);
        }

        public void HandlePacketCustomPayload(PacketCustomPayload packetCustomPayload)
        {
            if(packetCustomPayload.channel == "FML|HS")
            {
                PacketBuffer payload = new PacketBuffer(packetCustomPayload.Payload);
                byte Discriminator = payload.ReadByte();
                if (Discriminator == 0)
                {
                    PacketBuffer answer = new PacketBuffer();
                    byte Version = payload.ReadByte();
                    answer.WriteByte(1);
                    answer.WriteByte(Version);
                    model.SendPacket(new PacketCustomPayload { channel = "FML|HS", MyPayload = answer.GetBlob() });
                    answer = new PacketBuffer();
                    answer.WriteByte(2);
                    answer.WriteVarInt(model.modList.Count);
                    foreach (JObject obj in model.modList)
                    {
                        answer.WriteString((String)obj["modid"]);
                        answer.WriteString((String)obj["version"]);
                    }
                    model.SendPacket(new PacketCustomPayload { channel = "FML|HS", MyPayload = answer.GetBlob() });
                }
                else if(Discriminator == 2)
                {
                    PacketBuffer answer = new PacketBuffer();
                    answer.WriteByte(255);
                    answer.WriteByte(2);
                    model.SendPacket(new PacketCustomPayload { channel = "FML|HS", MyPayload = answer.GetBlob() });
                }
                else if(Discriminator == 255)
                {
                    byte stage = payload.ReadByte();
                    PacketBuffer answer = new PacketBuffer();
                    answer.WriteByte(255);
                    if (stage == 3)
                        answer.WriteByte(5);
                    else if (stage == 2)
                        answer.WriteByte(4);
                    else
                        log.Info("Unhandled Ack Stage : {0}", stage);
                    model.SendPacket(new PacketCustomPayload { channel = "FML|HS", MyPayload = answer.GetBlob() });
                }
                else
                    log.Info("Unhandled FmlDescriptor : {0}", Discriminator);
            }
        }
    }
}
