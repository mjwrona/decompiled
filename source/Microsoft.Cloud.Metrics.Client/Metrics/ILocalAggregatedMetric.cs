// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.ILocalAggregatedMetric
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public interface ILocalAggregatedMetric
  {
    string MonitoringAccount { get; }

    string MetricNamespace { get; }

    string MetricName { get; }

    DateTime MetricTimeUtc { get; }

    IReadOnlyDictionary<string, string> Dimensions { get; }

    float ScalingFactor { get; }

    uint Count { get; }

    float ScaledSum { get; }

    float ScaledMin { get; }

    float ScaledMax { get; }

    ulong Sum { get; }

    ulong Min { get; }

    ulong Max { get; }
  }
}
