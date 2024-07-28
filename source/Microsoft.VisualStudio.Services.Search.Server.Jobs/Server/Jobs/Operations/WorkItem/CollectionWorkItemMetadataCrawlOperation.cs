// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemMetadataCrawlOperation
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
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemMetadataCrawlOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080661, "Indexing Pipeline", "IndexingOperation");

    protected internal IHttpClientWrapperFactory HttpClientWrapperFactory { get; set; }

    public CollectionWorkItemMetadataCrawlOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory.GetInstance())
    {
    }

    protected CollectionWorkItemMetadataCrawlOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IHttpClientWrapperFactory httpClientWrapperFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.HttpClientWrapperFactory = httpClientWrapperFactory;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionWorkItemMetadataCrawlOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        if (coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/WorkItemDocCountBasedIndexProvisioningEnabled", true))
          this.ProvisionIndexToCollection(coreIndexingExecutionContext);
        IEnumerable<TeamProjectReference> projectReferences = (IEnumerable<TeamProjectReference>) null;
        try
        {
          projectReferences = this.HttpClientWrapperFactory.GetProjectHttpClient((ExecutionContext) coreIndexingExecutionContext).GetProjects();
        }
        catch (Exception ex)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Fetching projects from TFS failed with exception: [{0}]. Assuming there are no projects and moving on. CI events will take care of indexing the projects later on. ", (object) ex)));
        }
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryProjectIndexingUnits = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        bool isShadowIndexingUnit = this.IsShadowIndexingRequired(coreIndexingExecutionContext, out primaryProjectIndexingUnits);
        if (projectReferences != null)
        {
          foreach (TeamProjectReference teamProject in projectReferences)
            indexingUnitList.Add(teamProject.ToProjectWorkItemIndexingUnit(this.IndexingUnit, isShadowIndexingUnit));
          if (isShadowIndexingUnit)
            indexingUnitList = this.RemoveNewProjectsFromProjectIndexingUnits(indexingUnitList, primaryProjectIndexingUnits);
          if (indexingUnitList.Count > 0)
          {
            Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(coreIndexingExecutionContext.RequestContext, indexingUnitList, true).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (kvp => kvp.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (kvp => kvp));
            foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in indexingUnitList)
            {
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
              if (dictionary.TryGetValue(indexingUnit1.TFSEntityId, out indexingUnit2))
                indexingUnit2.Properties = indexingUnit1.Properties;
              else
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CollectionWorkItemMetadataCrawlOperation.s_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("No entry found for indexingUnitId {0},", (object) indexingUnit1.IndexingUnitId)) + FormattableString.Invariant(FormattableStringFactory.Create(" indexingUnitType {0}", (object) indexingUnit1.IndexingUnitType)) + FormattableString.Invariant(FormattableStringFactory.Create(" and TFSEntityId {0} in Search.tbl_IndexingUnit", (object) indexingUnit1.TFSEntityId)) + FormattableString.Invariant(FormattableStringFactory.Create(" table of search partition database")));
            }
            indexingUnitList = this.IndexingUnitDataAccess.UpdateIndexingUnits(coreIndexingExecutionContext.RequestContext, dictionary.Values.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
          }
        }
        this.SyncAllClassificationNodesAndAreaSecurityHashes((IndexingExecutionContext) coreIndexingExecutionContext, indexingUnitList);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully added {0} Project WorkItem IndexingUnits to Collection Id {1}", (object) indexingUnitList.Count, (object) this.IndexingUnit.TFSEntityId)));
        this.AddCollectionWorkItemBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext);
        operationResult.Status = OperationStatus.Succeeded;
        return operationResult;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionWorkItemMetadataCrawlOperation.s_traceMetaData, nameof (RunOperation));
      }
    }

    internal void ProvisionIndexToCollection(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignIndex((IndexingExecutionContext) indexingExecutionContext, new List<IndexingUnitWithSize>());
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> RemoveNewProjectsFromProjectIndexingUnits(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryProjectIndexingUnitsInDb)
    {
      if (projectIndexingUnits.Count > 0)
      {
        FriendlyDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> friendlyDictionary = new FriendlyDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) primaryProjectIndexingUnitsInDb.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (k => k.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (k => k)));
        for (int index = projectIndexingUnits.Count - 1; index > 0; --index)
        {
          if (!friendlyDictionary.TryGetValue(projectIndexingUnits[index].TFSEntityId, out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit _))
            projectIndexingUnits.RemoveAt(index);
        }
      }
      return projectIndexingUnits;
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception ex)
    {
      base.HandleOperationFailure(indexingExecutionContext, result, ex);
      if (result.Status != OperationStatus.Failed)
        return;
      TeamFoundationEventLog.Default.Log(result.Message, SearchEventId.CollectionWorkItemMetadataCrawlOperationFailed, EventLogEntryType.Error);
    }

    private void SyncAllClassificationNodesAndAreaSecurityHashes(
      IndexingExecutionContext executionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> source = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(projectIndexingUnits.Count);
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit in projectIndexingUnits)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = projectIndexingUnit.IndexingUnitId,
          ChangeType = "SyncAllClassificationNode",
          ChangeData = new ChangeEventData((ExecutionContext) executionContext),
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        source.Add(this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) executionContext, indexingUnitChangeEvent));
      }
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      eventPrerequisites.AddRange((IEnumerable<IndexingUnitChangeEventPrerequisitesFilter>) source.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>) (ev => new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = ev.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        }
      })).ToList<IndexingUnitChangeEventPrerequisitesFilter>());
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "AreaNodeSecurityAcesSync",
        ChangeData = new ChangeEventData((ExecutionContext) executionContext),
        Prerequisites = eventPrerequisites,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent1);
    }

    private void AddCollectionWorkItemBulkIndexOperation(ExecutionContext executionContext)
    {
      bool flag = false;
      if (this.IndexingUnitChangeEvent.ChangeData is WorkItemMetadataCrawlEventData changeData)
        flag = changeData.Trigger != 1;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new WorkItemBulkIndexEventData(executionContext)
        {
          Finalize = !flag
        },
        ChangeType = "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }

    private bool IsShadowIndexingRequired(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      out List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryProjectIndexingUnits)
    {
      primaryProjectIndexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(coreIndexingExecutionContext.RequestContext, "Project", false, (IEntityType) WorkItemEntityType.GetInstance(), -1);
      return primaryProjectIndexingUnits != null && primaryProjectIndexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() && coreIndexingExecutionContext.IsShadowIndexingRequired(this.IndexingUnitChangeEvent);
    }
  }
}
