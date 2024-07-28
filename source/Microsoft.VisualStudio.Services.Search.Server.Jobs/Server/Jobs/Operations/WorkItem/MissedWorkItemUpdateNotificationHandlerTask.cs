// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.MissedWorkItemUpdateNotificationHandlerTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
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
  internal class MissedWorkItemUpdateNotificationHandlerTask : IIndexingPatchTask
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080484, "Indexing Pipeline", "IndexingOperation");
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly IIndexingUnitChangeEventDataAccess m_indexingUnitChangeEventDataAccess;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private readonly WorkItemHttpClientWrapper m_workItemHttpClientWrapper;

    public string Name => nameof (MissedWorkItemUpdateNotificationHandlerTask);

    internal MissedWorkItemUpdateNotificationHandlerTask(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      WorkItemHttpClientWrapper workItemHttpClientWrapper)
    {
      this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_indexingUnitChangeEventDataAccess = dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.m_workItemHttpClientWrapper = workItemHttpClientWrapper;
    }

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", (IEntityType) WorkItemEntityType.GetInstance(), -1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = this.FilterOutProjectsWithSyncedContinuationToken((IReadOnlyCollection<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.FilterOutProjectsAlreadyInQueue((ExecutionContext) indexingExecutionContext, (IReadOnlyCollection<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits));
      int configValue1 = indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemProjectPatchQueueBatchSize");
      int configValue2 = indexingExecutionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemProjectPatchQueueDelayInSeconds");
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      for (int index = 0; index < indexingUnitList.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitList[index];
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
        indexingUnitChangeEvent1.IndexingUnitId = indexingUnit.IndexingUnitId;
        indexingUnitChangeEvent1.ChangeType = "UpdateIndex";
        WorkItemUpdateIndexEventData updateIndexEventData = new WorkItemUpdateIndexEventData((ExecutionContext) indexingExecutionContext);
        updateIndexEventData.Delay = TimeSpan.FromSeconds((double) (index / configValue1 * configValue2));
        indexingUnitChangeEvent1.ChangeData = (ChangeEventData) updateIndexEventData;
        indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
        indexingUnitChangeEvent1.AttemptCount = (byte) 0;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
        indexingUnitChangeEventList.Add(indexingUnitChangeEvent2);
      }
      if (indexingUnitChangeEventList.Count > 0)
        indexingUnitChangeEventList = this.m_indexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, indexingUnitChangeEventList);
      resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] {1} events for projects. ", (object) indexingUnitChangeEventList.Count, (object) "UpdateIndex")));
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> FilterOutProjectsAlreadyInQueue(
      ExecutionContext executionContext,
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits)
    {
      HashSet<int> intSet = new HashSet<int>(this.m_indexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(executionContext.RequestContext, new List<string>()
      {
        "BeginBulkIndex",
        "UpdateIndex"
      }, new List<IndexingUnitChangeEventState>()
      {
        IndexingUnitChangeEventState.Pending,
        IndexingUnitChangeEventState.Queued
      }, -1).Select<IndexingUnitChangeEventDetails, int>((Func<IndexingUnitChangeEventDetails, int>) (evt => evt.IndexingUnitChangeEvent.IndexingUnitId)));
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(projectIndexingUnits.Count);
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) projectIndexingUnits)
      {
        if (!intSet.Contains(projectIndexingUnit.IndexingUnitId))
          indexingUnitList.Add(projectIndexingUnit);
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(MissedWorkItemUpdateNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("There are [{0}] out of [{1}] projects with no pending/queued work item BI or CI events.", (object) indexingUnitList.Count, (object) projectIndexingUnits.Count)));
      return indexingUnitList;
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> FilterOutProjectsWithSyncedContinuationToken(
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(projectIndexingUnits.Count);
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) projectIndexingUnits)
      {
        string continuationToken1 = ((ProjectWorkItemIndexingProperties) projectIndexingUnit.Properties).LastIndexedContinuationToken;
        if (string.IsNullOrWhiteSpace(continuationToken1))
          continuationToken1 = ((ProjectWorkItemIndexingProperties) projectIndexingUnit.Properties).LastIndexedFieldsContinuationToken;
        try
        {
          string continuationToken2 = this.m_workItemHttpClientWrapper.GetCurrentContinuationToken(projectIndexingUnit.TFSEntityId);
          if (continuationToken1 != continuationToken2)
            indexingUnitList.Add(projectIndexingUnit);
        }
        catch (AggregateException ex) when (ex.InnerException is ProjectDoesNotExistException)
        {
        }
        catch (AggregateException ex) when (ex.InnerException is DefaultTeamNotFoundException)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(MissedWorkItemUpdateNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Project [{0}] does not have default team. Exception: [{1}].", (object) projectIndexingUnit.TFSEntityId, (object) ex)));
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(MissedWorkItemUpdateNotificationHandlerTask.s_traceMetadata, ex);
          indexingUnitList.Add(projectIndexingUnit);
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(MissedWorkItemUpdateNotificationHandlerTask.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("There are [{0}] out of [{1}] projects for which continuation tokens are out of sync.", (object) indexingUnitList.Count, (object) projectIndexingUnits.Count)));
      return indexingUnitList;
    }
  }
}
