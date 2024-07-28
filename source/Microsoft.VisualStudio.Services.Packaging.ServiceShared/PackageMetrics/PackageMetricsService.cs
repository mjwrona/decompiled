// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics.PackageMetricsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics
{
  public class PackageMetricsService : IPackageMetricsService, IVssFrameworkService
  {
    private static readonly TraceData TraceData = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants.TracePoints.PackageMetricsService.TraceData;
    private bool flushTaskQueued;
    private ILockName QueueFlushDownloadStatsLockName;
    private int _packageCount;
    protected internal readonly ConcurrentDictionary<string, PackageMetricsData> _currentDownloadStats = new ConcurrentDictionary<string, PackageMetricsData>();
    private PackageMetricsSettings _packageMetricsSettings;
    private static readonly RegistryQuery packageMetricsSettingsRegistryQuery = (RegistryQuery) (PackageMetricsConstants.PackageMetricsSettingsRoot + "*");

    public PackageMetricsService()
    {
    }

    internal PackageMetricsService(
      IVssRequestContext requestContext,
      PackageMetricsSettings packageMetricsSettings)
    {
      this._packageMetricsSettings = packageMetricsSettings;
      this.QueueFlushDownloadStatsLockName = requestContext.ServiceHost.CreateUniqueLockName(this.GetType().FullName + "/QueueFlushDownloadStatsLock");
    }

    public Task UpdatePackageMetricsAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      Guid? projectId,
      IProtocol protocol,
      IPackageIdentity packageIdentity,
      double downloadCount = 1.0)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetricsService.TraceData, 5726700, nameof (UpdatePackageMetricsAsync)))
      {
        try
        {
          if (this._packageMetricsSettings.IsPackageMetricsEnabled(requestContext))
          {
            int num = Volatile.Read(ref this._packageCount);
            if (num > this._packageMetricsSettings.MaxDownloadStatsDictionarySize)
            {
              tracer.TraceError(string.Format("PackageMetricsQueueOverflow Error: The current length of the package metrics queue is: {0} which is longer than the max length of {1}", (object) num, (object) this._packageMetricsSettings.MaxDownloadStatsDictionarySize));
              return Task.CompletedTask;
            }
            if (num >= this._packageMetricsSettings.SaveDownloadStatsDictionarySize && !this.flushTaskQueued)
            {
              tracer.TraceInfo(string.Format("Flushing package metrics queue as it exceeds the save threshold. Current length : {0} SaveThreshold : {1}", (object) num, (object) this._packageMetricsSettings.SaveDownloadStatsDictionarySize), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
              this.FlushStats(requestContext);
            }
            string str = requestContext.GetUserId().ToString();
            PackageMetricsData packageMetricsData = new PackageMetricsData()
            {
              Protocol = protocol.LowercasedName,
              FeedId = feedId,
              ProjectId = projectId,
              NormalizedPackageName = packageIdentity.Name.NormalizedName,
              NormalizedPackageVersion = packageIdentity.Version.NormalizedVersion,
              RawMetricType = RawMetricType.UserDownloadMetric,
              RawMetricData = (RawMetricData) new UserDownloadRawMetric()
              {
                Vsid = str,
                Count = downloadCount
              }
            };
            tracer.TraceInfo(string.Format("Adding {0} download(s) by {1} to package '{2}' version '{3}' in protocol '{4}' in feed {5}", (object) downloadCount, (object) str, (object) packageMetricsData.NormalizedPackageName, (object) packageMetricsData.NormalizedPackageVersion, (object) packageMetricsData.Protocol, (object) packageMetricsData.FeedId));
            this.UpdateCurrentDownloadStats(packageMetricsData);
          }
          else
            tracer.TraceInfo("Package metrics collection is disabled.");
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
        }
      }
      return Task.CompletedTask;
    }

    public virtual IEnumerable<PackageMetricsData> Write(IVssRequestContext requestContext) => throw new NotImplementedException();

    private void UpdateCurrentDownloadStats(PackageMetricsData packageMetricsData)
    {
      if (this._currentDownloadStats.AddOrUpdate(packageMetricsData.GetKey(), packageMetricsData, (Func<string, PackageMetricsData, PackageMetricsData>) ((packageMetricsKey, oldMetricsData) =>
      {
        if (oldMetricsData.RawMetricType != RawMetricType.UserDownloadMetric)
          return oldMetricsData;
        UserDownloadRawMetric rawMetricData1 = oldMetricsData.RawMetricData as UserDownloadRawMetric;
        UserDownloadRawMetric rawMetricData2 = packageMetricsData.RawMetricData as UserDownloadRawMetric;
        return new PackageMetricsData()
        {
          Protocol = packageMetricsData.Protocol,
          FeedId = packageMetricsData.FeedId,
          ProjectId = packageMetricsData.ProjectId,
          NormalizedPackageName = packageMetricsData.NormalizedPackageName,
          NormalizedPackageVersion = packageMetricsData.NormalizedPackageVersion,
          RawMetricType = RawMetricType.UserDownloadMetric,
          RawMetricData = (RawMetricData) new UserDownloadRawMetric()
          {
            Vsid = rawMetricData1.Vsid,
            Count = (rawMetricData1.Count + rawMetricData2.Count)
          }
        };
      })) != packageMetricsData)
        return;
      Interlocked.Increment(ref this._packageCount);
    }

    private void FlushStats(IVssRequestContext requestContext)
    {
      bool flag = false;
      try
      {
        flag = requestContext.LockManager.TryGetLock(this.QueueFlushDownloadStatsLockName, 0);
        if (!flag || this.flushTaskQueued)
          return;
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdatePackageMetricsInFeed), (object) 0, 0));
        this.flushTaskQueued = true;
      }
      catch (RequestCanceledException ex)
      {
      }
      finally
      {
        if (flag)
          requestContext.LockManager.ReleaseLock(this.QueueFlushDownloadStatsLockName);
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetricsService.TraceData, 5726710, nameof (ServiceStart)))
      {
        try
        {
          this.LoadPackageMetricsSettings(requestContext);
          requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), true, in PackageMetricsService.packageMetricsSettingsRegistryQuery);
          this.QueueFlushDownloadStatsLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "QueueFlushDownloadStatsLock"));
          requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdatePackageMetricsInFeed), (object) null, this._packageMetricsSettings.FlushMetricsTaskScheduleInterval));
        }
        catch (Exception ex) when (tracer.TraceExceptionFilter(ex))
        {
        }
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetricsService.TraceData, 5726720, nameof (ServiceEnd)))
      {
        try
        {
          int count = this._currentDownloadStats.Count;
          if (count > 0)
            tracer.TraceAlways(string.Format("Dropping package metrics because of host shutdown. Drop count: {0}", (object) count));
          requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(requestContext, new TeamFoundationTaskCallback(this.UpdatePackageMetricsInFeed));
          requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
        }
        catch (Exception ex) when (tracer.TraceExceptionFilter(ex))
        {
        }
      }
    }

    private void LoadPackageMetricsSettings(IVssRequestContext requestContext) => this._packageMetricsSettings = new PackageMetricsSettings(requestContext);

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadPackageMetricsSettings(requestContext);
    }

    protected internal void UpdatePackageMetricsInFeed(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      requestContext.RunSynchronously((Func<Task>) (() => this.UpdatePackageMetricsInFeedAsync(requestContext)));
    }

    private async Task UpdatePackageMetricsInFeedAsync(IVssRequestContext requestContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, PackageMetricsService.TraceData, 5726730, nameof (UpdatePackageMetricsInFeedAsync)))
      {
        ICollection<string> keys = this._currentDownloadStats.Keys;
        if (keys.Count > 0)
        {
          Dictionary<string, PackageMetricsData> dictionary = new Dictionary<string, PackageMetricsData>(keys.Count);
          foreach (string key in (IEnumerable<string>) keys)
          {
            PackageMetricsData packageMetricsData;
            if (this._currentDownloadStats.TryRemove(key, out packageMetricsData))
            {
              dictionary[key] = packageMetricsData;
              Interlocked.Decrement(ref this._packageCount);
            }
          }
          using (requestContext.Lock(this.QueueFlushDownloadStatsLockName))
            this.flushTaskQueued = false;
          if (dictionary != null)
          {
            if (dictionary.Count > 0)
            {
              if (this._packageMetricsSettings.ShouldWriteToFeed(requestContext))
              {
                tracer.TraceInfo(string.Format("Publishing package metrics to feed. Metrics Count:{0}", (object) dictionary.Count), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
                Task flushTask = dictionary.Values.InvokeInSplitBatchesAsync<PackageMetricsData>((Func<IEnumerable<PackageMetricsData>, Task>) (currentBatch => requestContext.Fork((Func<IVssRequestContext, Task>) (forkedRequestContext => this.PublishOneMetricsBatch(forkedRequestContext, currentBatch)), nameof (UpdatePackageMetricsInFeedAsync))), this._packageMetricsSettings.WriteToFeedBatchSize, this._packageMetricsSettings.MaxConcurrentBatchWrites);
                Task cancelListeningTask = Task.Delay(PackageMetricsConstants.DefaultFlushMetricsTaskTimeout, requestContext.CancellationToken);
                if (await Task.WhenAny(cancelListeningTask, flushTask) == cancelListeningTask)
                  tracer.TraceError("Failed while writing package metrics to feed. The publish metrics task timed out.");
                cancelListeningTask.ContinueWith<AggregateException>((Func<Task, AggregateException>) (t => t.Exception), TaskContinuationOptions.NotOnRanToCompletion);
                flushTask.ContinueWith<AggregateException>((Func<Task, AggregateException>) (t => t.Exception), TaskContinuationOptions.NotOnRanToCompletion);
                if (flushTask.IsFaulted)
                  tracer.TraceError("Failed while writing package metrics to feed. Exception: " + StackTraceCompressor.CompressStackTrace(flushTask.Exception.ToString()));
                tracer.TraceInfo("Package metrics published to feed.");
                flushTask = (Task) null;
                cancelListeningTask = (Task) null;
              }
            }
          }
        }
        else
          tracer.TraceInfo("No stats to flush");
      }
    }

    private async Task PublishOneMetricsBatch(
      IVssRequestContext forkedRequestContext,
      IEnumerable<PackageMetricsData> metricsToPublish)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer forkTracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(forkedRequestContext, PackageMetricsService.TraceData, 5726720, nameof (PublishOneMetricsBatch)))
      {
        try
        {
          forkedRequestContext.TraceDataConditionally(5726720, TraceLevel.Info, PackageMetricsService.TraceData.Area, PackageMetricsService.TraceData.Layer, "Publishing a batch of metrics", (Func<object>) (() =>
          {
            List<PackageMetricsData> list = metricsToPublish.ToList<PackageMetricsData>();
            metricsToPublish = (IEnumerable<PackageMetricsData>) list;
            return (object) new
            {
              count = list.Count,
              packageNames = list.Select<PackageMetricsData, string>((Func<PackageMetricsData, string>) (x => x.NormalizedPackageName)).Distinct<string>(),
              vsids = list.Select<PackageMetricsData, string>((Func<PackageMetricsData, string>) (x => !(x.RawMetricData is UserDownloadRawMetric rawMetricData2) ? (string) null : rawMetricData2.Vsid)).Distinct<string>(),
              feeds = list.Select<PackageMetricsData, Guid>((Func<PackageMetricsData, Guid>) (x => x.FeedId)).Distinct<Guid>()
            };
          }), nameof (PublishOneMetricsBatch));
          await forkedRequestContext.GetService<IFeedMetricsInternalClientService>().PublishMetricsAsync(forkedRequestContext, metricsToPublish);
        }
        catch (Exception ex)
        {
          forkTracer.TraceException(ex);
          throw;
        }
      }
    }
  }
}
