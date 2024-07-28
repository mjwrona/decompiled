// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Constants.ServiceProcessorConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Constants
{
  public static class ServiceProcessorConstants
  {
    public const string StartEnvironmentJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.StartEnvironmentJob";
    public const string StartEnvironmentJobName = "OnStartEnvironment";
    public const string OnReleaseCreatedJobName = "OnReleaseCreated";
    public const string CreateNextStepJob = "CreateNextStep";
    public const string CreateNextStepJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.CreateNextStepJob";
    public const string OnReleaseCreatedOrUpdatedJobName = "OnReleaseCreatedOrUpdated";
    public const string ReleaseDefinitionDeletionJobName = "OnReleaseDefinitionDeleted";
    public const string ReleaseDefinitionDeletionJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.ReleaseDefinitionDeletionJob";
    public const string ReleaseDeletionJobName = "OnReleaseDeleted";
    public const string ReleaseDeletionJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.ReleaseDeletionJob";
    public const string OnReleaseCreatedJobExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.OnReleaseCreatedJob";
    public const string OnReleaseCreatedOrUpdatedJobExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.OnReleaseCreatedOrUpdatedJob";
    public const string DefinitionEnvironmentHealthJobExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DefinitionEnvironmentHealthJob";
    public const string DefinitionEnvironmentHealthJobName = "OnDefinitionEnvironmentHealthJob";
    public const string SaveReleaseDefinitionSnapshotRevisionJobName = "SaveReleaseDefinitionSnapshotRevisionJob";
    public const string SaveReleaseDefinitionSnapshotRevisionJobExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.SaveReleaseDefinitionSnapshotRevisionJob";
    public const string UpdateRetainBuildJobExtensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.UpdateRetainBuildJob";
    public const string UpdateRetainBuildJobName = "UpdateRetainBuild";
    public const string ContinuousDeploymentSetupJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.ContinuousDeploymentSetupJob";
    public const string ContinuousDeploymentSetupJobName = "ContinuousDeploymentSetup";
    public const string DeploymentApprovalJobName = "OnDeploymentApproval";
    public const string DeploymentApprovalJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DeploymentApprovalJob";
    public const string DeploymentApprovalTimeoutJobName = "OnDeploymentApprovalTimeout";
    public const string DeploymentApprovalTimeoutJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.DeploymentApprovalTimeoutHandlerJob";
    public const string RedeployTriggerHandlerJobClassName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtension.RedeployTriggerHandlerJob";
    public const string RedeployTriggerHandlerJobName = "RedeployTriggerHandler";
    public const string DoNotQueueStartEnvironmentJobContextItemName = "DontQueueStartEnvironmentJob";
  }
}
