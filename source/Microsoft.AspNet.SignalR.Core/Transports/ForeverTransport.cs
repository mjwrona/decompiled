// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.ForeverTransport
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Tracing;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public abstract class ForeverTransport : TransportDisconnectBase, ITransport
  {
    private static readonly ProtocolResolver _protocolResolver = new ProtocolResolver();
    private readonly IPerformanceCounterManager _counters;
    private readonly JsonSerializer _jsonSerializer;
    private IDisposable _busRegistration;
    internal ForeverTransport.RequestLifetime _transportLifetime;
    internal Action AfterReceive;
    internal Action BeforeCancellationTokenCallbackRegistered;
    internal Action BeforeReceive;
    internal Action<Exception> AfterRequestEnd;

    protected ForeverTransport(HostContext context, IDependencyResolver resolver)
      : this(context, resolver.Resolve<JsonSerializer>(), resolver.Resolve<ITransportHeartbeat>(), resolver.Resolve<IPerformanceCounterManager>(), resolver.Resolve<ITraceManager>(), resolver.Resolve<IMemoryPool>())
    {
    }

    protected ForeverTransport(
      HostContext context,
      JsonSerializer jsonSerializer,
      ITransportHeartbeat heartbeat,
      IPerformanceCounterManager performanceCounterManager,
      ITraceManager traceManager,
      IMemoryPool pool)
      : base(context, heartbeat, performanceCounterManager, traceManager)
    {
      this.Pool = pool;
      this._jsonSerializer = jsonSerializer;
      this._counters = performanceCounterManager;
    }

    protected IMemoryPool Pool { get; private set; }

    protected virtual int MaxMessages => 10;

    protected JsonSerializer JsonSerializer => this._jsonSerializer;

    protected virtual void OnSending(string payload) => this.Heartbeat.MarkConnection((ITrackingConnection) this);

    protected virtual void OnSendingResponse(PersistentResponse response) => this.Heartbeat.MarkConnection((ITrackingConnection) this);

    public Func<string, Task> Received { get; set; }

    public Func<Task> Connected { get; set; }

    public Func<Task> Reconnected { get; set; }

    protected override Task InitializePersistentState() => base.InitializePersistentState().Then<ForeverTransport>((Action<ForeverTransport>) (t => t._transportLifetime = new ForeverTransport.RequestLifetime(t, t._requestLifeTime)), this);

    protected Task ProcessRequestCore(ITransportConnection connection)
    {
      this.Connection = connection;
      if (this.IsSendRequest)
        return this.ProcessSendRequest();
      if (!this.IsAbortRequest)
        return this.InitializePersistentState().Then<ForeverTransport, ITransportConnection>((Func<ForeverTransport, ITransportConnection, Task>) ((t, c) => t.ProcessReceiveRequest(c)), this, connection);
      this.Context.Response.ContentType = "text/plain";
      return this.Connection.Abort(this.ConnectionId);
    }

    public virtual Task ProcessRequest(ITransportConnection connection) => this.ProcessRequestCore(connection);

    public abstract Task Send(PersistentResponse response);

    public virtual Task Send(object value) => this.EnqueueOperation((Func<object, Task>) (state => ForeverTransport.PerformSend(state)), (object) new ForeverTransport.ForeverTransportContext(this, value));

    protected internal virtual Task InitializeResponse(ITransportConnection connection) => TaskAsyncHelper.Empty;

    protected void OnError(Exception ex)
    {
      this.IncrementErrors();
      this._transportLifetime.Complete(ex);
    }

    protected virtual async Task ProcessSendRequest()
    {
      ForeverTransport foreverTransport = this;
      string str = (await foreverTransport.Context.Request.ReadForm().PreserveCulture<INameValueCollection>())["data"];
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (foreverTransport.Received) == null)
        return;
      // ISSUE: explicit non-virtual call
      await __nonvirtual (foreverTransport.Received)(str).PreserveCulture();
    }

    private Task ProcessReceiveRequest(ITransportConnection connection)
    {
      Func<Task> initialize = (Func<Task>) null;
      ITrackingConnection oldConnection = this.Heartbeat.AddOrUpdateConnection((ITrackingConnection) this);
      bool flag = oldConnection == null;
      if (this.IsConnectRequest)
      {
        if (ForeverTransport._protocolResolver.SupportsDelayedStart(this.Context.Request))
        {
          initialize = (Func<Task>) (() => connection.Initialize(this.ConnectionId));
        }
        else
        {
          Func<Task> connected;
          if (flag)
          {
            connected = this.Connected ?? TransportDisconnectBase._emptyTaskFunc;
            this._counters.ConnectionsConnected.Increment();
          }
          else
            connected = (Func<Task>) (() => oldConnection.ConnectTask);
          initialize = (Func<Task>) (() => connected().Then<ITransportConnection, string>((Func<ITransportConnection, string, Task>) ((conn, id) => conn.Initialize(id)), connection, this.ConnectionId));
        }
      }
      else if (!this.SuppressReconnect)
      {
        initialize = this.Reconnected;
        this._counters.ConnectionsReconnected.Increment();
      }
      initialize = initialize ?? TransportDisconnectBase._emptyTaskFunc;
      Func<Task> initialize1 = (Func<Task>) (() => initialize().ContinueWith(this._connectTcs));
      return this.ProcessMessages(connection, initialize1);
    }

    private Task ProcessMessages(ITransportConnection connection, Func<Task> initialize)
    {
      Disposer disposer = new Disposer();
      if (this.BeforeCancellationTokenCallbackRegistered != null)
        this.BeforeCancellationTokenCallbackRegistered();
      this._busRegistration = this.ConnectionEndToken.SafeRegister((Action<object>) (state => ForeverTransport.Cancel(state)), (object) new ForeverTransport.ForeverTransportContext(this, (object) disposer));
      if (this.BeforeReceive != null)
        this.BeforeReceive();
      try
      {
        this.EnqueueOperation((Func<object, Task>) (state => this.InitializeResponse((ITransportConnection) state)), (object) connection).Catch<Task>((Action<AggregateException, object>) ((ex, state) => ((ForeverTransport) state).OnError((Exception) ex)), (object) this, this.Trace);
        IDisposable subscription = connection.Receive(this.LastMessageId, (Func<PersistentResponse, object, Task<bool>>) ((response, state) => ((ForeverTransport) state).OnMessageReceived(response)), this.MaxMessages, (object) this);
        if (this.AfterReceive != null)
          this.AfterReceive();
        initialize().Catch<Task>((Action<AggregateException, object>) ((ex, state) => ((ForeverTransport) state).OnError((Exception) ex)), (object) this, this.Trace).Finally((Action<object>) (state => ((ForeverTransport.SubscriptionDisposerContext) state).Set()), (object) new ForeverTransport.SubscriptionDisposerContext(disposer, subscription));
      }
      catch (Exception ex)
      {
        this._transportLifetime.Complete(ex);
      }
      return this._requestLifeTime.Task;
    }

    private static void Cancel(object state)
    {
      ForeverTransport.ForeverTransportContext transportContext = (ForeverTransport.ForeverTransportContext) state;
      transportContext.Transport.Trace.TraceEvent(TraceEventType.Verbose, 0, "Cancel(" + transportContext.Transport.ConnectionId + ")");
      ((IDisposable) transportContext.State).Dispose();
    }

    protected virtual Task<bool> OnMessageReceived(PersistentResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      response.Reconnect = this.HostShutdownToken.IsCancellationRequested;
      if (this.IsTimedOut || response.Aborted)
      {
        this._busRegistration.Dispose();
        if (response.Aborted)
          return this.Abort().Then<bool>((Func<Task<bool>>) (() => TaskAsyncHelper.False));
      }
      if (!response.Terminal)
        return this.Send(response).Then<bool>((Func<Task<bool>>) (() => TaskAsyncHelper.True));
      this._transportLifetime.Complete();
      return TaskAsyncHelper.False;
    }

    internal virtual MemoryPoolTextWriter CreateMemoryPoolWriter(IMemoryPool memoryPool) => (MemoryPoolTextWriter) new BinaryMemoryPoolTextWriter(memoryPool);

    private static Task PerformSend(object state)
    {
      ForeverTransport.ForeverTransportContext transportContext = (ForeverTransport.ForeverTransportContext) state;
      if (!transportContext.Transport.IsAlive)
        return TaskAsyncHelper.Empty;
      transportContext.Transport.Context.Response.ContentType = JsonUtility.JsonMimeType;
      using (MemoryPoolTextWriter memoryPoolWriter = transportContext.Transport.CreateMemoryPoolWriter(transportContext.Transport.Pool))
      {
        transportContext.Transport.JsonSerializer.Serialize(transportContext.State, (TextWriter) memoryPoolWriter);
        memoryPoolWriter.Flush();
        transportContext.Transport.Context.Response.Write(memoryPoolWriter.Buffer);
        transportContext.Transport.TraceOutgoingMessage(memoryPoolWriter.Buffer);
      }
      return TaskAsyncHelper.Empty;
    }

    internal void TraceOutgoingMessage(ArraySegment<byte> message)
    {
      if (!this.Trace.Switch.ShouldTrace(TraceEventType.Verbose))
        return;
      string str = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
      if (str == "{}")
        return;
      this.Trace.TraceVerbose("Sending outgoing message. Connection id: {0}, transport: {1}, message: {2}", (object) this.ConnectionId, (object) this.GetType().Name, (object) str);
    }

    private class ForeverTransportContext
    {
      public readonly object State;
      public readonly ForeverTransport Transport;

      public ForeverTransportContext(ForeverTransport foreverTransport, object state)
      {
        this.State = state;
        this.Transport = foreverTransport;
      }
    }

    private class SubscriptionDisposerContext
    {
      private readonly Disposer _disposer;
      private readonly IDisposable _supscription;

      public SubscriptionDisposerContext(Disposer disposer, IDisposable subscription)
      {
        this._disposer = disposer;
        this._supscription = subscription;
      }

      public void Set() => this._disposer.Set(this._supscription);
    }

    internal class RequestLifetime
    {
      private readonly HttpRequestLifeTime _lifetime;
      private readonly ForeverTransport _transport;

      public RequestLifetime(ForeverTransport transport, HttpRequestLifeTime lifetime)
      {
        this._lifetime = lifetime;
        this._transport = transport;
      }

      public void Complete() => this.Complete((Exception) null);

      public void Complete(Exception error)
      {
        this._lifetime.Complete(error);
        this._transport.Dispose();
        if (this._transport.AfterRequestEnd == null)
          return;
        this._transport.AfterRequestEnd(error);
      }
    }
  }
}
