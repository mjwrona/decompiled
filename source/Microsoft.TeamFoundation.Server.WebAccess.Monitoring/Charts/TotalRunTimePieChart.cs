// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.TotalRunTimePieChart
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts
{
  public class TotalRunTimePieChart : ChartBase
  {
    private const int c_defaultHeight = 500;
    private const int c_defaultWidth = 1000;
    private Color c_numberOfChartLineColor = Color.LightSalmon;
    private Color c_avgQueueTimeLineColor = Color.DarkGreen;
    private Color c_avgRunTimeLineColor = Color.DarkBlue;
    private bool m_useDataPoints = true;
    private int c_markerSize = 3;
    private Color c_markerColor = Color.Yellow;
    private MarkerStyle c_markerStyle = MarkerStyle.Circle;
    private Color c_markerBorderColor = Color.Black;
    private int c_markerBorderWidth = 1;
    private const string c_chartSeriesId = "Job Results";

    public TotalRunTimePieChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      int height = 500,
      int width = 1000,
      bool legendEnabled = true)
      : base(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      new ChartArea3DStyle(this.Chart.ChartAreas[0]).WallWidth = 2;
      this.Chart.ChartAreas[0].Area3DStyle.Enable3D = true;
      this.Chart.ChartAreas[0].Area3DStyle.Inclination = 60;
      List<TeamFoundationJobReportingResultTypeCount> resultTypeCount = requestContext.GetService<TeamFoundationJobReportingService>().GetResultTypeCount(requestContext);
      this.AddChartSeries("Job Results", SeriesChartType.Pie, ChartValueType.String, ChartValueType.Int64).CustomProperties = "PieLabelStyle=Outside";
      foreach (TeamFoundationJobReportingResultTypeCount entry in resultTypeCount)
      {
        DataPoint dataPoint = new DataPoint();
        dataPoint.Color = this.GetResultTypeColor(entry.ResultTypeId);
        dataPoint.SetValueXY((object) entry.ResultTypeName, (object) entry.Count);
        dataPoint.Label = string.Empty;
        dataPoint.LabelUrl = this.BuildHistoryUrl((int) entry.ResultTypeId);
        this.UpdateDataPoint(ref dataPoint, entry);
        this.Chart.Series["Job Results"].Points.Add(dataPoint);
      }
      this.Chart.Legends.Clear();
      this.Chart.IsMapEnabled = true;
    }

    private void UpdateDataPoint(
      ref DataPoint dataPoint,
      TeamFoundationJobReportingResultTypeCount entry)
    {
      dataPoint.ToolTip = this.GetToolTipText(entry);
      dataPoint.Url = this.BuildHistoryUrl((int) entry.ResultTypeId);
    }

    private string BuildHistoryUrl(int resultType) => string.Format("#_a=history&result={0}", (object) resultType);

    private string GetToolTipText(TeamFoundationJobReportingResultTypeCount entry) => string.Format(MonitoringServerResources.ChartTotalRunTimePieToolTipText, (object) entry.Count);

    private void UpdateDataPoint(ref DataPoint dataPoint)
    {
      if (!this.m_useDataPoints)
        return;
      dataPoint.MarkerSize = this.c_markerSize;
      dataPoint.MarkerColor = this.c_markerColor;
      dataPoint.MarkerStyle = this.c_markerStyle;
      dataPoint.MarkerBorderColor = this.c_markerBorderColor;
      dataPoint.MarkerBorderWidth = this.c_markerBorderWidth;
    }
  }
}
