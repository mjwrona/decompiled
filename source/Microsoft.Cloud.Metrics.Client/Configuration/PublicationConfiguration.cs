// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.PublicationConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class PublicationConfiguration : IPublicationConfiguration
  {
    public static readonly PublicationConfiguration MetricStore = new PublicationConfiguration(true, true, false);
    public static readonly PublicationConfiguration CacheServer = new PublicationConfiguration(false, false, false);
    public static readonly PublicationConfiguration CacheServerAndRawMetricsStore = new PublicationConfiguration(true, false, false);
    public static readonly PublicationConfiguration CacheServerAndAggregatedMetricsStore = new PublicationConfiguration(true, false, true);
    public static readonly PublicationConfiguration AggregatedMetricsStore = new PublicationConfiguration(true, true, true);

    [JsonConstructor]
    internal PublicationConfiguration(
      bool metricStorePublicationEnabled,
      bool cacheServerPublicationDisabled,
      bool aggregatedMetricsStorePublication)
    {
      this.MetricStorePublicationEnabled = metricStorePublicationEnabled;
      this.CacheServerPublicationDisabled = cacheServerPublicationDisabled;
      this.AggregatedMetricsStorePublication = aggregatedMetricsStorePublication;
    }

    public bool MetricStorePublicationEnabled { get; }

    public bool CacheServerPublicationDisabled { get; }

    public bool AggregatedMetricsStorePublication { get; }
  }
}
