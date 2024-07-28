// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CorePipelineContext`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class CorePipelineContext<TId, TDoc> : CorePipelineContext, IDisposable
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    private bool m_disposedValue;

    public CorePipelineContext(
      IndexingUnit indexingUnit,
      CoreIndexingExecutionContext indexingExecutionContext)
      : this(indexingUnit, indexingExecutionContext, TimeSpan.MinValue)
    {
    }

    public CorePipelineContext(
      IndexingUnit indexingUnit,
      CoreIndexingExecutionContext indexingExecutionContext,
      TimeSpan maxPipelineExecutionTime)
      : base(indexingUnit, indexingExecutionContext, maxPipelineExecutionTime)
    {
      this.PipelineDocs = new PipelineDocumentCollection<TId, TDoc>(indexingExecutionContext.RequestContext);
      GlobalPipelineContext.Set((CorePipelineContext) this);
    }

    public CorePipelineStage<TId, TDoc> CurrentStage { get; set; }

    public PipelineDocumentCollection<TId, TDoc> PipelineDocs { get; private set; }

    protected override void Dispose(bool disposing)
    {
      if (!this.m_disposedValue)
      {
        if (disposing)
          GlobalPipelineContext.Clear<TId, TDoc>();
        this.m_disposedValue = true;
        this.PipelineDocs = (PipelineDocumentCollection<TId, TDoc>) null;
      }
      base.Dispose(disposing);
    }
  }
}
