// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Monitors.IMonitorReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Online.Metrics.Serialization.Configuration;
using Microsoft.Online.Metrics.Serialization.Monitor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Monitors
{
  public interface IMonitorReader
  {
    Task<IReadOnlyList<MonitorIdentifier>> GetMonitorsAsync(MetricIdentifier metricIdentifier);

    Task<IReadOnlyList<MonitorIdentifier>> GetMonitorsAsync(
      string monitoringAccount,
      string metricNamespace = null);

    [Obsolete("Deprecated, please use GetCurrentHealthStatusAsync (notice fixed spelling) instead.")]
    Task<IMonitorHealthStatus> GetCurrentHeathStatusAsync(
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition);

    Task<IMonitorHealthStatus> GetCurrentHealthStatusAsync(
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition);

    Task<IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>> GetMultipleCurrentHeathStatusesAsync(
      params TimeSeriesDefinition<MonitorIdentifier>[] monitorInstanceDefinitions);

    Task<IReadOnlyList<KeyValuePair<TimeSeriesDefinition<MonitorIdentifier>, IMonitorHealthStatus>>> GetMultipleCurrentHeathStatusesAsync(
      IEnumerable<TimeSeriesDefinition<MonitorIdentifier>> monitorInstanceDefinitions);

    [Obsolete("We are going to retire this. Please use GetBatchWatchdogHealthHistory in Health SDK instead.")]
    Task<TimeSeries<MonitorIdentifier, bool?>> GetMonitorHistoryAsync(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      TimeSeriesDefinition<MonitorIdentifier> monitorInstanceDefinition);
  }
}
