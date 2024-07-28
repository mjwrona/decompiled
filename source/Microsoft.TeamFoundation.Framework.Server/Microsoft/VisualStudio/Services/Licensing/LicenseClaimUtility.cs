// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseClaimUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class LicenseClaimUtility
  {
    internal static bool AreClaimsEqual(ILicenseClaim lhs, ILicenseClaim rhs)
    {
      if (lhs == rhs)
        return true;
      if (lhs == null || rhs == null)
        return false;
      switch (lhs)
      {
        case AccountRightsClaim _ when rhs is AccountRightsClaim:
          return LicenseClaimUtility.AreAccountRightsClaimsEqual((AccountRightsClaim) lhs, (AccountRightsClaim) rhs);
        case AccountEntitlementClaim _ when rhs is AccountEntitlementClaim:
          return LicenseClaimUtility.AreAccountEntitlementClaimsEqual((AccountEntitlementClaim) lhs, (AccountEntitlementClaim) rhs);
        default:
          throw new NotImplementedException("Comparison for the provided types is not implemented");
      }
    }

    private static bool AreAccountRightsClaimsEqual(AccountRightsClaim lhs, AccountRightsClaim rhs) => object.Equals((object) lhs.AccountRights, (object) rhs.AccountRights) && DateTimeOffset.Equals(lhs.ValidationDate, rhs.ValidationDate);

    private static bool AreAccountEntitlementClaimsEqual(
      AccountEntitlementClaim lhs,
      AccountEntitlementClaim rhs)
    {
      return LicenseClaimUtility.AreEntitlementsEqual(lhs.AccountEntitlement, rhs.AccountEntitlement) && DateTimeOffset.Equals(lhs.ValidationDate, rhs.ValidationDate);
    }

    internal static bool AreEntitlementsEqual(AccountEntitlement lhs, AccountEntitlement rhs)
    {
      if (lhs == rhs)
        return true;
      return !(lhs == (AccountEntitlement) null) && !(rhs == (AccountEntitlement) null) && object.Equals((object) lhs.AccountId, (object) rhs.AccountId) && License.Equals(lhs.License, rhs.License) && object.Equals((object) lhs.UserId, (object) rhs.UserId) && object.Equals((object) lhs.UserStatus, (object) rhs.UserStatus);
    }
  }
}
