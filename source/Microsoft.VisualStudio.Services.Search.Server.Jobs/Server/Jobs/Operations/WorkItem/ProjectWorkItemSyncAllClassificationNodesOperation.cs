// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemSyncAllClassificationNodesOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemSyncAllClassificationNodesOperation : 
    CollectionWorkItemAreaNodeSecurityAcesSyncOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080670, "Indexing Pipeline", "IndexingOperation");
    private readonly WorkItemHttpClientWrapper m_workItemHttpClientWrapper;
    private readonly IClassificationNodeDataAccess m_classificationNodeDataAccess;

    public ProjectWorkItemSyncAllClassificationNodesOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new WorkItemHttpClientWrapper((ExecutionContext) executionContext, ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata), new ClassificationNodeSecurityAcesUpdater((ExecutionContext) executionContext))
    {
    }

    protected ProjectWorkItemSyncAllClassificationNodesOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      WorkItemHttpClientWrapper workItemHttpClientWrapper,
      ClassificationNodeSecurityAcesUpdater securityAcesUpdater)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory, securityAcesUpdater)
    {
      this.m_classificationNodeDataAccess = dataAccessFactory.GetClassificationNodeDataAccess();
      this.m_workItemHttpClientWrapper = workItemHttpClientWrapper;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        Guid tfsEntityId = this.IndexingUnit.TFSEntityId;
        List<ClassificationNode> tfsSyncedCssNodes = this.FetchTfsClassificationNodes(tfsEntityId);
        List<ClassificationNode> searchSyncedCssNodes = this.FetchSearchClassificationNodes(executionContext, tfsEntityId);
        IndexOperationsResponse operationsResponse = this.HandleMissingAndStaleClassificationNodes(executionContext, tfsSyncedCssNodes, searchSyncedCssNodes, resultMessage);
        operationsResponse.Success &= this.HandleDeletedClassificationNodes(executionContext, tfsSyncedCssNodes, searchSyncedCssNodes, resultMessage);
        if (!operationsResponse.Success)
          throw new SearchException(FormattableString.Invariant(FormattableStringFactory.Create("Update SearchPlatform documents or delete CSS nodes in DB failed for project: {0} ", (object) this.IndexingUnit)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      catch (AggregateException ex) when (new ClassificationNodeNotRecognizedFaultMapper().IsMatch((Exception) ex))
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] is not in a well-formed state; bailing out.", (object) this.IndexingUnit.TFSEntityId)));
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual IndexOperationsResponse HandleMissingAndStaleClassificationNodes(
      IndexingExecutionContext executionContext,
      List<ClassificationNode> tfsSyncedCssNodes,
      List<ClassificationNode> searchSyncedCssNodes,
      StringBuilder resultMessage)
    {
      List<ClassificationNode> list = tfsSyncedCssNodes.Except<ClassificationNode>((IEnumerable<ClassificationNode>) searchSyncedCssNodes, (IEqualityComparer<ClassificationNode>) new ClassificationNodeEqualityComparator()).ToList<ClassificationNode>();
      IndexOperationsResponse operationsResponse = new IndexOperationsResponse()
      {
        Success = true,
        IsOperationIncomplete = false
      };
      if (list.Count > 0)
      {
        operationsResponse = this.UpdateClassificationNodesInSearchPlatform(this.GetAllDescendantNodes(list), executionContext, resultMessage);
        if (operationsResponse.Success)
        {
          if (!operationsResponse.IsOperationIncomplete)
          {
            this.UpdateSecurityHashIfApplicableAndPersistToDb(executionContext.RequestContext, list, this.FetchRootAreaToken());
            resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Updated [{0}] CSS nodes in DB. ", (object) list.Count)));
          }
          else
          {
            this.RequeueOperation(executionContext);
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requeued the operation because of timeout for project {0}. ", (object) this.IndexingUnit)));
          }
        }
        else
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Failed to update CSS nodes in Search Index for project {0}. ", (object) this.IndexingUnit)));
      }
      return operationsResponse;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent RequeueOperation(
      IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.IndexingUnitChangeEvent.ChangeType,
        ChangeData = this.IndexingUnitChangeEvent.ChangeData,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    private bool HandleDeletedClassificationNodes(
      IndexingExecutionContext executionContext,
      List<ClassificationNode> tfsSyncedCssNodes,
      List<ClassificationNode> searchSyncedCssNodes,
      StringBuilder resultMessage)
    {
      List<ClassificationNode> list = searchSyncedCssNodes.Except<ClassificationNode>((IEnumerable<ClassificationNode>) tfsSyncedCssNodes, (IEqualityComparer<ClassificationNode>) new ClassificationNodeIdComparator()).ToList<ClassificationNode>();
      int num = this.DeleteClassificationNodesInDb(executionContext.RequestContext, list);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleted [{0}] CSS nodes from DB. ", (object) num)));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("CSS nodes deleted are [{0}]", (object) string.Join(";", list.Select<ClassificationNode, string>((Func<ClassificationNode, string>) (n => n.Path))))));
      return true;
    }

    private int DeleteClassificationNodesInDb(
      IVssRequestContext requestContext,
      List<ClassificationNode> nodesToBeDeleted)
    {
      return nodesToBeDeleted.Count > 0 ? this.m_classificationNodeDataAccess.DeleteClassificationNodes(requestContext, nodesToBeDeleted.Select<ClassificationNode, int>((Func<ClassificationNode, int>) (n => n.Id)).ToList<int>()) : 0;
    }

    internal virtual IndexOperationsResponse UpdateClassificationNodesInSearchPlatform(
      ISet<ClassificationNode> nodesToBeUpdated,
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      IndexIdentity index1 = executionContext.GetIndex();
      ISearchIndex index2 = executionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
      int num = 0;
      IndexOperationsResponse operationsResponse = new IndexOperationsResponse()
      {
        Success = false,
        IsOperationIncomplete = false
      };
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        foreach (ClassificationNode classificationNode in (IEnumerable<ClassificationNode>) nodesToBeUpdated)
        {
          TimeSpan requestTimeLimit = TimeSpan.FromSeconds((double) executionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec) - stopwatch.Elapsed;
          if (this.IsOperationTimedOut(requestTimeLimit))
          {
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updating CSS nodes in Search Index bailed out because timer expired during check. ")));
            return operationsResponse;
          }
          string type = classificationNode.NodeType == ClassificationNodeType.Area ? WorkItemContract.PlatformFieldNames.AreaId : WorkItemContract.PlatformFieldNames.IterationId;
          string str1 = classificationNode.NodeType == ClassificationNodeType.Area ? WorkItemContract.ContractFieldNames.AreaPath : WorkItemContract.ContractFieldNames.IterationPath;
          string str2 = classificationNode.NodeType == ClassificationNodeType.Area ? WorkItemContract.PlatformFieldNames.AreaPath : WorkItemContract.PlatformFieldNames.IterationPath;
          IndexInfo indexInfo = new IndexInfo();
          indexInfo.IndexName = index1.Name;
          int contractType = (int) executionContext.ProvisioningContext.ContractType;
          IExpression expression = (IExpression) new AndExpression(new IExpression[3]
          {
            (IExpression) new TermExpression("collectionId", Operator.Equals, executionContext.CollectionId.ToString().ToLowerInvariant()),
            (IExpression) new TermExpression(type, Operator.Equals, classificationNode.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
            (IExpression) new NotExpression((IExpression) new TermExpression(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) str2, (object) "lower")), Operator.Equals, classificationNode.Path.ToLowerInvariant()))
          });
          WorkItemContract updatedPartialAbstractSearchDocument = new WorkItemContract();
          updatedPartialAbstractSearchDocument.Fields = (IDictionary<string, object>) new FriendlyDictionary<string, object>()
          {
            [str1] = (object) classificationNode.Path
          };
          IExpression query = expression;
          TimeSpan requestTimeOut = requestTimeLimit;
          BulkUpdateByQueryRequest request = new BulkUpdateByQueryRequest(indexInfo, (DocumentContractType) contractType, (AbstractSearchDocumentContract) updatedPartialAbstractSearchDocument, query, requestTimeOut);
          operationsResponse = index2.BulkUpdateByQuery((ExecutionContext) executionContext, request);
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "BulkUpdateByQuery", (object) operationsResponse)));
          if (operationsResponse.IsOperationIncomplete)
          {
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updating CSS nodes in Search Index bailed out because timer expired. ")));
            return operationsResponse;
          }
          ++num;
        }
      }
      finally
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated [{0}] CSS nodes in Search Index versus [{1}] nodes in TFS targetted for update. ", (object) num, (object) nodesToBeUpdated.Count)));
      }
      if (nodesToBeUpdated.Count == num)
      {
        operationsResponse.Success = true;
        operationsResponse.IsOperationIncomplete = false;
      }
      return operationsResponse;
    }

    internal virtual bool IsOperationTimedOut(TimeSpan requestTimeLimit) => requestTimeLimit <= TimeSpan.Zero;

    private List<ClassificationNode> FetchTfsClassificationNodes(Guid projectId)
    {
      List<WorkItemClassificationNode> workItemCssNodes = new List<WorkItemClassificationNode>();
      try
      {
        workItemCssNodes = this.m_workItemHttpClientWrapper.GetRootClassificationNodes(projectId);
      }
      catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
      {
      }
      catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) projectId, (object) ex)));
      }
      return ClassificationNode.Expand(projectId, (IEnumerable<WorkItemClassificationNode>) workItemCssNodes);
    }

    private List<ClassificationNode> FetchSearchClassificationNodes(
      IndexingExecutionContext executionContext,
      Guid projectId)
    {
      return this.m_classificationNodeDataAccess.GetClassificationNodes(executionContext.RequestContext, -1).Where<ClassificationNode>((Func<ClassificationNode, bool>) (n => n.ProjectId == projectId)).ToList<ClassificationNode>();
    }

    private ISet<ClassificationNode> GetAllDescendantNodes(List<ClassificationNode> nodes)
    {
      HashSet<ClassificationNode> allDescendantNodes = new HashSet<ClassificationNode>((IEqualityComparer<ClassificationNode>) new ClassificationNodeEqualityComparator());
      foreach (ClassificationNode node in nodes)
      {
        foreach (ClassificationNode allDescendantNode in node.GetAllDescendantNodes())
          allDescendantNodes.Add(allDescendantNode);
      }
      return (ISet<ClassificationNode>) allDescendantNodes;
    }

    private string FetchRootAreaToken()
    {
      Guid tfsEntityId = this.IndexingUnit.TFSEntityId;
      try
      {
        WorkItemClassificationNode rootAreaNode = this.m_workItemHttpClientWrapper.GetRootAreaNode(tfsEntityId);
        if (rootAreaNode != null)
          return new ClassificationNode(rootAreaNode, (ClassificationNode) null, tfsEntityId).Token;
      }
      catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
      {
      }
      catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(ProjectWorkItemSyncAllClassificationNodesOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) tfsEntityId, (object) ex)));
      }
      return (string) null;
    }
  }
}
