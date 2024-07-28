// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.AvgRunQueueTimeAndTotalJobsChart
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts
{
  public class AvgRunQueueTimeAndTotalJobsChart : ChartBase
  {
    private const int c_defaultHeight = 300;
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
    private Decimal m_maxYValueMs;

    public AvgRunQueueTimeAndTotalJobsChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      Guid? jobId = null)
      : this(requestContext, limitedBrowser, 300, 1000, true, jobId)
    {
    }

    public override void MyCustomizeTest(object sender, EventArgs e)
    {
      foreach (CustomLabel customLabel in (Collection<CustomLabel>) this.Chart.ChartAreas[0].AxisY.CustomLabels)
      {
        double result;
        if (double.TryParse(customLabel.Text, out result))
        {
          TimeSpan ts = TimeSpan.FromMilliseconds(result);
          customLabel.ToolTip = string.Format(MonitoringServerResources.ChartSummaryLableToolTipFormat, (object) (int) ts.TotalDays, (object) ts.Hours, (object) ts.Minutes, (object) ts.Seconds, (object) ts.Milliseconds);
          customLabel.Text = this.FormatTimeSpan(ts);
        }
      }
    }

    public AvgRunQueueTimeAndTotalJobsChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      int height = 300,
      int width = 1000,
      bool legendEnabled = true,
      Guid? jobId = null)
      : base(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      TeamFoundationJobReportingService service = requestContext.GetService<TeamFoundationJobReportingService>();
      DateTime startTime;
      DateTime endTime;
      List<TeamFoundationJobReportingJobCountsAndRunTime> results = service.QueryQueueTimes(requestContext, service.MaxNumberOfHistoryResults, jobId, out startTime, out endTime);
      this.SetTitle(requestContext, jobId);
      this.Chart.ChartAreas[0].AxisX.Interval = 8.0;
      this.Chart.ChartAreas[0].AxisX.RoundAxisValues();
      this.Chart.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep45;
      this.Chart.ChartAreas[0].AxisY.Title = MonitoringServerResources.HistoryAverageTimeAxisLabel;
      this.Chart.ChartAreas[0].AxisY2.Title = MonitoringServerResources.HistoryJobCountsAxisLabel;
      Series series1 = this.AddChartSeries(MonitoringServerResources.AverageChartNumberOfJobsSeriesLabel, SeriesChartType.Area, ChartValueType.String, ChartValueType.Double, AxisType.Secondary);
      series1.Color = this.c_numberOfChartLineColor;
      Series series2 = this.AddChartSeries(MonitoringServerResources.AverageChartAverageQueueTimeSeriesLabel, SeriesChartType.Line, ChartValueType.String, ChartValueType.Double);
      series2.Color = this.c_avgQueueTimeLineColor;
      Series series3 = this.AddChartSeries(MonitoringServerResources.AverageChartAverageRunTimeSeriesLabel, SeriesChartType.Line, ChartValueType.String, ChartValueType.Double);
      series3.Color = this.c_avgRunTimeLineColor;
      foreach (TeamFoundationJobReportingJobCountsAndRunTime countsAndRunTime in this.EnsureValidResultTimeRange(startTime, endTime, results))
      {
        DataPoint dataPoint1 = new DataPoint();
        dataPoint1.SetValueXY((object) string.Format(MonitoringServerResources.ChartTimeFormatForHours, (object) countsAndRunTime.StartTime), (object) countsAndRunTime.Count);
        series1.Points.Add(dataPoint1);
        DataPoint dataPoint2 = new DataPoint();
        dataPoint2.SetValueXY((object) string.Format(MonitoringServerResources.ChartTimeFormatForHours, (object) countsAndRunTime.StartTime), (object) countsAndRunTime.AvgQueueTimeInMs);
        series2.Points.Add(dataPoint2);
        this.UpdateDataPoint(ref dataPoint2);
        DataPoint dataPoint3 = new DataPoint();
        dataPoint3.SetValueXY((object) string.Format(MonitoringServerResources.ChartTimeFormatForHours, (object) countsAndRunTime.StartTime), (object) countsAndRunTime.AvgRunTimeInMs);
        series3.Points.Add(dataPoint3);
        this.UpdateDataPoint(ref dataPoint3);
        this.m_maxYValueMs = Math.Max((Decimal) Math.Max(countsAndRunTime.AvgQueueTimeInMs, countsAndRunTime.AvgRunTimeInMs), this.m_maxYValueMs);
      }
    }

    private List<TeamFoundationJobReportingJobCountsAndRunTime> EnsureValidResultTimeRange(
      DateTime startTime,
      DateTime endTime,
      List<TeamFoundationJobReportingJobCountsAndRunTime> results)
    {
      if (results.Count == (endTime - startTime).Hours)
        return results;
      List<TeamFoundationJobReportingJobCountsAndRunTime> countsAndRunTimeList = new List<TeamFoundationJobReportingJobCountsAndRunTime>();
      for (DateTime dateTime = startTime; dateTime < results[0].StartTime; dateTime = dateTime.AddHours(1.0))
      {
        TeamFoundationJobReportingJobCountsAndRunTime countsAndRunTime = new TeamFoundationJobReportingJobCountsAndRunTime()
        {
          AvgQueueTimeInMs = 0,
          AvgRunTimeInMs = 0,
          StartTime = dateTime,
          Count = 0
        };
        countsAndRunTimeList.Add(countsAndRunTime);
      }
      for (int index1 = 1; index1 < results.Count - 1; ++index1)
      {
        countsAndRunTimeList.Add(results[index1]);
        for (int index2 = 1; index2 < (results[index1 + 1].StartTime - results[index1].StartTime).Hours; ++index2)
        {
          TeamFoundationJobReportingJobCountsAndRunTime countsAndRunTime = new TeamFoundationJobReportingJobCountsAndRunTime()
          {
            AvgQueueTimeInMs = 0,
            AvgRunTimeInMs = 0,
            StartTime = results[index1].StartTime.AddHours((double) index2),
            Count = 0
          };
          countsAndRunTimeList.Add(countsAndRunTime);
        }
      }
      countsAndRunTimeList.Add(results[results.Count - 1]);
      for (DateTime dateTime = results[results.Count - 1].StartTime; dateTime < endTime; dateTime = dateTime.AddHours(1.0))
      {
        TeamFoundationJobReportingJobCountsAndRunTime countsAndRunTime = new TeamFoundationJobReportingJobCountsAndRunTime()
        {
          AvgQueueTimeInMs = 0,
          AvgRunTimeInMs = 0,
          StartTime = dateTime,
          Count = 0
        };
        countsAndRunTimeList.Add(countsAndRunTime);
      }
      return countsAndRunTimeList;
    }

    private void SetTitle(IVssRequestContext requestContext, Guid? jobId)
    {
      string empty = string.Empty;
      this.DefaultTitle = string.Format(MonitoringServerResources.ConcatenatedJobTitle, jobId.HasValue ? (object) string.Format(MonitoringServerResources.JobSpeificTitle, (object) requestContext.GetService<TeamFoundationJobReportingService>().GetJobName(requestContext, jobId.Value)) : (object) MonitoringServerResources.AllJobs, (object) this.DefaultTitle);
    }

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
