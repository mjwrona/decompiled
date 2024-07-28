// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Model.ChartType
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Model
{
  public class ChartType
  {
    public const string PieChart = "PieChart";
    public const string BarChart = "BarChart";
    public const string ColumnChart = "ColumnChart";
    public const string StackBarChart = "StackBarChart";
    public const string PivotTable = "PivotTable";
    public const string StackAreaChart = "StackAreaChart";
    public const string AreaChart = "AreaChart";
    public const string LineChart = "LineChart";
    private static readonly IEnumerable<string> s_snapshotChartTypes = (IEnumerable<string>) new List<string>()
    {
      nameof (BarChart),
      nameof (ColumnChart),
      nameof (PieChart),
      nameof (PivotTable),
      nameof (StackBarChart)
    };
    private static readonly IEnumerable<string> s_historicalChartTypes = (IEnumerable<string>) new List<string>()
    {
      nameof (StackAreaChart),
      nameof (AreaChart),
      nameof (LineChart)
    };

    public static bool IsSupportedChartType(IVssRequestContext requestContext, string chartType) => ChartType.GetAllSupportedChartTypes().Any<string>((Func<string, bool>) (o => o.Equals(chartType, StringComparison.OrdinalIgnoreCase)));

    public static IEnumerable<string> GetAllSupportedChartTypes() => ChartType.s_snapshotChartTypes.Union<string>(ChartType.s_historicalChartTypes);
  }
}
