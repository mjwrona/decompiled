// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogViewModelProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Backlog;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Plugins.Common.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class BacklogViewModelProvider : IBacklogViewModelProvider
  {
    public BacklogViewModel GetBacklogViewModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      BacklogContext backlogContext,
      IEnumerable<string> additionalFields = null,
      bool? forecastVisibleOverride = null,
      bool? showInProgress = null,
      bool? showCompletedChildItems = null,
      IAgileSettings agileSettings = null,
      SortedIterationSubscriptions sortedTeamIterationNodes = null,
      IDictionary<string, bool> teamAutomationRulesStates = null)
    {
      using (requestContext.TraceBlock(290032, 290033, "Agile", TfsTraceLayers.Controller, nameof (GetBacklogViewModel)))
      {
        if (agileSettings != null || AgileUtils.TryGetAgileSettings(requestContext, project, team, out agileSettings))
        {
          ChartHelper chartHelper = new ChartHelper(requestContext, agileSettings, team, sortedTeamIterationNodes);
          SprintViewViewModel sprintViewViewModel = new SprintViewViewModel()
          {
            SelectedIteration = agileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext),
            ActionName = "Iteration"
          };
          ColumnPreferences columnPreferences = this.GetColumnPreferences(requestContext, team, backlogContext);
          ProductBacklogQueryResults backlogResultsModel = this.GetProductBacklogResultsModel(requestContext, team, backlogContext, agileSettings, columnPreferences, additionalFields, showInProgress, showCompletedChildItems);
          IReadOnlyCollection<string> workItemTypes = backlogContext.CurrentLevelConfiguration.WorkItemTypes;
          IEnumerable<string> workItemStates = (IEnumerable<string>) agileSettings.BacklogConfiguration.GetWorkItemStates(backlogContext.CurrentLevelConfiguration, new WorkItemStateCategory[2]
          {
            WorkItemStateCategory.InProgress,
            WorkItemStateCategory.Resolved
          });
          VelocityChartViewModel output1;
          AgileUtils.TryExecute<VelocityChartViewModel>((Func<VelocityChartViewModel>) (() => chartHelper.GetVelocityChartData(project.Id, 5)), TfsTraceLayers.Controller, out output1);
          CumulativeFlowDiagramViewModel output2;
          AgileUtils.TryExecute<CumulativeFlowDiagramViewModel>((Func<CumulativeFlowDiagramViewModel>) (() => chartHelper.GetCumulativeFlowChartData(backlogContext.CurrentLevelConfiguration)), TfsTraceLayers.Controller, out output2);
          BacklogViewModel backlogViewModel = new BacklogViewModel();
          backlogViewModel.BacklogContext = backlogContext;
          backlogViewModel.BacklogContextWorkItemTypeNames = workItemTypes.ToArray<string>();
          backlogViewModel.QueryResults = backlogResultsModel;
          backlogViewModel.AddPanelSettings = this.GetAddPanelSettings(requestContext, project, backlogContext);
          backlogViewModel.PluralName = AgileViewResources.Backlogs_PivotView_Backlog;
          backlogViewModel.SprintView = sprintViewViewModel;
          backlogViewModel.MappingPanel = this.GetMappingPanelSettings(agileSettings, backlogContext);
          backlogViewModel.IsRootBacklog = backlogContext.CurrentLevelConfiguration.Rank == agileSettings.BacklogConfiguration.GetAllBacklogLevels().OrderByDescending<BacklogLevelConfiguration, int>((System.Func<BacklogLevelConfiguration, int>) (bl => bl.Rank)).FirstOrDefault<BacklogLevelConfiguration>().Rank;
          backlogViewModel.IsRequirementBacklog = backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog;
          backlogViewModel.InProgressStates = string.Join(", ", workItemStates);
          backlogViewModel.PageTitle = backlogContext.CurrentLevelConfiguration == null ? backlogContext.CurrentLevelConfiguration.Name : AgileViewResources.ProductBacklog_Details_MainContentTitle;
          backlogViewModel.VelocityChartSettings = output1;
          backlogViewModel.CumulativeFlowDiagramSettings = output2;
          backlogViewModel.AgilePortfolioManagementNotificationSettings = this.GetAgilePortfolioManagementNotificationSettings(requestContext, backlogContext, project);
          backlogViewModel.ColumnPreferences = columnPreferences;
          backlogViewModel.AutomationRulesStates = teamAutomationRulesStates;
          BacklogViewModel viewModel = backlogViewModel;
          if (backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog && !backlogContext.IncludeParents)
            viewModel.ForecastSettings = this.GetForecastSettings(requestContext, team, agileSettings, (IEnumerable<int>) backlogResultsModel.OwnedIds, forecastVisibleOverride);
          this.RecordBacklogViewModelTelemetry(requestContext, agileSettings, backlogContext, viewModel, project.Id, team.Id);
          return viewModel;
        }
        requestContext.Trace(290781, TraceLevel.Error, "Agile", TfsTraceLayers.Controller, "Agile settings unable to be retrieved");
        return (BacklogViewModel) null;
      }
    }

    public EffortDataViewModel GetEffortDataViewModel(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      string name = settings.Process.EffortField.Name;
      IEnumerable<IDataRecord> workItemData = this.GetWorkItemData(requestContext, workItemIds, (IEnumerable<string>) new string[4]
      {
        CoreFieldReferenceNames.Id,
        name,
        CoreFieldReferenceNames.State,
        CoreFieldReferenceNames.WorkItemType
      });
      List<int> intList = new List<int>();
      List<object> objectList = new List<object>();
      BacklogLevelConfiguration requirementBacklog = settings.BacklogConfiguration.RequirementBacklog;
      HashSet<string> stringSet1 = new HashSet<string>((IEnumerable<string>) requirementBacklog.WorkItemTypes, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      HashSet<string> stringSet2 = new HashSet<string>((IEnumerable<string>) settings.BacklogConfiguration.GetWorkItemStates(requirementBacklog, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.Proposed
      }), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (IDataRecord dataRecord in workItemData)
      {
        string str1 = dataRecord.GetString(2);
        string str2 = dataRecord.GetString(3);
        if (stringSet1.Contains(str2) && stringSet2.Contains(str1))
        {
          int int32 = dataRecord.GetInt32(0);
          object obj = dataRecord.IsDBNull(1) ? (object) 0 : dataRecord.GetValue(1);
          intList.Add(int32);
          objectList.Add(obj);
        }
      }
      return new EffortDataViewModel()
      {
        Ids = intList.ToArray(),
        Efforts = objectList.ToArray(),
        EffortFieldName = name
      };
    }

    private IEnumerable<IDataRecord> GetWorkItemData(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> queryFields)
    {
      return requestContext.GetService<WebAccessWorkItemService>().GetWorkItems(requestContext, ids, queryFields);
    }

    private void RecordBacklogViewModelTelemetry(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogContext backlogContext,
      BacklogViewModel viewModel,
      Guid projectId,
      Guid teamId)
    {
      try
      {
        ILicenseType[] licensesForUser = requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().GetLicensesForUser(requestContext, requestContext.UserContext);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(AgileCustomerIntelligencePropertyName.Licenses, (object) ((IEnumerable<ILicenseType>) licensesForUser).Select<ILicenseType, string>((System.Func<ILicenseType, string>) (license => license.Name)));
        ForecastLinesViewModel forecastSettings = viewModel.ForecastSettings;
        properties.Add(AgileCustomerIntelligencePropertyName.Forecast, forecastSettings != null ? forecastSettings.VisibleState : string.Empty);
        properties.Add(AgileCustomerIntelligencePropertyName.ViewType, backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog ? "Product Backlog" : "Portfolio Backlog");
        properties.Add(AgileCustomerIntelligencePropertyName.BacklogLevelName, viewModel.BacklogContext.CurrentLevelConfiguration.Id);
        properties.Add(AgileCustomerIntelligencePropertyName.InProgressFilterState, viewModel.BacklogContext.ShowInProgress);
        properties.Add(AgileCustomerIntelligencePropertyName.NumberOfWorkItems, (double) viewModel.QueryResults.TargetIds.Count<int>());
        properties.Add(AgileCustomerIntelligencePropertyName.ColumnsCount, (double) viewModel.QueryResults.PageColumns.Count<string>());
        properties.Add(AgileCustomerIntelligencePropertyName.MappingPanelState, viewModel.MappingPanelFilterState);
        properties.Add(AgileCustomerIntelligencePropertyName.TeamId, teamId.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.BugBehavior, settings.TeamSettings.BugsBehavior.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.IncludeParents, viewModel.BacklogContext.IncludeParents);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.Pageload, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "Agile", TfsTraceLayers.Controller, ex);
      }
    }

    private ForecastLinesViewModel GetForecastSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      IAgileSettings settings,
      IEnumerable<int> workItemIds,
      bool? forecastVisibleOverride = null)
    {
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      EffortDataViewModel effortDataViewModel = (EffortDataViewModel) null;
      SettingsUtilities settingsUtilities = new SettingsUtilities();
      bool flag;
      string visibleState;
      if (forecastVisibleOverride.HasValue)
      {
        flag = forecastVisibleOverride.GetValueOrDefault();
        visibleState = flag ? "on" : "off";
      }
      else
      {
        visibleState = settingsUtilities.GetTeamUserSettingValue<string>(requestContext, team, "AgileBacklog.ShowForecastFilter", "off");
        flag = this.IsForecastVisible(visibleState);
      }
      if (flag || !requestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.ForecastOnDemand"))
        effortDataViewModel = this.GetEffortDataViewModel(requestContext, settings, workItemIds);
      return new ForecastLinesViewModel()
      {
        EffortData = effortDataViewModel,
        Velocity = settingsUtilities.GetTeamUserSettingValue<int>(requestContext, team, "AgileBacklog.SprintForecastVelocity", 10),
        VisibleState = visibleState
      };
    }

    private bool IsForecastVisible(string visibleState) => string.Equals(visibleState, "on", StringComparison.InvariantCultureIgnoreCase);

    private AgilePortfolioManagementNotificationViewModel GetAgilePortfolioManagementNotificationSettings(
      IVssRequestContext requestContext,
      BacklogContext backlogContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project)
    {
      if (!requestContext.FeatureContext().IsFeatureAvailable(LicenseFeatures.PortfolioBacklogManagementId))
        return (AgilePortfolioManagementNotificationViewModel) null;
      if (backlogContext == null || backlogContext.Portfolios == null || backlogContext.Portfolios.Length > 1)
        return (AgilePortfolioManagementNotificationViewModel) null;
      bool flag1 = false;
      using (WebProjectSettingsHive projectSettingsHive = new WebProjectSettingsHive(requestContext, project.Id))
        flag1 = projectSettingsHive.ReadSetting<bool>("/notification/dismiss/049DCC09-665A-4160-895A-06690983B3A4", false);
      if (flag1)
        return (AgilePortfolioManagementNotificationViewModel) null;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) project.Id, (object) "049DCC09-665A-4160-895A-06690983B3A4");
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(requestContext))
        flag1 = userSettingsHive.ReadSetting<bool>("/notification/dismiss/" + str, false);
      if (flag1)
        return (AgilePortfolioManagementNotificationViewModel) null;
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity foundationIdentity = service.ReadIdentity(requestContext, IdentitySearchFactor.AdministratorsGroup, project.Uri);
      string header;
      string id;
      bool flag2;
      WebSettingsScope scope;
      if (service.IsMember(requestContext, foundationIdentity.Descriptor, requestContext.GetUserIdentity().Descriptor))
      {
        header = string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileViewResources.AgilePortfolioManagementAdminNotification, (object) "");
        id = "049DCC09-665A-4160-895A-06690983B3A4";
        flag2 = false;
        scope = WebSettingsScope.Project;
      }
      else
      {
        id = str;
        header = AgileViewResources.AgilePortfolioManagementNotification;
        flag2 = true;
        scope = WebSettingsScope.User;
      }
      return new AgilePortfolioManagementNotificationViewModel()
      {
        ClassName = "agile-portfolio-management-notification",
        Closeable = flag2,
        Message = new NotificationMessageModel(id, MessageAreaType.Info, header, scope: scope)
      };
    }

    private MappingPanelViewModel GetMappingPanelSettings(
      IAgileSettings agileSettings,
      BacklogContext backlogContext)
    {
      BacklogLevelConfiguration parentBacklogLevel = (BacklogLevelConfiguration) null;
      string str = string.Empty;
      if (agileSettings.BacklogConfiguration.TryGetBacklogParent(backlogContext.CurrentLevelConfiguration.Id, out parentBacklogLevel))
        str = parentBacklogLevel.Name;
      return new MappingPanelViewModel()
      {
        BacklogDisplayName = str
      };
    }

    internal AddPanelViewModel GetAddPanelSettings(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      BacklogContext backlogContext)
    {
      using (requestContext.TraceBlock(290034, 290035, "Agile", TfsTraceLayers.Controller, nameof (GetAddPanelSettings)))
      {
        string toolSpecificId = LinkingUtilities.DecodeUri(project.Uri).ToolSpecificId;
        List<string> stringList = new List<string>();
        IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
        foreach (string addPanelField in backlogContext.CurrentLevelConfiguration.AddPanelFields)
        {
          FieldEntry field;
          if (fieldDictionary.TryGetField(addPanelField, out field))
            stringList.Add(field.ReferenceName);
        }
        return new AddPanelViewModel()
        {
          ProjectId = toolSpecificId,
          TeamId = backlogContext.Team.Id,
          WorkItemTypes = backlogContext.CurrentLevelConfiguration.WorkItemTypes.ToArray<string>(),
          DefaultWorkItemType = backlogContext.CurrentLevelConfiguration.DefaultWorkItemType,
          FieldRefNames = stringList.ToArray()
        };
      }
    }

    private ColumnPreferences GetColumnPreferences(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogContext backlogContext)
    {
      TeamUserSettings teamUserSettings = new TeamUserSettings(requestContext, team);
      ColumnPreferences columnPreferences = this._CreateColumnPreferences(requestContext, backlogContext, (IUserSettings) teamUserSettings);
      columnPreferences.EnsureColumnIsPresent("System.Title", 400);
      ColumnPreferences.EnsureValid(requestContext, columnPreferences, requestContext.WitContext().FieldDictionary);
      return columnPreferences;
    }

    private ProductBacklogQueryResults GetProductBacklogResultsModel(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogContext backlogContext,
      IAgileSettings settings,
      ColumnPreferences columnPreferences,
      IEnumerable<string> additionalFields = null,
      bool? showInProgress = null,
      bool? showCompletedChildItems = null)
    {
      return this.GetProductBacklogResultsModelWithColumns(requestContext, team, backlogContext, settings, columnPreferences, additionalFields, showInProgress, showCompletedChildItems);
    }

    private ProductBacklogQueryResults GetProductBacklogResultsModelWithColumns(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogContext backlogContext,
      IAgileSettings settings,
      ColumnPreferences columnPreferences,
      IEnumerable<string> additionalFields = null,
      bool? showInProgress = null,
      bool? showCompletedChildItems = null)
    {
      using (requestContext.TraceBlock(290036, 290037, "Agile", TfsTraceLayers.Controller, "GetProductBacklogResultsModel"))
      {
        IEnumerable<string> strings = columnPreferences.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column, bool>) (column => !column.NotAField && !column.Rollup)).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column, string>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column, string>) (column => column.FieldName));
        if (additionalFields != null)
          strings = strings.Union<string>(additionalFields);
        if (!showInProgress.HasValue)
          showInProgress = new bool?(this.GetSettingValue(requestContext, team, "AgileBacklog.ShowInProgressFilter"));
        if (!showCompletedChildItems.HasValue)
          showCompletedChildItems = new bool?(this.GetSettingValue(requestContext, team, "Agile/BacklogsHub/CompletedChildItemsFilter"));
        WorkItemStateCategory[] queryStates = QueryUtils.GetQueryStates(showInProgress.Value);
        bool requirementsBacklog = backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog;
        ProductBacklogGridOptions options = new ProductBacklogGridOptions()
        {
          EnableReorder = this.IsAdvancedBacklogFeaturesEnabled(requestContext, backlogContext),
          EnableReparent = true,
          ShowOrderColumn = !backlogContext.IncludeParents,
          EnableForecast = requirementsBacklog && !backlogContext.IncludeParents,
          ColumnOptionsKey = columnPreferences.PreferencesKey
        };
        return new ProductBacklog(requestContext, settings, backlogContext, strings, (IEnumerable<WorkItemStateCategory>) queryStates, true, showCompletedChildItems.Value).GetResults(requestContext, columnPreferences.GetColumnWidthMap(), options, settings.Process.TeamField.Name);
      }
    }

    private bool GetSettingValue(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string setting)
    {
      return requestContext.GetService<ISettingsService>().GetValue<bool>(requestContext, SettingsUserScope.User, "WebTeam", team.Id.ToString(), setting, true);
    }

    private bool IsAdvancedBacklogFeaturesEnabled(
      IVssRequestContext requestContext,
      BacklogContext backlogContext)
    {
      bool requirementsBacklog = backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog;
      return ProjectBacklogPermissionUtils.HasAdvanceBacklogManagementPermission(requestContext) || requestContext.FeatureContext().IsFeatureAvailable(requirementsBacklog ? LicenseFeatures.AdvancedBacklogManagementId : LicenseFeatures.AdvancedPortfolioBacklogManagementId);
    }

    private ColumnPreferences _CreateColumnPreferences(
      IVssRequestContext requestContext,
      BacklogContext backlogContext,
      IUserSettings userSettings)
    {
      return new ColumnPreferences(requestContext, backlogContext.CurrentLevelConfiguration, userSettings, backlogContext.CurrentLevelConfiguration.IsRequirementsBacklog ? ColumnPreferenceScope.ProductBacklog : ColumnPreferenceScope.PortfolioBacklog);
    }
  }
}
