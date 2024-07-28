// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Channel
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
    private readonly bool localRegionRequest;
    private bool disposed;
    private readonly ReaderWriterLockSlim stateLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private Channel.State state;
    private Task initializationTask;
    private volatile bool isInitializationComplete;
    private ChannelOpenArguments openArguments;
    private readonly SemaphoreSlim openingSlim;

    public Channel(
      Guid activityId,
      Uri serverUri,
      ChannelProperties channelProperties,
      bool localRegionRequest,
      SemaphoreSlim openingSlim)
    {
      this.dispatcher = new Dispatcher(serverUri, channelProperties.UserAgent, channelProperties.ConnectionStateListener, channelProperties.CertificateHostNameOverride, channelProperties.ReceiveHangDetectionTime, channelProperties.SendHangDetectionTime, channelProperties.IdleTimerPool, channelProperties.IdleTimeout, channelProperties.EnableChannelMultiplexing, channelProperties.MemoryStreamPool, channelProperties.RemoteCertificateValidationCallback);
      this.timerPool = channelProperties.RequestTimerPool;
      this.requestTimeoutSeconds = (int) channelProperties.RequestTimeout.TotalSeconds;
      this.serverUri = serverUri;
      this.localRegionRequest = localRegionRequest;
      TimeSpan openTimeout = localRegionRequest ? channelProperties.LocalRegionOpenTimeout : channelProperties.OpenTimeout;
      this.openArguments = new ChannelOpenArguments(activityId, new ChannelOpenTimeline(), openTimeout, channelProperties.PortReuseMode, channelProperties.UserPortPool, channelProperties.CallerId);
      this.openingSlim = openingSlim;
      this.Initialize();
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
            case Channel.State.WaitingToOpen:
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

    private void Initialize()
    {
      this.ThrowIfDisposed();
      this.stateLock.EnterWriteLock();
      try
      {
        this.state = Channel.State.WaitingToOpen;
        this.initializationTask = Task.Run((Func<Task>) (async () =>
        {
          System.Diagnostics.Trace.CorrelationManager.ActivityId = this.openArguments.CommonArguments.ActivityId;
          await this.InitializeAsync();
          this.isInitializationComplete = true;
          Action initializeComplete = this.TestOnInitializeComplete;
          if (initializeComplete == null)
            return;
          initializeComplete();
        }));
      }
      finally
      {
        this.stateLock.ExitWriteLock();
      }
    }

    public async Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      TransportAddressUri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId,
      TransportRequestStats transportRequestStats)
    {
      Channel channel = this;
      channel.ThrowIfDisposed();
      if (!channel.isInitializationComplete)
      {
        transportRequestStats.RequestWaitingForConnectionInitialization = new bool?(true);
        DefaultTrace.TraceInformation("Awaiting RNTBD channel initialization. Request URI: {0}", (object) physicalAddress);
        await channel.initializationTask;
      }
      else
        transportRequestStats.RequestWaitingForConnectionInitialization = new bool?(false);
      transportRequestStats.RecordState(TransportRequestStats.RequestStage.Pipelined);
      PooledTimer timer;
      Task[] tasks;
      Task<StoreResponse> dispatcherCall;
      StoreResponse storeResponse;
      using (ChannelCallArguments callArguments = new ChannelCallArguments(activityId))
      {
        try
        {
          callArguments.PreparedCall = channel.dispatcher.PrepareCall(request, physicalAddress, resourceOperation, activityId, transportRequestStats);
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
        timer = channel.timerPool.GetPooledTimer(channel.requestTimeoutSeconds);
        tasks = new Task[2]{ timer.StartTimerAsync(), null };
        dispatcherCall = channel.dispatcher.CallAsync(callArguments, transportRequestStats);
        TransportPerformanceCounters performanceCounters = TransportClient.GetTransportPerformanceCounters();
        int resourceType = (int) resourceOperation.resourceType;
        int operationType = (int) resourceOperation.operationType;
        int? requestSize = callArguments.PreparedCall?.SerializedRequest.RequestSize;
        long? bytes = requestSize.HasValue ? new long?((long) requestSize.GetValueOrDefault()) : new long?();
        performanceCounters.LogRntbdBytesSentCount((ResourceType) resourceType, (OperationType) operationType, bytes);
        tasks[1] = (Task) dispatcherCall;
        Task task = await Task.WhenAny(tasks);
        if (task == tasks[0])
        {
          TransportErrorCode timeoutCode;
          bool payloadSent;
          callArguments.CommonArguments.SnapshotCallState(out timeoutCode, out payloadSent);
          channel.dispatcher.CancelCall(callArguments.PreparedCall);
          Channel.HandleTaskTimeout(tasks[1], activityId);
          Exception innerException = task.Exception?.InnerException;
          DefaultTrace.TraceWarning("RNTBD call timed out on channel {0}. Error: {1}", (object) channel, (object) timeoutCode);
          throw new TransportException(timeoutCode, innerException, activityId, physicalAddress.Uri, channel.ToString(), callArguments.CommonArguments.UserPayload, payloadSent);
        }
        timer.CancelTimer();
        if (task.IsFaulted)
          await task;
        StoreResponse result = dispatcherCall.Result;
        TransportClient.GetTransportPerformanceCounters().LogRntbdBytesReceivedCount(resourceOperation.resourceType, resourceOperation.operationType, result?.ResponseBody?.Length);
        storeResponse = result;
      }
      timer = (PooledTimer) null;
      tasks = (Task[]) null;
      dispatcherCall = (Task<StoreResponse>) null;
      return storeResponse;
    }

    public Task OpenChannelAsync(Guid activityId) => this.initializationTask != null ? this.initializationTask : throw new InvalidOperationException("Channal Initialization Task Can't be null.");

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
      bool slimAcquired = false;
      try
      {
        channel.openArguments.CommonArguments.SetTimeoutCode(TransportErrorCode.ChannelWaitingToOpenTimeout);
        slimAcquired = await channel.openingSlim.WaitAsync(channel.openArguments.OpenTimeout).ConfigureAwait(false);
        if (!slimAcquired)
        {
          TransportErrorCode timeoutCode;
          bool payloadSent;
          channel.openArguments.CommonArguments.SnapshotCallState(out timeoutCode, out payloadSent);
          DefaultTrace.TraceWarning("RNTBD waiting to open timed out on channel {0}. Error: {1}", (object) channel, (object) timeoutCode);
          throw new TransportException(timeoutCode, (Exception) null, channel.openArguments.CommonArguments.ActivityId, channel.serverUri, channel.ToString(), channel.openArguments.CommonArguments.UserPayload, payloadSent);
        }
        channel.openArguments.CommonArguments.SetTimeoutCode(TransportErrorCode.ChannelOpenTimeout);
        channel.state = Channel.State.Opening;
        PooledTimer timer = channel.timerPool.GetPooledTimer(channel.openArguments.OpenTimeout);
        Task[] tasks = new Task[2]
        {
          !channel.localRegionRequest || !(channel.openArguments.OpenTimeout < timer.MinSupportedTimeout) ? timer.StartTimerAsync() : Task.Delay(channel.openArguments.OpenTimeout),
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
        if (slimAcquired)
          channel.openingSlim.Release();
      }
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
      System.Diagnostics.Trace.CorrelationManager.ActivityId = activityId;
      Exception innerException = task.Exception.InnerException;
      DefaultTrace.TraceInformation("Timed out task completed. Activity ID = {0}. HRESULT = {1:X}. Exception: {2}", (object) activityId, (object) innerException.HResult, (object) innerException);
    }), TaskContinuationOptions.OnlyOnFaulted);

    private enum State
    {
      New,
      WaitingToOpen,
      Opening,
      Open,
      Closed,
    }
  }
}
