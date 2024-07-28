// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebPlatform.Utils.OrganizationTakeoverHelpers
// Assembly: Microsoft.TeamFoundation.WebPlatform.Utils, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BED197B-2BF5-46F4-8D69-B1C197C739E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Utils.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WebPlatform.Utils
{
  public static class OrganizationTakeoverHelpers
  {
    public static readonly string s_TraceArea = nameof (OrganizationTakeoverHelpers);
    public static readonly string s_EnableOrganizationTakeOverFromErrorPageFeatureFlag = "VisualStudio.Services.WebAccess.EnableOrganizationTakeOverFromErrorPage";

    public static bool CanCurrentUserTakeOverOrg(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      bool flag = requestContext.IsFeatureEnabled(OrganizationTakeoverHelpers.s_EnableOrganizationTakeOverFromErrorPageFeatureFlag);
      if (!flag)
        return false;
      bool opsAdministrator = OrganizationTakeoverHelpers.GetIsCurrentUserAzureDevOpsAdministrator(requestContext, userIdentity);
      if (!opsAdministrator)
        return false;
      bool eligibleForTakeOver = OrganizationTakeoverHelpers.GetIsOrganizationEligibleForTakeOver(requestContext);
      return flag & opsAdministrator & eligibleForTakeOver;
    }

    private static bool GetIsOrganizationEligibleForTakeOver(IVssRequestContext requestContext)
    {
      bool eligibleForTakeOver = false;
      try
      {
        eligibleForTakeOver = requestContext.GetService<ICollectionService>().IsEligibleForTakeOver(requestContext.Elevate());
      }
      catch (Exception ex)
      {
        requestContext.Trace(11998, TraceLevel.Error, OrganizationTakeoverHelpers.s_TraceArea, nameof (GetIsOrganizationEligibleForTakeOver), string.Format("Unable find out if collection is eligible for taking over. {0}", (object) ex));
      }
      return eligibleForTakeOver;
    }

    private static bool GetIsCurrentUserAzureDevOpsAdministrator(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity)
    {
      Guid property1 = requestIdentity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
      Guid property2 = requestIdentity.GetProperty<Guid>("Domain", Guid.Empty);
      bool opsAdministrator = false;
      try
      {
        opsAdministrator = AadServiceUtils.IsAzureDevOpsAdministrator(requestContext, property1, property2);
      }
      catch (Exception ex)
      {
        requestContext.Trace(11999, TraceLevel.Error, OrganizationTakeoverHelpers.s_TraceArea, nameof (GetIsCurrentUserAzureDevOpsAdministrator), string.Format("Unable find out if current user is Azure DevOps administrator in AAD. {0}", (object) ex));
      }
      return opsAdministrator;
    }
  }
}
