using System;
using System.Collections.Generic;
using System.IO;

namespace Base
{
	internal enum ParserState
	{
		PacketSize,
		PacketBody,
        Finish
	}

    public class PacketParser : Disposer
	{
        private Buffer buffer;

		private int packetSize;
		private ParserState state = ParserState.PacketSize;
        private Packet packet = null;
        byte[] size = new byte[4];
        public void Init(Buffer buffer)
        {
            this.buffer = buffer;
            state = ParserState.PacketSize;
        }
		public Packet Parse()
		{
			if (this.state == ParserState.Finish)
			{
                return null;
            }

			bool finish = false;
			while (!finish)
			{
				switch (this.state)
				{
					case ParserState.PacketSize:
						if (this.buffer.Length < 4)
						{
							finish = true;
						}
						else
						{
                            this.buffer.Read(size, 4);
                            this.packetSize = BitConverter.ToInt32(size, 0);
                            if (packetSize <= 0)
							{
								throw new Exception($"packet size == 0, size == {this.packetSize}.");
							}
                            this.state = ParserState.PacketBody;
						}
						break;
					case ParserState.PacketBody:
						if (this.buffer.Length < this.packetSize)
						{
							finish = true;
						}
						else
						{
                            if (packet == null)
                            {
                                packet = Packet.Take(this.packetSize);
                            }
                            this.buffer.Read(packet.Stream, this.packetSize);
							this.state = ParserState.Finish;
							finish = true;
						}
						break;
				}
			}
            return this.packet;
        }
        public void Start()
        {
            if (this.packet != null)
                Packet.Back(this.packet);
            this.packet = null;
            this.state = ParserState.PacketSize;
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            if (this.packet != null)
                Packet.Back(this.packet);
            this.packet = null;
        }
    }
}