// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.AdminPermissionService.TeamFoundationProjectAdminPermissionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.AdminPermissionService
{
  public class TeamFoundationProjectAdminPermissionService : 
    ITeamFoundationProjectAdminPermissionService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void CheckAdminPermissions(IVssRequestContext requestContext, Guid projectId)
    {
      if (!this.HasAdminPermissions(requestContext, projectId))
        throw new WorkItemTrackingUnauthorizedOperationException();
    }

    public bool HasAdminPermissions(IVssRequestContext requestContext, Guid projectId)
    {
      string token = "$" + "/" + projectId.ToString("D");
      return this.GetSecurityNamespace(requestContext).HasPermission(requestContext, token, 1, false);
    }

    private IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WitProvisionSecurity.NamespaceId);
  }
}
