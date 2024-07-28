// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.MemoryPool
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class MemoryPool : IMemoryPool
  {
    internal static readonly byte[] EmptyArray = new byte[0];
    private readonly MemoryPool.Pool<byte> _pool1 = new MemoryPool.Pool<byte>();
    private readonly MemoryPool.Pool<byte> _pool2 = new MemoryPool.Pool<byte>();
    private readonly MemoryPool.Pool<char> _pool3 = new MemoryPool.Pool<char>();

    public byte[] AllocByte(int minimumSize)
    {
      if (minimumSize == 0)
        return MemoryPool.EmptyArray;
      if (minimumSize <= 1024)
        return this._pool1.Alloc(1024);
      return minimumSize <= 2048 ? this._pool2.Alloc(2048) : new byte[minimumSize];
    }

    public void FreeByte(byte[] memory)
    {
      if (memory == null)
        return;
      switch (memory.Length)
      {
        case 1024:
          this._pool1.Free(memory, 256);
          break;
        case 2048:
          this._pool2.Free(memory, 64);
          break;
      }
    }

    public char[] AllocChar(int minimumSize)
    {
      if (minimumSize == 0)
        return new char[0];
      return minimumSize <= 128 ? this._pool3.Alloc(128) : new char[minimumSize];
    }

    public void FreeChar(char[] memory)
    {
      if (memory == null || memory.Length != 128)
        return;
      this._pool3.Free(memory, 256);
    }

    public ArraySegment<byte> AllocSegment(int minimumSize) => new ArraySegment<byte>(this.AllocByte(minimumSize));

    public void FreeSegment(ArraySegment<byte> segment) => this.FreeByte(segment.Array);

    private class Pool<T>
    {
      private readonly Stack<T[]> _stack = new Stack<T[]>();
      private readonly object _sync = new object();

      public T[] Alloc(int size)
      {
        lock (this._sync)
        {
          if (this._stack.Count != 0)
            return this._stack.Pop();
        }
        return new T[size];
      }

      public void Free(T[] value, int limit)
      {
        lock (this._sync)
        {
          if (this._stack.Count >= limit)
            return;
          this._stack.Push(value);
        }
      }
    }
  }
}
