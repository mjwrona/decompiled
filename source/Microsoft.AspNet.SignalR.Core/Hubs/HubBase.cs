// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubBase
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public abstract class HubBase : IHub, IUntrackedDisposable, IDisposable
  {
    protected HubBase() => this.HubClients = (IHubCallerConnectionContext<object>) new HubConnectionContext();

    internal IHubCallerConnectionContext<object> HubClients { get; set; }

    IHubCallerConnectionContext<object> IHub.Clients
    {
      get => this.HubClients;
      set => this.HubClients = value;
    }

    public HubCallerContext Context { get; set; }

    public IGroupManager Groups { get; set; }

    public virtual Task OnDisconnected(bool stopCalled) => TaskAsyncHelper.Empty;

    public virtual Task OnConnected() => TaskAsyncHelper.Empty;

    public virtual Task OnReconnected() => TaskAsyncHelper.Empty;

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose() => this.Dispose(true);
  }
}
