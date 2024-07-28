// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.CollectionWikiPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class CollectionWikiPatchOperation : AbstractIndexingPatchOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080715, "Indexing Pipeline", "IndexingOperation");
    private IDictionary<Guid, TeamProjectReference> m_wellFormedProjects;
    private ISet<Guid> m_nonDeletedProjects;

    public CollectionWikiPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    private void FetchProjects(ExecutionContext executionContext)
    {
      List<TeamProjectReference> list = new ProjectHttpClientWrapper(executionContext, CollectionWikiPatchOperation.s_traceMetadata).GetProjects(new ProjectState?(ProjectState.All)).ToList<TeamProjectReference>();
      this.m_wellFormedProjects = (IDictionary<Guid, TeamProjectReference>) list.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (p => p.State == ProjectState.WellFormed)).ToDictionary<TeamProjectReference, Guid, TeamProjectReference>((Func<TeamProjectReference, Guid>) (p => p.Id), (Func<TeamProjectReference, TeamProjectReference>) (p => p));
      this.m_nonDeletedProjects = (ISet<Guid>) new HashSet<Guid>(list.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (p => p.State != ProjectState.Deleted)).Select<TeamProjectReference, Guid>((Func<TeamProjectReference, Guid>) (p => p.Id)));
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionWikiPatchOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessageBuilder = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (coreIndexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ContinuousIndexing"))
        {
          this.FetchProjects((ExecutionContext) coreIndexingExecutionContext);
          List<IIndexingPatchTask> patchTasks = new List<IIndexingPatchTask>();
          patchTasks.Add(this.GetMissedProjectDeleteAndRenameNotificationHandlerTask((ExecutionContext) coreIndexingExecutionContext));
          this.SyncGitRepositories(executionContext, resultMessageBuilder);
          patchTasks.Add(this.GetFailedWikiProcessorTask());
          this.ExecutePatchTasks(executionContext, (IEnumerable<IIndexingPatchTask>) patchTasks, resultMessageBuilder);
        }
        this.QueuePatchEventsForChildIndexingUnits(executionContext, resultMessageBuilder);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessageBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionWikiPatchOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    private IIndexingPatchTask GetMissedProjectDeleteAndRenameNotificationHandlerTask(
      ExecutionContext executionContext)
    {
      bool entityCrudOperationsFeatureEnabled = executionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ContinuousIndexing");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IIndexingPatchTask) new MissedDeleteAndRenameNotificationHandlerTask(this.DataAccessFactory, this.IndexingUnitChangeEventHandler, "Project", (IEntityType) WikiEntityType.GetInstance(), CollectionWikiPatchOperation.\u003C\u003EO.\u003C0\u003E__HasWikiProjectChanged ?? (CollectionWikiPatchOperation.\u003C\u003EO.\u003C0\u003E__HasWikiProjectChanged = new Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool>(WikiProjectUtils.HasWikiProjectChanged)), this.m_wellFormedProjects, this.m_nonDeletedProjects, entityCrudOperationsFeatureEnabled);
    }

    private IIndexingPatchTask GetFailedWikiProcessorTask() => (IIndexingPatchTask) new FailedWikiProcessorTask(this.DataAccessFactory, this.IndexingUnitChangeEventHandler);

    private void QueuePatchEventsForChildIndexingUnits(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      int configValueOrDefault1 = executionContext.RequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WikiPatchOperationQueueBatchSize", 2);
      int configValueOrDefault2 = executionContext.RequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WikiPatchOperationQueueDelayInSeconds", 1);
      this.CreatePatchEventsForChildIndexingUnits((ExecutionContext) executionContext, configValueOrDefault1, configValueOrDefault2, resultMessageBuilder);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionGitRepositoryAcesSyncOperation(
      ExecutionContext executionContext)
    {
      int indexingUnitId = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", (IEntityType) WikiEntityType.GetInstance()).IndexingUnitId;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnitId,
        ChangeData = (ChangeEventData) new GitRepositoryAcesSyncEventData(executionContext),
        ChangeType = "GitRepositorySecurityAcesSync",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
    }

    internal virtual void SyncGitRepositories(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer((ExecutionContext) indexingExecutionContext, CollectionWikiPatchOperation.s_traceMetadata, this.IndexingUnitChangeEventHandler, (IEntityType) WikiEntityType.GetInstance()).SyncGitRepositories(indexingExecutionContext, resultMessageBuilder);
      try
      {
        this.AddCollectionGitRepositoryAcesSyncOperation((ExecutionContext) indexingExecutionContext);
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories security hash sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CollectionWikiPatchOperation.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories security hash sync failed with exception: {0} ", (object) ex));
      }
    }
  }
}
