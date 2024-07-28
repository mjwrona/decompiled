// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class AdminAreaController : TfsAreaController
  {
    private IVssRequestContext m_tfsRequestContext;

    internal WebAccessWorkItemService WitService => this.TfsRequestContext.GetService<WebAccessWorkItemService>();

    public override string AreaName => "Admin";

    public override string TraceArea => "WebAccess.Admin";

    protected bool IsHosted => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment;

    protected bool CanCreateProjects => this.IsHosted || this.Request.Cookies["Tfs-EnablePCW"] != null;

    protected WebApiTeam Team => this.TfsRequestContext.GetWebTeamContext().Team;

    protected void CheckIsHosted(bool requireHosted)
    {
      if (requireHosted ^ this.IsHosted)
        throw new HttpException(404, WACommonResources.PageNotFound);
    }

    protected void CheckManageLicensesPermission()
    {
      if (!LicenseHelpers.HasManageLicensesPermission(this.TfsRequestContext))
        throw new HttpException(404, WACommonResources.PageNotFound);
    }

    internal static bool HasManageServicesPermission(IVssRequestContext requestContext) => true;

    protected void CheckManageServicesPermission()
    {
      if (!AdminAreaController.HasManageServicesPermission(this.TfsRequestContext))
        throw new HttpException(404, WACommonResources.PageNotFound);
    }

    internal SecurityModel CreateSecurityModel(
      Guid? permissionSet,
      string token,
      string tokenDisplayVal,
      out SecurityNamespacePermissionsManager permissionsManager,
      bool? showAllGroupsIfCollection,
      bool isOrganizationLevel = false,
      bool controlManagesFocus = true,
      bool allowAADSearchInHosted = false)
    {
      Guid permissionSetId = AdminAreaController.GetPermissionSetId(this.NavigationContext, permissionSet);
      if (AdminAreaController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
      {
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        permissionSetId = NamespacePermissionSetConstants.OrganizationLevel;
      }
      if (string.IsNullOrWhiteSpace(token))
        token = AdminAreaController.GetDefaultPermissionSetToken(this.TfsRequestContext, this.TfsWebContext, permissionSetId);
      permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, permissionSetId, token);
      if (permissionsManager == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameter, (object) nameof (permissionSet)));
      return new SecurityModel()
      {
        ViewTitle = tokenDisplayVal,
        TitlePrefix = AdminAreaController.GetPermissionTitlePrefix(permissionSetId),
        Title = AdminAreaController.GetPermissionTitle(this.TfsWebContext, permissionSetId, token, tokenDisplayVal),
        Header = AdminAreaController.GetPermissionHeader(permissionSetId),
        PermissionSetId = permissionSetId,
        Token = token,
        CanManageIdentities = permissionsManager.CanManageIdentities,
        CanManagePermissions = permissionsManager.CanManagePermissions,
        InheritPermissions = permissionsManager.InheritPermissions,
        CanTokenInheritPermissions = permissionsManager.CanTokenInheritPermissions,
        CustomDoNotHavePermissionsText = permissionsManager.CustomDoNotHavePermissionsText,
        HideExplicitClearButton = permissionsManager.HideExplicitClearButton,
        HideToolbar = permissionsManager.HideToolbar,
        ShowAllGroupsIfCollection = showAllGroupsIfCollection.HasValue && showAllGroupsIfCollection.Value,
        ControlManagesFocus = controlManagesFocus,
        AllowAADSearchInHosted = allowAADSearchInHosted
      };
    }

    protected static string GetPermissionTitlePrefix(Guid permissionSetId)
    {
      if (permissionSetId == NamespacePermissionSetConstants.VersionControl)
        return AdminResources.Path;
      if (permissionSetId == NamespacePermissionSetConstants.Build)
        return AdminResources.BuildDefinition;
      if (permissionSetId == NamespacePermissionSetConstants.Area)
        return AdminResources.Area;
      if (permissionSetId == NamespacePermissionSetConstants.Iteration)
        return AdminResources.Iteration;
      return permissionSetId == NamespacePermissionSetConstants.WitQueryFolders ? AdminResources.Path : string.Empty;
    }

    protected static string GetPermissionTitle(
      TfsWebContext webContext,
      Guid permissionSetId,
      string token,
      string tokenDisplayVal)
    {
      if (permissionSetId == NamespacePermissionSetConstants.ProjectLevel)
        return webContext.Project.Name;
      if (permissionSetId == NamespacePermissionSetConstants.CollectionLevel)
        return webContext.TfsRequestContext.ServiceHost.CollectionServiceHost.Name;
      return permissionSetId == NamespacePermissionSetConstants.VersionControl ? token : tokenDisplayVal;
    }

    internal SecurityNamespacePermissionsManager GetSecurityNamespacePermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionSetId,
      string token,
      Guid? teamId = null)
    {
      return SecurityNamespacePermissionsManagerFactory.CreateManager(requestContext, permissionSetId, token, this.TfsWebContext.ProjectContext?.Name, teamId);
    }

    protected static Guid GetPermissionSetId(NavigationContext navigationContext, Guid? id) => !id.HasValue ? (!navigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project) ? NamespacePermissionSetConstants.CollectionLevel : NamespacePermissionSetConstants.ProjectLevel) : id.Value;

    protected static string GetDefaultPermissionSetToken(
      IVssRequestContext requestContext,
      TfsWebContext webContext,
      Guid permissionSetId)
    {
      return permissionSetId == NamespacePermissionSetConstants.ProjectLevel ? webContext.Project.Uri : string.Empty;
    }

    private static string GetPermissionHeader(Guid permissionSetId)
    {
      if (permissionSetId == NamespacePermissionSetConstants.OrganizationLevel)
        return AdminServerResources.OrganizationPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.ProjectLevel)
        return AdminServerResources.ProjectPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.CollectionLevel)
        return AdminServerResources.CollectionPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.VersionControl)
        return AdminServerResources.VersionControlPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Build)
        return AdminServerResources.BuildPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Area)
        return AdminServerResources.AreaPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.Iteration)
        return AdminServerResources.IterationPermissions;
      if (permissionSetId == NamespacePermissionSetConstants.WitQueryFolders)
        return AdminServerResources.WorkItemQueryPermissions;
      return permissionSetId == NamespacePermissionSetConstants.Plan ? AdminServerResources.PlanPermissions : string.Empty;
    }

    public new virtual IVssRequestContext TfsRequestContext
    {
      get
      {
        if (this.m_tfsRequestContext == null)
          this.m_tfsRequestContext = base.TfsRequestContext;
        return this.m_tfsRequestContext;
      }
      set => this.m_tfsRequestContext = value;
    }

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }
  }
}
