// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.Disposer
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class Disposer : IDisposable
  {
    private static readonly object _disposedSentinel = new object();
    private object _disposable;

    public void Set(IDisposable disposable)
    {
      object obj = disposable != null ? Interlocked.CompareExchange(ref this._disposable, (object) disposable, (object) null) : throw new ArgumentNullException(nameof (disposable));
      if (obj == null || obj != Disposer._disposedSentinel)
        return;
      disposable.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || !(Interlocked.Exchange(ref this._disposable, Disposer._disposedSentinel) is IDisposable disposable))
        return;
      disposable.Dispose();
    }

    public void Dispose() => this.Dispose(true);
  }
}
