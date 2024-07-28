// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.WorkItem
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  public class WorkItem : TableEntity
  {
    public WorkItem()
    {
    }

    public WorkItem(
      string sourceControl,
      string branch,
      string depot,
      string identifier = null,
      bool isShadow = false)
    {
      if ("null".Equals(identifier, StringComparison.OrdinalIgnoreCase))
        identifier = string.Empty;
      this.PartitionKey = nameof (WorkItem);
      this.RowKey = sourceControl + branch + depot + identifier;
      this.SourceControl = sourceControl;
      this.Branch = branch;
      this.Depot = depot;
      this.ForceReindexing = "false";
      this.ForceSDSync = false;
      this.ForceHeal = false;
      this.LastSyncedChangedId = -1L;
      this.LastSyncedChangedTime = DateTime.MinValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.TargetSyncChangedId = -1L;
      this.TargetSyncChangedTime = DateTime.MinValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (isShadow)
      {
        this.CustomIndexingMode = "ReindexingShadow";
        this.ReRegisterRepository = "true";
        this.ForceReindexing = "true";
        this.ForceHeal = true;
      }
      else
        this.CustomIndexingMode = "IndexingDefault";
      this.LastIndexedChangeId = -1L;
      this.LatestChangeId = -1L;
      DateTime minValue = DateTime.MinValue;
      this.LastIndexedChangeIdChangeTime = minValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      minValue = DateTime.MinValue;
      this.LatestChangeIdChangeTime = minValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.FetchLastIndexedChangeId = false;
    }

    public string Branch { get; set; }

    public string Path { get; set; }

    public string SourceControl { get; set; }

    public string LastFullSync { get; set; }

    public string LastSync { get; set; }

    public string Indexer { get; set; }

    public string DedicatedIndexer { get; set; }

    public string NumberOfSourceFiles { get; set; }

    public string Mapping { get; set; }

    public string SubMapping { get; set; }

    public string ExcludeMapping { get; set; }

    public string Depot { get; set; }

    public string DepotPath { get; set; }

    public string ForceReindexing { get; set; }

    public bool ForceSDSync { get; set; }

    public long LastSyncedChangedId { get; set; }

    public string LastSyncedChangedTime { get; set; }

    public long TargetSyncChangedId { get; set; }

    public string TargetSyncChangedTime { get; set; }

    public string ReRegisterRepository { get; set; }

    public string NeedsDeduplication { get; set; }

    public string CloningDepthForCI { get; set; }

    public string ExtensionsForFullSync { get; set; }

    public bool ForceHeal { get; set; }

    public string SyncFailures { get; set; }

    public string CustomIndexingMode { get; set; }

    public string LastIndexedChangeIdChangeTime { get; set; }

    public string LatestChangeIdChangeTime { get; set; }

    public long LastIndexedChangeId { get; set; }

    public long LatestChangeId { get; set; }

    public bool FetchLastIndexedChangeId { get; set; }
  }
}
