// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.SynchronizedPool`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class SynchronizedPool<T> where T : class
  {
    private const int maxPendingEntries = 128;
    private const int maxPromotionFailures = 64;
    private const int maxReturnsBeforePromotion = 64;
    private const int maxThreadItemsPerProcessor = 16;
    private SynchronizedPool<T>.Entry[] entries;
    private SynchronizedPool<T>.GlobalPool globalPool;
    private int maxCount;
    private SynchronizedPool<T>.PendingEntry[] pending;
    private int promotionFailures;

    public SynchronizedPool(int maxCount)
    {
      int length = maxCount;
      int num = 16 + SynchronizedPool<T>.SynchronizedPoolHelper.ProcessorCount;
      if (length > num)
        length = num;
      this.maxCount = maxCount;
      this.entries = new SynchronizedPool<T>.Entry[length];
      this.pending = new SynchronizedPool<T>.PendingEntry[4];
      this.globalPool = new SynchronizedPool<T>.GlobalPool(maxCount);
    }

    private object ThisLock => (object) this;

    public void Clear()
    {
      foreach (SynchronizedPool<T>.Entry entry in this.entries)
        entry.value = default (T);
      this.globalPool.Clear();
    }

    private void HandlePromotionFailure(int thisThreadID)
    {
      int num = this.promotionFailures + 1;
      if (num >= 64)
      {
        lock (this.ThisLock)
        {
          this.entries = new SynchronizedPool<T>.Entry[this.entries.Length];
          this.globalPool.MaxCount = this.maxCount;
        }
        this.PromoteThread(thisThreadID);
      }
      else
        this.promotionFailures = num;
    }

    private bool PromoteThread(int thisThreadID)
    {
      lock (this.ThisLock)
      {
        for (int index = 0; index < this.entries.Length; ++index)
        {
          int threadId = this.entries[index].threadID;
          if (threadId == thisThreadID)
            return true;
          if (threadId == 0)
          {
            this.globalPool.DecrementMaxCount();
            this.entries[index].threadID = thisThreadID;
            return true;
          }
        }
      }
      return false;
    }

    private void RecordReturnToGlobalPool(int thisThreadID)
    {
      SynchronizedPool<T>.PendingEntry[] pending = this.pending;
      for (int index = 0; index < pending.Length; ++index)
      {
        int threadId = pending[index].threadID;
        if (threadId == thisThreadID)
        {
          int num = pending[index].returnCount + 1;
          if (num >= 64)
          {
            pending[index].returnCount = 0;
            if (this.PromoteThread(thisThreadID))
              break;
            this.HandlePromotionFailure(thisThreadID);
            break;
          }
          pending[index].returnCount = num;
          break;
        }
        if (threadId == 0)
          break;
      }
    }

    private void RecordTakeFromGlobalPool(int thisThreadID)
    {
      SynchronizedPool<T>.PendingEntry[] pending = this.pending;
      for (int index = 0; index < pending.Length; ++index)
      {
        int threadId = pending[index].threadID;
        if (threadId == thisThreadID)
          return;
        if (threadId == 0)
        {
          lock (pending)
          {
            if (pending[index].threadID == 0)
            {
              pending[index].threadID = thisThreadID;
              return;
            }
          }
        }
      }
      if (pending.Length >= 128)
      {
        this.pending = new SynchronizedPool<T>.PendingEntry[pending.Length];
      }
      else
      {
        SynchronizedPool<T>.PendingEntry[] destinationArray = new SynchronizedPool<T>.PendingEntry[pending.Length * 2];
        Array.Copy((Array) pending, (Array) destinationArray, pending.Length);
        this.pending = destinationArray;
      }
    }

    public bool Return(T value)
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      if (managedThreadId == 0)
        return false;
      return this.ReturnToPerThreadPool(managedThreadId, value) || this.ReturnToGlobalPool(managedThreadId, value);
    }

    private bool ReturnToPerThreadPool(int thisThreadID, T value)
    {
      SynchronizedPool<T>.Entry[] entries = this.entries;
      for (int index = 0; index < entries.Length; ++index)
      {
        int threadId = entries[index].threadID;
        if (threadId == thisThreadID)
        {
          if ((object) entries[index].value != null)
            return false;
          entries[index].value = value;
          return true;
        }
        if (threadId == 0)
          break;
      }
      return false;
    }

    private bool ReturnToGlobalPool(int thisThreadID, T value)
    {
      this.RecordReturnToGlobalPool(thisThreadID);
      return this.globalPool.Return(value);
    }

    public T Take()
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      return managedThreadId == 0 ? default (T) : this.TakeFromPerThreadPool(managedThreadId) ?? this.TakeFromGlobalPool(managedThreadId);
    }

    private T TakeFromPerThreadPool(int thisThreadID)
    {
      SynchronizedPool<T>.Entry[] entries = this.entries;
      for (int index = 0; index < entries.Length; ++index)
      {
        int threadId = entries[index].threadID;
        if (threadId == thisThreadID)
        {
          T fromPerThreadPool = entries[index].value;
          if ((object) fromPerThreadPool == null)
            return default (T);
          entries[index].value = default (T);
          return fromPerThreadPool;
        }
        if (threadId == 0)
          break;
      }
      return default (T);
    }

    private T TakeFromGlobalPool(int thisThreadID)
    {
      this.RecordTakeFromGlobalPool(thisThreadID);
      return this.globalPool.Take();
    }

    private struct Entry
    {
      public int threadID;
      public T value;
    }

    private struct PendingEntry
    {
      public int returnCount;
      public int threadID;
    }

    private static class SynchronizedPoolHelper
    {
      public static readonly int ProcessorCount = SynchronizedPool<T>.SynchronizedPoolHelper.GetProcessorCount();

      [EnvironmentPermission(SecurityAction.Assert, Read = "NUMBER_OF_PROCESSORS")]
      private static int GetProcessorCount() => Environment.ProcessorCount;
    }

    private class GlobalPool
    {
      private Stack<T> items;
      private int maxCount;

      public GlobalPool(int maxCount)
      {
        this.items = new Stack<T>();
        this.maxCount = maxCount;
      }

      public int MaxCount
      {
        get => this.maxCount;
        set
        {
          lock (this.ThisLock)
          {
            while (this.items.Count > value)
              this.items.Pop();
            this.maxCount = value;
          }
        }
      }

      private object ThisLock => (object) this;

      public void DecrementMaxCount()
      {
        lock (this.ThisLock)
        {
          if (this.items.Count == this.maxCount)
            this.items.Pop();
          --this.maxCount;
        }
      }

      public T Take()
      {
        if (this.items.Count > 0)
        {
          lock (this.ThisLock)
          {
            if (this.items.Count > 0)
              return this.items.Pop();
          }
        }
        return default (T);
      }

      public bool Return(T value)
      {
        if (this.items.Count < this.MaxCount)
        {
          lock (this.ThisLock)
          {
            if (this.items.Count < this.MaxCount)
            {
              this.items.Push(value);
              return true;
            }
          }
        }
        return false;
      }

      public void Clear()
      {
        lock (this.ThisLock)
          this.items.Clear();
      }
    }
  }
}
