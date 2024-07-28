// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ReindexingValidatorBase
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class ReindexingValidatorBase
  {
    protected const string TraceArea = "Indexing Pipeline";
    protected const string TraceLayer = "IndexingOperation";
    protected readonly TraceMetaData TraceMetadata = new TraceMetaData(1083120, "Indexing Pipeline", "IndexingOperation");

    public ReindexingValidatorBase(IndexingExecutionContext executionContext) => this.IndexingExecutionContext = executionContext;

    protected IndexingExecutionContext IndexingExecutionContext { get; private set; }

    public virtual bool ValidateReindexingCompleteness(StringBuilder result)
    {
      result.Append("No validations are performed.");
      return true;
    }
  }
}
