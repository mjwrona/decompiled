// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Memoizer`2
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.OData.Edm
{
  internal sealed class Memoizer<TArg, TResult>
  {
    private readonly Func<TArg, TResult> function;
    private readonly Dictionary<TArg, Memoizer<TArg, TResult>.Result> resultCache;
    private readonly Memoizer<TArg, TResult>.ReaderWriterLockSlim slimLock;

    internal Memoizer(Func<TArg, TResult> function, IEqualityComparer<TArg> argComparer)
    {
      this.function = function;
      this.resultCache = new Dictionary<TArg, Memoizer<TArg, TResult>.Result>(argComparer);
      this.slimLock = new Memoizer<TArg, TResult>.ReaderWriterLockSlim();
    }

    internal TResult Evaluate(TArg arg)
    {
      this.slimLock.EnterReadLock();
      Memoizer<TArg, TResult>.Result result;
      bool flag;
      try
      {
        flag = this.resultCache.TryGetValue(arg, out result);
      }
      finally
      {
        this.slimLock.ExitReadLock();
      }
      if (!flag)
      {
        this.slimLock.EnterWriteLock();
        try
        {
          if (!this.resultCache.TryGetValue(arg, out result))
          {
            result = new Memoizer<TArg, TResult>.Result((Func<TResult>) (() => this.function(arg)));
            this.resultCache.Add(arg, result);
          }
        }
        finally
        {
          this.slimLock.ExitWriteLock();
        }
      }
      return result.GetValue();
    }

    private class Result
    {
      private TResult value;
      private Func<TResult> createValueDelegate;

      internal Result(Func<TResult> createValueDelegate) => this.createValueDelegate = createValueDelegate;

      internal TResult GetValue()
      {
        if (this.createValueDelegate == null)
          return this.value;
        lock (this)
        {
          if (this.createValueDelegate == null)
            return this.value;
          this.value = this.createValueDelegate();
          this.createValueDelegate = (Func<TResult>) null;
          return this.value;
        }
      }
    }

    private sealed class ReaderWriterLockSlim
    {
      private object readerWriterLock = new object();

      internal void EnterReadLock() => Monitor.Enter(this.readerWriterLock);

      internal void EnterWriteLock() => Monitor.Enter(this.readerWriterLock);

      internal void ExitReadLock() => Monitor.Exit(this.readerWriterLock);

      internal void ExitWriteLock() => Monitor.Exit(this.readerWriterLock);
    }
  }
}
