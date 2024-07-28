// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Services.AgileProjectPermissionService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Agile.Server.Services
{
  public class AgileProjectPermissionService : IAgileProjectPermissionService, IVssFrameworkService
  {
    public bool GetAdvanceBacklogManagementPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      bool throwIfNoPermission)
    {
      return AgileProjectPermissionService.HasPermission(requestContext, projectId, TeamProjectSecurityConstants.AgileToolsBacklogManagement, throwIfNoPermission);
    }

    public bool GetPlansPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      bool throwIfNoPermission)
    {
      return AgileProjectPermissionService.HasPermission(requestContext, projectId, TeamProjectSecurityConstants.AgileToolsPlans, throwIfNoPermission);
    }

    private static bool HasPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int permission,
      bool throwIfNoPermission)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, TeamProjectSecurityConstants.NamespaceId);
      if (securityNamespace == null)
        return false;
      string token = TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(projectId));
      if (!throwIfNoPermission)
        return securityNamespace.HasPermission(requestContext, token, permission);
      securityNamespace.CheckPermission(requestContext, token, permission);
      return true;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
