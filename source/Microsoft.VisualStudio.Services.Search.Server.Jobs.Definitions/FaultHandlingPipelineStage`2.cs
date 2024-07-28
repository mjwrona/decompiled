// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.FaultHandlingPipelineStage`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public abstract class FaultHandlingPipelineStage<TId, TDoc> : CorePipelineStage<TId, TDoc>
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    protected FaultHandlingPipelineStage(TraceMetaData traceMetaData)
      : base(traceMetaData)
    {
    }

    protected FaultHandlingPipelineStage()
    {
    }

    public override sealed void Run(CorePipelineContext<TId, TDoc> pipelineContext)
    {
      try
      {
        this.RunInternal(pipelineContext);
      }
      catch (Exception ex) when (FaultHandlingPipelineStage<TId, TDoc>.RecordException(pipelineContext, ex))
      {
      }
    }

    private static bool RecordException(CorePipelineContext<TId, TDoc> pipelineContext, Exception e)
    {
      pipelineContext.IndexingExecutionContext.FaultService.SetError(e);
      return false;
    }

    public abstract void RunInternal(CorePipelineContext<TId, TDoc> pipelineContext);
  }
}
