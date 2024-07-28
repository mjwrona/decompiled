// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.V2Jobs.Operations.Code.Repository.TfvcDestroyOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.V2Jobs.Operations.Code.Repository
{
  internal class TfvcDestroyOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080136, "Indexing Pipeline", "IndexingOperation");

    public TfvcDestroyOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(TfvcDestroyOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        string normalizedString = this.IndexingUnit.GetTfsEntityIdAsNormalizedString();
        string destroyPath = ((TfvcDestroyEventData) this.IndexingUnitChangeEvent.ChangeData).DestroyPath;
        if (this.IndexingUnit != null)
        {
          IndexingProperties properties = this.IndexingUnit.Properties;
          if (!(this.IndexingUnit.TFSEntityAttributes is TfvcCodeRepoTFSAttributes entityAttributes))
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("TFVC repository attributes is not as expected, found {0} in place of {1}", (object) this.IndexingUnit.TFSEntityAttributes, (object) typeof (TfvcCodeRepoTFSAttributes))));
          DocumentContractType contractType = executionContext.ProvisioningContext.ContractType;
          StringComparison pathStringComparer = FileAttributes.GetTfvcFilePathStringComparer(contractType);
          if (!properties.IsDisabled && !string.IsNullOrWhiteSpace(destroyPath))
          {
            if (!destroyPath.StartsWith(entityAttributes.RepositoryName.TrimEnd('/') + CommonConstants.DirectorySeparatorString, pathStringComparer))
            {
              if (!destroyPath.Equals(entityAttributes.RepositoryName.TrimEnd('/'), pathStringComparer))
                goto label_15;
            }
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Attempting to destroy tfvc item/items represented by path: [{0}], ", (object) destroyPath)));
            CodeFileContract contract = AbstractSearchDocumentContract.CreateContract(contractType) as CodeFileContract;
            string str = contract.CorrectFilePath(destroyPath);
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Normalized path: [{0}], ", (object) str)));
            BulkDeleteByQueryRequest<AbstractSearchDocumentContract> bulkDeleteByQueryRequest = new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>()
            {
              ContractType = contractType,
              Query = (IExpression) new AndExpression(new IExpression[2]
              {
                (IExpression) new TermExpression(contract.GetSearchFieldForType(CodeFileContract.CodeContractQueryableElement.FilePath.InlineFilterName()), Operator.Equals, str),
                (IExpression) new TermExpression(contract.GetSearchFieldForType(CodeFileContract.CodeContractQueryableElement.RepositoryId.InlineFilterName()), Operator.Equals, normalizedString)
              }),
              RequestTimeOut = TimeSpan.FromSeconds((double) executionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec)
            };
            IndexOperationsResponse operationsResponse = this.GetIndex(executionContext).BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) executionContext, bulkDeleteByQueryRequest, false);
            if (operationsResponse == null)
              throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("received null IndexOperationsResponse")));
            if (!operationsResponse.Success)
              throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("{0} PerformBulkDelete Failed.", (object) this.GetType().Name)));
            if (operationsResponse.IsOperationIncomplete)
            {
              this.RequeueOperation(executionContext);
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially deleted documents. Re-queued the destroy operation for Repo {0}, and path {1}. ", (object) this.IndexingUnit, (object) str)));
              operationResult.Status = OperationStatus.PartiallySucceeded;
            }
            else
            {
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully destroyed tfvc item / items represented by for Repo {0}, and path {1}, ", (object) this.IndexingUnit, (object) str)));
              operationResult.Status = OperationStatus.Succeeded;
            }
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} IndexOperationsResponse: {1}", (object) nameof (TfvcDestroyOperation), (object) operationsResponse.ToString())));
            goto label_18;
          }
label_15:
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("TFVC repository is either disabled or name doesn't match with destroy path, repository name {0}, destroy path {1}", (object) entityAttributes.RepositoryName, (object) destroyPath)));
        }
      }
      catch (Exception ex)
      {
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} failed for repository Id [{1}, ", (object) nameof (TfvcDestroyOperation), (object) this.IndexingUnit.GetTfsEntityIdAsNormalizedString())) + FormattableString.Invariant(FormattableStringFactory.Create("Destroy path received {0}, ", (object) ((TfvcDestroyEventData) this.IndexingUnitChangeEvent.ChangeData).DestroyPath)) + FormattableString.Invariant(FormattableStringFactory.Create("Exception -> {0}", (object) ex.ToString())));
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(TfvcDestroyOperation.s_traceMetaData, nameof (RunOperation));
      }
label_18:
      return operationResult;
    }

    internal virtual ISearchIndex GetIndex(IndexingExecutionContext executionContext) => executionContext.ProvisioningContext.SearchPlatform.GetIndex(executionContext.GetIndex());

    private void RequeueOperation(IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "Destroy",
        ChangeData = (ChangeEventData) new TfvcDestroyEventData((ExecutionContext) executionContext)
        {
          DestroyPath = ((TfvcDestroyEventData) this.IndexingUnitChangeEvent.ChangeData).DestroyPath
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }
  }
}
