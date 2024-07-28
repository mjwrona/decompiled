// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackageCleanupFeedsOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal class CollectionPackageCleanupFeedsOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080743, "Indexing Pipeline", "IndexingOperation");

    public CollectionPackageCleanupFeedsOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackageCleanupFeedsOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsToBeDeleted = FeedIndexingUnitHelper.GetFeedIndexingUnitsToBeDeleted((CoreIndexingExecutionContext) indexingExecutionContext, this.IndexingUnit);
        this.DeleteFeedIndexingUnits(indexingExecutionContext, unitsToBeDeleted, resultMessage);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackageCleanupFeedsOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual void DeleteFeedIndexingUnits(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnitsToBeDeleted,
      StringBuilder resultMessage)
    {
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in feedIndexingUnitsToBeDeleted)
      {
        this.BulkDeleteFeedPackageDocumentsFromSearchPlatform(indexingExecutionContext, indexingUnit, resultMessage);
        indexingUnit.Properties.IsDisabled = true;
        this.IndexingUnitDataAccess.DeleteIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
        this.IndexingUnitDataAccess.DeleteIndexingUnitsPermanently(indexingExecutionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
        {
          indexingUnit
        });
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleted Feed {0} and removed data from ES", (object) indexingUnit.TFSEntityId)));
      }
    }

    internal virtual void BulkDeleteFeedPackageDocumentsFromSearchPlatform(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnit,
      StringBuilder resultMessage)
    {
      string str = feedIndexingUnit.TFSEntityId.ToString();
      IExpression query = (IExpression) new TermExpression("feedId", Operator.Equals, str);
      IndexIdentity index1 = indexingExecutionContext.GetIndex();
      if (index1 == null || string.IsNullOrWhiteSpace(index1.Name))
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} Index information is not available for feed {1}. So, skipping the BulkDeleteFeedPackageDocumentsFromSearchPlatform.", (object) "DeleteDocumentsByQuery", (object) str)));
      }
      else
      {
        ISearchIndex index2 = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
        BulkDeleteByQueryRequest<AbstractSearchDocumentContract> deleteByQueryRequest = new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>(query, indexingExecutionContext.ProvisioningContext.ContractType, TimeSpan.FromSeconds((double) indexingExecutionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec));
        IndexingExecutionContext executionContext = indexingExecutionContext;
        BulkDeleteByQueryRequest<AbstractSearchDocumentContract> bulkDeleteByQueryRequest = deleteByQueryRequest;
        IndexOperationsResponse operationsResponse = index2.BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) executionContext, bulkDeleteByQueryRequest, false);
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "DeleteDocumentsByQuery", (object) operationsResponse)));
      }
    }
  }
}
