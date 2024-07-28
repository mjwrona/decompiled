// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskFeatureFlags
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class DistributedTaskFeatureFlags
  {
    public const string AgentCloudsUI = "DistributedTask.AgentClouds.UI";
    public const string BatchPoolIdentityRequests = "DistributedTask.BatchPoolIdentityRequests";
    public const string BuildAndReleaseResourceLimits = "WebAccess.BuildAndRelease.ResourceLimits";
    public const string DeprecateLegacyAgent = "DistributedTask.DeprecateLegacyAgent";
    public const string Environments = "DistributedTask.Environments";
    public const string EnvironmentVirtualMachineResource = "DistributedTask.Environments.EnvironmentVirtualMachineResource";
    public const string IgnoreAgentChangeEvents = "DistributedTask.IgnoreAgentChangeEvents";
    public const string LimitHostedMaxParallelism = "DistributedTask.LimitHostedMaxParallelism";
    public const string MarkRequestsAllMatch = "DistributedTask.MarkRequestsAllMatch";
    public const string PipelineBillingModel2 = "DistributedTask.PipelineBillingModel2";
    public const string PipelineBillingModel2ForPublicProjects = "DistributedTask.PipelineBillingModel2.PublicProjects";
    public const string PipelineBillingModel2SelfHostedInfiniteResourceLimits = "DistributedTask.PipelineBillingModel2.SelfHosted.InfiniteResourceLimits";
    public const string PreserveOriginalTaskDataWhenPossible = "DistributedTask.PreserveOriginalTaskDataWhenPossible";
    public const string RemoveHostedPoolAccess = "DistributedTask.RemoveHostedPoolAccess";
    public const string EnableSecretScanning = "DistributedTask.EnableSecretScanning";
    public const string ServiceEndpointInlineSPNDescriptor = "DistributedTask.TaskEditor.ServiceEndpointInlineSPNDescriptor";
    public const string ShareVariableGroups = "WebAccess.DistributedTask.ShareVariableGroups";
    public const string TaskGroupVersionEdit = "DistributedTask.EnableTaskGroupVersionEdit";
    public const string VariableGroupCollectionApis = "WebAccess.DistributedTask.VariableGroupCollectionApis";
    public const string AssignmentJobNoTimeout = "DistributedTask.AssignmentJobNoTimeout";
    public const string EnvironmentOrchestrationTrace = "DistributedTask.Environment.Orchestration.Trace";
    public const string EnableEnvironmentOwnedServiceConnections = "Environments.EnableEnvironmentOwnedServiceConnections";
    public const string EnableLifeCycleHooks = "DistributedTask.LifeCycleHooks";
    public const string ManageTagsFeatureFlag = "Pipelines.Environments.Resources.ManageTags";
    public const string SupportPoolDemandFeatureFlag = "Pipelines.Environments.SupportPoolDemand";
    public const string DownloadArtifactInDeployHook = "Pipelines.Environments.DownloadArtifactInDeployHook";
    public const string EnableNewOutputVariableFormat = "Pipelines.Environments.EnableNewOutputVariableFormat";
    public const string EnableDecoratorForDeploymentJob = "Pipelines.Environments.EnableDecoratorForDeploymentJob";
    public const string ElasticPoolReimageFailureHandling = "DistributedTask.ElasticPoolReimageFailureHandling";
    public const string GetExtensionsFromCDN = "DistributedTask.GetExtensionsFromCDN";
    public const string DisableEnvironmentAutocreationWithSystemContext = "Pipelines.Environments.DisableEnvironmentAutocreationWithSystemContext";
    public const string ReturnTaskAgentPoolObjDuringQueueCreation = "DistributedTask.ReturnTaskAgentPoolObjDuringQueueCreation";
    public const string UseInternalCloudAgentDefinitionNewSearch = "DistributedTask.UseInternalCloudAgentDefinitionNewSearch";
    public const string DisableMMSInlineRedirect = "DistributedTask.DisableMMSInlineRedirect";
    public const string EnforceInternalAgentSpecifcation = "DistributedTask.EnforceInternalAgentSpecifcation";
    public const string PrioritizeElasticPoolOperations = "DistributedTask.PrioritizeElasticPoolOperations";
    public const string ElasticPoolCreateTimeout = "DistributedTask.ElasticPoolCreateTimeout";
    public const string EnableMMSCrossRealmTokenCaching = "DistributedTask.EnableMMSCrossRealmTokenCaching";
    public const string DontPrioritizeReimageWhenDeleteCountIsHigh = "DistributedTask.DontPrioritizeReimageWhenDeleteCountIsHigh";
    public const string TraceAgentAssignmentResults = "DistributedTask.TraceAgentAssignmentResults";
    public const string AgentCloudTimeoutJobDetectionMode = "DistributedTask.AgentCloudTimeoutJobDetectionMode";
    public const string AgentCloudTimeoutJobCorrectionMode = "DistributedTask.AgentCloudTimeoutJobCorrectionMode";
    public const string UseAssignAgentRequestsV2Sproc = "DistributedTask.UseAssignAgentRequestsV2Sproc";
    public const string FixedUnassignableAgentRequestCleanup = "DistributedTask.FixedUnassignableAgentRequestCleanup";
    public const string AgentPoolTelemetry = "DistributedTask.AgentPoolTelemetry";
    public const string AgentCloudCacheInvalidation = "DistributedTask.AgentCloudCacheInvalidation";
    public const string IncludeJobInNonMmsAgentCloudAcquireRequests = "DistributedTask.IncludeJobInNonMmsAgentCloudAcquireRequests";
    public const string IncludeDemandsInAgentCloudAcquireRequests = "DistributedTask.IncludeDemandsInAgentCloudAcquireRequests";
    public const string UseAssignAgentRequestsV3Sproc = "DistributedTask.UseAssignAgentRequestsV3Sproc";
    public const string AadAuthFor1ES = "DistributedTask.AadAuthFor1ES";
    public const string ElasticPoolTurnOnAzureClientTracing = "DistributedTask.ElasticPoolTurnOnAzureClientTracing";
    public const string EnableTier3AssignmentToGitHubForkedRepos = "DistributedTask.EnableTier3AssignmentToGitHubForkedRepos";
    public const string ElasticPoolFlexibleOrchestration = "DistributedTask.ElasticPoolFlexibleOrchestration";
    public const string ElasticPoolFlexibleOrchestrationReimageBatch = "DistributedTask.ElasticPoolFlexibleOrchestrationReimageBatch";
    public const string ElasticPoolFlexibleOrchestrationOperationId = "DistributedTask.ElasticPoolFlexibleOrchestrationOperationId";
    public const string UseQueueResourceLockRequestsV2Sproc = "DistributedTask.UseQueueResourceLockRequestsV2Sproc";
    public const string ElasticPoolDeleteStuckVM = "DistributedTask.ElasticPoolDeleteStuckVM";
    public const string ElasticPoolRetryDelete = "DistributedTask.ElasticPoolRetryDelete";
    public const string ElasticPoolForceDelete = "DistributedTask.ElasticPoolForceDelete";
    public const string MoveNodeToDeletingCompute = "DistributedTask.MoveNodeToDeletingCompute";
    public const string SkipActionsForNotExistingCompute = "DistributedTask.SkipActionsForNotExistingCompute";
    public const string ElasticPoolFlexibleOrchestrationRefreshVMs = "DistributedTask.ElasticPoolFlexibleOrchestrationRefreshVMs";
    public const string ElasticPoolReimageVmForOfflineAgent = "DistributedTask.ElasticPoolReimageVmForOfflineAgent";
    public const string ElasticPoolIdleNodeWithoutAgent = "DistributedTask.ElasticPoolIdleNodeWithoutAgent";
    public const string ElasticPoolLockAzureClients = "DistributedTask.ElasticPoolLockAzureClients";
    public const string ElasticPoolDeleteUnhealthyVms = "DistributedTask.ElasticPoolDeleteUnhealthyVms";
    public const string AgentCapabilityUpdateRequestsBasedOnDemands = "DistributedTask.AgentCapabilityUpdateRequestsBasedOnDemands";
    public const string ElasticPoolDisableRecycleForNonEphemeral = "DistributedTask.ElasticPoolDisableRecycleForNonEphemeral";
    public const string ElasticPoolUpdateCapacityToComputeSizeAfterRecreation = "DistributedTask.ElasticPoolUpdateCapacityToComputeSizeAfterRecreation";
    public const string UseAssignAgentRequestsSprocTimeout = "DistributedTask.UseAssignAgentRequestsSprocTimeout";
    public const string EnableGrantOrgLevelAccessPermissionToAllPipelinesInAgentPools = "DistributedTask.EnableGrantOrgLevelAccessPermissionToAllPipelinesInAgentPools";
    public const string SeparateThresholdsRegistryPerCloudTypeInAgentTimeoutCloudJob = "DistributedTask.SeparateThresholdsRegistryPerCloudTypeInAgentTimeoutCloudJob";
    public const string LogDeprovisionAgentJobResult = "DistributedTask.LogDeprovisionAgentJobResult";
    public const string ShardActivityDispatcherByPoolProvider = "DistributedTask.ShardActivityDispatcherByPoolProvider";
    public const string ElasticPoolValidateAzureHttpClientApiVersionQueryParam = "DistributedTask.ElasticPoolValidateAzureHttpClientApiVersionQueryParam";
    public const string ValidatePoolIdAndAgentIdOnAgentDelete = "DistributedTask.ValidatePoolIdAndAgentIdOnAgentDelete";
  }
}
