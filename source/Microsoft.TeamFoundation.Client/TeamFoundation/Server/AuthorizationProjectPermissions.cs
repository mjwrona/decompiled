// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationProjectPermissions
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server
{
  public static class AuthorizationProjectPermissions
  {
    public static readonly int GenericRead = TeamProjectPermissions.GenericRead;
    public static readonly int GenericWrite = TeamProjectPermissions.GenericWrite;
    public static readonly int Delete = TeamProjectPermissions.Delete;
    public static readonly int PublishTestResults = TeamProjectPermissions.PublishTestResults;
    public static readonly int AdministerBuild = TeamProjectPermissions.AdministerBuild;
    public static readonly int StartBuild = TeamProjectPermissions.StartBuild;
    public static readonly int EditBuildStatus = TeamProjectPermissions.EditBuildStatus;
    public static readonly int UpdateBuild = TeamProjectPermissions.UpdateBuild;
    public static readonly int DeleteTestResults = TeamProjectPermissions.DeleteTestResults;
    public static readonly int ViewTestResults = TeamProjectPermissions.ViewTestResults;
    public static readonly int ManageTestEnvironments = TeamProjectPermissions.ManageTestEnvironments;
    public static readonly int ManageTestConfigurations = TeamProjectPermissions.ManageTestConfigurations;
    public static readonly int WorkItemSoftDelete = TeamProjectPermissions.WorkItemDelete;
    public static readonly int WorkItemMove = TeamProjectPermissions.WorkItemMove;
    public static readonly int WorkItemPermanentlyDelete = TeamProjectPermissions.WorkItemPermanentlyDelete;
    public static readonly int Rename = TeamProjectPermissions.Rename;
    public static readonly int ManageProperties = TeamProjectPermissions.ManageProperties;
    public static readonly int ManageSystemProperties = TeamProjectPermissions.ManageSystemProperties;
    public static readonly int BypassPropertyCache = TeamProjectPermissions.BypassPropertyCache;
    public static readonly int BypassRules = TeamProjectPermissions.BypassRules;
    public static readonly int SuppressNotifications = TeamProjectPermissions.SuppressNotifications;
    public static readonly int UpdateVisibility = TeamProjectSecurityConstants.UpdateVisibility;
    public static readonly int AllPermissions = TeamProjectPermissions.AllPermissions;
  }
}
