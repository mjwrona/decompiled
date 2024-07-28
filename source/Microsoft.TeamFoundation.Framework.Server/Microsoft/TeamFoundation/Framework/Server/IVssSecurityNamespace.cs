// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssSecurityNamespace : IMutableSecurityNamespace, IQueryableSecurityNamespace
  {
    SecurityNamespaceDescription Description { get; }

    ISecurityNamespaceExtension NamespaceExtension { get; }

    void OnDataChanged(IVssRequestContext requestContext);

    bool PollForRequestLocalInvalidation(IVssRequestContext requestContext);

    void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      EvaluationPrincipal failingPrincipal = null);

    IQueryableAclStore GetQueryableAclStore(IVssRequestContext requestContext, Guid aclStoreId);
  }
}
