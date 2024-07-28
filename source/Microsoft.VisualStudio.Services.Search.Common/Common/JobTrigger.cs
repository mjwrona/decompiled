// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.JobTrigger
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class JobTrigger
  {
    public const int None = 0;
    public const int AccountFaultIn = 1;
    public const int PushEventNotification = 2;
    public const int RepositoryMetadataUpdater = 3;
    public const int ProjectRenameNotification = 4;
    public const int BulkIndexingInternalRetryForFailedEntities = 5;
    public const int BulkIndexingFailedRetry = 6;
    public const int AccountMigrator = 7;
    public const int RemoveWorkItemIndexedFieldsCmdlet = 8;
    public const int SearchExtensionEventNotification = 9;
    public const int AccountFaultinOverride = 10;
    public const int PeriodicCatchUp = 11;
    public const int SearchCustomHostUpgrade = 12;
    public const int ProjectsIndexer = 13;
    public const int AddPatchOperationCmdLet = 14;
    public const int CustomRepositoryBulkIndexing = 15;
    public const int RegisterCustomRepository = 16;
    public const int CustomTenantForwarder = 18;
    public const int PeriodicMaintenanceJob = 19;
    public const int SetBranchesInGitRepository = 20;
    public const int AccountDeletion = 21;
    public const int RepositoryEventNotification = 22;
    public const int HostEventNotification = 23;
    public const int AddRepositoryBulkIndexingOperation = 24;
    public const int CodeRepair = 25;
    public const int AddCollectionIndexFinalizeOperation = 26;
    public const int CollectionRenameRepairJob = 27;
    public const int SetIndexingState = 28;
    public const int SetupIndexRouting = 29;
    public const int AddCodeProjectRenameFinalizeOperation = 30;
    public const int LargeRepoPeriodicCI = 31;
    public const int SearchUrlUpdate = 32;
    public const int ReindexCollection = 33;
    public const int ClassificationNodeChangeNotification = 34;
    public const int WorkItemChangedNotification = 35;
    public const int BalanceShards = 36;
    public const int HostUpgradePopulateNamesInIndexingProperties = 39;
    public const int HostUpgradeResetGitBranches = 42;
    public const int SendSqlNotification = 44;
    public const int HostUpgradeFixCodeReindexingState = 45;
    public const int FeedChangedNotification = 47;
    public const int WikiUpdatedNotification = 48;
    public const int UpdateElasticsearchIndexSettings = 49;
    public const int AccountHealthStatusJob = 50;
    public const int ElasticsearchIndexOptimizeJob = 51;
    public const int ElasticsearchShardSizeReduction = 52;
    public const int FormatIndexInfoCmdlet = 55;
    public const int DuplicateDocumentDeletionCmdlet = 56;
    public const int DeleteIndexingUnits = 57;
    public const int ClearJobYieldDataCmdlet = 58;
    public const int SetIndexContractTypeCmdlet = 59;
    public const int PackageChangeEventNotification = 60;
    public const int SettingsSearchIndexUpdate = 61;
    public const int CodeIndexingDelayAnalyzerJob = 62;
    public const int UndoReindexing = 63;
  }
}
