// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.ChartDimensionality
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class ChartDimensionality
  {
    public static ChartDimensionality FromTransformOptions(
      TransformOptions options,
      bool areHistoryOptionsUnmodified)
    {
      bool isTrend = !string.IsNullOrEmpty(options.HistoryRange);
      bool flag = !string.IsNullOrEmpty(options.Series);
      return new ChartDimensionality(!isTrend ? (flag ? 2 : 1) : (!areHistoryOptionsUnmodified ? (flag ? 2 : 1) : (!string.IsNullOrEmpty(options.GroupBy) ? 2 : 1)), isTrend);
    }

    private ChartDimensionality(int totalDimensions, bool isTrend)
    {
      this.TotalDimensions = totalDimensions;
      this.IsTrend = isTrend;
    }

    public int TotalDimensions { get; private set; }

    public bool IsTrend { get; private set; }

    internal bool IsOneDimensionalSnapshot => !this.IsTrend && this.TotalDimensions == 1;
  }
}
