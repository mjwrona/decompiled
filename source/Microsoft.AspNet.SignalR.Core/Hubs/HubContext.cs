// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubContext : IHubContext<object>, IHubContext
  {
    public HubContext(IConnection connection, IHubPipelineInvoker invoker, string hubName)
    {
      this.Clients = (IHubConnectionContext<object>) new HubConnectionContextBase(connection, invoker, hubName);
      this.Groups = (IGroupManager) new GroupManager(connection, PrefixHelper.GetHubGroupName(hubName));
    }

    public IHubConnectionContext<object> Clients { get; private set; }

    public IGroupManager Groups { get; private set; }
  }
}
