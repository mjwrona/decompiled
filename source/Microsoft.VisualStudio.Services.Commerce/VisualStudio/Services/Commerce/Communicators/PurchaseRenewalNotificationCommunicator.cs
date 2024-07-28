// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Communicators.PurchaseRenewalNotificationCommunicator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common.Models;
using Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseRenewalNotificationFormatter;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Commerce.Communicators
{
  public class PurchaseRenewalNotificationCommunicator
  {
    private const string Area = "Commerce";
    private const string Layer = "PurchaseRenewalNotificationCommunicator";

    public void SendPurchaseRenewalNotificationEmail(
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
      List<string> toAddresses)
    {
      collectionContext.CheckHostedDeployment();
      try
      {
        PurchaseEmailBaseFormatter matchingFormatter = this.GetMatchingFormatter(collectionContext, offerMeterName);
        if (matchingFormatter != null)
        {
          matchingFormatter.SetEmailBody(renewalGroup);
          matchingFormatter.SetEmailSubject();
          collectionContext.TraceConditionally(5107432, TraceLevel.Info, "Commerce", nameof (PurchaseRenewalNotificationCommunicator), (Func<string>) (() => string.Format("OfferMeterName: {0} SubscriptionId: {1} RenewalGroup: {2} Quantity: {3} OfferCode: {4} RenewalDate: {5} TenantId: {6} ObjectId: {7}", (object) offerMeterName, (object) azureSubscriptionId, (object) renewalGroup, (object) quantity, (object) offerCode, (object) renewalDate, (object) tenantId, (object) objectId)));
          matchingFormatter.SetEmailAttributes(collectionContext, offerMeterName, galleryId, azureSubscriptionId, renewalGroup, quantity, offerCode, renewalDate, tenantId, objectId);
          this.SendEmailNotification(collectionContext, matchingFormatter, toAddresses);
        }
        else
          collectionContext.Trace(5107430, TraceLevel.Info, "Commerce", nameof (PurchaseRenewalNotificationCommunicator), "SendSubscriptionRenewalNotificationEmail: Couldn't find matching email formatter for offerMeterName: " + offerMeterName);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5107431, "Commerce", nameof (PurchaseRenewalNotificationCommunicator), ex);
      }
    }

    private PurchaseEmailBaseFormatter GetMatchingFormatter(
      IVssRequestContext collectionContext,
      string offerMeterName)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      return BundlePurchaseRenewalEmailFormatter.IsOfferMeterApplicable((OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName)) ? (PurchaseEmailBaseFormatter) new BundlePurchaseRenewalEmailFormatter() : (PurchaseEmailBaseFormatter) null;
    }

    private void SendEmailNotification(
      IVssRequestContext collectionContext,
      PurchaseEmailBaseFormatter formatter,
      List<string> toAddressList)
    {
      ICommerceEmailHandler extension = collectionContext.GetExtension<ICommerceEmailHandler>();
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      foreach (string toAddress in toAddressList)
      {
        extension.SendEmail(requestContext, (INotificationEmailData) formatter, new MailAddress(toAddress));
        collectionContext.TraceConditionally(5107433, TraceLevel.Info, "Commerce", nameof (PurchaseRenewalNotificationCommunicator), (Func<string>) (() => "Email sent"));
      }
    }
  }
}
