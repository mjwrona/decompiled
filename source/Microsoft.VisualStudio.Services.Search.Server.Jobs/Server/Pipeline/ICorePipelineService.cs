// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.ICorePipelineService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [DefaultServiceImplementation(typeof (CorePipelineService))]
  public interface ICorePipelineService : IVssFrameworkService
  {
    IEnumerable<ICorePipelineFlowHandlerCreator> GetPipelineFlowCreators();

    IEnumerable<ICorePipelineFailureHandler> GetPipelineFailureHandlers();

    IEnumerable<IOperationMapper> GetOperationMappers();

    CorePipelineStage<TId, TDoc> GetPipelineStage<TId, TDoc>(
      CoreIndexingExecutionContext iexContext,
      string stageName,
      params object[] args)
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>;
  }
}
