// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferContentV2
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  public sealed class OfferContentV2 : JsonSerializable
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

    [JsonProperty(PropertyName = "offerThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int OfferThroughput
    {
      get => this.GetValue<int>("offerThroughput");
      private set => this.SetValue("offerThroughput", (object) value);
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
    }
  }
}
