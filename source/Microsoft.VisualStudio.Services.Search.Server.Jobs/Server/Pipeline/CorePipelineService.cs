// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CorePipelineService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class CorePipelineService : ICorePipelineService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<ICorePipelineFlowHandlerCreator> m_pipelineFlowHandlerCreators;
    private IDisposableReadOnlyList<ICorePipelineFailureHandler> m_pipelineFailureHandlers;
    private IDisposableReadOnlyList<IOperationMapper> m_operationMappers;
    private IDisposableReadOnlyList<CorePipelineStage> m_pipelineStages;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_pipelineFlowHandlerCreators = SearchPlatformHelper.GetExtensions<ICorePipelineFlowHandlerCreator>(systemRequestContext);
      this.m_pipelineFailureHandlers = SearchPlatformHelper.GetExtensions<ICorePipelineFailureHandler>(systemRequestContext);
      this.m_operationMappers = SearchPlatformHelper.GetExtensions<IOperationMapper>(systemRequestContext);
      this.m_pipelineStages = SearchPlatformHelper.GetExtensions<CorePipelineStage>(systemRequestContext);
      this.VerifyStagesHaveUniqueNames();
    }

    private void VerifyStagesHaveUniqueNames()
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (CorePipelineStage pipelineStage in (IEnumerable<CorePipelineStage>) this.m_pipelineStages)
      {
        if (!stringSet.Add(pipelineStage.Name))
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Multiple stages with the same name [{0}] found", (object) pipelineStage.Name)));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      CorePipelineService.Dispose<ICorePipelineFlowHandlerCreator>(ref this.m_pipelineFlowHandlerCreators);
      CorePipelineService.Dispose<ICorePipelineFailureHandler>(ref this.m_pipelineFailureHandlers);
      CorePipelineService.Dispose<IOperationMapper>(ref this.m_operationMappers);
      CorePipelineService.Dispose<CorePipelineStage>(ref this.m_pipelineStages);
    }

    public IEnumerable<ICorePipelineFlowHandlerCreator> GetPipelineFlowCreators() => (IEnumerable<ICorePipelineFlowHandlerCreator>) this.m_pipelineFlowHandlerCreators;

    public IEnumerable<ICorePipelineFailureHandler> GetPipelineFailureHandlers() => (IEnumerable<ICorePipelineFailureHandler>) this.m_pipelineFailureHandlers;

    public IEnumerable<IOperationMapper> GetOperationMappers() => (IEnumerable<IOperationMapper>) this.m_operationMappers;

    public CorePipelineStage<TId, TDoc> GetPipelineStage<TId, TDoc>(
      CoreIndexingExecutionContext iexContext,
      string stageName,
      params object[] args)
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>
    {
      foreach (CorePipelineStage pipelineStage in (IEnumerable<CorePipelineStage>) this.m_pipelineStages)
      {
        if (stageName.Equals(pipelineStage.Name, StringComparison.OrdinalIgnoreCase))
        {
          object instance = Activator.CreateInstance(pipelineStage.GetType(), args);
          return instance is CorePipelineStage<TId, TDoc> corePipelineStage ? corePipelineStage : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Expected plugin [{0}] to be of type [{1}] but found [{2}]", (object) pipelineStage.Name, (object) typeof (CorePipelineStage<TId, TDoc>), (object) instance.GetType())));
        }
      }
      throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Stage with name [{0}] not found", (object) stageName)));
    }

    private static void Dispose<T>(ref IDisposableReadOnlyList<T> plugins)
    {
      if (plugins == null)
        return;
      plugins.Dispose();
      plugins = (IDisposableReadOnlyList<T>) null;
    }
  }
}
