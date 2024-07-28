// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.DeploymentDetailsService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal sealed class DeploymentDetailsService : 
    BaseMetadataDetailsService,
    IBaseMetadataDetailsService,
    IVssFrameworkService
  {
    public override void AddMetadataDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      BaseMetadataDetails details)
    {
      this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
      DeploymentDetails deploymentDetails = (DeploymentDetails) details;
      if (ArtifactMetadataHelper.IsPublishPipelineMetadataEnabled(requestContext, projectId))
      {
        IArtifactMetadataService service = requestContext.GetService<IArtifactMetadataService>();
        foreach (Grafeas.V1.Note deploymentNote in deploymentDetails.ToDeploymentNotes(projectId))
        {
          try
          {
            service.CreateNote(requestContext, deploymentNote);
          }
          catch (NoteExistsException ex)
          {
          }
        }
        foreach (Grafeas.V1.Occurrence deploymentOccurrence in deploymentDetails.ToDeploymentOccurrences(projectId))
          service.CreateOccurrence(requestContext, deploymentOccurrence);
      }
      PipelineRunInfo pipelineRunInfo = deploymentDetails.PipelineRunInfo;
      if (pipelineRunInfo == null || !(pipelineRunInfo.PlanId != Guid.Empty) || string.IsNullOrWhiteSpace(pipelineRunInfo.DeploymentPhaseIdentifier) || string.IsNullOrWhiteSpace(pipelineRunInfo.JobName) || string.IsNullOrWhiteSpace(pipelineRunInfo.HubName))
        return;
      requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService)?.AddArtifactTraceabilityForContainerResourceAtJobLevel(requestContext, pipelineRunInfo.PlanId, pipelineRunInfo.HubName, projectId, (IList<string>) details.ResourceUri, pipelineRunInfo.DeploymentPhaseIdentifier, pipelineRunInfo.JobName);
    }
  }
}
