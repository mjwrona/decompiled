// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.FrameworkArtifactTraceabilityService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public sealed class FrameworkArtifactTraceabilityService : 
    IArtifactTraceabilityService,
    IVssFrameworkService
  {
    private IDictionary<string, IArtifactTraceabilityService> m_AllTraceabilityExtensions;

    public void SavingArtifactTraceabilityDataForPipelineRunCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForRepositoryResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForPipelineResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForContainerResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      ContainerResource containerResource,
      PipelineEnvironment pipelineEnvironment)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForContainerResourceAtJobLevel(
      IVssRequestContext requestContext,
      Guid planId,
      string planType,
      Guid scopeId,
      IList<string> imageUriList,
      string jobId,
      string jobName)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForDownloadTask(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      throw new NotImplementedException();
    }

    public int GetArtifactsCountForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      throw new NotImplementedException();
    }

    public IList<ArtifactVersion> GetArtifactTraceabilityDataForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      throw new NotImplementedException();
    }

    public IList<ArtifactVersion> GetArtifactTraceabilityDataForJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      string jobId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      throw new NotImplementedException();
    }

    public IList<CDPipelineRunData> GetCDPipelinesRunsDataForCIPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      int paginationLimit,
      int continuationToken)
    {
      throw new NotImplementedException();
    }

    public void AddArtifactTraceabilityForPublishedArtifact(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      throw new NotImplementedException();
    }

    public string GetServiceName() => string.Empty;

    public IArtifactTraceabilityService GetArtifactTraceabilityService(string name)
    {
      IArtifactTraceabilityService traceabilityService;
      if (this.m_AllTraceabilityExtensions != null && this.m_AllTraceabilityExtensions.TryGetValue(name, out traceabilityService))
        return traceabilityService;
      throw new ServiceExtensionNotFoundException(TaskResources.ServiceExtensionNotFound((object) name));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      using (IDisposableReadOnlyList<IArtifactTraceabilityService> extensions = systemRequestContext.GetExtensions<IArtifactTraceabilityService>())
        this.m_AllTraceabilityExtensions = (IDictionary<string, IArtifactTraceabilityService>) extensions.ToDictionary<IArtifactTraceabilityService, string>((Func<IArtifactTraceabilityService, string>) (x => x.GetServiceName()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_AllTraceabilityExtensions = (IDictionary<string, IArtifactTraceabilityService>) null;

    public IList<CDPipelineRunData> GetPipelineRunsUsingExistingPipelineRun(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      Guid existingPipelineProjectId,
      int existingPipelineDefinitionId,
      int existingPipelineRunId)
    {
      throw new NotImplementedException();
    }
  }
}
