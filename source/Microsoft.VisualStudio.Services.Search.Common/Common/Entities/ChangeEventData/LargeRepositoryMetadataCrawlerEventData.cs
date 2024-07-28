// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData.LargeRepositoryMetadataCrawlerEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData
{
  [DataContract]
  [Export(typeof (Microsoft.VisualStudio.Services.Search.Common.ChangeEventData))]
  public class LargeRepositoryMetadataCrawlerEventData : Microsoft.VisualStudio.Services.Search.Common.ChangeEventData
  {
    private LargeRepositoryMetadataCrawlerEventData()
    {
    }

    public LargeRepositoryMetadataCrawlerEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 1)]
    public Dictionary<string, ScopedGitBranchIndexInfo> BranchIndexInfo { get; set; }

    [DataMember(Order = 2)]
    public Guid RequestId { get; set; }

    [DataMember(Order = 3)]
    public Dictionary<string, DepotLastChangeInfo> BranchToLatestChangeIdMap { get; set; }
  }
}
