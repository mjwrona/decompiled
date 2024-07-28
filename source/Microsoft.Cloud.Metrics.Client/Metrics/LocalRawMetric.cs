// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.LocalRawMetric
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Cloud.Metrics.Client.Metrics.Etw;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  internal sealed class LocalRawMetric : ILocalRawMetric
  {
    private const int PlatformMetricEtwOperationCode = 51;
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (LocalRawMetric));

    public string MonitoringAccount { get; private set; }

    public string MetricNamespace { get; private set; }

    public string MetricName { get; private set; }

    public IDictionary<string, string> Dimensions { get; private set; }

    public bool IsPlatformMetric { get; private set; }

    public RawMetricEventType EventType { get; private set; }

    public DateTime MetricTimeUtc { get; private set; }

    public ulong MetricLongValue { get; private set; }

    public double MetricDoubleValue { get; private set; }

    [HandleProcessCorruptedStateExceptions]
    internal static unsafe LocalRawMetric ConvertToMetricData(
      NativeMethods.EventRecord* etwMetricData)
    {
      LocalRawMetric metricData = new LocalRawMetric();
      try
      {
        metricData.EventType = (RawMetricEventType) etwMetricData->EventHeader.Id;
        metricData.IsPlatformMetric = metricData.EventType == RawMetricEventType.DoubleScaledToLongMetric;
        if (metricData.EventType != RawMetricEventType.OldIfx && metricData.EventType != RawMetricEventType.DoubleScaledToLongMetric)
        {
          Logger.Log(LoggerLevel.Error, LocalRawMetric.LogId, nameof (ConvertToMetricData), string.Format("LocalRawMetric reader doesn't understand event type '{0}' yet.", (object) metricData.EventType));
          return (LocalRawMetric) null;
        }
        DateTime dateTime = DateTime.FromFileTimeUtc(etwMetricData->EventHeader.TimeStamp);
        IntPtr userData = etwMetricData->UserData;
        ushort capacity = *(ushort*) (void*) userData;
        IntPtr ptr1 = EtwPayloadManipulationUtils.Shift(EtwPayloadManipulationUtils.Shift(userData, 2), 6);
        long fileTime = *(long*) (void*) ptr1;
        IntPtr ptr2 = EtwPayloadManipulationUtils.Shift(ptr1, 8);
        metricData.MetricTimeUtc = fileTime == 0L ? dateTime : DateTime.FromFileTimeUtc(fileTime);
        if (metricData.EventType == RawMetricEventType.DoubleScaledToLongMetric)
          metricData.MetricDoubleValue = *(double*) (void*) ptr2;
        else
          metricData.MetricLongValue = (ulong) *(long*) (void*) ptr2;
        IntPtr pointerInPayload = EtwPayloadManipulationUtils.Shift(ptr2, 8);
        metricData.MonitoringAccount = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        metricData.MetricNamespace = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        metricData.MetricName = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        List<string> stringList1 = new List<string>();
        for (int index = 0; index < (int) capacity; ++index)
          stringList1.Add(EtwPayloadManipulationUtils.ReadString(ref pointerInPayload));
        List<string> stringList2 = new List<string>();
        for (int index = 0; index < (int) capacity; ++index)
          stringList2.Add(EtwPayloadManipulationUtils.ReadString(ref pointerInPayload));
        metricData.Dimensions = (IDictionary<string, string>) new Dictionary<string, string>((int) capacity);
        for (int index = 0; index < (int) capacity; ++index)
          metricData.Dimensions[stringList1[index]] = stringList2[index];
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, LocalRawMetric.LogId, nameof (ConvertToMetricData), "Failed to read raw metric data from the ETW event payload, exception = {0}. Returning a partially constructed metric object to help troubleshooting.", (object) ex);
      }
      return metricData;
    }
  }
}
