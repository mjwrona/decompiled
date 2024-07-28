// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FeedIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class FeedIndexJobYieldData : AbstractJobYieldData
  {
    public FeedIndexJobYieldData() => this.LastIndexedContinuationToken = -1L;

    [DataMember]
    public long LastIndexedContinuationToken { get; set; }

    [DataMember]
    public Guid FeedId { get; set; }

    public override bool HasData() => this.LastIndexedContinuationToken != -1L && this.IncompleteTreeCrawl;

    public override AbstractJobYieldData Clone() => (AbstractJobYieldData) this.MemberwiseClone();
  }
}
