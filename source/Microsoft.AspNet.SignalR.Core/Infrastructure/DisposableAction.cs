// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.DisposableAction
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class DisposableAction : IDisposable
  {
    public static readonly DisposableAction Empty = new DisposableAction((Action) (() => { }));
    private Action<object> _action;
    private readonly object _state;

    public DisposableAction(Action action)
      : this((Action<object>) (state => ((Action) state)()), (object) action)
    {
    }

    public DisposableAction(Action<object> action, object state)
    {
      this._action = action;
      this._state = state;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Interlocked.Exchange<Action<object>>(ref this._action, (Action<object>) (state => { }))(this._state);
    }

    public void Dispose() => this.Dispose(true);
  }
}
