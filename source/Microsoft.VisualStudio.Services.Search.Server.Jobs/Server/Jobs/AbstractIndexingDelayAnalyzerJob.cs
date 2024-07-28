// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractIndexingDelayAnalyzerJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractIndexingDelayAnalyzerJob : 
    ISearchServiceJobExtension,
    ITeamFoundationJobExtension
  {
    protected static readonly string s_TraceArea = "Indexing Pipeline";
    protected static readonly string s_TraceLayer = "Job";

    protected IDataAccessFactory DataAccessFactory { get; private set; }

    protected abstract IEntityType EntityType { get; }

    protected abstract int TracePoint { get; }

    protected AbstractIndexingDelayAnalyzerJob(IDataAccessFactory dataAccessFactory) => this.DataAccessFactory = dataAccessFactory;

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
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer, nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 62);
      ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        resultMessage = this.AnalyzeIndexingDelay(executionContext);
        stringBuilder.Append(resultMessage);
      }
      catch (Exception ex)
      {
        if (IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Collection Not Available while processing IndexingDelayAnalyzerJob. Exception raised: {0}", (object) ex)));
        }
        else
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IndexingDelayAnalyzerJob failed with exception {0}. ", (object) ex);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer, str);
          TeamFoundationEventLog.Default.Log(str, SearchEventId.PeriodicCatchUpJobFailed, EventLogEntryType.Error);
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, str);
          return TeamFoundationJobExecutionResult.Failed;
        }
      }
      finally
      {
        resultMessage = stringBuilder.ToString();
        stopwatch.Stop();
        executionContext.ExecutionTracerContext.PublishKpi(jobDefinition.Name, AbstractIndexingDelayAnalyzerJob.s_TraceArea, (double) stopwatch.ElapsedMilliseconds);
        executionContext.ExecutionTracerContext.PublishCi(AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer, "OperationStatus", "Succeeded");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, AbstractIndexingDelayAnalyzerJob.s_TraceArea, AbstractIndexingDelayAnalyzerJob.s_TraceLayer, nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    internal abstract bool IsIndexingEnabled(IVssRequestContext requestContext);

    internal abstract string AnalyzeIndexingDelay(ExecutionContext executionContext);
  }
}
