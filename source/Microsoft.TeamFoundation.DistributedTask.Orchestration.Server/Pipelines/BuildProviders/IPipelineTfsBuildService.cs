// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders.IPipelineTfsBuildService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders
{
  [DefaultServiceImplementation(typeof (FrameworkTfsBuildService))]
  public interface IPipelineTfsBuildService : IVssFrameworkService
  {
    int GetDefinitionId(IVssRequestContext requestContext, string project, string definitionName);

    PipelineInfo GetLatestPipelineInfo(
      IVssRequestContext requestContext,
      string project,
      string definition,
      string branch,
      IList<string> tags);

    PipelineDefinitionReference GetPipelineDefinition(
      IVssRequestContext requestContext,
      TeamProjectReference project,
      int definitionId);

    PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      string project,
      string definition,
      string pipelineNumber);

    PipelineInfo GetPipelineInfo(IVssRequestContext requestContext, Guid projectId, int pipelineId);

    PipelineInfo QueuePipeline(
      IVssRequestContext requestContext,
      ProjectInfo project,
      Microsoft.TeamFoundation.Build.WebApi.Build buildToQueue);
  }
}
