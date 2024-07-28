// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.WellKnownDistributedTaskVariables
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class WellKnownDistributedTaskVariables
  {
    public static readonly string AccessToken = "system.accessToken";
    public static readonly string AccessTokenScope = "system.connection.accessTokenScope";
    public static readonly string AzureUserAgent = "AZURE_HTTP_USER_AGENT";
    public static readonly string BuildId = "build.buildId";
    public static readonly string CollectionId = "system.collectionId";
    public static readonly string CollectionUrl = "system.collectionUri";
    public static readonly string Culture = "system.culture";
    public static readonly string DefinitionId = "system.definitionId";
    public static readonly string DefinitionName = "system.definitionName";
    public static readonly string EnableAccessToken = "system.enableAccessToken";
    public static readonly string HostType = "system.hosttype";
    public static readonly string HubVersion = "system.hubversion";
    public static readonly string IsScheduled = "system.isScheduled";
    public static readonly string JobAttempt = "system.jobAttempt";
    public static readonly string JobDisplayName = "system.jobDisplayName";
    public static readonly string JobId = "system.jobId";
    public static readonly string JobIdentifier = "system.jobIdentifier";
    public static readonly string JobName = "system.jobName";
    public static readonly string JobParallelismTag = "system.jobParallelismTag";
    public static readonly string JobPositionInPhase = "System.JobPositionInPhase";
    public static readonly string JobStatus = "system.jobStatus";
    public static readonly string JobTimeout = "system.jobTimeout";
    public static readonly string LogToBlobstorageService = "agent.LogToBlobstorageService";
    public static readonly string MsDeployUserAgent = "MSDEPLOY_HTTP_USER_AGENT";
    public static readonly string ParallelExecutionType = "System.ParallelExecutionType";
    public static readonly string PhaseAttempt = "system.phaseAttempt";
    public static readonly string PhaseDisplayName = "system.phaseDisplayName";
    public static readonly string PhaseId = "system.phaseId";
    public static readonly string PhaseName = "system.phaseName";
    public static readonly string PipelineStartTime = "system.pipelineStartTime";
    public static readonly string PlanId = "system.planId";
    public static readonly string PostLinesSpeed = "system.postLinesSpeed";
    public static readonly string ReadOnlyVariables = "agent.readOnlyVariables";
    public static readonly string RestrictSecrets = "system.restrictSecrets";
    public static readonly string RetainDefaultEncoding = "agent.retainDefaultEncoding";
    public static readonly string ServerType = "system.servertype";
    public static readonly string StageAttempt = "system.stageAttempt";
    public static readonly string StageDisplayName = "system.stageDisplayName";
    public static readonly string StageId = "system.stageId";
    public static readonly string StageName = "system.stageName";
    public static readonly string PreferRedirectGetContainerItem = "system.preferRedirectGetContainerItem";
    public static readonly string System = "system";
    public static readonly string TFCollectionUrl = "system.teamFoundationCollectionUri";
    public static readonly string TaskRestrictionEnforcementMode = "agent.taskRestrictionsEnforcementMode";
    public static readonly string TaskDefinitionsUrl = "system.taskDefinitionsUri";
    public static readonly string TaskDisplayName = "system.taskDisplayName";
    public static readonly string TaskInstanceId = "system.taskInstanceId";
    public static readonly string TaskInstanceName = "system.taskInstanceName";
    public static readonly string TeamProject = "system.teamProject";
    public static readonly string TeamProjectId = "system.teamProjectId";
    public static readonly string TimelineId = "system.timelineId";
    public static readonly string TotalJobsInPhase = "System.TotalJobsInPhase";
    public static readonly string UploadTimelineAttachmentsToBlob = "agent.UploadTimelineAttachmentsToBlob";
    public static readonly string UploadBuildArtifactsToBlob = "agent.UploadBuildArtifactsToBlob";
    public static readonly string UseWorkspaceId = "agent.useWorkspaceId";
    public static readonly string ContinueAfterCancelProcessTreeKillAttempt = "VSTSAGENT_CONTINUE_AFTER_CANCEL_PROCESSTREEKILL_ATTEMPT";
    public static readonly string DockerActionRetries = "VSTSAGENT_DOCKER_ACTION_RETRIES";
    public static readonly string UseMsalLibrary = "USE_MSAL";
    public static readonly string EnableNodeWarnings = "VSTSAGENT_ENABLE_NODE_WARNINGS";
    public static readonly string DisableNode6Tasks = "AGENT_DISABLE_NODE6_TASKS";
    public static readonly string MajorUpgradeDisabled = "AZP_AGENT_MAJOR_UPGRADE_DISABLED";
    public static readonly string DisableDrainQueuesAfterTask = "AGENT_DISABLE_DRAIN_QUEUES_AFTER_TASK";
    public static readonly string AgentFailOnIncompatibleOS = "AGENT_FAIL_ON_INCOMPATIBLE_OS";
    public static readonly string EnableFCSItemPathFix = "ENABLE_FCS_ITEM_PATH_FIX";
    public static readonly string EnableBuildArtifactsPlusSignWorkaround = "AZP_TASK_FF_ENABLE_BUILDARTIFACTS_PLUS_SIGN_WORKAROUND";
    public static readonly string MSRC75787EnableSecureArgs = "AZP_75787_ENABLE_NEW_LOGIC";
    public static readonly string MSRC75787EnableSecureArgsAudit = "AZP_75787_ENABLE_NEW_LOGIC_LOG";
    public static readonly string MSRC75787EnableTelemetry = "AZP_75787_ENABLE_COLLECT";
    public static readonly string AgentEnablePipelineArtifactLargeChunkSize = "AGENT_ENABLE_PIPELINEARTIFACT_LARGE_CHUNK_SIZE";
    public static readonly string FailJobWhenAgentDies = "FAIL_JOB_WHEN_AGENT_DIES";
    public static readonly string AgentIgnoreVSTSTaskLib = "AZP_AGENT_IGNORE_VSTSTASKLIB";
    public static readonly string UseMsdeployTokenAuth = "USE_MSDEPLOY_TOKEN_AUTH";
    public static readonly string FixPossibleGitOutOfMemoryProblem = "FIX_POSSIBLE_GIT_OUT_OF_MEMORY_PROBLEM";
    public static readonly string EnableNewPowerShellInvokeProcessCmdlet = "AZP_PS_ENABLE_INVOKE_PROCESS";
    public static readonly string MSRC75787EnableNewAgentNewProcessHandlerSanitizer = "AZP_75787_ENABLE_NEW_PH_LOGIC";
    public static readonly string CheckForTaskDeprecation = "AZP_AGENT_CHECK_FOR_TASK_DEPRECATION";
    public static readonly string MountWorkspace = "AZP_AGENT_MOUNT_WORKSPACE";
    public static readonly string AgentDockerInitOption = "AZP_AGENT_DOCKER_INIT_OPTION";
    public static readonly string UseMaskingPerformanceEnhancements = "agent.agentUseMaskingPerformanceEnhancements";
    public static readonly string HighTaskFailRateDetected = "HIGH_TASK_FAIL_RATE_DETECTED";
    public static readonly string FailDeprecatedTask = "FAIL_DEPRECATED_TASK";
    public static readonly string RetireAzureRMPowerShellModule = "RETIRE_AZURERM_POWERSHELL_MODULE";
    public static readonly string FailDeprecatedBuildTask = "FAIL_DEPRECATED_BUILD_TASK";
    public static readonly string LogTaskNameInUserAgent = "AZP_AGENT_LOG_TASKNAME_IN_USERAGENT";
    public static readonly List<string> ReadOnlySystemVariables = new List<string>()
    {
      WellKnownDistributedTaskVariables.AccessToken,
      WellKnownDistributedTaskVariables.AccessTokenScope,
      WellKnownDistributedTaskVariables.BuildId,
      WellKnownDistributedTaskVariables.CollectionId,
      WellKnownDistributedTaskVariables.CollectionUrl,
      WellKnownDistributedTaskVariables.Culture,
      WellKnownDistributedTaskVariables.DefinitionId,
      WellKnownDistributedTaskVariables.DefinitionName,
      WellKnownDistributedTaskVariables.EnableAccessToken,
      WellKnownDistributedTaskVariables.HostType,
      WellKnownDistributedTaskVariables.HubVersion,
      WellKnownDistributedTaskVariables.IsScheduled,
      WellKnownDistributedTaskVariables.JobAttempt,
      WellKnownDistributedTaskVariables.JobDisplayName,
      WellKnownDistributedTaskVariables.JobId,
      WellKnownDistributedTaskVariables.JobIdentifier,
      WellKnownDistributedTaskVariables.JobName,
      WellKnownDistributedTaskVariables.JobParallelismTag,
      WellKnownDistributedTaskVariables.JobPositionInPhase,
      WellKnownDistributedTaskVariables.JobStatus,
      WellKnownDistributedTaskVariables.JobTimeout,
      WellKnownDistributedTaskVariables.LogToBlobstorageService,
      WellKnownDistributedTaskVariables.ParallelExecutionType,
      WellKnownDistributedTaskVariables.PhaseAttempt,
      WellKnownDistributedTaskVariables.PhaseDisplayName,
      WellKnownDistributedTaskVariables.PhaseId,
      WellKnownDistributedTaskVariables.PhaseName,
      WellKnownDistributedTaskVariables.PipelineStartTime,
      WellKnownDistributedTaskVariables.PlanId,
      WellKnownDistributedTaskVariables.PostLinesSpeed,
      WellKnownDistributedTaskVariables.ReadOnlyVariables,
      WellKnownDistributedTaskVariables.RestrictSecrets,
      WellKnownDistributedTaskVariables.RetainDefaultEncoding,
      WellKnownDistributedTaskVariables.ServerType,
      WellKnownDistributedTaskVariables.StageAttempt,
      WellKnownDistributedTaskVariables.StageDisplayName,
      WellKnownDistributedTaskVariables.StageId,
      WellKnownDistributedTaskVariables.StageName,
      WellKnownDistributedTaskVariables.TFCollectionUrl,
      WellKnownDistributedTaskVariables.TaskDefinitionsUrl,
      WellKnownDistributedTaskVariables.TaskDisplayName,
      WellKnownDistributedTaskVariables.TaskInstanceId,
      WellKnownDistributedTaskVariables.TaskInstanceName,
      WellKnownDistributedTaskVariables.TaskRestrictionEnforcementMode,
      WellKnownDistributedTaskVariables.TeamProject,
      WellKnownDistributedTaskVariables.TeamProjectId,
      WellKnownDistributedTaskVariables.TimelineId,
      WellKnownDistributedTaskVariables.TotalJobsInPhase,
      WellKnownDistributedTaskVariables.UploadBuildArtifactsToBlob,
      WellKnownDistributedTaskVariables.UploadTimelineAttachmentsToBlob,
      WellKnownDistributedTaskVariables.UseWorkspaceId,
      WellKnownDistributedTaskVariables.ContinueAfterCancelProcessTreeKillAttempt,
      WellKnownDistributedTaskVariables.DockerActionRetries,
      WellKnownDistributedTaskVariables.UseMsalLibrary,
      WellKnownDistributedTaskVariables.EnableNodeWarnings,
      WellKnownDistributedTaskVariables.DisableNode6Tasks,
      WellKnownDistributedTaskVariables.MajorUpgradeDisabled,
      WellKnownDistributedTaskVariables.AgentFailOnIncompatibleOS,
      WellKnownDistributedTaskVariables.EnableFCSItemPathFix,
      WellKnownDistributedTaskVariables.DisableDrainQueuesAfterTask,
      WellKnownDistributedTaskVariables.EnableBuildArtifactsPlusSignWorkaround,
      WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgs,
      WellKnownDistributedTaskVariables.MSRC75787EnableSecureArgsAudit,
      WellKnownDistributedTaskVariables.MSRC75787EnableTelemetry,
      WellKnownDistributedTaskVariables.AgentEnablePipelineArtifactLargeChunkSize,
      WellKnownDistributedTaskVariables.FailJobWhenAgentDies,
      WellKnownDistributedTaskVariables.AgentIgnoreVSTSTaskLib,
      WellKnownDistributedTaskVariables.PreferRedirectGetContainerItem,
      WellKnownDistributedTaskVariables.UseMsdeployTokenAuth,
      WellKnownDistributedTaskVariables.FixPossibleGitOutOfMemoryProblem,
      WellKnownDistributedTaskVariables.EnableNewPowerShellInvokeProcessCmdlet,
      WellKnownDistributedTaskVariables.MSRC75787EnableNewAgentNewProcessHandlerSanitizer,
      WellKnownDistributedTaskVariables.CheckForTaskDeprecation,
      WellKnownDistributedTaskVariables.MountWorkspace,
      WellKnownDistributedTaskVariables.AgentDockerInitOption,
      WellKnownDistributedTaskVariables.UseMaskingPerformanceEnhancements,
      WellKnownDistributedTaskVariables.HighTaskFailRateDetected,
      WellKnownDistributedTaskVariables.FailDeprecatedTask,
      WellKnownDistributedTaskVariables.RetireAzureRMPowerShellModule,
      WellKnownDistributedTaskVariables.FailDeprecatedBuildTask,
      WellKnownDistributedTaskVariables.LogTaskNameInUserAgent
    };
  }
}
