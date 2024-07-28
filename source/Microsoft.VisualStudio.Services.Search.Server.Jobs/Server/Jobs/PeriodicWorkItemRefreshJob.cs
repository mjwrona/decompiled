// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PeriodicWorkItemRefreshJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PeriodicWorkItemRefreshJob : ISearchServiceJobExtension, ITeamFoundationJobExtension
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080341, "Indexing Pipeline", "Job");
    private readonly IDataAccessFactory m_dataAccessFactory;
    private readonly IndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;

    public PeriodicWorkItemRefreshJob()
      : this(DataAccessFactory.GetInstance(), new IndexingUnitChangeEventHandler())
    {
    }

    internal PeriodicWorkItemRefreshJob(
      IDataAccessFactory dataAccessFactory,
      IndexingUnitChangeEventHandler changeEventHandler)
    {
      this.m_dataAccessFactory = dataAccessFactory;
      this.m_indexingUnitChangeEventHandler = changeEventHandler;
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(PeriodicWorkItemRefreshJob.s_traceMetadata, nameof (Run));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (!requestContext.IsJobEnabled(jobDefinition.JobId))
        {
          resultMessage = "Job is disabled for this host.";
          return TeamFoundationJobExecutionResult.Succeeded;
        }
        if (requestContext.IsCollectionSoftDeleted())
        {
          resultMessage = "Collection is in soft delete state, so exiting without performing any operation.";
          return TeamFoundationJobExecutionResult.Succeeded;
        }
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 11);
        ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
        if (!executionContext.RequestContext.IsWorkItemIndexingEnabled())
        {
          resultMessage = FormattableString.Invariant(FormattableStringFactory.Create("Feature {0} is disabled for the account.", (object) "Search.Server.WorkItem.Indexing"));
          return TeamFoundationJobExecutionResult.Succeeded;
        }
        StringBuilder stringBuilder = new StringBuilder();
        try
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", (IEntityType) WorkItemEntityType.GetInstance());
          if (indexingUnit == null)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(PeriodicWorkItemRefreshJob.s_traceMetadata, "No Collection Work Item Indexing Unit created.");
            stringBuilder.Append("No Collection Work Item Indexing Unit created.");
            return TeamFoundationJobExecutionResult.Succeeded;
          }
          if (indexingUnit.GetIndexInfo() == null)
          {
            string message = "IndexInfo is found as null. Triggering the account fault-in for Workitem entity. ";
            AccountWorkItemBulkIndexer.TriggerIndexing(executionContext, true);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(PeriodicWorkItemRefreshJob.s_traceMetadata, message);
            stringBuilder.Append(message);
          }
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "Patch",
            ChangeData = (ChangeEventData) new WorkItemUpdateIndexEventData(executionContext),
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = 0
          };
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent1);
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created patch event for collection [{0}]", (object) indexingUnitChangeEvent2.ToString())));
          this.GetIndexVersion(executionContext, indexingUnit);
          return TeamFoundationJobExecutionResult.Succeeded;
        }
        catch (Exception ex)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} failed with Exception [{1}].", (object) nameof (PeriodicWorkItemRefreshJob), (object) ex)));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(PeriodicWorkItemRefreshJob.s_traceMetadata, ex);
          return TeamFoundationJobExecutionResult.Failed;
        }
        finally
        {
          resultMessage = stringBuilder.ToString();
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(PeriodicWorkItemRefreshJob.s_traceMetadata, nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
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
        executionContext.ExecutionTracerContext.PublishCi(PeriodicWorkItemRefreshJob.s_traceMetadata.TraceArea, PeriodicWorkItemRefreshJob.s_traceMetadata.TraceLayer, ci);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(PeriodicWorkItemRefreshJob.s_traceMetadata, ex);
      }
    }
  }
}
