// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PerformanceCounters.MessagingPerformanceCounters
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;

namespace Microsoft.Azure.NotificationHubs.PerformanceCounters
{
  internal sealed class MessagingPerformanceCounters : PerformanceCounter
  {
    private static readonly SortedDictionary<MessagingPerformanceCounters.MessagingPerfCounterKeys, MessagingPerformanceCounters.CounterMetadata> PerfCounterDictionary = new SortedDictionary<MessagingPerformanceCounters.MessagingPerfCounterKeys, MessagingPerformanceCounters.CounterMetadata>()
    {
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageSuccessCount,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Success Count", 0, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageSuccessPerSec,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Success/sec", 1, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageFailureCount,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Failure Count", 2, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageFailurePerSec,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Failures/sec", 3, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Latency Base", 4, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.SendMessageLatency,
        new MessagingPerformanceCounters.CounterMetadata("SendMessage Latency", 5, CounterType.AverageTimer32)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageSuccessCount,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Success Count", 6, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageSuccessPerSec,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Success/sec", 7, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageFailureCount,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Failure Count", 8, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageFailurePerSec,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Failures/sec", 9, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Latency Base", 10, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ReceiveMessageLatency,
        new MessagingPerformanceCounters.CounterMetadata("ReceiveMessage Latency", 11, CounterType.AverageTimer32)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageSuccessCount,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Success Count", 12, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageSuccessPerSec,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Success/sec", 13, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageFailureCount,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Failures Count", 14, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageFailurePerSec,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Failures/sec", 15, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Latency Base", 16, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CompleteMessageLatency,
        new MessagingPerformanceCounters.CounterMetadata("CompleteMessage Latency", 17, CounterType.AverageTimer32)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceSuccessCount,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Completed Count", 18, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceSuccessPerSec,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Completed/sec", 19, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceFailureCount,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Failure Count", 20, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceFailurePerSec,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Failures/sec", 21, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Latency Base", 22, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.AcceptMessageSessionByNamespaceLatency,
        new MessagingPerformanceCounters.CounterMetadata("AcceptMessageSessionByNamespace Latency", 23, CounterType.AverageTimer32)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ExceptionCount,
        new MessagingPerformanceCounters.CounterMetadata("Exceptions Count", 24, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ExceptionPerSec,
        new MessagingPerformanceCounters.CounterMetadata("Exceptions/sec", 25, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.MessagingExceptionPerSec,
        new MessagingPerformanceCounters.CounterMetadata("MessagingExceptions/sec", 26, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.MessagingCommunicationExceptionPerSec,
        new MessagingPerformanceCounters.CounterMetadata("MessagingCommunicationExceptions/sec", 27, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.ServerBusyExceptionPerSec,
        new MessagingPerformanceCounters.CounterMetadata("ServerBusyException/sec", 28, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.MessagingFactoryCount,
        new MessagingPerformanceCounters.CounterMetadata("MessagingFactory count", 29, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.TokensAcquiredPerSec,
        new MessagingPerformanceCounters.CounterMetadata("Tokens Acquired/sec", 30, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.TokenAcquisitionFailuresPerSec,
        new MessagingPerformanceCounters.CounterMetadata("Token Acquisition Failures/sec", 31, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.TokenAcquisitionLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("Token Acquisition Latency Base", 32, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.TokenAcquisitionLatency,
        new MessagingPerformanceCounters.CounterMetadata("Token Acquisition Latency", 33, CounterType.AverageTimer32)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.PendingReceiveMessageCount,
        new MessagingPerformanceCounters.CounterMetadata("Pending ReceiveMessage Count", 34, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.PendingAcceptMessageSessionCount,
        new MessagingPerformanceCounters.CounterMetadata("Pending AcceptMessageSession Count", 35, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.PendingAcceptMessageSessionByNamespaceCount,
        new MessagingPerformanceCounters.CounterMetadata("Pending AcceptMessageSessionByNamespace Count", 36, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageSuccessCount,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Success Count", 37, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageSuccessPerSec,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Success/sec", 38, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageFailureCount,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Failure Count", 39, CounterType.RawData64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageFailurePerSec,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Failures/sec", 40, CounterType.RateOfCountPerSecond64)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageLatencyBase,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Latency Base", 41, CounterType.AverageBase)
      },
      {
        MessagingPerformanceCounters.MessagingPerfCounterKeys.CancelScheduledMessageLatency,
        new MessagingPerformanceCounters.CounterMetadata("Cancel Scheduled Message Latency", 42, CounterType.AverageTimer32)
      }
    };
    private static readonly object staticSyncRoot = new object();
    private static readonly Dictionary<string, MessagingPerformanceCounters> performanceCounters = new Dictionary<string, MessagingPerformanceCounters>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static CounterSet counterSet;
    private readonly string instanceName;
    private CounterSetInstance counterSetInstance;
    internal CounterData[] counters;

    private MessagingPerformanceCounters(string name)
    {
      this.instanceName = name != null ? name : throw new ArgumentNullException(nameof (name));
      this.Scope = new ClientPerformanceCounterScope(ClientPerformanceCounterLevel.All);
      this.Initialize(ClientPerformanceCounterLevel.All);
    }

    public static MessagingPerformanceCounters GetCounter(string name)
    {
      lock (MessagingPerformanceCounters.staticSyncRoot)
      {
        MessagingPerformanceCounters counter;
        if (!MessagingPerformanceCounters.performanceCounters.TryGetValue(name, out counter))
        {
          counter = new MessagingPerformanceCounters(name);
          MessagingPerformanceCounters.performanceCounters.Add(name, counter);
        }
        return counter;
      }
    }

    public override List<string> CounterNames
    {
      get
      {
        List<string> counterNames = new List<string>();
        foreach (MessagingPerformanceCounters.CounterMetadata counterMetadata in MessagingPerformanceCounters.PerfCounterDictionary.Values)
          counterNames.Add(counterMetadata.Name);
        return counterNames;
      }
    }

    public override int CounterStart => 0;

    public override int CounterEnd => 43;

    public override string InstanceName => this.instanceName;

    protected override CounterSetInstance CounterSetInstance => this.counterSetInstance;

    protected override CounterData[] Counters => this.counters;

    protected override void OnInitialize()
    {
      lock (MessagingPerformanceCounters.staticSyncRoot)
      {
        if (MessagingPerformanceCounters.counterSet == null)
        {
          CounterSet counterSet = MessagingPerformanceCounters.CreateCounterSet();
          foreach (KeyValuePair<MessagingPerformanceCounters.MessagingPerfCounterKeys, MessagingPerformanceCounters.CounterMetadata> perfCounter in MessagingPerformanceCounters.PerfCounterDictionary)
            counterSet.AddCounter(perfCounter.Value.Id, perfCounter.Value.Type, perfCounter.Value.Name);
          MessagingPerformanceCounters.counterSet = counterSet;
        }
      }
      this.counterSetInstance = MessagingPerformanceCounters.CreateCounterSetInstance(this.InstanceName);
      this.counters = new CounterData[43];
      for (int counterId = 0; counterId < this.counters.Length; ++counterId)
      {
        this.counters[counterId] = this.counterSetInstance.Counters[counterId];
        this.counters[counterId].Value = 0L;
      }
    }

    private static CounterSet CreateCounterSet() => new CounterSet(ClientPerformanceCounterConstants.MessagingProviderId, ClientPerformanceCounterConstants.ClientProviderId, CounterSetInstanceType.Multiple);

    private static CounterSetInstance CreateCounterSetInstance(string name) => MessagingPerformanceCounters.counterSet.CreateCounterSetInstance(name);

    public static void IncrementSendMessageSuccessPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[0].IncrementBy((long) count);
      counter.counters[1].IncrementBy((long) count);
    }

    public static void IncrementSendMessageFailurePerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[2].IncrementBy((long) count);
      counter.counters[3].IncrementBy((long) count);
    }

    public static void IncrementSendMessageLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[5].IncrementBy(ticks);
      counter.counters[4].Increment();
    }

    public static void IncrementReceiveMessageSuccessPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[6].IncrementBy((long) count);
      counter.counters[7].IncrementBy((long) count);
    }

    public static void IncrementReceiveMessageFailurePerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[8].IncrementBy((long) count);
      counter.counters[9].IncrementBy((long) count);
    }

    public static void IncrementReceiveMessageLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[11].IncrementBy(ticks);
      counter.counters[10].Increment();
    }

    public static void IncrementCompleteMessageSuccessPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[12].IncrementBy((long) count);
      counter.counters[13].IncrementBy((long) count);
    }

    public static void IncrementCompleteMessageFailurePerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[14].IncrementBy((long) count);
      counter.counters[15].IncrementBy((long) count);
    }

    public static void IncrementCompleteMessageLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[17].IncrementBy(ticks);
      counter.counters[16].Increment();
    }

    public static void IncrementAcceptMessageSessionByNamespaceSuccessPerSec(
      Uri endpoint,
      int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[18].IncrementBy((long) count);
      counter.counters[19].IncrementBy((long) count);
    }

    public static void IncrementAcceptMessageSessionByNamespaceFailurePerSec(
      Uri endpoint,
      int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[20].IncrementBy((long) count);
      counter.counters[21].IncrementBy((long) count);
    }

    public static void IncrementAcceptMessageSessionByNamespaceLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[23].IncrementBy(ticks);
      counter.counters[22].Increment();
    }

