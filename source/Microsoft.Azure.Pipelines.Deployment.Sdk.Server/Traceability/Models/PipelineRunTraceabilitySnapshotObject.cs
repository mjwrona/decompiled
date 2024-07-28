// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.PipelineRunTraceabilitySnapshotObject
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models
{
  public class PipelineRunTraceabilitySnapshotObject
  {
    public int CurrentRunId;
    public IList<ArtifactVersion> BaseRunArtifactVersions;
    public PipelineTraceabilityBaseRunDetails BaseRunDetails;
    public int? CommitsCount;
    public int? WorkItemsCount;
  }
}
