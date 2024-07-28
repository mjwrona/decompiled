// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.ElasticSearchPlatformSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public static class ElasticSearchPlatformSettings
  {
    public const string RefreshInterval = "refresh_interval";
    public const string NumberOfReplicas = "number_of_replicas";
    public const string NumberOfShards = "number_of_shards";
    public const string RefreshIntervalForBulkIndexing = "-1";
    public const string RefreshIntervalForContinuousIndexing = "30s";
    public const string RefreshIntervalPostIndexing = "30s";
    public const int MaximumSegments = 1;
    public const int MaximumRetries = 2;
    public const string Routing = "routing";
    public const string Allocation = "allocation";
    public const string Include = "include";
    public const string IndexAllocationMaxRetries = "index.allocation.max_retries";
    public const string IndexVersionCreated = "index.version.created";
    public const string NodeLeftDelayedTimeoutName = "unassigned.node_left.delayed_timeout";
    public const string NodeLeftDelayedTimeoutValue = "20m";
    public const string Green = "green";
    public const string Red = "red";
    public const string Yellow = "yellow";
    public const int DelayBetweenRetriesInMs = 1000;
    public const string MaxIndexInitializationWaitTimeInSec = "300s";
    public const int ActiveAccountMoveThresholdPercent = 40;
    public static readonly IndexIdentity AllIndices = IndexIdentity.CreateIndexIdentity("*");
    public const int IndexAllocationMaxRetriesValue = 1000;
    public const string DefaultQueryRequestTimeout = "62s";
  }
}
