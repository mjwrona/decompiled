// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemRenameOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemRenameOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080666, "Indexing Pipeline", "IndexingOperation");
    private readonly ProjectHttpClientWrapper m_projectHttpClientWrapper;
    private readonly WorkItemHttpClientWrapper m_workItemHttpClientWrapper;

    public ProjectWorkItemRenameOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new ProjectHttpClientWrapper(executionContext, ProjectWorkItemRenameOperation.s_traceMetadata), new WorkItemHttpClientWrapper(executionContext, ProjectWorkItemRenameOperation.s_traceMetadata))
    {
    }

    protected ProjectWorkItemRenameOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      ProjectHttpClientWrapper projectHttpClientWrapper,
      WorkItemHttpClientWrapper workItemHttpClientWrapper)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_projectHttpClientWrapper = projectHttpClientWrapper;
      this.m_workItemHttpClientWrapper = workItemHttpClientWrapper;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemRenameOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      try
      {
        string name = this.m_projectHttpClientWrapper.GetProjects().SingleOrDefault<TeamProjectReference>((Func<TeamProjectReference, bool>) (p => p.Id == this.IndexingUnit.TFSEntityId))?.Name;
        if (name == null)
        {
          resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Project with Id [{0}] not found in TFS. Not processing project rename.", (object) this.IndexingUnit.TFSEntityId)));
        }
        else
        {
          string oldProjectName = ((ProjectWorkItemTFSAttributes) this.IndexingUnit.TFSEntityAttributes)?.ProjectName ?? string.Empty;
          if (string.IsNullOrWhiteSpace(oldProjectName))
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(ProjectWorkItemRenameOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project name not present in Indexing Unit: [{0}]", (object) this.IndexingUnit)));
          if (oldProjectName.Equals(name, StringComparison.Ordinal))
          {
            resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Project name in Indexing Unit is already in sync with that in TFS. Doing nothing."))));
          }
          else
          {
            IndexOperationsResponse operationsResponse = this.UpdateProjectNameInSearchPlatform(name, oldProjectName, (IndexingExecutionContext) coreIndexingExecutionContext, resultMessage);
            if (!operationsResponse.Success)
              throw new SearchServiceException("Work item project rename failed on search platform. ");
            if (!operationsResponse.IsOperationIncomplete)
            {
              this.CreateEventsForUpdatingRootClassificationNodes((ExecutionContext) coreIndexingExecutionContext, resultMessage);
              ((ProjectWorkItemTFSAttributes) this.IndexingUnit.TFSEntityAttributes).ProjectName = name;
              this.IndexingUnit.Properties.Name = name;
              this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
            }
            else
            {
              this.RequeueOperation((ExecutionContext) coreIndexingExecutionContext);
              resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Work item Rename operation completed partially. Requeued the operation for project {0}. ", (object) this.IndexingUnit)));
            }
          }
        }
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemRenameOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent RequeueOperation(
      ExecutionContext executionContext)
    {
      EntityRenameEventData changeData = (EntityRenameEventData) this.IndexingUnitChangeEvent.ChangeData;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.IndexingUnitChangeEvent.ChangeType,
        ChangeData = (ChangeEventData) new EntityRenameEventData(executionContext)
        {
          NewEntityName = changeData.NewEntityName,
          OldEntityName = changeData.OldEntityName
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }

    internal virtual IndexOperationsResponse UpdateProjectNameInSearchPlatform(
      string newProjectName,
      string oldProjectName,
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      IndexIdentity index1 = executionContext.GetIndex();
      ISearchIndex index2 = executionContext.ProvisioningContext.SearchPlatform.GetIndex(index1);
      string normalizedString = this.IndexingUnit.GetTfsEntityIdAsNormalizedString();
      IndexInfo indexInfo = new IndexInfo();
      indexInfo.IndexName = index1.Name;
      int contractType = (int) executionContext.ProvisioningContext.ContractType;
      IExpression expression = (IExpression) new AndExpression(new IExpression[3]
      {
        (IExpression) new TermExpression("collectionId", Operator.Equals, executionContext.CollectionId.ToString().ToLowerInvariant()),
        (IExpression) new TermExpression("projectId", Operator.Equals, normalizedString),
        (IExpression) new TermExpression("projectName", Operator.Equals, oldProjectName.ToLowerInvariant())
      });
      WorkItemContract updatedPartialAbstractSearchDocument = new WorkItemContract();
      updatedPartialAbstractSearchDocument.ProjectName = newProjectName;
      updatedPartialAbstractSearchDocument.Fields = (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        [WorkItemContract.ContractFieldNames.TeamProject] = (object) newProjectName
      };
      IExpression query = expression;
      TimeSpan requestTimeOut = TimeSpan.FromSeconds((double) executionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec);
      BulkUpdateByQueryRequest request = new BulkUpdateByQueryRequest(indexInfo, (DocumentContractType) contractType, (AbstractSearchDocumentContract) updatedPartialAbstractSearchDocument, query, requestTimeOut);
      IndexOperationsResponse operationsResponse = index2.BulkUpdateByQuery((ExecutionContext) executionContext, request);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("{0} response: [{1}]. ", (object) "BulkUpdateByQuery", (object) operationsResponse)));
      return operationsResponse;
    }

    private void CreateEventsForUpdatingRootClassificationNodes(
      ExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      Guid tfsEntityId = this.IndexingUnit.TFSEntityId;
      List<WorkItemClassificationNode> classificationNodes;
      try
      {
        classificationNodes = this.m_workItemHttpClientWrapper.GetRootClassificationNodes(tfsEntityId);
      }
      catch (AggregateException ex) when (new ClassificationNodeNotRecognizedFaultMapper().IsMatch((Exception) ex))
      {
        return;
      }
      catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
      {
        return;
      }
      catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(ProjectWorkItemRenameOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) tfsEntityId, (object) ex)));
        return;
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(classificationNodes.Count);
      foreach (WorkItemClassificationNode classificationNode in classificationNodes)
      {
        ChangeEventData changeEventData = (ChangeEventData) new WorkItemClassificationNodesEventData(executionContext)
        {
          ProjectId = tfsEntityId,
          EventType = EventType.Rename,
          NodeId = classificationNode.Id,
          NodeType = (classificationNode.StructureType == TreeNodeStructureType.Area ? ClassificationNodeType.Area : ClassificationNodeType.Iteration)
        };
        indexingUnitChangeEventList1.Add(new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = this.IndexingUnit.IndexingUnitId,
          ChangeType = "UpdateClassificationNode",
          ChangeData = changeEventData,
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = (byte) 0
        });
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.IndexingUnitChangeEventHandler.HandleEvents(executionContext, indexingUnitChangeEventList1);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] [{1}] operations for root classification nodes. ", (object) indexingUnitChangeEventList2.Count, (object) "UpdateClassificationNode")));
    }
  }
}
