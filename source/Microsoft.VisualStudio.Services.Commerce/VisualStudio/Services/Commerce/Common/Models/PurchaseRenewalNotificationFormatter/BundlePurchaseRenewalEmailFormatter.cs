// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseRenewalNotificationFormatter.BundlePurchaseRenewalEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseRenewalNotificationFormatter
{
  public class BundlePurchaseRenewalEmailFormatter : PurchaseEmailBaseFormatter
  {
    private string editQueryStringForEnterpriseProfessionalBundlePurchase = "items?itemName={0}&install=true&subscriptionId={1}&changeQuantity=true";
    private const string editLinkRegistryPath = "/Service/Commerce/Emails/Renewal/EditLink";

    public static bool IsOfferMeterApplicable(OfferMeter offerMeter) => offerMeter.Category == MeterCategory.Bundle;

    public override void SetEmailBody(ResourceRenewalGroup renewalGroup) => this.Body = HostingResources.VSOBundlePurchaseRenewalEmailTemplate();

    public override void SetEmailSubject() => this.Subject = HostingResources.VSOBundlePurchaseRenewalEmailSubject();

    private void PopulateEmailLinks(
      IVssRequestContext collectionContext,
      string galleryId,
      Guid azureSubscriptionId)
    {
      string format = collectionContext.GetService<CachedRegistryService>().GetValue<string>(collectionContext, (RegistryQuery) "/Service/Commerce/Emails/Renewal/EditLink", this.editQueryStringForEnterpriseProfessionalBundlePurchase);
      this.EditLink = this.GetLocationServiceUrl(collectionContext.To(TeamFoundationHostType.Deployment), CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + string.Format(format, (object) galleryId, (object) azureSubscriptionId);
    }

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
      this.ExtensionName = this.GetOfferMeter(collectionContext, offerMeterName).Name;
      this.SubscriptionName = this.GetSubscriptionName(collectionContext, azureSubscriptionId);
      this.Quantity = quantity.ToString();
      this.RenewalDate = renewalDate.ToString("MMMM d, yyyy");
      this.Price = this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
      this.PopulateEmailLinks(collectionContext, galleryId, azureSubscriptionId);
    }

    protected string RenewalDate
    {
      set => this.Attributes[nameof (RenewalDate)] = value;
    }

    private string EditLink
    {
      set => this.Attributes[nameof (EditLink)] = value;
    }
  }
}
