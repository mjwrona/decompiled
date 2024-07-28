// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Migration.ExternalPipelinesService
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
  public class ExternalPipelinesService : IExternalPipelinesService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Pipeline CreatePipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      CreatePipelineParameters parameters)
    {
      using (IDisposableReadOnlyList<IExternalPipelineProvider> extensions = requestContext.GetExtensions<IExternalPipelineProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalPipelineProvider>) extensions).CreatePipeline(requestContext, projectId, parameters);
    }

    public Pipeline GetPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineRevision = null)
    {
      using (IDisposableReadOnlyList<IExternalPipelineProvider> extensions = requestContext.GetExtensions<IExternalPipelineProvider>())
        return this.GetSingleProvider((IReadOnlyList<IExternalPipelineProvider>) extensions).GetPipeline(requestContext, projectId, pipelineId, pipelineRevision);
    }

    public IReadOnlyList<Pipeline> GetPipelines(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryPipelinesParameters queryParameters,
      PipelineOrderByExpression orderBy,
      int count)
    {
      using (IDisposableReadOnlyList<IExternalPipelineProvider> extensions = requestContext.GetExtensions<IExternalPipelineProvider>())
        return (IReadOnlyList<Pipeline>) ((object) this.GetSingleProvider((IReadOnlyList<IExternalPipelineProvider>) extensions).GetPipelines(requestContext, projectId, queryParameters, orderBy, count) ?? (object) Array.Empty<Pipeline>());
    }

    private IExternalPipelineProvider GetSingleProvider(
      IReadOnlyList<IExternalPipelineProvider> providers)
    {
      return providers.SingleOrDefault<IExternalPipelineProvider>() ?? throw new NotImplementedException(PipelinesServerResources.ExternalPipelinesNotSupportedError());
    }
  }
}
