// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionQuantityChangeMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OfferSubscriptionQuantityChangeMessage
  {
    public OfferSubscriptionQuantityChangeMessage()
    {
    }

    public OfferSubscriptionQuantityChangeMessage(OfferSubscriptionQuantityChangeMessage copy)
    {
      this.EventType = copy.EventType;
      this.SubscriptionId = copy.SubscriptionId;
      this.VsoAccountId = copy.VsoAccountId;
      this.SubscriptionStatus = copy.SubscriptionStatus;
      this.TenantId = copy.TenantId;
      this.ActivityId = copy.ActivityId;
      this.PurchaserAccountName = copy.PurchaserAccountName;
      this.OfferMeterName = copy.OfferMeterName;
      this.OfferMeterCategory = copy.OfferMeterCategory;
      this.Quantity = copy.Quantity;
      this.NextBillingCycleQuantity = copy.NextBillingCycleQuantity;
      this.RenewalGroup = copy.RenewalGroup;
      this.NextResetTime = copy.NextResetTime;
      this.MeterGalleryId = copy.MeterGalleryId;
    }

    public OfferSubscriptionQuantityChangeEventType EventType { get; set; }

    public Guid SubscriptionId { get; set; }

    public Guid VsoAccountId { get; set; }

    public SubscriptionStatus SubscriptionStatus { get; set; }

    public string TenantId { get; set; }

    public Guid ActivityId { get; set; }

    public string PurchaserAccountName { get; set; }

    public string OfferMeterName { get; set; }

    public MeterCategory OfferMeterCategory { get; set; }

    public int Quantity { get; set; }

    public ResourceRenewalGroup RenewalGroup { get; set; }

    public int NextBillingCycleQuantity { get; set; }

    public MeterRenewalFrequecy RenewalFrequency { get; set; }

    public DateTime NextResetTime { get; set; }

    public DateTime MessageCreateTime { get; set; }

    public string MeterGalleryId { get; set; }
  }
}
