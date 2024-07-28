// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.TfvcCodeRepoIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [Export(typeof (IndexingProperties))]
  public class TfvcCodeRepoIndexingProperties : IndexingProperties
  {
    public TfvcCodeRepoIndexingProperties()
    {
      this.LastIndexedChangeSetId = -1;
      this.RepositoryLastProcessedTime = RepositoryConstants.DefaultLastProcessedTime;
      this.LastIndexedChangeSetIdPreReindexing = -1;
      this.BulkIndexJobYieldData = new TfvcBulkIndexJobYieldData();
      this.ContinuousIndexJobYieldData = new TfvcContinuousIndexJobYieldData();
      this.TfvcIndexJobYieldData = new TfvcIndexJobYieldData();
      this.TfvcIndexJobYieldStats = new TfvcIndexJobYieldStats();
    }

    [DataMember(Order = 1)]
    public int LastIndexedChangeSetId { get; set; }

    [DataMember(Order = 2)]
    public TfvcBulkIndexJobYieldData BulkIndexJobYieldData { get; set; }

    [DataMember(Order = 3)]
    public TfvcContinuousIndexJobYieldData ContinuousIndexJobYieldData { get; set; }

    [DataMember(Order = 4)]
    public int LastIndexedChangeSetIdPreReindexing { get; set; }

    [DataMember(Order = 5)]
    public TfvcBulkIndexJobYieldData BulkIndexJobYieldDataPreReindexing { get; set; }

    [DataMember(Order = 6)]
    public TfvcContinuousIndexJobYieldData ContinuousIndexJobYieldDataPreReindexing { get; set; }

    [DataMember(Order = 7)]
    public TfvcIndexJobYieldData TfvcIndexJobYieldData { get; set; }

    [DataMember(Order = 8)]
    public TfvcIndexJobYieldData TfvcIndexJobYieldDataPreReindexing { get; set; }

    [DataMember(Order = 9)]
    public TfvcIndexJobYieldStats TfvcIndexJobYieldStats { get; set; }

    [DataMember(Order = 10)]
    public DateTime RepositoryLastProcessedTime { get; set; }

    public override bool EraseIndexingWaterMarks(bool isShadowIndexing = false)
    {
      if (isShadowIndexing)
        return false;
      this.LastIndexedChangeSetId = -1;
      this.BulkIndexJobYieldData = new TfvcBulkIndexJobYieldData();
      this.ContinuousIndexJobYieldData = new TfvcContinuousIndexJobYieldData();
      this.TfvcIndexJobYieldData = new TfvcIndexJobYieldData();
      this.TfvcIndexJobYieldStats = new TfvcIndexJobYieldStats();
      return true;
    }

    public override void SaveIndexingStatePreReindexing()
    {
      base.SaveIndexingStatePreReindexing();
      this.LastIndexedChangeSetIdPreReindexing = this.LastIndexedChangeSetId;
      this.BulkIndexJobYieldDataPreReindexing = this.BulkIndexJobYieldData;
      this.ContinuousIndexJobYieldDataPreReindexing = this.ContinuousIndexJobYieldData;
      this.TfvcIndexJobYieldDataPreReindexing = this.TfvcIndexJobYieldData;
    }

    public override void ResetIndexingStatePreReindexing()
    {
      base.ResetIndexingStatePreReindexing();
      this.IndexIndices = new List<IndexInfo>();
    }

    public override void ErasePreReindexingState()
    {
      base.ErasePreReindexingState();
      this.LastIndexedChangeSetIdPreReindexing = -1;
      this.BulkIndexJobYieldDataPreReindexing = (TfvcBulkIndexJobYieldData) null;
      this.ContinuousIndexJobYieldDataPreReindexing = (TfvcContinuousIndexJobYieldData) null;
      this.TfvcIndexJobYieldDataPreReindexing = (TfvcIndexJobYieldData) null;
    }
  }
}
