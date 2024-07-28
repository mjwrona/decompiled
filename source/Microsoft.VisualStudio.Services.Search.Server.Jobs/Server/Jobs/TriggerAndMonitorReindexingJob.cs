// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.TriggerAndMonitorReindexingJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class TriggerAndMonitorReindexingJob : ITeamFoundationJobExtension
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private int m_batchSize;
    private int m_jobDelayInSecs;
    private int m_accountFaultinJobDelayInSecs;
    internal static readonly string[] SupportedEntityTypes = new string[4]
    {
      "Code",
      "WorkItem",
      "Wiki",
      "Package"
    };
    [StaticSafe("Grandfathered")]
    internal static readonly Dictionary<string, Guid> AccountReIndexJobIDs = new Dictionary<string, Guid>()
    {
      ["Code"] = JobConstants.AccountFaultInJobId,
      ["WorkItem"] = JobConstants.WorkItemAccountFaultInJobId,
      ["Wiki"] = JobConstants.WikiAccountFaultInJobId,
      ["Package"] = JobConstants.PackageAccountReindexerJobId
    };

    private IReindexingStatusDataAccess ReindexingStatusDataAccess { get; }

    private IDataAccessFactory DataAccessFactory { get; }

    internal StringBuilder ResultMessage { get; } = new StringBuilder();

    public TriggerAndMonitorReindexingJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal TriggerAndMonitorReindexingJob(IDataAccessFactory dataAccessFactory)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.ReindexingStatusDataAccess = this.DataAccessFactory.GetReindexingStatusDataAccess();
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080338, "Indexing Pipeline", "Job", nameof (Run));
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        bool flag = false;
        this.m_batchSize = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForReindexing");
        this.m_jobDelayInSecs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/TriggerAndMonitorReindexingJobDelayInSec");
        this.m_accountFaultinJobDelayInSecs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/AccountFaultInJobDelayInSec");
        try
        {
          List<ReindexingStatusEntry> reindexingStatusEntryList = new List<ReindexingStatusEntry>();
          IEnumerable<ReindexingStatusEntry> reindexingStatusEntries1 = this.ReindexingStatusDataAccess.GetReindexingStatusEntries(requestContext, -1, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress);
          IEnumerable<ReindexingStatusEntry> reindexingStatusEntries2 = this.ReindexingStatusDataAccess.GetReindexingStatusEntries(requestContext, -1, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Queued);
          reindexingStatusEntryList.AddRange(reindexingStatusEntries1);
          reindexingStatusEntryList.AddRange(reindexingStatusEntries2);
          if (reindexingStatusEntryList.Count >= this.m_batchSize)
          {
            this.QueueTriggerAndMonitorReindexingJob(requestContext);
            flag = true;
            this.ResultMessage.Append("Reindexing is in progress or queued for maximum accounts, so waiting for them to complete. ");
            jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
          }
          else
          {
            int count = this.m_batchSize - reindexingStatusEntryList.Count;
            IEnumerable<ReindexingStatusEntry> reindexingStatusEntries3 = this.ReindexingStatusDataAccess.GetReindexingStatusEntries(requestContext, count, Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.ReindexingRequired);
            if (reindexingStatusEntries3 != null && reindexingStatusEntries3.Any<ReindexingStatusEntry>())
            {
              int num = this.QueueJobsAndUpdateFeatureFlags(requestContext, reindexingStatusEntries3);
              this.QueueTriggerAndMonitorReindexingJob(requestContext);
              flag = true;
              this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued Account Faultin Jobs Successfully for {0} accounts. ", (object) num)));
              jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
            }
            else if (reindexingStatusEntryList.Count > 0)
            {
              this.MonitorInProgressAndQueuedEntries(requestContext, reindexingStatusEntryList);
              this.QueueTriggerAndMonitorReindexingJob(requestContext);
              flag = true;
              string str = string.Join(", ", reindexingStatusEntryList.Select<ReindexingStatusEntry, string>((Func<ReindexingStatusEntry, string>) (x => x.ToString())));
              this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Reindexing is in progress for last batch of size {0} so queueing next iteraion of TriggerAndMonitorReindexingJob to monitor the completion. ", (object) reindexingStatusEntryList.Count)));
              this.ResultMessage.Append("Reindexing Entries: " + str);
              jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
            }
            else
            {
              flag = true;
              this.ResultMessage.Append("All the accounts got reindexed, so exiting. ");
              jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
            }
          }
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080338, "Indexing Pipeline", "Job", ex);
          this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("TriggerAndMonitorReindexingJob failed with error : {0}. ", (object) ex)));
          TeamFoundationEventLog.Default.Log(this.ResultMessage.ToString(), SearchEventId.TriggerAndMonitorReindexingJobFailed, EventLogEntryType.Error);
          jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
        }
        finally
        {
          if (!flag)
            this.QueueTriggerAndMonitorReindexingJob(requestContext);
        }
      }
      finally
      {
        resultMessage = this.ResultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080338, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return jobExecutionResult;
    }

    protected internal int QueueJobsAndUpdateFeatureFlags(
      IVssRequestContext requestContext,
      IEnumerable<ReindexingStatusEntry> reindexingEntries)
    {
      int num = 0;
      foreach (ReindexingStatusEntry reindexingEntry in reindexingEntries)
      {
        try
        {
          using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, reindexingEntry.CollectionId, RequestContextType.SystemContext))
          {
            IVssRequestContext requestContext1 = vssRequestContext.To(TeamFoundationHostType.ProjectCollection).Elevate();
            IEntityType entityType = reindexingEntry.EntityType;
            if (!((IEnumerable<string>) TriggerAndMonitorReindexingJob.SupportedEntityTypes).Contains<string>(entityType.Name))
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The specified entity [{0}] is not supported, reindexing cannot be performed. ", (object) entityType));
            if (!requestContext1.IsIndexingEnabled(entityType))
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Indexing is not enabled for collection Id: {0}, reindexing cannot be performed. ", (object) reindexingEntry.CollectionId.ToString()));
            this.SetCrudOperationsFeatureFlagsState(vssRequestContext, reindexingEntry.EntityType, FeatureAvailabilityState.Off);
            this.QueueAccountReIndexingJob(vssRequestContext, reindexingEntry.EntityType);
          }
          this.ReindexingStatusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext, new ReindexingStatusEntry(reindexingEntry.CollectionId, reindexingEntry.EntityType)
          {
            Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Queued
          });
          ++num;
        }
        catch (Exception ex)
        {
          string message = FormattableString.Invariant(FormattableStringFactory.Create("Failed to trigger reindexing for collectionId: {0} due to exception:\n {1}. ", (object) reindexingEntry.CollectionId.ToString(), (object) ex.Message));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080338, "Indexing Pipeline", "Job", message);
          this.ResultMessage.Append(message);
          reindexingEntry.Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed;
          this.ReindexingStatusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext, reindexingEntry);
          IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
          properties.Add("ReindexingFailed", (object) 1);
          properties.Add("EntityType", (object) reindexingEntry.EntityType.Name);
          properties.Add("CollectionId", (object) reindexingEntry.CollectionId.ToString());
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Job", properties);
        }
      }
      return num;
    }

    protected internal void MonitorInProgressAndQueuedEntries(
      IVssRequestContext requestContext,
      List<ReindexingStatusEntry> reindexingEntriesToBeMonitored)
    {
      if (reindexingEntriesToBeMonitored == null || reindexingEntriesToBeMonitored.Count == 0)
        throw new ArgumentNullException(nameof (reindexingEntriesToBeMonitored));
      if (reindexingEntriesToBeMonitored.Any<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (x => x.Status != Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress && x.Status != Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Queued)))
        throw new ArgumentException("Expected InProgress or Queued entries. ");
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxTimeIntervalForAccountReindexingInDays");
      foreach (ReindexingStatusEntry entry in reindexingEntriesToBeMonitored)
      {
        try
        {
          if (DateTime.UtcNow > entry.LastUpdatedTimeStamp.AddDays((double) configValue))
          {
            using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, entry.CollectionId, RequestContextType.SystemContext))
              this.SetCrudOperationsFeatureFlagsState(vssRequestContext.To(TeamFoundationHostType.ProjectCollection).Elevate(), entry.EntityType, FeatureAvailabilityState.On);
            this.ReindexingStatusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext, entry);
            string message = FormattableString.Invariant(FormattableStringFactory.Create("Collection with id {0} and entity type {1} is stuck in re-indexing for longer than expected. ", (object) entry.CollectionId.ToString(), (object) entry.EntityType.Name));
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080338, "Indexing Pipeline", "Job", message);
            this.ResultMessage.Append(message);
            IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
            properties.Add("ReindexingFailed", (object) 1);
            properties.Add("EntityType", (object) entry.EntityType.Name);
            properties.Add("CollectionId", (object) entry.CollectionId.ToString());
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Job", properties);
            TeamFoundationEventLog.Default.Log(message, SearchEventId.TriggerAndMonitorReindexingJobWarning, EventLogEntryType.Error);
          }
        }
        catch (Exception ex)
        {
          string message = FormattableString.Invariant(FormattableStringFactory.Create("Failed to enable FFs for collectionId: {0} due to exception:\n {1}. ", (object) entry.CollectionId.ToString(), (object) ex.Message));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080338, "Indexing Pipeline", "Job", message);
          this.ResultMessage.Append(message);
        }
      }
    }

    private void QueueTriggerAndMonitorReindexingJob(IVssRequestContext requestContext)
    {
      requestContext.QueueDelayedNamedJob(JobConstants.TriggerAndMonitorReindexingJob, this.m_jobDelayInSecs, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080338, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Queued next iteration of TriggerAndMonitorReindexingJob to run after {0} seconds. ", (object) this.m_jobDelayInSecs)));
    }

    internal virtual void QueueAccountReIndexingJob(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      Guid jobId;
      if (!TriggerAndMonitorReindexingJob.AccountReIndexJobIDs.TryGetValue(entityType.Name, out jobId))
        return;
      requestContext.QueueDelayedNamedJob(jobId, this.m_accountFaultinJobDelayInSecs, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080338, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Queued Account reindexing Job for EntityType: {0}, reindexing collectionId: {1} to run after {2} seconds. ", (object) entityType, (object) requestContext.ServiceHost.InstanceId.ToString(), (object) this.m_accountFaultinJobDelayInSecs)));
    }

    internal virtual void SetCrudOperationsFeatureFlagsState(
      IVssRequestContext elevatedContext,
      IEntityType entity,
      FeatureAvailabilityState availabilityState)
    {
      List<string> stringList = new List<string>();
      switch (entity.Name)
      {
        case "Code":
          if (!elevatedContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
          {
            stringList.Add("Search.Server.Code.CrudOperations");
            break;
          }
          break;
        case "WorkItem":
          if (!elevatedContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled())
          {
            stringList.Add("Search.Server.WorkItem.CrudOperations");
            break;
          }
          break;
        case "Wiki":
          stringList.Add("Search.Server.Wiki.ContinuousIndexing");
          break;
        case "Package":
          stringList.Add("Search.Server.Package.ContinuousIndexing");
          break;
      }
      if (!stringList.Any<string>())
        return;
      this.ApplyFeatureFlagStates(elevatedContext, (IEnumerable<string>) stringList, availabilityState);
    }

    private void ApplyFeatureFlagStates(
      IVssRequestContext elevatedContext,
      IEnumerable<string> featureFlags,
      FeatureAvailabilityState state)
    {
      ITeamFoundationFeatureAvailabilityService service = elevatedContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      foreach (string featureFlag in featureFlags)
        service.SetFeatureState(elevatedContext, featureFlag, state);
      string str = string.Join(", ", featureFlags);
      if (string.IsNullOrWhiteSpace(str))
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080338, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("FeatureFlag:{0} Changed state to: {1}. ", (object) str, (object) state)));
    }
  }
}
