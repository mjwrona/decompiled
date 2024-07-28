// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.JobResultsOverTimeChart
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Common;
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
  public class JobResultsOverTimeChart : ChartBase
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

    public JobResultsOverTimeChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      int height = 300,
      int width = 1000,
      bool legendEnabled = true)
      : base(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      this.Chart.ChartAreas[0].AxisX.Interval = 1.0;
      this.Chart.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep45;
      this.Chart.ChartAreas[0].AxisY.Title = MonitoringServerResources.ChartJobResultsOverTimeYAxixTitle;
      TeamFoundationJobReportingService service = requestContext.GetService<TeamFoundationJobReportingService>();
      List<TeamFoundationJobReportingResultsOverTime> reportingResultsOverTimeList = service.QueryResultsOverTime(requestContext);
      reportingResultsOverTimeList.Sort((Comparison<TeamFoundationJobReportingResultsOverTime>) ((a, b) => a.TotalCount.CompareTo(b.TotalCount)));
      if (this.IsBrowserLimited() && reportingResultsOverTimeList.Count - this.c_limitedBrowserEntryLimit >= 1)
        reportingResultsOverTimeList.RemoveRange(0, reportingResultsOverTimeList.Count - this.c_limitedBrowserEntryLimit);
      foreach (TeamFoundationJobResult jobResult in Enum.GetValues(typeof (TeamFoundationJobResult)))
      {
        switch (jobResult)
        {
          case TeamFoundationJobResult.None:
          case TeamFoundationJobResult.Last:
            continue;
          default:
            this.AddChartSeries(jobResult.ToString(), SeriesChartType.StackedBar, ChartValueType.String, ChartValueType.Int64).Color = this.GetResultTypeColor(jobResult);
            continue;
        }
      }
      int num = 0;
      foreach (TeamFoundationJobReportingResultsOverTime entry in reportingResultsOverTimeList)
      {
        string jobName = service.GetJobName(requestContext, entry.JobId);
        if (!string.IsNullOrEmpty(jobName))
        {
          if (!this.m_jobNamesToGuids.ContainsKey(jobName))
            this.m_jobNamesToGuids.Add(jobName, entry.JobId);
          DataPoint dataPoint1 = new DataPoint();
          dataPoint1.SetValueXY((object) jobName, (object) entry.SucceededCount);
          this.UpdateDataPoint(ref dataPoint1, entry, jobName);
          this.Chart.Series["Succeeded"].Points.Add(dataPoint1);
          DataPoint dataPoint2 = new DataPoint();
          dataPoint2.SetValueXY((object) jobName, (object) entry.InactiveCount);
          this.UpdateDataPoint(ref dataPoint2, entry, jobName);
          this.Chart.Series["Inactive"].Points.Add(dataPoint2);
          DataPoint dataPoint3 = new DataPoint();
          dataPoint3.SetValueXY((object) jobName, (object) entry.DisabledCount);
          this.UpdateDataPoint(ref dataPoint3, entry, jobName);
          this.Chart.Series["Disabled"].Points.Add(dataPoint3);
          DataPoint dataPoint4 = new DataPoint();
          dataPoint4.SetValueXY((object) jobName, (object) entry.StoppedCount);
          this.UpdateDataPoint(ref dataPoint4, entry, jobName);
          this.Chart.Series["Stopped"].Points.Add(dataPoint4);
          DataPoint dataPoint5 = new DataPoint();
          dataPoint5.SetValueXY((object) jobName, (object) entry.BlockedCount);
          this.UpdateDataPoint(ref dataPoint5, entry, jobName);
          this.Chart.Series["Blocked"].Points.Add(dataPoint5);
          DataPoint dataPoint6 = new DataPoint();
          dataPoint6.SetValueXY((object) jobName, (object) entry.PartiallySucceededCount);
          this.UpdateDataPoint(ref dataPoint6, entry, jobName);
          this.Chart.Series["PartiallySucceeded"].Points.Add(dataPoint6);
          DataPoint dataPoint7 = new DataPoint();
          dataPoint7.SetValueXY((object) jobName, (object) entry.FailedCount);
          this.UpdateDataPoint(ref dataPoint7, entry, jobName);
          this.Chart.Series["Failed"].Points.Add(dataPoint7);
          DataPoint dataPoint8 = new DataPoint();
          dataPoint8.SetValueXY((object) jobName, (object) entry.KilledCount);
          this.UpdateDataPoint(ref dataPoint8, entry, jobName);
          this.Chart.Series["Killed"].Points.Add(dataPoint8);
          DataPoint dataPoint9 = new DataPoint();
          dataPoint9.SetValueXY((object) jobName, (object) entry.ExtensionNotFoundCount);
          this.UpdateDataPoint(ref dataPoint9, entry, jobName);
          this.Chart.Series["ExtensionNotFound"].Points.Add(dataPoint9);
          ++num;
        }
      }
      this.Chart.IsMapEnabled = true;
      this.Chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
      this.Chart.Height = (Unit) (int) ((double) num * ((double) this.Chart.ChartAreas[0].AxisY.LabelStyle.Font.Size + (double) this.c_lineBuffer) + (double) this.c_overAllBuffer);
    }

    public override void MyCustomizeTest(object sender, EventArgs e)
    {
      foreach (CustomLabel customLabel in (Collection<CustomLabel>) this.Chart.ChartAreas[0].AxisX.CustomLabels)
      {
        if (this.m_jobNamesToGuids.TryGetValue(customLabel.Text, out Guid _))
          customLabel.Url = this.BuildHistoryUrl(this.m_jobNamesToGuids[customLabel.Text]);
      }
    }

    private void UpdateDataPoint(
      ref DataPoint dataPoint,
      TeamFoundationJobReportingResultsOverTime entry,
      string jobName)
    {
      dataPoint.ToolTip = this.GetToolTipText(entry, jobName);
      dataPoint.Url = this.BuildHistoryUrl(entry.JobId);
    }

    public string BuildHistoryUrl(Guid jobId) => string.Format("#_a=history&id={0}", (object) jobId);

    private string GetToolTipText(TeamFoundationJobReportingResultsOverTime entry, string jobName) => string.Format(MonitoringServerResources.JobResultsChartToolTipText, (object) Environment.NewLine, (object) jobName, (object) entry.SucceededCount, (object) entry.PartiallySucceededCount, (object) entry.FailedCount, (object) entry.StoppedCount, (object) entry.KilledCount, (object) entry.BlockedCount, (object) entry.ExtensionNotFoundCount, (object) entry.InactiveCount, (object) entry.DisabledCount);

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
