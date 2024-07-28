// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.DashboardPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Dashboards;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class DashboardPermissionsManager : SecurityNamespacePermissionsManager
  {
    public bool IsUserTeamAdmin;
    public bool IsProjectDashboard;
    public bool isTeamAgnosticEnabled;

    public DashboardPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token,
      string projectName)
      : base(permissionsIdentifier)
    {
      this.isTeamAgnosticEnabled = requestContext.IsFeatureEnabled("WebAccess.Dashboards.TeamAgnosticDashboards");
      IProjectService service1 = requestContext.GetService<IProjectService>();
      this.ParsedToken = JsonConvert.DeserializeObject<DashboardToken>(token);
      this.ProjectId = service1.GetProjectId(requestContext, projectName);
      ITeamService service2 = requestContext.GetService<ITeamService>();
      Guid teamGuid = this.ParsedToken.teamId.Value;
      if (teamGuid == Guid.Empty)
      {
        this.IsProjectDashboard = true;
      }
      else
      {
        WebApiTeam teamByGuid = service2.GetTeamByGuid(requestContext, teamGuid);
        this.IsUserTeamAdmin = service2.UserIsTeamAdmin(requestContext, teamByGuid.Identity);
      }
      string securityToken = DashboardSecurityManager.GetSecurityToken(requestContext, this.ProjectId, new Guid?(teamGuid), this.ParsedToken.dashboardId);
      this.Initialize(requestContext, securityToken);
    }

    public DashboardToken ParsedToken { get; set; }

    public Guid ProjectId { get; set; }

    public override bool HideExplicitClearButton => true;

    public override bool HideToolbar => !this.isTeamAgnosticEnabled || !this.IsProjectDashboard;

    public override string CustomDoNotHavePermissionsText => AdminResources.CustomDoNotHavePermissionsText;

    protected override bool CanUserManageIdentities(IVssRequestContext requestContext) => this.isTeamAgnosticEnabled && this.IsProjectDashboard;

    public override bool CanManagePermissions => this.IsUserTeamAdmin || this.IsProjectDashboard;

    public override IList<SettableAction> GetPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      IList<SettableAction> permissions1 = base.GetPermissions(requestContext, descriptor);
      if (!this.IsUserTeamAdmin)
        return permissions1;
      List<SettableAction> permissions2 = new List<SettableAction>();
      foreach (SettableAction settableAction in (IEnumerable<SettableAction>) permissions1)
      {
        settableAction.CanEdit = true;
        permissions2.Add(settableAction);
      }
      return (IList<SettableAction>) permissions2;
    }

    public override void SetPermission(
      IVssRequestContext requestContext,
      SettableAction settableAction,
      bool allowSet,
      bool denySet)
    {
      SecurityNamespacePermissionSet permissionSet = this.PermissionSets[settableAction.NamespaceId];
      IAccessControlEntry accessControlEntry = settableAction.AccessControlEntry;
      int bit = settableAction.ActionDefinition.Bit;
      accessControlEntry.Allow = allowSet ? accessControlEntry.Allow | bit : accessControlEntry.Allow & ~bit;
      accessControlEntry.Deny = denySet ? accessControlEntry.Deny | bit : accessControlEntry.Deny & ~bit;
      IdentityDescriptor descriptor = accessControlEntry.Descriptor;
      if (requestContext.ServiceHost.InstanceId != permissionSet.InstanceId)
      {
        descriptor = requestContext.GetService<IdentityService>().MapFromWellKnownIdentifier(accessControlEntry.Descriptor);
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      }
      if (allowSet)
        permissionSet.SecurityNamespace.SetAccessControlEntries(requestContext, settableAction.Token, (IEnumerable<IAccessControlEntry>) new AccessControlEntry[1]
        {
          new AccessControlEntry(descriptor, bit, 0)
        }, true);
      else if (denySet)
        permissionSet.SecurityNamespace.SetAccessControlEntries(requestContext, settableAction.Token, (IEnumerable<IAccessControlEntry>) new AccessControlEntry[1]
        {
          new AccessControlEntry(descriptor, 0, bit)
        }, true);
      else
        permissionSet.SecurityNamespace.RemovePermissions(requestContext, settableAction.Token, accessControlEntry.Descriptor, bit);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = 12 | (!this.isTeamAgnosticEnabled || !this.IsProjectDashboard ? 0 : 16) | (!this.ParsedToken.dashboardId.HasValue ? 2 : 0);
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, DashboardSecurityConstants.SecurityNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(DashboardSecurityConstants.SecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    public override bool ShouldIncludeIdentity(TeamFoundationIdentity teamFoundationIdentity)
    {
      if (this.isTeamAgnosticEnabled && this.IsProjectDashboard)
        return true;
      Guid teamFoundationId = teamFoundationIdentity.TeamFoundationId;
      Guid? teamId = this.ParsedToken.teamId;
      return teamId.HasValue && teamFoundationId == teamId.GetValueOrDefault();
    }
  }
}
