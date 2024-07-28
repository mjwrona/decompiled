// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionRenameRepairJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class CollectionRenameRepairJob : ITeamFoundationJobExtension
  {
    private IDataAccessFactory m_dataAccessFactory;
    private IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private const string s_traceArea = "Indexing Pipeline";
    private const string s_traceLayer = "Job";
    [StaticSafe]
    private static TraceMetaData s_traceMetaData = new TraceMetaData(1080343, "Indexing Pipeline", "Job");
    private const int TracePoint = 1080343;

    public CollectionRenameRepairJob()
      : this(DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler())
    {
    }

    internal CollectionRenameRepairJob(
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
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080343, "Indexing Pipeline", "Job", nameof (Run));
      StringBuilder stringBuilder = new StringBuilder();
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 27);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      try
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.ServiceHost.InstanceId, "Collection", (IEntityType) CodeEntityType.GetInstance());
        if (indexingUnit == null)
        {
          string message = "No Collection Indexing Unit exists.";
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(CollectionRenameRepairJob.s_traceMetaData, message);
          stringBuilder.Append(message);
          return TeamFoundationJobExecutionResult.Failed;
        }
        string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionRenameRepairJob queued Event {0}.", (object) this.CreateCollectionRenameEvent(executionContext, indexingUnit).ToString());
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(CollectionRenameRepairJob.s_traceMetaData, message1);
        stringBuilder.Append(message1);
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(CollectionRenameRepairJob.s_traceMetaData, ex);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "CollectionRenameRepairJob failed with exception {0}", (object) ex);
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        resultMessage = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080343, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateCollectionRenameEvent(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = collectionIndexingUnit.IndexingUnitId,
        ChangeType = "BeginEntityRename",
        ChangeData = (ChangeEventData) new EntityRenameEventData(executionContext)
        {
          OldEntityName = (collectionIndexingUnit.TFSEntityAttributes as CollectionAttributes).CollectionName
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
    }
  }
}
