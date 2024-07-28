// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ChartHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Charts.Utility;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class ChartHelper
  {
    private IVssRequestContext m_requestContext;
    private IAgileSettings m_settings;
    private WebApiTeam _team;
    private SortedIterationSubscriptions m_sortedIterations;

    protected ChartHelper()
    {
    }

    public ChartHelper(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      WebApiTeam team,
      SortedIterationSubscriptions sortedIterations = null)
    {
      this.m_requestContext = requestContext;
      this.m_settings = settings;
      this._team = team;
      this.m_sortedIterations = sortedIterations;
    }

    public virtual void AddBurnDownChartData(
      IDictionary<string, object> data,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode)
    {
      using (this.m_requestContext.TraceBlock(290044, 290045, "Agile", TfsTraceLayers.Controller, nameof (AddBurnDownChartData)))
      {
        BurndownChartOptionsViewModel optionsViewModel = ChartHelper.CreateBurnDownChartOptionsViewModel(this.m_requestContext, iterationNode);
        data["BurnDownChartData"] = (object) optionsViewModel;
      }
    }

    public static BurndownChartOptionsViewModel CreateBurnDownChartOptionsViewModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode)
    {
      string path = iterationNode.GetPath(requestContext);
      string[] strArray = ChartUtils.ValidateBurnDownIterationDates(requestContext, iterationNode);
      if (strArray.Length != 0)
        requestContext.Trace(280004, TraceLevel.Warning, "Agile", nameof (ChartHelper), "Burndown Validation failed with : {0}", (object) string.Join(",", strArray));
      BurndownChartOptionsViewModel optionsViewModel = new BurndownChartOptionsViewModel();
      optionsViewModel.Title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileViewResources.BurnDownChart_Title, (object) iterationNode.GetName(requestContext));
      optionsViewModel.IterationPath = path;
      optionsViewModel.Errors = strArray.Length != 0 ? (IEnumerable<string>) strArray : (IEnumerable<string>) (string[]) null;
      return optionsViewModel;
    }

    public virtual object AddCumulativeFlowChartData(
      BacklogLevelConfiguration backlogLevel,
      IDictionary<string, object> data)
    {
      JsObject json = this.GetCumulativeFlowChartData(backlogLevel).ToJson();
      data["CumulativeFlowChartData"] = (object) json;
      return (object) json;
    }

    public virtual CumulativeFlowDiagramViewModel GetCumulativeFlowChartData(
      BacklogLevelConfiguration backlogLevel)
    {
      using (PerformanceTimer.StartMeasure(this.m_requestContext, "ChartHelper.GetCumulativeFlowChartData"))
      {
        using (this.m_requestContext.TraceBlock(290046, 290047, "Agile", TfsTraceLayers.Controller, nameof (GetCumulativeFlowChartData)))
        {
          ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, "backlogLevelConfiguration");
          CumulativeFlowDiagramSettings flowTeamSettings = ChartHelper.GetCumulativeFlowTeamSettings(this.m_requestContext, this._team, backlogLevel.Id);
          return new CumulativeFlowDiagramViewModel()
          {
            TeamId = this._team.Id,
            Title = AgileViewResources.CumulativeFlowDiagram_Title,
            Errors = new string[0],
            BacklogLevelId = backlogLevel.Id,
            StartDate = flowTeamSettings.StartDate.HasValue ? flowTeamSettings.StartDate.Value.ToString() : (string) null,
            HideIncoming = flowTeamSettings.HideIncoming,
            HideOutgoing = flowTeamSettings.HideOutgoing
          };
        }
      }
    }

    public virtual object AddVelocityChartData(
      Guid projectId,
      int iterationsNumber,
      IDictionary<string, object> data)
    {
      VelocityChartViewModel velocityChartData = this.GetVelocityChartData(projectId, iterationsNumber);
      if (velocityChartData != null)
        data["VelocityChartData"] = (object) velocityChartData.ToJson();
      return (object) velocityChartData;
    }

    public virtual VelocityChartViewModel GetVelocityChartData(Guid projectId, int iterationsNumber)
    {
      using (PerformanceTimer.StartMeasure(this.m_requestContext, "ChartHelper.GetVelocityChartData"))
      {
        using (this.m_requestContext.TraceBlock(290048, 290049, "Agile", TfsTraceLayers.Controller, nameof (GetVelocityChartData)))
        {
          VelocityChartViewModel velocityChartData = (VelocityChartViewModel) null;
          if (this.m_settings.TeamSettings.Iterations.Count<ITeamIteration>() > 0)
          {
            string[] strArray = ChartUtils.ValidateVelocityInputs(this.m_requestContext, projectId, this.m_settings, iterationsNumber, this.m_sortedIterations);
            if (strArray.Length != 0)
              this.m_requestContext.Trace(280005, TraceLevel.Warning, "Agile", nameof (ChartHelper), "Velocity Validation failed with : {0}", (object) string.Join(",", strArray));
            velocityChartData = new VelocityChartViewModel()
            {
              TeamId = this._team.Id,
              Title = AgileViewResources.VelocityChart_Title,
              IterationsNumber = iterationsNumber,
              Errors = strArray
            };
          }
          return velocityChartData;
        }
      }
    }

    public static string GetCumulativeFlowStartDateProperty(string backlogLevelId) => TeamConstants.CumulativeFlowDiagramStartDate + "." + backlogLevelId;

    public static CumulativeFlowDiagramSettings GetCumulativeFlowTeamSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string backlogLevelId)
    {
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false, true);
      CumulativeFlowDiagramSettings flowDiagramSettings;
      return teamSettings.CumulativeFlowDiagramSettings == null || !teamSettings.CumulativeFlowDiagramSettings.TryGetValue(backlogLevelId, out flowDiagramSettings) ? new CumulativeFlowDiagramSettings() : flowDiagramSettings;
    }
  }
}
