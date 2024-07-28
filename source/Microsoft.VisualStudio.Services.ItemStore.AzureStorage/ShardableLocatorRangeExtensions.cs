// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.ShardableLocatorRangeExtensions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public static class ShardableLocatorRangeExtensions
  {
    public static RangeFilter<RowKeyColumn> ToRangeFilter(this ShardableLocatorRange range)
    {
      ArgumentUtility.CheckForNull<ShardableLocatorRange>(range, nameof (range));
      string columnValue1 = TableKeyUtility.RowKeyFromLocator(range.LowerBound.Value.Locator);
      string columnValue2 = TableKeyUtility.RowKeyFromLocator(range.UpperBound.Value.Locator);
      RangeBoundaryType boundaryType1 = ShardableLocatorRangeExtensions.ConvertRangeType(range.LowerBound.RangeType);
      RangeBoundaryType boundaryType2 = ShardableLocatorRangeExtensions.ConvertRangeType(range.UpperBound.RangeType);
      return new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new RowKeyColumnValue(columnValue1), boundaryType1), new RangeMaximumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new RowKeyColumnValue(columnValue2), boundaryType2));
    }

    private static RangeBoundaryType ConvertRangeType(LocatorRangeType locatorRangeType)
    {
      if (locatorRangeType == LocatorRangeType.Inclusive)
        return RangeBoundaryType.Inclusive;
      if (locatorRangeType == LocatorRangeType.Exclusive)
        return RangeBoundaryType.Exclusive;
      throw new ArgumentException(string.Format("{0} with value {1} is unsupported.", (object) nameof (locatorRangeType), (object) locatorRangeType));
    }
  }
}
