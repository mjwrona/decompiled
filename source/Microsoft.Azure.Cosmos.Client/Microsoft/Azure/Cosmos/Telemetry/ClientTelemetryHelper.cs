// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.ClientTelemetryHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal static class ClientTelemetryHelper
  {
    internal static async 
    #nullable disable
    Task<AccountProperties> SetAccountNameAsync(DocumentClient documentclient)
    {
      DefaultTrace.TraceVerbose("Getting Account Information for Telemetry.");
      try
      {
        if (documentclient.GlobalEndpointManager != null)
          return await documentclient.GlobalEndpointManager.GetDatabaseAccountAsync();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Exception while getting account information in client telemetry : {0}", (object) ex.Message);
      }
      return (AccountProperties) null;
    }

    internal static void RecordSystemUsage(
      SystemUsageHistory systemUsageHistory,
      List<SystemInfo> systemInfoCollection,
      bool isDirectConnectionMode)
    {
      if (systemUsageHistory.Values == null)
        return;
      DefaultTrace.TraceInformation("System Usage recorded by telemetry is : {0}", (object) systemUsageHistory);
      systemInfoCollection.Add(TelemetrySystemUsage.GetCpuInfo((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
      systemInfoCollection.Add(TelemetrySystemUsage.GetMemoryRemainingInfo((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
      systemInfoCollection.Add(TelemetrySystemUsage.GetAvailableThreadsInfo((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
      systemInfoCollection.Add(TelemetrySystemUsage.GetThreadWaitIntervalInMs((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
      systemInfoCollection.Add(TelemetrySystemUsage.GetThreadStarvationSignalCount((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
      if (!isDirectConnectionMode)
        return;
      systemInfoCollection.Add(TelemetrySystemUsage.GetTcpConnectionCount((IReadOnlyCollection<SystemUsageLoad>) systemUsageHistory.Values));
    }

    internal static List<OperationInfo> ToListWithMetricsInfo(
      IDictionary<OperationInfo, (LongConcurrentHistogram latency, LongConcurrentHistogram requestcharge)> metrics)
    {
      DefaultTrace.TraceInformation("Aggregating operation information to list started");
      List<OperationInfo> listWithMetricsInfo = new List<OperationInfo>();
      foreach (KeyValuePair<OperationInfo, (LongConcurrentHistogram latency, LongConcurrentHistogram requestcharge)> metric in (IEnumerable<KeyValuePair<OperationInfo, (LongConcurrentHistogram latency, LongConcurrentHistogram requestcharge)>>) metrics)
      {
        OperationInfo key = metric.Key;
        key.MetricInfo = new MetricInfo("RequestLatency", "MilliSecond");
        key.SetAggregators(metric.Value.latency, 10000.0);
        listWithMetricsInfo.Add(key);
        OperationInfo operationInfo = key.Copy();
        operationInfo.MetricInfo = new MetricInfo("RequestCharge", "RU");
        operationInfo.SetAggregators(metric.Value.requestcharge, 100.0);
        listWithMetricsInfo.Add(operationInfo);
      }
      DefaultTrace.TraceInformation("Aggregating operation information to list done");
      return listWithMetricsInfo;
    }

    internal static string GetContactedRegions(CosmosDiagnostics cosmosDiagnostics)
    {
      IReadOnlyList<(string regionName, Uri uri)> contactedRegions = cosmosDiagnostics.GetContactedRegions();
      if (contactedRegions == null || contactedRegions.Count == 0)
        return (string) null;
      if (contactedRegions.Count == 1)
        return contactedRegions[0].regionName;
      StringBuilder stringBuilder = new StringBuilder();
      foreach ((string regionName, Uri _) in (IEnumerable<(string regionName, Uri uri)>) contactedRegions)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(regionName);
      }
      return stringBuilder.ToString();
    }
  }
}
