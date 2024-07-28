// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.TransportManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class TransportManager : ITransportManager
  {
    private readonly ConcurrentDictionary<string, Func<HostContext, ITransport>> _transports = new ConcurrentDictionary<string, Func<HostContext, ITransport>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public TransportManager(IDependencyResolver resolver)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      this.Register("foreverFrame", (Func<HostContext, ITransport>) (context => (ITransport) new ForeverFrameTransport(context, resolver)));
      this.Register("serverSentEvents", (Func<HostContext, ITransport>) (context => (ITransport) new ServerSentEventsTransport(context, resolver)));
      this.Register("longPolling", (Func<HostContext, ITransport>) (context => (ITransport) new LongPollingTransport(context, resolver)));
      this.Register("webSockets", (Func<HostContext, ITransport>) (context => (ITransport) new WebSocketTransport(context, resolver)));
    }

    public void Register(string transportName, Func<HostContext, ITransport> transportFactory)
    {
      if (string.IsNullOrEmpty(transportName))
        throw new ArgumentNullException(nameof (transportName));
      if (transportFactory == null)
        throw new ArgumentNullException(nameof (transportFactory));
      this._transports.TryAdd(transportName, transportFactory);
    }

    public void Remove(string transportName)
    {
      if (string.IsNullOrEmpty(transportName))
        throw new ArgumentNullException(nameof (transportName));
      this._transports.TryRemove(transportName, out Func<HostContext, ITransport> _);
    }

    public ITransport GetTransport(HostContext hostContext)
    {
      if (hostContext == null)
        throw new ArgumentNullException(nameof (hostContext));
      string key = hostContext.Request.QueryString["transport"];
      if (string.IsNullOrEmpty(key))
        return (ITransport) null;
      Func<HostContext, ITransport> func;
      return this._transports.TryGetValue(key, out func) ? func(hostContext) : (ITransport) null;
    }

    public bool SupportsTransport(string transportName) => !string.IsNullOrEmpty(transportName) && this._transports.ContainsKey(transportName);
  }
}
