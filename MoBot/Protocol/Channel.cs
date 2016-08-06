using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MoBot.Protocol.Packets;
using MoBot.Protocol.Packets.Handshake;
using MoBot.Protocol.Packets.Play;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MoBot.Protocol
{
    public class Channel
    {
        public enum State
        {
            Login,
            Play,
            Ping
        }

        private StreamWrapper _channel;

        private readonly HashSet<int> _ignoredIds = new HashSet<int> {
            3, //PacketTimeUpdate
            4, //PacketEntityEquipment
            5, //PacketSpawnPosition
            18,//PacketEntityVelocity
            22, //PacketEntityLook
            25, //PacketEntityHeadLook
            28, //TODO: PacketEntityMetadata
            31, //TODO: PacketSetExperience
            32, //PacketEntityProperties
            41, //PacketSoundEffect
            53, //PacketUpdateTileEntity
            55, //PacketStatistics
            56, //PacketPlayerListItem
            62, //PacketTeams
        };

        private readonly Dictionary<int, Type> _loginMap = new Dictionary<int, Type>
        {
            {0, typeof(PacketDisconnect)},
            {1, typeof(PacketEncriptionRequest)},
            {2, typeof(PacketLoginSuccess)}
        };

        private readonly Dictionary<int, Type> _playMap = new Dictionary<int, Type>
        {
            {0, typeof(PacketKeepAlive)},
            {1, typeof(PacketJoinGame)},
            {2, typeof(PacketChat)},
            {6, typeof(PacketUpdateHealth)},
            {7, typeof(PacketRespawn)},
            {8, typeof(PacketPlayerPosLook)},
            {9, typeof(PacketHeldItemChange)},
            {12, typeof(PacketSpawnPlayer)},
            {14, typeof(PacketSpawnObject)},
            {15, typeof(PacketSpawnMob)},
            {19, typeof(PacketDestroyEntities) },
            {20, typeof(PacketEntity)},
            {21, typeof(PacketEntity.PacketEntityMove)},
            {23, typeof(PacketEntity.PacketEntityMove)},
            {24, typeof(PacketEntityTeleport)},
            {26, typeof(PacketEntityStatus)},
            {33, typeof(PacketChunkData)},
            {34, typeof(PacketMultiBlockChange)},
            {35, typeof(PacketBlockChange)},
            {38, typeof(PacketMapChunk)},
            {47, typeof(PacketSetSlot)},
            {48, typeof(PacketWindowItems)},
            {50, typeof(PacketConfirmTransaction)},
            {57, typeof(PacketPlayerAbilities)},
            {63, typeof(PacketCustomPayload)},
            {64, typeof(PacketDisconnect)}
        };

        private readonly Dictionary<Type, int> _reverseLoginMap = new Dictionary<Type, int>
        {
            {typeof(PacketHandshake), 0},
            {typeof(PacketLoginStart), 0},
            {typeof(PacketEncriptionResponse), 1}
        };

        private readonly Dictionary<Type, int> _reversePlayMap = new Dictionary<Type, int>()
        {
            {typeof(PacketKeepAlive), 0},
            {typeof(PacketChat), 1},
            {typeof(PacketUseEntity), 2 },
            {typeof(PacketPlayerPosLook), 6},
            {typeof(PacketHeldItemChange), 9},
            {typeof(PacketClickWindow), 14},
            {typeof(PacketConfirmTransaction), 15},
            {typeof(PacketClientStatus), 22 },
            {typeof(PacketCustomPayload), 23}
        };

        private readonly Dictionary<Type, int> _reversePingMap = new Dictionary<Type, int>()
        {
            {typeof(PacketHandshake), 0 },
            {typeof(EmptyPacket), 0 }
        };

        private readonly Dictionary<int, Type> _pingMap = new Dictionary<int, Type>()
        {
            {0, typeof(PacketResponse) }
        };

        private Dictionary<int, Type> _currentMap;
        private Dictionary<Type, int> _currentReverseMap;

        public Channel(Stream networkStream, State state)
        {
            _channel = new StreamWrapper(networkStream);
            ChangeState(state);
        }

        public Channel(Stream networkStream)
        {
            _channel = new StreamWrapper(networkStream);
            ChangeState(State.Ping);
        }

        public void ChangeState(State state)
        {
            switch (state)
            {
                case State.Login:
                    _currentMap = _loginMap;
                    _currentReverseMap = _reverseLoginMap;
                    break;
                case State.Play:
                    _currentReverseMap = _reversePlayMap;
                    _currentMap = _playMap;
                    break;
                case State.Ping:
                    _currentReverseMap = _reversePingMap;
                    _currentMap = _pingMap;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void EncriptChannel(byte[] secretKey)
        {
            BufferedBlockCipher output = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
            output.Init(true, new ParametersWithIV(new KeyParameter(secretKey), secretKey, 0, 16));
            BufferedBlockCipher input = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
            input.Init(false, new ParametersWithIV(new KeyParameter(secretKey), secretKey, 0, 16));
            var cipherStream =  new CipherStream(_channel.GetStream(), input, output);
            _channel = new StreamWrapper(cipherStream);
        }

        public void SendPacket(Packet packet)
        {
            if (!_currentReverseMap.ContainsKey(packet.GetType()))
            {
                Console.WriteLine(packet.GetType());
                return;
            }
            var id = _currentReverseMap[packet.GetType()];
            var tmp = new StreamWrapper();
            tmp.WriteVarInt(id);
            packet.WritePacketData(tmp);

            var buffer = tmp.GetBlob();
            _channel.WriteVarInt((int)buffer.ActualLength);
            _channel.WriteBytes(buffer);
        }

        public Packet GetPacket()
        {
            var length = _channel.ReadVarInt();
            var data = _channel.ReadBytes(length);
            var tmp = new StreamWrapper(data);
            var id = tmp.ReadVarInt();
            Type packetType;
            if (!_currentMap.TryGetValue(id, out packetType))
            {
                if (!_ignoredIds.Contains(id))
                    Program.GetLogger().Error($"Unknown packet id : {id}");
                return null;
            }
            Debug.Assert(packetType != null, $"Error in getting packet {id}");
            var packet = Activator.CreateInstance(packetType) as Packet;
            packet?.ReadPacketData(tmp);
            return packet;
        }
    }
}