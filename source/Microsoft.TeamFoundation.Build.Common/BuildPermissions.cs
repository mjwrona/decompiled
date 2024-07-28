// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildPermissions
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build.Common
{
  [GenerateAllConstants(null)]
  public static class BuildPermissions
  {
    public static readonly int ViewBuilds = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuilds;
    public static readonly int EditBuildQuality = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildQuality;
    public static readonly int RetainIndefinitely = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.RetainIndefinitely;
    public static readonly int DeleteBuilds = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.DeleteBuilds;
    public static readonly int ManageBuildQualities = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ManageBuildQualities;
    public static readonly int DestroyBuilds = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.DestroyBuilds;
    public static readonly int UpdateBuildInformation = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.UpdateBuildInformation;
    public static readonly int QueueBuilds = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.QueueBuilds;
    public static readonly int ManageBuildQueue = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ManageBuildQueue;
    public static readonly int StopBuilds = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.StopBuilds;
    public static readonly int ViewBuildDefinition = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.ViewBuildDefinition;
    public static readonly int EditBuildDefinition = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditBuildDefinition;
    public static readonly int DeleteBuildDefinition = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.DeleteBuildDefinition;
    public static readonly int OverrideBuildCheckInValidation = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.OverrideBuildCheckInValidation;
    public static readonly int AdministerBuildPermissions = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.AdministerBuildPermissions;
    public static readonly int EditPipelineQueueConfigurationPermission = Microsoft.TeamFoundation.Build.WebApi.BuildPermissions.EditPipelineQueueConfigurationPermission;
    public static readonly int AllPermissions = BuildPermissions.ViewBuilds | BuildPermissions.EditBuildQuality | BuildPermissions.RetainIndefinitely | BuildPermissions.DeleteBuilds | BuildPermissions.ManageBuildQualities | BuildPermissions.DestroyBuilds | BuildPermissions.UpdateBuildInformation | BuildPermissions.QueueBuilds | BuildPermissions.ManageBuildQueue | BuildPermissions.StopBuilds | BuildPermissions.ViewBuildDefinition | BuildPermissions.EditBuildDefinition | BuildPermissions.DeleteBuildDefinition | BuildPermissions.OverrideBuildCheckInValidation | BuildPermissions.AdministerBuildPermissions | BuildPermissions.EditPipelineQueueConfigurationPermission;
  }
}
