// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.PurchaseRequestNotification
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  internal class PurchaseRequestNotification : PurchaseNotification
  {
    private const string Layer = "PurchaseRequestNotification";
    private const string ExtensionPurchaseRequestType = "request";

    public virtual void SendPurchaseRequestEmail(
      IVssRequestContext collectionContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string requestMessage)
    {
      if (offerMeterName == null)
      {
        collectionContext.Trace(5109164, TraceLevel.Error, "Commerce", nameof (PurchaseRequestNotification), string.Format("No notification is published since offerMeterName is null. Azure Subscription Id: {0}.", (object) azureSubscriptionId));
      }
      else
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.GetIdentityList(collectionContext, azureSubscriptionId, offerMeterName);
        identityList.Add(identity);
        OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = collectionContext.GetUserIdentity();
        bool flag = azureSubscriptionId == Guid.Empty;
        string vsoAccountLink;
        string manageExtensionUsersLink;
        this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter, out vsoAccountLink, out manageExtensionUsersLink);
        ExtensionPurchaseEvent extensionPurchaseEvent = new ExtensionPurchaseEvent();
        extensionPurchaseEvent.NotificationType = "request";
        extensionPurchaseEvent.DisplayName = userIdentity.DisplayName;
        extensionPurchaseEvent.SubscriptionName = flag ? "" : this.GetSubscriptionName(collectionContext, azureSubscriptionId);
        extensionPurchaseEvent.ExtensionName = offerMeterName;
        extensionPurchaseEvent.Quantity = quantity.ToString();
        extensionPurchaseEvent.AccountName = CollectionHelper.GetCollectionName(collectionContext);
        extensionPurchaseEvent.Price = flag ? "" : this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
        extensionPurchaseEvent.RequestorEmail = userIdentity.GetProperty<string>("Mail", string.Empty);
        extensionPurchaseEvent.AccountLink = vsoAccountLink;
        extensionPurchaseEvent.ManageExtensionUsersLink = manageExtensionUsersLink;
        extensionPurchaseEvent.AcceptRequestLink = this.PopulateAcceptRequestLink(collectionContext, offerMeter, quantity, azureSubscriptionId);
        extensionPurchaseEvent.RequestMessage = requestMessage;
        ExtensionPurchaseEvent emailEvent = extensionPurchaseEvent;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
          this.PublishNotificationEvent(collectionContext, identity1, (object) emailEvent);
      }
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentityList(
      IVssRequestContext collectionContext,
      Guid azureSubscriptionId,
      string offerMeterName)
    {
      CommunicatorHelper communicatorHelper = new CommunicatorHelper();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (azureSubscriptionId.Equals(Guid.Empty))
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> administratorIdentities = communicatorHelper.GetProjectCollectionAdministratorIdentities(collectionContext);
        collectionContext.TraceAlways(5109204, TraceLevel.Info, "Commerce", nameof (PurchaseRequestNotification), string.Format("SubscriptionId {0} OfferMeterName {1} PCAIdentityCount {2} in case of unlinked accounts", (object) azureSubscriptionId, (object) offerMeterName, (object) administratorIdentities.Count));
        return administratorIdentities;
      }
      IVssRequestContext collectionContext1 = collectionContext.Elevate();
      IList<string> stringList = communicatorHelper.GetAdminCoAdminMailAddresses(collectionContext1, azureSubscriptionId) ?? (IList<string>) new List<string>();
      foreach (string accountName in (IEnumerable<string>) stringList)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identityFromEmail = this.GetIdentityFromEmail(collectionContext, accountName);
        if (identityFromEmail != null)
        {
          collectionContext.Trace(5109202, TraceLevel.Info, "Commerce", nameof (PurchaseRequestNotification), "Returned identity's CUID for an Azure subsription's Admin/Coadmin " + string.Format("email is {0}.", (object) identityFromEmail.GetProperty<Guid>("CUID", new Guid())));
          identityList.Add(identityFromEmail);
        }
        else
          collectionContext.Trace(5109203, TraceLevel.Error, "Commerce", nameof (PurchaseRequestNotification), "Returned identity is null for an Azure subsription's Admin/Coadmin email.");
      }
      collectionContext.TraceAlways(5109201, TraceLevel.Info, "Commerce", nameof (PurchaseRequestNotification), string.Format("SubscriptionId {0} OfferMeterName {1} AdminCoAdminEmailListCount ", (object) azureSubscriptionId, (object) offerMeterName) + string.Format("{0} TargetIdentityList {1} in case of linked accounts", (object) (stringList != null ? stringList.Count : 0), (object) identityList.Count));
      return identityList;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetIdentityFromEmail(
      IVssRequestContext collectionContext,
      string accountName)
    {
      IdentityService service = collectionContext.GetService<IdentityService>();
      Guid organizationAadTenantId = collectionContext.GetOrganizationAadTenantId();
      try
      {
        if (!collectionContext.IsOrganizationAadBacked())
          return service.ReadIdentities(collectionContext, IdentitySearchFilter.AccountName, accountName, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity.GetProperty<string>("Domain", string.Empty).Equals("Windows Live ID", StringComparison.InvariantCultureIgnoreCase))).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsBindPending)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsCspPartnerUser)).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        IdentityDescriptor descriptorFromAccountName = IdentityHelper.CreateDescriptorFromAccountName(organizationAadTenantId.ToString(), accountName);
        return service.ReadIdentities(collectionContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptorFromAccountName
        }, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsBindPending)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsCspPartnerUser)).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (IdentityNotFoundException ex)
      {
        IdentityDescriptor identityDescriptor = IdentityAuthenticationHelper.BuildTemporaryDescriptorFromEmailAddress(accountName);
        return service.ReadIdentities(collectionContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
    }

    protected string PopulateAcceptRequestLink(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter,
      int quantity,
      Guid azureSubscriptionId)
    {
      return new Uri(this.GetLocationServiceUrl(collectionContext, ServiceInstanceTypes.TFS, "https://marketplace.visualstudio.com/") + "/_apis/purchaserequest/handle").AppendQuery("itemName", offerMeter.GalleryId).AppendQuery("accountId", collectionContext.ServiceHost.InstanceId.ToString()).AppendQuery("subscriptionId", azureSubscriptionId.ToString()).AppendQuery(nameof (quantity), quantity.ToString()).ToString() + "&purchaseResponse=" + Enum.GetName(typeof (PurchaseRequestResponse), (object) PurchaseRequestResponse.Approved);
    }
  }
}
