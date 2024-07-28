// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.IArtifactTraceabilityService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  [DefaultServiceImplementation(typeof (FrameworkArtifactTraceabilityService))]
  [InheritedExport]
  public interface IArtifactTraceabilityService : IVssFrameworkService
  {
    void SavingArtifactTraceabilityDataForPipelineRunCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId);

    void AddArtifactTraceabilityForRepositoryResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData);

    void AddArtifactTraceabilityForDownloadTask(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData);

    void AddArtifactTraceabilityForPipelineResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData);

    void AddArtifactTraceabilityForContainerResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      ContainerResource containerResource,
      PipelineEnvironment pipelineEnvironment);

    void AddArtifactTraceabilityForContainerResourceAtJobLevel(
      IVssRequestContext requestContext,
      Guid planId,
      string planType,
      Guid scopeId,
      IList<string> imageUriList,
      string jobId,
      string jobName);

    int GetArtifactsCountForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true);

    IList<ArtifactVersion> GetArtifactTraceabilityDataForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true);

    IList<CDPipelineRunData> GetCDPipelinesRunsDataForCIPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      int paginationLimit,
      int continuationToken);

    IList<CDPipelineRunData> GetPipelineRunsUsingExistingPipelineRun(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      Guid existingPipelineProjectId,
      int existingPipelineDefinitionId,
      int existingPipelineRunId);

    IList<ArtifactVersion> GetArtifactTraceabilityDataForJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      string jobId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true);

    void AddArtifactTraceabilityForPublishedArtifact(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData);

    string GetServiceName();

    IArtifactTraceabilityService GetArtifactTraceabilityService(string name);
  }
}
