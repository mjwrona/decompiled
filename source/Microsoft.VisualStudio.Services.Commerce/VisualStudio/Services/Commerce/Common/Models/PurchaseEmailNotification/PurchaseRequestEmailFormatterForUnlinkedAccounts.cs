// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification.PurchaseRequestEmailFormatterForUnlinkedAccounts
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification
{
  internal class PurchaseRequestEmailFormatterForUnlinkedAccounts : PurchaseRequestEmailFormatter
  {
    internal PurchaseRequestEmailFormatterForUnlinkedAccounts(PurchaseRequest request)
      : base(request)
    {
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
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = collectionContext.GetUserIdentity();
      OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
      this.DisplayName = userIdentity.DisplayName;
      this.ExtensionName = offerMeter.Name;
      this.Quantity = quantity.ToString();
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.RequestorEmail = userIdentity.GetProperty<string>("Mail", string.Empty);
      this.RequestMessage = this.PurchaseRequest.Reason;
      this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter);
      this.AcceptRequestLink = new Uri(this.GetLocationServiceUrl(collectionContext, ServiceInstanceTypes.TFS, "https://marketplace.visualstudio.com/") + "/_apis/purchaserequest/handle").AppendQuery("itemName", offerMeter.GalleryId).AppendQuery("accountId", collectionContext.ServiceHost.InstanceId.ToString()).AppendQuery("subscriptionId", Guid.Empty.ToString()).AppendQuery(nameof (quantity), quantity.ToString()).ToString() + "&purchaseResponse=" + Enum.GetName(typeof (PurchaseRequestResponse), (object) PurchaseRequestResponse.Approved);
    }

    public override void SetEmailBody(ResourceRenewalGroup renewalGroup) => this.Body = HostingResources.VSOPurchaseRequestEmailForUnlinkedAccounts();
  }
}
