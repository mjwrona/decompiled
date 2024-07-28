// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.AggregatedArtifactsUpdateJob
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
  public abstract class AggregatedArtifactsUpdateJob : ITeamFoundationJobExtension
  {
    private IArtifactsAggregator m_artifactsAggregator;

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      try
      {
        SocialActivityAggregationService service = requestContext.GetService<SocialActivityAggregationService>();
        int take = 20000;
        int skip = 0;
        DateTime utcNow = DateTime.UtcNow;
        DateTime fromDate = DateTime.UtcNow.AddSeconds((double) -this.FetchAggregatedMetricsForTimeInSec);
        IList<SocialActivityAggregatedMetric> aggregatedMetricList = (IList<SocialActivityAggregatedMetric>) new List<SocialActivityAggregatedMetric>();
        while (true)
        {
          IList<SocialActivityAggregatedMetric> aggregatedMetrics = service.GetActivityAggregatedMetrics(requestContext, Guid.Empty, this.AggregatorProviderId, fromDate, utcNow, skip, take);
          aggregatedMetricList.AddRange<SocialActivityAggregatedMetric, IList<SocialActivityAggregatedMetric>>((IEnumerable<SocialActivityAggregatedMetric>) aggregatedMetrics);
          if (aggregatedMetrics.Count<SocialActivityAggregatedMetric>() == 20000 && aggregatedMetricList.Count<SocialActivityAggregatedMetric>() < this.MaxAggregatedMetricsRecordsToFetch)
            skip += take;
          else
            break;
        }
        this.GetArtifactsAggregator().Aggregate(requestContext, aggregatedMetricList, out resultMessage);
      }
      catch (Exception ex)
      {
        resultMessage = string.Format("Job failed with Exception [{0}].", (object) ex);
        requestContext.TraceException(11000003, this.Area, this.Layer, ex);
        return TeamFoundationJobExecutionResult.Failed;
      }
      return TeamFoundationJobExecutionResult.Succeeded;
    }

    private IArtifactsAggregator GetArtifactsAggregator()
    {
      if (this.m_artifactsAggregator == null)
        this.m_artifactsAggregator = this.InitializeArtifactsAggregator();
      return this.m_artifactsAggregator;
    }

    protected abstract Guid AggregatorProviderId { get; }

    protected abstract int FetchAggregatedMetricsForTimeInSec { get; }

    protected abstract int MaxAggregatedMetricsRecordsToFetch { get; }

    protected abstract string Area { get; }

    protected abstract string Layer { get; }

    protected abstract IArtifactsAggregator InitializeArtifactsAggregator();
  }
}
