// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.OfferMeterEntry
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class OfferMeterEntry
  {
    [JsonProperty("c")]
    public string CurrencyCode;
    [JsonProperty("r")]
    public string Region;
    [JsonProperty("n")]
    public string CurrencyName;
    [JsonProperty("s")]
    public string CurrencySymbol;
    [JsonProperty("m")]
    public List<MeterPrice> MeterPrices;
  }
}
