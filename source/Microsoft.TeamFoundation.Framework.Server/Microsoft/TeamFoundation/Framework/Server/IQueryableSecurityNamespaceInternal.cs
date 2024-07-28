// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IQueryableSecurityNamespaceInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IQueryableSecurityNamespaceInternal : IQueryableSecurityNamespace
  {
    void CheckRequestContext(IVssRequestContext requestContext);

    void QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider = -1);

    bool PrincipalHasPermission(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    bool? GetPrincipalPermissionState(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    bool PrincipalHasPermissionForAllChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators);

    bool PrincipalHasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators);
  }
}
