// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ProjectPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class ProjectPermissionsManager : SecurityNamespacePermissionsManager
  {
    public ProjectPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token,
      Guid? teamFoundationId = null)
      : base(requestContext, permissionsIdentifier, token)
    {
    }

    protected override bool CanUserViewPermissions(IVssRequestContext requestContext)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      return this.PermissionSets.TryGetValue(FrameworkSecurity.TeamProjectNamespaceId, out namespacePermissionSet) && namespacePermissionSet.HasReadPermission(requestContext, namespacePermissionSet.HandleIncomingToken(requestContext, this.Token));
    }

    protected override bool CanUserManageIdentities(IVssRequestContext requestContext)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      return this.PermissionSets.TryGetValue(FrameworkSecurity.TeamProjectNamespaceId, out namespacePermissionSet) && namespacePermissionSet.HasWritePermission(requestContext, namespacePermissionSet.HandleIncomingToken(requestContext, this.Token));
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = SecurityNamespacePermissionSet.AllPermissions & ~(TeamProjectPermissions.AdministerBuild | TeamProjectPermissions.StartBuild | TeamProjectPermissions.EditBuildStatus | TeamProjectPermissions.UpdateBuild);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        permissionsToDisplay &= ~TeamProjectPermissions.UpdateVisibility;
      SecurityNamespacePermissionSet namespacePermissionSet1 = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.TeamProjectNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(FrameworkSecurity.TeamProjectNamespaceId, namespacePermissionSet1);
      Guid projectId = ProjectInfo.GetProjectId(ProjectInfo.NormalizeProjectUri(this.Token));
      SecurityNamespacePermissionSet namespacePermissionSet2 = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.TaggingNamespaceId, TaggingService.GetSecurityToken(new Guid?(projectId)), TaggingPermissions.Create);
      permissionSets.Add(FrameworkSecurity.TaggingNamespaceId, namespacePermissionSet2);
      IAnalyticsFeatureService service1 = requestContext.GetService<IAnalyticsFeatureService>();
      ITeamFoundationFeatureAvailabilityService service2 = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      IVssRequestContext requestContext1 = requestContext;
      if (service1.IsAnalyticsEnabled(requestContext1))
      {
        SecurityNamespacePermissionSet namespacePermissionSet3 = new SecurityNamespacePermissionSet(requestContext, AnalyticsSecurityNamespace.Id, AnalyticsSecurityNamespace.GetSecurityToken(projectId), 1);
        permissionSets.Add(AnalyticsSecurityNamespace.Id, namespacePermissionSet3);
        if (service2.IsFeatureEnabled(requestContext, "Analytics.Views.EditableUI") && service2.IsFeatureEnabled(requestContext, "Analytics.Views.ProjectLevelPermissions"))
        {
          SecurityNamespacePermissionSet namespacePermissionSet4 = new SecurityNamespacePermissionSet(requestContext, AnalyticsViewsSecurityNamespace.SecurityNamespaceId, AnalyticsViewsSecurityNamespace.GetSecurityToken(AnalyticsViewVisibility.Shared, projectId), 6);
          permissionSets.Add(AnalyticsViewsSecurityNamespace.SecurityNamespaceId, namespacePermissionSet4);
        }
      }
      return permissionSets;
    }
  }
}
