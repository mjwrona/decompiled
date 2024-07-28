// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.WeightedRatingComparer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class WeightedRatingComparer : StatisticComparer
  {
    private readonly SortOrderType sortOrderType;
    private double averageRating;
    private int minVotesRequired;

    public WeightedRatingComparer(
      SortOrderType sortOrderType,
      double averageRating,
      int minVotesRequired)
      : base(sortOrderType, string.Empty)
    {
      this.sortOrderType = sortOrderType;
      this.averageRating = averageRating;
      this.minVotesRequired = minVotesRequired;
    }

    public override int Compare(PublishedExtension extension1, PublishedExtension extension2)
    {
      double statisticValue1 = this.GetStatisticValue(extension1, "averagerating");
      double statisticValue2 = this.GetStatisticValue(extension2, "averagerating");
      double statisticValue3 = this.GetStatisticValue(extension1, "ratingcount");
      double statisticValue4 = this.GetStatisticValue(extension2, "ratingcount");
      double num1 = statisticValue3 / (statisticValue3 + (double) this.minVotesRequired) * statisticValue1 + (double) this.minVotesRequired / (statisticValue3 + (double) this.minVotesRequired) * this.averageRating;
      double num2 = statisticValue4 / (statisticValue4 + (double) this.minVotesRequired) * statisticValue2 + (double) this.minVotesRequired / (statisticValue4 + (double) this.minVotesRequired) * this.averageRating;
      return this.sortOrderType != SortOrderType.Ascending ? num2.CompareTo(num1) : num1.CompareTo(num2);
    }
  }
}
