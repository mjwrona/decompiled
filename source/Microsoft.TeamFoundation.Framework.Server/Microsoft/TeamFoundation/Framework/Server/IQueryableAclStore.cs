// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IQueryableAclStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IQueryableAclStore : ISecurityAclStore
  {
    void QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider = -1);

    IEnumerable<EnumeratedPermission> QueryChildPermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      int bitsToConsider = -1);

    IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse);

    int QueryAclCount(IVssRequestContext requestContext);
  }
}
