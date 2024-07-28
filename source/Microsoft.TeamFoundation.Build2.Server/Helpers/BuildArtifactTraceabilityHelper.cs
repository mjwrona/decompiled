// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Helpers.BuildArtifactTraceabilityHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build2.Server.Helpers
{
  internal class BuildArtifactTraceabilityHelper
  {
    public static void TracePublishedArtifact(
      IVssRequestContext requestContext,
      BuildData build,
      BuildArtifact artifact)
    {
      ArtifactTraceabilityData artifactTraceabilityData = (ArtifactTraceabilityData) null;
      try
      {
        if (build == null || artifact == null)
          throw new ArgumentNullException("Invalid input value while saving traceability data for published artifacts");
        artifactTraceabilityData = new ArtifactTraceabilityData()
        {
          ArtifactCategory = ArtifactCategory.Pipeline,
          ArtifactType = "Pipeline",
          ArtifactName = artifact.Name,
          ArtifactVersionId = build.Id.ToString(),
          ArtifactVersionName = build.BuildNumber,
          ProjectId = build.ProjectId,
          PipelineDefinitionId = build.Definition.Id,
          ResourcePipelineDefinitionId = build.Definition.Id,
          PipelineRunId = build.Id,
          IsSelfArtifact = true
        };
        artifactTraceabilityData.ResourceProperties.Add(PipelinePropertyNames.ProjectId, build.ProjectId.ToString());
        artifactTraceabilityData.ResourceProperties.Add(PipelinePropertyNames.DefinitionId, build.Definition.Id.ToString());
        artifactTraceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ProjectId, build.ProjectId.ToString());
        artifactTraceabilityData.JobId = !string.IsNullOrEmpty(artifact.Source) ? artifact.Source : throw new MissingMemberException("Artifact source is missing in BuildArtifact");
        requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService).AddArtifactTraceabilityForPublishedArtifact(requestContext, artifactTraceabilityData);
      }
      catch (Exception ex)
      {
        string format = "Exception occured while saving traceability data. " + string.Format("Details: build is null?: {0}, ", (object) (build == null)) + string.Format("artifact is null?: {0}, traceabilityData : {1} ", (object) (artifact == null), (object) artifactTraceabilityData?.ToString()) + "Exception : " + ex.ToString();
        requestContext.TraceAlways(12030249, TraceLevel.Error, "Build2", ArtifactTraceabilityConstants.TraceLayer, format);
      }
    }
  }
}
