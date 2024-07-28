// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.OfferAutoscaleAutoUpgradeProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class OfferAutoscaleAutoUpgradeProperties
  {
    [JsonConstructor]
    internal OfferAutoscaleAutoUpgradeProperties()
    {
    }

    internal OfferAutoscaleAutoUpgradeProperties(int incrementPercent) => this.ThroughputProperties = new OfferAutoscaleAutoUpgradeProperties.AutoscaleThroughputProperties(incrementPercent);

    [JsonProperty(PropertyName = "throughputPolicy", NullValueHandling = NullValueHandling.Ignore)]
    public OfferAutoscaleAutoUpgradeProperties.AutoscaleThroughputProperties ThroughputProperties { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    internal string GetJsonString() => JsonConvert.SerializeObject((object) this, Formatting.None);

    public class AutoscaleThroughputProperties
    {
      public AutoscaleThroughputProperties(int incrementPercent) => this.IncrementPercent = incrementPercent;

      [JsonProperty(PropertyName = "incrementPercent", NullValueHandling = NullValueHandling.Ignore)]
      public int IncrementPercent { get; private set; }

      [JsonExtensionData]
      internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
    }
  }
}
