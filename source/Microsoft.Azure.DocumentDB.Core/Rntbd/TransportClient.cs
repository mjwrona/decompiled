// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.TransportClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class TransportClient : Microsoft.Azure.Documents.TransportClient, IDisposable
  {
    private static TransportPerformanceCounters transportPerformanceCounters = new TransportPerformanceCounters();
    private readonly TimerPool timerPool;
    private readonly TimerPool idleTimerPool;
    private readonly ChannelDictionary channelDictionary;
    private readonly CpuMonitor cpuMonitor;
    private bool disposed;
    private readonly object disableRntbdChannelLock = new object();
    private bool disableRntbdChannel;

    public TransportClient(TransportClient.Options clientOptions)
    {
      if (clientOptions == null)
        throw new ArgumentNullException(nameof (clientOptions));
      TransportClient.LogClientOptions(clientOptions);
      UserPortPool userPortPool = (UserPortPool) null;
      if (clientOptions.PortReuseMode == PortReuseMode.PrivatePortPool)
        userPortPool = new UserPortPool(clientOptions.PortPoolReuseThreshold, clientOptions.PortPoolBindAttempts);
      this.timerPool = new TimerPool((int) clientOptions.TimerPoolResolution.TotalSeconds);
      this.idleTimerPool = !(clientOptions.IdleTimeout > TimeSpan.Zero) ? (TimerPool) null : new TimerPool(30);
      this.channelDictionary = new ChannelDictionary(new ChannelProperties(clientOptions.UserAgent, clientOptions.CertificateHostNameOverride, clientOptions.ConnectionStateListener, this.timerPool, clientOptions.RequestTimeout, clientOptions.OpenTimeout, clientOptions.PortReuseMode, userPortPool, clientOptions.MaxChannels, clientOptions.PartitionCount, clientOptions.MaxRequestsPerChannel, clientOptions.ReceiveHangDetectionTime, clientOptions.SendHangDetectionTime, clientOptions.IdleTimeout, this.idleTimerPool, clientOptions.CallerId));
      if (!clientOptions.EnableCpuMonitor)
        return;
      this.cpuMonitor = new CpuMonitor();
      this.cpuMonitor.Start();
    }

    internal override async Task<StoreResponse> InvokeStoreAsync(
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      this.ThrowIfDisposed();
      Guid activityId = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId;
      if (!request.IsBodySeekableClonableAndCountable)
        throw new InternalServerErrorException();
      StoreResponse storeResponse = (StoreResponse) null;
      string operation = "Unknown operation";
      DateTime requestStartTime = DateTime.UtcNow;
      int transportResponseStatusCode = 0;
      try
      {
        TransportClient.IncrementCounters();
        operation = "GetChannel";
        IChannel channel = this.channelDictionary.GetChannel(physicalAddress);
        TransportClient.GetTransportPerformanceCounters().IncrementRntbdRequestCount(resourceOperation.resourceType, resourceOperation.operationType);
        operation = "RequestAsync";
        DocumentServiceRequest request1 = request;
        Uri physicalAddress1 = physicalAddress;
        ResourceOperation resourceOperation1 = resourceOperation;
        Guid activityId1 = activityId;
        storeResponse = await channel.RequestAsync(request1, physicalAddress1, resourceOperation1, activityId1);
      }
      catch (TransportException ex)
      {
        if (this.cpuMonitor != null)
          ex.SetCpuLoad(this.cpuMonitor.GetCpuLoad());
        transportResponseStatusCode = (int) ex.ErrorCode;
        ex.RequestStartTime = new DateTime?(requestStartTime);
        ex.RequestEndTime = new DateTime?(DateTime.UtcNow);
        ex.OperationType = resourceOperation.operationType;
        ex.ResourceType = resourceOperation.resourceType;
        TransportClient.GetTransportPerformanceCounters().IncrementRntbdResponseCount(resourceOperation.resourceType, resourceOperation.operationType, (int) ex.ErrorCode);
        DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", (object) operation, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        if (request.IsReadOnlyRequest)
        {
          DefaultTrace.TraceInformation("Converting to Gone (read-only request)");
          throw TransportExceptions.GetGoneException(physicalAddress, activityId, (Exception) ex);
        }
        if (!ex.UserRequestSent)
        {
          DefaultTrace.TraceInformation("Converting to Gone (write request, not sent)");
          throw TransportExceptions.GetGoneException(physicalAddress, activityId, (Exception) ex);
        }
        if (TransportException.IsTimeout(ex.ErrorCode))
        {
          DefaultTrace.TraceInformation("Converting to RequestTimeout");
          throw TransportExceptions.GetRequestTimeoutException(physicalAddress, activityId, (Exception) ex);
        }
        DefaultTrace.TraceInformation("Converting to ServiceUnavailable");
        throw TransportExceptions.GetServiceUnavailableException(physicalAddress, activityId, (Exception) ex);
      }
      catch (DocumentClientException ex)
      {
        transportResponseStatusCode = -1;
        DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", (object) operation, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        throw;
      }
      catch (Exception ex)
      {
        transportResponseStatusCode = -2;
        DefaultTrace.TraceInformation("{0} failed: RID: {1}, Resource Type: {2}, Op: {3}, Address: {4}, Exception: {5}", (object) operation, (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        throw;
      }
      finally
      {
        TransportClient.DecrementCounters();
        TransportClient.GetTransportPerformanceCounters().IncrementRntbdResponseCount(resourceOperation.resourceType, resourceOperation.operationType, transportResponseStatusCode);
        this.RaiseProtocolDowngradeRequest(storeResponse);
      }
      Microsoft.Azure.Documents.TransportClient.ThrowServerException(request.ResourceAddress, storeResponse, physicalAddress, activityId, request);
      return storeResponse;
    }

    public override void Dispose()
    {
      this.ThrowIfDisposed();
      this.disposed = true;
      this.channelDictionary.Dispose();
      if (this.cpuMonitor != null)
      {
        this.cpuMonitor.Stop();
        this.cpuMonitor.Dispose();
      }
      if (this.idleTimerPool != null)
        this.idleTimerPool.Dispose();
      this.timerPool.Dispose();
      base.Dispose();
      DefaultTrace.TraceInformation("Rntbd.TransportClient disposed.");
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (TransportClient));
    }

    private static void LogClientOptions(TransportClient.Options clientOptions) => DefaultTrace.TraceInformation("Creating RNTBD TransportClient with options {0}", (object) clientOptions.ToString());

    private static void IncrementCounters()
    {
    }

    private static void DecrementCounters()
    {
    }

    public event Action OnDisableRntbdChannel;

    private void RaiseProtocolDowngradeRequest(StoreResponse storeResponse)
    {
      if (storeResponse == null)
        return;
      string a = (string) null;
      if (!storeResponse.TryGetHeaderValue("x-ms-disable-rntbd-channel", out a) || !string.Equals(a, "true"))
        return;
      bool flag = false;
      lock (this.disableRntbdChannelLock)
      {
        if (this.disableRntbdChannel)
          return;
        this.disableRntbdChannel = true;
        flag = true;
      }
      if (!flag)
        return;
      Task.Factory.StartNewOnCurrentTaskSchedulerAsync((Action) (() =>
      {
        Action disableRntbdChannel = this.OnDisableRntbdChannel;
        if (disableRntbdChannel == null)
          return;
        disableRntbdChannel();
      })).ContinueWith((Action<Task>) (failedTask => DefaultTrace.TraceError("RNTBD channel callback failed: {0}", (object) failedTask.Exception)), new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
    }

    internal static void SetTransportPerformanceCounters(
      TransportPerformanceCounters transportPerformanceCounters)
    {
      TransportClient.transportPerformanceCounters = transportPerformanceCounters != null ? transportPerformanceCounters : throw new ArgumentNullException(nameof (transportPerformanceCounters));
    }

    internal static TransportPerformanceCounters GetTransportPerformanceCounters() => TransportClient.transportPerformanceCounters;

    private enum TransportResponseStatusCode
    {
      UnknownException = -2, // 0xFFFFFFFE
      DocumentClientException = -1, // 0xFFFFFFFF
      Success = 0,
    }

    public sealed class Options
    {
      private UserAgentContainer userAgent;
      private TimeSpan openTimeout = TimeSpan.Zero;
      private TimeSpan timerPoolResolution = TimeSpan.Zero;

      public Options(TimeSpan requestTimeout)
      {
        this.RequestTimeout = requestTimeout;
        this.MaxChannels = (int) ushort.MaxValue;
        this.PartitionCount = 1;
        this.MaxRequestsPerChannel = 30;
        this.PortReuseMode = PortReuseMode.ReuseUnicastPort;
        this.PortPoolReuseThreshold = 256;
        this.PortPoolBindAttempts = 5;
        this.ReceiveHangDetectionTime = TimeSpan.FromSeconds(65.0);
        this.SendHangDetectionTime = TimeSpan.FromSeconds(10.0);
        this.IdleTimeout = TimeSpan.FromSeconds(1800.0);
        this.CallerId = RntbdConstants.CallerId.Anonymous;
        this.EnableCpuMonitor = true;
      }

      public TimeSpan RequestTimeout { get; private set; }

      public int MaxChannels { get; set; }

      public int PartitionCount { get; set; }

      public int MaxRequestsPerChannel { get; set; }

      public TimeSpan ReceiveHangDetectionTime { get; set; }

      public TimeSpan SendHangDetectionTime { get; set; }

      public TimeSpan IdleTimeout { get; set; }

      public bool EnableCpuMonitor { get; set; }

      public RntbdConstants.CallerId CallerId { get; set; }

      public UserAgentContainer UserAgent
      {
        get
        {
          if (this.userAgent != null)
            return this.userAgent;
          this.userAgent = new UserAgentContainer();
          return this.userAgent;
        }
        set => this.userAgent = value;
      }

      public string CertificateHostNameOverride { get; set; }

      public IConnectionStateListener ConnectionStateListener { get; set; }

      public TimeSpan OpenTimeout
      {
        get => this.openTimeout > TimeSpan.Zero ? this.openTimeout : this.RequestTimeout;
        set => this.openTimeout = value;
      }

      public PortReuseMode PortReuseMode { get; set; }

      public int PortPoolReuseThreshold { get; internal set; }

      public int PortPoolBindAttempts { get; internal set; }

      public TimeSpan TimerPoolResolution
      {
        get => TransportClient.Options.GetTimerPoolResolutionSeconds(this.timerPoolResolution, this.RequestTimeout, this.openTimeout);
        set => this.timerPoolResolution = value;
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Rntbd.TransportClient.Options");
        stringBuilder.Append("  OpenTimeout: ");
        stringBuilder.AppendLine(this.OpenTimeout.ToString("c"));
        stringBuilder.Append("  RequestTimeout: ");
        stringBuilder.AppendLine(this.RequestTimeout.ToString("c"));
        stringBuilder.Append("  TimerPoolResolution: ");
        stringBuilder.AppendLine(this.TimerPoolResolution.ToString("c"));
        stringBuilder.Append("  MaxChannels: ");
        stringBuilder.AppendLine(this.MaxChannels.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("  PartitionCount: ");
        stringBuilder.AppendLine(this.PartitionCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("  MaxRequestsPerChannel: ");
        stringBuilder.AppendLine(this.MaxRequestsPerChannel.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append("  ReceiveHangDetectionTime: ");
        stringBuilder.AppendLine(this.ReceiveHangDetectionTime.ToString("c"));
        stringBuilder.Append("  SendHangDetectionTime: ");
        stringBuilder.AppendLine(this.SendHangDetectionTime.ToString("c"));
        stringBuilder.Append("  IdleTimeout: ");
        stringBuilder.AppendLine(this.IdleTimeout.ToString("c"));
        stringBuilder.Append("  EnableCpuMonitor: ");
        stringBuilder.AppendLine(this.EnableCpuMonitor.ToString());
        stringBuilder.Append("  UserAgent: ");
        stringBuilder.Append(this.UserAgent.UserAgent);
        stringBuilder.Append(" Suffix: ");
        stringBuilder.AppendLine(this.UserAgent.Suffix);
        stringBuilder.Append("  CertificateHostNameOverride: ");
        stringBuilder.AppendLine(this.CertificateHostNameOverride);
        return stringBuilder.ToString();
      }

      private static TimeSpan GetTimerPoolResolutionSeconds(
        TimeSpan timerPoolResolution,
        TimeSpan requestTimeout,
        TimeSpan openTimeout)
      {
        if (timerPoolResolution > TimeSpan.Zero && timerPoolResolution < openTimeout && timerPoolResolution < requestTimeout)
          return timerPoolResolution;
        return openTimeout > TimeSpan.Zero && requestTimeout > TimeSpan.Zero ? (!(openTimeout < requestTimeout) ? requestTimeout : openTimeout) : (!(openTimeout > TimeSpan.Zero) ? requestTimeout : openTimeout);
      }
    }
  }
}
