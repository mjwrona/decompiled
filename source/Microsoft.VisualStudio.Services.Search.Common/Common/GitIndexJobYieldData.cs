// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.GitIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class GitIndexJobYieldData : AbstractJobYieldData
  {
    public GitIndexJobYieldData()
    {
      this.BaseVersion = string.Empty;
      this.TargetVersion = string.Empty;
      this.TargetVersionDate = RepositoryConstants.DefaultLastIndexChangeUtcTime;
      this.ContinuationToken = string.Empty;
    }

    public GitIndexJobYieldData(
      string baseVersion,
      string targetVersion,
      string continuationToken,
      DateTime targetVersionDate)
    {
      this.BaseVersion = baseVersion;
      this.TargetVersion = targetVersion;
      this.TargetVersionDate = targetVersionDate;
      this.ContinuationToken = continuationToken;
    }

    [DataMember]
    public string BaseVersion { get; set; }

    [DataMember]
    public string TargetVersion { get; set; }

    [DataMember]
    public DateTime TargetVersionDate { get; set; }

    [DataMember]
    public string ContinuationToken { get; set; }

    public override AbstractJobYieldData Clone() => (AbstractJobYieldData) this.MemberwiseClone();

    public override bool HasData() => !string.IsNullOrEmpty(this.TargetVersion) && !this.TargetVersion.Equals(RepositoryConstants.BranchCreationOrDeletionCommitId) && !string.IsNullOrWhiteSpace(this.ContinuationToken);

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[BaseVersion: ");
      stringBuilder.Append(this.BaseVersion);
      stringBuilder.Append(" ,TargetVersion: ");
      stringBuilder.Append(this.TargetVersion);
      stringBuilder.Append(" ,TargetVersionDate: ");
      stringBuilder.Append((object) this.TargetVersionDate);
      stringBuilder.Append(" ,ContinuationToken: ");
      stringBuilder.Append(this.ContinuationToken);
      stringBuilder.Append("]");
      stringBuilder.Append(base.ToString());
      return stringBuilder.ToString();
    }

    public void CopyFrom(GitBulkIndexJobYieldData gitBulkIndexJobYieldData)
    {
      if (gitBulkIndexJobYieldData == null || !gitBulkIndexJobYieldData.HasData())
        return;
      this.BaseVersion = RepositoryConstants.BranchCreationOrDeletionCommitId;
      this.TargetVersion = gitBulkIndexJobYieldData.CrawlerTraversalTopCommitId;
      this.ContinuationToken = gitBulkIndexJobYieldData.ContinuationToken;
      this.TargetVersionDate = gitBulkIndexJobYieldData.CrawlerTraversalTopCommitDate;
    }
  }
}
