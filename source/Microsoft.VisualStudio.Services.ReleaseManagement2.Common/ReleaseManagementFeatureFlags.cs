// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.ReleaseManagementFeatureFlags
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common
{
  [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Defines RM feature flags")]
  public static class ReleaseManagementFeatureFlags
  {
    public const string UseReleaseManagementTaskDispatcherForEvents = "VisualStudio.ReleaseManagement.UseTaskDispatcherForEvents";
    public const string DeleteJobDefinitionAfterSuccessfulRun = "VisualStudio.ReleaseManagement.DeleteJobDefinitionAfterSuccessfulRun";
    public const string ApprovalPolicies = "VisualStudio.ReleaseManagement.ApprovalPolicies";
    public const string AllowRedeployAndCancelForQueueReleasesPermission = "WebAccess.ReleaseManagement.ReleasesHub.AllowRedeployAndCancelForQueueReleasesPermission";
    public const string MachineGroupDynamicMachinePool = "VisualStudio.ReleaseManagement.MachineGroup.DynamicMachinePool";
    public const string ReleaseRetentionSettingsEditEnabled = "VisualStudio.ReleaseManagement.ReleaseRetentionSettingsEditEnabled";
    public const string ReleaseRetentionSettingsMaxDaysToKeepEditEnabled = "VisualStudio.ReleaseManagement.ReleaseRetentionSettingsMaxDaysToKeepEditEnabled";
    public const string BuildArtifactTasks = "VisualStudio.ReleaseManagement.BuildArtifactsTasks";
    public const string EnableBackCompatibilityValidations = "VisualStudio.ReleaseManagement.EnableBackCompatValidations";
    public const string CustomBuildArtifactTasks = "VisualStudio.ReleaseManagement.CustomBuildArtifactsTasks";
    public const string UseDropArtifactTask = "VisualStudio.ReleaseManagement.UseDropArtifactTask";
    public const string TaskGroupVersioning = "WebAccess.DistributedTask.EnableTaskGroupVersioning";
    public const string QueuedForPipeline = "WebAccess.ReleaseManagement.QueuedForPipeline";
    public const string EnableDeploymentPoolRename = "WebAccess.ReleaseManagement.DeploymentPoolRename";
    public const string DevOpsReporting = "WebAccess.ReleaseManagement.DevOpsReporting";
    public const string UseQueryDeploymentsToCancelPullRequestRelease = "VisualStudio.ReleaseManagement.UseQueryDeploymentsToCancelPullRequestRelease";
    public const string ShowReleaseProgressCanvasZoomOptions = "WebAccess.ReleaseManagement.ShowReleaseProgressCanvasZoomOptions";
    public const string AddPropertiesBagToPullRequestStatus = "VisualStudio.ReleaseManagement.AddPropertiesBagToPullRequestStatus";
    public const string EnableFetchingFlightAssignments = "WebAccess.ReleaseManagement.FlightAssignments";
    public const string DisableReleaseJobsWhenPipelinesAreDisabled = "VisualStudio.ReleaseManagement.DisableReleaseJobsWhenPipelinesAreDisabled";
    public const string EnableFastTaskGroupExpansion = "VisualStudio.ReleaseManagement.FastTaskGroupExpansion";
    public const string UseNewBranding = "VisualStudio.Services.WebPlatform.UseNewBranding";
    public const string DefaultToLatestArtifactVersion = "VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion";
    public const string PostGitHubDeploymentStatus = "VisualStudio.ReleaseManagement.PostGitHubDeploymentStatus";
    public const string ManageTaskHubExtensionPermissions = "VisualStudio.ReleaseManagement.ManageTaskHubExtensionPermissions";
    public const string SearchIdentityById = "AzureDevops.ReleaseManagement.SearchIdentityById";
    public const string UseSystemUserIdForScheduledRelease = "AzureDevops.ReleaseManagement.UseSystemUserIdForScheduledRelease";
    public const string GitHubArtifactPagination = "AzureDevOps.ReleaseManagement.EnablePaginationForGitHubRepositories";
    public const string StartReleaseEnvironmentActionRequestProcessorJob = "AzureDevops.ReleaseManagement.StartReleaseEnvironmentActionRequestProcessorJob";
    public const string DeploymentPoolSharingTelemetryEnable = "WebAccess.ReleaseManagement.EnableDeploymentPoolSharingTelemetry";
    public const string RemovePropertiesWithDefaultValuesInArtifactConditions = "AzureDevOps.ReleaseManagement.RemovePropertiesWithDefaultValuesInArtifactConditions";
    public const string BlockFolderNamesWithDigits = "AzureDevops.ReleaseManagement.BlockFolderNamesWithDigits";
    public const string DisableScheduledJobIdRegeneration = "VisualStudio.ReleaseManagement.DisableScheduledJobIdRegenerationForDataImport";
    public const string InjectDownloadPipelineArtifactV2 = "AzureDevOps.ReleaseManagement.InjectDownloadPipelineArtifactV2";
    public const string InjectLatestDownloadBuildArtifactsTask = "AzureDevOps.ReleaseManagement.InjectLatestDownloadBuildArtifactsTask";
    public const string FallbackToBuildArtifactsTaskV0 = "AzureDevOps.ReleaseManagement.FallbackToBuildArtifactsTaskV0";
    public const string ReturnOnlyLatestDeploymentData = "VisualStudio.ReleaseManagement.ReturnOnlyLatestDeploymentData";
    public const string EnableArtifactVersionPicker = "AzureDevOps.ReleaseManagement.EnableArtifactVersionPicker";
    public const string EnableGitHubDataSourcesForGitHubArtifact = "AzureDevOps.ReleaseManagement.EnableGitHubDataSourcesForGitHubArtifact";
    public const string EnableGitHubDataSourcesForBuildArtifact = "AzureDevOps.ReleaseManagement.EnableGitHubDataSourcesForBuildArtifact";
    public const string DisableSecretScanToBlockSave = "VisualStudio.ReleaseManagement.DisableSecretScanToBlockSave";
    public const string ReleaseComplianceSettingsEnabled = "VisualStudio.ReleaseManagement.ReleaseComplianceSettingsEnabled";
    public const string ValidateTaskInputTypesAgainstArtifactInputTypesForEndpointTypeInputs = "AzureDevOps.ReleaseManagement.ValidateTaskInputTypes";
    public const string DelayedReleaseTasksSignalRUpdate = "AzureDevOps.ReleaseManagement.DelayedReleaseTasksSignalRUpdate";
    public const string ShowServiceConnectionsUsedInLinkedArtifacts = "AzureDevOps.ReleaseManagement.ShowServiceConnectionsUsedInLinkedArtifacts";
    public const string UseDownloadPipelineArtifactTaskForServerBuildArtifacts = "AzureDevOps.ReleaseManagement.UseDownloadPipelineArtifactTaskForServerBuildArtifacts";
    public const string ValidateStageForResourcesWithChecks = "AzureDevOps.ReleaseManagement.ValidateStageForResourcesWithChecks";
    public const string RemoveExcessEventData = "AzureDevOps.ReleaseManagement.RemoveExcessEventData";
    public const string ValidateReleaseEnvironmentConditions = "AzureDevOps.ReleaseManagement.ValidateReleaseEnvironmentConditions";
    public const string SkipCompletedEventEnvironmentIdCheck = "AzureDevOps.ReleaseManagement.SkipCompletedEventEnvironmentIdCheck";
    public const string DeletedEndpointValidationsForServiceEndpointsInRM = "AzureDevOps.ReleaseManagement.DeletedEndpointValidationsForServiceEndpoints";
    public const string VariableNameValidationsForServiceEndpointsInRM = "AzureDevOps.ReleaseManagement.VariableNameValidationsForServiceEndpoints";
    public const string CancelPriorScheduledReleasesOnManualDeploy = "AzureDevOps.ReleaseManagement.CancelPriorScheduledReleasesOnManualDeploy";
    public const string HonorJobScopeSettings = "AzureDevOps.ReleaseManagement.HonorJobScopeSettings";
    public const string CacheJobScopeSettings = "AzureDevOps.ReleaseManagement.CacheJobServiceIdentity";
    public const string UseBuildRetentionLeases = "AzureDevOps.ReleaseManagement.UseBuildRetentionLeases";
    public const string EnableBuildRetentionBatchingCall = "AzureDevOps.ReleaseManagement.EnableBuildRetentionBatchingCall";
    public const string MakeBuildClientCallSync = "AzureDevOps.ReleaseManagement.MakeBuildClientCallSync";
    public const string UseDirectReleaseStageScheduling = "AzureDevOps.ReleaseManagement.UseDirectReleaseStageScheduling";
    public const string Return404WhenNoRevisionFound = "AzureDevOps.ReleaseManagement.Return404WhenNoRevisionFound";
    public const string PropertyValueDataTypeConversionEnabled = "AzureDevOps.ReleaseManagement.PropertyValueDataTypeConversionEnabled";
    public const string HonorIsGeneratedBitForIgnoredGates = "AzureDevOps.ReleaseManagement.HonorIsGeneratedBitForIgnoredGates";
    public const string InjectCheckAsGateForAgentQueues = "AzureDevOps.ReleaseManagement.InjectCheckAsGate.AgentQueue";
    public const string InjectCheckAsGateForServiceEndpoints = "AzureDevOps.ReleaseManagement.InjectCheckAsGate.ServiceEndpoint";
    public const string UsePipelineOrchestratorForDeployStep = "VisualStudio.ReleaseManagement.UsePipelineOrchestrator.DeployStep";
    public const string UsePipelineOrchestratorForPhase = "VisualStudio.ReleaseManagement.UsePipelineOrchestrator.Phase";
    public const string PurgeInvisibleEndpoints = "VisualStudio.ReleaseManagement.PipelineOrchestrator.PurgeInvisibleEndpoints";
    public const string ValidateTaskInputs = "VisualStudio.ReleaseManagement.PipelineOrchestrator.ValidateTaskInputs";
    public const string SkipResourceValidation = "VisualStudio.ReleaseManagement.PipelineOrchestrator.SkipResourceValidation";
    public const string ExcludePhasesInListReleasesPayload = "VisualStudio.ReleaseManagement.ListReleases.ExcludePhasesInPayload";
    public const string DisableReleaseIdCustomClaim = "VisualStudio.ReleaseManagement.PipelineOrchestrator.DisableReleaseIdCustomClaim";
    public const string YamlPreview = "VisualStudio.ReleaseManagement.Yaml.Preview";
    public const string YamlTemplates = "VisualStudio.ReleaseManagemenmt.Yaml.Templates";
    public const string YamlMustache = "VisualStudio.ReleaseManagemenmt.Yaml.Mustache";
  }
}
