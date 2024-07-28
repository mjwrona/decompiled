// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingUnitDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal abstract class IndexingUnitDeleteOperation : AbstractIndexingOperation
  {
    private IIndexingUnitWikisDataAccess m_indexingUnitWikisDataAccess;

    public IndexingUnitDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public IndexingUnitDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080627, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true);
      try
      {
        IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
        this.DeleteChangeEvents(executionContext);
        this.DeleteFailedItems(executionContext);
        this.DeleteIndexingUnitWikisIfApplicable(executionContext);
        if (currentHostConfigValue)
          this.DeleteIndexingUnitDetails(executionContext);
        this.DeleteIndexingUnit(executionContext);
        if (!executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.QueuePeriodicMaintenanceJob(executionContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080627, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return new OperationResult()
      {
        Status = OperationStatus.Succeeded
      };
    }

    internal override bool ShouldSkipOperation(
      IVssRequestContext requestContext,
      out string reasonToSkipOperation)
    {
      reasonToSkipOperation = string.Empty;
      return false;
    }

    internal virtual void DeleteIndexingUnit(IndexingExecutionContext executionContext)
    {
      this.IndexingUnitDataAccess.DeleteIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      if (executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        this.IndexingUnitDataAccess.DeleteIndexingUnitsPermanently(executionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
        {
          this.IndexingUnit
        });
      executionContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(executionContext.RequestContext, IndexingUnitNotifications.IndexingUnitDeletedEventClass, this.IndexingUnit.IndexingUnitId.Serialize<int>());
    }

    internal virtual void DeleteIndexingUnitDetails(
      IndexingExecutionContext indexingExecutionContext)
    {
      this.IndexingUnitDataAccess.DeleteIndexingUnitDetailsAndUpdateShardInformation(indexingExecutionContext.RequestContext, new List<int>()
      {
        this.IndexingUnit.IndexingUnitId
      });
    }

    internal virtual void DeleteIndexingUnitWikisIfApplicable(
      IndexingExecutionContext executionContext)
    {
      if (!(this.IndexingUnit.EntityType.Name == "Wiki") || !(this.IndexingUnit.IndexingUnitType == "Git_Repository"))
        return;
      this.IndexingUnitWikisDataAccess.DeleteIndexingUnitWikisEntry(executionContext.RequestContext, this.IndexingUnit.IndexingUnitId);
    }

    internal virtual void DeleteChangeEvents(IndexingExecutionContext executionContext)
    {
      List<IndexingUnitChangeEventState> stateList = new List<IndexingUnitChangeEventState>()
      {
        IndexingUnitChangeEventState.Pending,
        IndexingUnitChangeEventState.Queued
      };
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = this.IndexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(executionContext.RequestContext, this.IndexingUnit.IndexingUnitId, (List<string>) null, stateList, -1).ToList<IndexingUnitChangeEventDetails>().Select<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetails, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (item => item.IndexingUnitChangeEvent)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      if (list == null || !list.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080627, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Deleting the IndexingUnitChangeEvents with ID's ({0}) from IndexingUnitChangeEvent table.", (object) string.Join<long>(",", list.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, long>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, long>) (x => x.Id))))));
      this.IndexingUnitChangeEventDataAccess.DeleteIndexingUnitChangeEvents(executionContext.RequestContext, list);
    }

    internal virtual void DeleteFailedItems(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.ItemLevelFailureDataAccess.DeleteRecordsForIndexingUnit(indexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId);

    internal virtual void QueuePeriodicMaintenanceJob(
      IndexingExecutionContext indexingExecutionContext)
    {
      indexingExecutionContext.RequestContext.QueuePeriodicMaintenanceJob(120);
    }

    internal virtual IIndexingUnitWikisDataAccess IndexingUnitWikisDataAccess
    {
      get
      {
        if (this.m_indexingUnitWikisDataAccess == null)
          this.m_indexingUnitWikisDataAccess = this.DataAccessFactory.GetIndexingUnitWikisDataAccess();
        return this.m_indexingUnitWikisDataAccess;
      }
      set => this.m_indexingUnitWikisDataAccess = value;
    }
  }
}
