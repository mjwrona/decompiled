// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeRepairJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.JobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class CodeRepairJob : ITeamFoundationJobExtension
  {
    private IDataAccessFactory m_dataAccessFactory;
    private IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    [StaticSafe]
    private static TraceMetaData s_traceMetaData = new TraceMetaData(1080344, "Indexing Pipeline", "Job");
    private const int TracePoint = 1080344;

    public CodeRepairJob()
      : this(DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler())
    {
    }

    internal CodeRepairJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.m_dataAccessFactory = dataAccessFactory;
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080344, "Indexing Pipeline", "Job", nameof (Run));
      StringBuilder stringBuilder = new StringBuilder();
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 25);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance());
        if (indexingUnit1 == null)
        {
          string message = "No Collection Indexing Unit exists.";
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CodeRepairJob.s_traceMetaData, message);
          stringBuilder.Append(message);
          return TeamFoundationJobExecutionResult.Failed;
        }
        CodeRepairJobDataModel repairJobDataModel = TeamFoundationSerializationUtility.Deserialize<CodeRepairJobDataModel>(jobDefinition.Data);
        if (repairJobDataModel.IndexingUnitType == "Collection")
        {
          this.QueueCodeReindexOperationsForCollection(executionContext, indexingUnit1);
          string message = "Queued Code Repair operations.";
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(CodeRepairJob.s_traceMetaData, message);
          stringBuilder.Append(message);
        }
        else if (repairJobDataModel.IndexingUnitType == "Git_Repository" || repairJobDataModel.IndexingUnitType == "TFVC_Repository")
        {
          Guid TFSEntityId = new Guid(repairJobDataModel.RepositoryId);
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, TFSEntityId, repairJobDataModel.RepositoryType, (IEntityType) CodeEntityType.GetInstance());
          if (indexingUnit2 == null)
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No Indexing Unit exists for repository Id: '{0}' and Type: '{1}'", (object) TFSEntityId, (object) repairJobDataModel.RepositoryType);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CodeRepairJob.s_traceMetaData, message);
            stringBuilder.Append(message);
            return TeamFoundationJobExecutionResult.Failed;
          }
          this.QueueCodeReindexOperationsForRepository(executionContext, indexingUnit1, indexingUnit2);
          string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queued Code Repair operations for repository Id: '{0}' and Type: '{1}'", (object) TFSEntityId, (object) repairJobDataModel.RepositoryType);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(CodeRepairJob.s_traceMetaData, message1);
          stringBuilder.Append(message1);
        }
        else
        {
          string message = "Unsupported Repair Type.";
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CodeRepairJob.s_traceMetaData, message);
          stringBuilder.Append(message);
          return TeamFoundationJobExecutionResult.Failed;
        }
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(CodeRepairJob.s_traceMetaData, ex);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Code Repair Job failed with exception {0}", (object) ex);
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        resultMessage = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080344, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> QueueCodeReindexOperationsForCollection(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> deleteRepoEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent1 = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, collectionIndexingUnit.IndexingUnitId, -1);
      if (unitsWithGivenParent1 != null)
      {
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsWithGivenParent1)
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent2 = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
          if (unitsWithGivenParent2 != null)
          {
            foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit in unitsWithGivenParent2)
              deleteRepoEventList.Add(this.QueueCodeRepoDeleteOperation(executionContext, repoIndexingUnit));
          }
        }
        this.QueueCodeBulkIndexOperation(executionContext, collectionIndexingUnit, deleteRepoEventList);
      }
      return deleteRepoEventList;
    }

    private void QueueCodeBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> deleteRepoEventList)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent deleteRepoEvent in deleteRepoEventList)
        eventPrerequisites.Add(new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = deleteRepoEvent.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded
          }
        });
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = collectionIndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeMetadataCrawlEventData(executionContext)
        {
          TriggerBulkIndexing = true
        },
        ChangeType = "CrawlMetadata",
        State = IndexingUnitChangeEventState.Pending,
        Prerequisites = eventPrerequisites,
        AttemptCount = 0
      };
      this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueCodeRepoDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit)
    {
      this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoIndexingUnit.IndexingUnitId,
        ChangeType = "Delete",
        ChangeData = new ChangeEventData(executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueCodeReindexOperationsForRepository(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit)
    {
      this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoIndexingUnit.IndexingUnitId,
        ChangeType = "ReIndex",
        ChangeData = new ChangeEventData(),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }
  }
}
