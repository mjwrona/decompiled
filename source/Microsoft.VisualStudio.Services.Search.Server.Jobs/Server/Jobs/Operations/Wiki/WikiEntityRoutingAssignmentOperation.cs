// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiEntityRoutingAssignmentOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiEntityRoutingAssignmentOperation : BasicRoutingAssignmentOperation
  {
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    private readonly TraceMetaData m_traceMetaData;

    public WikiEntityRoutingAssignmentOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_traceMetaData = new TraceMetaData(1080292, "Indexing Pipeline", "IndexingOperation");
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionWikiFinalizeHelper();
  }
}
