// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.SafeCancellationTokenSource
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class SafeCancellationTokenSource : IDisposable
  {
    private CancellationTokenSource _cts;
    private int _state;

    public SafeCancellationTokenSource()
    {
      this._cts = new CancellationTokenSource();
      this.Token = this._cts.Token;
    }

    public CancellationToken Token { get; private set; }

    public void Cancel(bool useNewThread = true)
    {
      if (Interlocked.CompareExchange(ref this._state, 1, 0) != 0)
        return;
      if (!useNewThread)
        this.CancelCore();
      else
        ThreadPool.UnsafeQueueUserWorkItem((WaitCallback) (_ => this.CancelCore()), (object) null);
    }

    private void CancelCore()
    {
      try
      {
        this._cts.Cancel();
      }
      finally
      {
        if (Interlocked.CompareExchange(ref this._state, 2, 1) == 3)
        {
          this._cts.Dispose();
          Interlocked.Exchange(ref this._state, 4);
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      switch (Interlocked.Exchange(ref this._state, 3))
      {
        case 0:
        case 2:
          this._cts.Dispose();
          Interlocked.Exchange(ref this._state, 4);
          break;
        case 4:
          Interlocked.Exchange(ref this._state, 4);
          break;
      }
    }

    public void Dispose() => this.Dispose(true);

    private static class State
    {
      public const int Initial = 0;
      public const int Cancelling = 1;
      public const int Cancelled = 2;
      public const int Disposing = 3;
      public const int Disposed = 4;
    }
  }
}
