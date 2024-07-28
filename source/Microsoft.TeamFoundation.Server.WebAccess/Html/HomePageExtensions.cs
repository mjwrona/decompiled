// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.HomePageExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class HomePageExtensions
  {
    private const string s_MeteringServiceBaseRegistryKey = "/Service/Commerce/Metering";
    private const string s_earlyAdopterCutOffDateKey = "/Service/Commerce/Metering/EarlyAdopterSelectDate";
    private const string s_earlyAdopterDurationKey = "/Service/Commerce/Metering/EarlyAdopterIncentiveDuration";

    public static void UpgradeToFullVersionMessage(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (UpgradeToFullVersionMessage));
      try
      {
        TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
        bool flag1 = tfsWebContext.IsFeatureAvailable("WebAccess.TrialModeNotification");
        bool flag2 = tfsWebContext.IsFeatureAvailable("WebAccess.EarlyAdopterNotification");
        if (!tfsWebContext.IsHosted || !(flag2 | flag1) || tfsWebContext.TfsRequestContext.IsStakeholder())
          return;
        DateTime accountCreationDate;
        AccountTrialModeModel trialModeModel;
        AccountTrialExtensions.GetAccountTrialDates(tfsWebContext, out accountCreationDate, out trialModeModel);
        IVssRequestContext vssRequestContext = tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        DateTime cutOffDate = service.GetValue<DateTime>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/EarlyAdopterSelectDate", new DateTime());
        TimeSpan duration = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/EarlyAdopterIncentiveDuration", new TimeSpan(142, 8, 0, 0));
        if (flag2 && accountCreationDate.ToUniversalTime() < cutOffDate && DateTime.UtcNow < cutOffDate.Add(duration))
        {
          HomePageExtensions.DisplayEarlyAdopterNotification(htmlHelper, cutOffDate, duration);
        }
        else
        {
          if (!flag1)
            return;
          HomePageExtensions.DisplayTrialNotification(htmlHelper, trialModeModel);
        }
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (UpgradeToFullVersionMessage));
      }
    }

    private static void DisplayTrialNotification(
      HtmlHelper htmlHelper,
      AccountTrialModeModel accountTrialModel)
    {
      if (!accountTrialModel.IsAccountInTrialMode)
        return;
      int daysLeftOnTrialMode = accountTrialModel.DaysLeftOnTrialMode;
      if (daysLeftOnTrialMode < 0)
        return;
      string str = daysLeftOnTrialMode >= 1 ? (daysLeftOnTrialMode != 1 ? string.Format(WACommonResources.TrialModeReminderMessage_MultipleDays, (object) daysLeftOnTrialMode) : string.Format(WACommonResources.TrialModeReminderMessage_OneDay, (object) daysLeftOnTrialMode)) : WACommonResources.TrialModeReminderMessage_Today;
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("A08B0A06-214E-4B51-9BF6-DC61F057070D", daysLeftOnTrialMode > 7 ? MessageAreaType.Info : MessageAreaType.Warning, string.Format(WACommonResources.TrialModeReminderMessage, (object) str, (object) WACommonResources.TrialFeatureUrl))
      }, "tfs-upgrade-notification", showIcon: true);
    }

    private static void DisplayEarlyAdopterNotification(
      HtmlHelper htmlHelper,
      DateTime cutOffDate,
      TimeSpan duration)
    {
      int days = (cutOffDate.Add(duration) - DateTime.UtcNow).Days;
      if (days < 0)
        return;
      string header = days >= 1 ? (days != 1 ? string.Format(WACommonResources.EarlyAdopterModeReminderMessage_MultipleDays, (object) days, (object) WACommonResources.EarlyAdopterAccountLearnMoreUrl) : string.Format(WACommonResources.EarlyAdopterModeReminderMessage_OneDay, (object) days, (object) WACommonResources.EarlyAdopterAccountLearnMoreUrl)) : string.Format(WACommonResources.EarlyAdopterModeReminderMessage_Today, (object) WACommonResources.EarlyAdopterAccountLearnMoreUrl);
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("0C935C95-9F39-405D-A51C-4B6FC5367BE0", days > 7 ? MessageAreaType.Info : MessageAreaType.Warning, header)
      }, "tfs-upgrade-notification", showIcon: true);
    }

    public static string TeamTitle(this HtmlHelper htmlHelper)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (tfsWebContext.Team != null)
      {
        bool? nullable = htmlHelper.ViewData["DefaultTeam"] as bool?;
        if (!nullable.HasValue || !nullable.Value)
          return string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TeamTitleFormat, (object) tfsWebContext.Project.Name, (object) tfsWebContext.Team.Name);
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.ProjectTitleFormat, (object) tfsWebContext.Project.Name);
    }

    public static MvcHtmlString TeamInformation(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (TeamInformation));
      try
      {
        JsObject data = new JsObject();
        TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
        List<JsObject> jsObjectList1 = new List<JsObject>();
        if (tfsWebContext.Team != null)
        {
          data["teamId"] = (object) tfsWebContext.Team.Id;
          data["teamName"] = (object) tfsWebContext.Team.Name;
          IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
          if (tfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.BacklogManagementId))
          {
            List<JsObject> jsObjectList2 = jsObjectList1;
            JsObject jsObject = new JsObject();
            jsObject.Add("text", (object) WACommonResources.ViewBacklog);
            jsObject.Add("url", (object) tfsWebContext.Url.Action("backlog", "backlogs", (object) new
            {
              routeArea = ""
            }));
            jsObject.Add("icon", (object) "editor-list-bullet");
            jsObjectList2.Add(jsObject);
          }
          if (tfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.AgileBoardsId))
          {
            List<JsObject> jsObjectList3 = jsObjectList1;
            JsObject jsObject = new JsObject();
            jsObject.Add("text", (object) WACommonResources.ViewTaskBoard);
            jsObject.Add("url", (object) tfsWebContext.Url.Action("taskboard", "backlogs", (object) new
            {
              routeArea = ""
            }));
            jsObject.Add("icon", (object) "window");
            jsObjectList3.Add(jsObject);
          }
        }
        List<JsObject> jsObjectList4 = jsObjectList1;
        JsObject jsObject1 = new JsObject();
        jsObject1.Add("text", (object) WACommonResources.ViewQueries);
        jsObject1.Add("url", (object) tfsWebContext.Url.Action("index", "workItems", (object) new
        {
          routeArea = ""
        }));
        jsObject1.Add("icon", (object) "tfs-query-flat");
        jsObjectList4.Add(jsObject1);
        if (tfsWebContext.FeatureContext.GetFeatureMode(LicenseFeatures.FeedbackId) >= FeatureMode.Advertising)
        {
          List<JsObject> jsObjectList5 = jsObjectList1;
          JsObject jsObject2 = new JsObject();
          jsObject2.Add("text", (object) WACommonResources.RequestFeedback);
          jsObject2.Add("url", (object) "#");
          jsObject2.Add("icon", (object) "tfs-microsoft-requirementcategory");
          jsObject2.Add("css", (object) "request-feedback");
          jsObjectList5.Add(jsObject2);
        }
        if (!tfsWebContext.IsHosted && tfsWebContext.CurrentProjectGuid != Guid.Empty)
        {
          TeamProjectSettings teamProjectSettings = TeamProjectSettings.GetTeamProjectSettings(tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Application), tfsWebContext.CurrentProjectGuid);
          if (teamProjectSettings != null)
          {
            if (teamProjectSettings.Portal != (Uri) null)
            {
              List<JsObject> jsObjectList6 = jsObjectList1;
              JsObject jsObject3 = new JsObject();
              jsObject3.Add("text", (object) WACommonResources.GoToProjectPortal);
              jsObject3.Add("url", (object) teamProjectSettings.Portal.AbsoluteUri);
              jsObject3.Add("icon", (object) "tfs-project-portal");
              jsObject3.Add("target", (object) "_blank");
              jsObjectList6.Add(jsObject3);
            }
            if (teamProjectSettings.Guidance != (Uri) null)
            {
              List<JsObject> jsObjectList7 = jsObjectList1;
              JsObject jsObject4 = new JsObject();
              jsObject4.Add("text", (object) WACommonResources.ViewProcessGuidance);
              jsObject4.Add("url", (object) teamProjectSettings.Guidance.AbsoluteUri);
              jsObject4.Add("icon", (object) "tfs-process-guidance");
              jsObject4.Add("target", (object) "_blank");
              jsObjectList7.Add(jsObject4);
            }
            if (teamProjectSettings.ReportManagerFolderUrl != (Uri) null)
            {
              List<JsObject> jsObjectList8 = jsObjectList1;
              JsObject jsObject5 = new JsObject();
              jsObject5.Add("text", (object) WACommonResources.ViewReports);
              jsObject5.Add("url", (object) teamProjectSettings.ReportManagerFolderUrl.AbsoluteUri);
              jsObject5.Add("icon", (object) "tfs-reports");
              jsObject5.Add("target", (object) "_blank");
              jsObjectList8.Add(jsObject5);
            }
          }
        }
        List<JsObject> jsObjectList9 = jsObjectList1;
        JsObject jsObject6 = new JsObject();
        jsObject6.Add("text", (object) WACommonResources.OpenVisualStudio);
        jsObject6.Add("url", (object) VisualStudioLinkingUtility.GetTeamProjectLink(tfsWebContext.TfsRequestContext, tfsWebContext.CurrentProjectGuid, false));
        jsObject6.Add("icon", (object) "open-visualstudio");
        jsObjectList9.Add(jsObject6);
        data["activities"] = (object) jsObjectList1;
        return htmlHelper.JsonIsland((object) data, (object) new
        {
          @class = "options"
        });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (TeamInformation));
      }
    }

    public static MvcHtmlString GettingStartedOptions(
      this HtmlHelper htmlHelper,
      GettingStartedModel model)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (GettingStartedOptions));
      try
      {
        return htmlHelper.JsonIsland((object) new
        {
          jobId = model.JobId,
          createTeamProject = model.CreateTeamProject,
          collectionId = model.CollectionId,
          accountUrl = model.AccountUrl,
          collectionExists = model.CollectionExists,
          isProjectCreationLockdownMode = model.IsProjectCreationLockdownMode
        }, (object) new{ @class = "options" });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (GettingStartedOptions));
      }
    }

    public static MvcHtmlString AccountHomeViewData(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountHomeViewData));
      try
      {
        return htmlHelper.JsonIsland((object) new
        {
          TeamProjectsData = ((JsonResult) htmlHelper.ViewData["TeamProjectsData"]).Data,
          AboutTfsData = ((JsonResult) htmlHelper.ViewData["AboutTfsData"]).Data,
          NewProjectControlData = ((JsonResult) htmlHelper.ViewData["NewProjectControlData"]).Data,
          VsLinksData = ((JsonResult) htmlHelper.ViewData["VsLinksData"]).Data,
          AccountTrialData = ((JsonResult) htmlHelper.ViewData["AccountTrialData"]).Data,
          OpenPlatformAnnouncementData = ((JsonResult) htmlHelper.ViewData["OpenPlatformAnnouncementData"]).Data,
          PowerBIAnnouncementData = ((JsonResult) htmlHelper.ViewData["PowerBIAnnouncementData"]).Data,
          AzureBenefitsAnnouncementData = ((JsonResult) htmlHelper.ViewData["AzureBenefitsAnnouncementData"]).Data,
          ElsVsoIntegrationAnnouncementData = ((JsonResult) htmlHelper.ViewData["ElsVsoIntegrationAnnouncementData"]).Data,
          ResourceUsageData = (htmlHelper.ViewData["ResourceUsageData"] != null ? ((JsonResult) htmlHelper.ViewData["ResourceUsageData"]).Data : (object) null),
          CollectionData = ((JsonResult) htmlHelper.ViewData["CollectionData"]).Data,
          StakeholderData = (htmlHelper.ViewData["StakeholderData"] != null ? ((JsonResult) htmlHelper.ViewData["StakeholderData"]).Data : (object) null),
          StartedVideo = ((JsonResult) htmlHelper.ViewData["CanSeeGettingStartedVideo"]).Data
        }, (object) new{ @class = "options" });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountHomeViewData));
      }
    }

    public static MvcHtmlString TeamHomeViewData(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (TeamHomeViewData));
      try
      {
        return htmlHelper.JsonIsland((object) new
        {
          HowToData = ((JsonResult) htmlHelper.ViewData["HowToData"]).Data,
          TeamLinksData = ((JsonResult) htmlHelper.ViewData["TeamLinksData"]).Data,
          VsLinksData = ((JsonResult) htmlHelper.ViewData["VsLinksData"]).Data,
          WelcomeData = ((JsonResult) htmlHelper.ViewData["WelcomeData"]).Data,
          TeamMembersData = ((JsonResult) htmlHelper.ViewData["TeamMembersData"]).Data
        }, (object) new{ @class = "options" });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (TeamHomeViewData));
      }
    }
  }
}
