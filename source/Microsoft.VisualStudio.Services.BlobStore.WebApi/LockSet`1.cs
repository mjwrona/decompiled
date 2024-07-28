// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.LockSet`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public sealed class LockSet<TKey> where TKey : IEquatable<TKey>
  {
    private static long _currentHandleId = 1;
    private readonly ConcurrentDictionary<TKey, LockSet<TKey>.LockHandle> _exclusiveLocks = new ConcurrentDictionary<TKey, LockSet<TKey>.LockHandle>();

    public async Task<LockSet<TKey>.LockHandle> Acquire(TKey key)
    {
      LockSet<TKey> locks = this;
      LockSet<TKey>.LockHandle thisHandle = new LockSet<TKey>.LockHandle(locks, key);
      LockSet<TKey>.LockHandle finalValue;
      while (!locks._exclusiveLocks.GetOrAdd<TKey, LockSet<TKey>.LockHandle>(key, thisHandle, out finalValue))
      {
        ValueUnit valueUnit = await finalValue.TaskCompletionSource.Task.ConfigureAwait(false);
      }
      LockSet<TKey>.LockHandle lockHandle = thisHandle;
      thisHandle = new LockSet<TKey>.LockHandle();
      return lockHandle;
    }

    private void Release(LockSet<TKey>.LockHandle handle)
    {
      this._exclusiveLocks.TryRemoveSpecific<TKey, LockSet<TKey>.LockHandle>(handle.Key, handle);
      Task.Run((Action) (() => handle.TaskCompletionSource.SetResult(ValueUnit.Void)));
    }

    public async Task<LockSet<TKey>.LockHandleSet> Acquire(IEnumerable<TKey> keys)
    {
      LockSet<TKey> locks = this;
      List<TKey> keyList = new List<TKey>(keys);
      keyList.Sort();
      List<LockSet<TKey>.LockHandle> handles = new List<LockSet<TKey>.LockHandle>(keyList.Count);
      foreach (TKey key in keyList)
      {
        List<LockSet<TKey>.LockHandle> lockHandleList = handles;
        lockHandleList.Add(await locks.Acquire(key).ConfigureAwait(false));
        lockHandleList = (List<LockSet<TKey>.LockHandle>) null;
      }
      LockSet<TKey>.LockHandleSet lockHandleSet = new LockSet<TKey>.LockHandleSet(locks, (IEnumerable<LockSet<TKey>.LockHandle>) handles);
      handles = (List<LockSet<TKey>.LockHandle>) null;
      return lockHandleSet;
    }

    public struct LockHandle : IEquatable<LockSet<TKey>.LockHandle>, IDisposable
    {
      private readonly long _handleId;
      private LockSet<TKey> _locks;
      internal readonly SafeTaskCompletionSource<ValueUnit> TaskCompletionSource;

      public TKey Key { get; }

      internal LockHandle(LockSet<TKey> locks, TKey key)
      {
        this.TaskCompletionSource = new SafeTaskCompletionSource<ValueUnit>();
        this._locks = locks;
        this.Key = key;
        this._handleId = Interlocked.Increment(ref LockSet<TKey>._currentHandleId);
      }

      public void Dispose()
      {
        this._locks.Release(this);
        this._locks = (LockSet<TKey>) null;
      }

      public bool Equals(LockSet<TKey>.LockHandle other) => this == other;

      public override bool Equals(object obj) => obj is LockSet<TKey>.LockHandle other && this.Equals(other);

      public override int GetHashCode() => (int) this._handleId;

      internal void MarkUnused() => this.TaskCompletionSource.MarkTaskAsUnused();

      public static bool operator ==(LockSet<TKey>.LockHandle left, LockSet<TKey>.LockHandle right) => left._handleId == right._handleId;

      public static bool operator !=(LockSet<TKey>.LockHandle left, LockSet<TKey>.LockHandle right) => !(left == right);
    }

    public sealed class LockHandleSet : IDisposable
    {
      private readonly LockSet<TKey> _locks;
      private readonly IEnumerable<LockSet<TKey>.LockHandle> _handles;

      public LockHandleSet(LockSet<TKey> locks, IEnumerable<LockSet<TKey>.LockHandle> handles)
      {
        this._locks = locks;
        this._handles = handles;
      }

      public void Dispose()
      {
        foreach (LockSet<TKey>.LockHandle handle in this._handles)
          this._locks.Release(handle);
      }
    }
  }
}
