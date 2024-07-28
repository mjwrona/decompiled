// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.IOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public interface IOperationMapper
  {
    IEntityType SupportedEntityType { get; }

    int Weight { get; }

    IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnit indexingUnit);
  }
}
