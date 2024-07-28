// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Connection
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class Connection : IDisposable
  {
    private const string socketOptionTcpKeepAliveIntervalName = "AZURE_COSMOS_TCP_KEEPALIVE_INTERVAL_SECONDS";
    private const string socketOptionTcpKeepAliveTimeName = "AZURE_COSMOS_TCP_KEEPALIVE_TIME_SECONDS";
    private const int ResponseLengthByteLimit = 2147483647;
    private const SslProtocols TlsProtocols = SslProtocols.Tls12;
    private const uint TcpKeepAliveIntervalSocketOptionEnumValue = 17;
    private const uint TcpKeepAliveTimeSocketOptionEnumValue = 3;
    private const uint DefaultSocketOptionTcpKeepAliveInterval = 1;
    private const uint DefaultSocketOptionTcpKeepAliveTime = 30;
    private const int MinNumberOfSendsSinceLastReceiveForUnhealthyConnection = 3;
    private static readonly uint SocketOptionTcpKeepAliveInterval = Connection.GetUInt32FromEnvironmentVariableOrDefault("AZURE_COSMOS_TCP_KEEPALIVE_INTERVAL_SECONDS", 1U, 100U, 1U);
    private static readonly uint SocketOptionTcpKeepAliveTime = Connection.GetUInt32FromEnvironmentVariableOrDefault("AZURE_COSMOS_TCP_KEEPALIVE_TIME_SECONDS", 1U, 100U, 30U);
    private static readonly Lazy<ConcurrentPrng> rng = new Lazy<ConcurrentPrng>(LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<byte[]> keepAliveConfiguration = new Lazy<byte[]>(new Func<byte[]>(Connection.GetWindowsKeepAliveConfiguration), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<bool> isKeepAliveCustomizationSupported = new Lazy<bool>(new Func<bool>(Connection.IsKeepAliveCustomizationSupported), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly byte[] healthCheckBuffer = new byte[1];
    private static readonly TimeSpan recentReceiveWindow = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan sendHangGracePeriod = TimeSpan.FromSeconds(2.0);
    private static readonly TimeSpan receiveHangGracePeriod = TimeSpan.FromSeconds(10.0);
    private readonly Uri serverUri;
    private readonly string hostNameCertificateOverride;
    private readonly TimeSpan receiveDelayLimit;
    private readonly TimeSpan sendDelayLimit;
    private readonly MemoryStreamPool memoryStreamPool;
    private readonly RemoteCertificateValidationCallback remoteCertificateValidationCallback;
    private bool disposed;
    private TcpClient tcpClient;
    private UserPortPool portPool;
    private IPEndPoint localEndPoint;
    private IPEndPoint remoteEndPoint;
    private readonly TimeSpan idleConnectionTimeout;
    private readonly TimeSpan idleConnectionClosureTimeout;
    private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1);
    private Stream stream;
    private RntbdStreamReader streamReader;
    private readonly object timestampLock = new object();
    private DateTime lastSendAttemptTime;
    private DateTime lastSendTime;
    private DateTime lastReceiveTime;
    private long numberOfSendsSinceLastReceive;
    private DateTime firstSendSinceLastReceive;
    private readonly object nameLock = new object();
    private string name;
    private static int numberOfOpenTcpConnections;

    public Connection(
      Uri serverUri,
      string hostNameCertificateOverride,
      TimeSpan receiveHangDetectionTime,
      TimeSpan sendHangDetectionTime,
      TimeSpan idleTimeout,
      MemoryStreamPool memoryStreamPool,
      RemoteCertificateValidationCallback remoteCertificateValidationCallback = null)
    {
      this.serverUri = serverUri;
      this.hostNameCertificateOverride = hostNameCertificateOverride;
      this.BufferProvider = new BufferProvider();
      this.receiveDelayLimit = !(receiveHangDetectionTime <= Connection.receiveHangGracePeriod) ? receiveHangDetectionTime : throw new ArgumentOutOfRangeException(nameof (receiveHangDetectionTime), (object) receiveHangDetectionTime, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} must be greater than {1} ({2})", (object) nameof (receiveHangDetectionTime), (object) nameof (receiveHangGracePeriod), (object) Connection.receiveHangGracePeriod));
      this.sendDelayLimit = !(sendHangDetectionTime <= Connection.sendHangGracePeriod) ? sendHangDetectionTime : throw new ArgumentOutOfRangeException(nameof (sendHangDetectionTime), (object) sendHangDetectionTime, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} must be greater than {1} ({2})", (object) nameof (sendHangDetectionTime), (object) nameof (sendHangGracePeriod), (object) Connection.sendHangGracePeriod));
      this.lastSendAttemptTime = DateTime.MinValue;
      this.lastSendTime = DateTime.MinValue;
      this.lastReceiveTime = DateTime.MinValue;
      if (idleTimeout > TimeSpan.Zero)
      {
        this.idleConnectionTimeout = idleTimeout;
        this.idleConnectionClosureTimeout = this.idleConnectionTimeout + TimeSpan.FromTicks(2L * (sendHangDetectionTime.Ticks + receiveHangDetectionTime.Ticks));
      }
      this.name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<not connected> -> {0}", (object) this.serverUri);
      this.memoryStreamPool = memoryStreamPool;
      this.remoteCertificateValidationCallback = remoteCertificateValidationCallback;
    }

    public static int NumberOfOpenTcpConnections => Connection.numberOfOpenTcpConnections;

    public BufferProvider BufferProvider { get; }

    public Uri ServerUri => this.serverUri;

    public bool Healthy
    {
      get
      {
        this.ThrowIfDisposed();
        if (this.tcpClient == null)
          return false;
        DateTime utcNow = DateTime.UtcNow;
        DateTime lastSendAttempt;
        DateTime lastSend;
        DateTime lastReceive;
        DateTime? firstSendSinceLastReceive;
        long numberOfSendsSinceLastReceive;
        this.SnapshotConnectionTimestamps(out lastSendAttempt, out lastSend, out lastReceive, out firstSendSinceLastReceive, out numberOfSendsSinceLastReceive);
        if (utcNow - lastReceive < Connection.recentReceiveWindow)
          return true;
        if (lastSendAttempt - lastSend > this.sendDelayLimit && utcNow - lastSendAttempt > Connection.sendHangGracePeriod)
        {
          DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Hung send: {0}. Last send attempt: {1:o}. Last send: {2:o}. Tolerance {3:c}", (object) this, (object) lastSendAttempt, (object) lastSend, (object) this.sendDelayLimit);
          return false;
        }
        if (lastSend - lastReceive > this.receiveDelayLimit)
        {
          if (!(utcNow - lastSend > Connection.receiveHangGracePeriod))
          {
            if (numberOfSendsSinceLastReceive >= 3L && firstSendSinceLastReceive.HasValue)
            {
              DateTime dateTime = utcNow;
              DateTime? nullable1 = firstSendSinceLastReceive;
              TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(dateTime - nullable1.GetValueOrDefault()) : new TimeSpan?();
              TimeSpan receiveHangGracePeriod = Connection.receiveHangGracePeriod;
              if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > receiveHangGracePeriod ? 1 : 0) : 0) == 0)
                goto label_11;
            }
            else
              goto label_11;
          }
          DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Replies not getting back: {0}. Last send: {1:o}. Last receive: {2:o}. Tolerance: {3:c}. First send since last receieve: {4:o}. # of sends since last receive: {5}", (object) this, (object) lastSend, (object) lastReceive, (object) this.receiveDelayLimit, (object) firstSendSinceLastReceive, (object) numberOfSendsSinceLastReceive);
          return false;
        }
label_11:
        if (this.idleConnectionTimeout > TimeSpan.Zero && utcNow - lastReceive > this.idleConnectionTimeout)
          return false;
        try
        {
          Socket client = this.tcpClient.Client;
          if (client == null || !client.Connected)
            return false;
          client.Send(Connection.healthCheckBuffer, 0, SocketFlags.None);
          return true;
        }
        catch (SocketException ex)
        {
          bool healthy = ex.SocketErrorCode == SocketError.WouldBlock;
          if (!healthy)
            DefaultTrace.TraceWarning("Unhealthy RNTBD connection. Socket error code: {0}", (object) ex.SocketErrorCode.ToString());
          return healthy;
        }
        catch (ObjectDisposedException ex)
        {
          return false;
        }
      }
    }

    public bool Disposed => this.disposed;

    public async Task OpenAsync(ChannelOpenArguments args)
    {
      this.ThrowIfDisposed();
      await this.OpenSocketAsync(args);
      await this.NegotiateSslAsync(args);
    }

    public async Task WriteRequestAsync(
      ChannelCommonArguments args,
      TransportSerialization.SerializedRequest messagePayload,
      TransportRequestStats transportRequestStats)
    {
      this.ThrowIfDisposed();
      if (transportRequestStats != null)
      {
        DateTime lastSendAttempt;
        DateTime lastSend;
        DateTime lastReceive;
        this.SnapshotConnectionTimestamps(out lastSendAttempt, out lastSend, out lastReceive, out DateTime? _, out long _);
        transportRequestStats.ConnectionLastSendAttemptTime = new DateTime?(lastSendAttempt);
        transportRequestStats.ConnectionLastSendTime = new DateTime?(lastSend);
        transportRequestStats.ConnectionLastReceiveTime = new DateTime?(lastReceive);
      }
      args.SetTimeoutCode(TransportErrorCode.SendLockTimeout);
      await this.writeSemaphore.WaitAsync();
      try
      {
        args.SetTimeoutCode(TransportErrorCode.SendTimeout);
        args.SetPayloadSent();
        this.UpdateLastSendAttemptTime();
        await messagePayload.CopyToStreamAsync(this.stream);
      }
      finally
      {
        this.writeSemaphore.Release();
      }
      this.UpdateLastSendTime();
    }

    [SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
    public async Task<Connection.ResponseMetadata> ReadResponseMetadataAsync(
      ChannelCommonArguments args)
    {
      Connection connection = this;
      connection.ThrowIfDisposed();
      System.Diagnostics.Trace.CorrelationManager.ActivityId = args.ActivityId;
      int metadataHeaderLength = 24;
      BufferProvider.DisposableBuffer header = connection.BufferProvider.GetBuffer(metadataHeaderLength);
      await connection.ReadPayloadAsync(header.Buffer.Array, metadataHeaderLength, "header", args);
      uint uint32 = BitConverter.ToUInt32(header.Buffer.Array, 0);
      if (uint32 > (uint) int.MaxValue)
      {
        header.Dispose();
        DefaultTrace.TraceCritical("RNTBD header length says {0} but expected at most {1} bytes. Connection: {2}", (object) uint32, (object) int.MaxValue, (object) connection);
        throw TransportExceptions.GetInternalServerErrorException(connection.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseHeaderTooLargeError, (object) uint32, (object) connection));
      }
      if ((long) uint32 < (long) metadataHeaderLength)
      {
        DefaultTrace.TraceCritical("Invalid RNTBD header length {0} bytes. Expected at least {1} bytes. Connection: {2}", (object) uint32, (object) metadataHeaderLength, (object) connection);
        throw TransportExceptions.GetInternalServerErrorException(connection.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseInvalidHeaderLengthError, (object) metadataHeaderLength, (object) uint32, (object) connection));
      }
      int num = (int) uint32 - metadataHeaderLength;
      BufferProvider.DisposableBuffer metadata = connection.BufferProvider.GetBuffer(num);
      await connection.ReadPayloadAsync(metadata.Buffer.Array, num, "metadata", args);
      Connection.ResponseMetadata responseMetadata = new Connection.ResponseMetadata(header, metadata);
      header = new BufferProvider.DisposableBuffer();
      metadata = new BufferProvider.DisposableBuffer();
      return responseMetadata;
    }

    public async Task<MemoryStream> ReadResponseBodyAsync(ChannelCommonArguments args)
    {
      Connection connection1 = this;
      connection1.ThrowIfDisposed();
      System.Diagnostics.Trace.CorrelationManager.ActivityId = args.ActivityId;
      using (BufferProvider.DisposableBuffer bodyLengthHeader = connection1.BufferProvider.GetBuffer(4))
      {
        Connection connection2 = connection1;
        ArraySegment<byte> buffer = bodyLengthHeader.Buffer;
        byte[] array = buffer.Array;
        ChannelCommonArguments args1 = args;
        await connection2.ReadPayloadAsync(array, 4, "body length header", args1);
        buffer = bodyLengthHeader.Buffer;
        uint uint32 = BitConverter.ToUInt32(buffer.Array, 0);
        if (uint32 > (uint) int.MaxValue)
        {
          DefaultTrace.TraceCritical("Invalid RNTBD response body length {0} bytes. Connection: {1}", (object) uint32, (object) connection1);
          throw TransportExceptions.GetInternalServerErrorException(connection1.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseBodyTooLargeError, (object) uint32, (object) connection1));
        }
        MemoryStream memoryStream = (MemoryStream) null;
        MemoryStreamPool memoryStreamPool = connection1.memoryStreamPool;
        if ((memoryStreamPool != null ? (memoryStreamPool.TryGetMemoryStream((int) uint32, out memoryStream) ? 1 : 0) : 0) != 0)
        {
          await connection1.ReadPayloadAsync(memoryStream, (int) uint32, "body", args);
          memoryStream.Position = 0L;
          return memoryStream;
        }
        byte[] body = new byte[(int) uint32];
        await connection1.ReadPayloadAsync(body, (int) uint32, "body", args);
        return StreamExtension.CreateExportableMemoryStream(body);
      }
    }

    public override string ToString()
    {
      lock (this.nameLock)
        return this.name;
    }

    public void Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      string connectionTimestampsText = this.GetConnectionTimestampsText();
      if (this.tcpClient != null)
      {
        DefaultTrace.TraceInformation("Disposing RNTBD connection {0} -> {1} to server {2}. {3}", (object) this.localEndPoint, (object) this.remoteEndPoint, (object) this.serverUri, (object) connectionTimestampsText);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<disconnected> {0} -> {1}", (object) this.localEndPoint, (object) this.remoteEndPoint);
        lock (this.nameLock)
          this.name = str;
      }
      else
        DefaultTrace.TraceInformation("Disposing unused RNTBD connection to server {0}. {1}", (object) this.serverUri, (object) connectionTimestampsText);
      if (this.tcpClient == null)
        return;
      if (this.portPool != null)
        this.portPool.RemoveReference(this.localEndPoint.AddressFamily, checked ((ushort) this.localEndPoint.Port));
      this.tcpClient.Close();
      Interlocked.Decrement(ref Connection.numberOfOpenTcpConnections);
      this.tcpClient = (TcpClient) null;
      this.stream.Close();
      this.streamReader?.Dispose();
      TransportClient.GetTransportPerformanceCounters().IncrementRntbdConnectionClosedCount();
    }

    public bool IsActive(out TimeSpan timeToIdle)
    {
      this.ThrowIfDisposed();
      DateTime lastReceive;
      this.SnapshotConnectionTimestamps(out DateTime _, out DateTime _, out lastReceive, out DateTime? _, out long _);
      DateTime utcNow = DateTime.UtcNow;
      if (utcNow - lastReceive > this.idleConnectionTimeout)
      {
        timeToIdle = this.idleConnectionClosureTimeout;
        return false;
      }
      timeToIdle = lastReceive + this.idleConnectionClosureTimeout - utcNow;
      return true;
    }

    internal TimeSpan TestIdleConnectionClosureTimeout => this.idleConnectionClosureTimeout;

    internal void TestSetLastReceiveTime(DateTime lrt)
    {
      lock (this.timestampLock)
        this.lastReceiveTime = lrt;
    }

    private static uint GetUInt32FromEnvironmentVariableOrDefault(
      string name,
      uint minValue,
      uint maxValue,
      uint defaultValue)
    {
      string environmentVariable = Environment.GetEnvironmentVariable(name);
      uint result;
      if (string.IsNullOrEmpty(environmentVariable) || !uint.TryParse(environmentVariable, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return defaultValue;
      if (result > maxValue || result < minValue)
        throw new ArgumentOutOfRangeException(name, string.Format("Value for environment variable '{0}' is outside expected range of {1} - {2}.", (object) name, (object) minValue, (object) maxValue));
      return result;
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(string.Format("{0}:{1}", (object) nameof (Connection), (object) this.serverUri));
    }

    private async Task OpenSocketAsync(ChannelOpenArguments args)
    {
      Connection connection = this;
      if (connection.tcpClient != null)
        throw new InvalidOperationException("Attempting to call Connection.OpenSocketAsync on an " + string.Format("already initialized connection {0}", (object) connection));
      TcpClient tcpClient = (TcpClient) null;
      TransportErrorCode errorCode = TransportErrorCode.Unknown;
      try
      {
        errorCode = TransportErrorCode.DnsResolutionFailed;
        args.CommonArguments.SetTimeoutCode(TransportErrorCode.DnsResolutionTimeout);
        IPAddress ipAddress = await Connection.ResolveHostAsync(connection.serverUri.DnsSafeHost);
        errorCode = TransportErrorCode.ConnectFailed;
        args.CommonArguments.SetTimeoutCode(TransportErrorCode.ConnectTimeout);
        connection.UpdateLastSendAttemptTime();
        DefaultTrace.TraceInformation("Port reuse mode: {0}. Connection: {1}", (object) args.PortReuseMode, (object) connection);
        switch (args.PortReuseMode)
        {
          case PortReuseMode.ReuseUnicastPort:
            tcpClient = await Connection.ConnectUnicastPortAsync(connection.serverUri, ipAddress);
            break;
          case PortReuseMode.PrivatePortPool:
            Tuple<TcpClient, bool> tuple = await Connection.ConnectUserPortAsync(connection.serverUri, ipAddress, args.PortPool, connection.ToString());
            tcpClient = tuple.Item1;
            if (tuple.Item2)
            {
              connection.portPool = args.PortPool;
              break;
            }
            DefaultTrace.TraceInformation("PrivatePortPool: Configured but actually not used. Connection: {0}", (object) connection);
            break;
          default:
            throw new ArgumentException(string.Format("Unsupported port reuse policy {0}", (object) args.PortReuseMode.ToString()));
        }
        connection.UpdateLastSendTime();
        connection.UpdateLastReceiveTime();
        args.OpenTimeline.RecordConnectFinishTime();
        DefaultTrace.TraceInformation("RNTBD connection established {0} -> {1}", (object) tcpClient.Client.LocalEndPoint, (object) tcpClient.Client.RemoteEndPoint);
        TransportClient.GetTransportPerformanceCounters().IncrementRntbdConnectionEstablishedCount();
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -> {1}", (object) tcpClient.Client.LocalEndPoint, (object) tcpClient.Client.RemoteEndPoint);
        lock (connection.nameLock)
          connection.name = str;
      }
      catch (Exception ex)
      {
        tcpClient?.Close();
        DefaultTrace.TraceInformation("Connection.OpenSocketAsync failed. Converting to TransportException. Connection: {0}. Inner exception: {1}", (object) connection, (object) ex);
        throw new TransportException(errorCode, ex, args.CommonArguments.ActivityId, connection.serverUri, connection.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
      }
      connection.localEndPoint = (IPEndPoint) tcpClient.Client.LocalEndPoint;
      connection.remoteEndPoint = (IPEndPoint) tcpClient.Client.RemoteEndPoint;
      connection.tcpClient = tcpClient;
      connection.stream = (Stream) tcpClient.GetStream();
      Interlocked.Increment(ref Connection.numberOfOpenTcpConnections);
      connection.tcpClient.Client.Blocking = false;
      tcpClient = (TcpClient) null;
    }

    private async Task NegotiateSslAsync(ChannelOpenArguments args)
    {
      Connection connection = this;
      string targetHost = connection.hostNameCertificateOverride ?? connection.serverUri.DnsSafeHost;
      SslStream sslStream = new SslStream(connection.stream, false, connection.remoteCertificateValidationCallback);
      try
      {
        args.CommonArguments.SetTimeoutCode(TransportErrorCode.SslNegotiationTimeout);
        connection.UpdateLastSendAttemptTime();
        await sslStream.AuthenticateAsClientAsync(targetHost, (X509CertificateCollection) null, SslProtocols.Tls12, false);
        connection.UpdateLastSendTime();
        connection.UpdateLastReceiveTime();
        args.OpenTimeline.RecordSslHandshakeFinishTime();
        connection.stream = (Stream) sslStream;
        connection.streamReader = new RntbdStreamReader(connection.stream);
        DefaultTrace.TraceInformation("RNTBD SSL handshake complete {0} -> {1}", (object) connection.localEndPoint, (object) connection.remoteEndPoint);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("Connection.NegotiateSslAsync failed. Converting to TransportException. Connection: {0}. Inner exception: {1}", (object) connection, (object) ex);
        throw new TransportException(TransportErrorCode.SslNegotiationFailed, ex, args.CommonArguments.ActivityId, connection.serverUri, connection.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
      }
      sslStream = (SslStream) null;
    }

    private async Task ReadPayloadAsync(
      byte[] payload,
      int length,
      string type,
      ChannelCommonArguments args)
    {
      int read;
      for (int bytesRead = 0; bytesRead < length; bytesRead += read)
      {
        read = 0;
        try
        {
          read = await this.streamReader.ReadAsync(payload, bytesRead, length - bytesRead);
        }
        catch (IOException ex)
        {
          this.TraceAndThrowReceiveFailedException(ex, type, args);
        }
        if (read == 0)
          this.TraceAndThrowEndOfStream(type, args);
        this.UpdateLastReceiveTime();
      }
    }

    private async Task ReadPayloadAsync(
      MemoryStream payload,
      int length,
      string type,
      ChannelCommonArguments args)
    {
      int read;
      for (int bytesRead = 0; bytesRead < length; bytesRead += read)
      {
        read = 0;
        try
        {
          read = await this.streamReader.ReadAsync(payload, length - bytesRead);
        }
        catch (IOException ex)
        {
          this.TraceAndThrowReceiveFailedException(ex, type, args);
        }
        if (read == 0)
          this.TraceAndThrowEndOfStream(type, args);
        this.UpdateLastReceiveTime();
      }
    }

    private void TraceAndThrowReceiveFailedException(
      IOException e,
      string type,
      ChannelCommonArguments args)
    {
      DefaultTrace.TraceError("Hit IOException {0} with HResult {1} while reading {2} on connection {3}. {4}", (object) e.Message, (object) e.HResult, (object) type, (object) this, (object) this.GetConnectionTimestampsText());
      throw new TransportException(TransportErrorCode.ReceiveFailed, (Exception) e, args.ActivityId, this.serverUri, this.ToString(), args.UserPayload, true);
    }

    private void TraceAndThrowEndOfStream(string type, ChannelCommonArguments args)
    {
      DefaultTrace.TraceError("Reached end of stream. Read 0 bytes while reading {0} on connection {1}. {2}", (object) type, (object) this, (object) this.GetConnectionTimestampsText());
      throw new TransportException(TransportErrorCode.ReceiveStreamClosed, (Exception) null, args.ActivityId, this.serverUri, this.ToString(), args.UserPayload, true);
    }

    private void SnapshotConnectionTimestamps(
      out DateTime lastSendAttempt,
      out DateTime lastSend,
      out DateTime lastReceive,
      out DateTime? firstSendSinceLastReceive,
      out long numberOfSendsSinceLastReceive)
    {
      lock (this.timestampLock)
      {
        lastSendAttempt = this.lastSendAttemptTime;
        lastSend = this.lastSendTime;
        lastReceive = this.lastReceiveTime;
        firstSendSinceLastReceive = this.lastReceiveTime < this.firstSendSinceLastReceive ? new DateTime?(this.firstSendSinceLastReceive) : new DateTime?();
        numberOfSendsSinceLastReceive = this.numberOfSendsSinceLastReceive;
      }
    }

    private string GetConnectionTimestampsText()
    {
      DateTime lastSendAttempt;
      DateTime lastSend;
      DateTime lastReceive;
      DateTime? firstSendSinceLastReceive;
      long numberOfSendsSinceLastReceive;
      this.SnapshotConnectionTimestamps(out lastSendAttempt, out lastSend, out lastReceive, out firstSendSinceLastReceive, out numberOfSendsSinceLastReceive);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Last send attempt time: {0:o}. Last send time: {1:o}. Last receive time: {2:o}. First sends since last receieve: {3:o}. # of sends since last receive: {4}", (object) lastSendAttempt, (object) lastSend, (object) lastReceive, (object) firstSendSinceLastReceive, (object) numberOfSendsSinceLastReceive);
    }

    private void UpdateLastSendAttemptTime()
    {
      lock (this.timestampLock)
        this.lastSendAttemptTime = DateTime.UtcNow;
    }

    private void UpdateLastSendTime()
    {
      lock (this.timestampLock)
      {
        this.lastSendTime = DateTime.UtcNow;
        if (this.numberOfSendsSinceLastReceive++ != 0L)
          return;
        this.firstSendSinceLastReceive = this.lastSendTime;
      }
    }

    private void UpdateLastReceiveTime()
    {
      lock (this.timestampLock)
      {
        this.numberOfSendsSinceLastReceive = 0L;
        this.lastReceiveTime = DateTime.UtcNow;
      }
    }

    private static async Task<TcpClient> ConnectUnicastPortAsync(
      Uri serverUri,
      IPAddress resolvedAddress)
    {
      TcpClient tcpClient = new TcpClient(resolvedAddress.AddressFamily);
      Connection.SetCommonSocketOptions(tcpClient.Client);
      Connection.SetReuseUnicastPort(tcpClient.Client);
      DefaultTrace.TraceInformation("RNTBD: {0} connecting to {1} (address {2})", (object) nameof (ConnectUnicastPortAsync), (object) serverUri, (object) resolvedAddress);
      await tcpClient.ConnectAsync(resolvedAddress, serverUri.Port);
      TcpClient tcpClient1 = tcpClient;
      tcpClient = (TcpClient) null;
      return tcpClient1;
    }

    private static async Task<Tuple<TcpClient, bool>> ConnectReuseAddrAsync(
      Uri serverUri,
      IPAddress address,
      ushort candidatePort)
    {
      TcpClient candidateClient = new TcpClient(address.AddressFamily);
      TcpClient tcpClient = (TcpClient) null;
      try
      {
        Connection.SetCommonSocketOptions(candidateClient.Client);
        candidateClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        EndPoint localEP;
        switch (address.AddressFamily)
        {
          case AddressFamily.InterNetwork:
            localEP = (EndPoint) new IPEndPoint(IPAddress.Any, (int) candidatePort);
            break;
          case AddressFamily.InterNetworkV6:
            localEP = (EndPoint) new IPEndPoint(IPAddress.IPv6Any, (int) candidatePort);
            break;
          default:
            throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Address family {0} not supported", (object) address.AddressFamily));
        }
        DefaultTrace.TraceInformation("RNTBD: {0} binding local endpoint {1}", (object) nameof (ConnectReuseAddrAsync), (object) localEP);
        try
        {
          candidateClient.Client.Bind(localEP);
        }
        catch (SocketException ex)
        {
          if (ex.SocketErrorCode == SocketError.AccessDenied)
            return Tuple.Create<TcpClient, bool>((TcpClient) null, false);
          throw;
        }
        DefaultTrace.TraceInformation("RNTBD: {0} connecting to {1} (address {2})", (object) nameof (ConnectReuseAddrAsync), (object) serverUri, (object) address);
        try
        {
          await candidateClient.ConnectAsync(address, serverUri.Port);
        }
        catch (SocketException ex)
        {
          if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            return Tuple.Create<TcpClient, bool>((TcpClient) null, true);
          throw;
        }
        tcpClient = candidateClient;
        candidateClient = (TcpClient) null;
      }
      finally
      {
        candidateClient?.Close();
      }
      return Tuple.Create<TcpClient, bool>(tcpClient, true);
    }

    private static async Task<Tuple<TcpClient, bool>> ConnectUserPortAsync(
      Uri serverUri,
      IPAddress address,
      UserPortPool portPool,
      string connectionName)
    {
      ushort[] candidatePorts = portPool.GetCandidatePorts(address.AddressFamily);
      if (candidatePorts != null)
      {
        ushort[] numArray = candidatePorts;
        for (int index = 0; index < numArray.Length; ++index)
        {
          ushort candidatePort = numArray[index];
          Tuple<TcpClient, bool> tuple = await Connection.ConnectReuseAddrAsync(serverUri, address, candidatePort);
          TcpClient tcpClient = tuple.Item1;
          bool flag = tuple.Item2;
          if (tcpClient != null)
          {
            portPool.AddReference(address.AddressFamily, checked ((ushort) ((IPEndPoint) tcpClient.Client.LocalEndPoint).Port));
            return Tuple.Create<TcpClient, bool>(tcpClient, true);
          }
          if (!flag)
            portPool.MarkUnusable(address.AddressFamily, candidatePort);
        }
        numArray = (ushort[]) null;
        DefaultTrace.TraceInformation("PrivatePortPool: All {0} candidate ports have been tried but none connects. Connection: {1}", (object) candidatePorts.Length, (object) connectionName);
      }
      TcpClient tcpClient1 = (await Connection.ConnectReuseAddrAsync(serverUri, address, (ushort) 0)).Item1;
      if (tcpClient1 != null)
      {
        portPool.AddReference(address.AddressFamily, checked ((ushort) ((IPEndPoint) tcpClient1.Client.LocalEndPoint).Port));
        return Tuple.Create<TcpClient, bool>(tcpClient1, true);
      }
      DefaultTrace.TraceInformation("PrivatePortPool: Not enough reusable ports in the system or pool. Have to connect unicast port. Pool status: {0}. Connection: {1}", (object) portPool.DumpStatus(), (object) connectionName);
      return Tuple.Create<TcpClient, bool>(await Connection.ConnectUnicastPortAsync(serverUri, address), false);
    }

    private static async Task<IPAddress> ResolveHostAsync(string hostName)
    {
      IPAddress[] hostAddressesAsync = await Dns.GetHostAddressesAsync(hostName);
      int index = 0;
      if (hostAddressesAsync.Length > 1)
        index = Connection.rng.Value.Next(hostAddressesAsync.Length);
      return hostAddressesAsync[index];
    }

    private static void SetCommonSocketOptions(Socket clientSocket)
    {
      clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
      Connection.EnableTcpKeepAlive(clientSocket);
    }

    private static void EnableTcpKeepAlive(Socket clientSocket)
    {
      clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        try
        {
          clientSocket.IOControl(IOControlCode.KeepAliveValues, Connection.keepAliveConfiguration.Value, (byte[]) null);
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceWarning("IOControl(KeepAliveValues) failed: {0}", (object) ex);
        }
      }
      else
        Connection.SetKeepAliveSocketOptions(clientSocket);
    }

    private static void SetKeepAliveSocketOptions(Socket clientSocket)
    {
      if (!Connection.isKeepAliveCustomizationSupported.Value)
        return;
      clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, (object) Connection.SocketOptionTcpKeepAliveInterval);
      clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, (object) Connection.SocketOptionTcpKeepAliveTime);
    }

    private static bool IsKeepAliveCustomizationSupported()
    {
      try
      {
        using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
        {
          socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, (object) Connection.SocketOptionTcpKeepAliveInterval);
          socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, (object) Connection.SocketOptionTcpKeepAliveTime);
          return true;
        }
      }
      catch
      {
        return false;
      }
    }

    private static byte[] GetWindowsKeepAliveConfiguration()
    {
      byte[] aliveConfiguration = new byte[12];
      BitConverter.GetBytes(1U).CopyTo((Array) aliveConfiguration, 0);
      BitConverter.GetBytes(30000U).CopyTo((Array) aliveConfiguration, 4);
      BitConverter.GetBytes(1000U).CopyTo((Array) aliveConfiguration, 8);
      return aliveConfiguration;
    }

    private static void SetReuseUnicastPort(Socket clientSocket)
    {
      if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        return;
      try
      {
        clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseUnicastPort, true);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("SetSocketOption(Socket, ReuseUnicastPort) failed: {0}", (object) ex);
      }
    }

    public sealed class ResponseMetadata : IDisposable
    {
      private bool disposed;
      private BufferProvider.DisposableBuffer header;
      private BufferProvider.DisposableBuffer metadata;

      public ResponseMetadata(
        BufferProvider.DisposableBuffer header,
        BufferProvider.DisposableBuffer metadata)
      {
        this.header = header;
        this.metadata = metadata;
        this.disposed = false;
      }

      public ArraySegment<byte> Header => this.header.Buffer;

      public ArraySegment<byte> Metadata => this.metadata.Buffer;

      public void Dispose()
      {
        if (this.disposed)
          return;
        this.header.Dispose();
        this.metadata.Dispose();
        this.disposed = true;
      }
    }
  }
}
