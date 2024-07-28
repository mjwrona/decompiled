// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferContentV2
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class OfferContentV2 : JsonSerializable
  {
    public OfferContentV2()
      : this(0)
    {
    }

    public OfferContentV2(int offerThroughput)
    {
      this.OfferThroughput = offerThroughput;
      this.OfferIsRUPerMinuteThroughputEnabled = new bool?();
    }

    public OfferContentV2(int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
    {
      this.OfferThroughput = offerThroughput;
      this.OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
    }

    internal OfferContentV2(
      OfferContentV2 content,
      int offerThroughput,
      bool? offerEnableRUPerMinuteThroughput)
    {
      this.OfferThroughput = offerThroughput;
      this.OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
    }

    internal OfferContentV2(
      OfferContentV2 content,
      int offerThroughput,
      bool? offerEnableRUPerMinuteThroughput,
      double? bgTaskMaxAllowedThroughputPercent)
    {
      this.OfferThroughput = offerThroughput;
      this.OfferIsRUPerMinuteThroughputEnabled = offerEnableRUPerMinuteThroughput;
      if (!bgTaskMaxAllowedThroughputPercent.HasValue)
        return;
      this.BackgroundTaskMaxAllowedThroughputPercent = bgTaskMaxAllowedThroughputPercent;
    }

    [JsonProperty(PropertyName = "offerThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int OfferThroughput
    {
      get => this.GetValue<int>("offerThroughput");
      private set => this.SetValue("offerThroughput", (object) value);
    }

    [JsonProperty(PropertyName = "BackgroundTaskMaxAllowedThroughputPercent", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal double? BackgroundTaskMaxAllowedThroughputPercent
    {
      get => this.GetValue<double?>(nameof (BackgroundTaskMaxAllowedThroughputPercent));
      private set => this.SetValue(nameof (BackgroundTaskMaxAllowedThroughputPercent), (object) value);
    }

    [JsonProperty(PropertyName = "offerIsRUPerMinuteThroughputEnabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? OfferIsRUPerMinuteThroughputEnabled
    {
      get => this.GetValue<bool?>("offerIsRUPerMinuteThroughputEnabled");
      private set => this.SetValue("offerIsRUPerMinuteThroughputEnabled", (object) value);
    }

    internal override void Validate()
    {
      this.GetValue<int>("offerThroughput");
      this.GetValue<bool?>("offerIsRUPerMinuteThroughputEnabled");
      this.GetValue<double?>("BackgroundTaskMaxAllowedThroughputPercent");
    }
  }
}
