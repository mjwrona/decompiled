// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.LicensingUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Licensing;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing;

namespace Microsoft.Azure.DevOps.Licensing
{
  public static class LicensingUtil
  {
    public static string GetLicenseDisplayName(
      LicensingSource source,
      AccountLicenseType accountLicense,
      MsdnLicenseType msdnLicense)
    {
      string licenseDisplayName = (string) null;
      switch (source)
      {
        case LicensingSource.Account:
          switch (accountLicense)
          {
            case AccountLicenseType.EarlyAdopter:
              licenseDisplayName = LicensingResources.LicenseNameAccountEarlyAdopter;
              break;
            case AccountLicenseType.Express:
              licenseDisplayName = LicensingResources.LicenseNameAccountExpress;
              break;
            case AccountLicenseType.Professional:
              licenseDisplayName = LicensingResources.LicenseNameAccountProfessional;
              break;
            case AccountLicenseType.Advanced:
              licenseDisplayName = LicensingResources.LicenseNameAccountBasicPlusTestPlans;
              break;
            case AccountLicenseType.Stakeholder:
              licenseDisplayName = LicensingResources.LicenseNameAccountStakeholder;
              break;
          }
          break;
        case LicensingSource.Msdn:
          switch (msdnLicense)
          {
            case MsdnLicenseType.Eligible:
              licenseDisplayName = LicensingResources.LicenseNameMsdnEligible;
              break;
            case MsdnLicenseType.Professional:
              licenseDisplayName = LicensingResources.LicenseNameMsdnProfessional;
              break;
            case MsdnLicenseType.Platforms:
              licenseDisplayName = LicensingResources.LicenseNameMsdnPlatforms;
              break;
            case MsdnLicenseType.TestProfessional:
              licenseDisplayName = LicensingResources.LicenseNameMsdnTestProfessional;
              break;
            case MsdnLicenseType.Premium:
              licenseDisplayName = LicensingResources.LicenseNameMsdnPremium;
              break;
            case MsdnLicenseType.Ultimate:
              licenseDisplayName = LicensingResources.LicenseNameMsdnUltimate;
              break;
            case MsdnLicenseType.Enterprise:
              licenseDisplayName = LicensingResources.LicenseNameMsdnEnterprise;
              break;
          }
          break;
      }
      return licenseDisplayName;
    }

    public static string GetStatusMessage(License license, AccountUserStatus status)
    {
      string statusMessage = string.Empty;
      switch (status)
      {
        case AccountUserStatus.None:
          statusMessage = string.Empty;
          break;
        case AccountUserStatus.Active:
          statusMessage = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : LicensingResources.UserStatusMsdnNotValidated;
          break;
        case AccountUserStatus.Disabled:
          statusMessage = !license.Source.Equals((object) LicensingSource.Msdn) ? LicensingResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? LicensingResources.UserStatusMsdnExpired : LicensingResources.UserStatusMsdnDisabledEligible);
          break;
        case AccountUserStatus.Deleted:
          statusMessage = string.Empty;
          break;
        case AccountUserStatus.Pending:
          statusMessage = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : LicensingResources.UserStatusMsdnPending;
          break;
        case AccountUserStatus.Expired:
          statusMessage = !license.Source.Equals((object) LicensingSource.Msdn) ? LicensingResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? LicensingResources.UserStatusMsdnExpired : LicensingResources.UserStatusMsdnDisabledEligible);
          break;
        case AccountUserStatus.PendingDisabled:
          if (license.Source.Equals((object) LicensingSource.Msdn))
          {
            if (((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible))
            {
              statusMessage = LicensingResources.UserStatusMsdnDisabledEligible;
              break;
            }
          }
          else if (License.None.Equals(license))
          {
            statusMessage = string.Empty;
            break;
          }
          statusMessage = LicensingResources.UserStatusExpired;
          break;
      }
      return statusMessage;
    }
  }
}
