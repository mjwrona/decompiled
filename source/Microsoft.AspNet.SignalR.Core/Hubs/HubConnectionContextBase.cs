// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubConnectionContextBase
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubConnectionContextBase : IHubConnectionContext<object>
  {
    public HubConnectionContextBase()
    {
    }

    public HubConnectionContextBase(
      IConnection connection,
      IHubPipelineInvoker invoker,
      string hubName)
    {
      this.Connection = connection;
      this.Invoker = invoker;
      this.HubName = hubName;
      this.All = this.AllExcept(new string[0]);
    }

    protected IHubPipelineInvoker Invoker { get; private set; }

    protected IConnection Connection { get; private set; }

    protected string HubName { get; private set; }

    public object All { get; set; }

    public object AllExcept(params string[] excludeConnectionIds) => (object) new ClientProxy(this.Connection, this.Invoker, this.HubName, PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds));

    public object Client(string connectionId)
    {
      if (string.IsNullOrEmpty(connectionId))
        throw new ArgumentException(Resources.Error_ArgumentNullOrEmpty, nameof (connectionId));
      return (object) new ConnectionIdProxy(this.Connection, this.Invoker, connectionId, this.HubName, new string[0]);
    }

    public object Clients(IList<string> connectionIds)
    {
      if (connectionIds == null)
        throw new ArgumentNullException(nameof (connectionIds));
      return (object) new MultipleSignalProxy(this.Connection, this.Invoker, connectionIds, this.HubName, "hc-", ListHelper<string>.Empty);
    }

    public object Group(string groupName, params string[] excludeConnectionIds)
    {
      if (string.IsNullOrEmpty(groupName))
        throw new ArgumentException(Resources.Error_ArgumentNullOrEmpty, nameof (groupName));
      return (object) new GroupProxy(this.Connection, this.Invoker, groupName, this.HubName, PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds));
    }

    public object Groups(IList<string> groupNames, params string[] excludeConnectionIds)
    {
      if (groupNames == null)
        throw new ArgumentNullException(nameof (groupNames));
      return (object) new MultipleSignalProxy(this.Connection, this.Invoker, groupNames, this.HubName, "hg-", PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds));
    }

    public object User(string userId)
    {
      if (userId == null)
        throw new ArgumentNullException(nameof (userId));
      return (object) new UserProxy(this.Connection, this.Invoker, userId, this.HubName);
    }

    public object Users(IList<string> userIds)
    {
      if (userIds == null)
        throw new ArgumentNullException(nameof (userIds));
      return (object) new MultipleSignalProxy(this.Connection, this.Invoker, userIds, this.HubName, "hu-", ListHelper<string>.Empty);
    }
  }
}
