using System;
using System.Collections.Generic;
using System.IO;

namespace Base
{
	public class Buffer : Disposer
	{
		public const int ChunkSize = 8 * 1024;

		private readonly LinkedList<byte[]> bufferList = new LinkedList<byte[]>();

		public int LastIndex { get; set; }

		public int FirstIndex { get; set; }

		public int Length
		{
			get
			{
				int c = 0;
				if (this.bufferList.Count == 0)
				{
					c = 0;
				}
				else
				{
					c = (this.bufferList.Count - 1) * ChunkSize + this.LastIndex - this.FirstIndex;
				}
				if (c < 0)
				{
					Log.Error("TBuffer count < 0: {0}, {1}, {2}".Fmt(bufferList.Count, this.LastIndex, this.FirstIndex));
				}
				return c;
			}
		}

        static Queue<byte[]> cacheQueue = new Queue<byte[]>();
        public const int CacheMaxCount = 8 * 1024 * 1024 / ChunkSize;
        static byte[] Take()
        {
            byte[] buff = null;
            if (cacheQueue.Count > 0)
            {
                buff = cacheQueue.Dequeue();
            }
            else
            {
                buff = new byte[ChunkSize];
            }
            return buff;
        }
        static void Back(byte[] buff)
        {
            if(cacheQueue.Count < CacheMaxCount)
                cacheQueue.Enqueue(buff);
        }
		public void AddLast()
		{
            this.bufferList.AddLast(Take());
        }

		public void RemoveFirst()
		{
            if(this.bufferList.First != null)
            {
                Back(this.bufferList.First.Value);
            }
			this.bufferList.RemoveFirst();
		}

		public byte[] First
		{
			get
			{
				if (this.bufferList.First == null)
				{
					this.AddLast();
				}
				return this.bufferList.First.Value;
			}
		}

		public byte[] Last
		{
			get
			{
				if (this.bufferList.Last == null)
				{
					this.AddLast();
				}
				return this.bufferList.Last.Value;
			}
		}

        public void Read(byte[] buffer, int count)
        {
            if (this.Length < count)
            {
                throw new Exception($"bufferList size < n, bufferList: {this.Length} buffer length: {buffer.Length} {count}");
            }
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
        }
        public void Read(MemoryStream buffer, int count)
        {
            if (this.Length < count)
            {
                throw new Exception($"bufferList size < n, bufferList: {this.Length} buffer length: {buffer.Length} {count}");
            }
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    buffer.Write(this.First, this.FirstIndex, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    buffer.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
        }
        public void Write(byte[] buffer)
		{
			int alreadyCopyCount = 0;
			while (alreadyCopyCount < buffer.Length)
			{
				if (bufferList.Count == 0 || this.LastIndex == ChunkSize)
				{
                    AddLast();
					this.LastIndex = 0;
				}

				int n = buffer.Length - alreadyCopyCount;
				if (ChunkSize - this.LastIndex > n)
				{
					Array.Copy(buffer, alreadyCopyCount, this.bufferList.Last.Value, this.LastIndex, n);
					this.LastIndex += buffer.Length - alreadyCopyCount;
					alreadyCopyCount += n;
				}
				else
				{
					Array.Copy(buffer, alreadyCopyCount, this.bufferList.Last.Value, this.LastIndex, ChunkSize - this.LastIndex);
					alreadyCopyCount += ChunkSize - this.LastIndex;
					this.LastIndex = ChunkSize;
				}
			}
		}
        public void Write(MemoryStream stream)
        {
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < stream.Length)
            {
                if (bufferList.Count == 0 || this.LastIndex == ChunkSize)
                {
                    AddLast();
                    this.LastIndex = 0;
                }

                int n = (int)stream.Length - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n)
                {
                    stream.Read(this.bufferList.Last.Value, this.LastIndex, n);
                    this.LastIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Read(this.bufferList.Last.Value, this.LastIndex, ChunkSize - this.LastIndex);
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }
        public void Write(byte[] buffer, int count)
        {
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (bufferList.Count == 0 || this.LastIndex == ChunkSize)
                {
                    AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n)
                {
                    Array.Copy(buffer, alreadyCopyCount, this.bufferList.Last.Value, this.LastIndex, n);
                    this.LastIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    Array.Copy(buffer, alreadyCopyCount, this.bufferList.Last.Value, this.LastIndex, ChunkSize - this.LastIndex);
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();

            LastIndex = 0;
            FirstIndex = 0;
            while(bufferList.Count > 0)
            {
                RemoveFirst();
            }
        }
    }
}