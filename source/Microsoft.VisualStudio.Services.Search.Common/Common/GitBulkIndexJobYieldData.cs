// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.GitBulkIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class GitBulkIndexJobYieldData : AbstractJobYieldData
  {
    public GitBulkIndexJobYieldData()
    {
      this.CrawlerTraversalTopCommitId = string.Empty;
      this.CrawlerTraversalTopCommitDate = RepositoryConstants.DefaultLastIndexChangeUtcTime;
      this.ContinuationToken = string.Empty;
    }

    [DataMember]
    public string CrawlerTraversalTopCommitId { get; set; }

    [DataMember]
    public DateTime CrawlerTraversalTopCommitDate { get; set; }

    [DataMember]
    public string ContinuationToken { get; set; }

    public override bool HasData() => !string.IsNullOrEmpty(this.CrawlerTraversalTopCommitId);

    public override AbstractJobYieldData Clone() => (AbstractJobYieldData) this.MemberwiseClone();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[CrawlerTraversalTopCommitId: ");
      stringBuilder.Append(this.CrawlerTraversalTopCommitId);
      stringBuilder.Append(", CrawlerTraversalTopCommitDate: ");
      stringBuilder.Append((object) this.CrawlerTraversalTopCommitDate);
      stringBuilder.Append(", ContinuationToken: ");
      stringBuilder.Append(this.ContinuationToken);
      stringBuilder.Append("]");
      stringBuilder.Append(base.ToString());
      return stringBuilder.ToString();
    }
  }
}
