// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractPeriodicCatchUpJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractPeriodicCatchUpJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension
  {
    protected static readonly string s_TraceArea = "Indexing Pipeline";
    protected static readonly string s_TraceLayer = "Job";
    private IndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;

    protected AbstractPeriodicCatchUpJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal AbstractPeriodicCatchUpJob(IDataAccessFactory dataAccessFactory)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.m_indexingUnitChangeEventHandler = new IndexingUnitChangeEventHandler(this.DataAccessFactory);
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      if (requestContext.IsCollectionSoftDeleted())
      {
        resultMessage = "Collection is in soft delete state, so exiting without performing any operation.";
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      if (!this.IsIndexingEnabled(requestContext))
      {
        resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled for the account for enityType {0}", (object) this.EntityType));
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 11);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      StringBuilder resultMessageBuilder = new StringBuilder();
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = this.GetCollectionIndexingUnit(executionContext, out TeamFoundationJobExecutionResult _, resultMessageBuilder, out indexingUnitDataAccess);
        if (collectionIndexingUnit == null)
          return TeamFoundationJobExecutionResult.Failed;
        if (this.PreRunCatchUp(executionContext, collectionIndexingUnit, indexingUnitDataAccess, resultMessageBuilder))
          this.AddCollectionPatchOperation(executionContext, collectionIndexingUnit, resultMessageBuilder);
        this.GetIndexVersion(executionContext, collectionIndexingUnit);
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Collection Not Available ehile processing Peiodic Catch up job. Exception raised: {0}", (object) ex)));
        }
        else
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Periodic Catchup Job failed with exception {0}. ", (object) ex);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, str);
          TeamFoundationEventLog.Default.Log(str, SearchEventId.PeriodicCatchUpJobFailed, EventLogEntryType.Error);
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, str);
          return TeamFoundationJobExecutionResult.Failed;
        }
      }
      finally
      {
        resultMessage = resultMessageBuilder.ToString();
        stopwatch.Stop();
        executionContext.ExecutionTracerContext.PublishKpi(jobDefinition.Name, AbstractPeriodicCatchUpJob.s_TraceArea, (double) stopwatch.ElapsedMilliseconds);
        executionContext.ExecutionTracerContext.PublishCi(AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, "OperationStatus", "Succeeded");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    internal abstract bool IsIndexingEnabled(IVssRequestContext requestContext);

    internal virtual bool PreRunCatchUp(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      StringBuilder resultMessageBuilder)
    {
      return true;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      StringBuilder resultMessageBuilder)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = collectionIndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData(executionContext),
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent1);
      resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Created collection patch operation [{0}].", (object) indexingUnitChangeEvent2.ToString())));
      return indexingUnitChangeEvent2;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionFinalizeOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      StringBuilder resultMessageBuilder)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = collectionIndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData(executionContext),
        ChangeType = "CompleteBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      if (executionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && executionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, this.EntityType))
        indexingUnitChangeEvent1.LeaseId = new Guid().ToString();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent1);
      resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Created collection Finalize operation [{0}].", (object) indexingUnitChangeEvent2.ToString())));
      return indexingUnitChangeEvent2;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetCollectionIndexingUnit(
      ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult teamFoundationJobExecutionResult,
      StringBuilder resultMessageBuilder,
      out IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      teamFoundationJobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType);
      if (indexingUnit == null)
      {
        string message = "No Collection Indexing Unit is created. possibly because Account Fault in job hasn't run yet.";
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, message);
        resultMessageBuilder.Append(message);
        return indexingUnit;
      }
      if (indexingUnit.GetIndexInfo() == null)
      {
        string message = "IndexInfo is found as null. ";
        switch (this.EntityType.Name)
        {
          case "Code":
            message += "Triggering the account fault-in for Code entity.";
            AccountCodeBulkIndexer.TriggerIndexing(executionContext);
            break;
          case "Wiki":
            message += "Triggering the account fault-in for Wiki entity.";
            AccountWikiBulkIndexer.TriggerIndexing(executionContext, true);
            break;
          case "Package":
            message += "Triggering the account fault-in for Package entity.";
            AccountPackageBulkIndexer.TriggerIndexing(executionContext, true);
            break;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, message);
        resultMessageBuilder.Append(message);
      }
      return indexingUnit;
    }

    private void GetIndexVersion(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      try
      {
        if (!executionContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/LogIndexVersion"))
          return;
        IndexingExecutionContext executionContext1 = new IndexingExecutionContext(executionContext.RequestContext, collectionIndexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails);
        IndexInfo indexInfo = collectionIndexingUnit.GetIndexInfo();
        if (indexInfo == null)
          return;
        IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(indexInfo.IndexName);
        string setting = (string) executionContext1.ProvisioningContext.SearchPlatform.GetIndex(indexIdentity).GetSettings()["index.version.created"];
        CustomerIntelligenceData ci = new CustomerIntelligenceData();
        ci.Add("CollectionId", (object) collectionIndexingUnit.TFSEntityId);
        ci.Add("IndexName", indexInfo.IndexName);
        ci.Add("IndexVersion", setting);
        ci.Add("ESConnectionString", executionContext1.ProvisioningContext.SearchPlatformConnectionString);
        executionContext.ExecutionTracerContext.PublishCi(AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, ci);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, ex);
      }
    }

    internal virtual IndexingUnitChangeEventHandler IndexingUnitChangeEventHandler
    {
      get => this.m_indexingUnitChangeEventHandler;
      set => this.m_indexingUnitChangeEventHandler = value;
    }

    internal IDataAccessFactory DataAccessFactory { get; }

    protected abstract IEntityType EntityType { get; }

    protected abstract int TracePoint { get; }
  }
}
