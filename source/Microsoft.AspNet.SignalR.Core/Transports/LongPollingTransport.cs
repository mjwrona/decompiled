// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.LongPollingTransport
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Tracing;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public class LongPollingTransport : ForeverTransport, ITransport
  {
    private readonly IConfigurationManager _configurationManager;
    private readonly IPerformanceCounterManager _counters;
    private bool _responseSent;
    private static readonly ArraySegment<byte> _keepAlive = new ArraySegment<byte>(new byte[1]
    {
      (byte) 32
    });

    public LongPollingTransport(HostContext context, IDependencyResolver resolver)
      : this(context, resolver.Resolve<JsonSerializer>(), resolver.Resolve<ITransportHeartbeat>(), resolver.Resolve<IPerformanceCounterManager>(), resolver.Resolve<ITraceManager>(), resolver.Resolve<IConfigurationManager>(), resolver.Resolve<IMemoryPool>())
    {
    }

    public LongPollingTransport(
      HostContext context,
      JsonSerializer jsonSerializer,
      ITransportHeartbeat heartbeat,
      IPerformanceCounterManager performanceCounterManager,
      ITraceManager traceManager,
      IConfigurationManager configurationManager,
      IMemoryPool pool)
      : base(context, jsonSerializer, heartbeat, performanceCounterManager, traceManager, pool)
    {
      this._configurationManager = configurationManager;
      this._counters = performanceCounterManager;
    }

    public override TimeSpan DisconnectThreshold => this._configurationManager.LongPollDelay;

    private bool IsJsonp => !string.IsNullOrEmpty(this.JsonpCallback);

    private string JsonpCallback => this.Context.Request.QueryString["callback"];

    public override bool SupportsKeepAlive => !this.IsJsonp;

    public override bool RequiresTimeout => true;

    protected override int MaxMessages => 5000;

    protected override bool SuppressReconnect => !this.Context.Request.LocalPath.EndsWith("/reconnect", StringComparison.OrdinalIgnoreCase);

    protected override Task InitializeMessageId()
    {
      this._lastMessageId = this.Context.Request.QueryString["messageId"];
      return this._lastMessageId == null ? this.Context.Request.ReadForm().Then<INameValueCollection, LongPollingTransport>((Action<INameValueCollection, LongPollingTransport>) ((form, t) => t._lastMessageId = form["messageId"]), this) : TaskAsyncHelper.Empty;
    }

    public override Task<string> GetGroupsToken()
    {
      string result = this.Context.Request.QueryString["groupsToken"];
      return result == null ? this.Context.Request.ReadForm().Then<INameValueCollection, string>((Func<INameValueCollection, string>) (form => form["groupsToken"])) : Task.FromResult<string>(result);
    }

    public override Task KeepAlive() => this.EnqueueOperation((Func<object, Task>) (state => LongPollingTransport.PerformKeepAlive(state)), (object) this);

    public override Task Send(PersistentResponse response)
    {
      this.Heartbeat.MarkConnection((ITrackingConnection) this);
      this.AddTransportData(response);
      return this.EnqueueOperation((Func<object, Task>) (state => LongPollingTransport.PerformPartialSend(state)), (object) new LongPollingTransport.LongPollingTransportContext(this, (object) response));
    }

    public override Task Send(object value) => this.EnqueueOperation((Func<object, Task>) (state => LongPollingTransport.PerformCompleteSend(state)), (object) new LongPollingTransport.LongPollingTransportContext(this, value));

    public override void IncrementConnectionsCount() => this._counters.ConnectionsCurrentLongPolling.Increment();

    public override void DecrementConnectionsCount() => this._counters.ConnectionsCurrentLongPolling.Decrement();

    protected override Task<bool> OnMessageReceived(PersistentResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      response.Reconnect = this.HostShutdownToken.IsCancellationRequested;
      Task task = TaskAsyncHelper.Empty;
      if (response.Aborted)
        task = this.Abort();
      if (response.Terminal)
        return !this._responseSent ? task.Then<LongPollingTransport, PersistentResponse>((Func<LongPollingTransport, PersistentResponse, Task>) ((transport, resp) => transport.Send(resp)), this, response).Then<bool>((Func<Task<bool>>) (() =>
        {
          this._transportLifetime.Complete();
          return TaskAsyncHelper.False;
        })) : task.Then<bool>((Func<Task<bool>>) (() =>
        {
          this._transportLifetime.Complete();
          return TaskAsyncHelper.False;
        }));
      this._responseSent = true;
      return task.Then<LongPollingTransport, PersistentResponse>((Func<LongPollingTransport, PersistentResponse, Task>) ((transport, resp) => transport.Send(resp)), this, response).Then<bool>((Func<Task<bool>>) (() => TaskAsyncHelper.False));
    }

    protected internal override Task InitializeResponse(ITransportConnection connection) => base.InitializeResponse(connection).Then<LongPollingTransport>((Func<LongPollingTransport, Task>) (s => LongPollingTransport.WriteInit(s)), this);

    protected override async Task ProcessSendRequest()
    {
      LongPollingTransport pollingTransport = this;
      string str = (await pollingTransport.Context.Request.ReadForm().PreserveCulture<INameValueCollection>())["data"] ?? pollingTransport.Context.Request.QueryString["data"];
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (pollingTransport.Received) == null)
        return;
      // ISSUE: explicit non-virtual call
      await __nonvirtual (pollingTransport.Received)(str).PreserveCulture();
    }

    private static Task WriteInit(LongPollingTransport transport)
    {
      transport.Context.Response.ContentType = transport.IsJsonp ? JsonUtility.JavaScriptMimeType : JsonUtility.JsonMimeType;
      return transport.Context.Response.Flush();
    }

    private static Task PerformKeepAlive(object state)
    {
      LongPollingTransport pollingTransport = (LongPollingTransport) state;
      if (!pollingTransport.IsAlive)
        return TaskAsyncHelper.Empty;
      pollingTransport.Context.Response.Write(LongPollingTransport._keepAlive);
      return pollingTransport.Context.Response.Flush();
    }

    private static Task PerformPartialSend(object state)
    {
      LongPollingTransport.LongPollingTransportContext transportContext = (LongPollingTransport.LongPollingTransportContext) state;
      if (!transportContext.Transport.IsAlive)
        return TaskAsyncHelper.Empty;
      using (BinaryMemoryPoolTextWriter writer = new BinaryMemoryPoolTextWriter(transportContext.Transport.Pool))
      {
        if (transportContext.Transport.IsJsonp)
        {
          writer.Write(transportContext.Transport.JsonpCallback);
          writer.Write("(");
        }
        transportContext.Transport.JsonSerializer.Serialize(transportContext.State, (TextWriter) writer);
        if (transportContext.Transport.IsJsonp)
          writer.Write(");");
        writer.Flush();
        transportContext.Transport.Context.Response.Write(writer.Buffer);
      }
      return transportContext.Transport.Context.Response.Flush();
    }

    private static Task PerformCompleteSend(object state)
    {
      LongPollingTransport.LongPollingTransportContext transportContext = (LongPollingTransport.LongPollingTransportContext) state;
      if (!transportContext.Transport.IsAlive)
        return TaskAsyncHelper.Empty;
      transportContext.Transport.Context.Response.ContentType = transportContext.Transport.IsJsonp ? JsonUtility.JavaScriptMimeType : JsonUtility.JsonMimeType;
      return LongPollingTransport.PerformPartialSend(state);
    }

    private void AddTransportData(PersistentResponse response)
    {
      if (!(this._configurationManager.LongPollDelay != TimeSpan.Zero))
        return;
      response.LongPollDelay = new long?((long) this._configurationManager.LongPollDelay.TotalMilliseconds);
    }

    private class LongPollingTransportContext
    {
      public object State;
      public LongPollingTransport Transport;

      public LongPollingTransportContext(LongPollingTransport transport, object state)
      {
        this.State = state;
        this.Transport = transport;
      }
    }
  }
}
