// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubConnectionContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubConnectionContext : 
    HubConnectionContextBase,
    IHubCallerConnectionContext<object>,
    IHubConnectionContext<object>
  {
    private readonly string _connectionId;

    public HubConnectionContext()
    {
      this.All = (object) new NullClientProxy();
      this.Others = (object) new NullClientProxy();
      this.Caller = (object) new NullClientProxy();
    }

    public HubConnectionContext(
      IHubPipelineInvoker pipelineInvoker,
      IConnection connection,
      string hubName,
      string connectionId,
      StateChangeTracker tracker)
      : base(connection, pipelineInvoker, hubName)
    {
      this._connectionId = connectionId;
      this.Caller = (object) new StatefulSignalProxy(connection, pipelineInvoker, connectionId, "hc-", hubName, tracker);
      this.CallerState = (object) new CallerStateProxy(tracker);
      this.All = this.AllExcept(new string[0]);
      this.Others = this.AllExcept(new string[1]
      {
        connectionId
      });
    }

    public object Others { get; set; }

    public object Caller { get; set; }

    public object CallerState { get; set; }

    public object OthersInGroup(string groupName) => this.Group(groupName, new string[1]
    {
      this._connectionId
    });

    public object OthersInGroups(IList<string> groupNames) => this.Groups(groupNames, new string[1]
    {
      this._connectionId
    });
  }
}
