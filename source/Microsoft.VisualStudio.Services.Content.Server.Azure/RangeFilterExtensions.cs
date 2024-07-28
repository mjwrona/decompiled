// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.RangeFilterExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class RangeFilterExtensions
  {
    public static RangeFilter<RowKeyColumn> Intersect(
      this RangeFilter<RowKeyColumn> range1,
      RangeFilter<RowKeyColumn> range2)
    {
      ArgumentUtility.CheckForNull<RangeFilter<RowKeyColumn>>(range1, nameof (range1));
      ArgumentUtility.CheckForNull<RangeFilter<RowKeyColumn>>(range2, nameof (range2));
      return new RangeFilter<RowKeyColumn>(RangeFilterExtensions.GetMaxOf(range1.Minimum, range2.Minimum), RangeFilterExtensions.GetMinOf(range1.Maximum, range2.Maximum));
    }

    private static RangeMinimumBoundary<RowKeyColumn> GetMaxOf(
      RangeMinimumBoundary<RowKeyColumn> boundary1,
      RangeMinimumBoundary<RowKeyColumn> boundary2)
    {
      int num = boundary1.BoundaryValue.Value.CompareTo(boundary2.BoundaryValue.Value);
      return num < 0 || num <= 0 && boundary1.BoundaryType != RangeBoundaryType.Inclusive ? boundary2 : boundary1;
    }

    private static RangeMaximumBoundary<RowKeyColumn> GetMinOf(
      RangeMaximumBoundary<RowKeyColumn> boundary1,
      RangeMaximumBoundary<RowKeyColumn> boundary2)
    {
      int num = boundary1.BoundaryValue.Value.CompareTo(boundary2.BoundaryValue.Value);
      return num < 0 || num <= 0 && boundary1.BoundaryType == RangeBoundaryType.Inclusive ? boundary1 : boundary2;
    }
  }
}
