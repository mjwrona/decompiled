// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Communicators.PurchaseRequestEmailCommunicator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.VisualStudio.Services.Commerce.Common.Models;
using Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Communicators
{
  internal class PurchaseRequestEmailCommunicator : PurchaseNotificationCommunicator
  {
    private PurchaseRequest purchaseRequest;
    private Guid? subscriptionId;

    public PurchaseRequestEmailCommunicator(PurchaseRequest request, Guid? azureSubscriptionId)
    {
      this.purchaseRequest = request;
      this.subscriptionId = azureSubscriptionId;
    }

    protected override void SendEmails(
      IVssRequestContext collectionContext,
      string galleryId,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      PurchaseEmailBaseFormatter formatter)
    {
      CommunicatorHelper communicatorHelper = new CommunicatorHelper();
      IVssRequestContext collectionContext1 = collectionContext.Elevate();
      IList<string> emailList;
      if (!azureSubscriptionId.Equals(Guid.Empty))
      {
        emailList = communicatorHelper.GetAdminCoAdminMailAddresses(collectionContext1, azureSubscriptionId) ?? (IList<string>) new List<string>();
        collectionContext.TraceAlways(5109199, TraceLevel.Info, "Commerce", "PurchaseNotificationCommunicator", string.Format("SubscriptionId {0} GalleryId {1} EmailAddressCount {2} in case of linked accounts", (object) azureSubscriptionId, (object) galleryId, (object) (emailList != null ? emailList.Count : 0)));
      }
      else
      {
        emailList = (IList<string>) communicatorHelper.GetProjectCollectionAdministratorIdentities(collectionContext).Distinct<Microsoft.VisualStudio.Services.Identity.Identity>().Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => this.GetEmailAddressOfIdentity(collectionContext, x))).ToList<string>();
        collectionContext.TraceAlways(5109200, TraceLevel.Info, "Commerce", "PurchaseNotificationCommunicator", string.Format("SubscriptionId {0} GalleryId {1} EmailAddressCount {2} in case of unlinked accounts", (object) azureSubscriptionId, (object) galleryId, (object) (emailList != null ? emailList.Count : 0)));
      }
      emailList.Add(this.GetEmailAddressOfIdentity(collectionContext, identity));
      this.SendEmailToAddresses(collectionContext, formatter, azureSubscriptionId, galleryId, emailList);
    }

    protected override PurchaseEmailBaseFormatter GetFormatter(
      IVssRequestContext collectionContext,
      string offerMeterName)
    {
      if (this.subscriptionId.GetValueOrDefault() != Guid.Empty)
      {
        this.FormatterType = typeof (PurchaseRequestEmailFormatterForLinkedAccounts);
        return (PurchaseEmailBaseFormatter) new PurchaseRequestEmailFormatterForLinkedAccounts(this.purchaseRequest);
      }
      this.FormatterType = typeof (PurchaseRequestEmailFormatterForUnlinkedAccounts);
      return (PurchaseEmailBaseFormatter) new PurchaseRequestEmailFormatterForUnlinkedAccounts(this.purchaseRequest);
    }

    protected override void SendEmailToAddresses(
      IVssRequestContext collectionContext,
      PurchaseEmailBaseFormatter formatter,
      Guid azureSubscriptionId,
      string galleryId,
      IList<string> emailList)
    {
      ICommerceEmailHandler extension = collectionContext.GetExtension<ICommerceEmailHandler>();
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      if (emailList.IsNullOrEmpty<string>())
      {
        collectionContext.Trace(5108450, TraceLevel.Info, "Commerce", "PurchaseNotificationCommunicator", "Email list is null or empty");
      }
      else
      {
        collectionContext.Trace(5109198, TraceLevel.Info, "Commerce", "PurchaseNotificationCommunicator", string.Format("Sending Emails for CollectionId {0} SubscriptionId {1} GalleryId {2} EmailListCount {3}", (object) collectionContext.ServiceHost.InstanceId, (object) azureSubscriptionId, (object) galleryId, (object) emailList.Count));
        extension.SendEmail(requestContext, (INotificationEmailData) formatter, emailList);
        collectionContext.Trace(5109197, TraceLevel.Info, "Commerce", "PurchaseNotificationCommunicator", string.Format("Sent Emails for CollectionId {0} SubscriptionId {1} GalleryId {2} EmailListCount {3}", (object) collectionContext.ServiceHost.InstanceId, (object) azureSubscriptionId, (object) galleryId, (object) emailList.Count));
      }
    }

    public Type FormatterType { get; private set; }
  }
}
