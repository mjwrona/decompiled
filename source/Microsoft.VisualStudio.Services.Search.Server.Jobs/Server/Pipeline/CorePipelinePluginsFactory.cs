// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CorePipelinePluginsFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal static class CorePipelinePluginsFactory
  {
    internal static CorePipelineFlowHandler GetPipelineFlowHandler(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      TraceMetaData traceMetaData)
    {
      IEnumerable<ICorePipelineFlowHandlerCreator> pipelineFlowCreators = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ICorePipelineService>().GetPipelineFlowCreators();
      CorePipelineFlowHandler pipelineFlowHandler = new CorePipelineFlowHandler(indexingUnit, traceMetaData);
      foreach (ICorePipelineFlowHandlerCreator flowHandlerCreator in pipelineFlowCreators)
      {
        pipelineFlowHandler = flowHandlerCreator.GetPipelineFlowHandler(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent, traceMetaData);
        if (pipelineFlowHandler != null)
          break;
      }
      return pipelineFlowHandler;
    }

    internal static ICorePipelineFailureHandler GetPipelineFailureHandler(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IEnumerable<ICorePipelineFailureHandler> pipelineFailureHandlers = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ICorePipelineService>().GetPipelineFailureHandlers();
      int num = -1;
      ICorePipelineFailureHandler pipelineFailureHandler1 = (ICorePipelineFailureHandler) null;
      foreach (ICorePipelineFailureHandler pipelineFailureHandler2 in pipelineFailureHandlers)
      {
        if (pipelineFailureHandler2.SupportedEntityType.Equals((object) coreIndexingExecutionContext.IndexingUnit.EntityType) && pipelineFailureHandler2.Weight > num)
        {
          pipelineFailureHandler1 = pipelineFailureHandler2;
          num = pipelineFailureHandler2.Weight;
        }
      }
      return pipelineFailureHandler1;
    }

    internal static IOperationMapper GetOperationMapper(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IEnumerable<IOperationMapper> operationMappers = coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ICorePipelineService>().GetOperationMappers();
      int num = -1;
      IOperationMapper operationMapper1 = (IOperationMapper) null;
      foreach (IOperationMapper operationMapper2 in operationMappers)
      {
        if (operationMapper2.SupportedEntityType.Equals((object) coreIndexingExecutionContext.IndexingUnit.EntityType) && operationMapper2.Weight > num)
        {
          operationMapper1 = operationMapper2;
          num = operationMapper2.Weight;
        }
      }
      return operationMapper1;
    }

    internal static CorePipelineStage<TId, TDoc> GetPipelineStage<TId, TDoc>(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      string stageName,
      params object[] args)
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>
    {
      return coreIndexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ICorePipelineService>().GetPipelineStage<TId, TDoc>(coreIndexingExecutionContext, stageName, args);
    }
  }
}
