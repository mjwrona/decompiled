// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.IConnectionManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public interface IConnectionManager
  {
    IHubContext GetHubContext<T>() where T : IHub;

    IHubContext GetHubContext(string hubName);

    IHubContext<TClient> GetHubContext<T, TClient>()
      where T : IHub
      where TClient : class;

    IHubContext<TClient> GetHubContext<TClient>(string hubName) where TClient : class;

    IPersistentConnectionContext GetConnectionContext<T>() where T : PersistentConnection;
  }
}
