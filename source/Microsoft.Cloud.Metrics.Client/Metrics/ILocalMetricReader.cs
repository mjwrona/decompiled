// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.ILocalMetricReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public interface ILocalMetricReader
  {
    Task ReadLocalRawMetricsAsync(
      Action<ILocalRawMetric> metricProducedAction,
      CancellationToken cancellationToken,
      string etlFileName = null);

    Task ReadLocalAggregatedMetricsAsync(
      Action<ILocalAggregatedMetric> metricProducedAction,
      CancellationToken cancellationToken,
      string etlFileName = null);
  }
}
