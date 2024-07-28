// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.BoardsChartService
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.Azure.Boards.Charts.Exceptions;
using Microsoft.Azure.Boards.Charts.Utility;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace Microsoft.Azure.Boards.Charts
{
  public class BoardsChartService : IBoardsChartService, IVssFrameworkService
  {
    internal const int DEFAULT_MAX_CHART_DIMENSION = 10000;
    internal const string MAX_CHART_DIMENSION_REGISTRY_PATH = "/Configuration/Application/WebAccess/TeamCharts/MaxDimensionInPixels";
    private IChartDataComponent m_chartDataComponent;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Stream GenerateBoardChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      WebApiTeam team,
      string backlogLevelId,
      int width,
      int height,
      bool showDetails,
      string title)
    {
      if (!StringComparer.InvariantCultureIgnoreCase.Equals(name, "cumulativeFlow"))
        throw new ChartDoesNotExistException();
      this.CheckChartDimensions(requestContext, width, height);
      BacklogLevelConfiguration backlogLevel;
      if (!agileSettings.BacklogConfiguration.TryGetBacklogLevelConfiguration(backlogLevelId, out backlogLevel))
        backlogLevel = agileSettings.BacklogConfiguration.RequirementBacklog;
      CumulativeFlowDiagramSettings flowDiagramSettings;
      using (PerformanceTimer.StartMeasure(requestContext, "BoardsChartService.CumulativeFlow.CumulativeFlowDiagramSettings"))
      {
        ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false, true);
        if (teamSettings.CumulativeFlowDiagramSettings != null)
        {
          if (teamSettings.CumulativeFlowDiagramSettings.TryGetValue(backlogLevel.Id, out flowDiagramSettings))
            goto label_9;
        }
        flowDiagramSettings = new CumulativeFlowDiagramSettings();
      }
label_9:
      TimeZoneInfo collectionTimeZone = requestContext.GetCollectionTimeZone();
      DateTime? startDate = flowDiagramSettings.StartDate;
      bool hideIncoming = flowDiagramSettings.HideIncoming;
      bool hideOutgoing = flowDiagramSettings.HideOutgoing;
      CumulativeFlowDiagramInputs cumulativeFlowInputs = ChartUtils.GetCumulativeFlowInputs(requestContext, projectId, agileSettings, backlogLevel, collectionTimeZone, team, startDate, hideIncoming, hideOutgoing);
      ArgumentUtility.CheckForNull<CumulativeFlowDiagramInputs>(cumulativeFlowInputs, "cumulativeFlowInputs");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) cumulativeFlowInputs.FilterFieldValues, "cumulativeFlowDiagramInputs.FilterFieldValues");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) cumulativeFlowInputs.WorkItemStates, "cumulativeFlowDiagramInputs.WorkItemStates");
      using (PerformanceTimer.StartMeasure(requestContext, "BoardsChartService.CumulativeFlow.GenerateBoardChartImage"))
        return this.RenderCumulativeFlowDiagram(requestContext, team.Id, width, height, showDetails, title, cumulativeFlowInputs);
    }

    public Stream GenerateIterationsChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      int iterationsNumber,
      int width,
      int height,
      bool showDetails,
      string title)
    {
      if (!StringComparer.InvariantCultureIgnoreCase.Equals(name, "velocity"))
        throw new ChartDoesNotExistException();
      this.CheckChartDimensions(requestContext, width, height);
      if (agileSettings.TeamSettings.Iterations.IsNullOrEmpty<ITeamIteration>())
        throw new NoIterationsSetForTeamException();
      VelocityChartInputs velocityInputs = ChartUtils.GetVelocityInputs(requestContext, projectId, agileSettings, iterationsNumber);
      if (velocityInputs.Iterations == null || velocityInputs.Iterations.Count == 0)
        throw new NoIterationsSetForTeamException();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) velocityInputs.FilterFieldValues, "velocityChartInputs.TeamFieldValues");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) velocityInputs.Iterations, "velocityChartInputs.Iterations");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) velocityInputs.WorkItemTypes, "velocityChartInputs.WorkItemTypes");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) velocityInputs.CompletedStates, "velocityChartInputs.CompletedStates");
      ArgumentUtility.CheckForNull<List<string>>(velocityInputs.InProgressStates, "velocityChartInputs.InProgressStates");
      ArgumentUtility.CheckStringForNullOrEmpty(velocityInputs.EffortField, "velocityChartInputs.EffortField");
      return this.RenderVelocityChart(requestContext, width, height, showDetails, title, velocityInputs);
    }

    public Stream GenerateIterationChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      Guid iterationId,
      WebApiTeam team,
      int width,
      int height,
      bool showDetails,
      string title)
    {
      if (!StringComparer.InvariantCultureIgnoreCase.Equals(name, "burndown"))
        throw new ChartDoesNotExistException();
      this.CheckChartDimensions(requestContext, width, height);
      if (agileSettings.TeamSettings.Iterations.IsNullOrEmpty<ITeamIteration>())
        throw new NoIterationsSetForTeamException();
      if (agileSettings.TeamSettings.Iterations.FirstOrDefault<ITeamIteration>((Func<ITeamIteration, bool>) (i => i.IterationId == iterationId)) == null)
        throw new IterationNotFoundException(iterationId.ToString());
      string path = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, iterationId).GetPath(requestContext);
      IdentityChartCache identityChartCache = (IdentityChartCache) null;
      if (team != null)
        identityChartCache = IdentityPropertiesView.CreateView<IdentityChartCache>(requestContext, team.Id, TeamConstants.TeamChartCachePropertyName);
      try
      {
        TeamCapacity iterationCapacity = requestContext.GetService<ITeamConfigurationService>().GetTeamIterationCapacity(requestContext, team, agileSettings.TeamSettings, iterationId);
        TimeZoneInfo collectionTimeZone = requestContext.GetCollectionTimeZone();
        BurndownChartInputs burnDownInputs = ChartUtils.GetBurnDownInputs(requestContext, agileSettings, path, collectionTimeZone, identityChartCache, iterationCapacity, true, agileSettings.Process.TaskBacklog.WorkItemCountLimit);
        ArgumentUtility.CheckForNull<BurndownChartInputs>(burnDownInputs, "burndownChartInputs");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) burnDownInputs.FilterFieldValues, "burndownChartInputs.TeamFieldValues");
        ArgumentUtility.CheckForNull<IterationProperties>(burnDownInputs.Iteration, "burndownChartInputs.Iteration");
        ArgumentUtility.CheckStringForNullOrEmpty(burnDownInputs.Iteration.IterationPath, "burndownChartInputs.Iteration.IterationPath");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) burnDownInputs.WorkItemTypes, "burndownChartInputs.WorkItemTypes");
        ArgumentUtility.CheckStringForNullOrEmpty(burnDownInputs.RemainingWorkField, "burndownChartInputs.RemainingWorkField");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) burnDownInputs.InProgressStates, "burndownChartInputs.InProgressStates");
        return this.RenderBurndownChart(requestContext, width, height, showDetails, title, burnDownInputs);
      }
      catch (BurndownWorkItemLimitExceededException ex)
      {
        requestContext.Trace(290266, TraceLevel.Warning, "Agile", TfsTraceLayers.BusinessLogic, ex.Message);
        throw;
      }
    }

    internal Stream RenderVelocityChart(
      IVssRequestContext requestContext,
      int width,
      int height,
      bool showDetails,
      string title,
      VelocityChartInputs velocityChartInputs)
    {
      ThemedChartColorStore themedChartColorStore = new ThemedChartColorStore(requestContext);
      Chart chart1 = new Chart();
      chart1.BackColor = themedChartColorStore.BackgroundColor;
      chart1.Width = (Unit) width;
      chart1.Height = (Unit) height;
      Chart chart2 = chart1;
      if (!string.IsNullOrEmpty(title) & showDetails)
        chart2.Titles.Add(new Title(title)
        {
          ForeColor = themedChartColorStore.ForegroundColor,
          Font = new Font("Segoe UI", 8.25f, FontStyle.Bold)
        });
      chart2.ChartAreas.Add(new ChartArea(ChartsResources.Velocity)
      {
        BackColor = themedChartColorStore.BackgroundColor,
        AxisY = new Axis()
        {
          LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap,
          MajorGrid = new Grid()
          {
            LineColor = Color.FromArgb(128, Color.Gainsboro),
            Enabled = false
          },
          LabelStyle = new LabelStyle()
          {
            ForeColor = themedChartColorStore.ForegroundColor,
            Font = new Font("Segoe UI", 8.25f),
            Enabled = showDetails
          },
          Enabled = AxisEnabled.False,
          LineColor = themedChartColorStore.LineColor,
          TitleForeColor = themedChartColorStore.ForegroundColor,
          TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold)
        },
        AxisX = new Axis()
        {
          LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap,
          MajorGrid = new Grid()
          {
            LineColor = Color.FromArgb(128, Color.Gainsboro),
            Enabled = false
          },
          LabelStyle = new LabelStyle()
          {
            ForeColor = themedChartColorStore.ForegroundColor,
            Font = new Font("Segoe UI", 8.25f),
            Enabled = showDetails
          },
          LineColor = themedChartColorStore.LineColor,
          TitleForeColor = themedChartColorStore.ForegroundColor,
          TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold)
        }
      });
      chart2.Legends.Add(new Legend()
      {
        DockedToChartArea = chart2.ChartAreas[0].Name,
        LegendItemOrder = LegendItemOrder.SameAsSeriesOrder,
        Enabled = false
      });
      SeriesCollection series1 = chart2.Series;
      Series series2 = new Series(ChartsResources.VelocityCompletedSeries);
      series2.ChartArea = chart2.ChartAreas[0].Name;
      series2.ChartType = SeriesChartType.StackedColumn;
      series2.IsValueShownAsLabel = true;
      series2.LabelFormat = "0";
      series2.Color = ChartAreaColors.VelocityCompleted;
      series2.Legend = chart2.Legends[0].Name;
      series2.XValueType = ChartValueType.String;
      series2.YValueType = ChartValueType.Double;
      series1.Add(series2);
      SeriesCollection series3 = chart2.Series;
      Series series4 = new Series(ChartsResources.VelocityRemainingSeries);
      series4.ChartArea = chart2.ChartAreas[0].Name;
      series4.ChartType = SeriesChartType.StackedColumn;
      series4.IsValueShownAsLabel = true;
      series4.LabelFormat = "0";
      series4.Color = ChartAreaColors.VelocityRemaining;
      series4.Legend = chart2.Legends[0].Name;
      series4.XValueType = ChartValueType.String;
      series4.YValueType = ChartValueType.Double;
      series3.Add(series4);
      chart2.Series[0]["PointWidth"] = "0.5";
      chart2.Series[1]["PointWidth"] = "0.5";
      bool flag = true;
      foreach (VelocityChartDataPoint velocityChartDataPoint in this.ChartDataComponent.GetVelocityChartData(requestContext, velocityChartInputs))
      {
        DataPoint dataPoint1 = new DataPoint();
        dataPoint1.SetValueXY((object) velocityChartDataPoint.Iteration, (object) velocityChartDataPoint.CompletedWork);
        dataPoint1.IsValueShownAsLabel = showDetails && velocityChartDataPoint.CompletedWork > 0.0;
        DataPoint dataPoint2 = new DataPoint();
        dataPoint2.SetValueXY((object) velocityChartDataPoint.Iteration, (object) velocityChartDataPoint.RemainingWork);
        dataPoint2.IsValueShownAsLabel = showDetails && velocityChartDataPoint.RemainingWork > 0.0;
        chart2.Series[0].Points.Add(dataPoint1);
        chart2.Series[1].Points.Add(dataPoint2);
        if (velocityChartDataPoint.CompletedWork != 0.0 || velocityChartDataPoint.RemainingWork != 0.0)
          flag = false;
      }
      return flag && !showDetails ? this.Render(themedChartColorStore.IsDarkTheme ? ChartsResources.VelocityChart_ZeroData_Dark : ChartsResources.VelocityChart_ZeroData_Light) : this.Render(chart2);
    }

    internal Stream RenderBurndownChart(
      IVssRequestContext requestContext,
      int width,
      int height,
      bool showDetails,
      string title,
      BurndownChartInputs burndownChartInputs)
    {
      ThemedChartColorStore themedChartColorStore = new ThemedChartColorStore(requestContext);
      if (!burndownChartInputs.Iteration.StartDate.HasValue || !burndownChartInputs.Iteration.FinishDate.HasValue)
        burndownChartInputs.Iteration = this.GetIterationStartFinishDates(requestContext, burndownChartInputs.Iteration.IterationPath);
      this.AdjustIterationDates(requestContext, burndownChartInputs);
      IEnumerable<BurndownChartDataPoint> burndownChartData = this.ChartDataComponent.GetBurndownChartData(requestContext, burndownChartInputs);
      if (burndownChartData.Count<BurndownChartDataPoint>() == 0 & showDetails)
        return Stream.Null;
      Chart chart1 = new Chart();
      chart1.BackColor = themedChartColorStore.BackgroundColor;
      chart1.Width = (Unit) width;
      chart1.Height = (Unit) height;
      Chart chart2 = chart1;
      if (!string.IsNullOrEmpty(title) & showDetails)
        chart2.Titles.Add(new Title(title)
        {
          ForeColor = themedChartColorStore.ForegroundColor,
          Font = new Font("Segoe UI", 8.25f, FontStyle.Bold)
        });
      ChartAreaCollection chartAreas = chart2.ChartAreas;
      ChartArea chartArea = new ChartArea(ChartsResources.Burndown);
      chartArea.BackColor = themedChartColorStore.BackgroundColor;
      Axis axis1 = new Axis();
      axis1.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap;
      axis1.MajorGrid = new Grid()
      {
        LineColor = Color.FromArgb(128, Color.Gainsboro),
        Enabled = showDetails
      };
      TickMark tickMark1 = new TickMark();
      tickMark1.Enabled = showDetails;
      tickMark1.LineColor = themedChartColorStore.LineColor;
      axis1.MajorTickMark = tickMark1;
      axis1.LabelStyle = new LabelStyle()
      {
        ForeColor = themedChartColorStore.ForegroundColor,
        Font = new Font("Segoe UI", 8.25f),
        Enabled = showDetails
      };
      axis1.IsMarginVisible = showDetails;
      axis1.LineColor = themedChartColorStore.LineColor;
      axis1.TitleForeColor = themedChartColorStore.ForegroundColor;
      axis1.TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
      axis1.Title = showDetails ? ChartsResources.BurndownRemainingWorkAxisTitle : string.Empty;
      axis1.Minimum = 0.0;
      chartArea.AxisY = axis1;
      Axis axis2 = new Axis();
      axis2.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap;
      axis2.MajorGrid = new Grid()
      {
        LineColor = Color.FromArgb(128, Color.Gainsboro),
        Enabled = showDetails,
        Interval = (double) (burndownChartData.Count<BurndownChartDataPoint>() < 5 ? 1 : 0)
      };
      TickMark tickMark2 = new TickMark();
      tickMark2.Enabled = showDetails;
      tickMark2.LineColor = themedChartColorStore.LineColor;
      axis2.MajorTickMark = tickMark2;
      axis2.LabelStyle = new LabelStyle()
      {
        ForeColor = themedChartColorStore.ForegroundColor,
        Font = new Font("Segoe UI", 8.25f),
        Enabled = showDetails
      };
      axis2.IsMarginVisible = false;
      axis2.LineColor = themedChartColorStore.LineColor;
      axis2.TitleForeColor = themedChartColorStore.ForegroundColor;
      axis2.TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
      chartArea.AxisX = axis2;
      chartAreas.Add(chartArea);
      chart2.Legends.Add(new Legend()
      {
        DockedToChartArea = chart2.ChartAreas[0].Name,
        IsDockedInsideChartArea = false,
        LegendItemOrder = LegendItemOrder.SameAsSeriesOrder,
        Enabled = showDetails,
        BackColor = themedChartColorStore.BackgroundColor,
        ForeColor = themedChartColorStore.ForegroundColor
      });
      SeriesCollection series1 = chart2.Series;
      Series series2 = new Series(ChartsResources.BurndownRemainingWorkSeries);
      series2.ChartArea = chart2.ChartAreas[0].Name;
      series2.ChartType = SeriesChartType.StackedArea;
      series2.Color = themedChartColorStore.FillColor;
      series2.BackSecondaryColor = ChartAreaColors.BurndownCompleted;
      series2.Legend = chart2.Legends[0].Name;
      series2.XValueType = ChartValueType.Date;
      series2.YValueType = ChartValueType.Double;
      series2.IsXValueIndexed = true;
      series1.Add(series2);
      SeriesCollection series3 = chart2.Series;
      Series series4 = new Series(ChartsResources.BurndownIdealTrendSeries);
      series4.ChartArea = chart2.ChartAreas[0].Name;
      series4.ChartType = SeriesChartType.Line;
      series4.Color = Color.FromArgb(96, 96, 96);
      series4.Legend = chart2.Legends[0].Name;
      series4.XValueType = ChartValueType.Date;
      series4.YValueType = ChartValueType.Double;
      series4.IsXValueIndexed = true;
      series3.Add(series4);
      bool flag1 = burndownChartData.Count<BurndownChartDataPoint>() > 0 && burndownChartData.First<BurndownChartDataPoint>().AvailableCapacity > 0.0;
      if (flag1)
      {
        SeriesCollection series5 = chart2.Series;
        Series series6 = new Series(ChartsResources.AvailableCapacitySeries);
        series6.ChartArea = chart2.ChartAreas[0].Name;
        series6.ChartType = SeriesChartType.Line;
        series6.Color = Color.FromArgb(109, 183, 44);
        series6.Legend = chart2.Legends[0].Name;
        series6.XValueType = ChartValueType.Date;
        series6.YValueType = ChartValueType.Double;
        series6.IsXValueIndexed = true;
        series6.BorderWidth = 2;
        series5.Add(series6);
      }
      DateTime date1 = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, burndownChartInputs.TimeZone).Date;
      bool flag2 = false;
      bool flag3 = true;
      foreach (BurndownChartDataPoint burndownChartDataPoint in burndownChartData)
      {
        double? remainingWork = burndownChartDataPoint.RemainingWork;
        if (remainingWork.HasValue)
        {
          DataPointCollection points = chart2.Series[0].Points;
          // ISSUE: variable of a boxed type
          __Boxed<DateTime> date2 = (ValueType) burndownChartDataPoint.Date;
          object[] objArray = new object[1];
          remainingWork = burndownChartDataPoint.RemainingWork;
          objArray[0] = (object) remainingWork.Value;
          points.AddXY((object) date2, objArray);
          remainingWork = burndownChartDataPoint.RemainingWork;
          if (remainingWork.Value != 0.0)
            flag3 = false;
        }
        else
        {
          DataPoint dataPoint = new DataPoint();
          dataPoint.SetValueXY((object) burndownChartDataPoint.Date, (object) 0);
          chart2.DataManipulator.InsertEmptyPoints(1.0, IntervalType.Days, 1.0, IntervalType.Days, dataPoint.XValue - 1.0, dataPoint.XValue, chart2.Series[0].Name);
        }
        if (showDetails)
        {
          chart2.Series[1].Points.AddXY((object) burndownChartDataPoint.Date, (object) burndownChartDataPoint.IdealTrend);
          if (flag1)
            chart2.Series[2].Points.AddXY((object) burndownChartDataPoint.Date, (object) burndownChartDataPoint.AvailableCapacity);
        }
        if (((flag2 || !(burndownChartDataPoint.Date >= date1) ? 0 : (chart2.Series[0].Points.Count > 0 ? 1 : 0)) & (showDetails ? 1 : 0)) != 0)
        {
          chart2.ChartAreas[0].AxisX.StripLines.Add(new StripLine()
          {
            BackColor = themedChartColorStore.BackgroundColor,
            ForeColor = themedChartColorStore.ForegroundColor,
            BorderWidth = 0,
            BorderDashStyle = ChartDashStyle.Dash,
            BorderColor = themedChartColorStore.ForegroundColor,
            Font = new Font("Segoe UI", 8.25f),
            Text = ChartsResources.Today,
            TextAlignment = StringAlignment.Near,
            TextOrientation = TextOrientation.Horizontal,
            StripWidth = 0.0,
            StripWidthType = DateTimeIntervalType.Days,
            IntervalOffset = (double) chart2.Series[0].Points.Count
          });
          flag2 = true;
        }
      }
      chart2.Series[0].EmptyPointStyle.Color = Color.Transparent;
      chart2.Series[1].EmptyPointStyle.Color = Color.Transparent;
      return flag3 && !showDetails ? this.Render(themedChartColorStore.IsDarkTheme ? ChartsResources.BurndownChart_ZeroData_Dark : ChartsResources.BurndownChart_ZeroData_Light) : this.Render(chart2);
    }

    internal Stream RenderCumulativeFlowDiagram(
      IVssRequestContext requestContext,
      Guid teamId,
      int width,
      int height,
      bool showDetails,
      string title,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs)
    {
      ThemedChartColorStore themedChartColorStore = new ThemedChartColorStore(requestContext);
      IEnumerable<CumulativeFlowDiagramDataPoint> cumulativeFlowDiagramData = this.ChartDataComponent.GetCumulativeFlowDiagramData(requestContext, teamId, cumulativeFlowDiagramInputs);
      int minimumDoneCount = cumulativeFlowDiagramInputs.WorkItemStates.Any<string>((Func<string, bool>) (state => TFStringComparer.WorkItemStateName.Equals(state, cumulativeFlowDiagramInputs.DoneStateName))) ? BoardsChartService.GetMinimumDoneCount(cumulativeFlowDiagramData, cumulativeFlowDiagramInputs) : 0;
      Chart chart1 = new Chart();
      chart1.BackColor = themedChartColorStore.BackgroundColor;
      chart1.Width = (Unit) width;
      chart1.Height = (Unit) height;
      Chart chart2 = chart1;
      if (!string.IsNullOrEmpty(title) & showDetails)
        chart2.Titles.Add(new Title(title)
        {
          ForeColor = themedChartColorStore.ForegroundColor,
          Font = new Font("Segoe UI", 8.25f, FontStyle.Bold)
        });
      ChartAreaCollection chartAreas = chart2.ChartAreas;
      ChartArea chartArea = new ChartArea(ChartsResources.CumulativeFlow);
      chartArea.BackColor = themedChartColorStore.BackgroundColor;
      Axis axis1 = new Axis();
      axis1.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap;
      axis1.MajorGrid = new Grid()
      {
        LineColor = Color.FromArgb(128, Color.Gainsboro),
        Enabled = showDetails
      };
      TickMark tickMark1 = new TickMark();
      tickMark1.Enabled = showDetails;
      tickMark1.LineColor = themedChartColorStore.LineColor;
      axis1.MajorTickMark = tickMark1;
      axis1.LabelStyle = new LabelStyle()
      {
        ForeColor = themedChartColorStore.ForegroundColor,
        Font = new Font("Segoe UI", 8.25f),
        Enabled = showDetails
      };
      axis1.IsMarginVisible = showDetails;
      axis1.LineColor = themedChartColorStore.LineColor;
      axis1.TitleForeColor = themedChartColorStore.ForegroundColor;
      axis1.TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
      axis1.Title = showDetails ? ChartsResources.CumulativeFlowYAxisTitle : string.Empty;
      axis1.Minimum = (double) minimumDoneCount;
      chartArea.AxisY = axis1;
      Axis axis2 = new Axis();
      axis2.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont | LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.LabelsAngleStep30 | LabelAutoFitStyles.WordWrap;
      axis2.MajorGrid = new Grid()
      {
        Interval = 7.0,
        LineColor = Color.FromArgb(128, Color.Gainsboro),
        Enabled = showDetails
      };
      TickMark tickMark2 = new TickMark();
      tickMark2.Enabled = showDetails;
      tickMark2.LineColor = themedChartColorStore.LineColor;
      axis2.MajorTickMark = tickMark2;
      axis2.LabelStyle = new LabelStyle()
      {
        Interval = 7.0,
        ForeColor = themedChartColorStore.ForegroundColor,
        Font = new Font("Segoe UI", 8.25f),
        Enabled = showDetails
      };
      axis2.IsMarginVisible = false;
      axis2.LineColor = themedChartColorStore.LineColor;
      axis2.TitleForeColor = themedChartColorStore.ForegroundColor;
      axis2.TitleFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
      chartArea.AxisX = axis2;
      chartAreas.Add(chartArea);
      List<string> list = cumulativeFlowDiagramInputs.WorkItemStates.ToList<string>();
      if (cumulativeFlowDiagramInputs.UseKanbanColumns)
      {
        foreach (KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>> keyValuePair in cumulativeFlowDiagramInputs.BoardColumnMappings.ToArray<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>>())
        {
          KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>> item = keyValuePair;
          if (!item.Value.Item3 && !cumulativeFlowDiagramData.Any<CumulativeFlowDiagramDataPoint>((Func<CumulativeFlowDiagramDataPoint, bool>) (p => p.Counts.ContainsKey(item.Key) && p.Counts[item.Key] > 0)))
          {
            string state = item.Value.Item1;
            list.Remove(state);
            cumulativeFlowDiagramInputs.RemoveState(state);
          }
        }
      }
      chart2.Legends.Add(new Legend()
      {
        DockedToChartArea = chart2.ChartAreas[0].Name,
        IsDockedInsideChartArea = false,
        LegendItemOrder = LegendItemOrder.SameAsSeriesOrder,
        Enabled = showDetails,
        BackColor = themedChartColorStore.BackgroundColor,
        ForeColor = themedChartColorStore.ForegroundColor,
        Docking = list.Max<string>((Func<string, int>) (s => s.Length)) > 28 ? Docking.Bottom : Docking.Right
      });
      IDictionary<string, Color> colorMap = this.CreateColorMap(cumulativeFlowDiagramInputs);
      for (int index = list.Count - 1; index >= 0; --index)
      {
        string str = list[index];
        Series series1 = new Series(str);
        series1.ChartArea = chart2.ChartAreas[0].Name;
        series1.ChartType = SeriesChartType.StackedArea;
        series1.Legend = chart2.Legends[0].Name;
        series1.XValueType = ChartValueType.Date;
        series1.YValueType = ChartValueType.Int32;
        series1.BorderColor = themedChartColorStore.BackgroundColor;
        series1.BorderWidth = 2;
        series1.Color = colorMap[str];
        Series series2 = series1;
        chart2.Series.Add(series2);
      }
      bool flag = true;
      foreach (CumulativeFlowDiagramDataPoint diagramDataPoint in cumulativeFlowDiagramData)
      {
        if (cumulativeFlowDiagramInputs.UseKanbanColumns)
        {
          foreach (KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>> boardColumnMapping in (IEnumerable<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>>) cumulativeFlowDiagramInputs.BoardColumnMappings)
          {
            if (list.Contains<string>(boardColumnMapping.Value.Item1, (IEqualityComparer<string>) TFStringComparer.BoardColumnName))
            {
              int num = 0;
              if (diagramDataPoint.Counts.ContainsKey(boardColumnMapping.Key))
                num = diagramDataPoint.Counts[boardColumnMapping.Key];
              chart2.Series[boardColumnMapping.Value.Item1].Points.AddXY((object) diagramDataPoint.Date, (object) num);
              if (num != 0)
                flag = false;
            }
          }
        }
        else
        {
          foreach (string key in diagramDataPoint.StateCounts.Keys)
          {
            chart2.Series[key].Points.AddXY((object) diagramDataPoint.Date, (object) diagramDataPoint.StateCounts[key]);
            if (diagramDataPoint.StateCounts[key] != 0)
              flag = false;
          }
        }
      }
      foreach (var data in chart2.Series.Select(s => new
      {
        Name = s.Name,
        Max = ((IEnumerable<double>) s.Points.FindMaxByValue("Y").YValues).Max()
      }))
      {
        if (data.Max < (double) minimumDoneCount)
          requestContext.TraceAlways(290851, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, string.Format("Maximum Y-value is less than minimum done count. Use Kanban Columns: {0}, ", (object) cumulativeFlowDiagramInputs.UseKanbanColumns) + string.Format("Series: {0}, Max: {1}, Minimum Done Count: {2}", (object) data.Name, (object) data.Max, (object) minimumDoneCount));
      }
      return flag && !showDetails ? this.Render(themedChartColorStore.IsDarkTheme ? ChartsResources.CFDChart_ZeroData_Dark : ChartsResources.CFDChart_ZeroData_Light) : this.Render(chart2);
    }

    internal void CheckChartDimensions(IVssRequestContext requestContext, int width, int height)
    {
      int maxChartDimension = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Application/WebAccess/TeamCharts/MaxDimensionInPixels", true, 10000);
      if (width <= 0 || width > maxChartDimension || height <= 0 || height > maxChartDimension)
        throw new InvalidChartDimensionsException(maxChartDimension);
    }

    private void AdjustIterationDates(
      IVssRequestContext requestContext,
      BurndownChartInputs burndownChartInputs)
    {
      IterationProperties iteration = burndownChartInputs.Iteration;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Application/Backlogs/Team/SprintsDirectoryAllPivotPageSize", false, 90);
      DateTime universalTime = DateTime.Today.ToUniversalTime();
      DateTime dateTime1 = universalTime;
      DateTime? startDate = iteration.StartDate;
      DateTime? nullable1;
      TimeSpan? nullable2;
      if ((startDate.HasValue ? (dateTime1 >= startDate.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        nullable1 = iteration.FinishDate;
        DateTime dateTime2 = universalTime;
        nullable2 = nullable1.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - dateTime2) : new TimeSpan?();
        TimeSpan timeSpan = TimeSpan.FromDays((double) num);
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > timeSpan ? 1 : 0) : 0) != 0)
        {
          iteration.FinishDate = new DateTime?(universalTime.Add(TimeSpan.FromDays((double) num)));
          goto label_8;
        }
      }
      DateTime dateTime3 = universalTime;
      nullable1 = iteration.StartDate;
      DateTime? nullable3;
      if ((nullable1.HasValue ? (dateTime3 < nullable1.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        nullable1 = iteration.FinishDate;
        nullable3 = iteration.StartDate;
        nullable2 = nullable1.HasValue & nullable3.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - nullable3.GetValueOrDefault()) : new TimeSpan?();
        TimeSpan timeSpan = TimeSpan.FromDays((double) num);
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > timeSpan ? 1 : 0) : 0) != 0)
        {
          IterationProperties iterationProperties = iteration;
          nullable3 = iteration.StartDate;
          DateTime? nullable4 = new DateTime?(nullable3.Value.Add(TimeSpan.FromDays((double) num)));
          iterationProperties.FinishDate = nullable4;
        }
      }
