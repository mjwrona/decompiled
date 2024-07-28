// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.TrialNotification
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  public abstract class TrialNotification : CommerceNotification
  {
    protected const string TrialStartedType = "trial-started";
    protected const string TrialExpiringType = "trial-expiring";
    protected const string TrialExpiredType = "trial-expired";
    private const string TestPlansTrialStartedType = "testplans-trial-started";
    private const string TestPlansTrialExpiringType = "testplans-trial-expiring";
    private const string TestPlansTrialExpiredType = "testplans-trial-expired";

    protected void PopulateEmailLinks(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter,
      out string extensionLink,
      out string buyLink,
      string buyExtensionPath = null)
    {
      extensionLink = string.Format("https://marketplace.visualstudio.com/items?itemName={0}", (object) offerMeter.GalleryId);
      buyLink = "";
      if (buyExtensionPath == null)
        return;
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      string str = string.Format(buyExtensionPath, (object) offerMeter.GalleryId, (object) collectionContext.ServiceHost.InstanceId.ToString());
      buyLink = this.GetLocationServiceUrl(requestContext, CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + str;
    }

    protected string GetNotificationType(string galleryId, string trialType)
    {
      switch (trialType)
      {
        case "trial-expired":
          return this.GetTrialExpiredNotificationTypeForMeter(galleryId);
        case "trial-expiring":
          return this.GetTrialExpiringNotificationTypeForMeter(galleryId);
        case "trial-started":
          return this.GetTrialStartedNotificationTypeForMeter(galleryId);
        default:
          return string.Empty;
      }
    }

    private string GetTrialExpiredNotificationTypeForMeter(string galleryId) => string.Equals(galleryId, "ms.vss-testmanager-web", StringComparison.OrdinalIgnoreCase) ? "testplans-trial-expired" : "trial-expired";

    private string GetTrialExpiringNotificationTypeForMeter(string galleryId) => string.Equals(galleryId, "ms.vss-testmanager-web", StringComparison.OrdinalIgnoreCase) ? "testplans-trial-expiring" : "trial-expiring";

    private string GetTrialStartedNotificationTypeForMeter(string galleryId) => string.Equals(galleryId, "ms.vss-testmanager-web", StringComparison.OrdinalIgnoreCase) ? "testplans-trial-started" : "trial-started";
  }
}
