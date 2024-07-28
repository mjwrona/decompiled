// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities.LicensingExtensionEntitlementHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities
{
  internal class LicensingExtensionEntitlementHelper
  {
    private static Dictionary<License, VisualStudioOnlineServiceLevel> LicenseToVsoServiceLevelMap = new Dictionary<License, VisualStudioOnlineServiceLevel>()
    {
      {
        (License) AccountLicense.EarlyAdopter,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      },
      {
        (License) AccountLicense.Stakeholder,
        VisualStudioOnlineServiceLevel.Stakeholder
      },
      {
        (License) AccountLicense.Express,
        VisualStudioOnlineServiceLevel.Express
      },
      {
        (License) AccountLicense.Professional,
        VisualStudioOnlineServiceLevel.Express
      },
      {
        (License) AccountLicense.Advanced,
        VisualStudioOnlineServiceLevel.Advanced
      },
      {
        (License) MsdnLicense.Eligible,
        VisualStudioOnlineServiceLevel.Stakeholder
      },
      {
        (License) MsdnLicense.Professional,
        VisualStudioOnlineServiceLevel.Express
      },
      {
        (License) MsdnLicense.Platforms,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      },
      {
        (License) MsdnLicense.Premium,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      },
      {
        (License) MsdnLicense.Ultimate,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      },
      {
        (License) MsdnLicense.TestProfessional,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      },
      {
        (License) MsdnLicense.Enterprise,
        VisualStudioOnlineServiceLevel.AdvancedPlus
      }
    };
    private const VisualStudioOnlineServiceLevel DefaultServiceLevel = VisualStudioOnlineServiceLevel.Stakeholder;
    private const string s_area = "LicensingService";
    private const string s_layer = "FrameworkLicensingService";

    public static List<string> GetExtensionAssignmentBasedOnUserAccessLevel(
      IVssRequestContext requestContext,
      Guid userId)
    {
      try
      {
        AccountEntitlement accountEntitlement = requestContext.GetAccountEntitlement(userId);
        if (accountEntitlement == (AccountEntitlement) null)
          requestContext.Trace(1034277, TraceLevel.Error, "LicensingService", "FrameworkLicensingService", string.Format("Account Entitlement is null for user {0}", (object) userId));
        List<string> onUserAccessLevel = new List<string>();
        IEnumerable<IOfferMeter> offerMeters = requestContext.GetService<IOfferMeterService>().GetOfferMeters(requestContext);
        if (offerMeters == null)
          requestContext.Trace(1034278, TraceLevel.Error, "LicensingService", "FrameworkLicensingService", "GetOfferMeters returned null");
        if (offerMeters != null && accountEntitlement != (AccountEntitlement) null)
        {
          foreach (IOfferMeter offerMeter in offerMeters)
          {
            if (offerMeter != null && offerMeter.GalleryId != null && offerMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.None && LicensingExtensionEntitlementHelper.UsageRightsMatch(LicensingExtensionEntitlementHelper.MapToVisualStudioOnlineServiceLevel(offerMeter.IncludedInLicenseLevel), accountEntitlement.License))
            {
              requestContext.Trace(1034274, TraceLevel.Info, "LicensingService", "FrameworkLicensingService", string.Format("User {0} was assigned {1} due to license {2}", (object) userId, (object) offerMeter.GalleryId, (object) accountEntitlement.License));
              onUserAccessLevel.Add(offerMeter.GalleryId);
            }
          }
        }
        return onUserAccessLevel;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034276, "LicensingService", "FrameworkLicensingService", ex);
        return new List<string>();
      }
    }

    public static VisualStudioOnlineServiceLevel MapToVisualStudioOnlineServiceLevel(
      MinimumRequiredServiceLevel accessLevel)
    {
      return accessLevel >= MinimumRequiredServiceLevel.None && accessLevel <= MinimumRequiredServiceLevel.Stakeholder ? (VisualStudioOnlineServiceLevel) accessLevel : VisualStudioOnlineServiceLevel.None;
    }

    public static bool UsageRightsMatch(
      VisualStudioOnlineServiceLevel minimumRequiredAccessLevel,
      License license)
    {
      if (license == (License) null)
        return false;
      VisualStudioOnlineServiceLevel accessLevel;
      LicensingExtensionEntitlementHelper.LicenseToVsoServiceLevelMap.TryGetValue(license, out accessLevel);
      return LicensingExtensionEntitlementHelper.GetAccessLevelOrder(accessLevel) >= LicensingExtensionEntitlementHelper.GetAccessLevelOrder(minimumRequiredAccessLevel);
    }

    public static int GetAccessLevelOrder(VisualStudioOnlineServiceLevel accessLevel)
    {
      int accessLevelOrder;
      switch (accessLevel)
      {
        case VisualStudioOnlineServiceLevel.None:
          accessLevelOrder = 0;
          break;
        case VisualStudioOnlineServiceLevel.Express:
          accessLevelOrder = 2;
          break;
        case VisualStudioOnlineServiceLevel.Advanced:
          accessLevelOrder = 3;
          break;
        case VisualStudioOnlineServiceLevel.AdvancedPlus:
          accessLevelOrder = 4;
          break;
        case VisualStudioOnlineServiceLevel.Stakeholder:
          accessLevelOrder = 1;
          break;
        default:
          accessLevelOrder = 0;
          break;
      }
      return accessLevelOrder;
    }
  }
}
