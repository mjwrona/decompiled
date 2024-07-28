// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSecurityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class GraphSecurityHelper
  {
    private const string Area = "Graph";
    private const string Layer = "GraphSecurityHelper";

    public static void CheckPermissionToReadIdentity(
      IVssRequestContext requestContext,
      int permission)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.SkipFrameworkIdentityReadPermissionCheck") || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") || requestContext.IsSystemContext)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GraphSecurityConstants.NamespaceId);
      if (securityNamespace == null)
      {
        requestContext.Trace(10002008, TraceLevel.Error, "Graph", nameof (GraphSecurityHelper), "Skipping identity read permission check for system context as security namespace is null");
      }
      else
      {
        permission |= 1;
        securityNamespace.CheckPermission(requestContext, GraphSecurityConstants.RefsToken, permission);
      }
    }

    public static bool HasPermissionToReadIdentity(
      IVssRequestContext requestContext,
      int permission)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.SkipFrameworkIdentityReadPermissionCheck") || !requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") || requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GraphSecurityConstants.NamespaceId);
      if (securityNamespace == null)
      {
        requestContext.Trace(10002008, TraceLevel.Error, "Graph", nameof (GraphSecurityHelper), "Skipping identity read permission check for system context as security namespace is null");
        return true;
      }
      permission |= 1;
      return securityNamespace.HasPermission(requestContext, GraphSecurityConstants.RefsToken, permission);
    }
  }
}
