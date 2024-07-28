// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ControlDataHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class ControlDataHelper
  {
    internal const IterationPivot DefaultIterationPivot = IterationPivot.Backlog;
    internal const string SelectedPivotQueryParamName = "spv";
    private BacklogsController m_controller;

    protected ControlDataHelper()
    {
    }

    public ControlDataHelper(BacklogsController controller) => this.m_controller = controller;

    public virtual SprintViewViewModel AddSprintViewControlData(
      string iterationPath,
      string actionName)
    {
      using (WebPerformanceTimer.StartMeasure(this.m_controller.TfsWebContext.RequestContext, "ControlDataHelper.AddSprintViewControlData"))
      {
        this.m_controller.ViewData["iteration-selected"] = (object) iterationPath;
        this.m_controller.ViewData["action-name"] = (object) actionName;
        return new SprintViewViewModel()
        {
          SelectedIteration = iterationPath,
          ActionName = actionName
        };
      }
    }

    public virtual TeamWITSettingsModel AddTeamSettingsData()
    {
      using (WebPerformanceTimer.StartMeasure(this.m_controller.TfsWebContext.RequestContext, "ControlDataHelper.AddTeamSettingsData"))
      {
        TeamWITSettingsModel witSettingsModel = new TeamWITSettingsModel(this.m_controller.TfsRequestContext, this.m_controller.Team, this.m_controller.Settings.Process, this.m_controller.Settings.TeamSettings, this.m_controller.TfsRequestContext.GetCollectionTimeZone());
        this.m_controller.ViewData["TeamSettingsData"] = (object) witSettingsModel;
        return witSettingsModel;
      }
    }

    public virtual void AddSprintName(string teamName)
    {
      TreeNode requestIterationNode = this.m_controller.GetRequestIterationNode();
      this.m_controller.ViewData["sprintName"] = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileViewResources.ProductBacklog_Details_MainContentTitle_Unfiltered, (object) teamName, (object) requestIterationNode.GetName(this.m_controller.TfsRequestContext));
    }

    public virtual void AddSprintInformation(IVssRequestContext requestContext)
    {
      TreeNode requestIterationNode = this.m_controller.GetRequestIterationNode();
      this.m_controller.ViewData["SprintInformation"] = (object) new SprintInformation(requestContext, requestIterationNode);
    }

    public virtual void AddSprintInformation(IterationBacklogViewModel viewModel) => this.m_controller.ViewData["SprintInformation"] = (object) viewModel.SprintInformation;

    public virtual void AddRequirementBacklogContextData() => this.m_controller.ViewData["BacklogContextInformation"] = (object) this.m_controller.GetRequirementsLevelBacklogContext();

    public virtual void AddRequestBacklogContextData() => this.m_controller.ViewData["BacklogContextInformation"] = (object) this.m_controller.RequestBacklogContext;

    public virtual void AddRequirementBacklogContextData(IterationBacklogViewModel viewModel) => this.m_controller.ViewData["BacklogContextInformation"] = (object) viewModel.BacklogContext;

    public virtual void AddIterationTabContributionModel(
      NameValueCollection queryParams = null,
      string selectedTabContributionId = null)
    {
      if (queryParams == null)
        queryParams = HttpContext.Current.Request.QueryString;
      IterationPivot iterationPivot = this.SetSelectedPivot(queryParams);
      JsObject initialConfig = new JsObject();
      initialConfig["iterationId"] = (object) this.m_controller.GetRequestIterationNode().CssNodeId;
      this.m_controller.ViewData["agile-tab-contributions-model"] = (object) new TabContributionsModel(selectedTabContributionId, initialConfig, new int?((int) iterationPivot));
    }

    public virtual void AddBacklogContributionModel(
      string level,
      NameValueCollection queryParams = null,
      string selectedTabContributionId = null)
    {
      if (queryParams == null)
        queryParams = HttpContext.Current.Request.QueryString;
      IterationPivot iterationPivot = this.SetSelectedPivot(queryParams);
      BacklogLevelConfiguration levelConfiguration = this.m_controller.RequestBacklogContext.CurrentLevelConfiguration;
      JsObject initialConfig = new JsObject();
      initialConfig[nameof (level)] = (object) level;
      initialConfig["workItemTypeNames"] = (object) levelConfiguration.WorkItemTypes;
      this.m_controller.ViewData["agile-tab-contributions-model"] = (object) new TabContributionsModel(selectedTabContributionId, initialConfig, new int?((int) iterationPivot));
    }

    internal static IterationPivot ParseSelectedPivotQueryParam(NameValueCollection queryParams)
    {
      string queryParam = queryParams?["spv"];
      int result;
      return !string.IsNullOrWhiteSpace(queryParam) && int.TryParse(queryParam, out result) && Enum.IsDefined(typeof (IterationPivot), (object) result) ? (IterationPivot) result : IterationPivot.Backlog;
    }

    private IterationPivot SetSelectedPivot(NameValueCollection queryParams)
    {
      IterationPivot selectedPivotQueryParam = ControlDataHelper.ParseSelectedPivotQueryParam(queryParams);
      this.m_controller.ViewData["tab-contribution-selected-pivot-filter"] = (object) selectedPivotQueryParam;
      return selectedPivotQueryParam;
    }
  }
}
