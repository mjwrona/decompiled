// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TfvcContinuousIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class TfvcContinuousIndexJobYieldData : AbstractJobYieldData
  {
    public TfvcContinuousIndexJobYieldData() => this.CrawlerTraversalChangesetQueue = new List<TfvcContinuousIndexJobYieldData.ChangeSetEntry>();

    [DataMember]
    public List<TfvcContinuousIndexJobYieldData.ChangeSetEntry> CrawlerTraversalChangesetQueue { get; set; }

    public override bool HasData() => this.CrawlerTraversalChangesetQueue != null && this.CrawlerTraversalChangesetQueue.Count > 0;

    public override AbstractJobYieldData Clone()
    {
      TfvcContinuousIndexJobYieldData indexJobYieldData = (TfvcContinuousIndexJobYieldData) this.MemberwiseClone();
      if (this.CrawlerTraversalChangesetQueue != null)
      {
        indexJobYieldData.CrawlerTraversalChangesetQueue = new List<TfvcContinuousIndexJobYieldData.ChangeSetEntry>();
        indexJobYieldData.CrawlerTraversalChangesetQueue.AddRange((IEnumerable<TfvcContinuousIndexJobYieldData.ChangeSetEntry>) this.CrawlerTraversalChangesetQueue);
      }
      return (AbstractJobYieldData) indexJobYieldData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[CrawlerTraversalChangesetQueue_Count: ");
      stringBuilder.Append(this.CrawlerTraversalChangesetQueue != null ? this.CrawlerTraversalChangesetQueue.Count : 0);
      stringBuilder.Append("]");
      stringBuilder.Append(base.ToString());
      return stringBuilder.ToString();
    }

    [DataContract]
    public class ChangeSetEntry
    {
      public ChangeSetEntry()
      {
        this.ChangesetId = -1;
        this.CreationDate = RepositoryConstants.DefaultLastIndexChangeUtcTime;
        this.Skip = 0;
      }

      [DataMember]
      public int ChangesetId { get; set; }

      [DataMember]
      public DateTime CreationDate { get; set; }

      [DataMember]
      public int Skip { get; set; }
    }
  }
}
