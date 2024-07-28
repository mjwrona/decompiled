// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.ClientProxy
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class ClientProxy : DynamicObject, IClientProxy
  {
    private readonly IHubPipelineInvoker _invoker;
    private readonly IConnection _connection;
    private readonly string _hubName;
    private readonly string _signal;
    private readonly IList<string> _exclude;

    public ClientProxy(
      IConnection connection,
      IHubPipelineInvoker invoker,
      string hubName,
      IList<string> exclude)
    {
      this._connection = connection;
      this._invoker = invoker;
      this._hubName = hubName;
      this._exclude = exclude;
      this._signal = PrefixHelper.GetHubName(this._hubName);
    }

    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object[] args,
      out object result)
    {
      result = (object) this.Invoke(binder.Name, args);
      return true;
    }

    public Task Invoke(string method, params object[] args) => this._invoker.Send((IHubOutgoingInvokerContext) new HubOutgoingInvokerContext(this._connection, this._signal, new ClientHubInvocation()
    {
      Hub = this._hubName,
      Method = method,
      Args = args
    })
    {
      ExcludedSignals = this._exclude
    });
  }
}
