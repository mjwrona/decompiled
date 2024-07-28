// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hub`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR
{
  public abstract class Hub<T> : Hub where T : class
  {
    private IHubCallerConnectionContext<T> _testClients;

    public IHubCallerConnectionContext<T> Clients
    {
      get => this._testClients ?? (IHubCallerConnectionContext<T>) new TypedHubCallerConnectionContext<T>(base.Clients);
      set => this._testClients = value;
    }
  }
}
