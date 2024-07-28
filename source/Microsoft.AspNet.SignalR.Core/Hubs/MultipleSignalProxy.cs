// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.MultipleSignalProxy
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class MultipleSignalProxy : DynamicObject, IClientProxy
  {
    private readonly IConnection _connection;
    private readonly IHubPipelineInvoker _invoker;
    private readonly IList<string> _exclude;
    private readonly IList<string> _signals;
    private readonly string _hubName;

    public MultipleSignalProxy(
      IConnection connection,
      IHubPipelineInvoker invoker,
      IList<string> signals,
      string hubName,
      string prefix,
      IList<string> exclude)
    {
      MultipleSignalProxy multipleSignalProxy = this;
      this._connection = connection;
      this._invoker = invoker;
      this._hubName = hubName;
      this._signals = (IList<string>) signals.Select<string, string>((Func<string, string>) (signal => prefix + multipleSignalProxy._hubName + "." + signal)).ToList<string>();
      this._exclude = exclude;
    }

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

    public Task Invoke(string method, params object[] args) => this._invoker.Send((IHubOutgoingInvokerContext) new HubOutgoingInvokerContext(this._connection, this._signals, this.GetInvocationData(method, args))
    {
      ExcludedSignals = this._exclude
    });

    protected virtual ClientHubInvocation GetInvocationData(string method, object[] args) => new ClientHubInvocation()
    {
      Hub = this._hubName,
      Method = method,
      Args = args
    };
  }
}
