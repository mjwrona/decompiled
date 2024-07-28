// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.RecoveryWorkerJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob
{
  public abstract class RecoveryWorkerJob : VssAsyncJobExtension
  {
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.RecoveryWorkerJob.TraceData;
    private RecoveryWorkerJobFeedTelemetryData telemetryData;

    public abstract BookmarkTokenKey BookmarkTokenKey { get; }

    public abstract IProtocol GetProtocol();

    public abstract IUnsafeCommitLogService GetUnsafeCommitLogService(
      IVssRequestContext requestContext);

    public abstract Task<IEnumerable<Uri>> GetAddOperationDownloadUrisAsync(
      IVssRequestContext requestContext,
      IAddOperationData operationData);

    public virtual IBookmarkTokenProvider<FeedCore, CommitLogBookmark> GetBookmarkTokenProvider(
      IVssRequestContext requestContext)
    {
      return new CommitLogBookmarkTokenProviderBootstrapper(requestContext, this.BookmarkTokenKey).Bootstrap();
    }

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      RecoveryWorkerJob recoveryWorkerJob1 = this;
      DeploymentLevelRegistryServiceFacade registryServiceFacade = new DeploymentLevelRegistryServiceFacade(requestContext.To(TeamFoundationHostType.Deployment));
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      Guid feedId = Guid.Parse(FeedRequestJob.ParseFeedJobDefinition(jobDefinition.Data.InnerText));
      RecoveryWorkerJob recoveryWorkerJob2 = recoveryWorkerJob1;
      RecoveryWorkerJobFeedTelemetryData feedTelemetryData = new RecoveryWorkerJobFeedTelemetryData();
      feedTelemetryData.FeedId = feedId;
      recoveryWorkerJob2.telemetryData = feedTelemetryData;
      VssJobResult vssJobResult;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (RunAsync)))
      {
        try
        {
          IProtocol protocol = recoveryWorkerJob1.GetProtocol();
          if (!requestContext.IsFeatureEnabledWithLogging(protocol.ReadOnlyFeatureFlagName))
            throw new Exception("The read-only flag for " + protocol.CorrectlyCasedName + " is expected to be set.");
          if (requestContext.IsFeatureEnabledWithLogging(protocol.DisasterRecoveryChangeProcessingBypassFeatureFlagName))
            throw new Exception(string.Format("Collection {0} is already in disaster read-only bypass mode. ", (object) requestContext.ServiceHost.InstanceId) + "There could potentially be a CP job running on a repaired commit log.");
          bool performRepair = registryServiceFacade.GetValue<bool>((RegistryQuery) "/Configuration/Recovery/PerformRepair", false, false);
          DateTime result1;
          if (!DateTime.TryParse(registryServiceFacade.GetValue<string>((RegistryQuery) "/Configuration/Recovery/RecoveryUTCTimePoint", string.Empty, false), out result1) || result1.CompareTo(new DefaultTimeProvider().Now) >= 0)
            throw new Exception("You must specify a valid recovery time point for the recovery window.");
          RecoveryWorkerJob.WaitUntilPackagingJobsFinish(requestContext, feedId);
          result1 = result1.AddMinutes(-5.0);
          await recoveryWorkerJob1.ProcessCommitLogAsync(requestContext, feedId, performRepair, result1);
        }
        catch (Exception ex)
        {
          result = TeamFoundationJobExecutionResult.Failed;
          tracer.TraceException(ex);
          recoveryWorkerJob1.telemetryData.LogException(ex);
        }
        vssJobResult = new JobResult()
        {
          Result = result,
          Telemetry = ((JobTelemetry) recoveryWorkerJob1.telemetryData)
        }.ToVssJobResult();
      }
      return vssJobResult;
    }

    private async Task ProcessCommitLogAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      bool performRepair,
      DateTime riskWindowStart)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (ProcessCommitLogAsync)))
      {
        FeedCore feed = await new FeedServiceFacade(requestContext).GetFeedByIdForAnyScopeAsync(feedId, true, (Func<FeedCore, bool>) null);
        IList<CommitLogEntry> list = (IList<CommitLogEntry>) (await this.GetUnsafeCommitLogService(requestContext).BatchReadAsync(requestContext, feed, this.GetProtocol().CommitLogItemType, new DateTime?(riskWindowStart))).OrderBy<CommitLogEntry, long>((Func<CommitLogEntry, long>) (entry => entry.SequenceNumber)).ToList<CommitLogEntry>();
        if (list.Count == 0)
        {
          tracer.TraceInfo(string.Format("Skipping empty commit log for feed {0}", (object) feedId));
          return;
        }
        this.telemetryData.ReadCommitLogEntries = list.Count;
        Dictionary<string, List<CommitLogEntry>> commitLogEntries = await this.FindInvalidCommitLogEntries(requestContext, (IEnumerable<CommitLogEntry>) list);
        IList<CommitLogEntry> invalidList = this.FilterOutPermanentlyDeletedPackages(requestContext, (IDictionary<string, List<CommitLogEntry>>) commitLogEntries);
        if (!performRepair)
          return;
        if (invalidList.Count > 0)
          await this.DeleteInvalidCommitLogEntriesAsync(requestContext, (IEnumerable<CommitLogEntry>) invalidList, feed);
        this.GetBookmarkTokenProvider(requestContext).StoreToken(feed, CommitLogBookmark.Empty);
        this.telemetryData.ChangeProcessingBookmarkReset = true;
        feed = (FeedCore) null;
      }
    }

    private static void WaitUntilPackagingJobsFinish(IVssRequestContext requestContext, Guid feedId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      Dictionary<Guid, TeamFoundationJobQueueEntry> runningJobsSnapshot = service.QueryRunningJobs(requestContext).ToDictionary<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (x => x.JobId));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (WaitUntilPackagingJobsFinish)))
      {
        List<TeamFoundationJobDefinition> source;
        do
        {
          IEnumerable<Guid> jobIds = service.QueryRunningJobs(requestContext).Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (x => x.JobId));
          source = service.QueryJobDefinitions(requestContext, jobIds);
          tracer.TraceInfo(string.Format("RecoveryWorkerJob for feedId {0} is waiting on Packaging non-DR jobs to finish.", (object) feedId));
          Thread.Sleep(TimeSpan.FromSeconds(1.0));
        }
        while (source != null && source.Any<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (x => x != null && runningJobsSnapshot.ContainsKey(x.JobId) && x.ExtensionName.Contains("Packaging") && !x.ExtensionName.Contains("Recovery"))));
      }
    }

    private IList<CommitLogEntry> FilterOutPermanentlyDeletedPackages(
      IVssRequestContext requestContext,
      IDictionary<string, List<CommitLogEntry>> tempInvalidCommitLogEntries)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (FilterOutPermanentlyDeletedPackages)))
      {
        List<CommitLogEntry> commitLogEntryList = new List<CommitLogEntry>();
        foreach (KeyValuePair<string, List<CommitLogEntry>> invalidCommitLogEntry in (IEnumerable<KeyValuePair<string, List<CommitLogEntry>>>) tempInvalidCommitLogEntries)
        {
          if (invalidCommitLogEntry.Value.Last<CommitLogEntry>().CommitOperationData is IPermanentDeleteOperationData)
          {
            tracer.TraceInfo("Excluding package " + invalidCommitLogEntry.Key + " since it was permanently deleted.");
          }
          else
          {
            commitLogEntryList.AddRange((IEnumerable<CommitLogEntry>) invalidCommitLogEntry.Value);
            string str = string.Join(",", invalidCommitLogEntry.Value.Select<CommitLogEntry, string>((Func<CommitLogEntry, string>) (entry => entry.CommitId.ToString())));
            tracer.TraceAlways("Commit log entries to be deleted for " + invalidCommitLogEntry.Key + ": " + str);
          }
        }
        this.telemetryData.InvalidCommitLogEntries = commitLogEntryList.Count;
        return (IList<CommitLogEntry>) commitLogEntryList;
      }
    }

    private async Task DeleteInvalidCommitLogEntriesAsync(
      IVssRequestContext requestContext,
      IEnumerable<CommitLogEntry> invalidList,
      FeedCore feed)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (DeleteInvalidCommitLogEntriesAsync)))
      {
        IUnsafeCommitLogService unsafeCommitLog = this.GetUnsafeCommitLogService(requestContext);
        foreach (CommitLogEntry invalid in invalidList)
        {
          List<CommitLogRepairItem> source = await unsafeCommitLog.DeleteEntryAsync(requestContext, feed, invalid.CommitId);
          if (source.Count == 0)
            throw new Exception(string.Format("Deletion of commit log entry failed for CommitId: {0}.", (object) invalid.CommitId));
          this.telemetryData.RepairedItems += source.Count;
          string str = string.Join(",", source.Select<CommitLogRepairItem, string>((Func<CommitLogRepairItem, string>) (x => x.ToString())));
          tracer.TraceAlways("Deleted commit " + invalid.CommitId.ToString() + ". Repaired items: " + str);
        }
        unsafeCommitLog = (IUnsafeCommitLogService) null;
      }
    }

    private async Task<Dictionary<string, List<CommitLogEntry>>> FindInvalidCommitLogEntries(
      IVssRequestContext requestContext,
      IEnumerable<CommitLogEntry> entries)
    {
      Dictionary<string, List<CommitLogEntry>> commitLogEntries;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer1 = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (FindInvalidCommitLogEntries)))
      {
        Dictionary<string, List<CommitLogEntry>> invalidList = new Dictionary<string, List<CommitLogEntry>>();
        foreach (CommitLogEntry entry1 in entries)
        {
          CommitLogEntry entry = entry1;
          PackagingCommitId commitId;
          if (!(entry.CommitOperationData is IPackageVersionOperationData commitOperationData))
          {
            Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer2 = tracer1;
            commitId = entry.CommitId;
            string message = "Found commit log entry that does not operate on a package version; commit: " + commitId.ToString();
            tracer2.TraceInfo(message);
          }
          else
          {
            if (commitOperationData is IAddOperationData addOperationData)
            {
              if (!await this.CheckIfBlobExists(requestContext, addOperationData))
              {
                string key = addOperationData.Identity.Name.NormalizedName + "_" + addOperationData.Identity.Version.NormalizedVersion;
                invalidList.Add(key, new List<CommitLogEntry>()
                {
                  entry
                });
                Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer3 = tracer1;
                string[] strArray = new string[5]
                {
                  "Added AddOperation commit ",
                  null,
                  null,
                  null,
                  null
                };
                commitId = entry.CommitId;
                strArray[1] = commitId.ToString();
                strArray[2] = " for package ";
                strArray[3] = key;
                strArray[4] = " to invalid list.";
                string message = string.Concat(strArray);
                tracer3.TraceInfo(message);
              }
              else
                continue;
            }
            else
            {
              string key = commitOperationData.Identity.Name.NormalizedName + "_" + commitOperationData.Identity.Version.NormalizedVersion;
              if (invalidList.ContainsKey(key))
              {
                invalidList[key].Add(entry);
                Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer4 = tracer1;
                string[] strArray = new string[5]
                {
                  "Added non-AddOperation commit ",
                  null,
                  null,
                  null,
                  null
                };
                commitId = entry.CommitId;
                strArray[1] = commitId.ToString();
                strArray[2] = " for package ";
                strArray[3] = key;
                strArray[4] = " to invalid list.";
                string message = string.Concat(strArray);
                tracer4.TraceInfo(message);
              }
            }
            addOperationData = (IAddOperationData) null;
            entry = (CommitLogEntry) null;
          }
        }
        commitLogEntries = invalidList;
      }
      return commitLogEntries;
    }

    private async Task<bool> CheckIfBlobExists(
      IVssRequestContext requestContext,
      IAddOperationData addOperationData)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, RecoveryWorkerJob.TraceData, 5725900, nameof (CheckIfBlobExists)))
      {
        try
        {
          foreach (Uri requestUri in await this.GetAddOperationDownloadUrisAsync(requestContext, addOperationData))
          {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            httpWebRequest.Method = HttpMethod.Head.Method;
            httpWebRequest.AllowAutoRedirect = true;
            HttpWebResponse responseAsync = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
          }
        }
        catch (WebException ex)
        {
          if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
          {
            tracer.TraceAlways("The data blob for package: " + addOperationData.Identity.Name.DisplayName + "." + addOperationData.Identity.Version.DisplayVersion + " could not be found.");
            return false;
          }
          throw;
        }
        tracer.TraceInfo(string.Format("Found data blob for package: {0}.{1}.", (object) addOperationData.Identity.Name, (object) addOperationData.Identity.Version));
        return true;
      }
    }
  }
}
