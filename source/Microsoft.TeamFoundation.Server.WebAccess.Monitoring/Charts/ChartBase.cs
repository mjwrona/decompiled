// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts.ChartBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Charts
{
  public class ChartBase : IDisposable
  {
    private Color s_DefaultTextColor = Color.FromArgb(33, 33, 33);
    private Font s_DefaultFontBold = new Font("Segoe UI", 8.25f, FontStyle.Bold);
    private Font s_DefaultFont = new Font("Segoe UI", 8.25f);
    private Chart m_chart;
    private const string c_chartAreaName = "MonitoringChartArea";
    protected DateTime m_startTime = DateTime.MinValue;
    protected DateTime m_endTime = DateTime.MinValue;
    private string m_defaultTitle;
    private bool m_limitedBrowser;

    protected ChartBase(
      IVssRequestContext requestContext,
      bool limitedBrowser,
      DateTime startTime,
      DateTime endTime,
      int height,
      int width,
      bool legendEnabled = true)
      : this(requestContext, limitedBrowser, height, width, legendEnabled)
    {
      this.m_startTime = startTime;
      this.m_endTime = endTime;
    }

    protected string FormatTimeSpan(double milliseconds) => this.FormatTimeSpan(TimeSpan.FromMilliseconds(milliseconds));

    protected Color GetResultTypeColor(TeamFoundationJobResult jobResult)
    {
      switch (jobResult)
      {
        case TeamFoundationJobResult.Succeeded:
          return Color.Green;
        case TeamFoundationJobResult.PartiallySucceeded:
          return Color.GreenYellow;
        case TeamFoundationJobResult.Failed:
          return Color.Red;
        case TeamFoundationJobResult.Stopped:
          return Color.Orange;
        case TeamFoundationJobResult.Killed:
          return Color.DarkRed;
        case TeamFoundationJobResult.Blocked:
          return Color.Black;
        case TeamFoundationJobResult.ExtensionNotFound:
          return Color.DarkRed;
        case TeamFoundationJobResult.Inactive:
          return Color.LightGreen;
        case TeamFoundationJobResult.Disabled:
          return Color.Gray;
        case TeamFoundationJobResult.JobInitializationError:
          return Color.Purple;
        default:
          return Color.Yellow;
      }
    }

    protected string FormatTimeSpan(TimeSpan ts)
    {
      if (ts.TotalHours >= 1.0)
        return string.Format(MonitoringServerResources.ChartSummaryLabelTextFormatHours, (object) ts.TotalHours);
      if (ts.TotalSeconds < 1.0)
        return string.Format(MonitoringServerResources.ChartSummaryLabelTextFormatSeconds, (object) ts.TotalSeconds);
      if (ts.TotalMinutes < 1.0)
        return string.Format(MonitoringServerResources.ChartSummaryLabelTextFormatSeconds, (object) ts.TotalSeconds);
      return ts.TotalHours < 1.0 ? string.Format(MonitoringServerResources.ChartSummaryLabelTextFormatMinutes, (object) ts.TotalMinutes) : ts.ToString();
    }

    protected ChartBase(
      IVssRequestContext requestContext,
      bool limitiedBrowser,
      int height,
      int width,
      bool legendEnabled = true)
    {
      this.m_limitedBrowser = limitiedBrowser;
      Chart chart = new Chart();
      chart.Height = (Unit) height;
      chart.Width = (Unit) width;
      this.m_chart = chart;
      if (this.m_limitedBrowser)
      {
        this.m_chart.Height = (Unit) (height / 2);
        this.m_chart.Width = (Unit) (int) ((double) width / 1.5);
      }
      ChartArea chartArea = this.AddChartArea();
      chartArea.AxisX = this.NewAxis();
      chartArea.AxisY = this.NewAxis();
      this.m_chart.Legends.Add(new Legend()
      {
        Enabled = legendEnabled
      });
      this.m_endTime = DateTime.Now.ToUniversalTime();
      this.m_startTime = this.m_endTime.Subtract(new TimeSpan(48, 0, 0));
      this.AddTitle(this.DefaultTitle);
      this.Chart.Customize += new EventHandler(new ChartBase.MyCustomize(this.MyCustomizeTest).Invoke);
    }

    public Chart Chart => this.m_chart;

    protected string DefaultTitle
    {
      get
      {
        if (this.m_defaultTitle == null)
        {
          if (this.m_startTime == DateTime.MinValue || this.m_endTime == DateTime.MinValue)
            return string.Empty;
          this.m_defaultTitle = string.Format(MonitoringServerResources.DefaultChartTitle, (object) this.m_startTime, (object) this.m_endTime);
        }
        return this.m_defaultTitle;
      }
      set
      {
        this.m_defaultTitle = value;
        this.ClearTitles();
        this.AddTitle(this.m_defaultTitle);
      }
    }

    public virtual void MyCustomizeTest(object sender, EventArgs e)
    {
    }

    protected bool IsBrowserLimited() => this.m_limitedBrowser;

    protected ChartArea AddChartArea()
    {
      ChartArea chartArea = new ChartArea("MonitoringChartArea");
      this.m_chart.ChartAreas.Add(chartArea);
      return chartArea;
    }

    protected Title AddTitle(string title)
    {
      Title title1 = new Title(title)
      {
        ForeColor = this.s_DefaultTextColor,
        Font = this.s_DefaultFontBold
      };
      this.m_chart.Titles.Add(title1);
      return title1;
    }

    protected void ClearTitles() => this.m_chart.Titles.Clear();

    protected Axis NewAxis(bool enabled = true, bool gridEnabled = true, bool labelsEnabled = true) => new Axis()
    {
      LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap,
      MajorGrid = new Grid()
      {
        LineColor = Color.FromArgb(128, Color.Gainsboro),
        Enabled = gridEnabled
      },
      LabelStyle = new LabelStyle()
      {
        ForeColor = Color.FromArgb(33, 33, 33),
        Font = new Font("Segoe UI", 8.25f),
        Enabled = labelsEnabled
      },
      Enabled = enabled ? AxisEnabled.True : AxisEnabled.False,
      LineColor = Color.FromArgb(227, 227, 227),
      TitleForeColor = this.s_DefaultTextColor,
      TitleFont = this.s_DefaultFontBold
    };

    protected Series AddChartSeries(
      string name,
      SeriesChartType chartType,
      ChartValueType xType,
      ChartValueType yType,
      AxisType yAxisType = AxisType.Primary)
    {
      if (this.Chart.ChartAreas.Count < 1)
        this.AddChartArea();
      Series series = new Series(name)
      {
        ChartArea = this.Chart.ChartAreas[0].Name,
        Legend = this.Chart.Legends[0].Name,
        ChartType = chartType,
        XValueType = xType,
        YValueType = yType,
        YAxisType = yAxisType,
        SmartLabelStyle = new SmartLabelStyle()
        {
          Enabled = true,
          AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No,
          IsMarkerOverlappingAllowed = false,
          MovingDirection = LabelAlignmentStyles.Top,
          CalloutStyle = LabelCalloutStyle.None,
          CalloutLineAnchorCapStyle = LineAnchorCapStyle.None
        }
      };
      this.Chart.Series.Add(series);
      return series;
    }

    public void Dispose() => this.Chart.Customize -= new EventHandler(new ChartBase.MyCustomize(this.MyCustomizeTest).Invoke);

    public delegate void MyCustomize(object sender, EventArgs e);
  }
}
