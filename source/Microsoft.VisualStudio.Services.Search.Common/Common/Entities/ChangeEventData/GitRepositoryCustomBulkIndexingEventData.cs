// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData.GitRepositoryCustomBulkIndexingEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData
{
  [DataContract]
  [Export(typeof (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData))]
  public class GitRepositoryCustomBulkIndexingEventData : Microsoft.VisualStudio.Services.Search.Common.ChangeEventData
  {
    internal GitRepositoryCustomBulkIndexingEventData()
    {
    }

    public GitRepositoryCustomBulkIndexingEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 2)]
    public bool DefaultBranchChanged { get; set; }

    [DataMember(Order = 3)]
    public List<string> BranchesToIndex { get; set; }

    [DataMember(Order = 4)]
    public List<string> BranchesToDelete { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext ctx)
    {
      if (this.BranchesToIndex != null && this.BranchesToIndex.Any<string>())
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.BranchesToIndex.RemoveAll(GitRepositoryCustomBulkIndexingEventData.\u003C\u003EO.\u003C0\u003E__IsNullOrWhiteSpace ?? (GitRepositoryCustomBulkIndexingEventData.\u003C\u003EO.\u003C0\u003E__IsNullOrWhiteSpace = new Predicate<string>(string.IsNullOrWhiteSpace)));
      }
      if (this.BranchesToDelete == null || !this.BranchesToDelete.Any<string>())
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BranchesToDelete.RemoveAll(GitRepositoryCustomBulkIndexingEventData.\u003C\u003EO.\u003C0\u003E__IsNullOrWhiteSpace ?? (GitRepositoryCustomBulkIndexingEventData.\u003C\u003EO.\u003C0\u003E__IsNullOrWhiteSpace = new Predicate<string>(string.IsNullOrWhiteSpace)));
    }
  }
}
