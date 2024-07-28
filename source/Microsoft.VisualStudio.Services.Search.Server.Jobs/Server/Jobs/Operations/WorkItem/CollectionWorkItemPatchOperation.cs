// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemPatchOperation : AbstractIndexingPatchOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080667, "Indexing Pipeline", "IndexingOperation");
    private readonly IDictionary<Guid, TeamProjectReference> m_wellFormedProjects;
    private readonly ISet<Guid> m_nonDeletedProjects;

    public CollectionWorkItemPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), new ProjectHttpClientWrapper(executionContext, CollectionWorkItemPatchOperation.s_traceMetadata))
    {
    }

    internal CollectionWorkItemPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory,
      ProjectHttpClientWrapper projectHttpClientWrapper)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
      List<TeamProjectReference> list = projectHttpClientWrapper.GetProjects(new ProjectState?(ProjectState.All)).ToList<TeamProjectReference>();
      this.m_wellFormedProjects = (IDictionary<Guid, TeamProjectReference>) list.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (p => p.State == ProjectState.WellFormed)).ToDictionary<TeamProjectReference, Guid, TeamProjectReference>((Func<TeamProjectReference, Guid>) (p => p.Id), (Func<TeamProjectReference, TeamProjectReference>) (p => p));
      this.m_nonDeletedProjects = (ISet<Guid>) new HashSet<Guid>(list.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (p => p.State != ProjectState.Deleted)).Select<TeamProjectReference, Guid>((Func<TeamProjectReference, Guid>) (p => p.Id)));
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionWorkItemPatchOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessageBuilder = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        List<IIndexingPatchTask> patchTasks = new List<IIndexingPatchTask>()
        {
          this.GetMissingWorkItemProjectHandlerTask(),
          this.GetMissedDeleteAndRenameNotificationHandlerTask((ExecutionContext) executionContext),
          this.GetMissedWorkItemUpdateNotificationHandlerTask((ExecutionContext) executionContext),
          this.GetMissedClassificationNodeChangeNotificationHandlerTask(executionContext),
          this.GetClassificationNodeSecurityHashRefreshTask(),
          this.GetTeamProjectAdministratorRefreshTask(),
          this.GetWorkItemFieldCacheRefreshTask(),
          this.GetFailedWorkItemProcessorTask()
        };
        this.ExecutePatchTasks(executionContext, (IEnumerable<IIndexingPatchTask>) patchTasks, resultMessageBuilder);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessageBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionWorkItemPatchOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception ex)
    {
      result.Message = ex.ToString();
      result.Status = OperationStatus.Failed;
    }

    private IIndexingPatchTask GetFailedWorkItemProcessorTask() => (IIndexingPatchTask) new FailedWorkItemProcessorTask(this.DataAccessFactory, this.IndexingUnitChangeEventHandler);

    private IIndexingPatchTask GetMissingWorkItemProjectHandlerTask() => (IIndexingPatchTask) new MissingWorkItemProjectHandlerTask(this.IndexingUnit, this.DataAccessFactory, this.IndexingUnitChangeEventHandler, (IDictionary<Guid, string>) this.m_wellFormedProjects.ToDictionary<KeyValuePair<Guid, TeamProjectReference>, Guid, string>((Func<KeyValuePair<Guid, TeamProjectReference>, Guid>) (pair => pair.Key), (Func<KeyValuePair<Guid, TeamProjectReference>, string>) (pair => pair.Value.Name)));

    private IIndexingPatchTask GetMissedDeleteAndRenameNotificationHandlerTask(
      ExecutionContext executionContext)
    {
      bool entityCrudOperationsFeatureEnabled = executionContext.RequestContext.IsWorkItemCrudOperationsEnabled();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IIndexingPatchTask) new MissedDeleteAndRenameNotificationHandlerTask(this.DataAccessFactory, this.IndexingUnitChangeEventHandler, "Project", (IEntityType) WorkItemEntityType.GetInstance(), CollectionWorkItemPatchOperation.\u003C\u003EO.\u003C0\u003E__HasWorkItemProjectChanged ?? (CollectionWorkItemPatchOperation.\u003C\u003EO.\u003C0\u003E__HasWorkItemProjectChanged = new Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool>(WorkItemProjectUtils.HasWorkItemProjectChanged)), this.m_wellFormedProjects, this.m_nonDeletedProjects, entityCrudOperationsFeatureEnabled);
    }

    private IIndexingPatchTask GetMissedWorkItemUpdateNotificationHandlerTask(
      ExecutionContext executionContext)
    {
      return (IIndexingPatchTask) new MissedWorkItemUpdateNotificationHandlerTask(this.DataAccessFactory, (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler(), new WorkItemHttpClientWrapper(executionContext, CollectionWorkItemPatchOperation.s_traceMetadata));
    }

    private IIndexingPatchTask GetClassificationNodeSecurityHashRefreshTask() => (IIndexingPatchTask) new ClassificationNodeSecurityHashRefreshTask(this.IndexingUnit, (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler());

    private IIndexingPatchTask GetMissedClassificationNodeChangeNotificationHandlerTask(
      IndexingExecutionContext executionContext)
    {
      return (IIndexingPatchTask) new MissedClassificationNodeChangeNotificationHandlerTask((ExecutionContext) executionContext, this.DataAccessFactory);
    }

    private IIndexingPatchTask GetTeamProjectAdministratorRefreshTask() => (IIndexingPatchTask) new TeamProjectAdministratorRefreshTask(this.DataAccessFactory);

    private IIndexingPatchTask GetWorkItemFieldCacheRefreshTask() => (IIndexingPatchTask) new WorkItemFieldCacheRefreshTask();
  }
}
