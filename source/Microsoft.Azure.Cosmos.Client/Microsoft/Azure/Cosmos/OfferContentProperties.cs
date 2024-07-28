// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.OfferContentProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal class OfferContentProperties
  {
    [JsonConstructor]
    internal OfferContentProperties()
    {
    }

    internal OfferContentProperties(int manualThroughput)
    {
      this.OfferThroughput = new int?(manualThroughput);
      this.OfferAutoscaleSettings = (OfferAutoscaleProperties) null;
    }

    internal OfferContentProperties(OfferAutoscaleProperties autoscaleProperties)
    {
      this.OfferThroughput = new int?();
      this.OfferAutoscaleSettings = autoscaleProperties ?? throw new ArgumentNullException(nameof (autoscaleProperties));
    }

    [JsonProperty(PropertyName = "offerThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? OfferThroughput { get; private set; }

    [JsonProperty(PropertyName = "offerAutopilotSettings", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public OfferAutoscaleProperties OfferAutoscaleSettings { get; private set; }

    [JsonProperty(PropertyName = "offerIsRUPerMinuteThroughputEnabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? OfferIsRUPerMinuteThroughputEnabled { get; private set; }

    [JsonProperty(PropertyName = "offerLastReplaceTimestamp", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal long? OfferLastReplaceTimestamp { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    public static OfferContentProperties CreateManualOfferConent(int throughput) => new OfferContentProperties(throughput);

    public static OfferContentProperties CreateAutoscaleOfferConent(
      int startingMaxThroughput,
      int? autoUpgradeMaxThroughputIncrementPercentage)
    {
      return new OfferContentProperties(new OfferAutoscaleProperties(startingMaxThroughput, autoUpgradeMaxThroughputIncrementPercentage));
    }
  }
}
