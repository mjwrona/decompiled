// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferV2
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class OfferV2 : Offer
  {
    internal OfferV2()
    {
      this.OfferType = string.Empty;
      this.OfferVersion = "V2";
    }

    public OfferV2(int offerThroughput)
      : this()
    {
      this.Content = new OfferContentV2(offerThroughput);
    }

    public OfferV2(int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
      : this()
    {
      this.Content = new OfferContentV2(offerThroughput, offerEnableRUPerMinuteThroughput);
    }

    public OfferV2(Offer offer, int offerThroughput)
      : base(offer)
    {
      this.OfferType = string.Empty;
      this.OfferVersion = "V2";
      OfferContentV2 content = (OfferContentV2) null;
      if (offer is OfferV2)
        content = ((OfferV2) offer).Content;
      this.Content = new OfferContentV2(content, offerThroughput, new bool?());
    }

    public OfferV2(Offer offer, int offerThroughput, bool? offerEnableRUPerMinuteThroughput)
      : base(offer)
    {
      this.OfferType = string.Empty;
      this.OfferVersion = "V2";
      OfferContentV2 content = (OfferContentV2) null;
      if (offer is OfferV2)
        content = ((OfferV2) offer).Content;
      this.Content = new OfferContentV2(content, offerThroughput, offerEnableRUPerMinuteThroughput);
    }

    internal OfferV2(Offer offer, int offerThroughput, double? bgTaskMaxAllowedThroughputPercent)
      : base(offer)
    {
      this.OfferType = string.Empty;
      this.OfferVersion = "V2";
      OfferContentV2 content = (OfferContentV2) null;
      if (offer is OfferV2)
        content = ((OfferV2) offer).Content;
      this.Content = new OfferContentV2(content, offerThroughput, new bool?(), bgTaskMaxAllowedThroughputPercent);
    }

    [JsonProperty(PropertyName = "content", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public OfferContentV2 Content
    {
      get => this.GetObject<OfferContentV2>("content");
      internal set => this.SetObject<OfferContentV2>("content", value);
    }

    internal override void Validate()
    {
      base.Validate();
      this.Content?.Validate();
    }

    public bool Equals(OfferV2 offer)
    {
      if (offer == null || !this.Equals((Offer) offer))
        return false;
      if (this.Content == null && offer.Content == null)
        return true;
      if (this.Content == null || offer.Content == null || this.Content.OfferThroughput != offer.Content.OfferThroughput)
        return false;
      bool? throughputEnabled1 = this.Content.OfferIsRUPerMinuteThroughputEnabled;
      bool? throughputEnabled2 = offer.Content.OfferIsRUPerMinuteThroughputEnabled;
      return throughputEnabled1.GetValueOrDefault() == throughputEnabled2.GetValueOrDefault() & throughputEnabled1.HasValue == throughputEnabled2.HasValue;
    }
  }
}
