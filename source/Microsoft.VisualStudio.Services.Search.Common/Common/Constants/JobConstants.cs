// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Constants.JobConstants
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.Constants
{
  public static class JobConstants
  {
    public const string RepoIndexerJobName = "Repository Indexer Job";
    public const string CustomRepoIndexerJobName = "Custom Repository Indexer Job";
    public const string RepoIndexOperationsJobName = "Repository Index Operations Job";
    public const string GitPushIndexerJobName = "GitPush Indexer Job";
    public const string ProjectIndexerJobName = "Project Indexer Job";
    public const string ProjectIndexerJobExtension = "ProjectIndexerJob";
    public const string ElasticSearchQueueBackupJob = "ElasticSearch Queue Backup Job";
    public const string ElasticSearchMonitorBackupJob = "ElasticSearch Monitor Backup Job";
    public const string ElasticsearchIndexOptimizeJob = "Elasticsearch Index Optimize Job";
    public const string CodeIndexShardReductionJob = "Code Index Shard Size Reduction Job";
    public const string CodeIndexingDelayAnalyzerJob = "Code Indexing Delay Analyzer Job";
    public const string WorkItemRecentActivityCleanupJob = "Work Item Recent Activity Cleanup Job";
    public const string RepoDirName = "SC";
    public const int RequeueJobTimeIntervalInSeconds = 600;
    public const string CodeRepairJobIndexingUnitTypeData = "IndexingUnitType";
    public const string CodeRepairJobRepositoryIdData = "RepositoryId";
    public const string CodeRepairJobRepositoryTypeData = "RepositoryType";

    public static Guid CPFManagerJobId => new Guid("64FFD36A-A9FB-46F9-9E82-66B3FBB77F41");

    public static Guid RepositoryMetaDataCrawlJobId => new Guid("3789456f-a077-4bfb-b053-eeeaadf8ac3d");

    public static Guid AccountFaultInJobId => new Guid("02F271F3-0D40-4FA0-9328-C77EBCA59B6F");

    public static Guid WorkItemAccountFaultInJobId => new Guid("03CEE4B8-ECC1-4E57-95CE-FA430FE0DBFB");

    public static Guid ProjectCollectionFaultInJobId => new Guid("75C4EF69-37A3-4A9A-B3D9-FC11B7821CD4");

    public static Guid PeriodicProjectRepoRefreshJobId => new Guid("CDEF16A6-628A-4DAF-9339-DE950C461C3C");

    public static Guid PeriodicWorkItemRefreshJobId => new Guid("0054D1B2-1C24-4A2F-A1A4-B6F9D6684C8C");

    public static Guid RepositoryMetadataUpdaterJobId => new Guid("8FD216E9-6EA2-4A40-879F-E8D3639D1423");

    public static Guid ElasticSearchQueueBackupJobId => new Guid("957608E3-8013-4CB9-84EE-A8674EB1D40F");

    public static Guid ElasticSearchMonitorBackupJobId => new Guid("33B4F94E-6E54-40E7-B6AC-E80E23070EA0");

    public static Guid TriggerAndMonitorReindexingJob => new Guid("A1897EF6-4A36-4C0B-B3DE-FB5E775C7528");

    public static Guid PeriodicCatchUpJobId => new Guid("BDEF6A8C-DD9A-4987-8A62-83099B45398D");

    public static Guid PeriodicWikiCatchUpJobId => new Guid("1EAE81E0-D1FB-4208-8D80-126FE52CD6D6");

    public static Guid PeriodicMaintenanceJobId => new Guid("1761D61B-4708-42A2-9E29-A39540141277");

    public static Guid CleanupDisabledAccountsJobId => new Guid("7BCE4D28-7101-421B-9B3C-640049E48E00");

    public static Guid PartitionDeletionJobId => new Guid("DE1A9CEF-A95E-4BFF-8B7C-AD2D9164E445");

    public static Guid WikiAccountFaultInJobId => new Guid("27B11FD5-1DA5-48B4-A732-761CE99F5A5F");

    public static Guid ElasticsearchIndexOptimizeJobId => new Guid("6AE595B6-A697-4DA7-9F5F-85D0CDC7F5AA");

    public static Guid CodeIndexingDelayAnalyzerJobId => new Guid("A1FD9914-C114-4BF8-AAA5-B3C6861C20EC");

    public static Guid SettingsSearchIndexUpdateJobId => new Guid("7158CC5A-714E-421C-89FD-52412850E192");

    public static Guid AccountHealthStatusJobId => new Guid("808C1516-745F-4BC4-AEB1-CDE03DB48A58");

    public static Guid CodeIndexShardSizeReductionJobId => new Guid("FB6FD1EE-F3C2-41F0-9C88-15B9FD138519");

    public static Guid PackageAccountFaultInJobId => new Guid("10FA5A9A-27D7-4B49-9596-353E1AAA215C");

    public static Guid PackageAccountReindexerJobId => new Guid("F4680BF4-69CE-4767-88B8-676731CDE2AA");

    public static Guid BoardAccountFaultInJobId => new Guid("B398C825-DC43-4786-9797-47EBA9574F25");

    public static Guid BoardPeriodicReindexingJobJobId => new Guid("2789846F-B9BE-460D-9BDD-9C9B8BCC408F");

    public static Guid PeriodicPackageCatchupJobId => new Guid("27728891-B281-4F4D-A09B-915340ACD10F");

    public static Guid WorkItemRecentActivityCleanupJobId => new Guid("D037E3BB-53CD-46D1-BD7D-EBD56EB08C91");
  }
}
