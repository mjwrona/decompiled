// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.IPerformanceCounterManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public interface IPerformanceCounterManager
  {
    void Initialize(string instanceName, CancellationToken hostShutdownToken);

    IPerformanceCounter LoadCounter(
      string categoryName,
      string counterName,
      string instanceName,
      bool isReadOnly);

    IPerformanceCounter ConnectionsConnected { get; }

    IPerformanceCounter ConnectionsReconnected { get; }

    IPerformanceCounter ConnectionsDisconnected { get; }

    IPerformanceCounter ConnectionsCurrentForeverFrame { get; }

    IPerformanceCounter ConnectionsCurrentLongPolling { get; }

    IPerformanceCounter ConnectionsCurrentServerSentEvents { get; }

    IPerformanceCounter ConnectionsCurrentWebSockets { get; }

    IPerformanceCounter ConnectionsCurrent { get; }

    IPerformanceCounter ConnectionMessagesReceivedTotal { get; }

    IPerformanceCounter ConnectionMessagesSentTotal { get; }

    IPerformanceCounter ConnectionMessagesReceivedPerSec { get; }

    IPerformanceCounter ConnectionMessagesSentPerSec { get; }

    IPerformanceCounter MessageBusMessagesReceivedTotal { get; }

    IPerformanceCounter MessageBusMessagesReceivedPerSec { get; }

    IPerformanceCounter ScaleoutMessageBusMessagesReceivedPerSec { get; }

    IPerformanceCounter MessageBusMessagesPublishedTotal { get; }

    IPerformanceCounter MessageBusMessagesPublishedPerSec { get; }

    IPerformanceCounter MessageBusSubscribersCurrent { get; }

    IPerformanceCounter MessageBusSubscribersTotal { get; }

    IPerformanceCounter MessageBusSubscribersPerSec { get; }

    IPerformanceCounter MessageBusAllocatedWorkers { get; }

    IPerformanceCounter MessageBusBusyWorkers { get; }

    IPerformanceCounter MessageBusTopicsCurrent { get; }

    IPerformanceCounter ErrorsAllTotal { get; }

    IPerformanceCounter ErrorsAllPerSec { get; }

    IPerformanceCounter ErrorsHubResolutionTotal { get; }

    IPerformanceCounter ErrorsHubResolutionPerSec { get; }

    IPerformanceCounter ErrorsHubInvocationTotal { get; }

    IPerformanceCounter ErrorsHubInvocationPerSec { get; }

    IPerformanceCounter ErrorsTransportTotal { get; }

    IPerformanceCounter ErrorsTransportPerSec { get; }

    IPerformanceCounter ScaleoutStreamCountTotal { get; }

    IPerformanceCounter ScaleoutStreamCountOpen { get; }

    IPerformanceCounter ScaleoutStreamCountBuffering { get; }

    IPerformanceCounter ScaleoutErrorsTotal { get; }

    IPerformanceCounter ScaleoutErrorsPerSec { get; }

    IPerformanceCounter ScaleoutSendQueueLength { get; }
  }
}
