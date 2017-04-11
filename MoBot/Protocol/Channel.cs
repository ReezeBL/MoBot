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

        private StreamWrapper channel;

        private readonly HashSet<int> ignoredIds = new HashSet<int> {
            3,  //PacketTimeUpdate
            4,  //PacketEntityEquipment
            5,  //PacketSpawnPosition
            11, //PacketAnimation
            13, //PacketCollectItem
            17, //PacketSpawnExperienceOrb
            18, //PacketEntityVelocity
            22, //PacketEntityLook
            25, //PacketEntityHeadLook
            27, //PacketEntityAttach
            28, //TODO: PacketEntityMetadata
            29, //PacketEntityEffect
            30, //PacketEntityRemoveEffect
            31, //TODO: PacketSetExperience
            32, //PacketEntityProperties
            36, //PacketBlockAction
            37, //PacketBlockBreakAnim
            40, //PacketEffect
            43, //PacketChangeGameState
            41, //PacketSoundEffect
            51, //PacketUpdateSign
            55, //PacketStatistics
            56, //PacketPlayerListItem
            62, //PacketTeams
            -26, //Говно с экскалибура
        };

        private readonly Dictionary<int, Type> loginMap = new Dictionary<int, Type>
        {
            {0, typeof(PacketDisconnect)},
            {1, typeof(PacketEncriptionRequest)},
            {2, typeof(PacketLoginSuccess)}
        };

        private readonly Dictionary<int, Type> playMap = new Dictionary<int, Type>
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
            {45, typeof(PacketOpenWindow) },
            {46, typeof(PacketCloseWindow)},
            {47, typeof(PacketSetSlot)},
            {48, typeof(PacketWindowItems)},
            {50, typeof(PacketConfirmTransaction)},
            {53, typeof(PacketUpdateTileEntity)},
            {57, typeof(PacketPlayerAbilities)},
            {63, typeof(PacketCustomPayload)},
            {64, typeof(PacketDisconnect)}
        };

        private readonly Dictionary<Type, int> reverseLoginMap = new Dictionary<Type, int>
        {
            {typeof(PacketHandshake), 0},
            {typeof(PacketLoginStart), 0},
            {typeof(PacketEncriptionResponse), 1}
        };

        private readonly Dictionary<Type, int> reversePlayMap = new Dictionary<Type, int>()
        {
            {typeof(PacketKeepAlive), 0},
            {typeof(PacketChat), 1},
            {typeof(PacketUseEntity), 2},
            {typeof(PacketPlayerPosLook), 6},
            {typeof(PacketPlayerDigging), 7},
            {typeof(PacketPlayerBlockPlacement), 8},
            {typeof(PacketHeldItemChange), 9},
            {typeof(PacketCloseWindow), 13},
            {typeof(PacketClickWindow), 14},
            {typeof(PacketConfirmTransaction), 15},
            {typeof(PacketClientStatus), 22},
            {typeof(PacketCustomPayload), 23}
        };

        private readonly Dictionary<Type, int> reversePingMap = new Dictionary<Type, int>()
        {
            {typeof(PacketHandshake), 0 },
            {typeof(EmptyPacket), 0 }
        };

        private readonly Dictionary<int, Type> pingMap = new Dictionary<int, Type>()
        {
            {0, typeof(PacketResponse) }
        };

        private Dictionary<int, Type> currentMap;
        private Dictionary<Type, int> currentReverseMap;

        public Channel(Stream networkStream, State state)
        {
            channel = new StreamWrapper(networkStream);
            ChangeState(state);
        }

        public Channel(Stream networkStream)
        {
            channel = new StreamWrapper(networkStream);
            ChangeState(State.Ping);
        }

        public void ChangeState(State state)
        {
            switch (state)
            {
                case State.Login:
                    currentMap = loginMap;
                    currentReverseMap = reverseLoginMap;
                    break;
                case State.Play:
                    currentReverseMap = reversePlayMap;
                    currentMap = playMap;
                    break;
                case State.Ping:
                    currentReverseMap = reversePingMap;
                    currentMap = pingMap;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void EncriptChannel(byte[] secretKey)
        {
            var output = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
            output.Init(true, new ParametersWithIV(new KeyParameter(secretKey), secretKey, 0, 16));
            var input = new BufferedBlockCipher(new CfbBlockCipher(new AesFastEngine(), 8));
            input.Init(false, new ParametersWithIV(new KeyParameter(secretKey), secretKey, 0, 16));
            var cipherStream = new CipherStream(channel.GetStream(), input, output);
            channel = new StreamWrapper(cipherStream);
        }

        public void SendPacket(Packet packet)
        {
            if (!currentReverseMap.ContainsKey(packet.GetType()))
            {
                Console.WriteLine(packet.GetType());
                return;
            }
            var id = currentReverseMap[packet.GetType()];
            var tmp = new StreamWrapper();
            tmp.WriteVarInt(id);
            packet.WritePacketData(tmp);

            var buffer = tmp.GetBlob();
            channel.WriteVarInt((int)buffer.ActualLength);
            channel.WriteBytes(buffer);
        }

        public Packet GetPacket()
        {
            var length = channel.ReadVarInt();
            var data = channel.ReadBytes(length);
            var tmp = new StreamWrapper(data);
            var id = tmp.ReadVarInt();
            Type packetType;
            if (!currentMap.TryGetValue(id, out packetType))
            {
                if (!ignoredIds.Contains(id))
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