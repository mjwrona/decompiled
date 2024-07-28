// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsJobCommand
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Commerce;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public abstract class AnalyticsJobCommand : ITeamFoundationJobExtension
  {
    private int _batchSize;
    private TimeSpan _maxJobYieldDelay;
    private CancellationTokenSource _jobCancellationTokenSource;
    private JobRetryConfiguration _defaultRetryConfiguration;
    private const string BatchSizePathTemplate = "/Service/Analytics/Jobs/{0}/BatchSize";
    private const string YieldAfterSecondsPathTemplate = "/Service/Analytics/Jobs/{0}/YieldAfterSeconds";
    private const string MaxJobYieldDelaySecondsPathTemplate = "/Service/Analytics/Jobs/{0}/MaxJobYieldDelaySeconds";
    private const bool Fallback = true;
    protected const string FeatureFlagPreview = "Analytics.Jobs.Preview";
    public const int DefaultMaxJobYieldDelaySeconds = 15;

    public virtual int DefaultYieldAfterSeconds { get; } = 900;

    public virtual int DefaultBatchSize { get; } = 200;

    public virtual int GetStageVersion(IVssRequestContext requestContext, int analyticsStageVersion) => 0;

    public virtual int GetContentVersion(
      IVssRequestContext requestContext,
      int analyticsStageVersion)
    {
      return 0;
    }

    internal virtual AnalyticsJobScheduler JobScheduler { get; private set; }

    internal virtual AnalyticsJobEventHandler JobEventHandler { get; private set; }

    public virtual TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      IAnalyticsFeatureService service = requestContext.GetService<IAnalyticsFeatureService>();
      HostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      if (AnalyticsFeatureService.SubStatusPreventsStaging(hostProperties.SubStatus))
      {
        resultMessage = string.Format("Service Host {0} (Host Type {1}) has SubStatus of {2}.", (object) hostProperties.Name, (object) hostProperties.HostTypeValue, (object) hostProperties.SubStatus.ToString());
        return TeamFoundationJobExecutionResult.Blocked;
      }
      if (!service.IsAnalyticsEnabled(requestContext) && !service.IsAnalyticsEnabled(requestContext, bypassCache: true))
      {
        jobDefinition.Schedule.Clear();
        resultMessage = "Analytics is not enabled. Disabling.";
        return TeamFoundationJobExecutionResult.Disabled;
      }
      if (requestContext.IsInfrastructureHost())
      {
        jobDefinition.Schedule.Clear();
        resultMessage = "Infrastructure hosts are not supported. Disabling.";
        return TeamFoundationJobExecutionResult.Disabled;
      }
      StringBuilder stringBuilder = new StringBuilder();
      FeatureFlaggedSchedule featureFlaggedSchedule = (FeatureFlaggedSchedule) null;
      try
      {
        featureFlaggedSchedule = jobDefinition.Data != null ? TeamFoundationSerializationUtility.Deserialize<FeatureFlaggedSchedule>(jobDefinition.Data) : (FeatureFlaggedSchedule) null;
      }
      catch (Exception ex)
      {
        stringBuilder.AppendLine(string.Format("Could not deserialize job definition data {0} for {1}. Exception: {2}", (object) jobDefinition.Data, (object) jobDefinition.Name, (object) ex));
      }
      if (featureFlaggedSchedule != null)
      {
        string featureFlagName = featureFlaggedSchedule.FeatureFlagName;
        if (!string.IsNullOrEmpty(featureFlagName) && !requestContext.IsFeatureEnabled(featureFlagName))
        {
          stringBuilder.AppendLine("Feature flag " + featureFlagName + " is disabled");
          resultMessage = stringBuilder.ToString();
          return TeamFoundationJobExecutionResult.Disabled;
        }
        List<TeamFoundationJobSchedule> schedule = featureFlaggedSchedule.Schedule;
        // ISSUE: explicit non-virtual call
        if ((schedule != null ? (__nonvirtual (schedule.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          stringBuilder.AppendLine(string.Format("Overriding {0} job schedules with {1} feature-flag schedules", (object) jobDefinition.Schedule.Count, (object) featureFlaggedSchedule.Schedule.Count));
          jobDefinition.Schedule.Clear();
          Guid guid = requestContext.ServiceHost.InstanceId;
          int hashCode1 = guid.GetHashCode();
          guid = jobDefinition.JobId;
          int hashCode2 = guid.GetHashCode();
          int num = hashCode1 ^ hashCode2;
          foreach (TeamFoundationJobSchedule foundationJobSchedule in featureFlaggedSchedule.Schedule)
          {
            if (foundationJobSchedule.Interval != 0)
              foundationJobSchedule.ScheduledTime = foundationJobSchedule.ScheduledTime.AddSeconds((double) (num % foundationJobSchedule.Interval));
            jobDefinition.Schedule.Add(foundationJobSchedule);
          }
        }
      }
      this.ValidateJobsAttribute(requestContext, jobDefinition);
      this.JobEventHandler = new AnalyticsJobEventHandler(this.TableName);
      using (requestContext.TraceBlock(14000001, 14000002, "AnalyticsStaging", this.TraceLayer, nameof (Run)))
      {
        using (new TraceAnalyticsJobCommandListener(requestContext, this.JobEventHandler, this.TraceLayer))
        {
          using (new PerfCounterAnalyticsJobCommandListener(this.JobEventHandler, jobDefinition.Name))
          {
            using (ResultMessageAnalyticsJobCommandListener jobCommandListener = new ResultMessageAnalyticsJobCommandListener(this.JobEventHandler, this.ResultMessage))
            {
              try
              {
                int num = (int) this.Execute(requestContext, jobDefinition);
                this.JobScheduler.DeleteRetryRegistryRecord(requestContext);
                stringBuilder.AppendLine(jobCommandListener.ResultMessage.ToString());
                resultMessage = stringBuilder.ToString();
                if (num == 2)
                  AnalyticsJobCommand.LogJobFailure(requestContext, jobDefinition.Name);
                return (TeamFoundationJobExecutionResult) num;
              }
              catch (VssServiceResponseException ex) when (ex.Message.StartsWith("TF24668"))
              {
                resultMessage = string.Format("Error code: {0}. Message: {1}. Stack Trace: {2}", (object) ex.ErrorCode, (object) ex.Message, (object) ex.StackTrace);
                this.JobScheduler.QueueWithDelay(requestContext, this._defaultRetryConfiguration.MaximumRetryIntervalSeconds);
                return TeamFoundationJobExecutionResult.Blocked;
              }
              catch (ClientRequestThrottledException ex)
              {
                AnalyticsJobCommand.LogJobBlocked(requestContext, jobDefinition.Name, ex.Message);
                int val1 = (int) Math.Ceiling((ex.RetryAfterDateTime - DateTime.UtcNow).TotalSeconds);
                this.ScheduleRetry(requestContext, Math.Max(val1, 120), Math.Max(val1, 720), this._defaultRetryConfiguration.DeltaBaseSeconds);
                resultMessage = string.Format("received client throttled exception: \"{0}\", retrying after {1} past {2}", (object) ex.Message, (object) Math.Max(val1, 120), (object) DateTime.UtcNow.ToString());
                return TeamFoundationJobExecutionResult.Blocked;
              }
              catch (ServiceOwnerNotFoundException ex) when (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
              {
                resultMessage = "ServiceOwnerNotFoundException indicates the AX service is not available. The job can not proceed.";
                return TeamFoundationJobExecutionResult.Blocked;
              }
              catch (Exception ex)
              {
                AnalyticsJobCommand.LogJobFailure(requestContext, jobDefinition.Name);
                if (this.AlwaysRetryStageFailures || this.JobDefinition.JobId == this.RecoveryJobId)
                  this.JobScheduler.Retry(requestContext, this._defaultRetryConfiguration);
                throw;
              }
            }
          }
        }
      }
    }

    protected void AppendResultMessage(string message)
    {
      if (this.ResultMessage == null)
        return;
      this.ResultMessage.AppendLine(message);
    }

    private static bool LogJobFailure(IVssRequestContext requestContext, string jobName)
    {
      TeamFoundationEventLog.Default.Log(requestContext, "Analytics TFS Job '" + jobName + "' failed", TeamFoundationEventId.AnalyticsTFSJobFailed, EventLogEntryType.Error, (object) jobName);
      return false;
    }

    private static bool LogJobBlocked(
      IVssRequestContext requestContext,
      string jobName,
      string reason)
    {
      TeamFoundationEventLog.Default.Log(requestContext, "Analytics TFS Job '" + jobName + "' is blocked: " + reason, TeamFoundationEventId.AnalyticsTFSJobPartialSucces, EventLogEntryType.Warning, (object) jobName, (object) reason);
      return false;
    }

    private void ValidateJobsAttribute(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamFoundationJobDefinition>(jobDefinition, nameof (jobDefinition));
      ArgumentUtility.CheckStringForNullOrEmpty(this.TableName, "TableName");
    }

    public void Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      IVssRegistryService registryService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<IVssRegistryService>() : throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      TimeSpan delay = TimeSpan.FromSeconds((double) registryService.GetValue<int>(requestContext, (RegistryQuery) string.Format("/Service/Analytics/Jobs/{0}/YieldAfterSeconds", (object) this.TableName), true, this.DefaultYieldAfterSeconds));
      this._maxJobYieldDelay = TimeSpan.FromSeconds((double) registryService.GetValue<int>(requestContext, (RegistryQuery) string.Format("/Service/Analytics/Jobs/{0}/MaxJobYieldDelaySeconds", (object) this.TableName), true, 15));
      this._batchSize = registryService.GetValue<int>(requestContext, (RegistryQuery) string.Format("/Service/Analytics/Jobs/{0}/BatchSize", (object) this.TableName), true, this.DefaultBatchSize);
      ArgumentUtility.CheckForOutOfRange(this._batchSize, string.Format("/Service/Analytics/Jobs/{0}/BatchSize", (object) this.TableName), 1);
      ArgumentUtility.CheckForOutOfRange<double>(delay.TotalSeconds, string.Format("/Service/Analytics/Jobs/{0}/YieldAfterSeconds", (object) this.TableName), 0.0);
      ArgumentUtility.CheckForOutOfRange<double>(this._maxJobYieldDelay.TotalSeconds, string.Format("/Service/Analytics/Jobs/{0}/MaxJobYieldDelaySeconds", (object) this.TableName), 0.0);
      this._jobCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken);
      this._jobCancellationTokenSource.CancelAfter(delay);
      this.CancellationToken = this._jobCancellationTokenSource.Token;
      this.JobDefinition = jobDefinition;
      this._defaultRetryConfiguration = new JobRetryConfiguration();
      if (this.JobDefinition.JobId == this.RecoveryJobId)
        this._defaultRetryConfiguration.MaxRetry = int.MaxValue;
      this.JobScheduler = new AnalyticsJobScheduler(this.JobDefinition, this.TraceLayer);
    }

    public CancellationToken CancellationToken { get; private set; }

    public TeamFoundationJobDefinition JobDefinition { get; private set; }

    public StringBuilder ResultMessage { get; } = new StringBuilder();

    public abstract string TableName { get; }

    public virtual string TraceLayer => this.GetType().Name;

    public abstract Guid LiveJobId { get; }

    public abstract Guid RecoveryJobId { get; }

    public virtual bool AlwaysReplace => false;

    public virtual bool AlwaysRetryStageFailures => false;

    public void Cancel() => this._jobCancellationTokenSource.Cancel();

    public virtual IEnumerable<int> GetProviderShardIds(
      IVssRequestContext requestContext,
      StageTableInfo stageTable)
    {
      return (IEnumerable<int>) new int[1];
    }

    public virtual IEnumerable<StageProviderShardInfo> GetProviderShardIdsWithLocator(
      IVssRequestContext requestContext,
      StageTableInfo stageTable)
    {
      return this.GetProviderShardIds(requestContext, stageTable).Select<int, StageProviderShardInfo>((Func<int, StageProviderShardInfo>) (i => new StageProviderShardInfo()
      {
        ProviderShardId = i
      }));
    }

    public TeamFoundationJobExecutionResult Execute(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      this.Initialize(requestContext, jobDefinition);
      this.ProcessShards(requestContext);
      if (requestContext.IsCanceled)
        return TeamFoundationJobExecutionResult.Stopped;
      if (!this.CancellationToken.IsCancellationRequested)
        return TeamFoundationJobExecutionResult.Succeeded;
      this.JobScheduler.QueueWithDelay(requestContext, (int) this._maxJobYieldDelay.TotalSeconds);
      return TeamFoundationJobExecutionResult.PartiallySucceeded;
    }

    public void ProcessShards(IVssRequestContext requestContext)
    {
      int analyticsStageVersion;
      foreach (StageProviderShardInfo allShard in this.GetAllShards(requestContext, out analyticsStageVersion))
      {
        this.JobEventHandler.RaiseProcessingShard((object) this, allShard.ProviderShardId);
        this.ProcessStreams(requestContext, allShard, analyticsStageVersion);
      }
    }

    private IEnumerable<StageProviderShardInfo> GetAllShards(
      IVssRequestContext requestContext,
      out int analyticsStageVersion)
    {
      AnalyticsHttpClient client = requestContext.GetClient<AnalyticsHttpClient>();
      StageTableInfo stageTable = client.GetTableAsync(this.TableName, (object) this.CancellationToken).SyncResult<StageTableInfo>();
      IEnumerable<StageProviderShardInfo> shardIdsWithLocator = this.GetProviderShardIdsWithLocator(requestContext, stageTable);
      IList<StageProviderShardInfo> shards = stageTable.Shards;
      Dictionary<int, StageProviderShardInfo> dictionary = shards != null ? shards.ToDictionary<StageProviderShardInfo, int>((Func<StageProviderShardInfo, int>) (x => x.ProviderShardId)) : (Dictionary<int, StageProviderShardInfo>) null;
      analyticsStageVersion = stageTable.StageVersion;
      HashSet<int> providerShardIdsHashSet = new HashSet<int>(shardIdsWithLocator.Select<StageProviderShardInfo, int>((Func<StageProviderShardInfo, int>) (i => i.ProviderShardId)));
      List<StageProviderShardInfo> allShards = new List<StageProviderShardInfo>();
      foreach (StageProviderShardInfo providerShardInfo1 in shardIdsWithLocator)
      {
        StageProviderShardInfo providerShardInfo2 = (StageProviderShardInfo) null;
        if (dictionary == null || !dictionary.TryGetValue(providerShardInfo1.ProviderShardId, out providerShardInfo2))
        {
          this.JobEventHandler.RaiseCreateShard((object) this, providerShardInfo1.ProviderShardId);
          providerShardInfo2 = client.CreateShardAsync(this.TableName, providerShardInfo1.ProviderShardId, (object) this.CancellationToken).SyncResult<StageProviderShardInfo>();
        }
        providerShardInfo2.ProviderShardLocator = providerShardInfo1.ProviderShardLocator;
        allShards.Add(providerShardInfo2);
      }
      if (stageTable.Shards != null)
      {
        foreach (int num in stageTable.Shards.Select<StageProviderShardInfo, int>((Func<StageProviderShardInfo, int>) (x => x.ProviderShardId)).Where<int>((Func<int, bool>) (x => providerShardIdsHashSet == null || !providerShardIdsHashSet.Contains(x))))
        {
          this.JobEventHandler.RaiseDeleteShard((object) this, num);
          client.DeleteShardAsync(this.TableName, num, (object) this.CancellationToken).SyncResult();
        }
      }
      return (IEnumerable<StageProviderShardInfo>) allShards;
    }

    public abstract IEnumerable<StagePostEnvelope> GetRecords(
      IVssRequestContext requestContext,
      GetRecordsInfo info,
      CancellationToken cancellationToken);

    private void ProcessStreams(
      IVssRequestContext requestContext,
      StageProviderShardInfo shard,
      int analyticsStageVersion)
    {
      AnalyticsHttpClient client = requestContext.GetClient<AnalyticsHttpClient>();
      AnalyticsShardInvalidator shardInvalidator = new AnalyticsShardInvalidator(this.TableName, shard.ProviderShardId);
      bool flag1 = false;
      IList<StageStreamInfo> streams = shard.Streams;
      if (this.JobDefinition.JobId == this.LiveJobId)
      {
        if (shardInvalidator.InvalidateShardIfRegistrySet(requestContext))
          flag1 = true;
        else if (this.AlwaysReplace)
        {
          shardInvalidator.InvalidateShard(requestContext, true);
          flag1 = true;
        }
        else
        {
          IEnumerable<InvalidationOption> source = streams.Select<StageStreamInfo, InvalidationOption>((Func<StageStreamInfo, InvalidationOption>) (s => this.ShouldInvalidate(requestContext, s)));
          bool flag2 = source.All<InvalidationOption>((Func<InvalidationOption, bool>) (r => r == InvalidationOption.Invalidate));
          bool flag3 = source.All<InvalidationOption>((Func<InvalidationOption, bool>) (r => r == InvalidationOption.KeysOnlyInvalidate));
          if (flag2)
          {
            shardInvalidator.InvalidateShard(requestContext);
            flag1 = true;
          }
          else if (flag3)
          {
            shardInvalidator.InvalidateShard(requestContext, keysOnly: true);
            flag1 = true;
          }
        }
      }
      if (flag1)
        streams = client.GetShardAsync(this.TableName, shard.ProviderShardId, (object) this.CancellationToken).SyncResult<StageProviderShardInfo>().Streams;
      List<StageStreamInfo> stageStreamInfoList1 = new List<StageStreamInfo>();
      List<StageStreamInfo> stageStreamInfoList2 = new List<StageStreamInfo>();
      foreach (StageStreamInfo stageStreamInfo in (IEnumerable<StageStreamInfo>) streams.OrderByDescending<StageStreamInfo, int>((Func<StageStreamInfo, int>) (s => s.Priority)) ?? Enumerable.Empty<StageStreamInfo>())
        (stageStreamInfo.Current || this.AlwaysReplace ? stageStreamInfoList1 : stageStreamInfoList2).Add(stageStreamInfo);
      this.JobEventHandler.RaiseFoundStreams((object) this, stageStreamInfoList1.Count, stageStreamInfoList2.Count, shard.ProviderShardId);
      if (this.JobDefinition.JobId == this.LiveJobId)
      {
        if (stageStreamInfoList2.Count > 0)
          requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            this.RecoveryJobId
          }, 5);
        foreach (StageStreamInfo stream in stageStreamInfoList1)
          this.ProcessStream(requestContext, shard.ProviderShardId, shard.ProviderShardLocator, stream, analyticsStageVersion);
      }
      else
      {
        foreach (StageStreamInfo stream in stageStreamInfoList2)
          this.ProcessStream(requestContext, shard.ProviderShardId, shard.ProviderShardLocator, stream, analyticsStageVersion);
      }
    }

    protected virtual InvalidationOption ShouldInvalidate(
      IVssRequestContext requestContext,
      StageStreamInfo stream)
    {
      return InvalidationOption.NoInvalidate;
    }

    private void ProcessStream(
      IVssRequestContext requestContext,
      int shardId,
      string shardLocator,
      StageStreamInfo stream,
      int analyticsStageVersion)
    {
      this.JobEventHandler.RaiseProcessingStream((object) this, shardId, stream);
      int stageVersion = this.GetStageVersion(requestContext, analyticsStageVersion);
      int contentVersion = this.GetContentVersion(requestContext, analyticsStageVersion);
      try
      {
        GetRecordsInfo info = new GetRecordsInfo(shardId, shardLocator, stream.Watermark, this._batchSize, stream.KeysOnly, stageVersion);
        using (IEnumerator<StagePostEnvelope> enumerator = this.GetRecords(requestContext, info, this.CancellationToken).GetEnumerator())
        {
          string str = stream.Watermark;
          bool flag1 = false;
          bool flag2 = true;
          while (flag2 && !this.CancellationToken.IsCancellationRequested)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            flag2 = enumerator.MoveNext();
            stopwatch.Stop();
            if (flag2 && !this.CancellationToken.IsCancellationRequested)
            {
              StagePostEnvelope current = enumerator.Current;
              if (current != null)
              {
                if (current != null)
                {
                  int? count = current.Records?.Count;
                  int num = 0;
                  if (count.GetValueOrDefault() > num & count.HasValue)
                    goto label_8;
                }
                if (current == null || current.ToWatermark == null || !(str != current?.ToWatermark))
                  continue;
label_8:
                if (current.Records == null)
                  current.Records = (IList) new List<object>();
                current.KeysOnly = stream.KeysOnly;
                current.FromWatermark = str;
                current.StageVersion = stageVersion;
                current.ContentVersion = contentVersion;
                this.JobEventHandler.RaiseRecordsRetrieved((object) this, shardId, stream, current, stopwatch.Elapsed);
                IngestResult ingestResult = this.StageRecordsWithPartitioning(requestContext, shardId, stream, current);
                flag1 = true;
                str = current.ToWatermark;
                if (current.IsCurrent || ingestResult.Terminate)
                  break;
              }
            }
          }
          if (!this.CancellationToken.IsCancellationRequested && !stream.Current && !flag1)
          {
            StagePostEnvelope envelope = new StagePostEnvelope()
            {
              FromWatermark = str,
              ToWatermark = str,
              Records = (IList) Array.Empty<object>(),
              StageVersion = stageVersion,
              ContentVersion = contentVersion,
              IsCurrent = true
            };
            this.StageRecords(requestContext, shardId, stream, envelope);
          }
          else if (!flag1)
          {
            if (this.ShouldSendInvalidateStreamEnvelope(stageVersion, contentVersion, stream, new int?(analyticsStageVersion)))
            {
              StagePostEnvelope envelope = new StagePostEnvelope()
              {
                FromWatermark = str,
                ToWatermark = str,
                KeysOnly = stream.KeysOnly,
                Records = (IList) Array.Empty<object>(),
                StageVersion = stageVersion,
                ContentVersion = contentVersion,
                IsCurrent = stream.Current
              };
              this.StageRecords(requestContext, shardId, stream, envelope);
            }
          }
        }
        this.JobEventHandler.RaiseStreamProcessed((object) this, shardId, stream);
      }
      catch (StageStreamDisabledException ex)
      {
        this.JobEventHandler.RaiseStreamDisabled((object) this, shardId, stream);
        if (!(this.JobDefinition.JobId == this.LiveJobId))
          return;
        throw;
      }
      catch (StageStreamThrottledException ex)
      {
        this.JobEventHandler.RaiseStreamThrottled((object) this, shardId, stream);
        if (this.JobDefinition.JobId == this.RecoveryJobId)
          this.ScheduleRetry(requestContext, 120, 1800, 60);
        else
          this.ScheduleRetry(requestContext, 120, 720, this._defaultRetryConfiguration.DeltaBaseSeconds);
      }
      catch (StageKeysOnlyNotSupportedException ex)
      {
        this.JobEventHandler.RaiseKeysOnlyNotSupported((object) this, shardId, stream);
        if (stream.KeysOnly)
          this.ScheduleRetry(requestContext, 1, 60, this._defaultRetryConfiguration.DeltaBaseSeconds);
        else
          throw;
      }
      catch (StageTableInMaintenanceException ex)
      {
        this.JobEventHandler.RaiseTableInMaintenance((object) this, shardId, stream);
        this.ScheduleRetry(requestContext, 30, 60, this._defaultRetryConfiguration.DeltaBaseSeconds);
      }
      catch (StageStreamNotFoundException ex)
      {
        this.JobEventHandler.RaiseStageStreamUnknown((object) this, shardId, stream);
        throw;
      }
    }

    private bool ShouldSendInvalidateStreamEnvelope(
      int stageVersion,
      int contentVersion,
      StageStreamInfo streamInfo,
      int? analyticsStageVersion)
    {
      return analyticsStageVersion.HasValue && streamInfo.LatestContentVersion.HasValue && this.JobDefinition.JobId == this.LiveJobId && stageVersion == analyticsStageVersion.Value && contentVersion != streamInfo.LatestContentVersion.Value;
    }

    private IngestResult StageRecordsWithPartitioning(
      IVssRequestContext requestContext,
      int shardId,
      StageStreamInfo stream,
      StagePostEnvelope envelope)
    {
      IngestResult ingestResult1 = new IngestResult();
      if ((double) envelope.Records.Count > (double) this._batchSize * 1.5)
      {
        foreach (EnumerableExtensions.RecordsBatch batchRecord in envelope.Records.Cast<object>().BatchRecords<object>(this._batchSize, this.CancellationToken))
        {
          StagePostEnvelope envelope1 = new StagePostEnvelope()
          {
            Records = batchRecord.Records,
            Replace = envelope.Replace,
            ContentVersion = envelope.ContentVersion,
            StageVersion = envelope.StageVersion,
            FromWatermark = envelope.FromWatermark,
            ToWatermark = batchRecord.IsLast ? envelope.ToWatermark : envelope.FromWatermark,
            IsCurrent = batchRecord.IsLast && envelope.IsCurrent
          };
          IngestResult ingestResult2 = this.StageRecords(requestContext, shardId, stream, envelope1);
          ingestResult1.Count += ingestResult2.Count;
        }
      }
      else
        ingestResult1 = this.StageRecords(requestContext, shardId, stream, envelope);
      return ingestResult1;
    }

    private IngestResult StageRecords(
      IVssRequestContext requestContext,
      int shardId,
      StageStreamInfo stream,
      StagePostEnvelope envelope)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      AnalyticsHttpClient client = requestContext.GetClient<AnalyticsHttpClient>();
      stopwatch1.Stop();
      this.JobEventHandler.RaiseClientResolved((object) this, stopwatch1.Elapsed);
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      string tableName = this.TableName;
      int providerShard = shardId;
      int streamId = stream.StreamId;
      StagePostEnvelope envelope1 = envelope;
      // ISSUE: variable of a boxed type
      __Boxed<CancellationToken> cancellationToken1 = (ValueType) requestContext.CancellationToken;
      CancellationToken cancellationToken2 = new CancellationToken();
      IngestResult ingestResult = client.StageRecordsAsync(tableName, providerShard, streamId, envelope1, (object) cancellationToken1, cancellationToken2).SyncResult<IngestResult>();
      stopwatch2.Stop();
      this.JobEventHandler.RaiseRecordsStaged((object) this, shardId, stream, envelope, stopwatch2.Elapsed);
      return ingestResult;
    }

    private void ScheduleRetry(
      IVssRequestContext requestContext,
      int MinInterval,
      int MaxInterval,
      int Delta)
    {
      JobRetryConfiguration config = new JobRetryConfiguration(this._defaultRetryConfiguration)
      {
        MinimumRetryIntervalSeconds = MinInterval,
        MaximumRetryIntervalSeconds = MaxInterval,
        DeltaBaseSeconds = Delta
      };
      this.JobScheduler.Retry(requestContext, config);
    }
  }
}
