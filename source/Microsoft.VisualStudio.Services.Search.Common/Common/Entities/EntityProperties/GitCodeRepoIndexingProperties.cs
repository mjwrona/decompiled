// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.GitCodeRepoIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [Export(typeof (IndexingProperties))]
  public class GitCodeRepoIndexingProperties : IndexingProperties
  {
    [DataMember(Order = 1)]
    public Dictionary<string, GitBranchIndexInfo> BranchIndexInfo { get; set; }

    [DataMember(Order = 2)]
    public int LastProcessedPushId { get; set; }

    [DataMember(Order = 3)]
    public byte[] SecurityHashcode { get; set; }

    [DataMember(Order = 4)]
    public Dictionary<string, GitBranchIndexInfo> BranchIndexInfoPreReindexing { get; set; }

    [DataMember(Order = 5)]
    public GitIndexJobYieldStats GitIndexJobYieldStats { get; set; }

    [DataMember(Order = 6)]
    public DateTime RepositoryLastProcessedTime { get; set; }

    [DataMember(Order = 7)]
    public List<string> BranchesToClean { get; set; }

    public GitCodeRepoIndexingProperties()
    {
      this.BranchIndexInfo = new Dictionary<string, GitBranchIndexInfo>();
      this.GitIndexJobYieldStats = new GitIndexJobYieldStats();
      this.BranchesToClean = new List<string>();
      this.LastProcessedPushId = -1;
      this.RepositoryLastProcessedTime = RepositoryConstants.DefaultLastProcessedTime;
      this.SecurityHashcode = (byte[]) null;
      this.IsDisabled = false;
    }

    public override bool EraseIndexingWaterMarks(bool isShadowIndexing = false)
    {
      bool flag = false;
      this.GitIndexJobYieldStats = new GitIndexJobYieldStats();
      foreach (string key in this.BranchIndexInfo.Keys.ToList<string>())
      {
        if (!isShadowIndexing)
        {
          GitBranchIndexInfo gitBranchIndexInfo = this.BranchIndexInfo[key];
          if (gitBranchIndexInfo != null)
          {
            gitBranchIndexInfo.LastIndexedCommitId = RepositoryConstants.DefaultLastIndexCommitId;
            gitBranchIndexInfo.BulkIndexJobYieldData = new GitBulkIndexJobYieldData();
            gitBranchIndexInfo.GitIndexJobYieldData = new GitIndexJobYieldData();
            flag = true;
          }
        }
      }
      return flag;
    }

    public override void SaveIndexingStatePreReindexing()
    {
      base.SaveIndexingStatePreReindexing();
      this.BranchIndexInfoPreReindexing = this.BranchIndexInfo;
    }

    public override void ResetIndexingStatePreReindexing()
    {
      base.ResetIndexingStatePreReindexing();
      this.IndexIndices = new List<IndexInfo>();
    }

    public override void ErasePreReindexingState()
    {
      base.ErasePreReindexingState();
      this.BranchIndexInfoPreReindexing = (Dictionary<string, GitBranchIndexInfo>) null;
    }
  }
}
