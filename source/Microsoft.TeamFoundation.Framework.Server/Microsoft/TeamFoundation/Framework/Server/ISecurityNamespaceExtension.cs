// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ISecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [InheritedExport]
  public interface ISecurityNamespaceExtension
  {
    bool AlwaysAllowAdministrators { get; }

    bool HasWritePermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool recurse);

    bool HasWritePermissionExpect(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool recurse,
      bool expectedResult,
      out EvaluationPrincipal failingPrincipal);

    bool HasReadPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token);

    bool HasReadPermissionExpect(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      bool expectedResult,
      out EvaluationPrincipal failingPrincipal);

    bool HasPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal evalutionPrincipal,
      string token,
      int requestedPermissions,
      int effectiveAllows,
      int effectiveDenys,
      bool preliminaryDecision);

    bool HasPermissionForAllChildren(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal evalutionPrincipal,
      string token,
      int requestedPermissions,
      bool preliminaryDecision);

    bool HasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal evalutionPrincipal,
      string token,
      int requestedPermissions,
      bool preliminaryDecision);

    string HandleIncomingToken(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken);

    string HandleOutgoingToken(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken);

    void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string token,
      int requestedPermissions);

    IEnumerable<QueriedAccessControlList> QueryPermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse,
      IEnumerable<QueriedAccessControlList> preliminaryAccessControlLists);

    int QueryEffectivePermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      EvaluationPrincipal evalutionPrincipal,
      int preliminaryEffectivePermissions);
  }
}
