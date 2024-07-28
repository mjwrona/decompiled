// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification.ExtensionPurchaseEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification
{
  public class ExtensionPurchaseEmailFormatter : PurchaseEmailBaseFormatter
  {
    public static bool IsOfferMeterApplicable(OfferMeter offerMeter) => offerMeter.Category == MeterCategory.Extension;

    public override void SetEmailBody(ResourceRenewalGroup renewalGroup) => this.Body = HostingResources.VSOExtensionEmailTemplate();

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
      OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
      this.DisplayName = collectionContext.GetUserIdentity().DisplayName;
      this.SubscriptionName = this.GetSubscriptionName(collectionContext, azureSubscriptionId);
      this.ExtensionName = offerMeter.Name;
      this.Quantity = quantity.ToString();
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.Price = this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
      this.PopulateEmailLinks(collectionContext, offerMeter);
    }

    private void PopulateEmailLinks(IVssRequestContext collectionContext, OfferMeter offerMeter)
    {
      this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter);
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      string str = string.Format("install/{0}?accountId={1}", (object) offerMeter.GalleryId, (object) collectionContext.ServiceHost.InstanceId.ToString());
      this.BuyMoreLink = this.GetLocationServiceUrl(requestContext, CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + str;
    }

    private string BuyMoreLink
    {
      set => this.Attributes[nameof (BuyMoreLink)] = value;
    }
  }
}
