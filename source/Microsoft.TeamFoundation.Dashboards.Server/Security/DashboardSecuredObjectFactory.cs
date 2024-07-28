// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Security.DashboardSecuredObjectFactory
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Security
{
  public class DashboardSecuredObjectFactory
  {
    public static ISecuredObject GenerateSecuredObject(
      IVssRequestContext requestContext,
      IDashboardSecurityManager securityManager,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId)
    {
      securityManager.CheckReadPermission(requestContext, projectId, teamId, dashboardId);
      return (ISecuredObject) new DashboardSecuredObjectImplementation(DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId), dashboardId));
    }

    public static bool TryGenerateSecuredObject(
      IVssRequestContext requestContext,
      IDashboardSecurityManager securityManager,
      Guid projectId,
      Guid teamId,
      Guid? dashboardId,
      out ISecuredObject securedDashboardObject)
    {
      if (securityManager.HasReadPermission(requestContext, projectId, teamId, dashboardId))
      {
        string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, projectId, new Guid?(teamId), dashboardId);
        securedDashboardObject = (ISecuredObject) new DashboardSecuredObjectImplementation(securityToken);
        return true;
      }
      securedDashboardObject = (ISecuredObject) null;
      return false;
    }
  }
}
