// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.StatefulSignalProxy
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System.Dynamic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class StatefulSignalProxy : SignalProxy
  {
    private readonly StateChangeTracker _tracker;

    public StatefulSignalProxy(
      IConnection connection,
      IHubPipelineInvoker invoker,
      string signal,
      string hubName,
      string prefix,
      StateChangeTracker tracker)
      : base(connection, invoker, signal, prefix, hubName, ListHelper<string>.Empty)
    {
      this._tracker = tracker;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      this._tracker[binder.Name] = value;
      return true;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = this._tracker[binder.Name];
      return true;
    }

    protected override ClientHubInvocation GetInvocationData(string method, object[] args) => new ClientHubInvocation()
    {
      Hub = this.HubName,
      Method = method,
      Args = args,
      State = this._tracker.GetChanges()
    };
  }
}
