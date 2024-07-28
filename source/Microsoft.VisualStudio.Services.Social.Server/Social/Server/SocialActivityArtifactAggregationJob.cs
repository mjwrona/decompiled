// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.Server.SocialActivityArtifactAggregationJob
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SocialServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Social.Server
{
  public abstract class SocialActivityArtifactAggregationJob : ITeamFoundationJobExtension
  {
    private const int m_AggregationBatchSizeInHrs = 24;
    private const int m_maxTimeSpanToAggregateInDays = 30;

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      DateTime utcNow = DateTime.UtcNow;
      SocialActivityAggregationService service = requestContext.GetService<SocialActivityAggregationService>();
      DateTime processedTillTime = service.GetSocialPluginProcessedTillTime(requestContext, this.JobId, this.JobId);
      DateTime dateTime1 = this.GetStartTime(utcNow, processedTillTime);
      resultMessage = string.Format("Aggregating social activity metrics for valid time buckets till: {0}.", (object) utcNow);
      try
      {
        int take = 20000;
        DateTime dateTime2;
        for (; dateTime1 < utcNow; dateTime1 = dateTime2)
        {
          int skip = 0;
          dateTime2 = dateTime1.AddMilliseconds((double) this.AggregationBatchSizeInMilliSeconds(requestContext));
          if (dateTime2 > utcNow)
            dateTime2 = utcNow;
          IList<SocialActivityAggregatedMetric> aggregatedMetricList = (IList<SocialActivityAggregatedMetric>) new List<SocialActivityAggregatedMetric>();
          while (true)
          {
            IList<SocialActivityAggregatedMetric> aggregatedMetrics = service.GetActivityAggregatedMetrics(requestContext, Guid.Empty, this.AggregatorProviderId, dateTime1, dateTime2, skip, take);
            aggregatedMetricList.AddRange<SocialActivityAggregatedMetric, IList<SocialActivityAggregatedMetric>>((IEnumerable<SocialActivityAggregatedMetric>) aggregatedMetrics);
            if (aggregatedMetrics.Count<SocialActivityAggregatedMetric>() == 20000 && aggregatedMetricList.Count<SocialActivityAggregatedMetric>() < this.MaxAggregatedMetricsRecordsToFetch)
              skip += take;
            else
              break;
          }
          string resultMessage1;
          IList<SocialActivityAggregatedArtifact> aggregatedArtifactList = this.AggregateArtifacts(requestContext, aggregatedMetricList, dateTime1, dateTime2, out resultMessage1);
          resultMessage += resultMessage1;
          if (aggregatedArtifactList.Count<SocialActivityAggregatedArtifact>() > 0)
            service.AddOrUpdateSocialActivityAggregatedArtifactRecords(requestContext, Guid.Empty, aggregatedArtifactList);
          resultMessage += string.Format(" {0} socialActivityAggregatedArtifact were added/updated.", (object) aggregatedArtifactList.Count<SocialActivityAggregatedArtifact>());
          service.AddOrUpdateSocialPluginWatermark(requestContext, this.JobId, this.JobId, dateTime2);
        }
        DateTime till = DateTime.UtcNow.AddDays((double) (-1 * this.DaysOldMetricsToDelete(requestContext)));
        int num = service.DeleteActivityAggregatedMetricRecords(requestContext, this.AggregatorProviderId, till);
        resultMessage += string.Format(" {0} metrics deleted older than {1}", (object) num, (object) till);
      }
      catch (Exception ex)
      {
        resultMessage += string.Format(" Job failed with Exception [{0}].", (object) ex);
        requestContext.TraceException(11000008, this.Area, this.Layer, ex);
        return TeamFoundationJobExecutionResult.Failed;
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    public abstract IList<SocialActivityAggregatedArtifact> AggregateArtifacts(
      IVssRequestContext requestContext,
      IList<SocialActivityAggregatedMetric> aggregatedMetrics,
      DateTime startTime,
      DateTime endTime,
      out string resultMessage);

    protected abstract int DaysOldMetricsToDelete(IVssRequestContext requestContext);

    protected abstract Guid JobId { get; }

    protected abstract string Area { get; }

    protected abstract string Layer { get; }

    protected abstract Guid AggregatorProviderId { get; }

    protected abstract int MaxAggregatedMetricsRecordsToFetch { get; }

    protected virtual DateTime GetStartTimeForAggregation(DateTime startTime) => startTime;

    protected virtual int AggregationBatchSizeInMilliSeconds(IVssRequestContext requestContext) => 86400000;

    private DateTime GetStartTime(DateTime aggregateTill, DateTime processedTillTime) => this.GetStartTimeForAggregation(!(aggregateTill - processedTillTime > TimeSpan.FromDays(30.0)) ? processedTillTime : aggregateTill.AddSeconds(-2592000.0));
  }
}
