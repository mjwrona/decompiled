// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.ProjectWorkItemIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (IndexingProperties))]
  public class ProjectWorkItemIndexingProperties : IndexingProperties
  {
    [Obsolete("This property has been replaced by LastIndexedContinuationToken")]
    [DataMember(Order = 0)]
    public string LastIndexedWaterMark { get; set; }

    [DataMember(Order = 1)]
    public string LastIndexedContinuationToken { get; set; }

    [DataMember(Order = 2)]
    public WorkItemIndexJobYieldData WorkItemIndexJobYieldData { get; set; }

    [DataMember(Order = 3)]
    public IdentityDescriptor ProjectAdministrators { get; set; }

    [DataMember(Order = 4)]
    public string LastIndexedFieldsContinuationToken { get; set; }

    [DataMember(Order = 5)]
    public string LastIndexedDiscussionContinuationToken { get; set; }

    public override bool EraseIndexingWaterMarks(bool isShadowIndexing = false)
    {
      if (isShadowIndexing)
        return false;
      this.LastIndexedContinuationToken = (string) null;
      this.LastIndexedFieldsContinuationToken = (string) null;
      this.LastIndexedDiscussionContinuationToken = (string) null;
      this.WorkItemIndexJobYieldData = new WorkItemIndexJobYieldData();
      return true;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext ctx)
    {
      if (string.IsNullOrWhiteSpace(this.LastIndexedContinuationToken))
        this.LastIndexedContinuationToken = (string) null;
      if (string.IsNullOrWhiteSpace(this.LastIndexedFieldsContinuationToken))
        this.LastIndexedFieldsContinuationToken = (string) null;
      if (!string.IsNullOrWhiteSpace(this.LastIndexedDiscussionContinuationToken))
        return;
      this.LastIndexedDiscussionContinuationToken = (string) null;
    }
  }
}
