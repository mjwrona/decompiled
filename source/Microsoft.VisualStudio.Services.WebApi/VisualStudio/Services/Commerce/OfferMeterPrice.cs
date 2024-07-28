// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterPrice
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OfferMeterPrice : IOfferMeterPrice
  {
    public string MeterName { get; set; }

    public string PlanName { get; set; }

    public string Region { get; set; }

    public string CurrencyCode { get; set; }

    public double Quantity { get; set; }

    public double Price { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is OfferMeterPrice))
        return false;
      OfferMeterPrice offerMeterPrice = (OfferMeterPrice) obj;
      return string.Equals(this.MeterName, offerMeterPrice.MeterName) && string.Equals(this.PlanName, offerMeterPrice.PlanName) && string.Equals(this.Region, offerMeterPrice.Region) && string.Equals(this.CurrencyCode, offerMeterPrice.CurrencyCode) && this.Quantity == offerMeterPrice.Quantity && this.Price == offerMeterPrice.Price;
    }

    public override int GetHashCode() => this.MeterName.GetHashCode() + this.PlanName.GetHashCode() + this.Region.GetHashCode();
  }
}
