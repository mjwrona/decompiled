// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.RawMetricEventType
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  internal enum RawMetricEventType : ushort
  {
    OldIfx = 0,
    UInt64Metric = 50, // 0x0032
    DoubleScaledToLongMetric = 51, // 0x0033
    BatchMetric = 52, // 0x0034
    ExternallyAggregatedULongMetric = 53, // 0x0035
    ExternallyAggregatedDoubleMetric = 54, // 0x0036
    DoubleMetric = 55, // 0x0037
    ExternallyAggregatedULongDistributionMetric = 56, // 0x0038
    ExternallyAggregatedDoubleDistributionMetric = 57, // 0x0039
    ExternallyAggregatedDoubleScaledToLongDistributionMetric = 58, // 0x003A
  }
}
