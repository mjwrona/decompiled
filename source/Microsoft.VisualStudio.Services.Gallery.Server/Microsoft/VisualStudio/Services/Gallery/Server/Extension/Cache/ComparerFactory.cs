// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.ComparerFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class ComparerFactory
  {
    public IComparer<PublishedExtension> GetComparer(
      SortByType sortByType,
      SortOrderType order,
      double averageRating,
      int minVotesRequired)
    {
      IComparer<PublishedExtension> comparer = (IComparer<PublishedExtension>) null;
      switch (sortByType)
      {
        case SortByType.Relevance:
        case SortByType.InstallCount:
          comparer = (IComparer<PublishedExtension>) new StatisticComparer(order, "install");
          break;
        case SortByType.LastUpdatedDate:
          comparer = (IComparer<PublishedExtension>) new LastUpdatedDateComparer(order);
          break;
        case SortByType.Title:
          comparer = (IComparer<PublishedExtension>) new TitleComparer(order);
          break;
        case SortByType.Publisher:
          comparer = (IComparer<PublishedExtension>) new PublisherNameComparer(order);
          break;
        case SortByType.PublishedDate:
          comparer = (IComparer<PublishedExtension>) new PublishedDateComparer(order);
          break;
        case SortByType.AverageRating:
          comparer = (IComparer<PublishedExtension>) new StatisticComparer(order, "averagerating", "ratingcount");
          break;
        case SortByType.TrendingDaily:
          comparer = (IComparer<PublishedExtension>) new StatisticComparer(order, "trendingdaily", "install");
          break;
        case SortByType.TrendingWeekly:
          comparer = (IComparer<PublishedExtension>) new StatisticComparer(order, "trendingweekly", "install");
          break;
        case SortByType.TrendingMonthly:
          comparer = (IComparer<PublishedExtension>) new StatisticComparer(order, "trendingmonthly", "install");
          break;
        case SortByType.ReleaseDate:
          comparer = (IComparer<PublishedExtension>) new ReleaseDateComparer(order);
          break;
        case SortByType.WeightedRating:
          comparer = (IComparer<PublishedExtension>) new WeightedRatingComparer(order, averageRating, minVotesRequired);
          break;
      }
      return comparer;
    }
  }
}
