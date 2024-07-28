// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders.PipelineProviderExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders
{
  public static class PipelineProviderExtensions
  {
    public static string ResolveVersion(
      this IPipelineProvider pipelineProvider,
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      ServiceEndpoint endpoint)
    {
      Guid projectId1 = projectId;
      if (!pipeline.Properties.TryGetValue<Guid?>(PipelinePropertyNames.ProjectId, out Guid? _))
      {
        projectId1 = PipelineArtifact.GetProjectId(requestContext, pipeline) ?? projectId;
        pipeline.Properties.Set<Guid>(PipelinePropertyNames.ProjectId, projectId1);
      }
      PipelineInfo pipelineInfo;
      if (string.IsNullOrEmpty(pipeline.Version))
      {
        pipelineInfo = pipelineProvider.GetLatestPipelineInfo(requestContext, projectId1, pipeline, endpoint);
        pipeline.Version = pipelineInfo != null ? pipelineInfo.PipelineNumber : throw new PipelineProviderException(TaskResources.UnableToResolveLatestPipelineVersion((object) pipeline.Alias));
      }
      else
      {
        pipelineInfo = pipelineProvider.GetPipelineInfo(requestContext, projectId1, pipeline, pipeline.Version);
        if (pipelineInfo == null)
          throw new PipelineProviderException(TaskResources.UnableToResolvePipelineVersion((object) pipeline.Version, (object) pipeline.Alias));
      }
      string enumerable1;
      if (!pipeline.Properties.TryGetValue<string>(PipelinePropertyNames.PipelineId, out enumerable1) || enumerable1.IsNullOrEmpty<char>())
        pipeline.Properties.Set<int>(PipelinePropertyNames.PipelineId, pipelineInfo.Id);
      string enumerable2;
      if (!pipeline.Properties.TryGetValue<string>(PipelinePropertyNames.DefinitionId, out enumerable2) || enumerable2.IsNullOrEmpty<char>())
        pipeline.Properties.Set<int>(PipelinePropertyNames.DefinitionId, pipelineInfo.DefinitionId);
      if (!pipeline.Properties.TryGetValue<IList<BuildArtifact>>(PipelinePropertyNames.Artifacts, out IList<BuildArtifact> _))
      {
        IList<BuildArtifact> buildArtifacts = PipelineArtifact.GetBuildArtifacts(requestContext, projectId1, pipelineInfo.Id);
        pipeline.Properties.Set<IList<BuildArtifact>>(PipelinePropertyNames.Artifacts, buildArtifacts);
      }
      return pipeline.Version;
    }
  }
}
