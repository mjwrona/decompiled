// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.AuthorizationProjectPermissions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Integration.Server
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
    public static readonly int BypassRules = TeamProjectPermissions.BypassRules;
    public static readonly int SuppressNotifications = TeamProjectPermissions.SuppressNotifications;
    public static readonly int AllPermissions = TeamProjectPermissions.AllPermissions;
  }
}
