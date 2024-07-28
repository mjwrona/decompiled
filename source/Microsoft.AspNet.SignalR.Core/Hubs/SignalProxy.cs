// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.SignalProxy
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public abstract class SignalProxy : DynamicObject, IClientProxy
  {
    private readonly IList<string> _exclude;

    protected SignalProxy(
      IConnection connection,
      IHubPipelineInvoker invoker,
      string signal,
      string hubName,
      string prefix,
      IList<string> exclude)
    {
      this.Connection = connection;
      this.Invoker = invoker;
      this.HubName = hubName;
      this.Signal = prefix + hubName + "." + signal;
      this._exclude = exclude;
    }

    protected IConnection Connection { get; private set; }

    protected IHubPipelineInvoker Invoker { get; private set; }

    protected string Signal { get; private set; }

    protected string HubName { get; private set; }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = (object) null;
      return false;
    }

    public override bool TryInvokeMember(
      InvokeMemberBinder binder,
      object[] args,
      out object result)
    {
      result = (object) this.Invoke(binder.Name, args);
      return true;
    }

    public Task Invoke(string method, params object[] args) => this.Invoker.Send((IHubOutgoingInvokerContext) new HubOutgoingInvokerContext(this.Connection, this.Signal, this.GetInvocationData(method, args))
    {
      ExcludedSignals = this._exclude
    });

    protected virtual ClientHubInvocation GetInvocationData(string method, object[] args) => new ClientHubInvocation()
    {
      Hub = this.HubName,
      Method = method,
      Args = args
    };
  }
}
