// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Migration.ExternalRunsService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Server.Migration
{
  internal class ExternalRunsService : IExternalRunsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Run GetRun(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      using (IDisposableReadOnlyList<IExternalRunProvider> extensions = requestContext.GetExtensions<IExternalRunProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalRunProvider>) extensions).GetRun(requestContext, projectId, pipelineId, runId);
    }

    public IReadOnlyList<Run> GetRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int count)
    {
      using (IDisposableReadOnlyList<IExternalRunProvider> extensions = requestContext.GetExtensions<IExternalRunProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalRunProvider>) extensions).GetRuns(requestContext, projectId, pipelineId, count);
    }

    public Run RunPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineVersion,
      RunPipelineParameters parameters)
    {
      using (IDisposableReadOnlyList<IExternalRunProvider> extensions = requestContext.GetExtensions<IExternalRunProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalRunProvider>) extensions).RunPipeline(requestContext, projectId, pipelineId, pipelineVersion, parameters);
    }

    public IReadOnlyList<Artifact> GetArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      string artifactName)
    {
      using (IDisposableReadOnlyList<IExternalRunProvider> extensions = requestContext.GetExtensions<IExternalRunProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalRunProvider>) extensions).GetArtifacts(requestContext, projectId, pipelineId, runId, artifactName);
    }

    private IExternalRunProvider GetSingleProvider(IReadOnlyList<IExternalRunProvider> providers) => providers.SingleOrDefault<IExternalRunProvider>() ?? throw new NotImplementedException(PipelinesServerResources.ExternalRunsNotSupportedError());
  }
}
