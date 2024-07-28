// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationSecurityService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationSecurityService))]
  public interface ITeamFoundationSecurityService<T> where T : IVssSecurityNamespace
  {
    T CreateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description);

    T UpdateSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description);

    bool DeleteSecurityNamespace(IVssRequestContext requestContext, Guid namespaceId);

    T GetSecurityNamespace(IVssRequestContext requestContext, Guid namespaceId);

    IList<T> GetSecurityNamespaces(IVssRequestContext requestContext);

    void RemoveIdentityACEs(IVssRequestContext requestContext, IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities);
  }
}
