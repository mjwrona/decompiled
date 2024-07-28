// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.HtmlHelperExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Plugins.Common.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public static class HtmlHelperExtensions
  {
    public static MvcHtmlString TeamField(
      this HtmlHelper htmlHelper,
      object teamFieldData,
      string containerClass)
    {
      TagBuilder tagBuilder1 = new TagBuilder("div");
      tagBuilder1.AddCssClass(containerClass);
      TagBuilder tagBuilder2 = new TagBuilder("div");
      tagBuilder2.AddCssClass("data-content");
      tagBuilder2.InnerHtml = htmlHelper.JsonIsland(teamFieldData).ToString();
      tagBuilder1.InnerHtml = tagBuilder2.ToString();
      return MvcHtmlString.Create(tagBuilder1.ToString(TagRenderMode.Normal));
    }

    public static MvcHtmlString FieldControl(this HtmlHelper htmlHelper, object fieldData) => htmlHelper.BasicControl(fieldData, "field-control");

    public static void NewBacklogLevelVisibilityNotSetMessage(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.NewBacklogLevelVisibilityNotSetMessage"))
      {
        TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
        if (tfsWebContext.Team == null)
          throw new TeamNotFoundInUrlException();
        if (tfsWebContext.TfsRequestContext.IsStakeholder())
          return;
        IAgileSettings teamSettings = tfsWebContext.GetTeamAgileSettings(tfsWebContext.Team.Id);
        if (teamSettings.TeamSettings.DefaultedBacklogVisibilities.Count <= 0 || !teamSettings.TeamSettings.DefaultedBacklogVisibilities.Any<string>((Func<string, bool>) (id =>
        {
          BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
          return teamSettings.BacklogConfiguration.TryGetBacklogLevelConfiguration(id, out backlogLevel) && backlogLevel.Custom && !teamSettings.BacklogConfiguration.IsBacklogVisible(id);
        })))
          return;
        string str = HtmlHelperExtensions.BuildLink(AgileViewResources.NewBacklogLevelVisibilityNotSetNotificationLinkText, (string) null);
        htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
        {
          new NotificationMessageModel("F6070FC4-EF8E-48B8-B0E5-1E9A99979981", MessageAreaType.Info, string.Format(AgileViewResources.NewBacklogLevelVisibilityNotSetNotificationMessage, (object) str))
        }, "tfs-new-backlog-level-visibility-not-set-notification", true, true, true, true);
      }
    }

    public static void BasicUserMessage(this HtmlHelper htmlHelper)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (tfsWebContext.TfsRequestContext.IsStakeholder() || tfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.AdvancedPortfolioBacklogManagementId))
        return;
      string str = HtmlHelperExtensions.BuildLink(AgileViewResources.BasicUserLimitedPortfolioAccessNotificationLinkText, AgileViewResources.BasicUserLimitedPortfolioAccessNotificationLink);
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("16676E24-8F6A-4EAA-9A9D-FCDF57E8BC8A", MessageAreaType.Info, AgileViewResources.BasicUserLimitedPortfolioAccessNotificationMessage + " " + str)
      }, "tfs-basic-user-limited-portfolio-access-notification", true, true, true, true);
    }

    public static void MissingProposedStateInBugsWarning(this HtmlHelper htmlHelper)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      IAgileSettings teamAgileSettings = tfsWebContext.GetTeamAgileSettings(tfsWebContext.Team.Id);
      ProjectProcessConfiguration processConfiguration = tfsWebContext.TfsRequestContext.GetProjectProcessConfiguration(tfsWebContext.Project.Uri);
      if (teamAgileSettings.TeamSettings.BugsBehavior != BugsBehavior.AsRequirements)
        return;
      string str = processConfiguration.BugWorkItems == null || string.IsNullOrEmpty(processConfiguration.BugWorkItems.PluralName) ? AgileProductBacklogServerResources.Backlog_Bugs : processConfiguration.BugWorkItems.PluralName;
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("02ADFCA6-1633-483A-972D-E0192AB07A0D", MessageAreaType.Info, string.Format(AgileProductBacklogServerResources.MissingProposedStateMappingForBugsWarning, (object) str) + " " + AgileProductBacklogServerResources.BugsOnBacklogProposedStateMappingDocLink)
      }, "tfs-missing-proposed-state-mapping-for-bugs-notification", true, true, true, true);
    }

    private static string BuildLink(string text, string uri)
    {
      TagBuilder tagBuilder = new TagBuilder("a");
      if (uri == null)
      {
        tagBuilder.MergeAttribute("role", "button");
        tagBuilder.MergeAttribute("tabindex", "0");
      }
      else
      {
        tagBuilder.MergeAttribute("href", uri);
        tagBuilder.MergeAttribute("target", "_blank");
      }
      tagBuilder.Text(text);
      return tagBuilder.ToString();
    }

    public static MvcHtmlString CapacityInfo(this HtmlHelper htmlHelper)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      object data = htmlHelper.ViewData["teamCapacity"];
      MvcHtmlString mvcHtmlString1 = htmlHelper.JsonIsland(data, (object) new
      {
        @class = "team-capacity-data"
      });
      MvcHtmlString mvcHtmlString2 = htmlHelper.CapacityMetadata();
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.InnerHtml += mvcHtmlString1.ToString();
      tagBuilder.InnerHtml += mvcHtmlString2.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString CapacityMetadata(this HtmlHelper htmlHelper)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      JsObject data1 = (JsObject) htmlHelper.ViewData["aggregated-capacity-data"];
      JsObject data2 = (JsObject) htmlHelper.ViewData["capacity-options"];
      MvcHtmlString mvcHtmlString1 = htmlHelper.JsonIsland((object) data1, (object) new
      {
        @class = "aggregated-capacity-data"
      });
      MvcHtmlString mvcHtmlString2 = htmlHelper.JsonIsland((object) data2, (object) new
      {
        @class = "capacity-options"
      });
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.InnerHtml += mvcHtmlString1.ToString();
      tagBuilder.InnerHtml += mvcHtmlString2.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString CapacityMetadata(
      this HtmlHelper htmlHelper,
      AggregatedCapacityDataViewModel aggregatedCapacityData,
      CapacityOptionsViewModel capacityOptions)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      ArgumentUtility.CheckForNull<AggregatedCapacityDataViewModel>(aggregatedCapacityData, nameof (aggregatedCapacityData));
      ArgumentUtility.CheckForNull<CapacityOptionsViewModel>(capacityOptions, nameof (capacityOptions));
      MvcHtmlString mvcHtmlString1 = htmlHelper.RestApiJsonIsland((object) aggregatedCapacityData, (object) new
      {
        @class = "aggregated-capacity-data"
      });
      MvcHtmlString mvcHtmlString2 = htmlHelper.RestApiJsonIsland((object) capacityOptions, (object) new
      {
        @class = "capacity-options"
      });
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.InnerHtml += mvcHtmlString1.ToString();
      tagBuilder.InnerHtml += mvcHtmlString2.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString CapacityPane(this HtmlHelper htmlHelper)
    {
      TagBuilder tagBuilder1 = new TagBuilder("div");
      tagBuilder1.AddCssClass("capacity-pane-container");
      TagBuilder tagBuilder2 = new TagBuilder("div");
      tagBuilder2.AddCssClass("team-capacity-control");
      tagBuilder1.InnerHtml += tagBuilder2.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder3 = new TagBuilder("div");
      tagBuilder3.AddCssClass("activity-grouped-progress-control");
      tagBuilder1.InnerHtml += tagBuilder3.ToString(TagRenderMode.Normal);
      TagBuilder tagBuilder4 = new TagBuilder("div");
      tagBuilder4.AddCssClass("assigned-to-grouped-progress-control");
      tagBuilder1.InnerHtml += tagBuilder4.ToString(TagRenderMode.Normal);
      return MvcHtmlString.Create(tagBuilder1.ToString(TagRenderMode.Normal));
    }

    public static MenuItem GetCommonSettingsConfigMenuItem(this HtmlHelper htmlHelper)
    {
      htmlHelper.ViewContext.TfsWebContext();
      return new MenuItem()
      {
        CommandId = "cmd-common-settings",
        Icon = "bowtie-icon bowtie-settings-gear"
      };
    }

    public static MvcHtmlString BurnDownChart(this HtmlHelper htmlHelper) => HtmlHelperExtensions.Chart(htmlHelper, "BurnDownChartData", "burndown-chart");

    public static MvcHtmlString BurnDownChart(
      this HtmlHelper htmlHelper,
      BurndownChartOptionsViewModel viewModel)
    {
      return HtmlHelperExtensions.Chart(htmlHelper, (object) viewModel, "burndown-chart");
    }

    public static MvcHtmlString VelocityChart(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.VelocityChart"))
        return HtmlHelperExtensions.Chart(htmlHelper, "VelocityChartData", "velocity-chart");
    }

    public static MvcHtmlString CumulativeFlowChart(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.CumulativeFlowChart"))
        return HtmlHelperExtensions.Chart(htmlHelper, "CumulativeFlowChartData", "cumulative-flow-chart");
    }

    public static MvcHtmlString SprintDatesControl(
      this HtmlHelper htmlHelper,
      SprintInformation sprint)
    {
      IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
      SprintDatesOptionsViewModel sprintDatesOptions = TeamIterationsUtils.CreateSprintDatesOptions(sprint, tfsRequestContext, tfsRequestContext.EnsureWebTeamContext().Team.Id);
      return htmlHelper.SprintDatesControl(sprintDatesOptions);
    }

    public static MvcHtmlString SprintDatesControl(
      this HtmlHelper htmlHelper,
      SprintDatesOptionsViewModel sprintDatesOptions)
    {
      MvcHtmlString mvcHtmlString = htmlHelper.DataContractJsonIsland<SprintDatesOptionsViewModel>(sprintDatesOptions, (object) new
      {
        @class = "options"
      });
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddCssClass("sprint-dates-working-days");
      tagBuilder.InnerHtml = mvcHtmlString.ToHtmlString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static MvcHtmlString SprintDatesControl(this HtmlHelper htmlHelper)
    {
      SprintInformation sprint = (SprintInformation) htmlHelper.ViewData["SprintInformation"];
      return htmlHelper.SprintDatesControl(sprint);
    }

    public static MvcHtmlString AgileContext(this HtmlHelper htmlHelper, SprintInformation sprint)
    {
      var data = sprint != null ? new
      {
        name = sprint.Name,
        path = sprint.IterationPath,
        id = sprint.IterationId.ToString("D"),
        start = sprint.StartDate,
        finish = sprint.FinishDate
      } : null;
      return htmlHelper.JsonIsland((object) new
      {
        iteration = data
      }, (object) new{ @class = "agile-context" });
    }

    public static MvcHtmlString BacklogContext(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Agile.Server.BacklogContext backlogContext)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BacklogContext"))
      {
        JsObject jsObject = new JsObject();
        jsObject["levelName"] = (object) backlogContext.CurrentLevelConfiguration.Name;
        jsObject["includeParents"] = (object) backlogContext.IncludeParents;
        jsObject["showInProgress"] = (object) backlogContext.ShowInProgress;
        jsObject["portfolios"] = (object) backlogContext.Portfolios;
        jsObject["backlogLevel?.Id"] = backlogContext.CurrentLevelConfiguration != null ? (object) backlogContext.CurrentLevelConfiguration.Id : (object) (string) null;
        jsObject["updateUrlActionAndParameter"] = (object) backlogContext.UpdateUrlActionAndParameter;
        jsObject["actionNameFromMru"] = (object) backlogContext.ActionNameFromMru;
        jsObject["team"] = (object) backlogContext.Team.ToJson();
        return htmlHelper.JsonIsland((object) new
        {
          backlogContext = jsObject
        }, (object) new{ @class = "backlog-context" });
      }
    }

    public static MvcHtmlString BoardModel(this HtmlHelper htmlHelper, Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BoardModel boardModel)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BoardModel"))
      {
        TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
        IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
        IAgileSettings teamAgileSettings;
        using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BoardModel.GetTeamAgileSettings"))
          teamAgileSettings = tfsWebContext.GetTeamAgileSettings(tfsWebContext.Team.Id);
        bool isBoardSettingsValid = true;
        using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BoardModel.BoardSettingsIsValid"))
          isBoardSettingsValid = boardModel.BoardSettings.IsValid(tfsRequestContext, teamAgileSettings.Process, teamAgileSettings.ProjectName, tfsWebContext.Team, teamAgileSettings.TeamSettings);
        MvcHtmlString mvcHtmlString;
        using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BoardModel.GetPayload"))
          mvcHtmlString = htmlHelper.JsonIsland((object) new
          {
            boardModel = boardModel.ToJson(tfsRequestContext, isBoardSettingsValid)
          }, (object) new{ @class = "backlog-board-model" });
        return mvcHtmlString;
      }
    }

    public static MvcHtmlString BugsBehaviorUIState(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.BugsBehaviorUIState"))
      {
        TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
        return htmlHelper.JsonIsland((object) new
        {
          bugsBehaviorState = TeamSettingsControlHelpers.GetBugBehaviorState(tfsWebContext.TfsRequestContext, tfsWebContext.Project)
        }, (object) new{ @class = "bugs-behavior-ui-state" });
      }
    }

    public static MvcHtmlString SprintTitle(this HtmlHelper htmlHelper)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      string str = (string) htmlHelper.ViewData["sprintName"];
      TagBuilder tagBuilder1 = new TagBuilder("div");
      tagBuilder1.AddCssClass("chart-page-title");
      TagBuilder tagBuilder2 = new TagBuilder("div");
      tagBuilder2.AddCssClass("sprint-title");
      tagBuilder2.AddCssClass("ellipsis");
      tagBuilder2.Attributes["title"] = str;
      tagBuilder2.InnerHtml = str;
      tagBuilder1.InnerHtml = tagBuilder2.ToString();
      return MvcHtmlString.Create(tagBuilder1.ToString());
    }

    public static MvcHtmlString SprintViewControl(this HtmlHelper htmlHelper, string cssClass)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      string str1 = (string) htmlHelper.ViewData["iteration-selected"];
      string str2 = (string) htmlHelper.ViewData["action-name"];
      HtmlHelper htmlHelper1 = htmlHelper;
      SprintViewViewModel viewModel = new SprintViewViewModel();
      viewModel.ActionName = str2;
      viewModel.SelectedIteration = str1;
      string cssClass1 = cssClass;
      return htmlHelper1.SprintViewControl(viewModel, cssClass1);
    }

    public static MvcHtmlString SprintViewControl(
      this HtmlHelper htmlHelper,
      SprintViewViewModel viewModel,
      string cssClass)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      ArgumentUtility.CheckStringForNullOrEmpty(viewModel.SelectedIteration, "selectedIteration");
      ArgumentUtility.CheckStringForNullOrEmpty(viewModel.ActionName, "actionName");
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.MergeAttribute("class", cssClass);
      tagBuilder.InnerHtml += htmlHelper.DataContractJsonIsland<SprintViewViewModel>(viewModel, (object) new
      {
        @class = "sprint-data"
      }).ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString BacklogViewControl(
      this HtmlHelper htmlHelper,
      string cssClass,
      string actionName = "backlog")
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.MergeAttribute("class", cssClass);
      tagBuilder.InnerHtml += htmlHelper.JsonIsland((object) new
      {
        actionName = actionName
      }, (object) new{ @class = "backlog-data" }).ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString CreateForecastFilter(this HtmlHelper htmlHelper, string selection) => htmlHelper.PivotFilter(AgileViewResources.ProductBacklog_PivotControl_ShowForecast, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
    {
      new PivotFilterItem(AgileProductBacklogServerResources.Backlog_ON, (object) "on")
      {
        Selected = selection == "on"
      },
      new PivotFilterItem(AgileProductBacklogServerResources.Backlog_OFF, (object) "off")
      {
        Selected = selection == "off"
      }
    }, (object) new{ @class = "show-forecast-filter" });

    public static MvcHtmlString TeamSettingsControlOptions(
      this HtmlHelper htmlHelper,
      TeamViewModel model)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      return htmlHelper.JsonIsland((object) new
      {
        processSettings = model.ProcessSettings.ToJson(tfsWebContext.TfsRequestContext, tfsWebContext.Project.Name),
        bugsBehaviorState = TeamSettingsControlHelpers.GetBugBehaviorState(tfsWebContext.TfsRequestContext, tfsWebContext.Project)
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString TeamSettingsData(this HtmlHelper htmlHelper, TeamViewModel model)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      TeamWITSettingsModel data = new TeamWITSettingsModel(tfsWebContext.TfsRequestContext, tfsWebContext.Team, model.ProcessSettings, model.Settings, tfsWebContext.TfsRequestContext.GetCollectionTimeZone());
      return htmlHelper.DataContractJsonIsland<TeamWITSettingsModel>(data, (object) new
      {
        @class = "options"
      }, true);
    }

    public static MvcHtmlString TeamSettingsData(this HtmlHelper htmlHelper)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "HtmlHelperExtensions.TeamSettingsData"))
      {
        TeamWITSettingsModel witSettingsModel = (TeamWITSettingsModel) htmlHelper.ViewData[nameof (TeamSettingsData)];
        ArgumentUtility.CheckForNull<TeamWITSettingsModel>(witSettingsModel, "teamSettings");
        return htmlHelper.TeamSettingsData(witSettingsModel);
      }
    }

    public static MvcHtmlString AddPanelSettings(
      this HtmlHelper htmlHelper,
      AddPanelViewModel addPanelSettings)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      ArgumentUtility.CheckForNull<AddPanelViewModel>(addPanelSettings, nameof (addPanelSettings));
      return htmlHelper.DataContractJsonIsland<AddPanelViewModel>(addPanelSettings, (object) new
      {
        @class = "add-panel-settings"
      });
    }

    public static MvcHtmlString DashboardTile(
      this HtmlHelper htmlHelper,
      object model,
      string controlClassName)
    {
      return htmlHelper.DashboardTile(model, (Func<MvcHtmlString>) null, controlClassName);
    }

    public static MvcHtmlString DashboardTile(
      this HtmlHelper htmlHelper,
      object model,
      Func<MvcHtmlString> tileBuilderAction)
    {
      return htmlHelper.DashboardTile(model, tileBuilderAction, (string) null);
    }

    public static MvcHtmlString DashboardTile(
      this HtmlHelper htmlHelper,
      object model,
      Func<MvcHtmlString> tileBuilderAction,
      string controlClassName)
    {
      ArgumentUtility.CheckForNull<HtmlHelper>(htmlHelper, nameof (htmlHelper));
      SettingsErrorModel settingsErrorModel = model as SettingsErrorModel;
      TagBuilder tagBuilder1 = new TagBuilder("div");
      if (settingsErrorModel != null)
      {
        TagBuilder tagBuilder2 = new TagBuilder("div");
        tagBuilder2.MergeAttribute("class", "error-text");
        tagBuilder2.Text(settingsErrorModel.ErrorText);
        tagBuilder1.InnerHtml += tagBuilder2.ToString();
        if (!string.IsNullOrEmpty(settingsErrorModel.LinkTarget))
        {
          TagBuilder tagBuilder3 = new TagBuilder("a");
          tagBuilder3.MergeAttribute("href", settingsErrorModel.LinkTarget);
          tagBuilder3.MergeAttribute("target", "_blank");
          tagBuilder3.Text(settingsErrorModel.LinkText);
          tagBuilder1.InnerHtml += tagBuilder3.ToString();
        }
      }
      else if (tileBuilderAction != null)
      {
        MvcHtmlString mvcHtmlString = tileBuilderAction();
        if (mvcHtmlString != null)
          return mvcHtmlString;
      }
      else
      {
        TagBuilder tagBuilder4 = new TagBuilder("div");
        if (!string.IsNullOrEmpty(controlClassName))
          tagBuilder4.MergeAttribute("class", controlClassName);
        if (model != null)
          tagBuilder4.InnerHtml = JsonIslandHtmlExtensions.JsonIsland(htmlHelper, model, (IDictionary<string, object>) null).ToString();
        tagBuilder1.InnerHtml += tagBuilder4.ToString();
      }
      return MvcHtmlString.Create(tagBuilder1.ToString());
    }

    public static void AgilePortfolioManagementNotification(
      this HtmlHelper htmlHelper,
      AgilePortfolioManagementNotificationViewModel notificationViewModel)
    {
      if (notificationViewModel == null)
        return;
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        notificationViewModel.Message
      }, notificationViewModel.ClassName, (notificationViewModel.Closeable ? 1 : 0) != 0);
    }

    public static MvcHtmlString WorkPivotViews(this HtmlHelper htmlHelper, AdminWorkModel model)
    {
      UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.TfsWebContext().RequestContext);
      List<PivotView> views = new List<PivotView>()
      {
        new PivotView(AgileServerResources.AdminWorkHub_PivotView_General)
        {
          Id = "general",
          Link = urlHelper.FragmentAction("general")
        },
        new PivotView(AgileServerResources.AdminWorkHub_PivotView_Iterations)
        {
          Id = "iterations",
          Link = urlHelper.FragmentAction("iterations")
        }
      };
      if (model.IsTeamFieldAreaPath)
        views.Add(new PivotView(AgileServerResources.AdminWorkHub_PivotView_Areas)
        {
          Id = "areas",
          Link = urlHelper.FragmentAction("areas")
        });
      else
        views.Add(new PivotView(AgileServerResources.AdminWorkHub_PivotView_TeamField)
        {
          Id = "team-field",
          Link = urlHelper.FragmentAction("team-field")
        });
      views.Add(new PivotView(AgileServerResources.AdminWorkHub_PivotView_Templates)
      {
        Id = "templates",
        Link = urlHelper.FragmentAction("templates")
      });
      return htmlHelper.PivotViews((IEnumerable<PivotView>) views, (object) new
      {
        @class = "work-admin-pivot"
      });
    }

    public static MvcHtmlString WorkProjectPivotViews(this HtmlHelper htmlHelper)
    {
      UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.TfsWebContext().RequestContext);
      List<PivotView> views = new List<PivotView>()
      {
        new PivotView(AgileServerResources.AdminWorkHub_PivotView_Iterations)
        {
          Id = "iterations",
          Link = urlHelper.FragmentAction("iterations")
        },
        new PivotView(AgileServerResources.AdminWorkHub_PivotView_Areas)
        {
          Id = "areas",
          Link = urlHelper.FragmentAction("areas")
        }
      };
      return htmlHelper.PivotViews((IEnumerable<PivotView>) views, (object) new
      {
        @class = "project-admin-work-pivot"
      });
    }

    public static MvcHtmlString BasicControl(
      this HtmlHelper htmlHelper,
      object payloadData,
      string className,
      object payloadHtmlAttributes = null)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddCssClass(className);
      tagBuilder.InnerHtml = !HtmlHelperExtensions.IsDataContactSerializable(payloadData) ? htmlHelper.JsonIsland(payloadData, payloadHtmlAttributes).ToString() : htmlHelper.DataContractJsonIsland<object>(payloadData, payloadHtmlAttributes).ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    private static MvcHtmlString Chart(HtmlHelper htmlHelper, string dataId, string className) => htmlHelper.ViewData.ContainsKey(dataId) ? HtmlHelperExtensions.Chart(htmlHelper, htmlHelper.ViewData[dataId], className) : MvcHtmlString.Empty;

    private static MvcHtmlString Chart(HtmlHelper htmlHelper, object data, string className)
    {
      if (data == null)
        return MvcHtmlString.Empty;
      TagBuilder tagBuilder = new TagBuilder("div");
      tagBuilder.AddCssClass("small-chart-container");
      tagBuilder.AddCssClass(className);
      tagBuilder.InnerHtml = !HtmlHelperExtensions.IsDataContactSerializable(data) ? htmlHelper.JsonIsland(data, (object) new
      {
        @class = "options"
      }).ToString() : htmlHelper.DataContractJsonIsland<object>(data, (object) new
      {
        @class = "options"
      }).ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static MvcHtmlString IterationPivotFilters(
      this HtmlHelper htmlHelper,
      IterationPivot selectedPivot)
    {
      UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.TfsWebContext().RequestContext);
      SprintInformation sprintInformation = (SprintInformation) htmlHelper.ViewData["SprintInformation"];
      List<PivotView> views = new List<PivotView>()
      {
        new PivotView(AgileViewResources.Backlogs_PivotView_Backlog)
        {
          Id = "backlog",
          Link = urlHelper.ActionWithParameters("iteration", "backlogs", (object) new
          {
            parameters = sprintInformation.IterationPath
          }),
          Selected = selectedPivot == IterationPivot.Backlog
        },
        new PivotView(AgileViewResources.Boards_PivotView)
        {
          Id = "board",
          Link = urlHelper.ActionWithParameters("taskboard", "backlogs", (object) new
          {
            parameters = sprintInformation.IterationPath
          }),
          Selected = selectedPivot == IterationPivot.Board
        }
      };
      if (htmlHelper.isAdvanceBacklogManagementFeatureAvailable())
        views.Add(new PivotView(AgileViewResources.SprintPlanning_CapacityFilterTitle)
        {
          Id = "capacity",
          Link = urlHelper.ActionWithParameters("capacity", "backlogs", (object) new
          {
            parameters = sprintInformation.IterationPath
          }),
          Selected = selectedPivot == IterationPivot.Capacity
        });
      return htmlHelper.PivotViews((IEnumerable<PivotView>) views, (object) new
      {
        @class = "sprintplanning-view-tabs"
      }, "ms.vss-work-web.iteration-backlog-tabs", false);
    }

    public static bool isAdvanceBacklogManagementFeatureAvailable(this HtmlHelper htmlHelper)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      return ProjectBacklogPermissionUtils.HasAdvanceBacklogManagementPermission(tfsWebContext.TfsRequestContext) || tfsWebContext.FeatureContext.GetFeatureMode(LicenseFeatures.AdvancedBacklogManagementId) >= FeatureMode.Trial;
    }

    private static bool IsDataContactSerializable(object o) => HtmlHelperExtensions.HasCustomAttribute(o, typeof (DataContractAttribute));

    private static bool HasCustomAttribute(object o, Type attributeType) => o != null && o.GetType().GetCustomAttributes(attributeType, true).Length != 0;
  }
}
