// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.RegistryKeys
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class RegistryKeys
  {
    public static readonly string RegistrySettingsPath = "/Service/DistributedTask/Settings/";
    public static readonly string YamlRegistrySettingsPath = RegistryKeys.RegistrySettingsPath + "Yaml/";
    public static readonly string OrchestratorRegistrySettingsPath = RegistryKeys.RegistrySettingsPath + "Orchestrator/";
    public static readonly RegistryQuery PipelinePlansPerParallelismMinute = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelinePlansPerParallelismMinute));
    public static readonly RegistryQuery PipelinesPlansPerMinuteMax = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelinesPlansPerMinuteMax));
    public static readonly RegistryQuery PipelineParserMaxFiles = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelineParserMaxFiles));
    public static readonly RegistryQuery PipelineParserMaxResultSize = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelineParserMaxResultSize));
    public static readonly RegistryQuery PipelineRedisCacheExpiryInterval = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelineRedisCacheExpiryInterval));
    public static readonly RegistryQuery PipelineParserMaxFileSize = (RegistryQuery) (RegistryKeys.YamlRegistrySettingsPath + nameof (PipelineParserMaxFileSize));
    public static readonly RegistryQuery PipelinePhaseExpansionLimit = (RegistryQuery) (RegistryKeys.OrchestratorRegistrySettingsPath + nameof (PipelinePhaseExpansionLimit));
    public static readonly RegistryQuery PipelineParserMaxEvents = (RegistryQuery) (RegistryKeys.OrchestratorRegistrySettingsPath + nameof (PipelineParserMaxEvents));
    public static readonly RegistryQuery FeedlinesLimit = (RegistryQuery) (RegistryKeys.OrchestratorRegistrySettingsPath + nameof (FeedlinesLimit));
    public static readonly string SourceProviderRegistrySettingsPath = RegistryKeys.RegistrySettingsPath + "SourceProvider/";
    public static readonly string SubversionSourceProviderRegistrySettingsPath = RegistryKeys.SourceProviderRegistrySettingsPath + "Subversion/";
    public static readonly RegistryQuery SubversionClientTimeout = (RegistryQuery) (RegistryKeys.SubversionSourceProviderRegistrySettingsPath + nameof (SubversionClientTimeout));
    public static readonly RegistryQuery TaskLockDownAllowedAvailabilityState = new RegistryQuery("/FeatureAvailability/Entries/DistributedTask.TaskLockdownAllowed/AvailabilityState");
    public static readonly string DemandsOnSingleHostedPoolBlockedRegistrySettingsPath = RegistryKeys.OrchestratorRegistrySettingsPath + "DemandsOnSingleHostedPoolBlocked";
    public static readonly string MaxReferencedReposRegistrySettingsPath = RegistryKeys.OrchestratorRegistrySettingsPath + "MaxReferencedRepos";
    public static readonly RegistryQuery PipelineParserMaxDepth = (RegistryQuery) (RegistryKeys.OrchestratorRegistrySettingsPath + nameof (PipelineParserMaxDepth));
    public static readonly string TasksRegistrySettingsPath = RegistryKeys.RegistrySettingsPath + "Tasks/";
    public static readonly RegistryQuery EnableTaskIntegrityValidationOnPremise = (RegistryQuery) (RegistryKeys.TasksRegistrySettingsPath + nameof (EnableTaskIntegrityValidationOnPremise));
    public static readonly string TasksVersionOverridePath = RegistryKeys.TasksRegistrySettingsPath + "VersionOverrides";
    public static readonly string TasksBuildConfigExceptionsPath = RegistryKeys.TasksRegistrySettingsPath + "buildConfigExceptions";
    public static readonly string FastPostLinesSpeedKey = RegistryKeys.OrchestratorRegistrySettingsPath + "FastPostLinesSpeed";
    public static readonly string SlowPostLinesSpeedKey = RegistryKeys.OrchestratorRegistrySettingsPath + "SlowPostLinesSpeed";
    public static readonly string DefaultHostedParallelismKey = "/Service/DistributedTask/Settings/HostedPool/DefaultHostedParallelism";
    public static readonly string InternalCloudAgentDefinitionTargetImageLabel = "/Service/DistributedTask/Settings/HostedPool/InternalCloudAgentDefinition/{0}/TargetImageLabel";
    public static readonly string InternalCloudAgentDefinitionTargetPercent = "/Service/DistributedTask/Settings/HostedPool/InternalCloudAgentDefinition/{0}/TargetPercent";
    public static readonly RegistryQuery OidcTokenMaxValidTime = (RegistryQuery) (RegistryKeys.OrchestratorRegistrySettingsPath + nameof (OidcTokenMaxValidTime));
    public static readonly string EnableShellTasksArgsSanitizing = "/Service/DistributedTask/Settings/EnableShellTasksArgsSanitizing2";
    public static readonly string EnableShellTasksArgsSanitizingAudit = "/Service/DistributedTask/Settings/EnableShellTasksArgsSanitizingAudit";
    public static readonly string PlanConcurrencySettingsPath = RegistryKeys.RegistrySettingsPath + "PlanConcurrencyConfigurationJson/";
    public static readonly string PlanConcurrencyConfigurationJson = RegistryKeys.PlanConcurrencySettingsPath + "ConfigurationJson";
    public static readonly string PlanConcurrencyMaxConcurrencyLimit = RegistryKeys.PlanConcurrencySettingsPath + "MaxConcurrencyLimit";
    public static readonly string PlanConcurrencyMaxCountLimit = RegistryKeys.PlanConcurrencySettingsPath + "MaxCountLimit";
    public static readonly string PipelineShardOverridesSettingsRoot = RegistryKeys.RegistrySettingsPath + "PipelineShardOverridesJson/";
    public static readonly string PipelineShardOverridesSettingPathFormat = RegistryKeys.PipelineShardOverridesSettingsRoot + "Hubs/{0}";
    public static readonly string PipelineBlockTimelinePeriod = RegistryKeys.RegistrySettingsPath + "PipelineBlockTimelineUpdates";

    public static class SettingsKeys
    {
      public const string PipelinesGeneral = "Pipelines/General/Settings";
    }
  }
}
