// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdConnection
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal class RntbdConnection : IConnection
  {
    private static readonly TimeSpan MaxIdleConnectionTimeout = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan MinIdleConnectionTimeout = TimeSpan.FromSeconds(100.0);
    private static readonly TimeSpan DefaultIdleConnectionTimeout = TimeSpan.FromSeconds(100.0);
    private static readonly TimeSpan DefaultUnauthenticatedTimeout = TimeSpan.FromSeconds(10.0);
    private const uint MinimumUnauthenticatedTimeoutInSeconds = 1;
    private const uint UnauthenticatedTimeoutBufferInSeconds = 5;
    private const SslProtocols EnabledTLSProtocols = SslProtocols.Tls12;
    private static readonly char[] UrlTrim = new char[1]
    {
      '/'
    };
    private static readonly byte[] KeepAliveOn = BitConverter.GetBytes(1U);
    private static readonly byte[] KeepAliveTimeInMilliseconds = BitConverter.GetBytes(30000U);
    private static readonly byte[] KeepAliveIntervalInMilliseconds = BitConverter.GetBytes(1000U);
    internal static string LocalIpv4Address;
    private static bool AddSourceIpAddressInNetworkExceptionMessagePrivate = false;
    private const int MaxContextResponse = 8000;
    private const int MaxResponse = 2147483647;
    private readonly Uri initialOpenUri;
    private readonly string poolKey;
    private Uri targetPhysicalAddress;
    private Stream stream;
    private Socket socket;
    private TcpClient tcpClient;
    private double requestTimeoutInSeconds;
    private bool isOpen;
    private string serverAgent;
    private string serverVersion;
    private TimeSpan idleTimeout;
    private TimeSpan unauthenticatedTimeout;
    private string overrideHostNameInCertificate;
    private double openTimeoutInSeconds;
    private DateTime lastUsed;
    private DateTime opened;
    private UserAgentContainer userAgent;
    private bool hasIssuedSuccessfulRequest;
    private RntbdConnectionOpenTimers connectionTimers;
    private readonly TimerPool timerPool;

    public RntbdConnection(
      Uri address,
      double requestTimeoutInSeconds,
      string overrideHostNameInCertificate,
      double openTimeoutInSeconds,
      double idleConnectionTimeoutInSeconds,
      string poolKey,
      UserAgentContainer userAgent,
      TimerPool pool)
    {
      this.connectionTimers.CreationTimestamp = DateTimeOffset.Now;
      this.initialOpenUri = address;
      this.poolKey = poolKey;
      this.requestTimeoutInSeconds = requestTimeoutInSeconds;
      this.overrideHostNameInCertificate = overrideHostNameInCertificate;
      this.openTimeoutInSeconds = openTimeoutInSeconds;
      this.idleTimeout = !(TimeSpan.FromSeconds(idleConnectionTimeoutInSeconds) < RntbdConnection.MaxIdleConnectionTimeout) || !(TimeSpan.FromSeconds(idleConnectionTimeoutInSeconds) > RntbdConnection.MinIdleConnectionTimeout) ? RntbdConnection.DefaultIdleConnectionTimeout : TimeSpan.FromSeconds(idleConnectionTimeoutInSeconds);
      this.BufferProvider = new BufferProvider();
      this.serverVersion = (string) null;
      this.opened = DateTime.UtcNow;
      this.lastUsed = this.opened;
      this.userAgent = userAgent ?? new UserAgentContainer();
      this.timerPool = pool;
    }

    protected BufferProvider BufferProvider { get; }

    public string PoolKey => this.poolKey;

    public RntbdConnectionOpenTimers ConnectionTimers => this.connectionTimers;

    public static bool AddSourceIpAddressInNetworkExceptionMessage
    {
      get => RntbdConnection.AddSourceIpAddressInNetworkExceptionMessagePrivate;
      set
      {
        if (value && !RntbdConnection.AddSourceIpAddressInNetworkExceptionMessagePrivate)
          RntbdConnection.LocalIpv4Address = NetUtil.GetNonLoopbackIpV4Address() ?? string.Empty;
        RntbdConnection.AddSourceIpAddressInNetworkExceptionMessagePrivate = value;
      }
    }

    public void Close()
    {
      if (this.stream != null)
      {
        DefaultTrace.TraceVerbose("Closing connection stream for TargetAddress: {0}, creationTime: {1}, lastUsed: {2}, poolKey: {3}", (object) this.targetPhysicalAddress, (object) this.connectionTimers.CreationTimestamp.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.lastUsed.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.poolKey);
        this.stream.Close();
        this.stream = (Stream) null;
      }
      if (this.tcpClient != null)
        this.tcpClient.Close();
      if (this.socket != null)
        this.socket = (Socket) null;
      if (!this.isOpen)
        return;
      this.isOpen = false;
    }

    public async Task Open(Guid activityId, Uri fullTargetAddress)
    {
      this.targetPhysicalAddress = fullTargetAddress;
      DateTimeOffset openStartTime = DateTimeOffset.Now;
      Task[] awaitTasks = new Task[2];
      PooledTimer delayTaskTimer = this.openTimeoutInSeconds == 0.0 ? this.timerPool.GetPooledTimer((int) this.requestTimeoutInSeconds) : this.timerPool.GetPooledTimer((int) this.openTimeoutInSeconds);
      awaitTasks[0] = delayTaskTimer.StartTimerAsync();
      awaitTasks[1] = this.OpenSocket(activityId);
      Task task1 = await Task.WhenAny(awaitTasks);
      if (task1 == awaitTasks[0])
      {
        this.CleanupWorkTask(awaitTasks[1], activityId, openStartTime);
        if (!awaitTasks[0].IsFaulted)
          throw RntbdConnection.GetGoneException(fullTargetAddress, activityId);
        throw RntbdConnection.GetGoneException(fullTargetAddress, activityId, task1.Exception.InnerException);
      }
      if (task1.IsFaulted)
      {
        delayTaskTimer.CancelTimer();
        if (!(task1.Exception.InnerException is DocumentClientException))
          throw RntbdConnection.GetGoneException(fullTargetAddress, activityId, task1.Exception.InnerException);
        ((DocumentClientException) task1.Exception.InnerException).Headers.Set("x-ms-activity-id", activityId.ToString());
        await task1;
      }
      this.connectionTimers.TcpConnectCompleteTimestamp = DateTimeOffset.Now;
      RntbdResponseState state = new RntbdResponseState();
      awaitTasks[1] = this.PerformHandshakes(activityId, state);
      Task task2 = await Task.WhenAny(awaitTasks);
      if (task2 == awaitTasks[0])
      {
        this.CleanupWorkTask(awaitTasks[1], activityId, openStartTime);
        if (!awaitTasks[0].IsFaulted)
          throw RntbdConnection.GetGoneException(fullTargetAddress, activityId);
        throw RntbdConnection.GetGoneException(fullTargetAddress, activityId, task2.Exception.InnerException);
      }
      delayTaskTimer.CancelTimer();
      if (!task2.IsFaulted)
      {
        awaitTasks = (Task[]) null;
        delayTaskTimer = (PooledTimer) null;
      }
      else
      {
        if (!(task2.Exception.InnerException is DocumentClientException))
          throw RntbdConnection.GetGoneException(fullTargetAddress, activityId, task2.Exception.InnerException);
        ((DocumentClientException) task2.Exception.InnerException).Headers.Set("x-ms-activity-id", activityId.ToString());
        await task2;
        awaitTasks = (Task[]) null;
        delayTaskTimer = (PooledTimer) null;
      }
    }

    public async Task<StoreResponse> RequestAsync(
      DocumentServiceRequest request,
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      Guid activityId)
    {
      this.targetPhysicalAddress = physicalAddress;
      BufferProvider.DisposableBuffer requestPayload = new BufferProvider.DisposableBuffer();
      int headerAndMetadataSize = 0;
      int? bodySize = new int?();
      try
      {
        requestPayload = this.BuildRequest(request, physicalAddress.PathAndQuery.TrimEnd(RntbdConnection.UrlTrim), resourceOperation, out headerAndMetadataSize, out bodySize, activityId);
      }
      catch (Exception ex)
      {
        requestPayload.Dispose();
        if (ex is DocumentClientException documentClientException1)
        {
          documentClientException1.Headers.Add("x-ms-request-validation-failure", "1");
          throw;
        }
        else
        {
          DefaultTrace.TraceError("RntbdConnection.BuildRequest failure due to assumed malformed request payload: {0}", (object) ex);
          DocumentClientException documentClientException = (DocumentClientException) new BadRequestException(ex);
          documentClientException.Headers.Add("x-ms-request-validation-failure", "1");
          throw documentClientException;
        }
      }
      StoreResponse result;
      using (requestPayload)
      {
        PooledTimer delayTaskTimer = this.timerPool.GetPooledTimer((int) this.requestTimeoutInSeconds);
        Task task1 = delayTaskTimer.StartTimerAsync();
        DateTimeOffset requestStartTime = DateTimeOffset.Now;
        Task[] awaitTasks = new Task[2]
        {
          task1,
          this.SendRequestAsyncInternal(requestPayload.Buffer, activityId)
        };
        Task task2 = await Task.WhenAny(awaitTasks);
        if (task2 == awaitTasks[0])
        {
          DateTimeOffset now = DateTimeOffset.Now;
          this.CleanupWorkTask(awaitTasks[1], activityId, requestStartTime);
          DefaultTrace.TraceError("Throwing RequestTimeoutException while awaiting request send. Task start time {0}. Task end time {1}. Request message size: {2}", (object) requestStartTime, (object) now, (object) requestPayload.Buffer.Count);
          if (!awaitTasks[0].IsFaulted)
          {
            if (request.IsReadOnlyRequest)
            {
              DefaultTrace.TraceVerbose("Converting RequestTimeout to GoneException for ReadOnlyRequest");
              throw RntbdConnection.GetGoneException(physicalAddress, activityId);
            }
            throw RntbdConnection.GetRequestTimeoutException(physicalAddress, activityId);
          }
          if (request.IsReadOnlyRequest)
          {
            DefaultTrace.TraceVerbose("Converting RequestTimeout to GoneException for ReadOnlyRequest");
            throw RntbdConnection.GetGoneException(physicalAddress, activityId, task2.Exception.InnerException);
          }
          throw RntbdConnection.GetRequestTimeoutException(physicalAddress, activityId, task2.Exception.InnerException);
        }
        if (task2.IsFaulted)
        {
          delayTaskTimer.CancelTimer();
          if (!(task2.Exception.InnerException is DocumentClientException))
            throw RntbdConnection.GetServiceUnavailableException(physicalAddress, activityId, task2.Exception.InnerException);
          ((DocumentClientException) task2.Exception.InnerException).Headers.Set("x-ms-activity-id", activityId.ToString());
          await task2;
        }
        DateTimeOffset requestSendDoneTime = DateTimeOffset.Now;
        RntbdResponseState state = new RntbdResponseState();
        Task<StoreResponse> responseTask = this.GetResponseAsync(activityId, request.IsReadOnlyRequest, state);
        awaitTasks[1] = (Task) responseTask;
        Task task3 = await Task.WhenAny(awaitTasks);
        if (task3 == awaitTasks[0])
        {
          DateTimeOffset now = DateTimeOffset.Now;
          this.CleanupWorkTask(awaitTasks[1], activityId, requestStartTime);
          DefaultTrace.TraceError("Throwing RequestTimeoutException while awaiting response receive. Task start time {0}. Request Send End time: {1}. Request header size: {2}. Request body size: {3}. Request size: {4}. Task end time {5}. State {6}.", (object) requestStartTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) requestSendDoneTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) headerAndMetadataSize, bodySize.HasValue ? (object) bodySize.Value : (object) "No body", (object) requestPayload.Buffer.Count, (object) now.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture), (object) state.ToString());
          if (!awaitTasks[0].IsFaulted)
          {
            if (request.IsReadOnlyRequest)
            {
              DefaultTrace.TraceVerbose("Converting RequestTimeout to GoneException for ReadOnlyRequest");
              throw RntbdConnection.GetGoneException(physicalAddress, activityId);
            }
            throw RntbdConnection.GetRequestTimeoutException(physicalAddress, activityId);
          }
          if (request.IsReadOnlyRequest)
          {
            DefaultTrace.TraceVerbose("Converting RequestTimeout to GoneException for ReadOnlyRequest");
            throw RntbdConnection.GetGoneException(physicalAddress, activityId, task3.Exception.InnerException);
          }
          throw RntbdConnection.GetRequestTimeoutException(physicalAddress, activityId, task3.Exception.InnerException);
        }
        delayTaskTimer.CancelTimer();
        if (task3.IsFaulted)
        {
          if (!(task3.Exception.InnerException is DocumentClientException))
            throw RntbdConnection.GetServiceUnavailableException(physicalAddress, activityId, task3.Exception.InnerException);
          ((DocumentClientException) task3.Exception.InnerException).Headers.Set("x-ms-activity-id", activityId.ToString());
          await task3;
        }
        if (responseTask.Result.Status >= 200 && responseTask.Result.Status != 410 && responseTask.Result.Status != 401 && responseTask.Result.Status != 403)
          this.hasIssuedSuccessfulRequest = true;
        this.lastUsed = DateTime.UtcNow;
        result = responseTask.Result;
      }
      requestPayload = new BufferProvider.DisposableBuffer();
      return result;
    }

    private void CleanupWorkTask(Task workTask, Guid activityId, DateTimeOffset requestStartTime) => workTask.ContinueWith((Action<Task>) (t =>
    {
      if (t.Exception == null)
        return;
      if (!(t.Exception.InnerException is ObjectDisposedException innerException2) || innerException2.ObjectName != null && string.Compare(innerException2.ObjectName, "SslStream", StringComparison.Ordinal) != 0)
        DefaultTrace.TraceError("Ignoring exception {0} on ActivityId {1}. Task start time {2} Hresult {3}", (object) t.Exception, (object) activityId.ToString(), (object) requestStartTime, (object) t.Exception.HResult);
      else
        DefaultTrace.TraceVerbose("Ignoring exception {0} on ActivityId {1}. Task start time {2} Hresult {3}", (object) innerException2, (object) activityId.ToString(), (object) requestStartTime, (object) innerException2.HResult);
    }));

    private async Task OpenSocket(Guid activityId)
    {
      TcpClient client = (TcpClient) null;
      try
      {
        IPAddress[] hostAddressesAsync = await Dns.GetHostAddressesAsync(this.initialOpenUri.DnsSafeHost);
        if (hostAddressesAsync.Length > 1)
          DefaultTrace.TraceWarning("Found multiple addresses for host, choosing the first. Host: {0}. Addresses: {1}", (object) this.initialOpenUri.DnsSafeHost, (object) hostAddressesAsync);
        IPAddress address = hostAddressesAsync[0];
        client = new TcpClient(address.AddressFamily);
        client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        RntbdConnection.SetKeepAlive(client.Client);
        await client.ConnectAsync(address, this.initialOpenUri.Port);
      }
      catch (Exception ex)
      {
        client?.Close();
        int num = ex is SocketException socketException ? (int) socketException.SocketErrorCode : throw RntbdConnection.GetGoneException(this.targetPhysicalAddress, activityId, ex);
      }
      this.tcpClient = client;
      this.socket = client.Client;
      this.stream = (Stream) client.GetStream();
      client = (TcpClient) null;
    }

    private async Task PerformHandshakes(Guid activityId, RntbdResponseState state)
    {
      string targetHost = this.overrideHostNameInCertificate != null ? this.overrideHostNameInCertificate : this.initialOpenUri.Host;
      SslStream sslStream = new SslStream(this.stream, false);
      try
      {
        await sslStream.AuthenticateAsClientAsync(targetHost, (X509CertificateCollection) null, SslProtocols.Tls12, false);
      }
      catch (Exception ex)
      {
        throw RntbdConnection.GetGoneException(this.targetPhysicalAddress, activityId, ex);
      }
      this.connectionTimers.SslHandshakeCompleteTimestamp = DateTimeOffset.Now;
      this.stream = (Stream) sslStream;
      try
      {
        await this.NegotiateRntbdContextAsync((Stream) sslStream, activityId, state);
      }
      catch (Exception ex)
      {
        if (!(ex is DocumentClientException))
          throw RntbdConnection.GetGoneException(this.targetPhysicalAddress, activityId, ex);
        throw;
      }
      this.connectionTimers.RntbdHandshakeCompleteTimestamp = DateTimeOffset.Now;
      this.isOpen = true;
      sslStream = (SslStream) null;
    }

    public bool HasExpired()
    {
      TimeSpan timeSpan1 = DateTime.UtcNow - this.lastUsed;
      TimeSpan timeSpan2 = DateTime.UtcNow - this.opened;
      TimeSpan idleTimeout = this.idleTimeout;
      if (timeSpan1 > idleTimeout)
        return true;
      return !this.hasIssuedSuccessfulRequest && timeSpan2 > this.unauthenticatedTimeout;
    }

    public bool ConfirmOpen() => CustomTypeExtensions.ConfirmOpen(this.socket);

    protected virtual byte[] BuildContextRequest(Guid activityId) => TransportSerialization.BuildContextRequest(activityId, this.userAgent, RntbdConstants.CallerId.Anonymous, false);

    private async Task NegotiateRntbdContextAsync(
      Stream negotiatingStream,
      Guid activityId,
      RntbdResponseState state)
    {
      byte[] buffer = this.BuildContextRequest(activityId);
      await negotiatingStream.WriteAsync(buffer, 0, buffer.Length);
      Tuple<byte[], byte[]> tuple = await this.ReadHeaderAndMetadata(8000, true, activityId, state);
      byte[] src = tuple.Item1;
      byte[] metadata = tuple.Item2;
      StatusCodes status = (StatusCodes) BitConverter.ToUInt32(src, 4);
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) src, 8, (Array) numArray, 0, 16);
      Guid responseActivityId = new Guid(numArray);
      RntbdConstants.ConnectionContextResponse response = (RntbdConstants.ConnectionContextResponse) null;
      BytesDeserializer reader = new BytesDeserializer(metadata, metadata.Length);
      response = new RntbdConstants.ConnectionContextResponse();
      response.ParseFrom(ref reader);
      this.serverAgent = BytesSerializer.GetStringFromBytes(response.serverAgent.value.valueBytes);
      this.serverVersion = BytesSerializer.GetStringFromBytes(response.serverVersion.value.valueBytes);
      this.SetIdleTimers(response);
      if ((uint) status < 200U || (uint) status >= 400U)
      {
        using (MemoryStream memoryStream = new MemoryStream(await this.ReadBody(true, responseActivityId, state)))
        {
          Error error = JsonSerializable.LoadFrom<Error>((Stream) memoryStream);
          System.Diagnostics.Trace.CorrelationManager.ActivityId = responseActivityId;
          DocumentClientException documentClientException = new DocumentClientException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) error.ToString()), (Exception) null, new HttpStatusCode?((HttpStatusCode) status), this.targetPhysicalAddress);
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
        response = (RntbdConstants.ConnectionContextResponse) null;
    }

    private void SetIdleTimers(RntbdConstants.ConnectionContextResponse response)
    {
      if (response.unauthenticatedTimeoutInSeconds.isPresent)
        this.unauthenticatedTimeout = TimeSpan.FromSeconds(response.unauthenticatedTimeoutInSeconds.value.valueULong <= 5U ? 1.0 : (double) (response.unauthenticatedTimeoutInSeconds.value.valueULong - 5U));
      else
        this.unauthenticatedTimeout = RntbdConnection.DefaultUnauthenticatedTimeout;
    }

    private async Task SendRequestAsyncInternal(ArraySegment<byte> requestPayload, Guid activityId)
    {
      try
      {
        await this.stream.WriteAsync(requestPayload.Array, requestPayload.Offset, requestPayload.Count);
      }
      catch (SocketException ex)
      {
        throw RntbdConnection.GetServiceUnavailableException(this.targetPhysicalAddress, activityId, (Exception) ex);
      }
      catch (IOException ex)
      {
        throw RntbdConnection.GetServiceUnavailableException(this.targetPhysicalAddress, activityId, (Exception) ex);
      }
    }

    protected virtual BufferProvider.DisposableBuffer BuildRequest(
      DocumentServiceRequest request,
      string replicaPath,
      ResourceOperation resourceOperation,
      out int headerAndMetadataSize,
      out int? bodySize,
      Guid activityId)
    {
      using (TransportSerialization.SerializedRequest serializedRequest = TransportSerialization.BuildRequest(request, replicaPath, resourceOperation, activityId, this.BufferProvider, out headerAndMetadataSize, out bodySize))
      {
        BufferProvider.DisposableBuffer buffer = this.BufferProvider.GetBuffer(serializedRequest.RequestSize);
        serializedRequest.CopyTo(buffer.Buffer);
        return buffer;
      }
    }

    private async Task<StoreResponse> GetResponseAsync(
      Guid requestActivityId,
      bool isReadOnlyRequest,
      RntbdResponseState state)
    {
      state.SetState(RntbdResponseStateEnum.Called);
      Tuple<byte[], byte[]> tuple = await this.ReadHeaderAndMetadata(int.MaxValue, isReadOnlyRequest, requestActivityId, state);
      byte[] src = tuple.Item1;
      byte[] metadata = tuple.Item2;
      StatusCodes status = (StatusCodes) BitConverter.ToUInt32(src, 4);
      byte[] numArray = new byte[16];
      Buffer.BlockCopy((Array) src, 8, (Array) numArray, 0, 16);
      Guid responseActivityId = new Guid(numArray);
      RntbdConstants.Response response = (RntbdConstants.Response) null;
      BytesDeserializer reader = new BytesDeserializer(metadata, metadata.Length);
      response = new RntbdConstants.Response();
      response.ParseFrom(ref reader);
      MemoryStream body = (MemoryStream) null;
      if (response.payloadPresent.value.valueByte != (byte) 0)
      {
        byte[] buffer;
        try
        {
          buffer = await this.ReadBody(false, responseActivityId, state);
        }
        catch (Exception ex)
        {
          if (!(ex is DocumentClientException))
            throw RntbdConnection.GetServiceUnavailableException(this.targetPhysicalAddress, responseActivityId, ex);
          throw;
        }
        body = new MemoryStream(buffer);
      }
      state.SetState(RntbdResponseStateEnum.Done);
      StoreResponse responseAsync = TransportSerialization.MakeStoreResponse(status, responseActivityId, response, (Stream) body, this.serverVersion);
      response = (RntbdConstants.Response) null;
      return responseAsync;
    }

    private async Task<Tuple<byte[], byte[]>> ReadHeaderAndMetadata(
      int maxAllowed,
      bool throwGoneOnChannelFailure,
      Guid activityId,
      RntbdResponseState state)
    {
      state.SetState(RntbdResponseStateEnum.StartHeader);
      byte[] header = new byte[24];
      int headerRead;
      int read;
      for (headerRead = 0; headerRead < header.Length; headerRead += read)
      {
        read = 0;
        try
        {
          read = await this.stream.ReadAsync(header, headerRead, header.Length - headerRead);
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError("Hit IOException while reading header on connection with last used time {0}", (object) this.lastUsed.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId, (Exception) ex);
        }
        if (read == 0)
        {
          DefaultTrace.TraceError("Read 0 bytes while reading header");
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId);
        }
        state.SetState(RntbdResponseStateEnum.BufferingHeader);
        state.AddHeaderMetadataRead(read);
      }
      state?.SetState(RntbdResponseStateEnum.DoneBufferingHeader);
      uint uint32 = BitConverter.ToUInt32(header, 0);
      if ((long) uint32 > (long) maxAllowed)
      {
        DefaultTrace.TraceCritical("RNTBD header length says {0} but expected at most {1} bytes", (object) uint32, (object) maxAllowed);
        throw RntbdConnection.GetInternalServerErrorException(this.targetPhysicalAddress, activityId);
      }
      if ((long) uint32 < (long) header.Length)
      {
        DefaultTrace.TraceCritical("RNTBD header length says {0} but expected at least {1} bytes and read {2} bytes from wire", (object) uint32, (object) header.Length, (object) headerRead);
        throw RntbdConnection.GetInternalServerErrorException(this.targetPhysicalAddress, activityId);
      }
      int metadataLength = (int) uint32 - header.Length;
      byte[] metadata = new byte[metadataLength];
      for (int responseMetadataRead = 0; responseMetadataRead < metadataLength; responseMetadataRead += read)
      {
        read = 0;
        try
        {
          read = await this.stream.ReadAsync(metadata, responseMetadataRead, metadataLength - responseMetadataRead);
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError("Hit IOException while reading metadata on connection with last used time {0}", (object) this.lastUsed.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId, (Exception) ex);
        }
        if (read == 0)
        {
          DefaultTrace.TraceError("Read 0 bytes while reading metadata");
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId);
        }
        state.SetState(RntbdResponseStateEnum.BufferingMetadata);
        state.AddHeaderMetadataRead(read);
      }
      state.SetState(RntbdResponseStateEnum.DoneBufferingMetadata);
      Tuple<byte[], byte[]> tuple = new Tuple<byte[], byte[]>(header, metadata);
      header = (byte[]) null;
      metadata = (byte[]) null;
      return tuple;
    }

    private async Task<byte[]> ReadBody(
      bool throwGoneOnChannelFailure,
      Guid activityId,
      RntbdResponseState state)
    {
      byte[] bodyLengthHeader = new byte[4];
      int read;
      for (int bodyLengthRead = 0; bodyLengthRead < 4; bodyLengthRead += read)
      {
        read = 0;
        try
        {
          read = await this.stream.ReadAsync(bodyLengthHeader, bodyLengthRead, bodyLengthHeader.Length - bodyLengthRead);
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError("Hit IOException while reading BodyLengthHeader on connection with last used time {0}", (object) this.lastUsed.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId, (Exception) ex);
        }
        if (read == 0)
        {
          DefaultTrace.TraceError("Read 0 bytes while reading BodyLengthHeader");
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId);
        }
        state.SetState(RntbdResponseStateEnum.BufferingBodySize);
        state.AddBodyRead(read);
      }
      state.SetState(RntbdResponseStateEnum.DoneBufferingBodySize);
      uint bodyRead = 0;
      uint length = BitConverter.ToUInt32(bodyLengthHeader, 0);
      byte[] body = new byte[(int) length];
      for (; bodyRead < length; bodyRead += (uint) read)
      {
        read = 0;
        try
        {
          read = await this.stream.ReadAsync(body, (int) bodyRead, body.Length - (int) bodyRead);
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError("Hit IOException while reading Body on connection with last used time {0}", (object) this.lastUsed.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId, (Exception) ex);
        }
        if (read == 0)
        {
          DefaultTrace.TraceError("Read 0 bytes while reading Body");
          this.ThrowOnFailure(throwGoneOnChannelFailure, activityId);
        }
        state.SetState(RntbdResponseStateEnum.BufferingBody);
        state.AddBodyRead(read);
      }
      state.SetState(RntbdResponseStateEnum.DoneBufferingBody);
      byte[] numArray = body;
      bodyLengthHeader = (byte[]) null;
      body = (byte[]) null;
      return numArray;
    }

    private void ThrowOnFailure(
      bool throwGoneOnChannelFailure,
      Guid activityId,
      Exception innerException = null)
    {
      if (throwGoneOnChannelFailure)
        throw RntbdConnection.GetGoneException(this.targetPhysicalAddress, activityId, innerException);
      throw RntbdConnection.GetServiceUnavailableException(this.targetPhysicalAddress, activityId, innerException);
    }

    private static GoneException GetGoneException(
      Uri fullTargetAddress,
      Guid activityId,
      Exception inner = null)
    {
      return TransportExceptions.GetGoneException(fullTargetAddress, activityId, inner);
    }

    private static RequestTimeoutException GetRequestTimeoutException(
      Uri fullTargetAddress,
      Guid activityId,
      Exception inner = null)
    {
      return TransportExceptions.GetRequestTimeoutException(fullTargetAddress, activityId, inner);
    }

    private static ServiceUnavailableException GetServiceUnavailableException(
      Uri fullTargetAddress,
      Guid activityId,
      Exception inner = null)
    {
      return TransportExceptions.GetServiceUnavailableException(fullTargetAddress, activityId, inner);
    }

    private static InternalServerErrorException GetInternalServerErrorException(
      Uri fullTargetAddress,
      Guid activityId,
      Exception inner = null)
    {
      return TransportExceptions.GetInternalServerErrorException(fullTargetAddress, activityId, inner);
    }

    private static void SetKeepAlive(Socket socket)
    {
    }
  }
}
