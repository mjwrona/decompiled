// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityNamespacePermissionsManagerFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal static class SecurityNamespacePermissionsManagerFactory
  {
    public static SecurityNamespacePermissionsManager CreateManager(
      IVssRequestContext requestContext,
      Guid identifier,
      string token,
      string project,
      Guid? teamId = null)
    {
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.OrganizationLevel))
        return (SecurityNamespacePermissionsManager) new OrganizationPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.CollectionLevel))
        return (SecurityNamespacePermissionsManager) new CollectionPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.ProjectLevel))
        return (SecurityNamespacePermissionsManager) new ProjectPermissionsManager(requestContext, identifier, token, teamId);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.VersionControl))
        return (SecurityNamespacePermissionsManager) new VersionControlPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Build))
        return (SecurityNamespacePermissionsManager) new BuildPermissionsManager(requestContext, identifier, token, project);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.WitQueryFolders))
        return (SecurityNamespacePermissionsManager) new WitQueryFolderPermissionsManager(requestContext, identifier, token, project);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Iteration))
        return (SecurityNamespacePermissionsManager) new IterationPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Area))
        return (SecurityNamespacePermissionsManager) new AreaPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Git))
        return (SecurityNamespacePermissionsManager) new GitPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.ReleaseManagement))
        return (SecurityNamespacePermissionsManager) new ReleaseManagementPermissionsManager(requestContext, identifier, token, project);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Process))
        return (SecurityNamespacePermissionsManager) new ProcessPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.MetaTask))
        return (SecurityNamespacePermissionsManager) new MetaTaskPermissionsManager(requestContext, identifier, token);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Plan))
        return (SecurityNamespacePermissionsManager) new PlanPermissionsManager(requestContext, identifier, token, project);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.Dashboard))
        return (SecurityNamespacePermissionsManager) new DashboardPermissionsManager(requestContext, identifier, token, project);
      if (object.Equals((object) identifier, (object) NamespacePermissionSetConstants.AnalyticsViews))
        return (SecurityNamespacePermissionsManager) new AnalyticsViewsPermissionsManager(requestContext, identifier, token, project);
      using (IDisposableReadOnlyList<ISecurityNamespacePermissionsManagerFactory> extensions = requestContext.GetExtensions<ISecurityNamespacePermissionsManagerFactory>())
      {
        foreach (ISecurityNamespacePermissionsManagerFactory permissionsManagerFactory in (IEnumerable<ISecurityNamespacePermissionsManagerFactory>) extensions)
        {
          if (object.Equals((object) identifier, (object) permissionsManagerFactory.Identifier))
            return permissionsManagerFactory.GetPermissionsManager(requestContext, identifier, token, project);
        }
      }
      return (SecurityNamespacePermissionsManager) null;
    }
  }
}
