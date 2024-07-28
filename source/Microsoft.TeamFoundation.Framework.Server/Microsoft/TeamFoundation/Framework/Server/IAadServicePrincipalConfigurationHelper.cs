// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IAadServicePrincipalConfigurationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IAadServicePrincipalConfigurationHelper
  {
    bool IsTokenValidationEnabled(IVssRequestContext requestContext);

    bool IsEntitlementsApiEnabled(IVssRequestContext requestContext);

    bool IsAdoGraphApiEnabled(IVssRequestContext requestContext);

    bool IsSearchingForServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsUserHubSupportServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsSyncingForServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsGroupRulesForServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsFixServicePrincipalsWithUnknownMetaTypeEnabled(
      IVssRequestContext requestContext,
      Guid oid);

    bool IsOrgGroupMembershipSupportServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsRestrictionOfCertainApisForServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsRequireAadBackedOrgAttributeEnabled(IVssRequestContext requestContext);

    bool AreSPPermissionsOperationsInUIEnabled(IVssRequestContext requestContext);

    bool IsMergeUserAndSPPermissionsInUIEnabled(IVssRequestContext requestContext);

    bool IsObjectLevelSecurityMembersSupportServicePrincipalsEnabled(
      IVssRequestContext requestContext);

    bool IsBranchPolicyStatusChecksSupportServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsArtifactsPermissionsSupportServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsEmsPermissionsSupportServicePrincipalsEnabled(IVssRequestContext requestContext);

    bool IsAllowedSubjectTypesEnabled(IVssRequestContext requestContext);

    bool IsAadServicePrincipalWhitelistedForCategory(
      IVssRequestContext requestContext,
      string categoryName,
      Guid aadServicePrincipalCuid);
  }
}
