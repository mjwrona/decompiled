// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TeamProjectSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamProjectSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("52D39943-CB85-4d7f-8FA8-C6BAAC873819");
    public static readonly int GenericRead = 1;
    public static readonly int GenericWrite = 2;
    public static readonly int Delete = 4;
    public static readonly int PublishTestResults = 8;
    public static readonly int AdministerBuild = 16;
    public static readonly int StartBuild = 32;
    public static readonly int EditBuildStatus = 64;
    public static readonly int UpdateBuild = 128;
    public static readonly int DeleteTestResults = 256;
    public static readonly int ViewTestResults = 512;
    public static readonly int ManageTestEnvironments = 2048;
    public static readonly int ManageTestConfigurations = 4096;
    public static readonly int WorkItemDelete = 8192;
    public static readonly int WorkItemMove = 16384;
    public static readonly int WorkItemPermanentlyDelete = 32768;
    public static readonly int Rename = 65536;
    public static readonly int ManageProperties = 131072;
    public static readonly int ManageSystemProperties = 262144;
    public static readonly int BypassPropertyCache = 524288;
    public static readonly int BypassRules = 1048576;
    public static readonly int SuppressNotifications = 2097152;
    public static readonly int UpdateVisibility = 4194304;
    public static readonly int ChangeProjectsProcess = 8388608;
    public static readonly int AgileToolsBacklogManagement = 16777216;
    public static readonly int AgileToolsPlans = 33554432;
    public static readonly int AllPermissions = TeamProjectSecurityConstants.GenericRead | TeamProjectSecurityConstants.GenericWrite | TeamProjectSecurityConstants.Delete | TeamProjectSecurityConstants.PublishTestResults | TeamProjectSecurityConstants.AdministerBuild | TeamProjectSecurityConstants.StartBuild | TeamProjectSecurityConstants.EditBuildStatus | TeamProjectSecurityConstants.UpdateBuild | TeamProjectSecurityConstants.DeleteTestResults | TeamProjectSecurityConstants.ViewTestResults | TeamProjectSecurityConstants.ManageTestEnvironments | TeamProjectSecurityConstants.ManageTestConfigurations | TeamProjectSecurityConstants.WorkItemDelete | TeamProjectSecurityConstants.WorkItemMove | TeamProjectSecurityConstants.WorkItemPermanentlyDelete | TeamProjectSecurityConstants.Rename | TeamProjectSecurityConstants.ManageProperties | TeamProjectSecurityConstants.BypassRules | TeamProjectSecurityConstants.SuppressNotifications | TeamProjectSecurityConstants.UpdateVisibility | TeamProjectSecurityConstants.ChangeProjectsProcess;
    public const string ProjectTokenPrefix = "$PROJECT:";

    public static string GetToken(string projectUri)
    {
      if (!string.IsNullOrEmpty(projectUri) && projectUri.StartsWith("$PROJECT:", StringComparison.OrdinalIgnoreCase))
        return projectUri + ":";
      if (projectUri == null)
        projectUri = string.Empty;
      return "$PROJECT:" + projectUri + ":";
    }
  }
}
