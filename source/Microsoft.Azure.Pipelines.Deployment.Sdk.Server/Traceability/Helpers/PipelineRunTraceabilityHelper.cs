// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Helpers.PipelineRunTraceabilityHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Helpers
{
  public static class PipelineRunTraceabilityHelper
  {
    public static PipelineRunTraceabilitySnapshot ToPipelineRunTraceabilitySnapshot(
      this PipelineRunTraceabilitySnapshotObject snapshotObject)
    {
      if (snapshotObject == null)
        return new PipelineRunTraceabilitySnapshot();
      return new PipelineRunTraceabilitySnapshot()
      {
        CurrentRunId = snapshotObject.CurrentRunId,
        CommitsCount = snapshotObject.CommitsCount,
        WorkItemsCount = snapshotObject.WorkItemsCount,
        BaseRunArtifactVersions = JsonUtility.ToString<ArtifactVersion>(snapshotObject.BaseRunArtifactVersions),
        BaseRunDetails = JsonUtility.ToString((object) snapshotObject.BaseRunDetails)
      };
    }

    public static PipelineRunTraceabilitySnapshotObject ToPipelineRunTraceabilitySnapshotObject(
      this PipelineRunTraceabilitySnapshot snapshotData)
    {
      if (snapshotData == null)
        return new PipelineRunTraceabilitySnapshotObject();
      PipelineRunTraceabilitySnapshotObject traceabilitySnapshotObject = new PipelineRunTraceabilitySnapshotObject()
      {
        CurrentRunId = snapshotData.CurrentRunId,
        CommitsCount = snapshotData.CommitsCount,
        WorkItemsCount = snapshotData.WorkItemsCount
      };
      if (!string.IsNullOrWhiteSpace(snapshotData.BaseRunArtifactVersions))
        traceabilitySnapshotObject.BaseRunArtifactVersions = JsonUtility.FromString<IList<ArtifactVersion>>(snapshotData.BaseRunArtifactVersions);
      if (!string.IsNullOrWhiteSpace(snapshotData.BaseRunDetails))
        traceabilitySnapshotObject.BaseRunDetails = JsonUtility.FromString<PipelineTraceabilityBaseRunDetails>(snapshotData.BaseRunDetails);
      return traceabilitySnapshotObject;
    }
  }
}
