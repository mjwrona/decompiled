// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.SearchEventId
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public static class SearchEventId
  {
    public static readonly int SearchEventsBaseId = 60000;
    public static readonly int PeriodicCatchUpJobFailed = SearchEventId.SearchEventsBaseId + 12;
    public static readonly int NewSharedIndexCreationFailed = SearchEventId.SearchEventsBaseId + 16;
    public static readonly int CollectionCodeMetadataCrawlOperationFailed = SearchEventId.SearchEventsBaseId + 20;
    public static readonly int ProjectRepoJobRetryFailed = SearchEventId.SearchEventsBaseId + 23;
    public static readonly int ElasticSearchQueueBackupJobFailed = SearchEventId.SearchEventsBaseId + 24;
    public static readonly int ElasticSearchMonitorBackupJobFailed = SearchEventId.SearchEventsBaseId + 25;
    public static readonly int ElasticSearchLongRunningBackupJobWarning = SearchEventId.SearchEventsBaseId + 26;
    public static readonly int AccountFaultInOverrideTriggerJobFailed = SearchEventId.SearchEventsBaseId + 27;
    public static readonly int TriggerAndMonitorReindexingJobFailed = SearchEventId.SearchEventsBaseId + 28;
    public static readonly int TriggerAndMonitorReindexingJobWarning = SearchEventId.SearchEventsBaseId + 29;
    public static readonly int CollectionWorkItemMetadataCrawlOperationFailed = SearchEventId.SearchEventsBaseId + 31;
    public static readonly int CollectionCodeUpdateMetadataOperationFailed = SearchEventId.SearchEventsBaseId + 32;
    public static readonly int CollectionProjectRepoMetadataCrawlOperationFailed = SearchEventId.SearchEventsBaseId + 33;
    public static readonly int CollectionPackageBulkIndexingOperationFailed = SearchEventId.SearchEventsBaseId + 34;
    public static readonly int ElasticsearchIndexOptimize = SearchEventId.SearchEventsBaseId + 35;
    public static readonly int ElasticsearchShardReduction = SearchEventId.SearchEventsBaseId + 36;
    public static readonly int AccountHealthStatusJobFailed = SearchEventId.SearchEventsBaseId + 37;
    public static readonly int CollectionPackageUpdateIndexingOperationFailed = SearchEventId.SearchEventsBaseId + 38;
    public static readonly int CollectionBoardMetadataCrawlOperationFailed = SearchEventId.SearchEventsBaseId + 39;
    public static readonly int CollectionBoardBulkIndexingOperationFailed = SearchEventId.SearchEventsBaseId + 40;
  }
}
