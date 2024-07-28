// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.ConnectionManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Tracing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class ConnectionManager : IConnectionManager
  {
    private readonly IDependencyResolver _resolver;
    private readonly IPerformanceCounterManager _counters;

    public ConnectionManager(IDependencyResolver resolver)
    {
      this._resolver = resolver;
      this._counters = this._resolver.Resolve<IPerformanceCounterManager>();
    }

    public IPersistentConnectionContext GetConnectionContext<T>() where T : PersistentConnection => this.GetConnection(typeof (T));

    public IPersistentConnectionContext GetConnection(Type type)
    {
      string str = !(type == (Type) null) ? type.FullName : throw new ArgumentNullException(nameof (type));
      Connection connectionCore = this.GetConnectionCore(PrefixHelper.GetPersistentConnectionName(str));
      return (IPersistentConnectionContext) new PersistentConnectionContext((IConnection) connectionCore, (IConnectionGroupManager) new GroupManager((IConnection) connectionCore, PrefixHelper.GetPersistentConnectionGroupName(str)));
    }

    public IHubContext GetHubContext<T>() where T : IHub => this.GetHubContext(typeof (T).GetHubName());

    public IHubContext GetHubContext(string hubName)
    {
      Connection connectionCore = this.GetConnectionCore((string) null);
      IHubManager hubManager = this._resolver.Resolve<IHubManager>();
      IHubPipelineInvoker invoker = this._resolver.Resolve<IHubPipelineInvoker>();
      string hubName1 = hubName;
      IPerformanceCounter[] performanceCounterArray = new IPerformanceCounter[4]
      {
        this._counters.ErrorsHubResolutionTotal,
        this._counters.ErrorsHubResolutionPerSec,
        this._counters.ErrorsAllTotal,
        this._counters.ErrorsAllPerSec
      };
      hubManager.EnsureHub(hubName1, performanceCounterArray);
      return (IHubContext) new HubContext((IConnection) connectionCore, invoker, hubName);
    }

    public IHubContext<TClient> GetHubContext<T, TClient>()
      where T : IHub
      where TClient : class
    {
      return this.GetHubContext<TClient>(typeof (T).GetHubName());
    }

    public IHubContext<TClient> GetHubContext<TClient>(string hubName) where TClient : class => (IHubContext<TClient>) new HubContext<TClient>(this.GetHubContext(hubName));

    internal Connection GetConnectionCore(string connectionName)
    {
      IList<string> stringList;
      if (connectionName != null)
        stringList = (IList<string>) new string[1]
        {
          connectionName
        };
      else
        stringList = ListHelper<string>.Empty;
      IList<string> signals = stringList;
      this._resolver.Resolve<AckSubscriber>();
      string connectionId = Guid.NewGuid().ToString();
      return new Connection(this._resolver.Resolve<IMessageBus>(), this._resolver.Resolve<JsonSerializer>(), connectionName, connectionId, signals, ListHelper<string>.Empty, this._resolver.Resolve<ITraceManager>(), this._resolver.Resolve<IAckHandler>(), this._resolver.Resolve<IPerformanceCounterManager>(), this._resolver.Resolve<IProtectedData>(), this._resolver.Resolve<IMemoryPool>());
    }
  }
}
