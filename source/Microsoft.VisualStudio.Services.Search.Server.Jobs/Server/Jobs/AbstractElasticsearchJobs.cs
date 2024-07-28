// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractElasticsearchJobs
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractElasticsearchJobs : ITeamFoundationJobExtension
  {
    protected internal ISearchPlatform SearchPlatform { get; set; }

    protected internal ISearchClusterManagementService SearchClusterStateService { get; set; }

    protected ExecutionContext ExecutionContext { get; set; }

    protected internal ElasticsearchFeedbackProcessor ElasticsearchFeedbackProcessor { get; set; }

    internal IDataAccessFactory DataAccessFactory { get; }

    protected internal int TracePoint { get; set; }

    protected internal int SearchEventIdentifier { get; set; }

    protected internal string JobName { get; set; }

    protected internal Guid JobId { get; set; }

    protected internal ISearchPlatformFactory SearchPlatformFactory { get; set; }

    protected internal string _esConnectionString { get; set; }

    protected internal MaintenanceJobStatus m_lastJobStatus { get; set; }

    protected IDictionary<MaintenanceJobStatus, int> _eventDelayIntervalMap { get; set; }

    [Info("InternalForTestPurpose")]
    internal AbstractElasticsearchJobs(
      IDataAccessFactory dataAccessFactory,
      ISearchPlatformFactory searchPlatformFactory)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.SearchPlatformFactory = searchPlatformFactory;
    }

    protected internal virtual void Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      this._esConnectionString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      this.SearchPlatform = this.SearchPlatformFactory.Create(this._esConnectionString, requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.SearchClusterStateService = this.SearchPlatformFactory.CreateSearchClusterManagementService(this._esConnectionString, requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.ElasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();
      this.JobId = jobDefinition.JobId;
      this._eventDelayIntervalMap = this.GetEventDelayIntervalMap(requestContext);
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      resultMessage = (string) null;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TracePoint, "Indexing Pipeline", "Job", nameof (Run));
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      try
      {
        this.ValidateRequestContext(requestContext);
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          resultMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} not supported in OnPrem environment.", (object) jobDefinition.Name);
          return jobExecutionResult;
        }
        this.Initialize(requestContext, jobDefinition);
        this.MonitorOnGoingJob(requestContext);
        if (this.PreRunCheck(requestContext))
          jobExecutionResult = this.InvokeJob(requestContext, out resultMessage);
        else
          resultMessage += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Pre Run Check not met for the job {0} \n", (object) jobDefinition.Name);
        if (this.m_lastJobStatus.IsInProgressOrPending())
          resultMessage += this.QueueSelfJob(requestContext, this._eventDelayIntervalMap[this.m_lastJobStatus]);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.TracePoint, "Indexing Pipeline", "Job", ex);
        resultMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} failed with error : {1}", (object) jobDefinition.Name, (object) ex);
        TeamFoundationEventLog.Default.Log(resultMessage, this.SearchEventIdentifier, EventLogEntryType.Error);
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TracePoint, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return jobExecutionResult;
    }

    protected internal virtual string QueueSelfJob(IVssRequestContext requestContext, int delay)
    {
      requestContext.QueueDelayedNamedJob(this.JobId, delay, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083040, "Indexing Pipeline", "Job", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Queued next iteration of {0} to run after {1} seconds", (object) this.JobName, (object) delay));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Re-Queuing self for {0} \n", (object) (this.m_lastJobStatus == MaintenanceJobStatus.InProgress ? "Monitoring" : "Retrying"))));
    }

    protected internal void QueueSelfJobForMonitoringInProgressProcess(
      IVssRequestContext requestContext)
    {
      this.QueueSelfJob(requestContext, this._eventDelayIntervalMap[MaintenanceJobStatus.InProgress]);
    }

    protected internal abstract TeamFoundationJobExecutionResult InvokeJob(
      IVssRequestContext requestContext,
      out string resultMessage);

    protected internal abstract bool PreRunCheck(IVssRequestContext requestContext);

    protected internal virtual IDictionary<MaintenanceJobStatus, int> GetEventDelayIntervalMap(
      IVssRequestContext requestContext)
    {
      return (IDictionary<MaintenanceJobStatus, int>) new Dictionary<MaintenanceJobStatus, int>()
      {
        [MaintenanceJobStatus.InProgress] = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ESMaintenanceJobMonitoringDelay", TeamFoundationHostType.Deployment, 3600),
        [MaintenanceJobStatus.Pending] = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/InvokeJobRetryDelay", TeamFoundationHostType.Deployment)
      };
    }

    protected internal abstract void UpdateMaintenanceJobStatus(
      IVssRequestContext requestContext,
      MaintenanceJobStatus maintenanceJobStatus);

    protected internal abstract void MonitorOnGoingJob(IVssRequestContext requestContext);

    [Info("InternalForTestPurpose")]
    internal void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
    }
  }
}
