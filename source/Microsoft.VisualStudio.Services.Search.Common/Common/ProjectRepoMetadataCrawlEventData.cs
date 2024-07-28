// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ProjectRepoMetadataCrawlEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class ProjectRepoMetadataCrawlEventData : ChangeEventData
  {
    private ProjectRepoMetadataCrawlEventData()
    {
    }

    public ProjectRepoMetadataCrawlEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember]
    public bool ShouldFinalize { get; set; }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Guid RepositoryId { get; set; }

    [DataMember]
    public DateTime EventTime { get; set; }
  }
}
