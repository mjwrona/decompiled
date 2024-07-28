// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.SystemInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class SystemInfo
  {
    [JsonProperty(PropertyName = "resource")]
    internal string Resource => "HostMachine";

    [JsonProperty(PropertyName = "metricInfo")]
    internal MetricInfo MetricInfo { get; set; }

    internal SystemInfo(string metricsName, string unitName) => this.MetricInfo = new MetricInfo(metricsName, unitName);

    internal SystemInfo(string metricsName, string unitName, int count) => this.MetricInfo = new MetricInfo(metricsName, unitName, count: (long) count);

    public SystemInfo(MetricInfo metricInfo) => this.MetricInfo = metricInfo;

    internal void SetAggregators(LongConcurrentHistogram histogram, double adjustment = 1.0) => this.MetricInfo.SetAggregators(histogram, adjustment);
  }
}
