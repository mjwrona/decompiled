// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.NavigationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Navigation;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class NavigationExtensions
  {
    private const int MaxMenuItems = 5;
    private const string c_hubGroupViewDataKey = "_HubGroups_";
    private const string c_hubGroupSkipMruUpdateViewDataKey = "_HubGroupSkipMRUUpdate_";
    private static int DefaultJsonMaxLength = NavigationExtensions.GetDefaultJsonMaxLength();
    private static readonly HashSet<string> s_navigationHeaderContributionTypesFilter = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-web.hubs-provider",
      "ms.vss-tfs-web.hub-group-action"
    };
    private static ConcurrentDictionary<string, GetTargetRouteParametersDelegate> s_getTargetRouteParamsLookup = new ConcurrentDictionary<string, GetTargetRouteParametersDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static void RegisterGetTargetRouteParamsCallback(
      string controllerName,
      string actionName,
      GetTargetRouteParametersDelegate callback)
    {
      NavigationExtensions.s_getTargetRouteParamsLookup.TryAdd(controllerName + "." + actionName, callback);
    }

    public static string GetTargetRouteParametersValue(
      TfsWebContext tfsWebContext,
      string controllerName,
      string actionName,
      string currentParameters)
    {
      GetTargetRouteParametersDelegate parametersDelegate;
      return NavigationExtensions.s_getTargetRouteParamsLookup.TryGetValue(controllerName + "." + actionName, out parametersDelegate) ? parametersDelegate(tfsWebContext, currentParameters) : (string) null;
    }

    public static IEnumerable<NavigationExtensions.MruItem> GetProjectMRUList(
      TfsWebContext tfsWebContext)
    {
      return ((IEnumerable<MRUNavigationContextEntry>) tfsWebContext.MruNavigationContexts).Where<MRUNavigationContextEntry>((Func<MRUNavigationContextEntry, bool>) (mru => mru.TopMostLevel >= NavigationContextLevels.Project)).Select<MRUNavigationContextEntry, NavigationExtensions.MruItem>((Func<MRUNavigationContextEntry, NavigationExtensions.MruItem>) (entry => new NavigationExtensions.MruItem()
      {
        Text = NavigationExtensions.GetContextMenuItemName(entry.Project, entry.Team),
        Title = NavigationExtensions.GetContextMenuItemTitle(entry.Project, entry.Team),
        Url = NavigationExtensions.GetProjectMruUrl(tfsWebContext, entry),
        HashCode = entry.GetHashCode(),
        IsTeam = !string.IsNullOrEmpty(entry.Team),
        LastAccessed = entry.LastAccessedByUser
      }));
    }

    private static string GetProjectMruUrl(
      TfsWebContext tfsWebContext,
      MRUNavigationContextEntry mruEntry)
    {
      return tfsWebContext.Url.Action("index", "dashboards", (object) new
      {
        routeArea = "",
        serviceHost = mruEntry.ServiceHost,
        project = (mruEntry.Project ?? ""),
        team = (mruEntry.Team ?? "")
      });
    }

    private static string GetContextMenuItemName(WebContext webContext)
    {
      string project = (string) null;
      string team = (string) null;
      if (webContext.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project))
        project = webContext.ProjectContext.Name;
      if (webContext.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Team))
        team = webContext.TeamContext.Name;
      return NavigationExtensions.GetContextMenuItemName(project, team);
    }

    private static string GetContextMenuItemName(string project, string team)
    {
      if (string.IsNullOrEmpty(project))
        return WACommonResources.NavigationContextMenuDefaultLabel;
      return string.IsNullOrEmpty(team) ? project : string.Format((IFormatProvider) CultureInfo.InvariantCulture, WACommonResources.NavigationContextMenuTeamLabelFormat, (object) project, (object) team);
    }

    private static string GetContextMenuItemTitle(string project, string team)
    {
      if (string.IsNullOrEmpty(project))
        return WACommonResources.NavigationContextMenuDefaultLabelTitle;
      return string.IsNullOrEmpty(team) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, WACommonResources.NavigationContextMenuProjectLabelTitleFormat, (object) project) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, WACommonResources.NavigationContextMenuTeamLabelTitleFormat, (object) team, (object) project);
    }

    public static string GetTargetUrl(
      TfsWebContext tfsWebContext,
      string area,
      string controller,
      string action,
      string parameters,
      NavigationContextLevels currentLevel,
      NavigationContextLevels targetNavigationContextLevel,
      RouteValueDictionary routeValues,
      bool includeQueryString)
    {
      string str1 = currentLevel == targetNavigationContextLevel ? action : "index";
      string str2 = (string) null;
      string str3 = (string) null;
      string str4;
      if (!string.IsNullOrEmpty(controller) && TfsRouteAreaConstraint.IsControllerSupported(area, controller, targetNavigationContextLevel))
      {
        str4 = controller;
        str3 = NavigationExtensions.GetTargetRouteParametersValue(tfsWebContext, str4, str1, parameters);
        if (includeQueryString)
          str2 = tfsWebContext.RequestContext.HttpContext.Request.Url.Query;
      }
      else
      {
        str4 = "home";
        str1 = "index";
        if (!TfsRouteAreaConstraint.IsControllerSupported(area, str4, targetNavigationContextLevel))
          area = "";
      }
      string routeName = !string.Equals("home", str4, StringComparison.OrdinalIgnoreCase) || !string.Equals("index", str1, StringComparison.OrdinalIgnoreCase) ? TfsRouteHelpers.GetControllerActionRouteName(targetNavigationContextLevel, area, !string.IsNullOrEmpty(str3)) : TfsRouteHelpers.GetLevelRouteName(targetNavigationContextLevel, area);
      string targetUrl = Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions.RouteUrl(tfsWebContext.Url, routeName, str1, str4, new RouteValueDictionary((IDictionary<string, object>) routeValues)
      {
        {
          "routeArea",
          (object) area
        },
        {
          nameof (parameters),
          (object) str3
        }
      });
      if (includeQueryString && !string.IsNullOrEmpty(str2))
        targetUrl += str2;
      return targetUrl;
    }

    public static MvcHtmlString UserInfo(this HtmlHelper htmlHelper) => htmlHelper.UserInfo((TeamFoundationIdentity) null);

    public static MvcHtmlString UserInfo(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (identity == null)
        identity = tfsWebContext.CurrentUserIdentity;
      string email = identity.GetAttribute("Mail", string.Empty);
      return new TagBuilder("div").AddClass("user-info").Append((object) htmlHelper.IdentityImageTag(identity)).AppendTag("span", (Action<TagBuilder>) (span =>
      {
        span.AddClass("display-name").Attribute("title", identity.UniqueName).Text(identity.DisplayName);
        if (string.IsNullOrEmpty(email))
          return;
        span.Data("sip", (object) email).Data("email", (object) email);
      })).Scope((Action<TagBuilder>) (div =>
      {
        if (string.IsNullOrEmpty(email))
          return;
        div.AppendTag("a", (Action<TagBuilder>) (a => a.Attribute("href", "mailto:" + email).AddClass("email").Text(email)));
      })).ToHtmlString();
    }

    public static MvcHtmlString UserImage(this HtmlHelper htmlHelper) => htmlHelper.UserImage((TeamFoundationIdentity) null);

    public static MvcHtmlString UserImage(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (identity == null)
        identity = tfsWebContext.CurrentUserIdentity;
      return htmlHelper.IdentityImageTag(identity, (object) null, (object) new
      {
        size = 2,
        t = DateTime.Now.Ticks
      }).ToHtmlString();
    }

    private static MenuOwner GenerateUserMenu(
      this HtmlHelper htmlHelper,
      bool includeUserManagementItems)
    {
      TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
      PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      MenuBar menuBase = ControlFactory.Create<MenuBar>().CssClass<MenuBar>("top-level-menu user-header").ShowIcon<MenuBar>(false).PopupAlign<MenuBar>("right-bottom");
      TeamFoundationIdentity currentUserIdentity = webContext.CurrentUserIdentity;
      bool flag = currentUserIdentity.Descriptor.IdentityType.Equals("Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase) || currentUserIdentity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase);
      string attribute1 = currentUserIdentity.GetAttribute("Mail", string.Empty);
      string attribute2 = currentUserIdentity.GetAttribute("Account", string.Empty);
      string attribute3 = currentUserIdentity.GetAttribute("Domain", string.Empty);
      menuBase.AddMenuItem().Text(currentUserIdentity.DisplayName).Title(flag ? attribute1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) attribute3, (object) attribute2)).CommandId("user").IdIsAction(false).Scope<MenuItem>((Action<MenuItem>) (menuItem =>
      {
        if (includeUserManagementItems && webContext.NavigationContext.TopMostLevel != NavigationContextLevels.Deployment)
        {
          menuItem.AddMenuItem().Html(htmlHelper.UserInfo().ToHtmlString());
          NavigationExtensions.AddProfileSecurityAndUsageActions(webContext, menuItem);
        }
        if (includeUserManagementItems && webContext.NavigationContext.TopMostLevel >= NavigationContextLevels.Project && !StringComparer.OrdinalIgnoreCase.Equals(webContext.NavigationContext.Area, "Admin") && webContext.FeatureContext.AreStandardFeaturesAvailable && pageContext.WebAccessConfiguration.MailSettings.Enabled)
          menuItem.AddMenuItem().Text(WACommonResources.ManageAlerts).CommandId("manageAlerts");
        string rawUrl = htmlHelper.ViewContext.RequestContext.HttpContext.Request.RawUrl;
        string empty = string.Empty;
        string routeName = "ServiceHostControllerAction";
        if (!webContext.IsHosted)
          menuItem.AddMenuItem().Text(WACommonResources.SignInAsDifferentUser).ActionLink(webContext.Url.RouteUrl(routeName, "index", "signout", (object) new
          {
            routeArea = "",
            serviceHost = webContext.TfsRequestContext.ServiceHost.OrganizationServiceHost,
            mode = "SignInAsDifferentUser",
            redirectUrl = rawUrl,
            project = "",
            team = ""
          }));
        menuItem.AddMenuItem().Text(WACommonResources.SignOut).ActionLink(NavigationExtensions.BuildSignOutLink(webContext, rawUrl));
        menuItem.AddSeparator();
        menuItem.AddMenuItem().Text(WACommonResources.Header_Community).Action("navigate", (object) new
        {
          url = "https://go.microsoft.com/fwlink/?LinkId=253552",
          target = "_blank"
        });
        if (!webContext.IsHosted)
          return;
        menuItem.AddMenuItem().Text(WACommonResources.Header_Support).Action("navigate", (object) new
        {
          url = "https://go.microsoft.com/fwlink/?LinkId=253553",
          target = "_blank"
        });
      }));
      return (MenuOwner) menuBase;
    }

    private static MenuOwner GenerateProfileMenu(
      this HtmlHelper htmlHelper,
      bool includeUserManagementItems)
    {
      TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
      HeaderAction userAction = UserSettingsContext.GetUserAction(webContext.TfsRequestContext);
      if (userAction != null)
      {
        Microsoft.TeamFoundation.Server.WebAccess.Navigation.UserContext userContext = UserSettingsContext.GetUserContext(webContext.TfsRequestContext);
        MenuBar menuBase = ControlFactory.Create<MenuBar>().CssClass<MenuBar>("top-level-menu-v2 user-menu header-item").ShowIcon<MenuBar>(false).PopupAlign<MenuBar>("right-bottom");
        menuBase.AddMenuItem().Text(userAction.Text).Title(userAction.Title).CommandId("user").IdIsAction(false).TextClass("alignment-marker").AddChildMenuOptions((object) new
        {
          alignToMarkerHorizontal = true
        }).Scope<MenuItem>((Action<MenuItem>) (menuItem =>
        {
          if (includeUserManagementItems)
          {
            menuItem.AddMenuItem().CssClass<MenuItem>("identity-image").Disabled(true).Html(htmlHelper.UserImage().ToHtmlString());
            NavigationExtensions.AddProfileSecurityAndUsageActions(webContext, menuItem);
            HeaderAction alertsAction = UserSettingsContext.GetAlertsAction(webContext.TfsRequestContext);
            if (alertsAction != null)
              menuItem.AddMenuItem().Text(alertsAction.Text).CssClass<MenuItem>("my-alerts").CommandId(alertsAction.CommandId);
          }
          HeaderAction signInAsAction = UserSettingsContext.GetSignInAsAction(webContext.TfsRequestContext);
          if (signInAsAction != null)
            menuItem.AddMenuItem().Text(signInAsAction.Text).CssClass<MenuItem>("sign-in-as").ActionLink(signInAsAction.Url);
          HeaderAction signOutAction = UserSettingsContext.GetSignOutAction(webContext.TfsRequestContext);
          menuItem.AddMenuItem().Text(signOutAction.Text).TextClass("alignment-marker").CssClass<MenuItem>("sign-out").ActionLink(signOutAction.Url);
          menuItem.AddLabel().Text(userContext.IsAcsAccount ? userContext.AccountName : userContext.FormattedAccountName).Title(userContext.FormattedAccountName).CssClass<MenuItem>("user-email");
        }));
        return (MenuOwner) menuBase;
      }
      MenuBar menuBase1 = ControlFactory.Create<MenuBar>().CssClass<MenuBar>("top-level-menu-v2 header-item").ShowIcon<MenuBar>(false);
      menuBase1.AddMenuItem().Text(WebAccessServerResources.HeaderSignInText).Title(WebAccessServerResources.HeaderSignInTitle).ActionLink(UserSettingsContext.GetSignInUrl(webContext.TfsRequestContext));
      return (MenuOwner) menuBase1;
    }

    public static MvcHtmlString UserMenu(
      this HtmlHelper htmlHelper,
      bool includeUserManagementItems,
      bool isUpdatedStyle)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (UserMenu));
      try
      {
        return !isUpdatedStyle ? htmlHelper.GenerateUserMenu(includeUserManagementItems).ToHtml(htmlHelper) : htmlHelper.GenerateProfileMenu(includeUserManagementItems).ToHtml(htmlHelper);
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (UserMenu));
      }
    }

    public static MvcHtmlString JsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      IDictionary<string, object> htmlAttributes,
      int maxJsonLength)
    {
      TagBuilder tagBuilder = new TagBuilder("script");
      if (htmlAttributes != null)
        tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      if (htmlHelper.ViewContext.TfsWebContext().TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        string str = JsonConvert.SerializeObject(data);
        if (maxJsonLength <= 0 || str == null || str.Length > maxJsonLength)
          throw new InvalidOperationException("Maximum json size exceeded:" + maxJsonLength.ToString() + " but got: " + str.Length.ToString());
        tagBuilder.InnerHtml = str;
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        if (maxJsonLength > 0)
          scriptSerializer.MaxJsonLength = maxJsonLength;
        tagBuilder.InnerHtml = scriptSerializer.Serialize(data);
      }
      tagBuilder.MergeAttribute("type", "application/json");
      tagBuilder.MergeAttribute("defer", "defer");
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static void SkipHubGroupMruUpdate(ViewDataDictionary viewData) => viewData["_HubGroupSkipMRUUpdate_"] = (object) true;

    public static bool ShouldSkipHubGroupMruUpdate(this HtmlHelper htmlHelper)
    {
      bool flag;
      return htmlHelper.ViewData.TryGetValue<bool>("_HubGroupSkipMRUUpdate_", out flag) && flag;
    }

    private static int GetDefaultJsonMaxLength()
    {
      int defaultJsonMaxLength = new JavaScriptSerializer().MaxJsonLength;
      int result;
      if (int.TryParse(ConfigurationManager.AppSettings["maxJsonLength"], out result))
        defaultJsonMaxLength = result;
      return defaultJsonMaxLength;
    }

    public static void RenderTfsHeader(this HtmlHelper htmlHelper) => htmlHelper.RenderPartial("WebPlatformHeader");

    public static void ContributedHeaderInit(this HtmlHelper htmlHelper) => NavigationExtensions.ContributedHeaderInit(htmlHelper.ViewContext.RequestContext);

    public static void ContributedHeaderInit(RequestContext requestContext)
    {
      PageContext pageContext = WebContextFactory.GetPageContext(requestContext);
      GeneralHtmlExtensions.UseCommonScriptModules(requestContext.HttpContext, "VSS/Controls/Header");
      GeneralHtmlExtensions.UseCommonScriptModules(requestContext.HttpContext, "TfsCommon/Scripts/Navigation/Bundle");
      IContributionService service = pageContext.WebContext.TfsRequestContext.GetService<IContributionService>();
      IEnumerable<Contribution> contributions1 = service.QueryContributions(pageContext.WebContext.TfsRequestContext, (IEnumerable<string>) new string[1]
      {
        pageContext.HubsContext.HubGroupsCollectionContributionId
      }, NavigationExtensions.s_navigationHeaderContributionTypesFilter, ContributionQueryOptions.IncludeAll);
      pageContext.AddContributions(contributions1);
      IEnumerable<Contribution> contributions2 = service.QueryContributions(pageContext.WebContext.TfsRequestContext, contributions1.Select<Contribution, string>((Func<Contribution, string>) (c => c.Id)), queryOptions: ContributionQueryOptions.IncludeSubTree);
      pageContext.AddContributions(contributions2);
    }

    private static void AddProfileSecurityAndUsageActions(
      TfsWebContext webContext,
      MenuItem menuItem)
    {
      HeaderAction profileAction = UserSettingsContext.GetProfileAction(webContext.TfsRequestContext);
      if (profileAction != null)
      {
        if (!string.IsNullOrWhiteSpace(profileAction.Url) && !profileAction.TargetSelf)
          menuItem.AddMenuItem().Text(profileAction.Text).CssClass<MenuItem>("my-profile").Title(profileAction.Title).Action("navigate", (object) new
          {
            url = profileAction.Url,
            target = "_blank"
          });
        else
          menuItem.AddMenuItem().Text(profileAction.Text).CssClass<MenuItem>("my-profile").CommandId(profileAction.CommandId);
      }
      HeaderAction securityAction = UserSettingsContext.GetSecurityAction(webContext.TfsRequestContext);
      if (securityAction != null)
        menuItem.AddMenuItem().Text(securityAction.Text).CssClass<MenuItem>("my-security").Action("navigate", (object) new
        {
          url = securityAction.Url
        });
      HeaderAction usageAction = UserSettingsContext.GetUsageAction(webContext.TfsRequestContext);
      if (usageAction == null)
        return;
      menuItem.AddMenuItem().Text(usageAction.Text).CssClass<MenuItem>("my-usage").CommandId(usageAction.CommandId).ActionLink(usageAction.Url);
    }

    private static string BuildSignOutLink(TfsWebContext webContext, string redirectUrl)
    {
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      if (!tfsRequestContext.IsHosted())
        return webContext.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
        {
          routeArea = "",
          serviceHost = tfsRequestContext.ServiceHost.OrganizationServiceHost,
          redirectUrl = redirectUrl,
          project = "",
          team = ""
        });
      if (tfsRequestContext.IsDevOpsDomainRequest())
        return tfsRequestContext.GetService<ITeamFoundationAuthenticationService>().BuildHostedSignOutUrl(tfsRequestContext);
      redirectUrl = (string) null;
      return webContext.Url.RouteUrl("ServiceHostControllerAction", "index", "signout", (object) new
      {
        routeArea = "",
        serviceHost = webContext.TfsRequestContext.ServiceHost,
        redirectUrl = redirectUrl,
        project = "",
        team = ""
      });
    }

    public class MruItem
    {
      public string Text;
      public string Title;
      public string Url;
      public int HashCode;
      public DateTime? LastAccessed;
      public bool IsTeam;
    }
  }
}
