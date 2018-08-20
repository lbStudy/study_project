﻿using System;
using System.Collections.Generic;
using Base;
using UnityEngine;
namespace Model
{
	public class TBuffer
	{
		public const int ChunkSize = 8192;

		private readonly LinkedList<byte[]> bufferList = new LinkedList<byte[]>();

        private readonly Queue<LinkedListNode<byte[]>> cacheBuffers = new Queue<LinkedListNode<byte[]>>();

		public int LastIndex { get; set; }

		public int FirstIndex { get; set; }

		public TBuffer()
		{
            AddLast();
        }

		public int Count
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
				     Debug.LogError("TBuffer count < 0: {0}, {1}, {2}".Fmt(bufferList.Count, this.LastIndex, this.FirstIndex));
				}
				return c;
			}
		}

		public void AddLast()
		{
            if(cacheBuffers.Count > 0)
            {
                this.bufferList.AddLast(cacheBuffers.Dequeue());
            }
            else
            {
                this.bufferList.AddLast(new byte[ChunkSize]);
            }
		}

		public void RemoveFirst()
		{
            if (bufferList.Count == 0)
                return;
            cacheBuffers.Enqueue(bufferList.First);
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

		public void RecvFrom(byte[] buffer)
		{
			if (this.Count < buffer.Length || buffer.Length == 0)
			{
				throw new Exception($"bufferList size < n, bufferList: {this.Count} buffer length: {buffer.Length}");
			}
			int alreadyCopyCount = 0;
			while (alreadyCopyCount < buffer.Length)
			{
				int n = buffer.Length - alreadyCopyCount;
				if (ChunkSize - this.FirstIndex > n)
				{
					Array.Copy(this.bufferList.First.Value, this.FirstIndex, buffer, alreadyCopyCount, n);
					this.FirstIndex += n;
					alreadyCopyCount += n;
				}
				else
				{
					Array.Copy(this.bufferList.First.Value, this.FirstIndex, buffer, alreadyCopyCount, ChunkSize - this.FirstIndex);
					alreadyCopyCount += ChunkSize - this.FirstIndex;
					this.FirstIndex = 0;
					RemoveFirst();
				}
			}
		}

		public void SendTo(byte[] buffer)
		{
			int alreadyCopyCount = 0;
			while (alreadyCopyCount < buffer.Length)
			{
				if (this.LastIndex == ChunkSize)
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
	}
}