    public static void IncrementExceptionPerSec(Uri endpoint, int count, Exception exception)
    {
      if (exception == null)
        return;
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[24].IncrementBy((long) count);
      counter.counters[25].IncrementBy((long) count);
      switch (exception)
      {
        case ServerBusyException _:
          counter.counters[28].IncrementBy((long) count);
          break;
        case MessagingCommunicationException _:
          counter.counters[27].IncrementBy((long) count);
          break;
        case MessagingException _:
          counter.counters[26].IncrementBy((long) count);
          break;
      }
    }

    public static void IncrementMessagingFactoryCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[29].IncrementBy((long) count);
    }

    public static void DecrementMessagingFactoryCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[29].IncrementBy((long) (-1 * count));
    }

    public static void IncrementTokensAcquiredPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[30].IncrementBy((long) count);
    }

    public static void IncrementTokenAcquisitionFailuresPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[31].IncrementBy((long) count);
    }

    public static void IncrementTokenAcquisitionLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[33].IncrementBy(ticks);
      counter.counters[32].Increment();
    }

    public static void IncrementPendingReceiveMessageCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[34].IncrementBy((long) count);
    }

    public static void DecrementPendingReceiveMessageCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[34].IncrementBy((long) (-1 * count));
    }

    public static void IncrementPendingAcceptMessageSessionCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[35].IncrementBy((long) count);
    }

    public static void DecrementPendingAcceptMessageSessionCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[35].IncrementBy((long) (-1 * count));
    }

    public static void IncrementPendingAcceptMessageSessionByNamespaceCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[36].IncrementBy((long) count);
    }

    public static void DecrementPendingAcceptMessageSessionByNamespaceCount(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[36].IncrementBy((long) (-1 * count));
    }

    public static void IncrementCancelScheduledMessageSuccessPerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[37].IncrementBy((long) count);
      counter.counters[38].IncrementBy((long) count);
    }

    public static void IncrementCancelScheduledMessageFailurePerSec(Uri endpoint, int count)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[39].IncrementBy((long) count);
      counter.counters[40].IncrementBy((long) count);
    }

    public static void IncrementCancelScheduledMessageLatency(Uri endpoint, long ticks)
    {
      MessagingPerformanceCounters counter = MessagingPerformanceCounters.GetCounter(endpoint.Host);
      if (!counter.Initialized)
        return;
      counter.counters[42].IncrementBy(ticks);
      counter.counters[41].Increment();
    }

    private enum MessagingPerfCounterKeys
    {
      SendMessageSuccessCount,
      SendMessageSuccessPerSec,
      SendMessageFailureCount,
      SendMessageFailurePerSec,
      SendMessageLatencyBase,
      SendMessageLatency,
      ReceiveMessageSuccessCount,
      ReceiveMessageSuccessPerSec,
      ReceiveMessageFailureCount,
      ReceiveMessageFailurePerSec,
      ReceiveMessageLatencyBase,
      ReceiveMessageLatency,
      CompleteMessageSuccessCount,
      CompleteMessageSuccessPerSec,
      CompleteMessageFailureCount,
      CompleteMessageFailurePerSec,
      CompleteMessageLatencyBase,
      CompleteMessageLatency,
      AcceptMessageSessionByNamespaceSuccessCount,
      AcceptMessageSessionByNamespaceSuccessPerSec,
      AcceptMessageSessionByNamespaceFailureCount,
      AcceptMessageSessionByNamespaceFailurePerSec,
      AcceptMessageSessionByNamespaceLatencyBase,
      AcceptMessageSessionByNamespaceLatency,
      ExceptionCount,
      ExceptionPerSec,
      MessagingExceptionPerSec,
      MessagingCommunicationExceptionPerSec,
      ServerBusyExceptionPerSec,
      MessagingFactoryCount,
      TokensAcquiredPerSec,
      TokenAcquisitionFailuresPerSec,
      TokenAcquisitionLatencyBase,
      TokenAcquisitionLatency,
      PendingReceiveMessageCount,
      PendingAcceptMessageSessionCount,
      PendingAcceptMessageSessionByNamespaceCount,
      CancelScheduledMessageSuccessCount,
      CancelScheduledMessageSuccessPerSec,
      CancelScheduledMessageFailureCount,
      CancelScheduledMessageFailurePerSec,
      CancelScheduledMessageLatencyBase,
      CancelScheduledMessageLatency,
      TotalCounters,
    }

    private sealed class CounterMetadata
    {
      public CounterMetadata(string name, int id, CounterType type)
      {
        this.Name = name;
        this.Id = id;
        this.Type = type;
      }

      public string Name { get; private set; }

      public int Id { get; private set; }

      public CounterType Type { get; private set; }
    }
  }
}
