// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TeamProjectPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class TeamProjectPermissions
  {
    public static readonly int GenericRead = TeamProjectSecurityConstants.GenericRead;
    public static readonly int GenericWrite = TeamProjectSecurityConstants.GenericWrite;
    public static readonly int Delete = TeamProjectSecurityConstants.Delete;
    public static readonly int PublishTestResults = TeamProjectSecurityConstants.PublishTestResults;
    public static readonly int AdministerBuild = TeamProjectSecurityConstants.AdministerBuild;
    public static readonly int StartBuild = TeamProjectSecurityConstants.StartBuild;
    public static readonly int EditBuildStatus = TeamProjectSecurityConstants.EditBuildStatus;
    public static readonly int UpdateBuild = TeamProjectSecurityConstants.UpdateBuild;
    public static readonly int DeleteTestResults = TeamProjectSecurityConstants.DeleteTestResults;
    public static readonly int ViewTestResults = TeamProjectSecurityConstants.ViewTestResults;
    public static readonly int ManageTestEnvironments = TeamProjectSecurityConstants.ManageTestEnvironments;
    public static readonly int ManageTestConfigurations = TeamProjectSecurityConstants.ManageTestConfigurations;
    public static readonly int WorkItemDelete = TeamProjectSecurityConstants.WorkItemDelete;
    public static readonly int WorkItemMove = TeamProjectSecurityConstants.WorkItemMove;
    public static readonly int WorkItemPermanentlyDelete = TeamProjectSecurityConstants.WorkItemPermanentlyDelete;
    public static readonly int Rename = TeamProjectSecurityConstants.Rename;
    public static readonly int ManageProperties = TeamProjectSecurityConstants.ManageProperties;
    public static readonly int ManageSystemProperties = TeamProjectSecurityConstants.ManageSystemProperties;
    public static readonly int BypassPropertyCache = TeamProjectSecurityConstants.BypassPropertyCache;
    public static readonly int BypassRules = TeamProjectSecurityConstants.BypassRules;
    public static readonly int SuppressNotifications = TeamProjectSecurityConstants.SuppressNotifications;
    public static readonly int UpdateVisibility = TeamProjectSecurityConstants.UpdateVisibility;
    public static readonly int ChangeProjectsProcess = TeamProjectSecurityConstants.ChangeProjectsProcess;
    public static readonly int AllPermissions = TeamProjectSecurityConstants.AllPermissions;
  }
}
