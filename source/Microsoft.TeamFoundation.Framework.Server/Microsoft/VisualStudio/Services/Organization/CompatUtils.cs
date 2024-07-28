// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CompatUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class CompatUtils
  {
    private static readonly HashSet<string> s_wellKnownAccountPropertiesToPrefetch = new HashSet<string>()
    {
      "SystemProperty.TrialStartDate",
      "SystemProperty.TrialEndDate",
      "SystemProperty.TimeZone",
      "SystemProperty.CustomerStage",
      "SystemProperty.AllowStakeholdersToUseBuildAndRelease"
    };
    private static readonly HashSet<string> s_wellKnownPolicyPropertiesToPrefetch = new HashSet<string>()
    {
      "Policy.AllowAnonymousAccess",
      "Policy.AllowOrgAccess",
      "Policy.DisallowAadGuestUserAccess",
      "Policy.DisallowBasicAuthentication",
      "Policy.DisallowOAuthAuthentication",
      "Policy.DisallowSecureShell",
      "Policy.EnforceAadAuthorization",
      "Policy.IsInternal",
      "Policy.AuthenticationCredentialValidFrom",
      "Policy.EnforceAADConditionalAccess",
      "Policy.AllowGitHubInvitationsAccessToken",
      "Policy.AllowRequestAccessToken",
      OrganizationPolicyService.GetEnforceKey("Policy.AllowAnonymousAccess"),
      OrganizationPolicyService.GetEnforceKey("Policy.AllowOrgAccess"),
      OrganizationPolicyService.GetEnforceKey("Policy.DisallowAadGuestUserAccess"),
      OrganizationPolicyService.GetEnforceKey("Policy.DisallowBasicAuthentication"),
      OrganizationPolicyService.GetEnforceKey("Policy.DisallowOAuthAuthentication"),
      OrganizationPolicyService.GetEnforceKey("Policy.DisallowSecureShell"),
      OrganizationPolicyService.GetEnforceKey("Policy.EnforceAadAuthorization"),
      OrganizationPolicyService.GetEnforceKey("Policy.IsInternal"),
      OrganizationPolicyService.GetEnforceKey("Policy.AuthenticationCredentialValidFrom"),
      OrganizationPolicyService.GetEnforceKey("Policy.EnforceAADConditionalAccess")
    };

    internal static HashSet<string> EnsureWellKnownCollectionPropertiesExists(
      this IEnumerable<string> propertyNames)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) CompatUtils.s_wellKnownAccountPropertiesToPrefetch);
      stringSet.UnionWith((IEnumerable<string>) CompatUtils.s_wellKnownPolicyPropertiesToPrefetch);
      if (!propertyNames.IsNullOrEmpty<string>())
        stringSet.UnionWith(propertyNames);
      return stringSet;
    }

    internal static HashSet<string> EnsureWellKnownOrganizationPropertiesExists(
      this IEnumerable<string> propertyNames)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) CompatUtils.s_wellKnownPolicyPropertiesToPrefetch);
      if (!propertyNames.IsNullOrEmpty<string>())
        stringSet.UnionWith(propertyNames);
      return stringSet;
    }
  }
}
