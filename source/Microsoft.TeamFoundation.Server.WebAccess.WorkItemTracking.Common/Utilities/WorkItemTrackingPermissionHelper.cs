// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities.WorkItemTrackingPermissionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities
{
  public static class WorkItemTrackingPermissionHelper
  {
    public static WorkItemTrackingPermission QueryWorkItemTrackingPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int requestedPermissions,
      string token,
      bool alwaysAllowAdministrators = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId);
      WorkItemTrackingPermission trackingPermission = new WorkItemTrackingPermission(projectId, requestedPermissions, token);
      trackingPermission.HasPermission = true;
      if (securityNamespace != null)
      {
        string token1 = string.Format("/{0}/{1}/{2}", (object) "WorkItemTracking", (object) projectId, (object) token);
        if (securityNamespace.HasPermission(requestContext, token1, requestedPermissions, alwaysAllowAdministrators))
          trackingPermission.HasPermission = true;
        else
          trackingPermission = (WorkItemTrackingPermission) null;
      }
      return trackingPermission;
    }

    public static ProjectPermission QueryProjectPermission(
      IVssRequestContext requestContext,
      string projectUri,
      int requestedPermission,
      bool alwaysAllowAdministrators = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.ProjectSecurityGuid);
      if (securityNamespace != null)
      {
        string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri);
        if (securityNamespace.HasPermission(requestContext, token, requestedPermission, alwaysAllowAdministrators))
          return new ProjectPermission(requestedPermission, token)
          {
            HasPermission = true
          };
      }
      return (ProjectPermission) null;
    }
  }
}
