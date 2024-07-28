// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Offer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  public class Offer : Resource
  {
    public Offer() => this.OfferVersion = "V1";

    public Offer(Offer offer)
      : base((Resource) offer)
    {
      this.OfferVersion = "V1";
      this.ResourceLink = offer.ResourceLink;
      this.OfferType = offer.OfferType;
      this.OfferResourceId = offer.OfferResourceId;
    }

    [JsonProperty(PropertyName = "offerVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string OfferVersion
    {
      get => this.GetValue<string>("offerVersion");
      internal set => this.SetValue("offerVersion", (object) value);
    }

    [JsonProperty(PropertyName = "resource")]
    public string ResourceLink
    {
      get => this.GetValue<string>("resource");
      internal set => this.SetValue("resource", (object) value);
    }

    [JsonProperty(PropertyName = "offerType", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string OfferType
    {
      get => this.GetValue<string>("offerType");
      set => this.SetValue("offerType", (object) value);
    }

    [JsonProperty(PropertyName = "offerResourceId")]
    internal string OfferResourceId
    {
      get => this.GetValue<string>("offerResourceId");
      set => this.SetValue("offerResourceId", (object) value);
    }

    internal override void Validate()
    {
      base.Validate();
      this.GetValue<string>("offerVersion");
      this.GetValue<string>("resource");
      this.GetValue<string>("offerType");
    }

    public bool Equals(Offer offer) => this.OfferVersion.Equals(offer.OfferVersion) && this.OfferResourceId.Equals(offer.OfferResourceId) && (!this.OfferVersion.Equals("V1") || this.OfferType.Equals(offer.OfferType));
  }
}
