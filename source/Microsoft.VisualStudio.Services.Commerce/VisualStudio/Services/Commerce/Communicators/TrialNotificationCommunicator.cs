// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Communicators.TrialNotificationCommunicator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Communicators
{
  public abstract class TrialNotificationCommunicator
  {
    private const string Area = "Commerce";
    private const string Layer = "TrialNotificationCommunicator";

    protected abstract TrialEmailBaseFormatter GetFormatter(
      OfferMeter offerMeter,
      string offerMeterName,
      int includedQuantity);

    public void SendTrialNotificationEmail(
      IVssRequestContext collectionContext,
      string offerMeterName,
      int includedQuantity)
    {
      collectionContext.CheckHostedDeployment();
      try
      {
        TrialEmailBaseFormatter matchingFormatter = this.GetMatchingFormatter(collectionContext, offerMeterName, includedQuantity);
        if (matchingFormatter != null)
        {
          this.SetEmailContent(collectionContext, matchingFormatter);
          this.SendEmailNotification(collectionContext, matchingFormatter, offerMeterName);
        }
        else
          collectionContext.Trace(5107380, TraceLevel.Info, "Commerce", nameof (TrialNotificationCommunicator), "SendTrialNotificationEmail: Couldn't find matching email formatter for offerMeterName: " + offerMeterName);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5107420, "Commerce", nameof (TrialNotificationCommunicator), ex);
      }
    }

    protected TrialEmailBaseFormatter GetMatchingFormatter(
      IVssRequestContext collectionContext,
      string offerMeterName,
      int includedQuantity = -1)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
      collectionContext.Trace(5107422, TraceLevel.Info, "Commerce", nameof (TrialNotificationCommunicator), "OfferMeterName: " + offerMeterName);
      if (!TrialEmailBaseFormatter.IsOfferMeterCategoryExtension(offerMeter))
        return (TrialEmailBaseFormatter) null;
      TrialEmailBaseFormatter formatter = this.GetFormatter(offerMeter, offerMeterName, includedQuantity);
      collectionContext.Trace(5107423, TraceLevel.Info, "Commerce", nameof (TrialNotificationCommunicator), string.Format("Input Params - OfferMeterName: {0} IncludedQuantity: {1} Output Params - Formatter: {2}", (object) offerMeterName, (object) includedQuantity, (object) formatter.GetType()));
      return formatter;
    }

    private void SetEmailContent(
      IVssRequestContext collectionContext,
      TrialEmailBaseFormatter formatter)
    {
      formatter.SetEmailBody(collectionContext);
      formatter.SetEmailSubject(collectionContext);
      formatter.SetEmailAttributes(collectionContext);
    }

    private void SendEmailNotification(
      IVssRequestContext collectionContext,
      TrialEmailBaseFormatter formatter,
      string offerMeterName)
    {
      ICommerceEmailHandler extension = collectionContext.GetExtension<ICommerceEmailHandler>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administratorIdentities = new CommunicatorHelper().GetProjectCollectionAdministratorIdentities(collectionContext);
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) administratorIdentities)
      {
        Microsoft.VisualStudio.Services.Identity.Identity id = identity;
        extension.SendEmail(requestContext, (INotificationEmailData) formatter, id);
        collectionContext.TraceConditionally(5107421, TraceLevel.Info, "Commerce", nameof (TrialNotificationCommunicator), (Func<string>) (() => string.Format("CollectionId: {0} OfferMeterName: {1} Cuid: {2}", (object) collectionContext.ServiceHost.InstanceId, (object) offerMeterName, (object) id.GetProperty<string>("CUID", string.Empty))));
      }
    }
  }
}
