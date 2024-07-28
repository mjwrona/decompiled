// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Dispatcher
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class Dispatcher : IDisposable
  {
    private readonly Connection connection;
    private readonly UserAgentContainer userAgent;
    private readonly Uri serverUri;
    private readonly IConnectionStateListener connectionStateListener;
    private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
    private readonly TimerPool idleTimerPool;
    private readonly bool enableChannelMultiplexing;
    private bool disposed;
    private ServerProperties serverProperties;
    private int nextRequestId;
    private readonly object callLock = new object();
    private Task receiveTask;
    private readonly Dictionary<uint, Dispatcher.CallInfo> calls = new Dictionary<uint, Dispatcher.CallInfo>();
    private bool callsAllowed = true;
    private readonly object connectionLock = new object();
    private PooledTimer idleTimer;
    private Task idleTimerTask;

    public Dispatcher(
      Uri serverUri,
      UserAgentContainer userAgent,
      IConnectionStateListener connectionStateListener,
      string hostNameCertificateOverride,
      TimeSpan receiveHangDetectionTime,
      TimeSpan sendHangDetectionTime,
      TimerPool idleTimerPool,
      TimeSpan idleTimeout,
      bool enableChannelMultiplexing,
      MemoryStreamPool memoryStreamPool,
      RemoteCertificateValidationCallback remoteCertificateValidationCallback = null)
    {
      this.connection = new Connection(serverUri, hostNameCertificateOverride, receiveHangDetectionTime, sendHangDetectionTime, idleTimeout, memoryStreamPool, remoteCertificateValidationCallback);
      this.userAgent = userAgent;
      this.connectionStateListener = connectionStateListener;
      this.serverUri = serverUri;
      this.idleTimerPool = idleTimerPool;
      this.enableChannelMultiplexing = enableChannelMultiplexing;
    }

    internal event Action TestOnConnectionClosed;

    internal bool TestIsIdle
    {
      get
      {
        lock (this.connectionLock)
          return this.connection.Disposed || !this.connection.IsActive(out TimeSpan _);
      }
    }

    public bool Healthy
    {
      get
      {
        this.ThrowIfDisposed();
        if (this.cancellation.IsCancellationRequested)
          return false;
        lock (this.callLock)
        {
          if (!this.callsAllowed)
            return false;
        }
        bool flag;
        try
        {
          flag = this.connection.Healthy;
        }
        catch (ObjectDisposedException ex)
        {
          DefaultTrace.TraceWarning("RNTBD Dispatcher {0}: ObjectDisposedException from Connection.Healthy", (object) this);
          flag = false;
        }
        if (flag)
          return true;
        lock (this.callLock)
          this.callsAllowed = false;
        return false;
      }
    }

    public async Task OpenAsync(ChannelOpenArguments args)
    {
      Dispatcher dispatcher = this;
      dispatcher.ThrowIfDisposed();
      try
      {
        await dispatcher.connection.OpenAsync(args);
        await dispatcher.NegotiateRntbdContextAsync(args);
        lock (dispatcher.callLock)
        {
          // ISSUE: reference to a compiler-generated method
          dispatcher.receiveTask = Task.Factory.StartNew<Task>(new Func<Task>(dispatcher.\u003COpenAsync\u003Eb__25_0), dispatcher.cancellation.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
          // ISSUE: reference to a compiler-generated method
          dispatcher.receiveTask.ContinueWith(new Action<Task>(dispatcher.\u003COpenAsync\u003Eb__25_1), new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
        }
        if (dispatcher.idleTimerPool == null)
          return;
        dispatcher.StartIdleTimer();
      }
      catch (DocumentClientException ex)
      {
        dispatcher.DisallowInitialCalls();
        throw;
      }
      catch (TransportException ex)
      {
        dispatcher.DisallowInitialCalls();
        throw;
      }
    }

    public Dispatcher.PrepareCallResult PrepareCall(
      DocumentServiceRequest request,
      TransportAddressUri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId,
      TransportRequestStats transportRequestStats)
    {
      uint requestId = (uint) Interlocked.Increment(ref this.nextRequestId);
      lock (request)
      {
        request.Headers.Set("x-ms-transport-request-id", requestId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        int? bodySize;
        TransportSerialization.SerializedRequest serializedRequest = TransportSerialization.BuildRequest(request, physicalAddress.PathAndQuery, resourceOperation, activityId, this.connection.BufferProvider, out int _, out bodySize);
        TransportRequestStats transportRequestStats1 = transportRequestStats;
        int? nullable1 = bodySize;
        long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
        transportRequestStats1.RequestBodySizeInBytes = nullable2;
        transportRequestStats.RequestSizeInBytes = new long?((long) serializedRequest.RequestSize);
        return new Dispatcher.PrepareCallResult(requestId, physicalAddress.Uri, serializedRequest);
      }
    }

    public async Task<StoreResponse> CallAsync(
      ChannelCallArguments args,
      TransportRequestStats transportRequestStats)
    {
      Dispatcher dispatcher = this;
      dispatcher.ThrowIfDisposed();
      StoreResponse storeResponse;
      using (Dispatcher.CallInfo callInfo = new Dispatcher.CallInfo(args.CommonArguments.ActivityId, args.PreparedCall.Uri, TaskScheduler.Current, transportRequestStats))
      {
        uint requestId = args.PreparedCall.RequestId;
        lock (dispatcher.callLock)
        {
          transportRequestStats.NumberOfInflightRequestsInConnection = new int?(dispatcher.calls.Count);
          if (!dispatcher.callsAllowed)
            throw new TransportException(TransportErrorCode.ChannelMultiplexerClosed, (Exception) null, args.CommonArguments.ActivityId, args.PreparedCall.Uri, dispatcher.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
          dispatcher.calls.Add(requestId, callInfo);
        }
        try
        {
          try
          {
            await dispatcher.connection.WriteRequestAsync(args.CommonArguments, args.PreparedCall.SerializedRequest, transportRequestStats);
            transportRequestStats.RecordState(TransportRequestStats.RequestStage.Sent);
          }
          catch (Exception ex)
          {
            callInfo.SendFailed();
            throw new TransportException(TransportErrorCode.SendFailed, ex, args.CommonArguments.ActivityId, args.PreparedCall.Uri, dispatcher.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
          }
          storeResponse = await callInfo.ReadResponseAsync(args);
        }
        catch (DocumentClientException ex)
        {
          dispatcher.DisallowRuntimeCalls();
          throw;
        }
        catch (TransportException ex)
        {
          dispatcher.DisallowRuntimeCalls();
          throw;
        }
        finally
        {
          dispatcher.RemoveCall(requestId);
        }
      }
      return storeResponse;
    }

    public void CancelCall(Dispatcher.PrepareCallResult preparedCall)
    {
      this.ThrowIfDisposed();
      this.RemoveCall(preparedCall.RequestId)?.Cancel();
    }

    public override string ToString() => this.connection.ToString();

    public void Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      DefaultTrace.TraceInformation("Disposing RNTBD Dispatcher {0}", (object) this);
      Task t1 = (Task) null;
      lock (this.connectionLock)
      {
        this.StartConnectionShutdown();
        t1 = this.StopIdleTimer();
      }
      this.WaitTask(t1, "idle timer");
      Task t2 = (Task) null;
      lock (this.connectionLock)
        t2 = this.CloseConnection();
      this.WaitTask(t2, "receive loop");
      DefaultTrace.TraceInformation("RNTBD Dispatcher {0} is disposed", (object) this);
    }

    private void StartIdleTimer()
    {
      DefaultTrace.TraceInformation("RNTBD idle connection monitor: Timer is starting...");
      TimeSpan timeToIdle = TimeSpan.MinValue;
      bool flag = false;
      try
      {
        lock (this.connectionLock)
        {
          if (!this.connection.IsActive(out timeToIdle))
          {
            DefaultTrace.TraceCritical("RNTBD Dispatcher {0}: New connection already idle.", (object) this);
          }
          else
          {
            this.ScheduleIdleTimer(timeToIdle);
            flag = true;
          }
        }
      }
      finally
      {
        if (flag)
          DefaultTrace.TraceInformation("RNTBD idle connection monitor {0}: Timer is scheduled to fire {1} seconds later at {2}.", (object) this, (object) timeToIdle.TotalSeconds, (object) (DateTime.UtcNow + timeToIdle));
        else
          DefaultTrace.TraceInformation("RNTBD idle connection monitor {0}: Timer is not scheduled.", (object) this);
      }
    }

    private void OnIdleTimer(Task precedentTask)
    {
      Task t = (Task) null;
      lock (this.connectionLock)
      {
        if (this.cancellation.IsCancellationRequested)
          return;
        TimeSpan timeToIdle;
        bool flag = this.connection.IsActive(out timeToIdle);
        if (flag)
        {
          this.ScheduleIdleTimer(timeToIdle);
          return;
        }
        lock (this.callLock)
        {
          if (this.calls.Count > 0)
          {
            DefaultTrace.TraceCritical("RNTBD Dispatcher {0}: Looks idle but still has {1} pending requests", (object) this, (object) this.calls.Count);
            flag = true;
          }
          else
            this.callsAllowed = false;
        }
        if (flag)
        {
          this.ScheduleIdleTimer(timeToIdle);
          return;
        }
        this.idleTimer = (PooledTimer) null;
        this.idleTimerTask = (Task) null;
        this.StartConnectionShutdown();
        t = this.CloseConnection();
      }
      this.WaitTask(t, "receive loop");
    }

    private void ScheduleIdleTimer(TimeSpan timeToIdle)
    {
      this.idleTimer = this.idleTimerPool.GetPooledTimer((int) timeToIdle.TotalSeconds);
      this.idleTimerTask = this.idleTimer.StartTimerAsync().ContinueWith(new Action<Task>(this.OnIdleTimer), TaskContinuationOptions.OnlyOnRanToCompletion);
      this.idleTimerTask.ContinueWith((Action<Task>) (failedTask => DefaultTrace.TraceWarning("RNTBD Dispatcher {0} idle timer callback failed: {1}", (object) this, (object) failedTask.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
    }

    private void StartConnectionShutdown()
    {
      if (this.cancellation.IsCancellationRequested)
        return;
      try
      {
        lock (this.callLock)
          this.callsAllowed = false;
        this.cancellation.Cancel();
      }
      catch (AggregateException ex)
      {
        DefaultTrace.TraceWarning("RNTBD Dispatcher {0}: Registered cancellation callbacks failed: {1}", (object) this, (object) ex);
      }
    }

    private Task StopIdleTimer()
    {
      Task task = (Task) null;
      if (this.idleTimer != null)
      {
        if (this.idleTimer.CancelTimer())
        {
          this.idleTimer = (PooledTimer) null;
          this.idleTimerTask = (Task) null;
        }
        else
          task = this.idleTimerTask;
      }
      return task;
    }

    private Task CloseConnection()
    {
      Task task = (Task) null;
      if (!this.connection.Disposed)
      {
        lock (this.callLock)
          task = this.receiveTask;
        this.connection.Dispose();
        Action connectionClosed = this.TestOnConnectionClosed;
        if (connectionClosed != null)
          connectionClosed();
      }
      return task;
    }

    private void WaitTask(Task t, string description)
    {
      if (t == null)
        return;
      try
      {
        t.Wait();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("RNTBD Dispatcher {0}: Parallel task failed: {1}. Exception: {2}", (object) this, (object) description, (object) ex);
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(string.Format("{0}:{1}", (object) nameof (Dispatcher), (object) this.serverUri));
    }

    private async Task NegotiateRntbdContextAsync(ChannelOpenArguments args)
    {
      byte[] buffer = TransportSerialization.BuildContextRequest(args.CommonArguments.ActivityId, this.userAgent, args.CallerId, this.enableChannelMultiplexing);
      await this.connection.WriteRequestAsync(args.CommonArguments, new TransportSerialization.SerializedRequest(new BufferProvider.DisposableBuffer(buffer), (CloneableStream) null), (TransportRequestStats) null);
      RntbdConstants.ConnectionContextResponse response;
      using (Connection.ResponseMetadata responseMd = await this.connection.ReadResponseMetadataAsync(args.CommonArguments))
      {
        ArraySegment<byte> arraySegment = responseMd.Header;
        StatusCodes status = (StatusCodes) BitConverter.ToUInt32(arraySegment.Array, 4);
        byte[] numArray = new byte[16];
        arraySegment = responseMd.Header;
        Buffer.BlockCopy((Array) arraySegment.Array, 8, (Array) numArray, 0, 16);
        Guid activityId = new Guid(numArray);
        System.Diagnostics.Trace.CorrelationManager.ActivityId = activityId;
        arraySegment = responseMd.Metadata;
        byte[] array = arraySegment.Array;
        arraySegment = responseMd.Metadata;
        int count = arraySegment.Count;
        BytesDeserializer reader = new BytesDeserializer(array, count);
        response = new RntbdConstants.ConnectionContextResponse();
        response.ParseFrom(ref reader);
        this.serverProperties = new ServerProperties(BytesSerializer.GetStringFromBytes(response.serverAgent.value.valueBytes), BytesSerializer.GetStringFromBytes(response.serverVersion.value.valueBytes));
        if ((uint) status < 200U || (uint) status >= 400U)
        {
          using (MemoryStream memoryStream = await this.connection.ReadResponseBodyAsync(new ChannelCommonArguments(activityId, TransportErrorCode.TransportNegotiationTimeout, args.CommonArguments.UserPayload)))
          {
            DocumentClientException documentClientException = new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) JsonSerializable.LoadFrom<Error>((Stream) memoryStream).ToString()), (Exception) null, new HttpStatusCode?((HttpStatusCode) status), this.connection.ServerUri);
            if (response.clientVersion.isPresent)
              documentClientException.Headers.Add("RequiredClientVersion", BytesSerializer.GetStringFromBytes(response.clientVersion.value.valueBytes));
            if (response.protocolVersion.isPresent)
              documentClientException.Headers.Add("RequiredProtocolVersion", response.protocolVersion.value.valueULong.ToString());
            if (response.serverAgent.isPresent)
              documentClientException.Headers.Add("ServerAgent", BytesSerializer.GetStringFromBytes(response.serverAgent.value.valueBytes));
            if (response.serverVersion.isPresent)
              documentClientException.Headers.Add("x-ms-serviceversion", BytesSerializer.GetStringFromBytes(response.serverVersion.value.valueBytes));
            throw documentClientException;
          }
        }
        else
          args.OpenTimeline.RecordRntbdHandshakeFinishTime();
      }
      response = (RntbdConstants.ConnectionContextResponse) null;
    }

    private async Task ReceiveLoopAsync()
    {
      CancellationToken cancellationToken = this.cancellation.Token;
      ChannelCommonArguments args = new ChannelCommonArguments(Guid.Empty, TransportErrorCode.ReceiveTimeout, true);
      RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner response = new RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner();
      Connection.ResponseMetadata responseMd = (Connection.ResponseMetadata) null;
      try
      {
        while (!cancellationToken.IsCancellationRequested)
        {
          args.ActivityId = Guid.Empty;
          response = RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.Instance.Get();
          responseMd = await this.connection.ReadResponseMetadataAsync(args);
          ArraySegment<byte> metadata = responseMd.Metadata;
          TransportSerialization.RntbdHeader header = TransportSerialization.DecodeRntbdHeader(responseMd.Header.Array);
          args.ActivityId = header.ActivityId;
          BytesDeserializer reader = new BytesDeserializer(metadata.Array, metadata.Count);
          response.Entity.ParseFrom(ref reader);
          MemoryStream responseBody = (MemoryStream) null;
          if (response.Entity.payloadPresent.value.valueByte != (byte) 0)
            responseBody = await this.connection.ReadResponseBodyAsync(args);
          this.DispatchRntbdResponse(responseMd, response, header, responseBody);
          responseMd = (Connection.ResponseMetadata) null;
          header = (TransportSerialization.RntbdHeader) null;
        }
        this.DispatchCancellation();
        args = (ChannelCommonArguments) null;
        response = new RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner();
        responseMd = (Connection.ResponseMetadata) null;
      }
      catch (OperationCanceledException ex)
      {
        response.Dispose();
        responseMd?.Dispose();
        this.DispatchCancellation();
        args = (ChannelCommonArguments) null;
        response = new RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner();
        responseMd = (Connection.ResponseMetadata) null;
      }
      catch (ObjectDisposedException ex)
      {
        response.Dispose();
        responseMd?.Dispose();
        this.DispatchCancellation();
        args = (ChannelCommonArguments) null;
        response = new RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner();
        responseMd = (Connection.ResponseMetadata) null;
      }
      catch (Exception ex)
      {
        response.Dispose();
        responseMd?.Dispose();
        this.DispatchChannelFailureException(ex);
        args = (ChannelCommonArguments) null;
        response = new RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner();
        responseMd = (Connection.ResponseMetadata) null;
      }
    }

    private Dictionary<uint, Dispatcher.CallInfo> StopCalls()
    {
      Dictionary<uint, Dispatcher.CallInfo> dictionary;
      lock (this.callLock)
      {
        dictionary = new Dictionary<uint, Dispatcher.CallInfo>((IDictionary<uint, Dispatcher.CallInfo>) this.calls);
        this.calls.Clear();
        this.callsAllowed = false;
      }
      return dictionary;
    }

    private void DispatchRntbdResponse(
      Connection.ResponseMetadata responseMd,
      RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner rntbdResponse,
      TransportSerialization.RntbdHeader responseHeader,
      MemoryStream responseBody)
    {
      if (!rntbdResponse.Entity.transportRequestID.isPresent || rntbdResponse.Entity.transportRequestID.GetTokenType() != RntbdTokenTypes.ULong)
      {
        responseBody?.Dispose();
        rntbdResponse.Dispose();
        responseMd.Dispose();
        throw TransportExceptions.GetInternalServerErrorException(this.serverUri, RMResources.ServerResponseTransportRequestIdMissingError);
      }
      Dispatcher.CallInfo callInfo = this.RemoveCall(rntbdResponse.Entity.transportRequestID.value.valueULong);
      if (callInfo != null)
      {
        callInfo.TransportRequestStats.RecordState(TransportRequestStats.RequestStage.Received);
        callInfo.TransportRequestStats.ResponseMetadataSizeInBytes = new long?((long) responseMd.Metadata.Count);
        callInfo.TransportRequestStats.ResponseBodySizeInBytes = responseBody?.Length;
        callInfo.SetResponse(responseMd, rntbdResponse, responseHeader, responseBody, this.serverProperties.Version);
      }
      else
      {
        responseBody?.Dispose();
        responseMd.Dispose();
        rntbdResponse.Dispose();
      }
    }

    private void DispatchChannelFailureException(Exception ex)
    {
      Dictionary<uint, Dispatcher.CallInfo> dictionary = this.StopCalls();
      foreach (KeyValuePair<uint, Dispatcher.CallInfo> keyValuePair in dictionary)
        keyValuePair.Value.SetConnectionBrokenException(ex, this.ToString());
      if (dictionary.Count > 0 || this.connectionStateListener == null)
        return;
      if (!(ex is TransportException transportException))
      {
        DefaultTrace.TraceWarning("Not a TransportException. Will not raise the connection state change event: {0}", (object) ex);
      }
      else
      {
        ConnectionEvent connectionEvent;
        switch (transportException.ErrorCode)
        {
          case TransportErrorCode.ReceiveFailed:
            connectionEvent = ConnectionEvent.ReadFailure;
            break;
          case TransportErrorCode.ReceiveStreamClosed:
            connectionEvent = ConnectionEvent.ReadEof;
            break;
          default:
            DefaultTrace.TraceWarning("Will not raise the connection state change event for TransportException error code {0}. Exception: {1}", (object) transportException.ErrorCode.ToString(), (object) transportException.Message);
            return;
        }
        IConnectionStateListener connectionStateListener = this.connectionStateListener;
        ServerKey serverKey = new ServerKey(this.serverUri);
        DateTime exceptionTime = transportException.Timestamp;
        Task.Run((Action) (() => connectionStateListener.OnConnectionEvent(connectionEvent, exceptionTime, serverKey))).ContinueWith((Action<Task>) (failedTask => DefaultTrace.TraceError("OnConnectionEvent callback failed: {0}", (object) failedTask.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);
      }
    }

    private void DispatchCancellation()
    {
      foreach (KeyValuePair<uint, Dispatcher.CallInfo> stopCall in this.StopCalls())
        stopCall.Value.Cancel();
    }

    private Dispatcher.CallInfo RemoveCall(uint requestId)
    {
      Dispatcher.CallInfo callInfo = (Dispatcher.CallInfo) null;
      lock (this.callLock)
      {
        this.calls.TryGetValue(requestId, out callInfo);
        this.calls.Remove(requestId);
      }
      return callInfo;
    }

    private void DisallowInitialCalls()
    {
      lock (this.callLock)
        this.callsAllowed = false;
    }

    private void DisallowRuntimeCalls()
    {
      lock (this.callLock)
        this.callsAllowed = false;
    }

    public sealed class PrepareCallResult : IDisposable
    {
      private bool disposed;

      public PrepareCallResult(
        uint requestId,
        Uri uri,
        TransportSerialization.SerializedRequest serializedRequest)
      {
        this.RequestId = requestId;
        this.Uri = uri;
        this.SerializedRequest = serializedRequest;
      }

      public uint RequestId { get; private set; }

      public TransportSerialization.SerializedRequest SerializedRequest { get; }

      public Uri Uri { get; private set; }

      public void Dispose()
      {
        if (this.disposed)
          return;
        this.SerializedRequest.Dispose();
        this.disposed = true;
      }
    }

    internal sealed class CallInfo : IDisposable
    {
      private readonly TaskCompletionSource<StoreResponse> completion = new TaskCompletionSource<StoreResponse>();
      private readonly SemaphoreSlim sendComplete = new SemaphoreSlim(0);
      private readonly Guid activityId;
      private readonly Uri uri;
      private readonly TaskScheduler scheduler;
      private bool disposed;
      private readonly object stateLock = new object();
      private Dispatcher.CallInfo.State state;

      public TransportRequestStats TransportRequestStats { get; }

      public CallInfo(
        Guid activityId,
        Uri uri,
        TaskScheduler scheduler,
        TransportRequestStats transportRequestStats)
      {
        this.activityId = activityId;
        this.uri = uri;
        this.scheduler = scheduler;
        this.TransportRequestStats = transportRequestStats;
      }

      public Task<StoreResponse> ReadResponseAsync(ChannelCallArguments args)
      {
        this.ThrowIfDisposed();
        this.CompleteSend(Dispatcher.CallInfo.State.Sent);
        args.CommonArguments.SetTimeoutCode(TransportErrorCode.ReceiveTimeout);
        return this.completion.Task;
      }

      public void SendFailed()
      {
        this.ThrowIfDisposed();
        this.CompleteSend(Dispatcher.CallInfo.State.SendFailed);
      }

      public void SetResponse(
        Connection.ResponseMetadata responseMd,
        RntbdConstants.RntbdEntityPool<RntbdConstants.Response, RntbdConstants.ResponseIdentifiers>.EntityOwner rntbdResponse,
        TransportSerialization.RntbdHeader responseHeader,
        MemoryStream responseBody,
        string serverVersion)
      {
        this.ThrowIfDisposed();
        this.RunAsynchronously((Action) (() =>
        {
          System.Diagnostics.Trace.CorrelationManager.ActivityId = this.activityId;
          try
          {
            this.completion.SetResult(TransportSerialization.MakeStoreResponse(responseHeader.Status, responseHeader.ActivityId, rntbdResponse.Entity, (Stream) responseBody, serverVersion));
          }
          catch (Exception ex)
          {
            this.completion.SetException(ex);
            responseBody?.Dispose();
          }
          finally
          {
            rntbdResponse.Dispose();
            responseMd.Dispose();
          }
        }));
      }

      public void SetConnectionBrokenException(Exception inner, string sourceDescription)
      {
        this.ThrowIfDisposed();
        this.RunAsynchronously((Func<Task>) (async () =>
        {
          System.Diagnostics.Trace.CorrelationManager.ActivityId = this.activityId;
          await this.sendComplete.WaitAsync();
          lock (this.stateLock)
          {
            if (this.state != Dispatcher.CallInfo.State.Sent)
              return;
          }
          this.completion.SetException((Exception) new TransportException(TransportErrorCode.ConnectionBroken, inner, this.activityId, this.uri, sourceDescription, true, true));
        }));
      }

      public void Cancel()
      {
        this.ThrowIfDisposed();
        this.RunAsynchronously((Action) (() =>
        {
          System.Diagnostics.Trace.CorrelationManager.ActivityId = this.activityId;
          this.completion.SetCanceled();
        }));
      }

      public void Dispose()
      {
        this.ThrowIfDisposed();
        this.disposed = true;
        this.sendComplete.Dispose();
      }

      private void ThrowIfDisposed()
      {
        if (this.disposed)
          throw new ObjectDisposedException(nameof (CallInfo));
      }

      private void RunAsynchronously(Action action) => Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.scheduler).ContinueWith((Action<Task>) (failedTask => DefaultTrace.TraceError("Unexpected: Rntbd asynchronous completion call failed. Consuming the task exception asynchronously. Exception: {0}", (object) failedTask.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      private void RunAsynchronously(Func<Task> action) => Task.Factory.StartNew<Task>(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, this.scheduler).Unwrap().ContinueWith((Action<Task>) (failedTask => DefaultTrace.TraceError("Unexpected: Rntbd asynchronous completion call failed. Consuming the task exception asynchronously. Exception: {0}", (object) failedTask.Exception?.InnerException)), TaskContinuationOptions.OnlyOnFaulted);

      private void CompleteSend(Dispatcher.CallInfo.State newState)
      {
        lock (this.stateLock)
        {
          this.state = this.state == Dispatcher.CallInfo.State.New ? newState : throw new InvalidOperationException("Send may only complete once");
          this.sendComplete.Release();
        }
      }

      private enum State
      {
        New,
        Sent,
        SendFailed,
      }
    }
  }
}
