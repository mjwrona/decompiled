// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.TotalRunTimeChart
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
using System.Web.UI.WebControls;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts
{
  public class TotalRunTimeChart : ChartBase, IDisposable
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
    private int c_lineBuffer = 10;
    private int c_overAllBuffer = 150;
    private int c_limitedBrowserEntryLimit = 5;
    private Dictionary<string, Guid> m_jobNamesToGuids = new Dictionary<string, Guid>();
    private const string c_chartSeriesId = "Job Count";

    public TotalRunTimeChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      int height = 300,
      int width = 1000,
      bool legendEnabled = true)
      : base(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      this.Chart.ChartAreas[0].AxisX.Interval = 1.0;
      this.Chart.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep45;
      this.Chart.ChartAreas[0].AxisY.Title = MonitoringServerResources.ChartTotalRunTimeYAxisTitle;
      TeamFoundationJobReportingService service = requestContext.GetService<TeamFoundationJobReportingService>();
      int jobsToShowInChart = service.NumberOfJobsToShowInChart;
      List<TeamFoundationJobReportingHistoryQueueTime> historyQueueTimeList = service.QueryJobCountsAndRunTime(requestContext, new int?(jobsToShowInChart));
      historyQueueTimeList.Sort((Comparison<TeamFoundationJobReportingHistoryQueueTime>) ((a, b) => a.TotalRunTimeMilliseconds.CompareTo(b.TotalRunTimeMilliseconds)));
      if (this.IsBrowserLimited() && historyQueueTimeList.Count - this.c_limitedBrowserEntryLimit >= 1)
        historyQueueTimeList.RemoveRange(0, historyQueueTimeList.Count - this.c_limitedBrowserEntryLimit);
      this.AddChartSeries("Job Count", SeriesChartType.Bar, ChartValueType.String, ChartValueType.Int64).Color = Color.Blue;
      int num = 0;
      foreach (TeamFoundationJobReportingHistoryQueueTime entry in historyQueueTimeList)
      {
        DataPoint dataPoint = new DataPoint();
        string jobName = service.GetJobName(requestContext, entry.JobId);
        dataPoint.SetValueXY((object) jobName, (object) entry.TotalRunTimeMilliseconds);
        if (!this.m_jobNamesToGuids.ContainsKey(jobName))
          this.m_jobNamesToGuids.Add(jobName, entry.JobId);
        this.UpdateDataPoint(ref dataPoint, entry);
        this.Chart.Series["Job Count"].Points.Add(dataPoint);
        ++num;
      }
      this.Chart.Legends.Clear();
      this.Chart.IsMapEnabled = true;
      this.Chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
      this.Chart.Height = (Unit) (int) ((double) num * ((double) this.Chart.ChartAreas[0].AxisY.LabelStyle.Font.Size + (double) this.c_lineBuffer) + (double) this.c_overAllBuffer);
    }

    public override void MyCustomizeTest(object sender, EventArgs e)
    {
      foreach (CustomLabel customLabel in (Collection<CustomLabel>) this.Chart.ChartAreas[0].AxisY.CustomLabels)
      {
        if (!string.IsNullOrEmpty(customLabel.Text.Trim()))
        {
          TimeSpan ts = TimeSpan.FromMilliseconds(double.Parse(customLabel.Text));
          customLabel.ToolTip = string.Format(MonitoringServerResources.ChartSummaryLableToolTipFormat, (object) (int) ts.TotalDays, (object) ts.Hours, (object) ts.Minutes, (object) ts.Seconds, (object) ts.Milliseconds);
          customLabel.Text = this.FormatTimeSpan(ts);
        }
      }
      foreach (CustomLabel customLabel in (Collection<CustomLabel>) this.Chart.ChartAreas[0].AxisX.CustomLabels)
      {
        Guid jobId;
        if (this.m_jobNamesToGuids.TryGetValue(customLabel.Text, out jobId))
          customLabel.Url = this.BuildHistoryUrl(jobId);
      }
    }

    private void UpdateDataPoint(
      ref DataPoint dataPoint,
      TeamFoundationJobReportingHistoryQueueTime entry)
    {
      dataPoint.ToolTip = this.GetToolTipText(entry);
      dataPoint.Url = this.BuildHistoryUrl(entry.JobId);
    }

    private string BuildHistoryUrl(Guid jobId) => string.Format("#_a=history&id={0}", (object) jobId);

    private string GetToolTipText(TeamFoundationJobReportingHistoryQueueTime entry) => string.Format(MonitoringServerResources.ChartTotalRunTimeToolTipText, (object) Environment.NewLine, (object) entry.JobId, (object) entry.Count, (object) this.FormatTimeSpan((double) entry.TotalRunTimeMilliseconds));

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
