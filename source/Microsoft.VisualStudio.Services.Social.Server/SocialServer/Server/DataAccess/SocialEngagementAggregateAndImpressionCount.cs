// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialEngagementAggregateAndImpressionCount
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialEngagementAggregateAndImpressionCount
  {
    public SocialEngagementAggregate socialEngagementAggregate;
    public SocialEngagementStatistics socialEngagementStatistics;

    public static List<SocialEngagementAggregate> GetSocialEngagementAggregateList(
      List<SocialEngagementAggregateAndImpressionCount> socialEngagementAggregateAndImpressionCountList)
    {
      if (socialEngagementAggregateAndImpressionCountList == null || !socialEngagementAggregateAndImpressionCountList.Any<SocialEngagementAggregateAndImpressionCount>())
        return new List<SocialEngagementAggregate>();
      return socialEngagementAggregateAndImpressionCountList.Count == 1 && socialEngagementAggregateAndImpressionCountList[0].socialEngagementAggregate.AggregateDateTime.Equals(DateTime.MinValue) ? new List<SocialEngagementAggregate>() : socialEngagementAggregateAndImpressionCountList.Select<SocialEngagementAggregateAndImpressionCount, SocialEngagementAggregate>((Func<SocialEngagementAggregateAndImpressionCount, SocialEngagementAggregate>) (x => x.socialEngagementAggregate)).ToList<SocialEngagementAggregate>();
    }

    public static SocialEngagementStatistics GetSocialEngagementStatistics(
      List<SocialEngagementAggregateAndImpressionCount> socialEngagementAggregateAndImpressionCountList)
    {
      return socialEngagementAggregateAndImpressionCountList.FirstOrDefault<SocialEngagementAggregateAndImpressionCount>()?.socialEngagementStatistics;
    }
  }
}
