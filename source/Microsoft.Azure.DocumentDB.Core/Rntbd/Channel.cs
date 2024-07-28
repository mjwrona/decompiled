// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Channel
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class Channel : IChannel, IDisposable
  {
    private readonly Dispatcher dispatcher;
    private readonly TimerPool timerPool;
    private readonly int requestTimeoutSeconds;
    private readonly Uri serverUri;
    private bool disposed;
    private readonly ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private Channel.State state;
    private Task initializationTask;
    private ChannelOpenArguments openArguments;

    public Channel(Guid activityId, Uri serverUri, ChannelProperties channelProperties)
    {
      this.dispatcher = new Dispatcher(serverUri, channelProperties.UserAgent, channelProperties.ConnectionStateListener, channelProperties.CertificateHostNameOverride, channelProperties.ReceiveHangDetectionTime, channelProperties.SendHangDetectionTime, channelProperties.IdleTimerPool, channelProperties.IdleTimeout);
      this.timerPool = channelProperties.RequestTimerPool;
      this.requestTimeoutSeconds = (int) channelProperties.RequestTimeout.TotalSeconds;
      this.serverUri = serverUri;
      this.openArguments = new ChannelOpenArguments(activityId, new ChannelOpenTimeline(), (int) channelProperties.OpenTimeout.TotalSeconds, channelProperties.PortReuseMode, channelProperties.UserPortPool, channelProperties.CallerId);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Healthy
    {
      get
      {
        this.ThrowIfDisposed();
        Dispatcher dispatcher = (Dispatcher) null;
        this.stateLock.EnterReadLock();
        try
        {
          switch (this.state)
          {
            case Channel.State.New:
              return false;
            case Channel.State.Opening:
              return true;
            case Channel.State.Open:
              dispatcher = this.dispatcher;
              break;
            case Channel.State.Closed:
              return false;
            default:
              return false;
          }
        }
        finally
        {
          this.stateLock.ExitReadLock();
        }
        return dispatcher.Healthy;
      }
    }

    public void Initialize()
    {
      this.ThrowIfDisposed();
      this.stateLock.EnterWriteLock();
      try
      {
        this.state = Channel.State.Opening;
        this.initializationTask = Task.Run((Func<Task>) (async () =>
        {
          Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = this.openArguments.CommonArguments.ActivityId;
          await this.InitializeAsync();
        }));
      }
      finally
      {
        this.stateLock.ExitWriteLock();
      }
    }

    public async Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId)
    {
      Channel channel = this;
      channel.ThrowIfDisposed();
      Task task1 = (Task) null;
      channel.stateLock.EnterReadLock();
      try
      {
        if (channel.state != Channel.State.Open)
          task1 = channel.initializationTask;
      }
      finally
      {
        channel.stateLock.ExitReadLock();
      }
      if (task1 != null)
      {
        DefaultTrace.TraceInformation("Awaiting RNTBD channel initialization. Request URI: {0}", (object) physicalAddress);
        await task1;
      }
      ChannelCallArguments callArguments = new ChannelCallArguments(activityId);
      try
      {
        callArguments.PreparedCall = channel.dispatcher.PrepareCall(request, physicalAddress, resourceOperation, activityId);
      }
      catch (DocumentClientException ex)
      {
        ex.Headers.Add("x-ms-request-validation-failure", "1");
        throw;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Failed to serialize request. Assuming malformed request payload: {0}", (object) ex);
        BadRequestException requestException = new BadRequestException(ex);
        requestException.Headers.Add("x-ms-request-validation-failure", "1");
        throw requestException;
      }
      PooledTimer timer = channel.timerPool.GetPooledTimer(channel.requestTimeoutSeconds);
      Task[] tasks = new Task[2]
      {
        timer.StartTimerAsync(),
        null
      };
      Task<StoreResponse> dispatcherCall = channel.dispatcher.CallAsync(callArguments);
      TransportPerformanceCounters performanceCounters = TransportClient.GetTransportPerformanceCounters();
      int resourceType = (int) resourceOperation.resourceType;
      int operationType = (int) resourceOperation.operationType;
      int? length = callArguments.PreparedCall?.SerializedRequest?.Length;
      long? bytes = length.HasValue ? new long?((long) length.GetValueOrDefault()) : new long?();
      performanceCounters.LogRntbdBytesSentCount((ResourceType) resourceType, (OperationType) operationType, bytes);
      tasks[1] = (Task) dispatcherCall;
      Task task2 = await Task.WhenAny(tasks);
      if (task2 == tasks[0])
      {
        TransportErrorCode timeoutCode;
        bool payloadSent;
        callArguments.CommonArguments.SnapshotCallState(out timeoutCode, out payloadSent);
        channel.dispatcher.CancelCall(callArguments.PreparedCall);
        Channel.HandleTaskTimeout(tasks[1], activityId);
        Exception innerException = task2.Exception?.InnerException;
        DefaultTrace.TraceWarning("RNTBD call timed out on channel {0}. Error: {1}", (object) channel, (object) timeoutCode);
        throw new TransportException(timeoutCode, innerException, activityId, physicalAddress, channel.ToString(), callArguments.CommonArguments.UserPayload, payloadSent);
      }
      timer.CancelTimer();
      if (task2.IsFaulted)
        await task2;
      StoreResponse result = dispatcherCall.Result;
      TransportClient.GetTransportPerformanceCounters().LogRntbdBytesReceivedCount(resourceOperation.resourceType, resourceOperation.operationType, result?.ResponseBody?.Length);
      return result;
    }

    public override string ToString() => this.dispatcher.ToString();

    public void Close() => ((IDisposable) this).Dispose();

    void IDisposable.Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      DefaultTrace.TraceInformation("Disposing RNTBD Channel {0}", (object) this);
      Task task = (Task) null;
      this.stateLock.EnterWriteLock();
      try
      {
        if (this.state != Channel.State.Closed)
          task = this.initializationTask;
        this.state = Channel.State.Closed;
      }
      finally
      {
        this.stateLock.ExitWriteLock();
      }
      if (task != null)
      {
        try
        {
          task.Wait();
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceWarning("{0} initialization failed. Consuming the task exception in {1}. Server URI: {2}. Exception: {3}", (object) nameof (Channel), (object) "Dispose", (object) this.serverUri, (object) ex.Message);
        }
      }
      this.dispatcher.Dispose();
      this.stateLock.Dispose();
    }

    internal event Action TestOnInitializeComplete;

    internal event Action TestOnConnectionClosed
    {
      add => this.dispatcher.TestOnConnectionClosed += value;
      remove => this.dispatcher.TestOnConnectionClosed -= value;
    }

    internal bool TestIsIdle => this.dispatcher.TestIsIdle;

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (Channel));
    }

    private async Task InitializeAsync()
    {
      Channel channel = this;
      try
      {
        PooledTimer timer = channel.timerPool.GetPooledTimer(channel.openArguments.OpenTimeoutSeconds);
        Task[] tasks = new Task[2]
        {
          timer.StartTimerAsync(),
          channel.dispatcher.OpenAsync(channel.openArguments)
        };
        Task task = await Task.WhenAny(tasks);
        if (task == tasks[0])
        {
          TransportErrorCode timeoutCode;
          bool payloadSent;
          channel.openArguments.CommonArguments.SnapshotCallState(out timeoutCode, out payloadSent);
          Channel.HandleTaskTimeout(tasks[1], channel.openArguments.CommonArguments.ActivityId);
          Exception innerException = task.Exception?.InnerException;
          DefaultTrace.TraceWarning("RNTBD open timed out on channel {0}. Error: {1}", (object) channel, (object) timeoutCode);
          throw new TransportException(timeoutCode, innerException, channel.openArguments.CommonArguments.ActivityId, channel.serverUri, channel.ToString(), channel.openArguments.CommonArguments.UserPayload, payloadSent);
        }
        timer.CancelTimer();
        if (task.IsFaulted)
          await task;
        channel.FinishInitialization(Channel.State.Open);
        timer = (PooledTimer) null;
        tasks = (Task[]) null;
      }
      catch (DocumentClientException ex)
      {
        channel.FinishInitialization(Channel.State.Closed);
        ex.Headers.Set("x-ms-activity-id", channel.openArguments.CommonArguments.ActivityId.ToString());
        DefaultTrace.TraceWarning("Channel.InitializeAsync failed. Channel: {0}. DocumentClientException: {1}", (object) channel, (object) ex);
        throw;
      }
      catch (TransportException ex)
      {
        channel.FinishInitialization(Channel.State.Closed);
        DefaultTrace.TraceWarning("Channel.InitializeAsync failed. Channel: {0}. TransportException: {1}", (object) channel, (object) ex);
        throw;
      }
      catch (Exception ex)
      {
        channel.FinishInitialization(Channel.State.Closed);
        DefaultTrace.TraceWarning("Channel.InitializeAsync failed. Wrapping exception in TransportException. Channel: {0}. Inner exception: {1}", (object) channel, (object) ex);
        throw new TransportException(TransportErrorCode.ChannelOpenFailed, ex, channel.openArguments.CommonArguments.ActivityId, channel.serverUri, channel.ToString(), channel.openArguments.CommonArguments.UserPayload, channel.openArguments.CommonArguments.PayloadSent);
      }
      finally
      {
        channel.openArguments.OpenTimeline.WriteTrace();
        channel.openArguments = (ChannelOpenArguments) null;
      }
      Action initializeComplete = channel.TestOnInitializeComplete;
      if (initializeComplete == null)
        return;
      initializeComplete();
    }

    private void FinishInitialization(Channel.State nextState)
    {
      Task task = (Task) null;
      this.stateLock.EnterWriteLock();
      try
      {
        if (this.state != Channel.State.Closed)
        {
          this.state = nextState;
          task = this.initializationTask;
        }
      }
      finally
      {
        this.stateLock.ExitWriteLock();
      }
      if (nextState != Channel.State.Closed || task == null)
        return;
      task.ContinueWith((Action<Task>) (completedTask => DefaultTrace.TraceWarning("{0} initialization failed. Consuming the task exception asynchronously. Server URI: {1}. Exception: {2}", (object) nameof (Channel), (object) this.serverUri, (object) completedTask.Exception.InnerException?.Message)), TaskContinuationOptions.OnlyOnFaulted);
    }

    private static void HandleTaskTimeout(Task runawayTask, Guid activityId) => runawayTask.ContinueWith((Action<Task>) (task =>
    {
      Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = activityId;
      Exception innerException = task.Exception.InnerException;
      DefaultTrace.TraceInformation("Timed out task completed. Activity ID = {0}. HRESULT = {1:X}. Exception: {2}", (object) activityId, (object) innerException.HResult, (object) innerException);
    }), TaskContinuationOptions.OnlyOnFaulted);

    private enum State
    {
      New,
      Opening,
      Open,
      Closed,
    }
  }
}
