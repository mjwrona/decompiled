// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.SocialEngagementActivities
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class SocialEngagementActivities
  {
    private const int MinimumLikesCountForTrending = 10;

    public int TrendFactorCount1Day { get; private set; }

    public int TrendFactorCount7Days { get; private set; }

    public int TrendFactorCount30Days { get; private set; }

    public int ProjectLikesCount { get; private set; }

    public SocialEngagementActivities()
    {
    }

    public SocialEngagementActivities(SocialEngagementAggregateMetric metrics)
    {
      if (metrics == null)
        throw new ArgumentNullException(nameof (metrics));
      if (metrics.SocialEngagementStatistics != null)
        this.ProjectLikesCount = metrics.SocialEngagementStatistics.UserCount;
      if (this.ProjectLikesCount < 10 || metrics.SocialEngagementAggregateData == null)
        return;
      IList<SocialEngagementAggregate> engagementAggregateData = metrics.SocialEngagementAggregateData;
      int likeCountForXdays1 = SocialEngagementActivities.CalculateLikeCountForXDays(engagementAggregateData, 1);
      int likeCountForXdays2 = SocialEngagementActivities.CalculateLikeCountForXDays(engagementAggregateData, 7);
      int likeCountForXdays3 = SocialEngagementActivities.CalculateLikeCountForXDays(engagementAggregateData, 30);
      if (this.ProjectLikesCount - likeCountForXdays1 > 0)
        this.TrendFactorCount1Day = likeCountForXdays1 * 10000 / (this.ProjectLikesCount - likeCountForXdays1);
      if (this.ProjectLikesCount - likeCountForXdays2 > 0)
        this.TrendFactorCount7Days = likeCountForXdays2 * 10000 / (this.ProjectLikesCount - likeCountForXdays2);
      if (this.ProjectLikesCount - likeCountForXdays3 <= 0)
        return;
      this.TrendFactorCount30Days = likeCountForXdays3 * 10000 / (this.ProjectLikesCount - likeCountForXdays3);
    }

    private static int CalculateLikeCountForXDays(
      IList<SocialEngagementAggregate> socialEngagementTrend,
      int xdays)
    {
      DateTime timeFrom = DateTime.UtcNow.Subtract(TimeSpan.FromHours((double) (24 * xdays)));
      return socialEngagementTrend.Where<SocialEngagementAggregate>((Func<SocialEngagementAggregate, bool>) (x => x.AggregateDateTime.ToUniversalTime() > timeFrom)).Sum<SocialEngagementAggregate>((Func<SocialEngagementAggregate, int>) (x => x.AggregateCount));
    }
  }
}
