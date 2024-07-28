// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.IRawMetricConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public interface IRawMetricConfiguration : IMetricConfiguration
  {
    float? ScalingFactor { get; set; }

    bool EnableClientPublication { get; set; }

    bool EnableClientForking { get; set; }

    bool EnableClientEtwPublication { get; set; }

    IEnumerable<SamplingType> RawSamplingTypes { get; }

    IEnumerable<IPreaggregation> Preaggregations { get; }

    IEnumerable<string> Dimensions { get; }

    IEnumerable<IComputedSamplingTypeExpression> ComputedSamplingTypes { get; }

    bool EnableClientSideLastSamplingMode { get; }

    bool CanAddPreaggregation(IPreaggregation preaggregationToAdd);

    void AddPreaggregation(IPreaggregation preaggregate);

    void RemovePreaggregation(string preaggregateName);

    void AddComputedSamplingType(
      IComputedSamplingTypeExpression computedSamplingType);

    void RemoveComputedSamplingType(string computedSamplingTypeName);
  }
}
