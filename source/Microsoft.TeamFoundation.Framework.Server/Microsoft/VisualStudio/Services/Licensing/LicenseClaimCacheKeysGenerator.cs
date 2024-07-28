// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseClaimCacheKeysGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicenseClaimCacheKeysGenerator
  {
    private const string s_Area = "Licensing";
    private const string s_Layer = "LicenseClaimCacheKeysGenerator";

    public static string[] Generate(
      IVssRequestContext requestContext,
      EntitlementChangeMessage entitlementChangeMessage)
    {
      string[] strArray = (string[]) null;
      if (entitlementChangeMessage.EntitlementChangeType == EntitlementChangeType.AccountEntitlement && !(entitlementChangeMessage.AccountId == Guid.Empty) && entitlementChangeMessage.UserIds != null && ((IEnumerable<Guid>) entitlementChangeMessage.UserIds).Any<Guid>() && !(((IEnumerable<Guid>) entitlementChangeMessage.UserIds).First<Guid>() == Guid.Empty))
        return LicenseClaimCacheKeysGenerator.Generate(entitlementChangeMessage.AccountId, ((IEnumerable<Guid>) entitlementChangeMessage.UserIds).Single<Guid>());
      requestContext.TraceConditionally(1039112, TraceLevel.Info, "Licensing", nameof (LicenseClaimCacheKeysGenerator), (Func<string>) (() => string.Format("Cannot convert entitlement change message: {0} to license claim cache keys", (object) entitlementChangeMessage)));
      return strArray;
    }

    public static string[] Generate(Guid accountId, Guid userId)
    {
      if (accountId == Guid.Empty || userId == Guid.Empty)
        return (string[]) null;
      return new string[2]
      {
        Constants.LicenseClaimConstants.AccountEntitlementClaimTypePrefix + (object) accountId + (object) Constants.LicenseClaimConstants.KeyPathSeparator + (object) userId,
        Constants.LicenseClaimConstants.AccountRightsClaimTypePrefix + (object) accountId + (object) Constants.LicenseClaimConstants.KeyPathSeparator + (object) userId
      };
    }
  }
}
