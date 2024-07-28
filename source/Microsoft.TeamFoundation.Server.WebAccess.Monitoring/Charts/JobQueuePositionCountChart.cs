// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.JobQueuePositionCountChart
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts
{
  public class JobQueuePositionCountChart : ChartBase
  {
    private const int c_defaultHeight = 300;
    private const int c_defaultWidth = 1000;
    private const string c_jobQueueCountsSeriesId = "Job Queue Counts";
    private Color c_avgRunTimeLineColor = Color.DarkBlue;

    public JobQueuePositionCountChart(
      IVssRequestContext requestContext,
      bool limitedBrowser = false,
      int height = 300,
      int width = 1000,
      bool legendEnabled = true)
      : base(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      this.Chart.ChartAreas[0].AxisX.Interval = 1.0;
      this.Chart.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep45;
      this.Chart.ChartAreas[0].AxisY.Title = MonitoringServerResources.ChartQueuePositionYAxixTitle;
      List<TeamFoundationJobReportingQueuePositionCount> queuePositionCountList = requestContext.GetService<TeamFoundationJobReportingService>().QueryQueuePositionCounts(requestContext);
      this.AddChartSeries("Job Queue Counts", SeriesChartType.Column, ChartValueType.String, ChartValueType.Int64).Color = Color.Blue;
      foreach (TeamFoundationJobReportingQueuePositionCount entry in queuePositionCountList)
      {
        DataPoint dataPoint = new DataPoint();
        dataPoint.SetValueXY((object) this.GetQueuePositionName(entry.QueuePosition), (object) entry.JobCount);
        this.UpdateDataPoint(ref dataPoint, entry);
        this.Chart.Series["Job Queue Counts"].Points.Add(dataPoint);
      }
      this.Chart.IsMapEnabled = true;
    }

    private string GetQueuePositionName(int position)
    {
      switch (position)
      {
        case 1:
          return "In Progress";
        case 2:
          return "Queued";
        case 3:
          return "Scheduled";
        case 4:
          return "Host Offline";
        case 5:
          return "Host Dormant";
        default:
          return position.ToString();
      }
    }

    private void UpdateDataPoint(
      ref DataPoint dataPoint,
      TeamFoundationJobReportingQueuePositionCount entry)
    {
      dataPoint.ToolTip = this.GetToolTipText(entry);
      dataPoint.Url = string.Format("#_a=queue&position={0}", (object) entry.QueuePosition);
    }

    private string GetToolTipText(TeamFoundationJobReportingQueuePositionCount entry) => string.Format(MonitoringServerResources.ChartQueuePositionToolTipFormat, (object) Environment.NewLine, (object) entry.QueuePosition, (object) entry.JobCount);
  }
}
