// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.RecoveryJobMaster
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob
{
  public abstract class RecoveryJobMaster : VssAsyncJobExtension
  {
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.RecoveryJobMaster.TraceData;
    private RecoveryJobMasterTelemetryData telemetryData;

    protected abstract IProtocol Protocol { get; }

    protected abstract string ProtocolReadOnlyFeatureFlagName { get; }

    protected abstract string ProtocolDisasterRecoveryBypassFeatureName { get; }

    protected abstract Guid ProtocolCollectionChangeProcessingJobId { get; }

    protected abstract JobCreationInfo RecoveryWorkerJobCreationInfo { get; }

    protected abstract string ProtocolAlternateMetadataRevisionKey { get; }

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.telemetryData = new RecoveryJobMasterTelemetryData();
      DeploymentLevelRegistryServiceFacade registryServiceFacade = new DeploymentLevelRegistryServiceFacade(vssRequestContext);
      CollectionContextFactoryFacade<IDisposableFeedService> feedServiceFromCollectionFactory = new CollectionContextFactoryFacade<IDisposableFeedService>(vssRequestContext, (Func<IVssRequestContext, IDisposableFeedService>) (collectionContext => (IDisposableFeedService) new ContextDisposingFeedServiceFacade(collectionContext, (IFeedService) new FeedServiceFacade(collectionContext))));
      CollectionContextFactoryFacade<IDisposingJobQueuer> jobQueuerFromCollectionFactory = new CollectionContextFactoryFacade<IDisposingJobQueuer>(vssRequestContext, (Func<IVssRequestContext, IDisposingJobQueuer>) (collectionContext => (IDisposingJobQueuer) new DisposingJobQueuer((IJobQueuer) new JobServiceFacade(collectionContext, collectionContext.GetService<ITeamFoundationJobService>()), collectionContext)));
      VssJobResult vssJobResult;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(vssRequestContext, RecoveryJobMaster.TraceData, 5725800, nameof (RunAsync)))
      {
        TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
        try
        {
          if (!requestContext.IsFeatureEnabledWithLogging(this.ProtocolReadOnlyFeatureFlagName))
            throw new Exception("The service must be in ReadOnly mode for recovery to function.");
          DateTime result1;
          if (!DateTime.TryParse(registryServiceFacade.GetValue<string>((RegistryQuery) "/Configuration/Recovery/RecoveryUTCTimePoint", string.Empty, false), out result1) || result1.CompareTo(new DefaultTimeProvider().Now) >= 0)
            throw new Exception("You must specify a valid recovery time point for the recovery window.");
          this.telemetryData.RecoveryUtcTimePoint = result1.AddMinutes(-5.0);
          string feedId = registryServiceFacade.GetValue<string>((RegistryQuery) "/Configuration/Recovery/FeedId", string.Empty, false);
          this.telemetryData.FeedId = feedId;
          Guid parsedFeedId = Guid.Empty;
          if (string.IsNullOrWhiteSpace(feedId))
          {
            tracer.TraceInfo("Recovery job started - total recovery.");
          }
          else
          {
            if (!Guid.TryParse(feedId, out parsedFeedId))
              throw new ArgumentException("Invalid feed ID: " + feedId);
            tracer.TraceInfo("Recovery job started for feed " + feedId);
          }
          bool performRepair = registryServiceFacade.GetValue<bool>((RegistryQuery) "/Configuration/Recovery/PerformRepair", false, false);
          this.telemetryData.PerformRepair = performRepair;
          tracer.TraceInfo(performRepair ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Recovery Job running in Perform Repair mode. Data will be changed") : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Recovery Job running in Verification mode. Data will not be changed"));
          TeamFoundationHostManagementService hostMgmtSvc = vssRequestContext.GetService<TeamFoundationHostManagementService>();
          IEnumerable<HostProperties> source1 = hostMgmtSvc.QueryServiceHostProperties(vssRequestContext, new DateTime?(), new DateTime?()).Where<HostProperties>((Func<HostProperties, bool>) (p => p.HostType == TeamFoundationHostType.ProjectCollection));
          this.telemetryData.NumberOfCollection = source1.Count<HostProperties>();
          Dictionary<Guid, List<Guid>> collectionRepairJobs = new Dictionary<Guid, List<Guid>>();
          foreach (HostProperties collection in source1)
          {
            IEnumerable<FeedCore> enumerable;
            using (IDisposableFeedService feedService = feedServiceFromCollectionFactory.Get((CollectionId) collection.Id))
            {
              if (!string.IsNullOrWhiteSpace(feedId))
              {
                try
                {
                  List<FeedCore> feedCoreList1 = new List<FeedCore>();
                  List<FeedCore> feedCoreList2 = feedCoreList1;
                  feedCoreList2.Add(await feedService.GetFeedByIdForAnyScopeAsync(parsedFeedId, true));
                  enumerable = (IEnumerable<FeedCore>) feedCoreList1;
                  feedCoreList2 = (List<FeedCore>) null;
                  feedCoreList1 = (List<FeedCore>) null;
                }
                catch (FeedIdNotFoundException ex)
                {
                  continue;
                }
              }
              else
                enumerable = await feedService.GetFeedsAsync(true);
              if (enumerable.IsNullOrEmpty<FeedCore>())
              {
                ++this.telemetryData.NumberOfCollectionWithoutFeed;
                continue;
              }
            }
            List<Guid> source2 = new List<Guid>();
            using (IDisposingJobQueuer disposingJobQueuer = jobQueuerFromCollectionFactory.Get((CollectionId) collection.Id))
            {
              foreach (FeedCore feed in enumerable)
              {
                Guid guid = disposingJobQueuer.QueueOneTimeJob(this.RecoveryWorkerJobCreationInfo.JobName, this.RecoveryWorkerJobCreationInfo.JobExtensionName, FeedRequestJob.SerializeFeedJobDefinition(feed), JobPriorityLevel.Highest, this.RecoveryWorkerJobCreationInfo.PriorityClass);
                source2.Add(guid);
                tracer.TraceInfo(string.Format("Queued RecoveryWorkerJob for {0}. Job ID: {1}", (object) feedId, (object) guid));
                ++this.telemetryData.RecoveryWorkerJobsQueued;
              }
            }
            collectionRepairJobs[collection.Id] = source2.ToList<Guid>();
            tracer.TraceInfo(string.Format("Queued RecoveryWorkerJobs for collection {0}.", (object) collection.Id));
            ++this.telemetryData.CollectionsQueuedForRecovery;
          }
          if (!this.ValidateCollectionRepairAndQueueChangeProcessingJobs(requestContext, (ITeamFoundationHostManagementService) hostMgmtSvc, collectionRepairJobs, performRepair))
            result = TeamFoundationJobExecutionResult.Failed;
          feedId = (string) null;
          parsedFeedId = new Guid();
          hostMgmtSvc = (TeamFoundationHostManagementService) null;
          collectionRepairJobs = (Dictionary<Guid, List<Guid>>) null;
        }
        catch (Exception ex)
        {
          result = TeamFoundationJobExecutionResult.Failed;
          tracer.TraceException(ex);
          this.telemetryData.LogException(ex);
        }
        vssJobResult = new JobResult()
        {
          Result = result,
          Telemetry = ((JobTelemetry) this.telemetryData)
        }.ToVssJobResult();
      }
      feedServiceFromCollectionFactory = (CollectionContextFactoryFacade<IDisposableFeedService>) null;
      jobQueuerFromCollectionFactory = (CollectionContextFactoryFacade<IDisposingJobQueuer>) null;
      return vssJobResult;
    }

    private bool ValidateCollectionRepairAndQueueChangeProcessingJobs(
      IVssRequestContext deploymentContext,
      ITeamFoundationHostManagementService hostManagementService,
      Dictionary<Guid, List<Guid>> jobs,
      bool performRepair)
    {
      bool flag1 = true;
      List<Guid> guidList = new List<Guid>();
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(deploymentContext, RecoveryJobMaster.TraceData, 5725800, nameof (ValidateCollectionRepairAndQueueChangeProcessingJobs)))
      {
        bool flag2;
        do
        {
          flag2 = false;
          foreach (Guid key in jobs.Keys)
          {
            if (!guidList.Contains(key))
            {
              using (IVssRequestContext vssRequestContext = hostManagementService.BeginRequest(deploymentContext, key, RequestContextType.SystemContext))
              {
                ITeamFoundationJobService service1 = vssRequestContext.GetService<ITeamFoundationJobService>();
                List<TeamFoundationJobHistoryEntry> source = service1.QueryJobHistory(vssRequestContext, (IEnumerable<Guid>) jobs[key]);
                if (source.Count == jobs[key].Count)
                {
                  int num1 = source.Count<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (j => j.Result != 0));
                  if (num1 > 0)
                  {
                    tracer.TraceAlways(string.Format("{0} RecoveryWorkerJobs failed for collection {1}.", (object) num1, (object) key));
                    this.telemetryData.RecoveryWorkerJobsFailed += num1;
                    flag1 = false;
                  }
                  else
                  {
                    this.telemetryData.RecoveryWorkerJobsSucceeded += source.Count;
                    if (performRepair)
                    {
                      tracer.TraceInfo(string.Format("Recovery jobs for collection {0} were successful. Scheduling CP jobs.", (object) key));
                      IVssRegistryService service2 = vssRequestContext.GetService<IVssRegistryService>();
                      int num2 = service2.GetValue<int>(vssRequestContext, (RegistryQuery) this.ProtocolAlternateMetadataRevisionKey, 0);
                      service2.SetValue<int>(vssRequestContext, this.ProtocolAlternateMetadataRevisionKey, num2 + 1);
                      vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, this.ProtocolDisasterRecoveryBypassFeatureName, FeatureAvailabilityState.On);
                      service1.QueueJobsNow(vssRequestContext, (IEnumerable<Guid>) new List<Guid>()
                      {
                        this.ProtocolCollectionChangeProcessingJobId
                      });
                      ++this.telemetryData.CollectionChangeProcessingJobsQueued;
                    }
                    else
                      tracer.TraceInfo(string.Format("Recovery jobs for collection {0} were successful. Perform repair was set to false. Scan complete.", (object) key));
                  }
                  ++this.telemetryData.CollectionsCompletedRecovery;
                  guidList.Add(key);
                }
                else
                  flag2 = true;
              }
            }
          }
        }
        while (flag2);
      }
      return flag1;
    }
  }
}
