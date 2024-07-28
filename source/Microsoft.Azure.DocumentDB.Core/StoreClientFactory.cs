// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreClientFactory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreClientFactory : IStoreClientFactory, IDisposable
  {
    private bool isDisposed;
    private readonly Protocol protocol;
    private readonly RetryWithConfiguration retryWithConfiguration;
    private TransportClient transportClient;
    private TransportClient fallbackClient;
    private ConnectionStateListener connectionStateListener;

    public StoreClientFactory(
      Protocol protocol,
      int requestTimeoutInSeconds,
      int maxConcurrentConnectionOpenRequests,
      UserAgentContainer userAgent = null,
      ICommunicationEventSource eventSource = null,
      string overrideHostNameInCertificate = null,
      int openTimeoutInSeconds = 0,
      int idleTimeoutInSeconds = -1,
      int timerPoolGranularityInSeconds = 0,
      int maxRntbdChannels = 65535,
      int rntbdPartitionCount = 1,
      int maxRequestsPerRntbdChannel = 30,
      PortReuseMode rntbdPortReuseMode = PortReuseMode.ReuseUnicastPort,
      int rntbdPortPoolReuseThreshold = 256,
      int rntbdPortPoolBindAttempts = 5,
      int receiveHangDetectionTimeSeconds = 65,
      int sendHangDetectionTimeSeconds = 10,
      bool enableCpuMonitor = true,
      RetryWithConfiguration retryWithConfiguration = null,
      RntbdConstants.CallerId callerId = RntbdConstants.CallerId.Anonymous,
      bool enableTcpConnectionEndpointRediscovery = false,
      IAddressResolverExtension addressResolver = null)
    {
      if (idleTimeoutInSeconds > 0 && idleTimeoutInSeconds < 600)
        throw new ArgumentOutOfRangeException(nameof (idleTimeoutInSeconds));
      if (protocol == Protocol.Https)
      {
        if (eventSource == null)
          throw new ArgumentOutOfRangeException(nameof (eventSource));
        this.transportClient = (TransportClient) new HttpTransportClient(requestTimeoutInSeconds, eventSource, userAgent, idleTimeoutInSeconds);
      }
      else
      {
        if (protocol != Protocol.Tcp)
          throw new ArgumentOutOfRangeException(nameof (protocol), (object) protocol, "Invalid protocol value");
        if (maxRntbdChannels <= 0)
          throw new ArgumentOutOfRangeException(nameof (maxRntbdChannels));
        if (rntbdPartitionCount < 1 || rntbdPartitionCount > 8)
          throw new ArgumentOutOfRangeException(nameof (rntbdPartitionCount));
        if (maxRequestsPerRntbdChannel <= 0)
          throw new ArgumentOutOfRangeException(nameof (maxRequestsPerRntbdChannel));
        if (maxRntbdChannels > (int) ushort.MaxValue)
          DefaultTrace.TraceWarning("The value of {0} is unreasonably large. Received: {1}. Use {2} to represent \"effectively infinite\".", (object) nameof (maxRntbdChannels), (object) maxRntbdChannels, (object) ushort.MaxValue);
        if (maxRequestsPerRntbdChannel < 6)
          DefaultTrace.TraceWarning("The value of {0} is unreasonably small. Received: {1}. Small values of {0} can cause a large number of RNTBD channels to be opened to the same back-end. Reasonable values are between {2} and {3}", (object) nameof (maxRequestsPerRntbdChannel), (object) maxRequestsPerRntbdChannel, (object) 6, (object) 256);
        if (maxRequestsPerRntbdChannel > 256)
          DefaultTrace.TraceWarning("The value of {0} is unreasonably large. Received: {1}. Large values of {0} can cause significant head-of-line blocking over RNTBD channels. Reasonable values are between {2} and {3}", (object) nameof (maxRequestsPerRntbdChannel), (object) maxRequestsPerRntbdChannel, (object) 6, (object) 256);
        if (checked (maxRntbdChannels * maxRequestsPerRntbdChannel) < 512)
          DefaultTrace.TraceWarning("The number of simultaneous requests allowed per backend is unreasonably small. Received {0} = {1}, {2} = {3}. Reasonable values are at least {4}", (object) nameof (maxRntbdChannels), (object) maxRntbdChannels, (object) nameof (maxRequestsPerRntbdChannel), (object) maxRequestsPerRntbdChannel, (object) 512);
        StoreClientFactory.ValidatePortPoolReuseThreshold(ref rntbdPortPoolReuseThreshold);
        StoreClientFactory.ValidatePortPoolBindAttempts(ref rntbdPortPoolBindAttempts);
        if (rntbdPortPoolBindAttempts > rntbdPortPoolReuseThreshold)
        {
          DefaultTrace.TraceWarning("Raising the value of {0} from {1} to {2} to match the value of {3}", (object) nameof (rntbdPortPoolReuseThreshold), (object) rntbdPortPoolReuseThreshold, (object) (rntbdPortPoolBindAttempts + 1), (object) nameof (rntbdPortPoolBindAttempts));
          rntbdPortPoolReuseThreshold = rntbdPortPoolBindAttempts;
        }
        if (receiveHangDetectionTimeSeconds < 65)
        {
          DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", (object) nameof (receiveHangDetectionTimeSeconds), (object) receiveHangDetectionTimeSeconds, (object) 65);
          receiveHangDetectionTimeSeconds = 65;
        }
        if (receiveHangDetectionTimeSeconds > 180)
        {
          DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", (object) nameof (receiveHangDetectionTimeSeconds), (object) receiveHangDetectionTimeSeconds, (object) 180);
          receiveHangDetectionTimeSeconds = 180;
        }
        if (sendHangDetectionTimeSeconds < 2)
        {
          DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", (object) nameof (sendHangDetectionTimeSeconds), (object) sendHangDetectionTimeSeconds, (object) 2);
          sendHangDetectionTimeSeconds = 2;
        }
        if (sendHangDetectionTimeSeconds > 60)
        {
          DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", (object) nameof (sendHangDetectionTimeSeconds), (object) sendHangDetectionTimeSeconds, (object) 60);
          sendHangDetectionTimeSeconds = 60;
        }
        if (enableTcpConnectionEndpointRediscovery && addressResolver != null)
          this.connectionStateListener = new ConnectionStateListener(addressResolver);
        this.fallbackClient = (TransportClient) new RntbdTransportClient(requestTimeoutInSeconds, maxConcurrentConnectionOpenRequests, userAgent, overrideHostNameInCertificate, openTimeoutInSeconds, idleTimeoutInSeconds, timerPoolGranularityInSeconds);
        this.transportClient = (TransportClient) new Microsoft.Azure.Documents.Rntbd.TransportClient(new Microsoft.Azure.Documents.Rntbd.TransportClient.Options(TimeSpan.FromSeconds((double) requestTimeoutInSeconds))
        {
          MaxChannels = maxRntbdChannels,
          PartitionCount = rntbdPartitionCount,
          MaxRequestsPerChannel = maxRequestsPerRntbdChannel,
          PortReuseMode = rntbdPortReuseMode,
          PortPoolReuseThreshold = rntbdPortPoolReuseThreshold,
          PortPoolBindAttempts = rntbdPortPoolBindAttempts,
          ReceiveHangDetectionTime = TimeSpan.FromSeconds((double) receiveHangDetectionTimeSeconds),
          SendHangDetectionTime = TimeSpan.FromSeconds((double) sendHangDetectionTimeSeconds),
          UserAgent = userAgent,
          CertificateHostNameOverride = overrideHostNameInCertificate,
          OpenTimeout = TimeSpan.FromSeconds((double) openTimeoutInSeconds),
          TimerPoolResolution = TimeSpan.FromSeconds((double) timerPoolGranularityInSeconds),
          IdleTimeout = TimeSpan.FromSeconds((double) idleTimeoutInSeconds),
          EnableCpuMonitor = enableCpuMonitor,
          CallerId = callerId,
          ConnectionStateListener = (IConnectionStateListener) this.connectionStateListener
        });
      }
      this.protocol = protocol;
      this.retryWithConfiguration = retryWithConfiguration;
    }

    internal void WithTransportInterceptor(
      Func<TransportClient, TransportClient> transportClientHandlerFactory)
    {
      if (transportClientHandlerFactory == null)
        throw new ArgumentNullException(nameof (transportClientHandlerFactory));
      this.transportClient = transportClientHandlerFactory(this.transportClient);
      this.fallbackClient = transportClientHandlerFactory(this.fallbackClient);
    }

    public StoreClient CreateStoreClient(
      IAddressResolver addressResolver,
      ISessionContainer sessionContainer,
      IServiceConfigurationReader serviceConfigurationReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      bool enableRequestDiagnostics = false,
      bool enableReadRequestsFallback = false,
      bool useFallbackClient = true,
      bool useMultipleWriteLocations = false,
      bool detectClientConnectivityIssues = false)
    {
      this.ThrowIfDisposed();
      return useFallbackClient && this.fallbackClient != null ? new StoreClient(addressResolver, sessionContainer, serviceConfigurationReader, authorizationTokenProvider, this.protocol, this.fallbackClient, this.connectionStateListener, enableRequestDiagnostics, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, this.retryWithConfiguration) : new StoreClient(addressResolver, sessionContainer, serviceConfigurationReader, authorizationTokenProvider, this.protocol, this.transportClient, this.connectionStateListener, enableRequestDiagnostics, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, this.retryWithConfiguration);
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.transportClient != null)
      {
        this.transportClient.Dispose();
        this.transportClient = (TransportClient) null;
      }
      if (this.fallbackClient != null)
      {
        this.fallbackClient.Dispose();
        this.fallbackClient = (TransportClient) null;
      }
      this.isDisposed = true;
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (StoreClientFactory));
    }

    private static void ValidatePortPoolReuseThreshold(ref int rntbdPortPoolReuseThreshold)
    {
      if (rntbdPortPoolReuseThreshold < 32)
      {
        DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", (object) nameof (rntbdPortPoolReuseThreshold), (object) rntbdPortPoolReuseThreshold, (object) 32);
        rntbdPortPoolReuseThreshold = 32;
      }
      else
      {
        if (rntbdPortPoolReuseThreshold <= 2048)
          return;
        DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", (object) nameof (rntbdPortPoolReuseThreshold), (object) rntbdPortPoolReuseThreshold, (object) 2048);
        rntbdPortPoolReuseThreshold = 2048;
      }
    }

    private static void ValidatePortPoolBindAttempts(ref int rntbdPortPoolBindAttempts)
    {
      if (rntbdPortPoolBindAttempts < 3)
      {
        DefaultTrace.TraceWarning("The value of {0} is too small. Received {1}. Adjusting to {2}", (object) nameof (rntbdPortPoolBindAttempts), (object) rntbdPortPoolBindAttempts, (object) 3);
        rntbdPortPoolBindAttempts = 3;
      }
      else
      {
        if (rntbdPortPoolBindAttempts <= 32)
          return;
        DefaultTrace.TraceWarning("The value of {0} is too large. Received {1}. Adjusting to {2}", (object) nameof (rntbdPortPoolBindAttempts), (object) rntbdPortPoolBindAttempts, (object) 32);
        rntbdPortPoolBindAttempts = 32;
      }
    }
  }
}
