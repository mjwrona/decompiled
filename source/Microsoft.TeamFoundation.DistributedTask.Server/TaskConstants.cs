// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskConstants
  {
    public const string HubName = "DistributedTask";
    public static readonly Guid AgentCapabilityUpdateJob = new Guid("5820B920-19B7-4983-A9E7-1AF68090CC53");
    public static readonly Guid AgentRequestMonitorJob = new Guid("3F9FBAB5-E92E-46C7-9798-D739205B9AF3");
    public static readonly Guid HostedPoolResizeJob = new Guid("A0A89175-9AE7-4DD1-A907-DCA2B73AAFB9");
    public static readonly Guid HostedPoolRemoveJob = new Guid("28EE8171-724D-4D34-A8BE-7E27790CA03F");
    public static readonly Guid AgentRequestAssignmentJob = new Guid("B5DADE7F-B7E5-4D8E-A1BF-E0A067113286");
    public static readonly Guid AgentCloudPoolResizeJob = new Guid("A1FEE006-FE99-47D1-BB96-FEEB83D8046C");
    public static readonly Guid PrivateAgentCustomerIntelligenceJob = new Guid("AC186C63-F1d7-4221-9E16-41D07CF62C6A");
    public static readonly Guid DeploymentPoolMonitorJobJob = new Guid("2EDC10E4-34F3-45CD-902A-C38269D4FCA7");
    public static readonly Guid TaskContributionInstallJob = new Guid("E591F896-A440-4431-9EBB-57D72908DF62");
    public static readonly Guid UpdateAgentCloudParallelismJob = new Guid("7A9C9B51-C4D3-4EE7-B7E0-1C362EC46BF8");
    public static readonly Guid AgentCloudRequestOrchestrationMonitorJob = new Guid("8DBE123B-BEF1-4446-B60B-0362E3FA61CC");
    public static readonly Guid ResourceAssignmentJobId = new Guid("CD9A3BA3-CC89-4320-BD01-0C971434D15F");
    public static Guid CommerceDataChangedNotification = new Guid("AC90A1F8-B901-48B8-BB13-87EF2BD36D81");
    public const string TaskMessageBusName = "Microsoft.TeamFoundation.DistributedTask.Server";
    public const string ExtensionDownloadJobExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.ContributionTasksDownloaderJob";
    public const string ExtensionUpdateJobExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.ContributionTasksUpdateJob";
    public const string ExtensionDeleteJobExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.ContributionTasksDeleteJob";
    public static readonly string BuildTaskContributionIdentifier = "ms.vss-distributed-task.tasks";
    public static readonly string BuildTaskContributionType = "ms.vss-distributed-task.task";
    public static readonly string ContributionDownloaderJobNameFormat = "Tasks Downloader Job for '{0}.{1}'";
    public static readonly string ContributionUpdateJobNameFormat = "Tasks Update Job for '{0}.{1}'";
    public static readonly string ContributionDeleteJobNameFormat = "Tasks Delete Job for '{0}.{1}'";
    public static readonly int ContributionJobNameLength = 128;
    public static readonly int ThresholdForWarrningRequestDealyedFromPPInSeconds = 300;
    public static readonly string LatestExtensionVersion = "latest";
    public static readonly string PreInstalledEventName = "preinstall";
    public static readonly string InstallEventName = "install";
    public static readonly string UnInstallEventName = "uninstall";
    public static readonly string[] BuildTaskTargetContributions = new string[1]
    {
      TaskConstants.BuildTaskContributionIdentifier
    };
    public const string DataMigrationLockNameFormat = "vsts://DistributedTask/DataMigration/{0}";
    public const string DataMigrationKeyPath = "/Service/DistributedTask/Settings/DataMigration";
    public const string PoolsAtCollectionKeyPath = "/Service/DistributedTask/Settings/PoolsAtCollection";
    public const string LocalPackageLocationKeyPath = "/Service/DistributedTask/Settings/PackageLocation";
    public const string PoolRequestContextProperty = "MS.VS.DistributedTask.PoolRequestContext";
    public const string DataMigrationLockProperty = "MS.VS.DistributedTask.DataMigrationLock";
    public const string AgentDrawerFormat = "ms.vss.distributedtask.pools.{0}.agents.{1}";
    public const string AgentEncryptionKeyPath = "/keys/encryption";
    public const string MobileCenterIntMacPoolName = "Hosted Mac Mobile Center INT";
    public const string MobileCenterStagingMacPoolName = "Hosted Mac Mobile Center Staging";
    public const string MobileCenterProdMacPoolName = "Hosted Mac Mobile Center Prod";
    public const string MobileCenterIntHSMacPoolName = "Hosted Mac Mobile Center High Sierra INT";
    public const string MobileCenterStagingHSMacPoolName = "Hosted Mac Mobile Center High Sierra Staging";
    public const string MobileCenterProdHSMacPoolName = "Hosted Mac Mobile Center High Sierra Prod";
    public const string HostedMacOSPreviewPoolName = "Hosted macOS Preview";
    public const string AgentCorePlatformPrefix = "vstsagentcore-";
    public const string InternalAgentCloudName = "Azure Pipelines";
    public const string AgentRequestSettingsRootPath = "/Service/DistributedTask/Settings/AgentRequest";
    public const string HostedPoolRemovalDataKeyPath = "/Service/DistributedTask/Settings/HostedPoolRemovalData";
    public const string HostedPoolMigrationStagePath = "/Service/DistributedTask/Settings/HostedPoolMigrationStage";
    public const string PoolProviderAgentSettingRootPath = "/Service/DistributedTask/Settings/PoolProvider";
    public const string DisableInBoxTasks = "/Service/DistributedTask/Settings/DisableInBoxTasks";
    public const string DisableMarketplaceTasks = "/Service/DistributedTask/Settings/DisableMarketplaceTasks";
    public const string AssignmentJobTimeoutInSeconds = "/Service/DistributedTask/Settings/AssignmentJobTimeoutInSeconds";
    public const int AssignmentJobDefaultTimeout = 60;
    public const int AssignmentJobRetryAfter = 60;
    public const string ElasticPoolResizedPublisherEventType = "ms.vss-distributed-task.elastic-pool-resized-event";
    public static readonly ApiResourceVersion ApiVersion = new ApiResourceVersion(VssRestApiVersion.v5_0.ToVersion())
    {
      IsPreview = true
    };
    public static readonly ApiResourceVersion DeploymentApiVersion = new ApiResourceVersion(VssRestApiVersion.v6_0.ToVersion())
    {
      IsPreview = true
    };
  }
}
