// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.RefCount`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal sealed class RefCount<T>
  {
    private readonly WeakReference<RefCount<T>.Count> m_count;

    public RefCount(T target)
    {
      this.DangerousTarget = target;
      this.m_count = new WeakReference<RefCount<T>.Count>((RefCount<T>.Count) null);
    }

    public RefCount<T>.Handle AcquireHandle()
    {
      RefCount<T>.Count target;
      if (!this.m_count.TryGetTarget(out target))
      {
        target = new RefCount<T>.Count();
        this.m_count.SetTarget(target);
      }
      return new RefCount<T>.Handle(this.DangerousTarget, target);
    }

    public bool IsAlive
    {
      get
      {
        RefCount<T>.Count target;
        return this.m_count.TryGetTarget(out target) && target.Value != 0;
      }
    }

    public T DangerousTarget { get; }

    internal class Count
    {
      internal int Value;
    }

    internal sealed class Handle : IDisposable
    {
      private readonly T m_target;
      private RefCount<T>.Count m_count;

      internal Handle(T target, RefCount<T>.Count count)
      {
        this.m_target = target;
        this.m_count = count;
        Interlocked.Increment(ref this.m_count.Value);
      }

      public void Dispose()
      {
        if (this.m_count == null)
          return;
        Interlocked.Decrement(ref this.m_count.Value);
        this.m_count = (RefCount<T>.Count) null;
      }

      public T Target
      {
        get
        {
          if (this.m_count == null)
            throw new ObjectDisposedException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) nameof (RefCount<T>), (object) nameof (Handle<>))));
          return this.m_target;
        }
      }
    }
  }
}
