// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcBulkIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcBulkIndexJobYieldData : AbstractJobYieldData
  {
    public TfvcBulkIndexJobYieldData()
    {
      this.CrawlerTraversalChangesetId = -1;
      this.CrawlerTraversalChangesetDate = RepositoryConstants.DefaultLastIndexChangeUtcTime;
      this.CurrentBatchId = -1;
      this.LastBatchId = -1;
    }

    [DataMember]
    public int CrawlerTraversalChangesetId { get; set; }

    [DataMember]
    public DateTime CrawlerTraversalChangesetDate { get; set; }

    [DataMember]
    public int CurrentBatchId { get; set; }

    [DataMember]
    public int LastBatchId { get; set; }

    public override bool HasData() => this.CrawlerTraversalChangesetId != -1;

    public override AbstractJobYieldData Clone() => (AbstractJobYieldData) this.MemberwiseClone();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[CrawlerTraversalChangesetId: ");
      stringBuilder.Append(this.CrawlerTraversalChangesetId);
      stringBuilder.Append(", CrawlerTraversalChangesetDate: ");
      stringBuilder.Append((object) this.CrawlerTraversalChangesetDate);
      stringBuilder.Append(", CurrentBatchId: ");
      stringBuilder.Append(this.CurrentBatchId);
      stringBuilder.Append(", LastBatchId: ");
      stringBuilder.Append(this.LastBatchId);
      stringBuilder.Append("]");
      stringBuilder.Append(base.ToString());
      return stringBuilder.ToString();
    }
  }
}
