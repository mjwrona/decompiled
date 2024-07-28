// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.RegistryKeys
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RegistryKeys
  {
    public const string RegistrySettingsPath = "/Service/Build/Settings/";
    public const string ElasticQueueRoot = "/Service/Build/Settings/ElasticQueue/";
    public const string MachinePoolRoot = "/Service/Build/Settings/MachinePool/";
    public const string AlertsForPool = "Alerts/";
    public const string QueueConnectionString = "/Service/Build/Settings/ElasticQueue/ConnectionString";
    public const string SubscriptionId = "/Service/Build/Settings/ElasticQueue/SubscriptionId";
    public const string ManagementCertificate = "/Service/Build/Settings/ElasticQueue/ManagementCert";
    public const string MachinePoolProperties = "Properties";
    public const string MachinePoolHostedServiceName = "HostedServiceName";
    public const string MachinePoolPoolAuthToken = "PoolAuthToken";
    public const string MachinePoolDeployments = "Deployments";
    public const string MachinePoolVMImages = "VMImages";
    public const string DeleteBuildsBatchSize = "/Service/Build/Settings/DeleteBuildsBatchSize";
    public const string BuildRetentionPolicyLastEvaluated = "/Service/Build/Settings/BuildRetentionPolicyLastEvaluated";
    public const string QueuePausedBuildsBatchSize = "/Service/Build/Settings/QueuePausedBuildsBatchSize";
    public const string CancelOrphanedBuildsInQueueBatchSize = "/Service/Build/Settings/CancelOrphanedBuildsInQueueBatchSize";
    public const string BuildDeletionBatchSizeForJob = "/Service/Build/Settings/DeleteBuildsBatchSizeForJob";
    public const string EventPublisherJobDelay = "/Service/Build/Settings/EventPublisherJobDelay";
    public const string BuildEventListenerTimeWarning = "/Service/Build/Settings/BuildEventListenerTimeWarning";
    public const string BuildEventListenerConcurrency = "/Service/Build/Settings/BuildEventListenerConcurrency";
    public const string LastTimeChecked = "Alerts/LastTimeChecked";
    public const string LastHourChecked = "Alerts/LastHourlyCheck";
    public const string BuildsWaitingAlertThreshold = "Alerts/BuildsWaitingAlertThreshold";
    public const string QueueViolationWeight = "Alerts/QueueViolationWeight";
    public const string OnlineMachineRatioGood = "Alerts/OnlineMachineRatioWarning";
    public const string OnlineMachineRatioWarning = "Alerts/OnlineMachineRatioError";
    public const string ReimageDurationAlertThreshold = "Alerts/ReimageDurationAlertThreshold";
    public const string ReimageDurationDelayThreshold = "Alerts/ReimageDurationDelayThreshold";
    public const string AvailabilityGood = "Alerts/AvailabilityGoodMinimumThreshold";
    public const string AvailabilityWarning = "Alerts/AvailabilityWarningMinimumThreshold";
    public const string HealthDataTimeSkew = "Alerts/HealthDataTimeSkew";
    public const string SlowCommandThresholdPath = "/Service/Build/Settings/SlowCommandThresholdsInMs/";
    public const string GetBuildsLegacyThreshold = "/Service/Build/Settings/SlowCommandThresholdsInMs/GetBuildsLegacy";
    public const string FilterBuildsThreshold = "/Service/Build/Settings/SlowCommandThresholdsInMs/FilterBuilds";
    public const string GetBuildsByIdsThreshold = "/Service/Build/Settings/SlowCommandThresholdsInMs/GetBuildsByIds";
    public const string BuildJobCancellingTimeout = "/Service/Build/Settings/BuildJobCancellingTimeout";
    public const string RetentionPolicyPath = "/Service/Build/Settings/Retention/";
    public const string RetentionCurrentProjectToProcess = "/Service/Build/Settings/Retention/CurrentProjectToProcess";
    public const string RetentionNextDefinitionToProcess = "/Service/Build/Settings/Retention/NextDefinitionToProcess";
    public const string RetentionLastFinishTimeProcessed = "/Service/Build/Settings/Retention/LastFinishTimeProcessed";
    public const string RetentionMaxBuildBatchSize = "/Service/Build/Settings/Retention/MaxBuildBatchSize";
    public const string RetentionMaxDefinitionBatchSize = "/Service/Build/Settings/Retention/MaxDefinitionBatchSize";
    public const string RetentionMaxFinishDate = "/Service/Build/Settings/Retention/MaxFinishDate";
    public const string RetentionPolicyDefaultPolicyPath = "/Service/Build/Settings/Retention/DefaultPolicy/";
    public const string RetentionPolicyDefaultDaysToKeep = "/Service/Build/Settings/Retention/DefaultPolicy/DaysToKeep";
    public const string RetentionPolicyDefaultMinimumToKeep = "/Service/Build/Settings/Retention/DefaultPolicy/MinimumToKeep";
    public const string RetentionPolicyMaximumPolicyPath = "/Service/Build/Settings/Retention/MaximumPolicy/";
    public const string RetentionPolicyMaximumDaysToKeep = "/Service/Build/Settings/Retention/MaximumPolicy/DaysToKeep";
    public const string RetentionPolicyMaximumMinimumToKeep = "/Service/Build/Settings/Retention/MaximumPolicy/MinimumToKeep";
    public const string RetentionDaysToKeepDeletedBuildsBeforeDestroy = "/Service/Build/Settings/Retention/DaysBeforeDestroy";
    public const string RetentionMaxDestroyBatchSize = "/Service/Build/Settings/Retention/MaxDestroyBatchSize";
    public const string RetentionIgnoreDefinitions = "/Service/Build/Settings/Retention/IgnoreDefinitions";
    public const string RetentionDefinitionsWithDefaultCount = "/Service/Build/Settings/Retention/RetentionDefinitionsWithDefaultPolicyCount";
    public const string MetricsPath = "/Service/Build/Settings/Metrics/";
    public const string DaysToKeep = "DaysToKeep";
    public const string OrgSettingsPath = "/Service/Build/Settings/OrgSettings/";
    public const string StatusBadgesArePublic = "/Service/Build/Settings/OrgSettings/StatusBadgesArePublic";
    public const string EnforceSettableVar = "/Service/Build/Settings/OrgSettings/EnforceSettableVar";
    public const string AuditEnforceSettableVar = "/Service/Build/Settings/OrgSettings/AuditEnforceSettableVar";
    public const string EnforceJobAuthScope = "/Service/Build/Settings/OrgSettings/EnforceJobAuthScope";
    public const string EnforceJobAuthScopeForReleases = "/Service/Build/Settings/OrgSettings/EnforceJobAuthScopeForReleases";
    public const string EnforceReferencedRepoScopedToken = "/Service/Build/Settings/OrgSettings/EnforceReferencedRepoScopedToken";
    public const string DisableStageChooser = "/Service/Build/Settings/OrgSettings/DisableStageChooser";
    public const string DisableClassicPipelineCreation = "/Service/Build/Settings/OrgSettings/DisableClassicPipelineCreation";
    public const string DisableClassicBuildPipelineCreation = "/Service/Build/Settings/OrgSettings/DisableClassicBuildPipelineCreation";
    public const string DisableClassicReleasePipelineCreation = "/Service/Build/Settings/OrgSettings/DisableClassicReleasePipelineCreation";
    public const string DisableImpliedYAMLCiTrigger = "/Service/Build/Settings/OrgSettings/DisableImpliedYAMLCiTrigger";
    public const string ForkProtectionEnabled = "/Service/Build/Settings/OrgSettings/ForkProtectionEnabled";
    public const string BuildsEnabledForForks = "/Service/Build/Settings/OrgSettings/BuildsEnabledForForks";
    public const string EnforceJobAuthScopeForForks = "/Service/Build/Settings/OrgSettings/EnforceJobAuthScopeForForks";
    public const string EnforceNoAccessToSecretsFromForks = "/Service/Build/Settings/OrgSettings/EnforceNoAccessToSecretsFromForks";
    public const string IsCommentRequiredForPullRequest = "/Service/Build/Settings/OrgSettings/IsCommentRequiredForPullRequest";
    public const string RequireCommentsForNonTeamMembersOnly = "/Service/Build/Settings/OrgSettings/RequireCommentsForNonTeamMembersOnly";
    public const string RequireCommentsForNonTeamMemberAndNonContributors = "/Service/Build/Settings/OrgSettings/RequireCommentsForNonTeamMemberAndNonContributors";
    public const string DefinitionMetricsPath = "/Service/Build/Settings/Metrics/Definition/";
    public const string DaysToKeepDefinitionMetrics = "/Service/Build/Settings/Metrics/Definition/DaysToKeep";
    public const string ProjectMetricsPath = "/Service/Build/Settings/Metrics/Project/";
    public const string DaysToKeepDailyProjectMetrics = "/Service/Build/Settings/Metrics/Project/Daily/DaysToKeep";
    public const string DaysToKeepHourlyProjectMetrics = "/Service/Build/Settings/Metrics/Project/Hourly/DaysToKeep";
    public const string LastAggregatedTime = "/Service/Build/Settings/Metrics/Project/LastAggregatedTime";
    public const string PollingPolicyPath = "/Service/Build/Settings/Polling/";
    public const string PollingTimeoutInSeconds = "/Service/Build/Settings/Polling/TimeoutInSeconds";
    public const string LeaseRenewalTimeout = "/Service/Build/Settings/Lease/RenewalTimeout";
    public const string AzureServiceManagementUrl = "/Configuration/Azure/ServiceManagementUrl";
    public const string ChangesetQueryPageSize = "/Service/Build/Settings/ChangesetPageSize";
    public const string EventsPath = "/Service/Build/Settings/Events/";
    public const string FiredDaysToKeep = "FiredDaysToKeep";
    public const string FailedDaysToKeep = "FailedDaysToKeep";
    public const string BatchSize = "BatchSize";
    public const string JobTimeout = "JobTimeout";
    public const string CheckEventsFiredDaysToKeep = "/Service/Build/Settings/CheckEvents/FiredDaysToKeep";
    public const string CheckEventsFailedDaysToKeep = "/Service/Build/Settings/CheckEvents/FailedDaysToKeep";
    public const string CheckEventsBatchSize = "/Service/Build/Settings/CheckEvents/BatchSize";
    public const string CheckEventsMaxAttempts = "/Service/Build/Settings/CheckEvents/MaxAttempts";
    public const string CheckEventsBatchSizeExecute = "/Service/Build/Settings/CheckEvents/BatchSizeExecute";
    public const string CheckEventsMaxDegreeOfParallelism = "/Service/Build/Settings/CheckEvents/MaxDegreeOfParallelism";
    public const string RepositoryAnalysisPath = "/Service/Build/Settings/RepositoryAnalysis/";
    public const string MaxNodeCount = "/Service/Build/Settings/RepositoryAnalysis/MaxNodeCount";
    public const string OrphanedBuildCleanupLookbackMaxTimeInHours = "/Service/Build/Settings/OrphanedBuildCleanupLookbackMaxTimeInHours";
    public const string OrphanedBuildCleanupLookbackMinTimeInDays = "/Service/Build/Settings/OrphanedBuildCleanupLookbackMinTimeInDays";
    public const string OrphanedBuildCleanupCancelOrphans = "/Service/Build/Settings/OrphanedBuildCleanupCancelOrphans";
    public const string BuildSchedulesMaxThreshold = "/Service/Build/Settings/BuildSchedulesMaxThreshold";
    public const string BuildSchedulesMaxLookback = "/Service/Build/Settings/BuildSchedulesMaxLookback";
    public const string BuildSchedulesDelayRangeSec = "/Service/Build/Settings/BuildSchedulesDelayRangeSec";
    public const string BuildSchedulesMaxRetries = "/Service/Build/Settings/BuildSchedulesMaxRetries";
    public const string BuildSchedulesRequeueIntervalMin = "/Service/Build/Settings/BuildSchedulesRequeueIntervalMin";
    public const string CronSchedulesPerJobThreshold = "/Service/Build/Settings/CronSchedulesPerJobThreshold";
    public const string NScheduledRunsToDisplay = "/Service/Build/Settings/NScheduledRunsToDisplay";
    public const string NJobsFromLimit = "/Service/Build/Settings/NJobsFromLimit";
    public const string PoisonedBuildsCleanupBatch = "/Service/Build/Settings/PoisonedBuildsCleanupBatch";
    public const string PoisonedBuildsContinueInSec = "/Service/Build/Settings/PoisonedBuildsContinueCleanupInSec";
    public const string PoisonedBuildsJobRerunInSec = "/Service/Build/Settings/PoisonedBuildsContinueRerunInSec";
    public const string MaxItemsBetweenBuildsForWorkItems = "/Service/Build/Settings/MaxItemsBetweenBuildsForWorkItems";
    public const string MaxNumberPipelinesForCommentTrigger = "/Service/Build/Settings/MaxNumberPipelinesForCommentTrigger";
    public const string MaxBuildsCount = "/Service/Build/Settings/MaxBuildsCount";
    public const string MachinePoolDescription = "Description";
    public const string MachinePoolIsDefault = "IsDefault";
    public const string MachinePoolVMImageName = "VMImageName";
    public const string MachinePoolVMSize = "VMSize";
    public const string MachinePoolMachineCount = "MachineCount";
    public const string MachinePoolControllerName = "ControllerName";
    public const string MachinePoolServiceUserName = "ServiceUserName";
    public const string MachinePoolServicePassword = "ServicePassword";
    public static readonly string EnableShellTasksArgsSanitizing = "/Service/DistributedTask/Settings/EnableShellTasksArgsSanitizing2";
    public static readonly string EnableShellTasksArgsSanitizingAudit = "/Service/DistributedTask/Settings/EnableShellTasksArgsSanitizingAudit";
  }
}
