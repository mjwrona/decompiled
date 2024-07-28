// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildVariables
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [GenerateAllConstants(null)]
  public static class BuildVariables
  {
    public const string CollectionId = "system.collectionId";
    public const string DefinitionId = "system.definitionId";
    public const string HostType = "system.hosttype";
    public const string IsFork = "system.pullRequest.isFork";
    public const string ForkSecretsRemoved = "system.pullRequest.forkSecretsRemoved";
    public const string PullRequestId = "system.pullRequest.pullRequestId";
    public const string PullRequestNumber = "system.pullRequest.pullRequestNumber";
    public const string PullRequestIterationId = "system.pullRequest.pullRequestIteration";
    public const string PullRequestSourceBranch = "system.pullRequest.sourceBranch";
    public const string PullRequestTargetBranch = "system.pullRequest.targetBranch";
    public const string PullRequestTargetBranchName = "system.pullRequest.targetBranchName";
    public const string PullRequestSourceRepositoryUri = "system.pullRequest.sourceRepositoryUri";
    public const string PullRequestSourceCommitId = "system.pullRequest.sourceCommitId";
    public const string PullRequestMergedAt = "system.pullRequest.mergedAt";
    public const string System = "system";
    public const string TeamProject = "system.teamProject";
    public const string TeamProjectId = "system.teamProjectId";
    public const string IsTriggeringRepository = "system.istriggeringrepository";
    public const string SystemDebug = "system.debug";
    public const string AgentDiagnostic = "agent.diagnostic";
    public const string BuildId = "build.buildId";
    public const string BuildNumber = "build.buildNumber";
    public const string BuildUri = "build.buildUri";
    public const string ContainerId = "build.containerId";
    public const string CronDisplayName = "build.cronSchedule.displayName";
    public const string DefinitionName = "build.definitionName";
    public const string DefinitionVersion = "build.definitionVersion";
    public const string JobAuthorizeAs = "Job.AuthorizeAs";
    public const string JobAuthorizeAsId = "Job.AuthorizeAsId";
    public const string QueuedBy = "build.queuedBy";
    public const string QueuedById = "build.queuedById";
    public const string Reason = "build.reason";
    public const string RepoUri = "build.repository.uri";
    public const string RepositoryId = "build.repository.id";
    public const string RepositoryName = "build.repository.name";
    public const string RepositoryUri = "build.repository.uri";
    public const string RequestedFor = "build.requestedFor";
    public const string RequestedForEmail = "build.requestedForEmail";
    public const string RequestedForId = "build.requestedForId";
    public const string SourceBranch = "build.sourceBranch";
    public const string SourceBranchName = "build.sourceBranchName";
    public const string SourceTfvcShelveset = "build.sourceTfvcShelveset";
    public const string SourceVersion = "build.sourceVersion";
    public const string SourceVersionAuthor = "build.sourceVersionAuthor";
    public const string SourceVersionMessage = "build.sourceVersionMessage";
    public const string SyncSources = "build.syncSources";
    public const string DefinitionFolderPath = "build.definitionFolderPath";
    public const string AgentContinueAfterCancelProcessTreeKillAttempt = "VSTSAGENT_CONTINUE_AFTER_CANCEL_PROCESSTREEKILL_ATTEMPT";
    public const string AgentDockerActionRetries = "VSTSAGENT_DOCKER_ACTION_RETRIES";
    public const string AgentUseMsalLibrary = "USE_MSAL";
    public const string AgentEnableNodeWarnings = "VSTSAGENT_ENABLE_NODE_WARNINGS";
    public const string AgentMajorUpgradeDisabled = "AZP_AGENT_MAJOR_UPGRADE_DISABLED";
    public const string AgentFailOnIncompatibleOS = "AGENT_FAIL_ON_INCOMPATIBLE_OS";
    public const string EnableFCSItemPathFix = "ENABLE_FCS_ITEM_PATH_FIX";
    public const string AgentDisableDrainQueuesAfterTask = "AGENT_DISABLE_DRAIN_QUEUES_AFTER_TASK";
    public const string EnableBuildArtifactsPlusSignWorkaround = "AZP_TASK_FF_ENABLE_BUILDARTIFACTS_PLUS_SIGN_WORKAROUND";
    public const string MSRC75787EnableSecureArgs = "AZP_75787_ENABLE_NEW_LOGIC";
    public const string MSRC75787EnableSecureArgsAudit = "AZP_75787_ENABLE_NEW_LOGIC_LOG";
    public const string MSRC75787EnableTelemetry = "AZP_75787_ENABLE_COLLECT";
    public const string AgentIgnoreVSTSTaskLib = "AZP_AGENT_IGNORE_VSTSTASKLIB";
    public static readonly string AgentEnablePipelineArtifactLargeChunkSize = "AGENT_ENABLE_PIPELINEARTIFACT_LARGE_CHUNK_SIZE";
    public const string FailJobWhenAgentDies = "FAIL_JOB_WHEN_AGENT_DIES";
    public const string AgentUseMsdeployTokenAuth = "USE_MSDEPLOY_TOKEN_AUTH";
    public const string FixPossibleGitOutOfMemoryProblem = "FIX_POSSIBLE_GIT_OUT_OF_MEMORY_PROBLEM";
    public const string EnableNewPowerShellInvokeProcessCmdlet = "AZP_PS_ENABLE_INVOKE_PROCESS";
    public const string MSRC75787EnableNewAgentNewProcessHandlerSanitizer = "AZP_75787_ENABLE_NEW_PH_LOGIC";
    public const string CheckForTaskDeprecation = "AZP_AGENT_CHECK_FOR_TASK_DEPRECATION";
    public const string MountWorkspace = "AZP_AGENT_MOUNT_WORKSPACE";
    public const string AgentDockerInitOption = "AZP_AGENT_DOCKER_INIT_OPTION";
    public const string UseMaskingPerformanceEnhancements = "agent.agentUseMaskingPerformanceEnhancements";
    public const string HighTaskFailRateDetected = "HIGH_TASK_FAIL_RATE_DETECTED";
    public const string FailDeprecatedTask = "FAIL_DEPRECATED_TASK";
    public const string RetireAzureRMPowerShellModule = "RETIRE_AZURERM_POWERSHELL_MODULE";
    public const string FailDeprecatedBuildTask = "FAIL_DEPRECATED_BUILD_TASK";
    public const string LogTaskNameInUserAgent = "AZP_AGENT_LOG_TASKNAME_IN_USERAGENT";
    public static readonly List<string> ReadOnlySystemVariables = new List<string>()
    {
      "system.collectionId",
      "system.definitionId",
      "system.hosttype",
      "system.pullRequest.isFork",
      "system.pullRequest.forkSecretsRemoved",
      "system.pullRequest.pullRequestId",
      "system.pullRequest.pullRequestNumber",
      "system.pullRequest.pullRequestIteration",
      "system.pullRequest.sourceBranch",
      "system.pullRequest.targetBranch",
      "system.pullRequest.targetBranchName",
      "system.pullRequest.sourceRepositoryUri",
      "system.pullRequest.sourceCommitId",
      "system.pullRequest.mergedAt",
      "system.teamProject",
      "system.teamProjectId",
      "build.buildId",
      "build.buildNumber",
      "build.buildUri",
      "build.containerId",
      "build.cronSchedule.displayName",
      "build.definitionName",
      "build.definitionVersion",
      "system.istriggeringrepository",
      "Job.AuthorizeAs",
      "Job.AuthorizeAsId",
      "build.queuedBy",
      "build.queuedById",
      "build.reason",
      "build.repository.uri",
      "build.repository.id",
      "build.repository.name",
      "build.repository.uri",
      "build.requestedFor",
      "build.requestedForEmail",
      "build.requestedForId",
      "build.sourceBranch",
      "build.sourceBranchName",
      "build.sourceTfvcShelveset",
      "build.sourceVersion",
      "build.sourceVersionAuthor",
      "build.sourceVersionMessage",
      "build.syncSources",
      "system.debug",
      "agent.diagnostic",
      "build.definitionFolderPath",
      "VSTSAGENT_CONTINUE_AFTER_CANCEL_PROCESSTREEKILL_ATTEMPT",
      "VSTSAGENT_DOCKER_ACTION_RETRIES",
      "USE_MSAL",
      "VSTSAGENT_ENABLE_NODE_WARNINGS",
      "AZP_AGENT_MAJOR_UPGRADE_DISABLED",
      "AGENT_FAIL_ON_INCOMPATIBLE_OS",
      "ENABLE_FCS_ITEM_PATH_FIX",
      "AGENT_DISABLE_DRAIN_QUEUES_AFTER_TASK",
      "AZP_TASK_FF_ENABLE_BUILDARTIFACTS_PLUS_SIGN_WORKAROUND",
      "AZP_75787_ENABLE_NEW_LOGIC",
      "AZP_75787_ENABLE_NEW_LOGIC_LOG",
      "AZP_75787_ENABLE_COLLECT",
      "AZP_AGENT_IGNORE_VSTSTASKLIB",
      BuildVariables.AgentEnablePipelineArtifactLargeChunkSize,
      "FAIL_JOB_WHEN_AGENT_DIES",
      "USE_MSDEPLOY_TOKEN_AUTH",
      "FIX_POSSIBLE_GIT_OUT_OF_MEMORY_PROBLEM",
      "AZP_PS_ENABLE_INVOKE_PROCESS",
      "AZP_75787_ENABLE_NEW_PH_LOGIC",
      "AZP_AGENT_CHECK_FOR_TASK_DEPRECATION",
      "AZP_AGENT_MOUNT_WORKSPACE",
      "AZP_AGENT_DOCKER_INIT_OPTION",
      "agent.agentUseMaskingPerformanceEnhancements",
      "HIGH_TASK_FAIL_RATE_DETECTED",
      "FAIL_DEPRECATED_TASK",
      "RETIRE_AZURERM_POWERSHELL_MODULE",
      "FAIL_DEPRECATED_BUILD_TASK",
      "AZP_AGENT_LOG_TASKNAME_IN_USERAGENT"
    };
  }
}
