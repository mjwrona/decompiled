// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification.HockeyAppPurchaseEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification
{
  public class HockeyAppPurchaseEmailFormatter : PurchaseEmailBaseFormatter
  {
    private const string hockeyAppPrefix = "ms.hockeyapp";

    public static bool IsOfferMeterApplicable(OfferMeter offerMeter) => !string.IsNullOrWhiteSpace(offerMeter.GalleryId) && offerMeter.GalleryId.StartsWith("ms.hockeyapp");

    public override void SetEmailBody(ResourceRenewalGroup renewalGroup) => this.Body = HostingResources.HockeyAppEmailTemplate();

    public override void SetEmailSubject() => this.Subject = HostingResources.VSOExtensionOrBundlePurchaseEmailSubject();

    public override void SetEmailAttributes(
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId)
    {
      this.DisplayName = collectionContext.GetUserIdentity().DisplayName;
      this.SubscriptionName = this.GetSubscriptionName(collectionContext, azureSubscriptionId);
      this.ExtensionName = this.GetOfferMeter(collectionContext, offerMeterName).Name;
      this.Quantity = quantity.ToString();
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.Price = this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
    }
  }
}
