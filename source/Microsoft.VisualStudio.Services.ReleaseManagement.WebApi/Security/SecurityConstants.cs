// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security.SecurityConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Properties;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security
{
  public static class SecurityConstants
  {
    public const string ContributorGroupName = "Contributors";
    public const string ReadersGroupName = "Readers";
    public const string ProjectAdministratorGroupName = "Project Administrators";
    public const ReleaseManagementSecurityPermissions AllPermissions = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.EditReleaseDefinition | ReleaseManagementSecurityPermissions.DeleteReleaseDefinition | ReleaseManagementSecurityPermissions.ManageReleaseApprovers | ReleaseManagementSecurityPermissions.ManageReleases | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.CreateReleases | ReleaseManagementSecurityPermissions.EditReleaseEnvironment | ReleaseManagementSecurityPermissions.DeleteReleaseEnvironment | ReleaseManagementSecurityPermissions.AdministerReleasePermissions | ReleaseManagementSecurityPermissions.DeleteReleases | ReleaseManagementSecurityPermissions.ManageDeployments | ReleaseManagementSecurityPermissions.ManageReleaseSettings;
    public const ReleaseManagementSecurityPermissions NoPermission = ReleaseManagementSecurityPermissions.None;
    public const ReleaseManagementSecurityPermissions ContributorGroupAllowPermissionMask = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.EditReleaseDefinition | ReleaseManagementSecurityPermissions.DeleteReleaseDefinition | ReleaseManagementSecurityPermissions.ManageReleaseApprovers | ReleaseManagementSecurityPermissions.ManageReleases | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.CreateReleases | ReleaseManagementSecurityPermissions.EditReleaseEnvironment | ReleaseManagementSecurityPermissions.DeleteReleaseEnvironment | ReleaseManagementSecurityPermissions.DeleteReleases | ReleaseManagementSecurityPermissions.ManageDeployments;
    public const ReleaseManagementSecurityPermissions ReadersGroupAllowPermissionMask = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.ViewReleases;
    public const ReleaseManagementSecurityPermissions BuildServiceIdentityPermissionMask = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.ManageTaskHubExtension;
    public const ReleaseManagementSecurityPermissions ProjectAdministratorGroupAllowPermissionMask = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.EditReleaseDefinition | ReleaseManagementSecurityPermissions.DeleteReleaseDefinition | ReleaseManagementSecurityPermissions.ManageReleaseApprovers | ReleaseManagementSecurityPermissions.ManageReleases | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.CreateReleases | ReleaseManagementSecurityPermissions.EditReleaseEnvironment | ReleaseManagementSecurityPermissions.DeleteReleaseEnvironment | ReleaseManagementSecurityPermissions.AdministerReleasePermissions | ReleaseManagementSecurityPermissions.DeleteReleases | ReleaseManagementSecurityPermissions.ManageDeployments | ReleaseManagementSecurityPermissions.ManageReleaseSettings;
    public static readonly Guid ReleaseManagementSecurityNamespaceId = new Guid("c788c23e-1b46-4162-8f5e-d7585343b5de");
    private static readonly IDictionary<ReleaseManagementSecurityPermissions, string> PermissionResourceMapData = (IDictionary<ReleaseManagementSecurityPermissions, string>) new Dictionary<ReleaseManagementSecurityPermissions, string>()
    {
      {
        ReleaseManagementSecurityPermissions.ViewReleaseDefinition,
        Resources.ViewReleaseDefinition
      },
      {
        ReleaseManagementSecurityPermissions.EditReleaseDefinition,
        Resources.EditReleaseDefinition
      },
      {
        ReleaseManagementSecurityPermissions.DeleteReleaseDefinition,
        Resources.DeleteReleaseDefinition
      },
      {
        ReleaseManagementSecurityPermissions.DeleteReleases,
        Resources.DeleteReleases
      },
      {
        ReleaseManagementSecurityPermissions.ManageDeployments,
        Resources.ManageDeployments
      },
      {
        ReleaseManagementSecurityPermissions.ManageReleaseApprovers,
        Resources.ManageReleaseApprovers
      },
      {
        ReleaseManagementSecurityPermissions.ManageReleases,
        Resources.ManageReleases
      },
      {
        ReleaseManagementSecurityPermissions.ViewReleases,
        Resources.ViewReleases
      },
      {
        ReleaseManagementSecurityPermissions.CreateReleases,
        Resources.CreateReleases
      },
      {
        ReleaseManagementSecurityPermissions.EditReleaseEnvironment,
        Resources.EditReleaseEnvironment
      },
      {
        ReleaseManagementSecurityPermissions.DeleteReleaseEnvironment,
        Resources.DeleteReleaseEnvironment
      },
      {
        ReleaseManagementSecurityPermissions.AdministerReleasePermissions,
        Resources.AdministerReleasePermissions
      },
      {
        ReleaseManagementSecurityPermissions.ManageReleaseSettings,
        Resources.ManageReleaseSettings
      },
      {
        ReleaseManagementSecurityPermissions.ManageTaskHubExtension,
        Resources.ManageTaskHubExtension
      }
    };
    public static readonly Guid ReleaseManagementUISecurityNamespaceId = new Guid("7c7d32f7-0e86-4cd6-892e-b35dbba870bd");
    public static readonly string ReleaseManagementUIPermissionToken = "/ReleaseManagementUI";

    public static string ReleaseAdministratorsGroup => Resources.ReleaseAdministratorsGroupName;

    public static string ReleaseManagersGroupName => Resources.ReleaseManagersGroupName;

    public static IDictionary<ReleaseManagementSecurityPermissions, string> PermissionResourceMap => SecurityConstants.PermissionResourceMapData;
  }
}
