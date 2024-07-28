// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.InternalBufferManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal abstract class InternalBufferManager
  {
    public abstract byte[] TakeBuffer(int bufferSize);

    public abstract void ReturnBuffer(byte[] buffer);

    public abstract void Clear();

    public static InternalBufferManager Create(
      long maxBufferPoolSize,
      int maxBufferSize,
      bool isTransportBufferPool)
    {
      if (maxBufferPoolSize == 0L)
        return (InternalBufferManager) InternalBufferManager.GCBufferManager.Value;
      return isTransportBufferPool ? (InternalBufferManager) new InternalBufferManager.PreallocatedBufferManager(maxBufferPoolSize, maxBufferSize) : (InternalBufferManager) new InternalBufferManager.PooledBufferManager(maxBufferPoolSize, maxBufferSize);
    }

    public static byte[] AllocateByteArray(int size) => new byte[size];

    private class PreallocatedBufferManager : InternalBufferManager
    {
      private int maxBufferSize;
      private int medBufferSize;
      private int smallBufferSize;
      private byte[][] buffersList;
      private GCHandle[] handles;
      private ConcurrentStack<byte[]> freeSmallBuffers;
      private ConcurrentStack<byte[]> freeMedianBuffers;
      private ConcurrentStack<byte[]> freeLargeBuffers;

      internal PreallocatedBufferManager(long maxMemoryToPool, int maxBufferSize)
      {
        this.maxBufferSize = maxBufferSize;
        this.medBufferSize = maxBufferSize / 4;
        this.smallBufferSize = maxBufferSize / 16;
        long num1 = maxMemoryToPool / 3L;
        long num2 = num1 / (long) maxBufferSize;
        long num3 = num1 / (long) this.medBufferSize;
        long num4 = num1 / (long) this.smallBufferSize;
        long length = num2 + num3 + num4;
        this.buffersList = new byte[length][];
        this.handles = new GCHandle[length];
        this.freeSmallBuffers = new ConcurrentStack<byte[]>();
        this.freeMedianBuffers = new ConcurrentStack<byte[]>();
        this.freeLargeBuffers = new ConcurrentStack<byte[]>();
        int num5 = 0;
        int index1 = 0;
        while ((long) index1 < num2)
        {
          this.buffersList[index1] = new byte[maxBufferSize];
          this.handles[index1] = GCHandle.Alloc((object) this.buffersList[index1], GCHandleType.Pinned);
          this.freeLargeBuffers.Push(this.buffersList[index1]);
          ++index1;
          ++num5;
        }
        int num6 = num5;
        int index2 = num5;
        while ((long) index2 < num3 + (long) num5)
        {
          this.buffersList[index2] = new byte[this.medBufferSize];
          this.handles[index2] = GCHandle.Alloc((object) this.buffersList[index2], GCHandleType.Pinned);
          this.freeMedianBuffers.Push(this.buffersList[index2]);
          ++index2;
          ++num6;
        }
        for (int index3 = num6; (long) index3 < num4 + (long) num6; ++index3)
        {
          this.buffersList[index3] = new byte[this.smallBufferSize];
          this.handles[index3] = GCHandle.Alloc((object) this.buffersList[index3], GCHandleType.Pinned);
          this.freeSmallBuffers.Push(this.buffersList[index3]);
        }
      }

      public override byte[] TakeBuffer(int bufferSize)
      {
        if (bufferSize > this.maxBufferSize)
          return (byte[]) null;
        byte[] result = (byte[]) null;
        if (bufferSize <= this.smallBufferSize)
        {
          this.freeSmallBuffers.TryPop(out result);
          return result;
        }
        if (bufferSize <= this.medBufferSize)
        {
          this.freeMedianBuffers.TryPop(out result);
          return result;
        }
        this.freeLargeBuffers.TryPop(out result);
        return result;
      }

      public override void ReturnBuffer(byte[] buffer)
      {
        if (buffer.Length <= this.smallBufferSize)
          this.freeSmallBuffers.Push(buffer);
        else if (buffer.Length <= this.medBufferSize)
          this.freeMedianBuffers.Push(buffer);
        else
          this.freeLargeBuffers.Push(buffer);
      }

      public override void Clear()
      {
        for (int index = 0; index < this.buffersList.Length; ++index)
        {
          this.handles[index].Free();
          this.buffersList[index] = (byte[]) null;
        }
        this.buffersList = (byte[][]) null;
        this.freeSmallBuffers.Clear();
        this.freeMedianBuffers.Clear();
        this.freeLargeBuffers.Clear();
      }
    }

    private class PooledBufferManager : InternalBufferManager
    {
      private const int minBufferSize = 128;
      private const int maxMissesBeforeTuning = 8;
      private const int initialBufferCount = 1;
      private readonly object tuningLock;
      private int[] bufferSizes;
      private InternalBufferManager.PooledBufferManager.BufferPool[] bufferPools;
      private long remainingMemory;
      private bool areQuotasBeingTuned;
      private int totalMisses;

      public PooledBufferManager(long maxMemoryToPool, int maxBufferSize)
      {
        this.tuningLock = new object();
        this.remainingMemory = maxMemoryToPool;
        List<InternalBufferManager.PooledBufferManager.BufferPool> bufferPoolList = new List<InternalBufferManager.PooledBufferManager.BufferPool>();
        int bufferSize = 128;
        while (true)
        {
          long num1 = this.remainingMemory / (long) bufferSize;
          int limit = num1 > (long) int.MaxValue ? int.MaxValue : (int) num1;
          if (limit > 1)
            limit = 1;
          bufferPoolList.Add(InternalBufferManager.PooledBufferManager.BufferPool.CreatePool(bufferSize, limit));
          this.remainingMemory -= (long) limit * (long) bufferSize;
          if (bufferSize < maxBufferSize)
          {
            long num2 = (long) bufferSize * 2L;
            bufferSize = num2 <= (long) maxBufferSize ? (int) num2 : maxBufferSize;
          }
          else
            break;
        }
        this.bufferPools = bufferPoolList.ToArray();
        this.bufferSizes = new int[this.bufferPools.Length];
        for (int index = 0; index < this.bufferPools.Length; ++index)
          this.bufferSizes[index] = this.bufferPools[index].BufferSize;
      }

      public override void Clear()
      {
        for (int index = 0; index < this.bufferPools.Length; ++index)
          this.bufferPools[index].Clear();
      }

      private void ChangeQuota(
        ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool,
        int delta)
      {
        InternalBufferManager.PooledBufferManager.BufferPool bufferPool1 = bufferPool;
        int limit = bufferPool1.Limit + delta;
        InternalBufferManager.PooledBufferManager.BufferPool pool = InternalBufferManager.PooledBufferManager.BufferPool.CreatePool(bufferPool1.BufferSize, limit);
        for (int index = 0; index < limit; ++index)
        {
          byte[] buffer = bufferPool1.Take();
          if (buffer != null)
          {
            pool.Return(buffer);
            pool.IncrementCount();
          }
          else
            break;
        }
        this.remainingMemory -= (long) (bufferPool1.BufferSize * delta);
        bufferPool = pool;
      }

      private void DecreaseQuota(
        ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool)
      {
        this.ChangeQuota(ref bufferPool, -1);
      }

      private int FindMostExcessivePool()
      {
        long num1 = 0;
        int mostExcessivePool = -1;
        for (int index = 0; index < this.bufferPools.Length; ++index)
        {
          InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[index];
          if (bufferPool.Peak < bufferPool.Limit)
          {
            long num2 = (long) (bufferPool.Limit - bufferPool.Peak) * (long) bufferPool.BufferSize;
            if (num2 > num1)
            {
              mostExcessivePool = index;
              num1 = num2;
            }
          }
        }
        return mostExcessivePool;
      }

      private int FindMostStarvedPool()
      {
        long num1 = 0;
        int mostStarvedPool = -1;
        for (int index = 0; index < this.bufferPools.Length; ++index)
        {
          InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[index];
          if (bufferPool.Peak == bufferPool.Limit)
          {
            long num2 = (long) bufferPool.Misses * (long) bufferPool.BufferSize;
            if (num2 > num1)
            {
              mostStarvedPool = index;
              num1 = num2;
            }
          }
        }
        return mostStarvedPool;
      }

      private InternalBufferManager.PooledBufferManager.BufferPool FindPool(int desiredBufferSize)
      {
        for (int index = 0; index < this.bufferSizes.Length; ++index)
        {
          if (desiredBufferSize <= this.bufferSizes[index])
            return this.bufferPools[index];
        }
        return (InternalBufferManager.PooledBufferManager.BufferPool) null;
      }

      private void IncreaseQuota(
        ref InternalBufferManager.PooledBufferManager.BufferPool bufferPool)
      {
        this.ChangeQuota(ref bufferPool, 1);
      }

      public override void ReturnBuffer(byte[] buffer)
      {
        InternalBufferManager.PooledBufferManager.BufferPool pool = this.FindPool(buffer.Length);
        if (pool == null)
          return;
        if (buffer.Length != pool.BufferSize)
          throw Fx.Exception.Argument(nameof (buffer), SRCore.BufferIsNotRightSizeForBufferManager);
        if (!pool.Return(buffer))
          return;
        pool.IncrementCount();
      }

      public override byte[] TakeBuffer(int bufferSize)
      {
        InternalBufferManager.PooledBufferManager.BufferPool pool = this.FindPool(bufferSize);
        if (pool == null)
          return InternalBufferManager.AllocateByteArray(bufferSize);
        byte[] buffer = pool.Take();
        if (buffer != null)
        {
          pool.DecrementCount();
          return buffer;
        }
        if (pool.Peak == pool.Limit)
        {
          ++pool.Misses;
          if (++this.totalMisses >= 8)
            this.TuneQuotas();
        }
        return InternalBufferManager.AllocateByteArray(pool.BufferSize);
      }

      private void TuneQuotas()
      {
        if (this.areQuotasBeingTuned)
          return;
        bool lockTaken = false;
        try
        {
          Monitor.TryEnter(this.tuningLock, ref lockTaken);
          if (!lockTaken || this.areQuotasBeingTuned)
            return;
          this.areQuotasBeingTuned = true;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(this.tuningLock);
        }
        int mostStarvedPool = this.FindMostStarvedPool();
        if (mostStarvedPool >= 0)
        {
          InternalBufferManager.PooledBufferManager.BufferPool bufferPool = this.bufferPools[mostStarvedPool];
          if (this.remainingMemory < (long) bufferPool.BufferSize)
          {
            int mostExcessivePool = this.FindMostExcessivePool();
            if (mostExcessivePool >= 0)
              this.DecreaseQuota(ref this.bufferPools[mostExcessivePool]);
          }
          if (this.remainingMemory >= (long) bufferPool.BufferSize)
            this.IncreaseQuota(ref this.bufferPools[mostStarvedPool]);
        }
        for (int index = 0; index < this.bufferPools.Length; ++index)
          this.bufferPools[index].Misses = 0;
        this.totalMisses = 0;
        this.areQuotasBeingTuned = false;
      }

      private abstract class BufferPool
      {
        private int bufferSize;
        private int count;
        private int limit;
        private int misses;
        private int peak;

        public BufferPool(int bufferSize, int limit)
        {
          this.bufferSize = bufferSize;
          this.limit = limit;
        }

        public int BufferSize => this.bufferSize;

        public int Limit => this.limit;

        public int Misses
        {
          get => this.misses;
          set => this.misses = value;
        }

        public int Peak => this.peak;

        public void Clear()
        {
          this.OnClear();
          this.count = 0;
        }

        public void DecrementCount()
        {
          int num = this.count - 1;
          if (num < 0)
            return;
          this.count = num;
        }

        public void IncrementCount()
        {
          int num = this.count + 1;
          if (num > this.limit)
            return;
          this.count = num;
          if (num <= this.peak)
            return;
          this.peak = num;
        }

        internal abstract byte[] Take();

        internal abstract bool Return(byte[] buffer);

        internal abstract void OnClear();

        internal static InternalBufferManager.PooledBufferManager.BufferPool CreatePool(
          int bufferSize,
          int limit)
        {
          return bufferSize < 84976 ? (InternalBufferManager.PooledBufferManager.BufferPool) new InternalBufferManager.PooledBufferManager.BufferPool.SynchronizedBufferPool(bufferSize, limit) : (InternalBufferManager.PooledBufferManager.BufferPool) new InternalBufferManager.PooledBufferManager.BufferPool.LargeBufferPool(bufferSize, limit);
        }

        private class SynchronizedBufferPool : InternalBufferManager.PooledBufferManager.BufferPool
        {
          private SynchronizedPool<byte[]> innerPool;

          internal SynchronizedBufferPool(int bufferSize, int limit)
            : base(bufferSize, limit)
          {
            this.innerPool = new SynchronizedPool<byte[]>(limit);
          }

          internal override void OnClear() => this.innerPool.Clear();

          internal override byte[] Take() => this.innerPool.Take();

          internal override bool Return(byte[] buffer) => this.innerPool.Return(buffer);
        }

        private class LargeBufferPool : InternalBufferManager.PooledBufferManager.BufferPool
        {
          private Stack<byte[]> items;

          internal LargeBufferPool(int bufferSize, int limit)
            : base(bufferSize, limit)
          {
            this.items = new Stack<byte[]>(limit);
          }

          private object ThisLock => (object) this.items;

          internal override void OnClear()
          {
            lock (this.ThisLock)
              this.items.Clear();
          }

          internal override byte[] Take()
          {
            lock (this.ThisLock)
            {
              if (this.items.Count > 0)
                return this.items.Pop();
            }
            return (byte[]) null;
          }

          internal override bool Return(byte[] buffer)
          {
            lock (this.ThisLock)
            {
              if (this.items.Count < this.Limit)
              {
                this.items.Push(buffer);
                return true;
              }
            }
            return false;
          }
        }
      }
    }

    private class GCBufferManager : InternalBufferManager
    {
      private static InternalBufferManager.GCBufferManager value = new InternalBufferManager.GCBufferManager();

      private GCBufferManager()
      {
      }

      public static InternalBufferManager.GCBufferManager Value => InternalBufferManager.GCBufferManager.value;

      public override void Clear()
      {
      }

      public override byte[] TakeBuffer(int bufferSize) => InternalBufferManager.AllocateByteArray(bufferSize);

      public override void ReturnBuffer(byte[] buffer)
      {
      }
    }
  }
}
