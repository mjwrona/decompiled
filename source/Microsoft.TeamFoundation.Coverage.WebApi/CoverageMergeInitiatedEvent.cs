// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Coverage.WebApi.CoverageMergeInitiatedEvent
// Assembly: Microsoft.TeamFoundation.Coverage.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 18FFD1B8-3EB9-46CC-8BE4-74DD890A1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Coverage.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Coverage.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class CoverageMergeInitiatedEvent
  {
    public CoverageMergeInitiatedEvent(
      ProjectReference project,
      PipelineInstanceReference pipelineInstance)
    {
      this.Project = project;
      this.PipelineInstance = pipelineInstance;
    }

    [DataMember(IsRequired = true)]
    public ProjectReference Project { get; private set; }

    [DataMember(IsRequired = true)]
    public PipelineInstanceReference PipelineInstance { get; private set; }
  }
}