label_8:
      while (true)
      {
        BurndownChartInputs burndownChartInputs1 = burndownChartInputs;
        nullable3 = iteration.StartDate;
        DateTime date = nullable3.Value;
        if (burndownChartInputs1.IsNonWorkingDay(date))
        {
          nullable3 = iteration.StartDate;
          DateTime dateTime4 = nullable3.Value;
          nullable3 = iteration.FinishDate;
          DateTime dateTime5 = nullable3.Value;
          if (!(dateTime4 == dateTime5))
          {
            IterationProperties iterationProperties = iteration;
            nullable3 = iteration.StartDate;
            DateTime? nullable5 = new DateTime?(nullable3.Value.AddDays(1.0));
            iterationProperties.StartDate = nullable5;
          }
          else
            break;
        }
        else
          break;
      }
      while (true)
      {
        BurndownChartInputs burndownChartInputs2 = burndownChartInputs;
        nullable3 = iteration.FinishDate;
        DateTime date = nullable3.Value;
        if (burndownChartInputs2.IsNonWorkingDay(date))
        {
          nullable3 = iteration.StartDate;
          DateTime dateTime6 = nullable3.Value;
          nullable3 = iteration.FinishDate;
          DateTime dateTime7 = nullable3.Value;
          if (!(dateTime6 == dateTime7))
          {
            IterationProperties iterationProperties = iteration;
            nullable3 = iteration.FinishDate;
            DateTime? nullable6 = new DateTime?(nullable3.Value.AddDays(-1.0));
            iterationProperties.FinishDate = nullable6;
          }
          else
            break;
        }
        else
          goto label_13;
      }
      return;
