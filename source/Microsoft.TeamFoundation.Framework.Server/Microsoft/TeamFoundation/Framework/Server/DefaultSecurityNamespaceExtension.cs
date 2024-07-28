// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DefaultSecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Security;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DefaultSecurityNamespaceExtension : ISecurityNamespaceExtension
  {
    public static readonly DefaultSecurityNamespaceExtension Instance = new DefaultSecurityNamespaceExtension();

    public virtual bool AlwaysAllowAdministrators => true;

    public virtual bool HasWritePermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool recurse)
    {
      bool flag = securityNamespace.HasPermission(requestContext, token, securityNamespace.Description.WritePermission, this.AlwaysAllowAdministrators);
      if (flag & recurse)
        flag = securityNamespace.HasPermissionForAllChildren(requestContext, token, securityNamespace.Description.WritePermission, alwaysAllowAdministrators: this.AlwaysAllowAdministrators);
      return flag;
    }

    public virtual bool HasWritePermissionExpect(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool recurse,
      bool expectedResult,
      out EvaluationPrincipal failingPrincipal)
    {
      bool flag = securityNamespace.HasPermissionExpect(requestContext, token, securityNamespace.Description.WritePermission, expectedResult, true, out failingPrincipal, this.AlwaysAllowAdministrators);
      if (flag & recurse)
        flag = securityNamespace.HasPermissionForAllChildren(requestContext, token, securityNamespace.Description.WritePermission, alwaysAllowAdministrators: this.AlwaysAllowAdministrators);
      return flag;
    }

    public virtual bool HasReadPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token)
    {
      return securityNamespace.HasPermission(requestContext, token, securityNamespace.Description.ReadPermission);
    }

    public virtual bool HasReadPermissionExpect(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool expectedResult,
      out EvaluationPrincipal failingPrincipal)
    {
      return securityNamespace.HasPermissionExpect(requestContext, token, securityNamespace.Description.ReadPermission, expectedResult, true, out failingPrincipal);
    }

    public virtual bool HasPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      int effectiveAllows,
      int effectiveDenys,
      bool preliminaryDecision)
    {
      return preliminaryDecision;
    }

    public virtual bool HasPermissionForAllChildren(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal evalutionPrincipal,
      string token,
      int requestedPermissions,
      bool preliminaryDecision)
    {
      return preliminaryDecision;
    }

    public virtual bool HasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal evalutionPrincipal,
      string token,
      int requestedPermissions,
      bool preliminaryDecision)
    {
      return preliminaryDecision;
    }

    public virtual string HandleIncomingToken(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken)
    {
      return securityToken;
    }

    public virtual string HandleOutgoingToken(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken)
    {
      return securityToken;
    }

    public virtual void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string token,
      int requestedPermissions)
    {
      throw new AccessCheckException(identity.Descriptor, identity.DisplayName, token, requestedPermissions, securityNamespace.Description.NamespaceId, TFCommonResources.AccessCheckExceptionTokenFormat((object) identity.Id.ToString(), (object) token, (object) string.Join(", ", securityNamespace.Description.GetLocalizedActions(requestedPermissions))));
    }

    public virtual IEnumerable<QueriedAccessControlList> QueryPermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse,
      IEnumerable<QueriedAccessControlList> preliminaryACLs)
    {
      return preliminaryACLs;
    }

    public virtual string GetLocalizedActionDisplayName(
      IVssSecurityNamespace securityNamespace,
      int bit)
    {
      return (string) null;
    }

    public virtual int QueryEffectivePermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      EvaluationPrincipal principal,
      int preliminaryEffectivePermissions)
    {
      return preliminaryEffectivePermissions;
    }
  }
}
