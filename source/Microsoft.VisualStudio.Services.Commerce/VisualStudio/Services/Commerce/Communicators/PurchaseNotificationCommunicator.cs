// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Communicators.PurchaseNotificationCommunicator
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
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Commerce.Communicators
{
  internal abstract class PurchaseNotificationCommunicator
  {
    protected const string Area = "Commerce";
    protected const string Layer = "PurchaseNotificationCommunicator";

    public virtual void SendPurchaseNotificationEmail(
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        PurchaseEmailBaseFormatter formatter = this.GetFormatter(collectionContext, offerMeterName);
        if (formatter == null)
        {
          collectionContext.Trace(5108448, TraceLevel.Error, "Commerce", nameof (PurchaseNotificationCommunicator), "SendPurchaseNotificationEmail: Couldn't find matching email formatter for offerMeterName: " + offerMeterName);
        }
        else
        {
          this.SetEmailContent(formatter, collectionContext, offerMeterName, galleryId, azureSubscriptionId, renewalGroup, quantity, offerCode, renewalDate, tenantId, objectId, identity);
          this.SendEmails(collectionContext, galleryId, azureSubscriptionId, identity, formatter);
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5108446, "Commerce", nameof (PurchaseNotificationCommunicator), ex);
      }
    }

    protected virtual void SendEmails(
      IVssRequestContext collectionContext,
      string galleryId,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      PurchaseEmailBaseFormatter formatter)
    {
      IList<string> adminEmailAddresses = this.GetAdminCoAdminEmailAddresses(collectionContext, formatter, azureSubscriptionId, galleryId);
      string preferredEmailAddress = this.GetUserPreferredEmailAddress(collectionContext, identity);
      adminEmailAddresses.Add(preferredEmailAddress);
      this.SendEmailToAddresses(collectionContext, formatter, azureSubscriptionId, galleryId, adminEmailAddresses);
    }

    protected virtual void SendEmailToIdentities(
      IVssRequestContext collectionContext,
      PurchaseEmailBaseFormatter formatter,
      Guid azureSubscriptionId,
      string galleryId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList)
    {
      ICommerceEmailHandler extension = collectionContext.GetExtension<ICommerceEmailHandler>();
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      if (identityList.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        collectionContext.Trace(5108450, TraceLevel.Info, "Commerce", nameof (PurchaseNotificationCommunicator), "Email list is null or empty");
      }
      else
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        {
          extension.SendEmail(requestContext, (INotificationEmailData) formatter, identity);
          collectionContext.TraceConditionally(5108449, TraceLevel.Info, "Commerce", nameof (PurchaseNotificationCommunicator), (Func<string>) (() => string.Format("CollectionId {0} SubscriptionId {1} GalleryId {2}", (object) collectionContext.ServiceHost.InstanceId, (object) azureSubscriptionId, (object) galleryId)));
        }
      }
    }

    protected virtual void SendEmailToAddresses(
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
        collectionContext.Trace(5108450, TraceLevel.Info, "Commerce", nameof (PurchaseNotificationCommunicator), "Email list is null or empty");
      }
      else
      {
        foreach (string address in emailList.Distinct<string>())
        {
          extension.SendEmail(requestContext, (INotificationEmailData) formatter, new MailAddress(address));
          collectionContext.TraceConditionally(5108449, TraceLevel.Info, "Commerce", nameof (PurchaseNotificationCommunicator), (Func<string>) (() => string.Format("CollectionId {0} SubscriptionId {1} GalleryId {2}", (object) collectionContext.ServiceHost.InstanceId, (object) azureSubscriptionId, (object) galleryId)));
        }
      }
    }

    protected virtual PurchaseEmailBaseFormatter GetFormatter(
      IVssRequestContext collectionContext,
      string offerMeterName)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
      if (HockeyAppPurchaseEmailFormatter.IsOfferMeterApplicable(offerMeter))
        return (PurchaseEmailBaseFormatter) new HockeyAppPurchaseEmailFormatter();
      if (XamarinUniversityPurchaseEmailFormatter.IsOfferMeterApplicable(offerMeter))
        return (PurchaseEmailBaseFormatter) new XamarinUniversityPurchaseEmailFormatter();
      if (BundlePurchaseEmailFormatter.IsOfferMeterApplicable(offerMeter))
        return (PurchaseEmailBaseFormatter) new BundlePurchaseEmailFormatter();
      return ExtensionPurchaseEmailFormatter.IsOfferMeterApplicable(offerMeter) ? (PurchaseEmailBaseFormatter) new ExtensionPurchaseEmailFormatter() : (PurchaseEmailBaseFormatter) null;
    }

    protected string GetUserPreferredEmailAddress(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return IdentityHelper.GetPreferredEmailAddress(requestContext.To(TeamFoundationHostType.Deployment), identity.Id);
    }

    protected string GetEmailAddressOfIdentity(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return collectionContext.GetExtension<ICommerceEmailHandler>().GetIdentityMailAddress(identity);
    }

    private void SetEmailContent(
      PurchaseEmailBaseFormatter formatter,
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      formatter.SetEmailBody(renewalGroup);
      formatter.SetEmailSubject();
      formatter.SetEmailAttributes(collectionContext, offerMeterName, galleryId, azureSubscriptionId, renewalGroup, quantity, offerCode, renewalDate, tenantId, objectId);
    }

    private IList<string> GetAdminCoAdminEmailAddresses(
      IVssRequestContext collectionContext,
      PurchaseEmailBaseFormatter formatter,
      Guid azureSubscriptionId,
      string galleryId)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.PurchaseNotificationCommunicator.EnableAdminCoAdminEmail"))
        return (IList<string>) new List<string>();
      IList<string> adminMailAddresses = new CommunicatorHelper().GetAdminCoAdminMailAddresses(collectionContext, azureSubscriptionId);
      if (adminMailAddresses.IsNullOrEmpty<string>())
        collectionContext.TraceConditionally(5108450, TraceLevel.Info, "Commerce", nameof (PurchaseNotificationCommunicator), (Func<string>) (() => "Email list is null or empty"));
      return adminMailAddresses ?? (IList<string>) new List<string>();
    }
  }
}
