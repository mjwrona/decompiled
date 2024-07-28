// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsIterationActionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Plugins.Common.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class BacklogsIterationActionHelper : BacklogsControllerHelper
  {
    public BacklogsIterationActionHelper(BacklogsController backlogsController)
      : base(backlogsController)
    {
    }

    public ViewResult GetIterationView(string iterationName)
    {
      using (this.TfsRequestContext.TraceBlock(290042, 290043, "Agile", TfsTraceLayers.Controller, nameof (GetIterationView)))
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode = this.Controller.GetRequestIterationNode(iterationName);
        IterationBacklogViewModel backlogViewModel = this.GetIterationBacklogViewModel(this.TfsRequestContext, requestIterationNode);
        this.Controller.ViewData["iterationBacklogViewModel"] = (object) backlogViewModel;
        this.Controller.Trace(250003, TraceLevel.Verbose, "Obtained Iteration Backlog using Query {0}", (object) backlogViewModel.QueryResults.Wiql);
        this.ControlDataHelper.AddSprintViewControlData(requestIterationNode.GetPath(this.TfsRequestContext), "Iteration");
        this.ControlDataHelper.AddSprintName(this.Controller.Team.Name);
        this.ControlDataHelper.AddSprintInformation(backlogViewModel);
        this.ControlDataHelper.AddRequirementBacklogContextData(backlogViewModel);
        this.ControlDataHelper.AddIterationTabContributionModel();
        this.Controller.ViewData["AddPanelSettings"] = (object) this.GetAddPanelSettings();
        this.Controller.ViewData.AddSplitterClasses("RightHubSplitter", "no-split");
        this.ChartHelper.AddBurnDownChartData((IDictionary<string, object>) this.Controller.ViewData, requestIterationNode);
        this.RecordIterationViewModelTelemetry(backlogViewModel);
        return this.View("Iteration");
      }
    }

    public static IterationBacklogOptionsViewModel CreateIterationBacklogOptions(
      BacklogLevelConfiguration taskConfiguration)
    {
      IEnumerable<string> workItemTypes = (IEnumerable<string>) taskConfiguration.WorkItemTypes;
      return new IterationBacklogOptionsViewModel(taskConfiguration.WorkItemCountLimit, workItemTypes);
    }

    public JsonResult GetPayload(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iteration)
    {
      RestApiJsonResult payload = new RestApiJsonResult((object) this.GetIterationBacklogViewModel(this.TfsRequestContext, iteration));
      payload.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (JsonResult) payload;
    }

    public virtual IterationBacklogViewModel GetIterationBacklogViewModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iteration)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(iteration, nameof (iteration));
      IAgileSettings settings = this.Settings;
      return requestContext.TraceBlock<IterationBacklogViewModel>(290428, 290429, 290430, "Agile", TfsTraceLayers.Controller, nameof (GetIterationBacklogViewModel), (Func<IterationBacklogViewModel>) (() =>
      {
        SprintInformation sprint = new SprintInformation(requestContext, iteration);
        IterationBacklogOptionsViewModel iterationBacklogOptions = BacklogsIterationActionHelper.CreateIterationBacklogOptions(settings.BacklogConfiguration.TaskBacklog);
        TeamWITSettingsModel teamWitSettingsModel = new TeamWITSettingsModel(requestContext, this.Controller.Team, settings.Process, settings.TeamSettings, requestContext.GetCollectionTimeZone());
        Guid cssNodeId = iteration.CssNodeId;
        bool isFirstIteration = BacklogsControllerHelper.IsFirstIteration(teamWitSettingsModel, cssNodeId);
        ProductBacklogQueryResults backlogResultsModel = this.GetIterationBacklogResultsModel(this.SetupColumnPreferences(requestContext));
        return new IterationBacklogViewModel()
        {
          BacklogContext = this.Controller.GetRequirementsLevelBacklogContext(),
          SprintInformation = sprint,
          SprintDatesOptions = TeamIterationsUtils.CreateSprintDatesOptions(sprint, requestContext, teamWitSettingsModel.TeamId),
          BurndownChartOptions = ChartHelper.CreateBurnDownChartOptionsViewModel(requestContext, iteration),
          IterationBacklogOptions = iterationBacklogOptions,
          CapacityOptions = this.CapacityModel.GetCapacityOptionsViewData(isFirstIteration),
          AggregatedCapacityData = this.CapacityModel.GetAggregatedCapacityViewData(),
          TeamSettingsModel = teamWitSettingsModel,
          AddPanelViewModel = this.GetAddPanelSettings(),
          QueryResults = backlogResultsModel
        };
      }));
    }

    public virtual AddPanelViewModel GetAddPanelSettings()
    {
      string toolSpecificId = LinkingUtilities.DecodeUri(this.TfsWebContext.CurrentProjectUri.ToString()).ToolSpecificId;
      BacklogLevelConfiguration requirementBacklog = this.Settings.BacklogConfiguration.RequirementBacklog;
      IFieldTypeDictionary fieldDictionary = this.Controller.TfsRequestContext.WitContext().FieldDictionary;
      string[] addPanelFields = requirementBacklog.AddPanelFields;
      List<string> stringList = new List<string>();
      foreach (string name in addPanelFields)
      {
        FieldEntry field = fieldDictionary.GetField(name);
        if (field != null)
          stringList.Add(field.ReferenceName);
      }
      return new AddPanelViewModel()
      {
        ProjectId = toolSpecificId,
        TeamId = this.Controller.Team.Id,
        WorkItemTypes = requirementBacklog.WorkItemTypes.ToArray<string>(),
        DefaultWorkItemType = requirementBacklog.DefaultWorkItemType,
        FieldRefNames = stringList.ToArray()
      };
    }

    private void RecordIterationViewModelTelemetry(IterationBacklogViewModel viewModel)
    {
      try
      {
        this.TfsRequestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().GetLicensesForUser(this.TfsRequestContext, this.TfsRequestContext.UserContext);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(AgileCustomerIntelligencePropertyName.ViewType, "Iteration Backlog");
        properties.Add("sprintName", this.Controller.ViewData["sprintName"]);
        properties.Add("iteration-selected", this.Controller.ViewData["iteration-selected"]);
        properties.Add(AgileCustomerIntelligencePropertyName.TeamId, this.Controller.Team.Id.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.BugBehavior, this.Controller.BacklogActionHelper.Settings.TeamSettings.BugsBehavior.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.ProcessTemplateIndicator, BacklogsControllerHelper.GetProcessTemplateName(this.TfsRequestContext, this.TfsWebContext.Project.Id));
        properties.Add(AgileCustomerIntelligencePropertyName.NumberOfWorkItems, (double) viewModel.QueryResults.Payload.Rows.Count<object[]>());
        properties.Add(AgileCustomerIntelligencePropertyName.BacklogLevelName, viewModel.BacklogContext.CurrentLevelConfiguration.Name);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.Pageload, properties);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(599999, "Agile", TfsTraceLayers.Controller, ex);
      }
    }

    private ProductBacklogQueryResults GetIterationBacklogResultsModel(
      ColumnPreferences columnPreferences)
    {
      IDictionary<string, int> columnWidthMap = columnPreferences.GetColumnWidthMap();
      IEnumerable<string> strings = columnPreferences.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column, string>) (col => col.FieldName));
      this.CapacityModel.InitializeTaskBoardData(strings);
      bool flag = ProjectBacklogPermissionUtils.HasAdvanceBacklogManagementPermission(this.TfsRequestContext) || this.Controller.TfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.AdvancedBacklogManagementId);
      TaskBoardData taskboardData = this.CapacityModel.TaskboardData;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IDictionary<string, int> fieldWidthMap = columnWidthMap;
      ProductBacklogGridOptions options = new ProductBacklogGridOptions();
      options.EnableReorder = flag;
      options.EnableReparent = true;
      options.ShowOrderColumn = false;
      IDictionary defaultQueryContext = this.TfsRequestContext.GetDefaultQueryContext(this.TfsWebContext.Project.Name);
      ProductBacklogQueryResults queryResultModel = taskboardData.CreateQueryResultModel(tfsRequestContext, fieldWidthMap, options, defaultQueryContext);
      queryResultModel.Columns = strings.Join<string, QueryDisplayColumn, string, QueryDisplayColumn>(queryResultModel.Columns, (Func<string, string>) (f => f), (Func<QueryDisplayColumn, string>) (col => col.Name), (Func<string, QueryDisplayColumn, QueryDisplayColumn>) ((f, col) => col));
      queryResultModel.SortColumns = (IEnumerable<QuerySortColumn>) null;
      return queryResultModel;
    }

    private ColumnPreferences SetupColumnPreferences(IVssRequestContext requestContext)
    {
      IUserSettings userSettings = (IUserSettings) new TeamUserSettings(this.TfsRequestContext, this.Controller.Team);
      ColumnPreferences columnPreferences = new ColumnPreferences(requestContext, this.Settings.BacklogConfiguration.TaskBacklog, userSettings, ColumnPreferenceScope.SprintBacklog);
      columnPreferences.EnsureColumnIsPresent("System.Title", 400);
      ColumnPreferences.EnsureValid(requestContext, columnPreferences, this.GetWorkItemFields());
      return columnPreferences;
    }
  }
}
