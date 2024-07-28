// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.LocalAggregatedMetric
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
  internal sealed class LocalAggregatedMetric : ILocalAggregatedMetric
  {
    private static readonly char[] EtwListSeparatorChar = new char[1]
    {
      '^'
    };
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (LocalAggregatedMetric));
    private readonly Dictionary<string, string> dimensions = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public string MonitoringAccount { get; private set; }

    public string MetricNamespace { get; private set; }

    public string MetricName { get; private set; }

    public DateTime MetricTimeUtc { get; private set; }

    public IReadOnlyDictionary<string, string> Dimensions => (IReadOnlyDictionary<string, string>) this.dimensions;

    public float ScalingFactor { get; private set; }

    public uint Count { get; private set; }

    public float ScaledSum { get; private set; }

    public float ScaledMin { get; private set; }

    public float ScaledMax { get; private set; }

    public ulong Sum { get; private set; }

    public ulong Min { get; private set; }

    public ulong Max { get; private set; }

    [HandleProcessCorruptedStateExceptions]
    internal static unsafe LocalAggregatedMetric ConvertToMetricData(
      NativeMethods.EventRecord* etwMetricData)
    {
      LocalAggregatedMetric metricData = new LocalAggregatedMetric();
      try
      {
        IntPtr pointerInPayload = etwMetricData->UserData;
        metricData.MonitoringAccount = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        metricData.MetricNamespace = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        metricData.MetricName = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        long fileTime = *(long*) (void*) pointerInPayload;
        metricData.MetricTimeUtc = DateTime.FromFileTimeUtc(fileTime);
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 8);
        string str1 = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        string str2 = EtwPayloadManipulationUtils.ReadString(ref pointerInPayload);
        if (!string.IsNullOrWhiteSpace(str1) && !string.IsNullOrWhiteSpace(str2))
        {
          string[] strArray1 = str1.Split(LocalAggregatedMetric.EtwListSeparatorChar, StringSplitOptions.None);
          string[] strArray2 = str2.Split(LocalAggregatedMetric.EtwListSeparatorChar, StringSplitOptions.None);
          for (int index = 0; index < strArray1.Length && index < strArray2.Length; ++index)
            metricData.dimensions[strArray1[index]] = strArray2[index];
        }
        float num1 = *(float*) (void*) pointerInPayload;
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 4);
        metricData.ScalingFactor = num1;
        int num2 = *(int*) (void*) pointerInPayload;
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 4);
        if ((num2 & 1) != 0)
        {
          metricData.Min = (ulong) *(long*) (void*) pointerInPayload;
          metricData.ScaledMin = (float) metricData.Min / num1;
        }
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 8);
        if ((num2 & 2) != 0)
        {
          metricData.Max = (ulong) *(long*) (void*) pointerInPayload;
          metricData.ScaledMax = (float) metricData.Max / num1;
        }
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 8);
        if ((num2 & 4) != 0)
        {
          metricData.Sum = (ulong) *(long*) (void*) pointerInPayload;
          metricData.ScaledSum = (float) metricData.Sum / num1;
        }
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 8);
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 4);
        if ((num2 & 16) != 0)
          metricData.Count = *(uint*) (void*) pointerInPayload;
        pointerInPayload = EtwPayloadManipulationUtils.Shift(pointerInPayload, 4);
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, LocalAggregatedMetric.LogId, nameof (ConvertToMetricData), "Failed to read aggregated metric data from the ETW event payload, exception = {0}. Returning a partially constructed metric object to help troubleshooting.", (object) ex);
      }
      return metricData;
    }
  }
}
