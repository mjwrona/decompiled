// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Dispatcher
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
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
      TimeSpan idleTimeout)
    {
      this.connection = new Connection(serverUri, hostNameCertificateOverride, receiveHangDetectionTime, sendHangDetectionTime, idleTimeout);
      this.userAgent = userAgent;
      this.connectionStateListener = connectionStateListener;
      this.serverUri = serverUri;
      this.idleTimerPool = idleTimerPool;
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
          dispatcher.receiveTask = Task.Factory.StartNew<Task>(new Func<Task>(dispatcher.\u003COpenAsync\u003Eb__24_0), dispatcher.cancellation.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
          // ISSUE: reference to a compiler-generated method
          dispatcher.receiveTask.ContinueWith(new Action<Task>(dispatcher.\u003COpenAsync\u003Eb__24_1), new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
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
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId)
    {
      uint requestId = (uint) Interlocked.Increment(ref this.nextRequestId);
      lock (request)
      {
        request.Headers.Set("x-ms-transport-request-id", requestId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        byte[] serializedRequest = TransportSerialization.BuildRequest(request, physicalAddress.PathAndQuery.TrimEnd(TransportSerialization.UrlTrim), resourceOperation, activityId, out int _, out int _);
        return new Dispatcher.PrepareCallResult(requestId, physicalAddress, serializedRequest);
      }
    }

    public async Task<StoreResponse> CallAsync(ChannelCallArguments args)
    {
      Dispatcher dispatcher = this;
      dispatcher.ThrowIfDisposed();
      StoreResponse storeResponse;
      using (Dispatcher.CallInfo callInfo = new Dispatcher.CallInfo(args.CommonArguments.ActivityId, args.PreparedCall.Uri, TaskScheduler.Current))
      {
        uint requestId = args.PreparedCall.RequestId;
        lock (dispatcher.callLock)
        {
          if (!dispatcher.callsAllowed)
            throw new TransportException(TransportErrorCode.ChannelMultiplexerClosed, (Exception) null, args.CommonArguments.ActivityId, args.PreparedCall.Uri, dispatcher.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
          dispatcher.calls.Add(requestId, callInfo);
        }
        try
        {
          try
          {
            await dispatcher.connection.WriteRequestAsync(args.CommonArguments, args.PreparedCall.SerializedRequest);
            args.PreparedCall.SerializedRequest = (byte[]) null;
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
      byte[] messagePayload = TransportSerialization.BuildContextRequest(args.CommonArguments.ActivityId, this.userAgent, args.CallerId);
      await this.connection.WriteRequestAsync(args.CommonArguments, messagePayload);
      Connection.ResponseMetadata responseMetadata = await this.connection.ReadResponseMetadataAsync(args.CommonArguments);
      StatusCodes status = (StatusCodes) BitConverter.ToUInt32(responseMetadata.Header, 4);
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) responseMetadata.Header, 8, (Array) numArray, 0, 16);
      Guid activityId = new Guid(numArray);
      Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = activityId;
      using (MemoryStream readStream = new MemoryStream(responseMetadata.Metadata))
      {
        RntbdConstants.ConnectionContextResponse response = (RntbdConstants.ConnectionContextResponse) null;
        using (BinaryReader reader = new BinaryReader((Stream) readStream))
        {
          response = new RntbdConstants.ConnectionContextResponse();
          response.ParseFrom(reader);
        }
        this.serverProperties = new ServerProperties(Encoding.UTF8.GetString(response.serverAgent.value.valueBytes), Encoding.UTF8.GetString(response.serverVersion.value.valueBytes));
        if ((uint) status < 200U || (uint) status >= 400U)
        {
          using (MemoryStream memoryStream = new MemoryStream(await this.connection.ReadResponseBodyAsync(new ChannelCommonArguments(activityId, TransportErrorCode.TransportNegotiationTimeout, args.CommonArguments.UserPayload))))
          {
            DocumentClientException documentClientException = new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) JsonSerializable.LoadFrom<Error>((Stream) memoryStream).ToString()), (Exception) null, new HttpStatusCode?((HttpStatusCode) status), this.connection.ServerUri);
            if (response.clientVersion.isPresent)
              documentClientException.Headers.Add("RequiredClientVersion", Encoding.UTF8.GetString(response.clientVersion.value.valueBytes));
            if (response.protocolVersion.isPresent)
              documentClientException.Headers.Add("RequiredProtocolVersion", response.protocolVersion.value.valueULong.ToString());
            if (response.serverAgent.isPresent)
              documentClientException.Headers.Add("ServerAgent", Encoding.UTF8.GetString(response.serverAgent.value.valueBytes));
            if (response.serverVersion.isPresent)
              documentClientException.Headers.Add("x-ms-serviceversion", Encoding.UTF8.GetString(response.serverVersion.value.valueBytes));
            throw documentClientException;
          }
        }
        else
          response = (RntbdConstants.ConnectionContextResponse) null;
      }
      args.OpenTimeline.RecordRntbdHandshakeFinishTime();
    }

    private async Task ReceiveLoopAsync()
    {
      CancellationToken cancellationToken = this.cancellation.Token;
      ChannelCommonArguments args = new ChannelCommonArguments(Guid.Empty, TransportErrorCode.ReceiveTimeout, true);
      try
      {
        while (!cancellationToken.IsCancellationRequested)
        {
          args.ActivityId = Guid.Empty;
          RntbdConstants.Response response = new RntbdConstants.Response();
          Connection.ResponseMetadata responseMetadata = await this.connection.ReadResponseMetadataAsync(args);
          byte[] metadata = responseMetadata.Metadata;
          TransportSerialization.RntbdHeader header = TransportSerialization.DecodeRntbdHeader(responseMetadata.Header);
          args.ActivityId = header.ActivityId;
          using (MemoryStream input = new MemoryStream(metadata))
          {
            using (BinaryReader reader = new BinaryReader((Stream) input, Encoding.UTF8))
              response.ParseFrom(reader);
          }
          MemoryStream responseBody = (MemoryStream) null;
          if (response.payloadPresent.value.valueByte != (byte) 0)
            responseBody = new MemoryStream(await this.connection.ReadResponseBodyAsync(args));
          this.DispatchRntbdResponse(response, header, responseBody);
          response = (RntbdConstants.Response) null;
          header = (TransportSerialization.RntbdHeader) null;
        }
        this.DispatchCancellation();
      }
      catch (OperationCanceledException ex)
      {
        this.DispatchCancellation();
      }
      catch (ObjectDisposedException ex)
      {
        this.DispatchCancellation();
      }
      catch (Exception ex)
      {
        this.DispatchChannelFailureException(ex);
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
      RntbdConstants.Response rntbdResponse,
      TransportSerialization.RntbdHeader responseHeader,
      MemoryStream responseBody)
    {
      if (!rntbdResponse.transportRequestID.isPresent || rntbdResponse.transportRequestID.GetTokenType() != RntbdTokenTypes.ULong)
        throw TransportExceptions.GetInternalServerErrorException(this.serverUri, RMResources.ServerResponseTransportRequestIdMissingError);
      this.RemoveCall(rntbdResponse.transportRequestID.value.valueULong)?.SetResponse(rntbdResponse, responseHeader, responseBody, this.serverProperties.Version);
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

    public sealed class PrepareCallResult
    {
      public PrepareCallResult(uint requestId, Uri uri, byte[] serializedRequest)
      {
        this.RequestId = requestId;
        this.Uri = uri;
        this.SerializedRequest = serializedRequest;
      }

      public uint RequestId { get; private set; }

      public byte[] SerializedRequest { get; set; }

      public Uri Uri { get; private set; }
    }

    private sealed class CallInfo : IDisposable
    {
      private readonly TaskCompletionSource<StoreResponse> completion = new TaskCompletionSource<StoreResponse>();
      private readonly ManualResetEventSlim sendComplete = new ManualResetEventSlim();
      private readonly Guid activityId;
      private readonly Uri uri;
      private readonly TaskScheduler scheduler;
      private bool disposed;
      private readonly object stateLock = new object();
      private Dispatcher.CallInfo.State state;

      public CallInfo(Guid activityId, Uri uri, TaskScheduler scheduler)
      {
        this.activityId = activityId;
        this.uri = uri;
        this.scheduler = scheduler;
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
        RntbdConstants.Response rntbdResponse,
        TransportSerialization.RntbdHeader responseHeader,
        MemoryStream responseBody,
        string serverVersion)
      {
        this.ThrowIfDisposed();
        this.RunAsynchronously((Action) (() =>
        {
          Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = this.activityId;
          StoreResponse result;
          try
          {
            result = TransportSerialization.MakeStoreResponse(responseHeader.Status, responseHeader.ActivityId, rntbdResponse, (Stream) responseBody, serverVersion);
          }
          catch (Exception ex)
          {
            this.completion.SetException(ex);
            return;
          }
          this.completion.SetResult(result);
        }));
      }

      public void SetConnectionBrokenException(Exception inner, string sourceDescription)
      {
        this.ThrowIfDisposed();
        this.RunAsynchronously((Action) (() =>
        {
          Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = this.activityId;
          this.sendComplete.Wait();
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
          Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = this.activityId;
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

      private void CompleteSend(Dispatcher.CallInfo.State newState)
      {
        lock (this.stateLock)
        {
          if (this.sendComplete.IsSet)
            throw new InvalidOperationException("Send may only complete once");
          this.state = newState;
          this.sendComplete.Set();
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
