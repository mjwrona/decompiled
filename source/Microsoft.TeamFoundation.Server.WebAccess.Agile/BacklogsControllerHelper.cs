// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsControllerHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public abstract class BacklogsControllerHelper
  {
    protected const string c_actionNameBoard = "Board";
    protected const string c_actionNameBoardEmbedded = "BoardEmbedded";
    protected const string c_actionNameCapacity = "Capacity";
    protected const string c_actionNameTaskBoard = "TaskBoard";
    protected const string c_actionNameIterationContributions = "IterationContributions";
    internal const int c_defaultTitleColumnWidth = 400;
    private CapacityModel m_capacityModel;
    private bool? m_showInProgressFilterState;
    private bool? m_isEmbedded;
    private AgileTracingUtils m_tracingUtils = new AgileTracingUtils();

    public virtual BacklogsController Controller { get; private set; }

    protected BacklogsControllerHelper()
    {
    }

    public BacklogsControllerHelper(BacklogsController backlogsController)
    {
      ArgumentUtility.CheckForNull<BacklogsController>(backlogsController, nameof (backlogsController));
      this.Controller = backlogsController;
      this.ChartHelper = new ChartHelper(backlogsController.TfsRequestContext, backlogsController.Settings, backlogsController.Team);
      this.ControlDataHelper = new ControlDataHelper(backlogsController);
      this.TeamMruHelper = new TeamMru();
    }

    public virtual ChartHelper ChartHelper { get; private set; }

    public virtual ControlDataHelper ControlDataHelper { get; private set; }

    public virtual TeamMru TeamMruHelper { get; private set; }

    public virtual IVssRequestContext TfsRequestContext => this.Controller.TfsRequestContext;

    public virtual TfsWebContext TfsWebContext => this.Controller.TfsWebContext;

    public virtual IAgileSettings Settings => this.Controller.Settings;

    public virtual bool IsEmbedded
    {
      get
      {
        if (!this.m_isEmbedded.HasValue)
        {
          bool result;
          bool.TryParse(this.TfsWebContext.RequestContext.HttpContext.Request.QueryString["embedded"], out result);
          this.m_isEmbedded = new bool?(result);
        }
        return this.m_isEmbedded.Value;
      }
    }

    internal CapacityModel CapacityModel
    {
      get
      {
        if (this.m_capacityModel == null)
          this.m_capacityModel = new CapacityModel(this.TfsWebContext, this.Controller.GetRequestIterationNode());
        return this.m_capacityModel;
      }
    }

    public IFieldTypeDictionary GetWorkItemFields() => this.TfsRequestContext.WitContext().FieldDictionary;

    protected void SetupCapacityViewData(TeamWITSettingsModel teamWitSettingsModel, bool preFill = false) => this.m_tracingUtils.TraceAction(this.TfsRequestContext, 210141, 210149, (Action) (() =>
    {
      this.Controller.ViewData["teamCapacity"] = (object) this.CapacityModel.GetCapacityControlData(preFill);
      this.Controller.ViewData["userHasWriteAccess"] = (object) this.CapacityModel.UserHasWriteAccess;
      this.SetupAggregatedCapacityAndOptions(teamWitSettingsModel);
    }), nameof (SetupCapacityViewData));

    protected void SetupAggregatedCapacityAndOptions(TeamWITSettingsModel teamWitSettingsModel)
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290779, 290780, memberName: nameof (SetupAggregatedCapacityAndOptions)))
      {
        Guid cssNodeId = this.Controller.GetRequestIterationNode().CssNodeId;
        this.Controller.ViewData["capacity-options"] = (object) this.CapacityModel.GetCapacityOptionsViewDataJSObject(BacklogsControllerHelper.IsFirstIteration(teamWitSettingsModel, cssNodeId));
        this.Controller.ViewData["aggregated-capacity-data"] = (object) this.CapacityModel.GetAggregatedCapacityViewDataJSObject();
      }
    }

    public static bool IsFirstIteration(
      TeamWITSettingsModel teamWitSettingsModel,
      Guid requestIterationId)
    {
      bool flag = false;
      Guid guid = requestIterationId;
      if (teamWitSettingsModel.PreviousIterations != null && teamWitSettingsModel.PreviousIterations.Length != 0)
        guid = teamWitSettingsModel.PreviousIterations[0].Id;
      else if (teamWitSettingsModel.CurrentIteration != null)
        guid = teamWitSettingsModel.CurrentIteration.Id;
      else if (teamWitSettingsModel.FutureIterations != null && teamWitSettingsModel.FutureIterations.Length != 0)
        guid = teamWitSettingsModel.FutureIterations[0].Id;
      if (requestIterationId == guid)
        flag = true;
      return flag;
    }

    public virtual MappingPanelViewModel GetMappingPanelSettings()
    {
      BacklogContext requestBacklogContext = this.Controller.RequestBacklogContext;
      string str = string.Empty;
      BacklogLevelConfiguration parentBacklogLevel;
      if (this.Settings.BacklogConfiguration.TryGetBacklogParent(requestBacklogContext.CurrentLevelConfiguration.Id, out parentBacklogLevel))
        str = parentBacklogLevel.Name;
      return new MappingPanelViewModel()
      {
        BacklogDisplayName = str
      };
    }

    public virtual bool GetInProgressFilterState()
    {
      if (!this.m_showInProgressFilterState.HasValue)
        this.m_showInProgressFilterState = new bool?(!string.Equals(this.Controller.GetTeamUserSettingValue<string>(this.GetInProgressRegKey(), "on"), "off", StringComparison.OrdinalIgnoreCase));
      return this.m_showInProgressFilterState.Value;
    }

    private string GetInProgressRegKey() => "AgileBacklog.ShowInProgressFilter";

    protected virtual bool ProjectHasMoreThanDefaultTeam() => this.TfsWebContext.GetProjectTeams().Count<WebApiTeam>() > 1;

    public IEnumerable<MruTeamInfo> GetMruTeams()
    {
      IEnumerable<MruTeamInfo> source = this.TeamMruHelper.GetMruTeams(this.TfsRequestContext, this.TfsWebContext.CurrentProjectUri.ToString(), this.Controller.Team).Select<WebApiTeam, MruTeamInfo>((Func<WebApiTeam, MruTeamInfo>) (team => new MruTeamInfo()
      {
        TeamId = team.Id,
        TeamName = team.Name
      }));
      if (source.Any<MruTeamInfo>())
        return source;
      return (IEnumerable<MruTeamInfo>) new MruTeamInfo[1]
      {
        new MruTeamInfo()
        {
          TeamId = this.Controller.Team.Id,
          TeamName = this.Controller.Team.Name
        }
      };
    }

    protected internal ViewResult View(string viewName) => this.View(viewName, (object) null);

    protected internal ViewResult View(object model) => this.View((string) null, model);

    protected internal virtual ViewResult View(string viewName, object model)
    {
      if (model != null)
        this.Controller.ViewData.Model = model;
      ViewResult viewResult = new ViewResult();
      viewResult.ViewName = viewName;
      viewResult.MasterName = (string) null;
      viewResult.ViewData = this.Controller.ViewData;
      viewResult.TempData = this.Controller.TempData;
      viewResult.ViewEngineCollection = this.Controller.ViewEngineCollection;
      return viewResult;
    }

    protected JsObject GetDefaultSuccessJsObject()
    {
      JsObject defaultSuccessJsObject = new JsObject();
      defaultSuccessJsObject["success"] = (object) true;
      return defaultSuccessJsObject;
    }

    protected void PrepareIterationTabContributionData()
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290589, 290590, memberName: nameof (PrepareIterationTabContributionData)))
      {
        this.Controller.ViewData["iterationBacklogOptions"] = (object) BacklogsIterationActionHelper.CreateIterationBacklogOptions(this.Settings.BacklogConfiguration.TaskBacklog);
        this.ControlDataHelper.AddSprintViewControlData(this.Controller.GetRequestIterationNode().GetPath(this.TfsRequestContext), "IterationContributions");
        this.ControlDataHelper.AddSprintName(this.Controller.Team.Name);
        this.ControlDataHelper.AddSprintInformation(this.TfsRequestContext);
        this.ControlDataHelper.AddRequirementBacklogContextData();
        this.ChartHelper.AddBurnDownChartData((IDictionary<string, object>) this.Controller.ViewData, this.Controller.GetRequestIterationNode());
        this.ControlDataHelper.AddTeamSettingsData();
        this.ControlDataHelper.AddIterationTabContributionModel();
      }
    }

    protected void PrepareBacklogTabContributionData(
      NameValueCollection queryParams,
      string selectedTabContributionId)
    {
      using (this.m_tracingUtils.TraceBlock(this.TfsRequestContext, 290591, 290592, TfsTraceLayers.Controller, nameof (PrepareBacklogTabContributionData)))
      {
        this.ControlDataHelper.AddTeamSettingsData();
        this.ControlDataHelper.AddRequestBacklogContextData();
        this.ControlDataHelper.AddSprintViewControlData(this.Settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext), "IterationContributions");
        this.ControlDataHelper.AddBacklogContributionModel(this.Controller.RequestBacklogContext.LevelName, queryParams, selectedTabContributionId);
      }
    }

    protected static string GetProcessTemplateName(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return TeamProjectUtil.GetProjectProcessTemplateDescriptor(requestContext, projectId)?.Name ?? string.Empty;
    }
  }
}
