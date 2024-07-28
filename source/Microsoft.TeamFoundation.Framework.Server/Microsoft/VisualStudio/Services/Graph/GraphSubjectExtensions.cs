// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphSubjectExtensions
  {
    public static GraphSubject ToGraphSubjectClient(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (identity == null)
        return (GraphSubject) null;
      if (identity.IsContainer)
        return (GraphSubject) identity.ToGraphGroupClient(context);
      if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(context, identity.Descriptor))
        return (GraphSubject) identity.ToGraphSystemSubjectClient(context);
      return identity.IsAADServicePrincipal ? (GraphSubject) identity.ToGraphServicePrincipalClient(context) : (GraphSubject) identity.ToGraphUserClient(context);
    }

    public static GraphSubject ToGraphSubjectClient(
      this IdentityScope identityScope,
      IVssRequestContext context)
    {
      return identityScope == null ? (GraphSubject) null : (GraphSubject) identityScope.ToGraphScopeClient(context);
    }
  }
}
