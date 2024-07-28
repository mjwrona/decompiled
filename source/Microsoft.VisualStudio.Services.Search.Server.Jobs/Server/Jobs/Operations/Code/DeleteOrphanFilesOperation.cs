// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.DeleteOrphanFilesOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal abstract class DeleteOrphanFilesOperation : AbstractIndexingOperation
  {
    public DeleteOrphanFilesOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1080621, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultmessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        IndexOperationsResponse operationsResponse = this.PerformBulkDelete(executionContext, resultmessage);
        if (operationsResponse != null)
        {
          if (!operationsResponse.Success)
            throw new SearchPlatformException("Documents delete failed.");
          if (operationsResponse.IsOperationIncomplete)
          {
            this.RequeueOperation(executionContext);
            resultmessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("DeleteOrphanFilesOperation completed partially. Re-queued the operation for [{0}]. ", (object) this.IndexingUnit)));
          }
          else
            resultmessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("DeleteOrphanFilesOperation completed succesfully for {0} ", (object) this.IndexingUnit)));
          operationResult.Status = OperationStatus.Succeeded;
        }
        else
          resultmessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("DeleteOrphanFilesOperation for Repo {0} failed due to null IndexOperationsResponse. ", (object) this.IndexingUnit)));
      }
      finally
      {
        operationResult.Message = resultmessage.ToString();
        Tracer.TraceLeave(1080621, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected IndexOperationsMetadata CreateIndexOperationsMetadata(
      string repositoryId,
      string entityName,
      string unitType)
    {
      IndexOperationsMetadata operationsMetadata = new IndexOperationsMetadata();
      operationsMetadata.RepositoryId = repositoryId;
      switch (unitType)
      {
        case "Git_Repository":
          operationsMetadata.RepositoryName = entityName;
          break;
        case "Project":
          operationsMetadata.ProjectName = entityName;
          break;
        case "TFVC_Repository":
          operationsMetadata.RepositoryName = entityName;
          break;
        case "CustomRepository":
          operationsMetadata.RepositoryName = entityName;
          break;
        case "Collection":
          operationsMetadata.CollectionName = entityName;
          break;
      }
      return operationsMetadata;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent RequeueOperation(
      IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.IndexingUnitChangeEvent.ChangeType,
        ChangeData = this.IndexingUnitChangeEvent.ChangeData,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    protected abstract IndexOperationsResponse PerformBulkDelete(
      IndexingExecutionContext executionContext,
      StringBuilder resultmessage);
  }
}
