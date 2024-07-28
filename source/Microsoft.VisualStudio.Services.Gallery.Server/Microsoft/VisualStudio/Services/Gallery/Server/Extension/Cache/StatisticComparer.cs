// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.StatisticComparer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class StatisticComparer : Comparer<PublishedExtension>
  {
    private readonly SortOrderType sortOrderType;
    private readonly string primaryStatisticName;
    private readonly string secondaryStatisticName;

    public StatisticComparer(
      SortOrderType sortOrderType,
      string primaryStatisticName,
      string secondaryStatisticName = null)
    {
      this.sortOrderType = sortOrderType;
      this.primaryStatisticName = primaryStatisticName;
      this.secondaryStatisticName = secondaryStatisticName;
    }

    protected double GetStatisticValue(PublishedExtension extension, string statisticName)
    {
      double statisticValue = 0.0;
      if (!extension.Statistics.IsNullOrEmpty<ExtensionStatistic>())
      {
        foreach (ExtensionStatistic statistic in extension.Statistics)
        {
          if (statistic.StatisticName.Equals(statisticName))
            statisticValue = statistic.Value;
        }
      }
      return statisticValue;
    }

    public override int Compare(PublishedExtension extension1, PublishedExtension extension2)
    {
      double statisticValue1 = this.GetStatisticValue(extension1, this.primaryStatisticName);
      double statisticValue2 = this.GetStatisticValue(extension2, this.primaryStatisticName);
      double num1 = 0.0;
      double num2 = 0.0;
      if (this.secondaryStatisticName != null)
      {
        num1 = this.GetStatisticValue(extension1, this.secondaryStatisticName);
        num2 = this.GetStatisticValue(extension2, this.secondaryStatisticName);
      }
      double num3 = 1E-05;
      if (Math.Abs(statisticValue1 - statisticValue2) < num3)
      {
        if (this.secondaryStatisticName == null || Math.Abs(num1 - num2) < num3)
          return 0;
        return num1 < num2 ? (this.sortOrderType != SortOrderType.Ascending ? 1 : -1) : (this.sortOrderType != SortOrderType.Ascending ? -1 : 1);
      }
      return statisticValue1 < statisticValue2 ? (this.sortOrderType != SortOrderType.Ascending ? 1 : -1) : (this.sortOrderType != SortOrderType.Ascending ? -1 : 1);
    }
  }
}
