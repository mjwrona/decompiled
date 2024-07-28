// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification.BundlePurchaseEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification
{
  public class BundlePurchaseEmailFormatter : PurchaseEmailBaseFormatter
  {
    public static bool IsOfferMeterApplicable(OfferMeter offerMeter) => offerMeter.Category == MeterCategory.Bundle;

    public override void SetEmailBody(ResourceRenewalGroup renewalGroup)
    {
      if (renewalGroup == ResourceRenewalGroup.Monthly)
        this.Body = HostingResources.VSOSubscriptionMonthlyEmailTemplate();
      else
        this.Body = HostingResources.VSOExtensionAnnualEmailTemplate();
    }

    public override void SetEmailSubject() => this.Subject = HostingResources.VSOExtensionOrBundlePurchaseEmailSubject();

    private void PopulateEmailLinks(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter,
      ResourceRenewalGroup renewalGroup)
    {
      string str = string.Format("install/{0}?accountId={1}", (object) offerMeter.GalleryId, (object) collectionContext.ServiceHost.InstanceId.ToString());
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      this.ManageUsersLink = this.GetLocationServiceUrl(requestContext, CommerceConstants.MsdnAdminGuid, "https://go.microsoft.com/fwlink/?linkid=842050");
      this.BuyMoreLink = this.GetLocationServiceUrl(requestContext, CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + str;
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
      OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
      this.DisplayName = collectionContext.GetUserIdentity().DisplayName;
      this.SubscriptionName = this.GetSubscriptionName(collectionContext, azureSubscriptionId);
      this.ExtensionName = offerMeter.Name;
      this.Quantity = quantity.ToString();
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.Price = this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
      this.PopulateEmailLinks(collectionContext, offerMeter, renewalGroup);
    }

    private string ManageUsersLink
    {
      set => this.Attributes[nameof (ManageUsersLink)] = value;
    }

    private string BuyMoreLink
    {
      set => this.Attributes[nameof (BuyMoreLink)] = value;
    }
  }
}
