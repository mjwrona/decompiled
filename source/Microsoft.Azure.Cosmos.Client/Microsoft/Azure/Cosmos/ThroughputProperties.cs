// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ThroughputProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class ThroughputProperties
  {
    [JsonConstructor]
    internal ThroughputProperties()
    {
    }

    internal ThroughputProperties(OfferContentProperties offerContentProperties)
    {
      this.OfferVersion = "V2";
      this.Content = offerContentProperties;
    }

    [JsonProperty(PropertyName = "_etag", NullValueHandling = NullValueHandling.Ignore)]
    public string ETag { get; private set; }

    [JsonConverter(typeof (UnixDateTimeConverter))]
    [JsonProperty(PropertyName = "_ts", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastModified { get; private set; }

    [JsonIgnore]
    public int? Throughput
    {
      get => this.Content?.OfferThroughput;
      private set => this.Content = OfferContentProperties.CreateManualOfferConent(value.Value);
    }

    [JsonIgnore]
    public int? AutoscaleMaxThroughput => this.Content?.OfferAutoscaleSettings?.MaxThroughput;

    [JsonIgnore]
    internal int? AutoUpgradeMaxThroughputIncrementPercentage => this.Content?.OfferAutoscaleSettings?.AutoscaleAutoUpgradeProperties?.ThroughputProperties?.IncrementPercent;

    public static ThroughputProperties CreateManualThroughput(int throughput) => throughput > 0 ? new ThroughputProperties(OfferContentProperties.CreateManualOfferConent(throughput)) : throw new ArgumentOutOfRangeException("throughput must be greater than 0");

    public static ThroughputProperties CreateAutoscaleThroughput(int autoscaleMaxThroughput) => autoscaleMaxThroughput > 0 ? new ThroughputProperties(OfferContentProperties.CreateAutoscaleOfferConent(autoscaleMaxThroughput, new int?())) : throw new ArgumentOutOfRangeException("autoscaleMaxThroughput must be greater than 0");

    internal static ThroughputProperties CreateManualThroughput(int? throughput) => !throughput.HasValue ? (ThroughputProperties) null : ThroughputProperties.CreateManualThroughput(throughput.Value);

    internal static ThroughputProperties CreateAutoscaleThroughput(
      int maxAutoscaleThroughput,
      int? autoUpgradeMaxThroughputIncrementPercentage = null)
    {
      return new ThroughputProperties(OfferContentProperties.CreateAutoscaleOfferConent(maxAutoscaleThroughput, autoUpgradeMaxThroughputIncrementPercentage));
    }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public string SelfLink { get; private set; }

    [JsonProperty(PropertyName = "_rid", NullValueHandling = NullValueHandling.Ignore)]
    internal string OfferRID { get; private set; }

    [JsonProperty(PropertyName = "offerResourceId", NullValueHandling = NullValueHandling.Ignore)]
    internal string ResourceRID { get; private set; }

    [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal OfferContentProperties Content { get; set; }

    [JsonProperty(PropertyName = "offerVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal string OfferVersion { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
