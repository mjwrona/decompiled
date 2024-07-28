// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.Connection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
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
    private const int ResponseLengthByteLimit = 2147483647;
    private const SslProtocols TlsProtocols = SslProtocols.Tls12;
    private static readonly Lazy<ConcurrentPrng> rng = new Lazy<ConcurrentPrng>(LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<byte[]> keepAliveConfiguration = new Lazy<byte[]>(new Func<byte[]>(Connection.GetWindowsKeepAliveConfiguration), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly byte[] healthCheckBuffer = new byte[1];
    private static readonly TimeSpan recentReceiveWindow = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan sendHangGracePeriod = TimeSpan.FromSeconds(2.0);
    private static readonly TimeSpan receiveHangGracePeriod = TimeSpan.FromSeconds(10.0);
    private readonly Uri serverUri;
    private readonly string hostNameCertificateOverride;
    private readonly TimeSpan receiveDelayLimit;
    private readonly TimeSpan sendDelayLimit;
    private bool disposed;
    private TcpClient tcpClient;
    private UserPortPool portPool;
    private readonly TimeSpan idleConnectionTimeout;
    private readonly TimeSpan idleConnectionClosureTimeout;
    private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1);
    private Stream stream;
    private readonly object timestampLock = new object();
    private DateTime lastSendAttemptTime;
    private DateTime lastSendTime;
    private DateTime lastReceiveTime;
    private readonly object nameLock = new object();
    private string name;

    public Connection(
      Uri serverUri,
      string hostNameCertificateOverride,
      TimeSpan receiveHangDetectionTime,
      TimeSpan sendHangDetectionTime,
      TimeSpan idleTimeout)
    {
      this.serverUri = serverUri;
      this.hostNameCertificateOverride = hostNameCertificateOverride;
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
    }

    public Uri ServerUri => this.serverUri;

    public bool Healthy
    {
      get
      {
        this.ThrowIfDisposed();
        if (this.tcpClient == null)
          return false;
        DateTime lastSendAttempt;
        DateTime lastSend;
        DateTime lastReceive;
        this.SnapshotConnectionTimestamps(out lastSendAttempt, out lastSend, out lastReceive);
        DateTime utcNow = DateTime.UtcNow;
        if (utcNow - lastReceive < Connection.recentReceiveWindow)
          return true;
        if (lastSendAttempt - lastSend > this.sendDelayLimit && utcNow - lastSendAttempt > Connection.sendHangGracePeriod)
        {
          DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Hung send: {0}. Last send attempt: {1:o}. Last send: {2:o}. Tolerance {3:c}", (object) this, (object) lastSendAttempt, (object) lastSend, (object) this.sendDelayLimit);
          return false;
        }
        if (lastSend - lastReceive > this.receiveDelayLimit && utcNow - lastSend > Connection.receiveHangGracePeriod)
        {
          DefaultTrace.TraceWarning("Unhealthy RNTBD connection: Replies not getting back: {0}. Last send: {1:o}. Last receive: {2:o}. Tolerance: {3:c}", (object) this, (object) lastSend, (object) lastReceive, (object) this.receiveDelayLimit);
          return false;
        }
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

    public async Task WriteRequestAsync(ChannelCommonArguments args, byte[] messagePayload)
    {
      this.ThrowIfDisposed();
      args.SetTimeoutCode(TransportErrorCode.SendLockTimeout);
      await this.writeSemaphore.WaitAsync();
      try
      {
        args.SetTimeoutCode(TransportErrorCode.SendTimeout);
        args.SetPayloadSent();
        this.UpdateLastSendAttemptTime();
        await this.stream.WriteAsync(messagePayload, 0, messagePayload.Length);
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
      Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = args.ActivityId;
      byte[] header = await connection.ReadPayloadAsync(24, "header", args);
      uint uint32 = BitConverter.ToUInt32(header, 0);
      if (uint32 > (uint) int.MaxValue)
      {
        DefaultTrace.TraceCritical("RNTBD header length says {0} but expected at most {1} bytes. Connection: {2}", (object) uint32, (object) int.MaxValue, (object) connection);
        throw TransportExceptions.GetInternalServerErrorException(connection.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseHeaderTooLargeError, (object) uint32, (object) connection));
      }
      if ((long) uint32 < (long) header.Length)
      {
        DefaultTrace.TraceCritical("Invalid RNTBD header length {0} bytes. Expected at least {1} bytes. Connection: {2}", (object) uint32, (object) header.Length, (object) connection);
        throw TransportExceptions.GetInternalServerErrorException(connection.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseInvalidHeaderLengthError, (object) header.Length, (object) uint32, (object) connection));
      }
      int length = (int) uint32 - header.Length;
      return new Connection.ResponseMetadata(header, await connection.ReadPayloadAsync(length, "metadata", args));
    }

    public async Task<byte[]> ReadResponseBodyAsync(ChannelCommonArguments args)
    {
      Connection connection = this;
      connection.ThrowIfDisposed();
      Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId = args.ActivityId;
      uint uint32 = BitConverter.ToUInt32(await connection.ReadPayloadAsync(4, "body length header", args), 0);
      if (uint32 > (uint) int.MaxValue)
      {
        DefaultTrace.TraceCritical("Invalid RNTBD response body length {0} bytes. Connection: {1}", (object) uint32, (object) connection);
        throw TransportExceptions.GetInternalServerErrorException(connection.serverUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ServerResponseBodyTooLargeError, (object) uint32, (object) connection));
      }
      return await connection.ReadPayloadAsync((int) uint32, "body", args);
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
        DefaultTrace.TraceInformation("Disposing RNTBD connection {0} -> {1} to server {2}. {3}", (object) this.tcpClient.Client.LocalEndPoint, (object) this.tcpClient.Client.RemoteEndPoint, (object) this.serverUri, (object) connectionTimestampsText);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<disconnected> {0} -> {1}", (object) this.tcpClient.Client.LocalEndPoint, (object) this.tcpClient.Client.RemoteEndPoint);
        lock (this.nameLock)
          this.name = str;
      }
      else
        DefaultTrace.TraceInformation("Disposing unused RNTBD connection to server {0}. {1}", (object) this.serverUri, (object) connectionTimestampsText);
      if (this.tcpClient == null)
        return;
      if (this.portPool != null)
      {
        IPEndPoint localEndPoint = (IPEndPoint) this.tcpClient.Client.LocalEndPoint;
        this.portPool.RemoveReference(localEndPoint.AddressFamily, checked ((ushort) localEndPoint.Port));
      }
      CustomTypeExtensions.Close(this.tcpClient);
      this.tcpClient = (TcpClient) null;
      CustomTypeExtensions.Close(this.stream);
      TransportClient.GetTransportPerformanceCounters().IncrementRntbdConnectionClosedCount();
    }

    public bool IsActive(out TimeSpan timeToIdle)
    {
      this.ThrowIfDisposed();
      DateTime lastReceive;
      this.SnapshotConnectionTimestamps(out DateTime _, out DateTime _, out lastReceive);
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
        switch (args.PortReuseMode)
        {
          case PortReuseMode.ReuseUnicastPort:
            tcpClient = await Connection.ConnectUnicastPortAsync(connection.serverUri, ipAddress);
            break;
          case PortReuseMode.PrivatePortPool:
            Tuple<TcpClient, bool> tuple = await Connection.ConnectUserPortAsync(connection.serverUri, ipAddress, args.PortPool);
            tcpClient = tuple.Item1;
            if (tuple.Item2)
            {
              connection.portPool = args.PortPool;
              break;
            }
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
        TcpClient tcpClient1 = tcpClient;
        if (tcpClient1 != null)
          CustomTypeExtensions.Close(tcpClient1);
        DefaultTrace.TraceInformation("Connection.OpenSocketAsync failed. Converting to TransportException. Connection: {0}. Inner exception: {1}", (object) connection, (object) ex);
        throw new TransportException(errorCode, ex, args.CommonArguments.ActivityId, connection.serverUri, connection.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
      }
      connection.tcpClient = tcpClient;
      connection.stream = (Stream) tcpClient.GetStream();
      connection.tcpClient.Client.Blocking = false;
    }

    private async Task NegotiateSslAsync(ChannelOpenArguments args)
    {
      Connection connection = this;
      string targetHost = connection.hostNameCertificateOverride ?? connection.serverUri.DnsSafeHost;
      SslStream sslStream = new SslStream(connection.stream, false);
      try
      {
        args.CommonArguments.SetTimeoutCode(TransportErrorCode.SslNegotiationTimeout);
        connection.UpdateLastSendAttemptTime();
        await sslStream.AuthenticateAsClientAsync(targetHost, (X509CertificateCollection) null, SslProtocols.Tls12, false);
        connection.UpdateLastSendTime();
        connection.UpdateLastReceiveTime();
        args.OpenTimeline.RecordSslHandshakeFinishTime();
        connection.stream = (Stream) sslStream;
        DefaultTrace.TraceInformation("RNTBD SSL handshake complete {0} -> {1}", (object) connection.tcpClient.Client.LocalEndPoint, (object) connection.tcpClient.Client.RemoteEndPoint);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("Connection.NegotiateSslAsync failed. Converting to TransportException. Connection: {0}. Inner exception: {1}", (object) connection, (object) ex);
        throw new TransportException(TransportErrorCode.SslNegotiationFailed, ex, args.CommonArguments.ActivityId, connection.serverUri, connection.ToString(), args.CommonArguments.UserPayload, args.CommonArguments.PayloadSent);
      }
    }

    private async Task<byte[]> ReadPayloadAsync(
      int length,
      string type,
      ChannelCommonArguments args)
    {
      Connection connection = this;
      byte[] payload = new byte[length];
      int num;
      for (int bytesRead = 0; bytesRead < length; bytesRead += num)
      {
        try
        {
          num = await connection.stream.ReadAsync(payload, bytesRead, length - bytesRead);
        }
        catch (IOException ex)
        {
          DefaultTrace.TraceError("Hit IOException while reading {0} on connection {1}. {2}", (object) type, (object) connection, (object) connection.GetConnectionTimestampsText());
          throw new TransportException(TransportErrorCode.ReceiveFailed, (Exception) ex, args.ActivityId, connection.serverUri, connection.ToString(), args.UserPayload, true);
        }
        if (num == 0)
        {
          DefaultTrace.TraceError("Reached end of stream. Read 0 bytes while reading {0} on connection {1}. {2}", (object) type, (object) connection, (object) connection.GetConnectionTimestampsText());
          throw new TransportException(TransportErrorCode.ReceiveStreamClosed, (Exception) null, args.ActivityId, connection.serverUri, connection.ToString(), args.UserPayload, true);
        }
        connection.UpdateLastReceiveTime();
      }
      return payload;
    }

    private void SnapshotConnectionTimestamps(
      out DateTime lastSendAttempt,
      out DateTime lastSend,
      out DateTime lastReceive)
    {
      lock (this.timestampLock)
      {
        lastSendAttempt = this.lastSendAttemptTime;
        lastSend = this.lastSendTime;
        lastReceive = this.lastReceiveTime;
      }
    }

    private string GetConnectionTimestampsText()
    {
      DateTime lastSendAttempt;
      DateTime lastSend;
      DateTime lastReceive;
      this.SnapshotConnectionTimestamps(out lastSendAttempt, out lastSend, out lastReceive);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Last send attempt time: {0:o}. Last send time: {1:o}. Last receive time: {2:o}", (object) lastSendAttempt, (object) lastSend, (object) lastReceive);
    }

    private void UpdateLastSendAttemptTime()
    {
      lock (this.timestampLock)
        this.lastSendAttemptTime = DateTime.UtcNow;
    }

    private void UpdateLastSendTime()
    {
      lock (this.timestampLock)
        this.lastSendTime = DateTime.UtcNow;
    }

    private void UpdateLastReceiveTime()
    {
      lock (this.timestampLock)
        this.lastReceiveTime = DateTime.UtcNow;
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
      return tcpClient;
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
        if (candidateClient != null)
          CustomTypeExtensions.Close(candidateClient);
      }
      return Tuple.Create<TcpClient, bool>(tcpClient, true);
    }

    private static async Task<Tuple<TcpClient, bool>> ConnectUserPortAsync(
      Uri serverUri,
      IPAddress address,
      UserPortPool portPool)
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
      }
      TcpClient tcpClient1 = (await Connection.ConnectReuseAddrAsync(serverUri, address, (ushort) 0)).Item1;
      if (tcpClient1 == null)
        return Tuple.Create<TcpClient, bool>(await Connection.ConnectUnicastPortAsync(serverUri, address), false);
      portPool.AddReference(address.AddressFamily, checked ((ushort) ((IPEndPoint) tcpClient1.Client.LocalEndPoint).Port));
      return Tuple.Create<TcpClient, bool>(tcpClient1, true);
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

    private static void EnableTcpKeepAlive(Socket clientSocket) => clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

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
    }

    public struct ResponseMetadata
    {
      public byte[] Header;
      public byte[] Metadata;

      public ResponseMetadata(byte[] header, byte[] metadata)
      {
        this.Header = header;
        this.Metadata = metadata;
      }
    }
  }
}
