// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemDeleteOperation : IndexingUnitDeleteOperation
  {
    private readonly IClassificationNodeDataAccess m_classificationNodeDataAccess;

    public ProjectWorkItemDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    protected ProjectWorkItemDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_classificationNodeDataAccess = dataAccessFactory.GetClassificationNodeDataAccess();
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080665, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult1 = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (coreIndexingExecutionContext.IndexingUnit.IsShadow)
        {
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Calling IndexingUnitDelete Operation on indexing unit for shadow project indexing unit: {0}.", (object) this.IndexingUnit)));
          base.RunOperation(coreIndexingExecutionContext);
        }
        else
        {
          IndexOperationsResponse operationsResponse = this.BulkDeleteDocumentsFromSearchPlatform(executionContext, resultMessage);
          if (!operationsResponse.Success)
            throw new SearchPlatformException("Work item project delete failed.");
          if (!operationsResponse.IsOperationIncomplete)
          {
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully deleted documents for project [{0}]. ", (object) this.IndexingUnit)));
            this.DeleteAllCssNodesOfThisProject((ExecutionContext) coreIndexingExecutionContext, resultMessage);
            OperationResult operationResult2 = base.RunOperation(coreIndexingExecutionContext);
            resultMessage.Append(operationResult2.Message);
          }
          else
          {
            this.RequeueOperation(executionContext);
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Work item project delete completed partially. Re-queued the operation for project [{0}]. ", (object) this.IndexingUnit)));
          }
        }
        operationResult1.Status = OperationStatus.Succeeded;
      }
      catch (Exception ex)
      {
        if (coreIndexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && (int) this.IndexingUnitChangeEvent.AttemptCount >= this.m_maxIndexRetryCount)
        {
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("ProjectWorkItemDeleteOperation for Project [{0}] failed. Going ahead with partition DB cleanup as attempt count threshold has reached. ", (object) this.IndexingUnit)));
          this.DeleteAllCssNodesOfThisProject((ExecutionContext) coreIndexingExecutionContext, resultMessage);
          OperationResult operationResult3 = base.RunOperation(coreIndexingExecutionContext);
          resultMessage.Append(operationResult3.Message);
          operationResult1.Status = OperationStatus.Failed;
        }
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      finally
      {
        operationResult1.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080665, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult1;
    }

    internal override bool ShouldSkipOperation(
      IVssRequestContext requestContext,
      out string reasonToSkipOperation)
    {
      reasonToSkipOperation = string.Empty;
      return false;
    }

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.IndexingUnitChangeEvent.ChangeData.Trigger == 9 ? new TimeSpan(0L) : base.GetChangeEventDelay(executionContext, e);
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent RequeueOperation(
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

    internal virtual IndexOperationsResponse BulkDeleteDocumentsFromSearchPlatform(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      TermExpression query = new TermExpression("projectId", Operator.Equals, this.IndexingUnit.GetTfsEntityIdAsNormalizedString());
      IndexIdentity index1 = executionContext.GetIndex();
      ISearchIndex index2 = executionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
      int contractType = (int) executionContext.ProvisioningContext.ContractType;
      TimeSpan requestTimeOut = TimeSpan.FromSeconds((double) executionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec);
      BulkDeleteByQueryRequest<AbstractSearchDocumentContract> bulkDeleteByQueryRequest = new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>((IExpression) query, (DocumentContractType) contractType, requestTimeOut)
      {
        Lenient = true
      };
      IndexOperationsResponse operationsResponse = index2.BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) executionContext, bulkDeleteByQueryRequest, false);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "DeleteDocumentsByQuery", (object) operationsResponse)));
      return operationsResponse;
    }

    private void DeleteAllCssNodesOfThisProject(
      ExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      IEnumerable<ClassificationNode> source = this.m_classificationNodeDataAccess.GetClassificationNodes(executionContext.RequestContext, -1).Where<ClassificationNode>((Func<ClassificationNode, bool>) (n => n.ProjectId == this.IndexingUnit.TFSEntityId));
      int num = 0;
      if (source.Any<ClassificationNode>())
        num = this.m_classificationNodeDataAccess.DeleteClassificationNodes(executionContext.RequestContext, source.Select<ClassificationNode, int>((Func<ClassificationNode, int>) (n => n.Id)).ToList<int>());
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleted [{0}] CSS nodes from DB. ", (object) num)));
    }
  }
}
