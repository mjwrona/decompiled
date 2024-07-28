// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CorePipelineStage`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public abstract class CorePipelineStage<TId, TDoc> : CorePipelineStage
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    protected TraceMetaData TraceMetaData { get; }

    protected CorePipelineStage(TraceMetaData traceMetaData)
      : this()
    {
      this.TraceMetaData = traceMetaData;
    }

    protected CorePipelineStage()
    {
    }

    public virtual void ResetState()
    {
    }

    public abstract void Run(CorePipelineContext<TId, TDoc> pipelineContext);

    public abstract IReadOnlyCollection<PipelineDocumentState> ExpectedDocumentStates { get; }

    public virtual void HandleDocumentFlowErrors(
      CorePipelineContext<TId, TDoc> pipelineContext,
      IReadOnlyCollection<TDoc> errorDocs)
    {
    }
  }
}
