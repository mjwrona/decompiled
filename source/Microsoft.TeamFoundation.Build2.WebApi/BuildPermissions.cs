// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildPermissions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public static class BuildPermissions
  {
    public static readonly int ViewBuilds = 1;
    public static readonly int EditBuildQuality = 2;
    public static readonly int RetainIndefinitely = 4;
    public static readonly int DeleteBuilds = 8;
    public static readonly int ManageBuildQualities = 16;
    public static readonly int DestroyBuilds = 32;
    public static readonly int UpdateBuildInformation = 64;
    public static readonly int QueueBuilds = 128;
    public static readonly int ManageBuildQueue = 256;
    public static readonly int StopBuilds = 512;
    public static readonly int ViewBuildDefinition = 1024;
    public static readonly int EditBuildDefinition = 2048;
    public static readonly int DeleteBuildDefinition = 4096;
    public static readonly int OverrideBuildCheckInValidation = 8192;
    public static readonly int AdministerBuildPermissions = 16384;
    public static readonly int EditPipelineQueueConfigurationPermission = 65536;
    public static readonly int AllPermissions = BuildPermissions.ViewBuilds | BuildPermissions.EditBuildQuality | BuildPermissions.RetainIndefinitely | BuildPermissions.DeleteBuilds | BuildPermissions.ManageBuildQualities | BuildPermissions.DestroyBuilds | BuildPermissions.UpdateBuildInformation | BuildPermissions.QueueBuilds | BuildPermissions.ManageBuildQueue | BuildPermissions.StopBuilds | BuildPermissions.ViewBuildDefinition | BuildPermissions.EditBuildDefinition | BuildPermissions.DeleteBuildDefinition | BuildPermissions.OverrideBuildCheckInValidation | BuildPermissions.AdministerBuildPermissions | BuildPermissions.EditPipelineQueueConfigurationPermission;
  }
}
