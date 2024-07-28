// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemUpdateClassificationNodeOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemUpdateClassificationNodeOperation : 
    ProjectWorkItemSyncAllClassificationNodesOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080664, "Indexing Pipeline", "IndexingOperation");
    private readonly IClassificationNodeDataAccess m_classificationNodeDataAccess;
    private readonly WorkItemHttpClientWrapper m_workItemHttpClientWrapper;

    public ProjectWorkItemUpdateClassificationNodeOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new WorkItemHttpClientWrapper((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) executionContext, ProjectWorkItemUpdateClassificationNodeOperation.s_traceMetadata), new ClassificationNodeSecurityAcesUpdater((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) executionContext))
    {
    }

    protected ProjectWorkItemUpdateClassificationNodeOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      WorkItemHttpClientWrapper workItemHttpClientWrapper,
      ClassificationNodeSecurityAcesUpdater securityAcesUpdater)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory, workItemHttpClientWrapper, securityAcesUpdater)
    {
      this.m_workItemHttpClientWrapper = workItemHttpClientWrapper;
      this.m_classificationNodeDataAccess = dataAccessFactory.GetClassificationNodeDataAccess();
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemUpdateClassificationNodeOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      WorkItemClassificationNodesEventData changeData = (WorkItemClassificationNodesEventData) this.IndexingUnitChangeEvent.ChangeData;
      try
      {
        WorkItemClassificationNode tfsCssRootNode = changeData.EventType == EventType.Rename || changeData.EventType == EventType.Reclassify || changeData.EventType == EventType.Add ? this.GetRootClassificationNode(executionContext, changeData) : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Event Type [{0}] is not supported in [{1}] operation.", (object) changeData.EventType, (object) nameof (ProjectWorkItemUpdateClassificationNodeOperation))));
        if (tfsCssRootNode == null)
        {
          resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not exists in TFS or does not have a default team. Bailing out of [{1}] operation. ", (object) changeData.ProjectId, (object) nameof (ProjectWorkItemUpdateClassificationNodeOperation))));
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Change event: [{0}]. ", (object) changeData)));
        WorkItemClassificationNode resultNode;
        ClassificationNode classificationNode1 = this.SearchClasssificationNodeById(changeData.NodeId, changeData.ProjectId, tfsCssRootNode, out resultNode);
        if (classificationNode1 == null)
        {
          resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Classification node with Id [{0}] does not exists in TFS. Bailing out. ", (object) changeData.NodeId)));
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        ClassificationNode classificationNode2 = this.m_classificationNodeDataAccess.GetClassificationNode(coreIndexingExecutionContext.RequestContext, classificationNode1.Id);
        if (classificationNode2 == null || classificationNode2.ParentId != classificationNode1.ParentId || !classificationNode2.Name.Equals(classificationNode1.Name, StringComparison.Ordinal))
        {
          classificationNode1.RelateToItsDescendants(resultNode);
          List<ClassificationNode> tfsSyncedCssNodes = new List<ClassificationNode>()
          {
            classificationNode1
          };
          List<ClassificationNode> classificationNodeList;
          if (classificationNode2 == null)
          {
            classificationNodeList = new List<ClassificationNode>();
          }
          else
          {
            classificationNodeList = new List<ClassificationNode>();
            classificationNodeList.Add(classificationNode2);
          }
          List<ClassificationNode> searchSyncedCssNodes = classificationNodeList;
          this.HandleMissingAndStaleClassificationNodes(executionContext, tfsSyncedCssNodes, searchSyncedCssNodes, resultMessage);
        }
        else
          resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "CSS nodes are already in sync between TFS and Search. ");
        operationResult.Status = OperationStatus.Succeeded;
      }
      catch (AggregateException ex) when (new ClassificationNodeNotRecognizedFaultMapper().IsMatch((Exception) ex))
      {
        resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Could not update classification node with Id [{0}] because project [{1}] is not in a well-formed state; bailing out.", (object) changeData.NodeId, (object) changeData.ProjectId)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemUpdateClassificationNodeOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual WorkItemClassificationNode GetRootClassificationNode(
      IndexingExecutionContext indexingExecutionContext,
      WorkItemClassificationNodesEventData eventData)
    {
      int num1 = 1;
      int num2 = 3;
      int configValue = indexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/RootClassificationNodeRetryDelayInMillis", TeamFoundationHostType.ProjectCollection, true, 5000);
      WorkItemClassificationNode classificationNode = (WorkItemClassificationNode) null;
      int num3 = ((WorkItemClassificationNodesEventData) this.IndexingUnitChangeEvent.ChangeData).EventType == EventType.Add ? num2 : num1;
      int num4 = 0;
      while (num4 < num3)
      {
        try
        {
          if (eventData.NodeType == ClassificationNodeType.Area)
          {
            classificationNode = this.m_workItemHttpClientWrapper.GetRootAreaNode(eventData.ProjectId);
            break;
          }
          if (eventData.NodeType == ClassificationNodeType.Iteration)
          {
            classificationNode = this.m_workItemHttpClientWrapper.GetRootIterationNode(eventData.ProjectId);
            break;
          }
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Classification Node Type [{0}] is invalid in this context.", (object) eventData.NodeType)));
        }
        catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
        {
          break;
        }
        catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(ProjectWorkItemUpdateClassificationNodeOperation.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) eventData.ProjectId, (object) ex)));
        }
        ++num4;
        Thread.Sleep(TimeSpan.FromMilliseconds((double) configValue));
      }
      return classificationNode;
    }

    internal virtual ClassificationNode SearchClasssificationNodeById(
      int nodeId,
      Guid projectId,
      WorkItemClassificationNode tfsCssRootNode,
      out WorkItemClassificationNode resultNode)
    {
      ClassificationNode searchCssNode = new ClassificationNode(tfsCssRootNode, (ClassificationNode) null, projectId);
      if (tfsCssRootNode.Id != nodeId)
        return this.SearchClassificationNodeByIdInternal(nodeId, searchCssNode, tfsCssRootNode, out resultNode);
      resultNode = tfsCssRootNode;
      return searchCssNode;
    }

    private ClassificationNode SearchClassificationNodeByIdInternal(
      int nodeId,
      ClassificationNode searchCssNode,
      WorkItemClassificationNode tfsCssNode,
      out WorkItemClassificationNode resultNode)
    {
      if (tfsCssNode.Children != null)
      {
        foreach (WorkItemClassificationNode child in tfsCssNode.Children)
        {
          ClassificationNode searchCssNode1 = new ClassificationNode(child, searchCssNode, searchCssNode.ProjectId);
          if (child.Id == nodeId)
          {
            resultNode = child;
            return searchCssNode1;
          }
          ClassificationNode classificationNode = this.SearchClassificationNodeByIdInternal(nodeId, searchCssNode1, child, out resultNode);
          if (classificationNode != null)
            return classificationNode;
        }
      }
      resultNode = (WorkItemClassificationNode) null;
      return (ClassificationNode) null;
    }
  }
}
