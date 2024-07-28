// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ReleaseManagement.Common.ReleaseManagementPermissions
// Assembly: Microsoft.TeamFoundation.ReleaseManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8D41ADCA-F510-4062-8F3B-11F99E5C673A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ReleaseManagement.Common.dll

namespace Microsoft.TeamFoundation.ReleaseManagement.Common
{
  public static class ReleaseManagementPermissions
  {
    public static readonly int ViewReleaseDefinition = 1;
    public static readonly int EditReleaseDefinition = 2;
    public static readonly int DeleteReleaseDefinition = 4;
    public static readonly int ManageReleaseApprovers = 8;
    public static readonly int ManageReleases = 16;
    public static readonly int ViewReleases = 32;
    public static readonly int QueueReleases = 64;
    public static readonly int EditReleaseEnvironment = 128;
    public static readonly int DeleteReleaseEnvironment = 256;
    public static readonly int AdministerReleasePermissions = 512;
    public static readonly int DeleteReleases = 1024;
    public static readonly int ManageDeployments = 2048;
    public static readonly int AllPermissions = ReleaseManagementPermissions.ViewReleaseDefinition | ReleaseManagementPermissions.EditReleaseDefinition | ReleaseManagementPermissions.DeleteReleaseDefinition | ReleaseManagementPermissions.ManageReleaseApprovers | ReleaseManagementPermissions.ManageReleases | ReleaseManagementPermissions.ViewReleases | ReleaseManagementPermissions.QueueReleases | ReleaseManagementPermissions.EditReleaseEnvironment | ReleaseManagementPermissions.DeleteReleaseEnvironment | ReleaseManagementPermissions.AdministerReleasePermissions | ReleaseManagementPermissions.DeleteReleases | ReleaseManagementPermissions.ManageDeployments;
    public static readonly int EnvironmentDefinitionPermissions = ReleaseManagementPermissions.AdministerReleasePermissions | ReleaseManagementPermissions.EditReleaseEnvironment | ReleaseManagementPermissions.DeleteReleaseEnvironment | ReleaseManagementPermissions.ManageReleaseApprovers | ReleaseManagementPermissions.ManageDeployments;
  }
}
