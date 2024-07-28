// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BufferPool
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class BufferPool
  {
    private readonly object _poolLock = new object();
    private readonly BufferPool.BufferInfo[] _buffers;
    private readonly int _bufferSize;
    private readonly int _initialPoolSize;
    private readonly int _allocIncrement;
    private readonly int _maxPoolSize;
    private readonly string _instanceName;
    private int _capacity;
    private int _count;
    private readonly VssPerformanceCounter _allocatedCounter;
    private readonly VssPerformanceCounter _inUseCounter;
    private readonly VssPerformanceCounter _unPooledCounter;

    public BufferPool(int bufferSize, int initialNumberOfEntries)
    {
      this._bufferSize = bufferSize;
      this._instanceName = string.Format("{0}K", (object) (bufferSize / 1024));
      this._initialPoolSize = initialNumberOfEntries;
      this._allocIncrement = Math.Max(initialNumberOfEntries / 2, 1);
      this._maxPoolSize = 16 * initialNumberOfEntries;
      this._buffers = new BufferPool.BufferInfo[this._maxPoolSize];
      this._count = 0;
      for (int index = 0; index < initialNumberOfEntries; ++index)
        this._buffers[index] = new BufferPool.BufferInfo(this._bufferSize);
      this._capacity = initialNumberOfEntries;
      this._allocatedCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentCount", this._instanceName);
      this._inUseCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentlyUsed", this._instanceName);
      this._unPooledCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_BufferPoolCurrentlyUsedUnpooled", this._instanceName);
      this._allocatedCounter.IncrementBy((long) initialNumberOfEntries);
    }

    internal byte[] New() => this.New(true);

    internal byte[] New(bool cleanBuffer)
    {
      Interlocked.Increment(ref this._count);
      this._inUseCounter.Increment();
      while (true)
      {
        int capacity = this._capacity;
        for (int index = 0; index < this._capacity; ++index)
        {
          BufferPool.BufferInfo buffer = this._buffers[index];
          if (buffer.TryGet())
            return buffer.Buffer;
        }
        if (this._capacity < this._maxPoolSize)
        {
          lock (this._poolLock)
          {
            if (capacity == this._capacity)
            {
              if (this._capacity < this._maxPoolSize)
              {
                int num = Math.Min(this._allocIncrement, this._buffers.Length - this._capacity);
                for (int index = 0; index < num; ++index)
                  this._buffers[index + this._capacity] = new BufferPool.BufferInfo(this._bufferSize);
                this._capacity += num;
                this._allocatedCounter.IncrementBy((long) num);
              }
            }
          }
        }
        else
          break;
      }
      this._unPooledCounter.Increment();
      return new byte[this._bufferSize];
    }

    internal void Release(byte[] buffer)
    {
      if (buffer == null)
        return;
      Interlocked.Decrement(ref this._count);
      this._inUseCounter.Decrement();
      for (int index = 0; index < this._capacity; ++index)
      {
        if (this._buffers[index].Buffer == buffer)
        {
          this._buffers[index].Return(buffer);
          return;
        }
      }
      this._unPooledCounter.Decrement();
    }

    internal int BufferSize => this._bufferSize;

    private class BufferInfo
    {
      private int inUse;

      public BufferInfo(int bufferSize)
      {
        this.Buffer = new byte[bufferSize];
        this.inUse = 0;
      }

      public bool TryGet() => Interlocked.CompareExchange(ref this.inUse, 1, 0) == 0;

      public void Return(byte[] buffer)
      {
        if (buffer != this.Buffer || this.inUse == 0)
          throw new InvalidOperationException();
        Array.Clear((Array) buffer, 0, buffer.Length);
        this.inUse = 0;
      }

      public byte[] Buffer { get; private set; }
    }
  }
}
