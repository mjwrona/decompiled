// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.MissedClassificationNodeChangeNotificationHandlerTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class MissedClassificationNodeChangeNotificationHandlerTask : IIndexingPatchTask
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080486, "Indexing Pipeline", "IndexingOperation");
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private readonly WorkItemHttpClientWrapper m_workItemHttpClientWrapper;
    private readonly IClassificationNodeDataAccess m_classificationNodeDataAccess;

    public string Name { get; } = nameof (MissedClassificationNodeChangeNotificationHandlerTask);

    public MissedClassificationNodeChangeNotificationHandlerTask(
      ExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory)
      : this(dataAccessFactory, (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler(), new WorkItemHttpClientWrapper(executionContext, MissedClassificationNodeChangeNotificationHandlerTask.s_traceMetadata))
    {
    }

    protected MissedClassificationNodeChangeNotificationHandlerTask(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      WorkItemHttpClientWrapper workItemHttpClientWrapper)
    {
      this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_classificationNodeDataAccess = dataAccessFactory.GetClassificationNodeDataAccess();
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.m_workItemHttpClientWrapper = workItemHttpClientWrapper;
    }

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      if (!indexingExecutionContext.RequestContext.IsWorkItemCrudOperationsEnabled())
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Feature [{0}] is disabled. ", (object) "Search.Server.WorkItem.CrudOperations")) + FormattableString.Invariant(FormattableStringFactory.Create("Bailing out of [{0}] patch task. ", (object) this.Name)));
      }
      else
      {
        Dictionary<Guid, HashSet<ClassificationNode>> dictionary = this.m_classificationNodeDataAccess.GetClassificationNodes(indexingExecutionContext.RequestContext, -1).GroupBy<ClassificationNode, Guid>((Func<ClassificationNode, Guid>) (n => n.ProjectId)).AsParallel<IGrouping<Guid, ClassificationNode>>().ToDictionary<IGrouping<Guid, ClassificationNode>, Guid, HashSet<ClassificationNode>>((Func<IGrouping<Guid, ClassificationNode>, Guid>) (kvp => kvp.Key), (Func<IGrouping<Guid, ClassificationNode>, HashSet<ClassificationNode>>) (kvp => new HashSet<ClassificationNode>((IEnumerable<ClassificationNode>) kvp)));
        int num1 = 0;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", (IEntityType) WorkItemEntityType.GetInstance(), -1);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit in indexingUnits)
        {
          try
          {
            Guid tfsEntityId = projectIndexingUnit.TFSEntityId;
            HashSet<ClassificationNode> tfsCssNodes = new HashSet<ClassificationNode>((IEnumerable<ClassificationNode>) this.FetchTfsCssNodes(tfsEntityId));
            HashSet<ClassificationNode> searchCssNodes;
            if (!dictionary.TryGetValue(tfsEntityId, out searchCssNodes) || !this.AreCssNodesInSync(tfsCssNodes, searchCssNodes))
            {
              this.FireChangeEvent((ExecutionContext) indexingExecutionContext, projectIndexingUnit);
              ++num1;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(MissedClassificationNodeChangeNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Fired [{0}] operation for [{1}] indexing unit.", (object) "SyncAllClassificationNode", (object) projectIndexingUnit)));
            }
          }
          catch (AggregateException ex) when (new ClassificationNodeNotRecognizedFaultMapper().IsMatch((Exception) ex))
          {
            resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] is not in a well-formed state; bailing out. ", (object) projectIndexingUnit.TFSEntityId)));
          }
        }
        foreach (Guid key in dictionary.Keys.Except<Guid>(indexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (iu => iu.TFSEntityId))))
        {
          HashSet<ClassificationNode> source = dictionary[key];
          int num2 = 0;
          if (source.Count > 0)
            num2 = this.m_classificationNodeDataAccess.DeleteClassificationNodes(indexingExecutionContext.RequestContext, source.Select<ClassificationNode, int>((Func<ClassificationNode, int>) (n => n.Id)).ToList<int>());
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Deleted all [{0}] CSS nodes of project [{1}] from DB. ", (object) num2, (object) key)));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(MissedClassificationNodeChangeNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("CSS nodes attempted to delete are [{0}]", (object) string.Join(";", source.Select<ClassificationNode, string>((Func<ClassificationNode, string>) (n => n.Path))))));
        }
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Fired [{0}] [{1}] operations. ", (object) num1, (object) "SyncAllClassificationNode")));
      }
    }

    internal bool AreCssNodesInSync(
      HashSet<ClassificationNode> tfsCssNodes,
      HashSet<ClassificationNode> searchCssNodes)
    {
      if (tfsCssNodes.Count != searchCssNodes.Count)
        return false;
      ClassificationNodeIdComparator comparer1 = new ClassificationNodeIdComparator();
      foreach (ClassificationNode searchCssNode in searchCssNodes)
      {
        if (!tfsCssNodes.Contains<ClassificationNode>(searchCssNode, (IEqualityComparer<ClassificationNode>) comparer1))
          return false;
      }
      ClassificationNodeEqualityComparator comparer2 = new ClassificationNodeEqualityComparator();
      foreach (ClassificationNode tfsCssNode in tfsCssNodes)
      {
        if (!searchCssNodes.Contains<ClassificationNode>(tfsCssNode, (IEqualityComparer<ClassificationNode>) comparer2))
          return false;
      }
      return true;
    }

    internal List<ClassificationNode> FetchTfsCssNodes(Guid projectId)
    {
      List<ClassificationNode> classificationNodeList = new List<ClassificationNode>();
      try
      {
        List<WorkItemClassificationNode> classificationNodes = this.m_workItemHttpClientWrapper.GetRootClassificationNodes(projectId);
        classificationNodeList = ClassificationNode.Expand(projectId, (IEnumerable<WorkItemClassificationNode>) classificationNodes);
      }
      catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
      {
      }
      catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(MissedClassificationNodeChangeNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) projectId, (object) ex)));
      }
      return classificationNodeList;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent FireChangeEvent(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = projectIndexingUnit.IndexingUnitId,
        ChangeType = "SyncAllClassificationNode",
        ChangeData = new ChangeEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
    }
  }
}
