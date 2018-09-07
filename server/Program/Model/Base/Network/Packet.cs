using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Base
{
    public class BigPacketCache
    {
        int maxSize;
        int minSize;
        Queue<Packet> packets = new Queue<Packet>();
        int createCount;
        public const int OffsetSize = 64 * 1024;
        public int MaxSize { get { return maxSize; } }
        public int MinSize { get { return minSize; } }
        public int CreateCount { get { return createCount; } }
        public int Count { get { return packets.Count; } }

        public BigPacketCache(int size)
        {
            int offset = size / OffsetSize;
            this.minSize = offset * OffsetSize;
            this.maxSize = (offset + 1) * OffsetSize;
        }

        public Packet Take()
        {
            if (packets.Count > 0)
            {
                return packets.Dequeue();
            }
            createCount++;
            return new Packet(maxSize, PacketType.Big);
        }
        public void Back(Packet packet)
        {
            if (packet.Capacity != maxSize)
            {
                return;
            }
            packets.Enqueue(packet);
        }
        public void Remove(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (packets.Count > 0)
                    packets.Dequeue();
                else
                    break;
            }
        }
    }
    public enum PacketType
    {
        Normal,
        Big,
        Dynamic
    }
    public class Packet
    {
        MemoryStream stream;
        public MemoryStream Stream { get { return stream; } }
        PacketType pType;
        public PacketType PType { get { return pType; } }
        public int Capacity { get { return Stream.Capacity; } }

        public const int MaxLength = 8 * 1024;
        public const int NormalCacheCount = 8 * 1024 * 1024 / MaxLength;
        public const int DynamicSizeCacheCount = 8 * 1024 * 1024 / BigPacketCache.OffsetSize;

        static Queue<Packet> normalPackets = new Queue<Packet>();
        static List<BigPacketCache> bigPackets = new List<BigPacketCache>();
        static Queue<Packet> dynamicSizePackets = new Queue<Packet>();
        public static Packet Take(int length = 0)
        {
            Packet packet = null;
            if (length == 0)
            {
                if (dynamicSizePackets.Count > 0)
                {
                    packet = dynamicSizePackets.Dequeue();
                }
                else
                {
                    packet = new Packet(BigPacketCache.OffsetSize, PacketType.Dynamic);
                }
            }
            else
            {
                if (length <= MaxLength)
                {
                    if (normalPackets.Count > 0)
                    {
                        packet = normalPackets.Dequeue();
                    }
                    else
                    {
                        packet = new Packet();
                    }
                }
                else
                {
                    foreach (BigPacketCache bigPacket in bigPackets)
                    {
                        if (length >= bigPacket.MinSize && length <= bigPacket.MaxSize)
                        {
                            packet = bigPacket.Take();
                            break;
                        }
                    }
                    if (packet == null)
                    {
                        BigPacketCache bigCache = new BigPacketCache(length);
                        bigPackets.Add(bigCache);
                        packet = bigCache.Take();
                    }
                }
            }
            packet.Init();
            return packet;
        }
        public static void Back(Packet packet)
        {
            if (packet.PType == PacketType.Dynamic)
            {
                if (dynamicSizePackets.Count < DynamicSizeCacheCount)
                    dynamicSizePackets.Enqueue(packet);
            }
            else
            {
                if (packet.Capacity == MaxLength)
                {
                    if (normalPackets.Count < NormalCacheCount)
                    {//最多缓存数量,多余的不回收
                        normalPackets.Enqueue(packet);
                        return;
                    }
                }
                else if (packet.Capacity > MaxLength)
                {
                    foreach (BigPacketCache bigCache in bigPackets)
                    {
                        if (packet.Capacity == bigCache.MaxSize)
                        {
                            bigCache.Back(packet);
                            return;
                        }
                    }
                }
                Console.WriteLine($"Can Not Back Pack , Size {packet.Capacity}");
            }
        }
        public Packet(int length = MaxLength, PacketType pType = PacketType.Normal)
        {
            stream = new MemoryStream(length);
            this.pType = pType;
        }
        public void Init()
        {
            Stream.SetLength(0);
            Stream.Position = 0;
        }
    }
}
