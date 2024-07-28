// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityAggregationJob
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public abstract class SocialActivityAggregationJob : ITeamFoundationJobExtension
  {
    private const int c_maxTimeSpanToAggregateInHrs = 40;
    private const int c_AggregationBatchSizeInHrs = 4;

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      TeamFoundationJobExecutionResult jobExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregationTillTimeCushionInSec", 1800);
      DateTime aggregateTill = DateTime.UtcNow;
      if (!this.AggregateRecordsTillCurrentTime())
        aggregateTill = aggregateTill.AddSeconds((double) -registryValue);
      resultMessage = string.Format("Aggregating social activity records for valid time buckets till: {0}.", (object) aggregateTill);
      bool flag = false;
      try
      {
        SocialActivityAggregationService service = requestContext.GetService<SocialActivityAggregationService>();
        DateTime processedTillTime = service.GetSocialPluginProcessedTillTime(requestContext, this.JobId, this.JobId);
        DateTime dateTime;
        for (DateTime startTime = this.GetStartTime(aggregateTill, processedTillTime); aggregateTill - startTime >= TimeSpan.FromMilliseconds((double) this.AggregationBatchSizeInMilliSeconds(requestContext)); startTime = dateTime)
        {
          dateTime = startTime.AddMilliseconds((double) this.AggregationBatchSizeInMilliSeconds(requestContext));
          IList<SocialActivityAggregatedMetric> aggregationsForTimeRange = this.GetAggregationsForTimeRange(requestContext, startTime, dateTime, out string _);
          if (aggregationsForTimeRange.Count > 0)
          {
            service.AddOrUpdateActivityAggregatedMetricRecords(requestContext, aggregationsForTimeRange);
            flag = true;
          }
          service.AddOrUpdateSocialPluginWatermark(requestContext, this.JobId, this.JobId, dateTime);
          resultMessage += string.Format("Added {0} aggregation records for time range {1} - {2}.", (object) aggregationsForTimeRange.Count<SocialActivityAggregatedMetric>(), (object) startTime, (object) dateTime);
        }
        if (flag)
          this.QueueAggregatedArtifactsUpdateJob(requestContext);
      }
      catch (Exception ex)
      {
        resultMessage += string.Format("Job failed with Exception [{0}].", (object) ex);
        requestContext.TraceException(11000003, this.Area, this.Layer, ex);
        jobExecutionResult = TeamFoundationJobExecutionResult.Failed;
      }
      this.PostRun(requestContext);
      return jobExecutionResult;
    }

    public abstract IList<SocialActivityAggregatedMetric> Aggregate(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      IDictionary<string, IList<SocialActivityRecord>> activityRecords,
      out string resultMessage);

    protected virtual void PostRun(IVssRequestContext requestContext)
    {
    }

    protected virtual int AggregationBatchSizeInMilliSeconds(IVssRequestContext requestContext) => 14400000;

    protected virtual bool FetchExtendedData() => false;

    protected virtual bool AggregateRecordsTillCurrentTime() => false;

    protected abstract Guid JobId { get; }

    protected abstract IList<string> SocialActivityTypes { get; }

    protected abstract string Area { get; }

    protected abstract string Layer { get; }

    protected virtual void QueueAggregatedArtifactsUpdateJob(IVssRequestContext requestContext)
    {
    }

    private DateTime GetStartTime(DateTime aggregateTill, DateTime processedTillTime) => !(aggregateTill - processedTillTime > TimeSpan.FromHours(40.0)) ? processedTillTime : aggregateTill.AddSeconds(-144000.0);

    private IList<SocialActivityAggregatedMetric> GetAggregationsForTimeRange(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      out string resultMessage)
    {
      IDictionary<string, IList<SocialActivityRecord>> activityTypeActivityRecordsMap = (IDictionary<string, IList<SocialActivityRecord>>) new Dictionary<string, IList<SocialActivityRecord>>();
      this.SocialActivityTypes.ForEach<string>((Action<string>) (activityType =>
      {
        int take = 20000;
        int skip = 0;
        SocialActivityAggregationService service = requestContext.GetService<SocialActivityAggregationService>();
        activityTypeActivityRecordsMap.Add(activityType, (IList<SocialActivityRecord>) new List<SocialActivityRecord>());
        while (true)
        {
          IList<SocialActivityRecord> activityRecords = service.GetActivityRecords(requestContext, activityType, startTime, endTime, this.FetchExtendedData(), skip, take);
          activityTypeActivityRecordsMap[activityType].AddRange<SocialActivityRecord, IList<SocialActivityRecord>>((IEnumerable<SocialActivityRecord>) activityRecords);
          if (activityRecords.Count<SocialActivityRecord>() == 20000)
            skip += 20000;
          else
            break;
        }
        if (activityTypeActivityRecordsMap[activityType].Count<SocialActivityRecord>() <= 20000)
          return;
        requestContext.TraceException(11000003, "SocialActivityAggregation", nameof (SocialActivityAggregationJob), (Exception) new SocialActivityAggregationGetActivityException(string.Format("For activityType: {0}, JobId: {1} and fromTime: {2}", (object) activityType, (object) this.JobId, (object) startTime) + string.Format("{0} social activity records were loaded in memory.", (object) activityTypeActivityRecordsMap[activityType].Count<SocialActivityRecord>())));
      }));
      return this.Aggregate(requestContext, startTime, endTime, activityTypeActivityRecordsMap, out resultMessage);
    }
  }
}