label_13:;
    }

    private List<BoardsChartService.DayRange> GetDayRanges(List<DayOfWeek> days)
    {
      HashSet<DayOfWeek> dayOfWeekSet = new HashSet<DayOfWeek>((IEnumerable<DayOfWeek>) days);
      List<BoardsChartService.DayRange> dayRanges = new List<BoardsChartService.DayRange>();
      BoardsChartService.DayRange dayRange1 = (BoardsChartService.DayRange) null;
      for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; ++dayOfWeek)
      {
        if (dayOfWeekSet.Contains(dayOfWeek))
        {
          if (dayRange1 == null)
            dayRange1 = new BoardsChartService.DayRange()
            {
              StartDay = dayOfWeek,
              Days = 1
            };
          else
            ++dayRange1.Days;
          if (dayOfWeek == DayOfWeek.Saturday)
          {
            dayRanges.Add(dayRange1);
            dayRange1 = (BoardsChartService.DayRange) null;
          }
        }
        else if (dayRange1 != null)
        {
          dayRanges.Add(dayRange1);
          dayRange1 = (BoardsChartService.DayRange) null;
        }
      }
      if (dayRanges.Count > 1)
      {
        BoardsChartService.DayRange dayRange2 = dayRanges[0];
        BoardsChartService.DayRange dayRange3 = dayRanges[dayRanges.Count - 1];
        if (dayRange2.StartDay == DayOfWeek.Sunday && dayRange3.StartDay + (dayRange3.Days - 1) == DayOfWeek.Saturday)
        {
          dayRange3.Days += dayRange2.Days;
          dayRanges.Remove(dayRange2);
        }
      }
      return dayRanges;
    }

    private IterationProperties GetIterationStartFinishDates(
      IVssRequestContext requestContext,
      string iteration)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      int iterationId = service.GetIterationId(requestContext, iteration);
      if (iterationId != -1)
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TreeNode node = service.GetNode(requestContext, iterationId);
        if (node != null)
        {
          DateTime? nullable = node.StartDate;
          nullable = nullable.HasValue ? node.FinishDate : throw new ChartsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ChartsResources.IterationHasNoStartOrEndDates, (object) iteration));
          if (nullable.HasValue)
          {
            IterationProperties startFinishDates = new IterationProperties();
            startFinishDates.IterationPath = iteration;
            nullable = node.StartDate;
            startFinishDates.StartDate = new DateTime?(nullable.Value);
            nullable = node.FinishDate;
            startFinishDates.FinishDate = new DateTime?(nullable.Value);
            return startFinishDates;
          }
        }
      }
      throw new ChartsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ChartsResources.IterationNotFound, (object) iteration));
    }

    private Stream Render(Chart chart)
    {
      try
      {
        MemoryStream imageStream = new MemoryStream(2048);
        chart.SaveImage((Stream) imageStream, ChartImageFormat.Png);
        imageStream.Seek(0L, SeekOrigin.Begin);
        return (Stream) imageStream;
      }
      catch (OverflowException ex)
      {
        foreach (Series series in (Collection<Series>) chart.Series)
          series.Points.Clear();
        MemoryStream imageStream = new MemoryStream(2048);
        chart.SaveImage((Stream) imageStream, ChartImageFormat.Png);
        imageStream.Seek(0L, SeekOrigin.Begin);
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (BoardsChartService), (Exception) ex);
        return (Stream) imageStream;
      }
    }

    private Stream Render(Bitmap bitmap)
    {
      MemoryStream memoryStream = new MemoryStream(2048);
      bitmap.Save((Stream) memoryStream, ImageFormat.Png);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return (Stream) memoryStream;
    }

    private Color CreateColor(Color color, int step)
    {
      ArgumentUtility.CheckForOutOfRange(step, nameof (step), 0, (int) byte.MaxValue);
      return Color.FromArgb((int) byte.MaxValue, (int) (byte) ((double) ((int) Color.White.R - (int) color.R) * ((double) ((int) byte.MaxValue - step) / (double) byte.MaxValue) + (double) color.R), (int) (byte) ((double) ((int) Color.White.G - (int) color.G) * ((double) ((int) byte.MaxValue - step) / (double) byte.MaxValue) + (double) color.G), (int) (byte) ((double) ((int) Color.White.B - (int) color.B) * ((double) ((int) byte.MaxValue - step) / (double) byte.MaxValue) + (double) color.B));
    }

    private static int GetMinimumDoneCount(
      IEnumerable<CumulativeFlowDiagramDataPoint> chartData,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs)
    {
      if (cumulativeFlowDiagramInputs.UseKanbanColumns)
      {
        Guid doneColumnId = cumulativeFlowDiagramInputs.BoardColumnRevisions.First<BoardColumnRevision>((Func<BoardColumnRevision, bool>) (b =>
        {
          if (b.Deleted || b.ColumnType != BoardColumnType.Outgoing)
            return false;
          DateTime? revisedDate = b.RevisedDate;
          DateTime futureDateTimeValue = SharedVariables.FutureDateTimeValue;
          if (!revisedDate.HasValue)
            return false;
          return !revisedDate.HasValue || revisedDate.GetValueOrDefault() == futureDateTimeValue;
        })).Id.Value;
        return chartData.Min<CumulativeFlowDiagramDataPoint>((Func<CumulativeFlowDiagramDataPoint, int>) (data => !data.Counts.ContainsKey(doneColumnId) ? 0 : data.Counts[doneColumnId]));
      }
      string doneStateName = cumulativeFlowDiagramInputs.DoneStateName;
      return chartData.Min<CumulativeFlowDiagramDataPoint>((Func<CumulativeFlowDiagramDataPoint, int>) (data => data.StateCounts[doneStateName]));
    }

    private IDictionary<string, Color> CreateColorMap(
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs)
    {
      IDictionary<string, Color> colorMap = (IDictionary<string, Color>) new Dictionary<string, Color>();
      int maxValue1 = (int) byte.MaxValue;
      int maxValue2 = (int) byte.MaxValue;
      int maxValue3 = (int) byte.MaxValue;
      int count = cumulativeFlowDiagramInputs.WorkItemStates.Count;
      int stateCount1 = this.GetStateCount(cumulativeFlowDiagramInputs.StateColumnTypeMap, BoardColumnType.Incoming);
      int stateCount2 = this.GetStateCount(cumulativeFlowDiagramInputs.StateColumnTypeMap, BoardColumnType.Outgoing);
      int num1 = count - stateCount1 - stateCount2;
      int num2 = (int) byte.MaxValue / Math.Max(1, stateCount1 + 1);
      int num3 = (int) byte.MaxValue / Math.Max(1, stateCount2 + 1);
      int num4 = (int) byte.MaxValue / Math.Max(1, num1 + 1);
      for (int index = count - 1; index >= 0; --index)
      {
        string workItemState = cumulativeFlowDiagramInputs.WorkItemStates[index];
        BoardColumnType boardColumnType;
        cumulativeFlowDiagramInputs.StateColumnTypeMap.TryGetValue(workItemState, out boardColumnType);
        switch (boardColumnType)
        {
          case BoardColumnType.Incoming:
            colorMap[workItemState] = this.CreateColor(ChartAreaColors.CumulativeFlowNew, maxValue1);
            maxValue1 -= num2;
            break;
          case BoardColumnType.Outgoing:
            colorMap[workItemState] = this.CreateColor(ChartAreaColors.CumulativeFlowDone, maxValue3);
            maxValue3 -= num3;
            break;
          default:
            colorMap[workItemState] = this.CreateColor(ChartAreaColors.CumulativeFlowInProgress, maxValue2);
            maxValue2 -= num4;
            break;
        }
      }
      return colorMap;
    }

    private int GetStateCount(IDictionary<string, BoardColumnType> map, BoardColumnType columnType) => map.Values.Count<BoardColumnType>((Func<BoardColumnType, bool>) (t => t == columnType));

    internal IChartDataComponent ChartDataComponent
    {
      get
      {
        if (this.m_chartDataComponent == null)
          this.m_chartDataComponent = (IChartDataComponent) new Microsoft.Azure.Boards.Charts.ChartDataComponent();
        return this.m_chartDataComponent;
      }
      set => this.m_chartDataComponent = value;
    }

    private class DayRange
    {
      public DayOfWeek StartDay { get; set; }

      public int Days { get; set; }
    }
  }
}
