// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PeriodicProjectRepoRefreshJob
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
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PeriodicProjectRepoRefreshJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080346, "Indexing Pipeline", "Job");
    private readonly IDataAccessFactory m_dataAccessFactory;
    private readonly IndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;

    public PeriodicProjectRepoRefreshJob()
      : this(DataAccessFactory.GetInstance(), new IndexingUnitChangeEventHandler())
    {
    }

    internal PeriodicProjectRepoRefreshJob(
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
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(PeriodicProjectRepoRefreshJob.s_traceMetadata, nameof (Run));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 11);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", (IEntityType) ProjectRepoEntityType.GetInstance());
        if (indexingUnit == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(PeriodicProjectRepoRefreshJob.s_traceMetadata, "No Collection ProjectRepo Indexing Unit created.");
          stringBuilder.Append("No Collection ProjectRepo Indexing Unit created.");
          return TeamFoundationJobExecutionResult.Succeeded;
        }
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingUnit.IndexingUnitId,
          ChangeType = "Patch",
          ChangeData = new ChangeEventData(executionContext),
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent1);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created patch event for collection [{0}]", (object) indexingUnitChangeEvent2.ToString())));
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} failed with Exception [{1}].", (object) nameof (PeriodicProjectRepoRefreshJob), (object) ex)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(PeriodicProjectRepoRefreshJob.s_traceMetadata, ex);
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        resultMessage = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(PeriodicProjectRepoRefreshJob.s_traceMetadata, nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
