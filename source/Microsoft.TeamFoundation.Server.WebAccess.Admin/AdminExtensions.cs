// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public static class AdminExtensions
  {
    public static MvcHtmlString ManageViewOptions(this HtmlHelper htmlHelper, ManageViewModel model)
    {
      htmlHelper.ViewContext.TfsWebContext();
      return htmlHelper.JsonIsland((object) new
      {
        displayScope = model.DisplayScope,
        defaultFilter = model.DefaultFilter,
        isAadAccount = model.IsAadAccount
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString SecurityViewOptions(this HtmlHelper htmlHelper, SecurityModel model) => htmlHelper.JsonIsland((object) new
    {
      identityPrefix = model.TitlePrefix,
      identityTitle = model.Title,
      collectionServiceHost = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext.ToServiceHostJson(),
      controlManagesFocus = model.ControlManagesFocus,
      allowAADSearchInHosted = model.AllowAADSearchInHosted
    }, (object) new{ @class = "options" });

    public static MvcHtmlString IdentityViewModelOptions(
      this HtmlHelper htmlHelper,
      IdentityViewModelBase model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        currentIdentity = model.ToJson()
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString EditGroupOptions(
      this HtmlHelper htmlHelper,
      GroupIdentityViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        name = model.FriendlyDisplayName,
        description = model.Description,
        tfid = model.TeamFoundationId
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString EditProjectOptions(
      this HtmlHelper htmlHelper,
      TeamProjectModel model,
      string projectVisibility = null)
    {
      return htmlHelper.JsonIsland((object) new
      {
        description = model.Description,
        name = model.DisplayName,
        projectId = model.ProjectId,
        projectVisibility = projectVisibility,
        hasRenamePermission = model.HasRenamePermission
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString TeamViewModelOptions(
      this HtmlHelper htmlHelper,
      TeamViewModel model)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      return htmlHelper.JsonIsland((object) new
      {
        currentIdentity = model.Identity.ToJson(),
        admins = model.Identity.Administrators,
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
        @class = "team-settings-data"
      });
    }

    public static MvcHtmlString BrowseControlOptions(
      this HtmlHelper htmlHelper,
      BrowseControlModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        showTasks = model.ShowTasks,
        useHashNavigation = model.UseHashNavigation,
        showStoppedCollections = model.ShowStoppedCollections,
        collections = (model.Collections != null ? model.Collections.ToDictionary<TfsServiceHostDescriptor, string, JsObject>((Func<TfsServiceHostDescriptor, string>) (x => x.Name), (Func<TfsServiceHostDescriptor, JsObject>) (x => x.ToJson())) : (Dictionary<string, JsObject>) null),
        hideDefaultTeam = model.HideDefaultTeam,
        showTeamsOnly = model.ShowOnlyTeams,
        ignoreDefaultLoad = model.IgnoreDefaultLoad,
        selectedTeam = model.SelectedTeam
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString BrowseCollectionOptions(
      this HtmlHelper htmlHelper,
      CollectionViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        serviceHost = model.CollectionServiceHost.ToJson()
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString CollectionOverviewOptions(
      this HtmlHelper htmlHelper,
      CollectionViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        projects = model.TeamProjects,
        canCreateProjects = model.CanCreateProjects,
        projectRenameIsEnabled = true
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString ProcessOverviewOptions(
      this HtmlHelper htmlHelper,
      ProcessViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        canCreateProjects = model.CanCreateProjects,
        canCreateProcesses = model.CanCreateProcesses
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString ProcessWorkItemTypeOptions(
      this HtmlHelper htmlHelper,
      ProcessViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        controlContributionInputLimit = model.ControlContributionInputLimit
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString ProcessAddFieldOptions(
      this HtmlHelper htmlHelper,
      ProcessViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        maxPicklistItemsPerList = model.MaxPicklistItemsPerList
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString ProjectOverviewOptions(
      this HtmlHelper htmlHelper,
      TeamProjectModel model,
      bool launchNewTeamDialog = false)
    {
      return htmlHelper.JsonIsland((object) new
      {
        teams = model.Teams,
        defaultTeamId = model.DefaultTeamId,
        launchNewTeamDialog = launchNewTeamDialog
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString AdminLicensesViewOptions(
      this HtmlHelper htmlHelper,
      AdminLicensesViewModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        licenseTypes = model.LicenseTypes
      }, (object) new{ @class = "options" });
    }

    public static MvcHtmlString DisplayLicenseViewOptions(
      this HtmlHelper htmlHelper,
      LicenseModel model)
    {
      return htmlHelper.JsonIsland((object) new
      {
        licenseType = model.LicenseType
      }, (object) new{ @class = "options" });
    }

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

    public static MvcHtmlString AdminServicesViewOptions(
      this HtmlHelper htmlHelper,
      IEnumerable<ConnectedServiceViewModel> connectedServices)
    {
      return htmlHelper.JsonIsland((object) new
      {
        connectedServices = connectedServices.Select<ConnectedServiceViewModel, JsObject>((Func<ConnectedServiceViewModel, JsObject>) (x => x.ToJson()))
      }, (object) new{ @class = "options" });
    }
  }
}
