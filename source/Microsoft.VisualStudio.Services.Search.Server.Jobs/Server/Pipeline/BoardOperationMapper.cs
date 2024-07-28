// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.BoardOperationMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Board;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (IOperationMapper))]
  public class BoardOperationMapper : IOperationMapper
  {
    public IEntityType SupportedEntityType => (IEntityType) BoardEntityType.GetInstance();

    public virtual int Weight => 0;

    public virtual IRunnable<OperationResult> GetOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnit indexingUnit)
    {
      if (indexingUnit.IndexingUnitType == "Collection")
      {
        switch (indexingUnitChangeEvent.ChangeType)
        {
          case "CrawlMetadata":
            return (IRunnable<OperationResult>) new CollectionBoardMetadataCrawlOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          case "BeginBulkIndex":
            return (IRunnable<OperationResult>) new CollectionBoardBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          case "CompleteBulkIndex":
            return (IRunnable<OperationResult>) new CollectionBoardIndexFinalizeOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
          case "UpdateMetadata":
            return (IRunnable<OperationResult>) new UpdateMetadataOperation((ExecutionContext) coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent);
        }
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Operation class for Indexing Unit Kind: {0}, Indexing Unit Type: {1}, ChangeType: {2}", (object) indexingUnit.EntityType, (object) indexingUnit.IndexingUnitType, (object) indexingUnitChangeEvent.ChangeType)));
    }
  }
}
