// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardChartsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "charts")]
  public class BoardChartsApiController : BoardsApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.ChartDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.InvalidArgumentValueException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.ChartUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateChartException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateChartException>(HttpStatusCode.Forbidden);
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartDoesNotExistException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.ChartDoesNotExistException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateChartException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateChartException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.InvalidArgumentValueException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartUpdateFailureException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.ChartUpdateFailureException));
    }

    [HttpGet]
    public IEnumerable<BoardChartReference> GetBoardCharts(string board)
    {
      this.GetBoardIdFromNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      return (IEnumerable<BoardChartReference>) new List<BoardChartReference>()
      {
        new BoardChartReference()
        {
          Name = "cumulativeFlow",
          Url = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardChartsLocationId, this.ProjectId, this.TeamId, (object) new
          {
            board = board,
            name = "cumulativeFlow"
          })
        }
      };
    }

    [HttpGet]
    public BoardChart GetBoardChart(string board, string name)
    {
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      if (StringComparer.InvariantCultureIgnoreCase.Equals(name, "cumulativeFlow"))
        return this.GetCumulativeFlowChart(board, levelIdByNameOrId);
      throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartDoesNotExistException();
    }

    private BoardChart GetCumulativeFlowChart(string board, string boardCategoryReferenceName)
    {
      ArgumentUtility.CheckForEmptyGuid(this.TeamId, "TeamId");
      Dictionary<string, object> settings = new Dictionary<string, object>();
      IDictionary<string, CumulativeFlowDiagramSettings> flowDiagramSettings1 = this.AgileSettings.TeamSettings.CumulativeFlowDiagramSettings;
      CumulativeFlowDiagramSettings flowDiagramSettings2;
      if (flowDiagramSettings1 == null || !flowDiagramSettings1.TryGetValue(boardCategoryReferenceName, out flowDiagramSettings2))
      {
        settings["startDate"] = (object) null;
        settings["hideIncomingColumn"] = (object) false;
        settings["hideOutgoingColumn"] = (object) false;
      }
      else
      {
        settings["startDate"] = (object) flowDiagramSettings2.StartDate;
        settings["hideIncomingColumn"] = (object) flowDiagramSettings2.HideIncoming;
        settings["hideOutgoingColumn"] = (object) flowDiagramSettings2.HideOutgoing;
      }
      return this.CreateBoardChartModel(board, "cumulativeFlow", (IDictionary<string, object>) settings);
    }

    [HttpPatch]
    public BoardChart UpdateBoardChart(string board, string name, BoardChart chart)
    {
      ArgumentUtility.CheckForNull<BoardChart>(chart, "BoardChartsApiController.chart");
      this.CheckBacklogManagementLicense();
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      this.GetOrCreateBoardSettings(levelIdByNameOrId);
      if (!StringComparer.InvariantCultureIgnoreCase.Equals(name, "cumulativeFlow"))
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartDoesNotExistException();
      this.UpdateCumulativeFlowChart(board, levelIdByNameOrId, chart.Settings);
      BoardChart cumulativeFlowChart = this.GetCumulativeFlowChart(board, levelIdByNameOrId);
      this.VerifyDateUpdated(chart.Settings, cumulativeFlowChart.Settings);
      return cumulativeFlowChart;
    }

    private void VerifyDateUpdated(
      IDictionary<string, object> originalSettings,
      IDictionary<string, object> updatedSettings)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(originalSettings, nameof (originalSettings));
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(updatedSettings, nameof (originalSettings));
      DateTime result;
      if (originalSettings["startDate"] == null || !(originalSettings["startDate"].ToString().Trim() != string.Empty) || !DateTime.TryParse(originalSettings["startDate"].ToString().Trim(), out result))
        return;
      result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
      object updatedSetting = updatedSettings["startDate"];
      if (result.Equals(updatedSetting) || this.TfsRequestContext == null)
        return;
      this.TfsRequestContext.TraceException(103201, this.TraceArea, "UpdateBoardChart", new Exception(string.Format("ExpectedDate: {0}, ActualDate: {1}", (object) result, updatedSetting)));
    }

    [NonAction]
    public IDictionary<string, object> UpdateCumulativeFlowChart(
      string board,
      string boardCategoryReferenceName,
      IDictionary<string, object> settings)
    {
      ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "Team");
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(settings, nameof (settings));
      if (!this.Team.UserIsTeamAdmin(this.TfsRequestContext))
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateChartException();
      IDictionary<string, object> updatedSettings = (IDictionary<string, object>) new Dictionary<string, object>();
      this.UpdateCumulativeFlowChartInternal(board, boardCategoryReferenceName, settings, updatedSettings);
      return updatedSettings;
    }

    private void UpdateCumulativeFlowChartInternal(
      string board,
      string boardCategoryReferenceName,
      IDictionary<string, object> settings,
      IDictionary<string, object> updatedSettings)
    {
      bool flag = false;
      IDictionary<string, CumulativeFlowDiagramSettings> cfdSettings = this.AgileSettings.TeamSettings.CumulativeFlowDiagramSettings ?? (IDictionary<string, CumulativeFlowDiagramSettings>) new Dictionary<string, CumulativeFlowDiagramSettings>();
      CumulativeFlowDiagramSettings flowDiagramSettings;
      if (!cfdSettings.TryGetValue(boardCategoryReferenceName, out flowDiagramSettings))
      {
        flowDiagramSettings = new CumulativeFlowDiagramSettings();
        cfdSettings[boardCategoryReferenceName] = flowDiagramSettings;
      }
      foreach (string key in (IEnumerable<string>) settings.Keys)
      {
        object setting = settings[key];
        if (StringComparer.InvariantCultureIgnoreCase.Equals(key, "startDate"))
        {
          if (setting != null && setting.ToString().Trim() != string.Empty)
          {
            DateTime result;
            if (!DateTime.TryParse(setting.ToString().Trim(), out result))
              throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.InvalidDateMessage);
            if (DateTime.Today.Date < result.Date)
              throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.CumulativeFlowSettingsFutureDateMessage);
            flowDiagramSettings.StartDate = new DateTime?(DateTime.SpecifyKind(result, DateTimeKind.Utc));
          }
          else
            flowDiagramSettings.StartDate = new DateTime?();
          flag = true;
          updatedSettings["startDate"] = (object) flowDiagramSettings.StartDate.ToString();
        }
        else if (StringComparer.InvariantCultureIgnoreCase.Equals(key, "hideIncomingColumn"))
        {
          if (setting != null && setting.ToString().Trim() != string.Empty)
          {
            bool result;
            if (!bool.TryParse(setting.ToString().Trim(), out result))
              throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.GeneralInvalidDataMessage);
            flowDiagramSettings.HideIncoming = result;
          }
          else
            flowDiagramSettings.HideIncoming = false;
          flag = true;
          updatedSettings["hideIncomingColumn"] = (object) flowDiagramSettings.HideIncoming.ToString();
        }
        else if (StringComparer.InvariantCultureIgnoreCase.Equals(key, "hideOutgoingColumn"))
        {
          if (setting != null && setting.ToString().Trim() != string.Empty)
          {
            bool result;
            if (!bool.TryParse(setting.ToString().Trim(), out result))
              throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.GeneralInvalidDataMessage);
            flowDiagramSettings.HideOutgoing = result;
          }
          else
            flowDiagramSettings.HideOutgoing = false;
          flag = true;
          updatedSettings["hideOutgoingColumn"] = (object) flowDiagramSettings.HideOutgoing.ToString();
        }
      }
      if (!flag)
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidArgumentValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.CumulativeFlowSettingsInvalidValue);
      try
      {
        this.TfsRequestContext.GetService<ITeamConfigurationService>().SetCumulativeFlowDiagramSettings(this.TfsRequestContext, this.Team, cfdSettings);
      }
      catch (Exception ex)
      {
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.ChartUpdateFailureException(ex);
      }
    }

    private BoardChart CreateBoardChartModel(
      string board,
      string name,
      IDictionary<string, object> settings)
    {
      string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardChartsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = board,
        name = name
      });
      BoardChart boardChartModel = new BoardChart();
      boardChartModel.Name = name;
      boardChartModel.Url = resourceUriString;
      boardChartModel.Settings = settings;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid guid = this.ProjectId;
      string projectId = guid.ToString();
      guid = this.TeamId;
      string teamId = guid.ToString();
      string board1 = board;
      string selfUrl = resourceUriString;
      boardChartModel.Links = this.GetReferenceLinks(tfsRequestContext, projectId, teamId, board1, selfUrl);
      return boardChartModel;
    }

    private ReferenceLinks GetReferenceLinks(
      IVssRequestContext tfsRequestContext,
      string projectId,
      string teamId,
      string board,
      string selfUrl)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", selfUrl);
      referenceLinks.AddLink(nameof (board), AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        id = board
      }));
      return referenceLinks;
    }
  }
}
