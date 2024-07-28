// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.Preaggregation
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class Preaggregation : IPreaggregation
  {
    private readonly List<string> dimensions;
    private string name;
    private IMinMaxConfiguration minMaxConfiguration;
    private IPercentileConfiguration percentileConfiguration;
    private IRollupConfiguration rollupConfiguration;
    private IPublicationConfiguration publicationConfiguration;
    private IDistinctCountConfiguration distinctCountConfiguration;
    private IFilteringConfiguration filteringConfiguration;
    private DateTime disabledTimeInUtc;

    [JsonConstructor]
    internal Preaggregation(
      string name,
      IEnumerable<string> dimensions,
      DateTime disabledTimeInUtc,
      Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration minMaxConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration percentileConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration rollupConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration publicationConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration distinctCountConfiguration,
      IFilteringConfiguration filteringConfiguration)
    {
      this.Name = name;
      this.dimensions = dimensions.ToList<string>();
      this.disabledTimeInUtc = disabledTimeInUtc;
      this.minMaxConfiguration = (IMinMaxConfiguration) minMaxConfiguration;
      this.percentileConfiguration = (IPercentileConfiguration) percentileConfiguration;
      this.rollupConfiguration = (IRollupConfiguration) rollupConfiguration;
      this.publicationConfiguration = (IPublicationConfiguration) publicationConfiguration;
      this.distinctCountConfiguration = (IDistinctCountConfiguration) distinctCountConfiguration;
      this.filteringConfiguration = filteringConfiguration;
    }

    public string Name
    {
      get => this.name;
      set => this.name = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IEnumerable<string> Dimensions => (IEnumerable<string>) this.dimensions;

    public DateTime DisabledTimeInUtc => this.disabledTimeInUtc;

    public IMinMaxConfiguration MinMaxConfiguration
    {
      get => this.minMaxConfiguration;
      set => this.minMaxConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IFilteringConfiguration FilteringConfiguration
    {
      get => this.filteringConfiguration;
      set => this.filteringConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IPercentileConfiguration PercentileConfiguration
    {
      get => this.percentileConfiguration;
      set => this.percentileConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IRollupConfiguration RollupConfiguration
    {
      get => this.rollupConfiguration;
      set => this.rollupConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IPublicationConfiguration PublicationConfiguration
    {
      get => this.publicationConfiguration;
      set => this.publicationConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public IDistinctCountConfiguration DistinctCountConfiguration
    {
      get => this.distinctCountConfiguration;
      set => this.distinctCountConfiguration = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public static Preaggregation CreatePreaggregation(string name, IEnumerable<string> dimensions) => Preaggregation.CreatePreaggregationImpl(name, dimensions, Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration.MinMaxDisabled, Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration.PercentileDisabled, Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration.RollupDisabled, Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration.AggregatedMetricsStore, new Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration(), Microsoft.Cloud.Metrics.Client.Configuration.FilteringConfiguration.FilteringDisabled);

    public static Preaggregation CreatePreaggregationWithDefaults(
      string name,
      IEnumerable<string> dimensions,
      Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration minMaxConfiguration = null,
      Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration percentileConfiguration = null,
      Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration rollupConfiguration = null,
      Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration metricStoreConfiguration = null,
      Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration distinctCountConfiguration = null,
      IFilteringConfiguration filteringConfiguration = null)
    {
      return Preaggregation.CreatePreaggregationImpl(name, dimensions, minMaxConfiguration ?? Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration.MinMaxDisabled, percentileConfiguration ?? Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration.PercentileDisabled, rollupConfiguration ?? Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration.RollupDisabled, metricStoreConfiguration ?? Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration.AggregatedMetricsStore, distinctCountConfiguration ?? new Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration(), filteringConfiguration ?? Microsoft.Cloud.Metrics.Client.Configuration.FilteringConfiguration.FilteringDisabled);
    }

    public static Preaggregation CreatePreaggregation(
      string name,
      IEnumerable<string> dimensions,
      Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration minMaxConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration percentileConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration rollupConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration metricStoreConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration distinctCountConfiguration)
    {
      return Preaggregation.CreatePreaggregationImpl(name, dimensions, minMaxConfiguration, percentileConfiguration, rollupConfiguration, metricStoreConfiguration, distinctCountConfiguration, Microsoft.Cloud.Metrics.Client.Configuration.FilteringConfiguration.FilteringDisabled);
    }

    public void AddDimension(string dimensionToAdd)
    {
      if (string.IsNullOrWhiteSpace(dimensionToAdd))
        throw new ArgumentNullException(nameof (dimensionToAdd));
      int index;
      for (index = 0; index < this.dimensions.Count; ++index)
      {
        int num = string.Compare(this.dimensions[index], dimensionToAdd, StringComparison.OrdinalIgnoreCase);
        if (num == 0)
          throw new ConfigurationValidationException("Cannot add duplicate dimensions.", ValidationType.DuplicateDimension);
        if (num > 0)
          break;
      }
      if (index + 1 == this.dimensions.Count)
        this.dimensions.Add(dimensionToAdd);
      else
        this.dimensions.Insert(index, dimensionToAdd);
    }

    public void RemoveDimension(string dimensionName) => this.dimensions.RemoveAll((Predicate<string>) (x => string.Equals(x, dimensionName, StringComparison.OrdinalIgnoreCase)));

    public void Disable(int hours) => this.disabledTimeInUtc = hours >= 0 ? DateTime.UtcNow.AddHours((double) hours) : throw new ArgumentException(string.Format("A preaggregate cannot be set to be disabled in the past. Hours:{0}", (object) hours), nameof (hours));

    public void Enable() => this.disabledTimeInUtc = DateTime.MaxValue;

    private static Preaggregation CreatePreaggregationImpl(
      string name,
      IEnumerable<string> dimensions,
      Microsoft.Cloud.Metrics.Client.Configuration.MinMaxConfiguration minMaxConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration percentileConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.RollupConfiguration rollupConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration metricStoreConfiguration,
      Microsoft.Cloud.Metrics.Client.Configuration.DistinctCountConfiguration distinctCountConfiguration,
      IFilteringConfiguration filteringConfiguration)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof (name));
      List<string> dimensions1 = dimensions != null ? dimensions.ToList<string>() : throw new ArgumentNullException(nameof (dimensions));
      dimensions1.Sort((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < dimensions1.Count - 1; ++index)
      {
        if (string.Equals(dimensions1[index], dimensions1[index + 1], StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException("Cannot create a preaggregate with duplicate dimensions.");
      }
      if (minMaxConfiguration == null)
        throw new ArgumentNullException(nameof (minMaxConfiguration));
      if (percentileConfiguration == null)
        throw new ArgumentNullException(nameof (percentileConfiguration));
      if (rollupConfiguration == null)
        throw new ArgumentNullException(nameof (rollupConfiguration));
      if (metricStoreConfiguration == null)
        throw new ArgumentNullException(nameof (metricStoreConfiguration));
      if (distinctCountConfiguration == null)
        throw new ArgumentNullException(nameof (distinctCountConfiguration));
      if (filteringConfiguration == null)
        throw new ArgumentNullException(nameof (filteringConfiguration));
      return new Preaggregation(name, (IEnumerable<string>) dimensions1, new DateTime(), minMaxConfiguration, percentileConfiguration, rollupConfiguration, metricStoreConfiguration, distinctCountConfiguration, filteringConfiguration);
    }
  }
}
