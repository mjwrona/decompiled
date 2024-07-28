// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders.IPipelineProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders
{
  [InheritedExport]
  public interface IPipelineProvider
  {
    PipelineInfo GetLatestPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      ServiceEndpoint endpoint);

    PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      string pipelineNumber);

    void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResource pipeline,
      ServiceEndpoint endpoint);
  }
}
