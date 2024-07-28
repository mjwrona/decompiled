// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.TrialExpiringNotification
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  public class TrialExpiringNotification : TrialNotification
  {
    private const string Layer = "TrialExpiringNotification";
    private const string BuyTrialExpiringUrlPath = "items?itemName={0}&workflowId=marketplace&accountId={1}&wt.mc_id=TrialEndingEmail&install=true";

    public void SendTrialNotificationEmail(
      IVssRequestContext collectionContext,
      string offerMeterName,
      int includedQuantity)
    {
      if (offerMeterName == null)
      {
        collectionContext.Trace(5109164, TraceLevel.Error, "Commerce", nameof (TrialExpiringNotification), "No notification is published since offerMeterName is null");
      }
      else
      {
        OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
        string extensionLink;
        string buyLink;
        this.PopulateEmailLinks(collectionContext, offerMeter, out extensionLink, out buyLink, "items?itemName={0}&workflowId=marketplace&accountId={1}&wt.mc_id=TrialEndingEmail&install=true");
        string vsoAccountLink;
        string manageExtensionUsersLink;
        this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter, out vsoAccountLink, out manageExtensionUsersLink);
        ExtensionTrialEvent extensionTrialEvent = new ExtensionTrialEvent();
        extensionTrialEvent.NotificationType = this.GetNotificationType(offerMeter?.GalleryId, "trial-expiring");
        extensionTrialEvent.ExtensionName = offerMeterName;
        extensionTrialEvent.AccountName = CollectionHelper.GetCollectionName(collectionContext);
        extensionTrialEvent.IncludedQuantity = includedQuantity.ToString();
        extensionTrialEvent.ExtensionLink = extensionLink;
        extensionTrialEvent.BuyLink = buyLink;
        extensionTrialEvent.AccountLink = vsoAccountLink;
        extensionTrialEvent.ManageExtensionUsersLink = manageExtensionUsersLink;
        ExtensionTrialEvent emailEvent = extensionTrialEvent;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity administratorIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new CommunicatorHelper().GetProjectCollectionAdministratorIdentities(collectionContext))
          this.PublishNotificationEvent(collectionContext, administratorIdentity, (object) emailEvent);
      }
    }
  }
}
