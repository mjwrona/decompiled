// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.TransportDisconnectBase
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public abstract class TransportDisconnectBase : ITrackingConnection, IDisposable
  {
    private readonly HostContext _context;
    private readonly ITransportHeartbeat _heartbeat;
    private TraceSource _trace;
    private int _timedOut;
    private readonly IPerformanceCounterManager _counters;
    private int _ended;
    private TransportConnectionStates _state;
    protected string _lastMessageId;
    internal static readonly Func<Task> _emptyTaskFunc = (Func<Task>) (() => TaskAsyncHelper.Empty);
    internal DispatchingTaskCompletionSource<object> _connectTcs;
    private CancellationToken _connectionEndToken;
    private SafeCancellationTokenSource _connectionEndTokenSource;
    private Task _lastWriteTask = TaskAsyncHelper.Empty;
    private CancellationToken _hostShutdownToken;
    private IDisposable _hostRegistration;
    private IDisposable _connectionEndRegistration;
    internal HttpRequestLifeTime _requestLifeTime;

    protected TransportDisconnectBase(
      HostContext context,
      ITransportHeartbeat heartbeat,
      IPerformanceCounterManager performanceCounterManager,
      ITraceManager traceManager)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (heartbeat == null)
        throw new ArgumentNullException(nameof (heartbeat));
      if (performanceCounterManager == null)
        throw new ArgumentNullException(nameof (performanceCounterManager));
      if (traceManager == null)
        throw new ArgumentNullException(nameof (traceManager));
      this._context = context;
      this._heartbeat = heartbeat;
      this._counters = performanceCounterManager;
      this.WriteQueue = new TaskQueue();
      this._trace = traceManager["SignalR.Transports." + this.GetType().Name];
    }

    protected TraceSource Trace => this._trace;

    public string ConnectionId { get; set; }

    protected string LastMessageId => this._lastMessageId;

    protected virtual Task InitializeMessageId()
    {
      this._lastMessageId = this.Context.Request.QueryString["messageId"];
      return TaskAsyncHelper.Empty;
    }

    public virtual Task<string> GetGroupsToken() => TaskAsyncHelper.FromResult<string>(this.Context.Request.QueryString["groupsToken"]);

    internal TaskQueue WriteQueue { get; set; }

    public Func<bool, Task> Disconnected { get; set; }

    public virtual CancellationToken CancellationToken => this._context.Response.CancellationToken;

    public virtual bool IsAlive => !this.CancellationToken.IsCancellationRequested && (this._requestLifeTime == null || !this._requestLifeTime.Task.IsCompleted) && !this._lastWriteTask.IsCanceled && !this._lastWriteTask.IsFaulted;

    public Task ConnectTask => (Task) this._connectTcs.Task;

    protected CancellationToken ConnectionEndToken => this._connectionEndToken;

    protected CancellationToken HostShutdownToken => this._hostShutdownToken;

    public bool IsTimedOut => this._timedOut == 1;

    public virtual bool SupportsKeepAlive => true;

    public virtual bool RequiresTimeout => false;

    public virtual TimeSpan DisconnectThreshold => TimeSpan.FromSeconds(5.0);

    protected bool IsConnectRequest => this.Context.Request.LocalPath.EndsWith("/connect", StringComparison.OrdinalIgnoreCase);

    protected bool IsSendRequest => this.Context.Request.LocalPath.EndsWith("/send", StringComparison.OrdinalIgnoreCase);

    protected bool IsAbortRequest => this.Context.Request.LocalPath.EndsWith("/abort", StringComparison.OrdinalIgnoreCase);

    protected virtual bool SuppressReconnect => false;

    protected ITransportConnection Connection { get; set; }

    protected HostContext Context => this._context;

    protected ITransportHeartbeat Heartbeat => this._heartbeat;

    public Uri Url => this._context.Request.Url;

    protected void IncrementErrors()
    {
      this._counters.ErrorsTransportTotal.Increment();
      this._counters.ErrorsTransportPerSec.Increment();
      this._counters.ErrorsAllTotal.Increment();
      this._counters.ErrorsAllPerSec.Increment();
    }

    public abstract void IncrementConnectionsCount();

    public abstract void DecrementConnectionsCount();

    public Task Disconnect() => this.Abort(false);

    protected Task Abort() => this.Abort(true);

    private Task Abort(bool clean)
    {
      if (clean)
        this.ApplyState(TransportConnectionStates.Aborted);
      else
        this.ApplyState(TransportConnectionStates.Disconnected);
      this.Trace.TraceInformation("Abort(" + this.ConnectionId + ")");
      this.Heartbeat.RemoveConnection((ITrackingConnection) this);
      this.End();
      return (this.Disconnected != null ? this.Disconnected(clean) : TaskAsyncHelper.Empty).Catch<Task>((Action<AggregateException, object>) ((ex, state) => TransportDisconnectBase.OnDisconnectError(ex, state)), (object) this.Trace, this.Trace).Finally((Action<object>) (state => ((IPerformanceCounterManager) state).ConnectionsDisconnected.Increment()), (object) this._counters);
    }

    public void ApplyState(TransportConnectionStates states) => this._state |= states;

    public void Timeout()
    {
      if (Interlocked.Exchange(ref this._timedOut, 1) != 0)
        return;
      this.Trace.TraceInformation("Timeout(" + this.ConnectionId + ")");
      this.End();
    }

    public virtual Task KeepAlive() => TaskAsyncHelper.Empty;

    public void End()
    {
      if (Interlocked.Exchange(ref this._ended, 1) != 0)
        return;
      this.Trace.TraceInformation("End(" + this.ConnectionId + ")");
      if (this._connectionEndTokenSource == null)
        return;
      this._connectionEndTokenSource.Cancel();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.End();
      this._connectionEndTokenSource.Dispose();
      this._connectionEndRegistration.Dispose();
      this._hostRegistration.Dispose();
      this.ApplyState(TransportConnectionStates.Disposed);
    }

    protected internal Task EnqueueOperation(Func<Task> writeAsync) => this.EnqueueOperation((Func<object, Task>) (state => ((Func<Task>) state)()), (object) writeAsync);

    protected internal virtual Task EnqueueOperation(Func<object, Task> writeAsync, object state)
    {
      if (!this.IsAlive)
        return TaskAsyncHelper.Empty;
      Task task = this.WriteQueue.Enqueue(writeAsync, state);
      this._lastWriteTask = task;
      return task;
    }

    protected virtual Task InitializePersistentState()
    {
      this._hostShutdownToken = this._context.Environment.GetShutdownToken();
      this._requestLifeTime = new HttpRequestLifeTime(this, this.WriteQueue, this.Trace, this.ConnectionId);
      this._connectTcs = new DispatchingTaskCompletionSource<object>();
      this._connectionEndTokenSource = new SafeCancellationTokenSource();
      this._connectionEndToken = this._connectionEndTokenSource.Token;
      this._hostRegistration = this._hostShutdownToken.SafeRegister((Action<object>) (state => ((SafeCancellationTokenSource) state).Cancel()), (object) this._connectionEndTokenSource);
      this._connectionEndRegistration = this.CancellationToken.SafeRegister((Action<object>) (state => ((HttpRequestLifeTime) state).Complete()), (object) this._requestLifeTime);
      return this.InitializeMessageId();
    }

    private static void OnDisconnectError(AggregateException ex, object state) => ((TraceSource) state).TraceEvent(TraceEventType.Error, 0, "Failed to raise disconnect: " + (object) ex.GetBaseException());
  }
}
