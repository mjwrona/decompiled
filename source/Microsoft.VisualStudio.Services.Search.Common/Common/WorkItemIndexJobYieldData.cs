// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemIndexJobYieldData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class WorkItemIndexJobYieldData : AbstractJobYieldData
  {
    [DataMember]
    public string LastIndexedRevisionsContinuationToken { get; set; }

    [DataMember]
    public string LastIndexedDiscussionsContinuationToken { get; set; }

    [DataMember]
    public WorkItemCrawlStage WorkItemCrawlStage { get; set; }

    public override bool HasData() => !string.IsNullOrEmpty(this.LastIndexedRevisionsContinuationToken);

    public override AbstractJobYieldData Clone() => (AbstractJobYieldData) this.MemberwiseClone();
  }
}
