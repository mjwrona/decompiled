// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.LicensingExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Azure.DevOps.Licensing.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing;
using System;

namespace Microsoft.Azure.DevOps.Licensing
{
  public static class LicensingExtensions
  {
    public static bool Contains(
      this LicensingSettingsSelectProperty name,
      LicensingSettingsSelectProperty other)
    {
      return (name & other) != 0;
    }

    public static AccessLevel ToAccessLevel(
      this License license,
      AccountUserStatus status = AccountUserStatus.None,
      AssignmentSource assignmentSource = AssignmentSource.None)
    {
      AccountLicenseType accountLicense = AccountLicenseType.None;
      MsdnLicenseType msdnLicense = MsdnLicenseType.None;
      if (license.Source == LicensingSource.Account)
        accountLicense = (license as AccountLicense).License;
      else if (license.Source == LicensingSource.Msdn)
        msdnLicense = (license as MsdnLicense).License;
      string licenseDisplayName = LicensingUtil.GetLicenseDisplayName(license.Source, accountLicense, msdnLicense);
      return new AccessLevel()
      {
        LicensingSource = license.Source,
        LicenseDisplayName = string.IsNullOrEmpty(licenseDisplayName) ? license.ToString() : licenseDisplayName,
        AccountLicenseType = accountLicense,
        MsdnLicenseType = msdnLicense,
        AssignmentSource = assignmentSource,
        Status = status,
        StatusMessage = LicensingUtil.GetStatusMessage(license, status)
      };
    }

    public static License ToLicense(this AccessLevel accessLevel)
    {
      switch (accessLevel.LicensingSource)
      {
        case LicensingSource.Account:
          return AccountLicense.GetLicense(accessLevel.AccountLicenseType);
        case LicensingSource.Msdn:
          return MsdnLicense.GetLicense(accessLevel.MsdnLicenseType);
        default:
          return License.None;
      }
    }

    public static AccessLevel ToAccessLevel(
      this AccountEntitlement accountEntitlement,
      IVssRequestContext requestContext)
    {
      return accountEntitlement.License.ToAccessLevel(accountEntitlement.UserStatus, accountEntitlement.AssignmentSource);
    }

    public static AccountEntitlement ToAccountEntitlement(
      this AccessLevel accessLevel,
      DateTime lastAccessDate,
      DateTime dateCreated)
    {
      License license;
      switch (accessLevel.LicensingSource)
      {
        case LicensingSource.Account:
          license = AccountLicense.GetLicense(accessLevel.AccountLicenseType);
          break;
        case LicensingSource.Msdn:
          license = MsdnLicense.GetLicense(accessLevel.MsdnLicenseType);
          break;
        default:
          throw new ArgumentException(string.Format("Cannot determine license for AccessLevel {0}", (object) accessLevel));
      }
      return new AccountEntitlement()
      {
        License = license,
        UserStatus = accessLevel.Status,
        AssignmentSource = accessLevel.AssignmentSource,
        LastAccessedDate = (DateTimeOffset) lastAccessDate,
        DateCreated = (DateTimeOffset) dateCreated,
        Origin = LicensingOrigin.None
      };
    }
  }
}
