// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.ScopedGitRepositoryIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (IndexingProperties))]
  public class ScopedGitRepositoryIndexingProperties : IndexingProperties
  {
    [DataMember(Order = 1)]
    public Dictionary<string, ScopedGitBranchIndexInfo> BranchIndexInfo { get; set; }

    [DataMember(Order = 1)]
    public List<string> BranchesToClean { get; set; }

    public ScopedGitRepositoryIndexingProperties()
    {
      this.BranchesToClean = new List<string>();
      this.BranchIndexInfo = new Dictionary<string, ScopedGitBranchIndexInfo>();
    }

    public override bool EraseIndexingWaterMarks(bool isShadowIndexing = false)
    {
      if (isShadowIndexing)
        return false;
      bool flag = false;
      foreach (string key in this.BranchIndexInfo.Keys.ToList<string>())
      {
        ScopedGitBranchIndexInfo gitBranchIndexInfo = this.BranchIndexInfo[key];
        if (gitBranchIndexInfo != null)
        {
          gitBranchIndexInfo.CommitId = RepositoryConstants.DefaultLastIndexCommitId;
          gitBranchIndexInfo.GitIndexJobYieldData = new GitIndexJobYieldData();
          flag = true;
        }
      }
      return flag;
    }

    public override void SaveIndexingStatePreReindexing()
    {
      base.SaveIndexingStatePreReindexing();
      foreach (string key in this.BranchIndexInfo.Keys.ToList<string>())
      {
        ScopedGitBranchIndexInfo gitBranchIndexInfo = this.BranchIndexInfo[key];
        if (gitBranchIndexInfo != null)
          gitBranchIndexInfo.CommitIdPreReindexing = gitBranchIndexInfo.CommitId;
      }
    }

    public override void ErasePreReindexingState()
    {
      base.ErasePreReindexingState();
      foreach (string key in this.BranchIndexInfo.Keys.ToList<string>())
      {
        ScopedGitBranchIndexInfo gitBranchIndexInfo = this.BranchIndexInfo[key];
        if (gitBranchIndexInfo != null)
          gitBranchIndexInfo.CommitIdPreReindexing = RepositoryConstants.DefaultLastIndexCommitId;
      }
    }
  }
}
