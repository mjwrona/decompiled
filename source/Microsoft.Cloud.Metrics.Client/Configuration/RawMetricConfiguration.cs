// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.RawMetricConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class RawMetricConfiguration : IRawMetricConfiguration, IMetricConfiguration
  {
    private readonly List<IPreaggregation> preaggregations;
    private readonly List<IComputedSamplingTypeExpression> computedSamplingTypes;
    private string description;

    [JsonConstructor]
    internal RawMetricConfiguration(
      string metricNamespace,
      string name,
      DateTime lastUpdatedTime,
      string lastUpdatedBy,
      uint version,
      float? scalingFactor,
      bool enableClientPublication,
      bool enableClientForking,
      string description,
      IEnumerable<string> dimensions,
      IEnumerable<IPreaggregation> preaggregations,
      IEnumerable<SamplingType> rawSamplingTypes,
      IEnumerable<IComputedSamplingTypeExpression> computedSamplingTypes,
      bool useClientSideLastSamplingMode,
      bool useClientSideEtwPublication)
    {
      this.MetricNamespace = metricNamespace;
      this.Name = name;
      this.LastUpdatedTime = lastUpdatedTime;
      this.LastUpdatedBy = lastUpdatedBy;
      this.Version = version;
      this.ScalingFactor = scalingFactor;
      this.EnableClientPublication = enableClientPublication;
      this.EnableClientForking = enableClientForking;
      this.Description = description;
      this.Dimensions = dimensions;
      this.preaggregations = preaggregations.ToList<IPreaggregation>();
      this.RawSamplingTypes = rawSamplingTypes;
      this.computedSamplingTypes = computedSamplingTypes.ToList<IComputedSamplingTypeExpression>();
      this.EnableClientSideLastSamplingMode = useClientSideLastSamplingMode;
      this.EnableClientEtwPublication = useClientSideEtwPublication;
    }

    public string MetricNamespace { get; }

    public string Name { get; }

    public DateTime LastUpdatedTime { get; }

    public string LastUpdatedBy { get; }

    public uint Version { get; }

    public float? ScalingFactor { get; set; }

    public bool EnableClientPublication { get; set; }

    public bool EnableClientForking { get; set; }

    public string Description
    {
      get => this.description;
      set => this.description = value == null || value.Length <= 1024 ? value : throw new ArgumentOutOfRangeException(nameof (value), string.Format("The metric description cannot be greater than {0} characters.", (object) 1024));
    }

    public IEnumerable<SamplingType> RawSamplingTypes { get; }

    public IEnumerable<IPreaggregation> Preaggregations => (IEnumerable<IPreaggregation>) this.preaggregations;

    public IEnumerable<string> Dimensions { get; }

    public IEnumerable<IComputedSamplingTypeExpression> ComputedSamplingTypes => (IEnumerable<IComputedSamplingTypeExpression>) this.computedSamplingTypes;

    [JsonProperty]
    public bool EnableClientSideLastSamplingMode { get; set; }

    public bool EnableClientEtwPublication { get; set; }

    public bool CanAddPreaggregation(IPreaggregation preaggregationToAdd)
    {
      if (preaggregationToAdd == null)
        throw new ArgumentNullException(nameof (preaggregationToAdd));
      foreach (IPreaggregation preaggregation in this.preaggregations)
      {
        if (string.Equals(preaggregationToAdd.Name, preaggregation.Name, StringComparison.OrdinalIgnoreCase) || preaggregation.Dimensions.SequenceEqual<string>(preaggregationToAdd.Dimensions, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    public void AddPreaggregation(IPreaggregation preaggregate)
    {
      if (!this.CanAddPreaggregation(preaggregate))
        throw new ConfigurationValidationException("Duplicate preaggregates cannot be added.", ValidationType.DuplicatePreaggregate);
      this.preaggregations.Add(preaggregate);
    }

    public void RemovePreaggregation(string preaggregateName)
    {
      if (string.IsNullOrWhiteSpace(preaggregateName))
        throw new ArgumentNullException(nameof (preaggregateName));
      this.preaggregations.RemoveAll((Predicate<IPreaggregation>) (x => string.Equals(x.Name, preaggregateName, StringComparison.OrdinalIgnoreCase)));
    }

    public void AddComputedSamplingType(
      IComputedSamplingTypeExpression computedSamplingType)
    {
      if (this.computedSamplingTypes.Any<IComputedSamplingTypeExpression>((Func<IComputedSamplingTypeExpression, bool>) (x => x.Name.Equals(computedSamplingType.Name, StringComparison.OrdinalIgnoreCase))))
        throw new ConfigurationValidationException("Duplicate computed sampling types cannot be added.", ValidationType.DuplicateSamplingType);
      this.computedSamplingTypes.Add(computedSamplingType);
    }

    public void RemoveComputedSamplingType(string computedSamplingTypeName)
    {
      if (string.IsNullOrWhiteSpace(computedSamplingTypeName))
        throw new ArgumentNullException(nameof (computedSamplingTypeName));
      for (int index = 0; index < this.computedSamplingTypes.Count; ++index)
      {
        if (string.Equals(computedSamplingTypeName, this.computedSamplingTypes[index].Name, StringComparison.OrdinalIgnoreCase))
        {
          if (this.computedSamplingTypes[index].IsBuiltIn)
            throw new ConfigurationValidationException("Built in computed sampling types cannot be removed.", ValidationType.BuiltInTypeRemoved);
          this.computedSamplingTypes.RemoveAt(index);
          break;
        }
      }
    }
  }
}
