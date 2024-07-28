// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountRightsClaimsContainer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [JsonObject(MemberSerialization.OptOut)]
  internal class AccountRightsClaimsContainer
  {
    internal AccountRightsClaim AccountRightsClaim { get; private set; }

    internal AccountEntitlementClaim AccountEntitlementClaim { get; private set; }

    internal void SetClaim(ILicenseClaim claim)
    {
      switch (claim)
      {
        case AccountRightsClaim _:
          this.AccountRightsClaim = (AccountRightsClaim) claim;
          break;
        case AccountEntitlementClaim _:
          this.AccountEntitlementClaim = (AccountEntitlementClaim) claim;
          break;
        default:
          throw new NotImplementedException();
      }
    }

    internal IEnumerable<ILicenseClaim> GetClaims()
    {
      List<ILicenseClaim> claims = new List<ILicenseClaim>();
      if (this.AccountRightsClaim != null)
        claims.Add((ILicenseClaim) this.AccountRightsClaim);
      if (this.AccountEntitlementClaim != null)
        claims.Add((ILicenseClaim) this.AccountEntitlementClaim);
      return (IEnumerable<ILicenseClaim>) claims;
    }
  }
}
