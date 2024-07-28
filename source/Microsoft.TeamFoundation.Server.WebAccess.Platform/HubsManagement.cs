// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.HubsManagement
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class HubsManagement
  {
    private const string c_hubGroupsCollectionContributionIdFormat = "ms.vss-web.{0}-hub-groups-collection";
    private const int c_defaultHubOrder = 100;
    private const int c_defaultHubGroupOrder = 100;
    private const string c_defaultController = "home";
    private const string c_defaultAction = "index";

    public static void PopulateFromContributions(
      this HubsContext hubsContext,
      WebContext webContext,
      Func<Contribution, bool> hubContributionFilter = null,
      string hubGroupsCollectionContributionId = null,
      string navigationRelativeRequestUrl = null,
      string navigationRootUrl = null)
    {
      if (webContext.TfsRequestContext.IntendedHostType() == TeamFoundationHostType.Deployment)
        return;
      using (WebPerformanceTimer.StartMeasure(webContext.RequestContext, "HubsManagement.PopulateFromContributions"))
      {
        if (hubsContext.HubGroups == null)
          hubsContext.HubGroups = new List<HubGroup>();
        if (hubsContext.Hubs == null)
          hubsContext.Hubs = new List<Hub>();
        if (string.IsNullOrEmpty(hubGroupsCollectionContributionId))
        {
          IContributionNavigationService service = webContext.TfsRequestContext.GetService<IContributionNavigationService>();
          ContributedNavigation selectedElementByType1 = service.GetSelectedElementByType(webContext.TfsRequestContext, "ms.vss-web.hub-groups-collection");
          if (selectedElementByType1 != null)
          {
            hubsContext.HubGroupsCollectionContributionId = selectedElementByType1.Id;
            ContributedNavigation selectedElementByType2 = service.GetSelectedElementByType(webContext.TfsRequestContext, "ms.vss-web.hub");
            if (selectedElementByType2 != null)
              hubsContext.SelectedHubId = selectedElementByType2.Id;
            List<ContributedNavigation> selectedElementsByType = service.GetSelectedElementsByType(webContext.TfsRequestContext, "ms.vss-web.navigation");
            if (selectedElementsByType != null && selectedElementsByType.Count > 0)
              hubsContext.SelectedNavigationIds = selectedElementsByType.Select<ContributedNavigation, string>((Func<ContributedNavigation, string>) (cn => cn.Id)).ToArray<string>();
          }
          else
            hubsContext.HubGroupsCollectionContributionId = HubsManagement.GetHubGroupsCollectionContributionId(webContext);
        }
        else
          hubsContext.HubGroupsCollectionContributionId = hubGroupsCollectionContributionId;
        IContributionService service1 = webContext.TfsRequestContext.GetService<IContributionService>();
        IDictionary<string, ContributionNode> dictionary = service1.QueryContributions(webContext.TfsRequestContext, (IEnumerable<string>) new string[1]
        {
          hubsContext.HubGroupsCollectionContributionId
        }, ContributionQueryOptions.None, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) =>
        {
          if (parentContribution == null || !string.Equals(parentContribution.Type, "ms.vss-web.hub", StringComparison.OrdinalIgnoreCase))
            return ContributionQueryOptions.IncludeAll;
          return string.Equals(contribution.Type, "ms.vss-web.property-provider", StringComparison.OrdinalIgnoreCase) ? ContributionQueryOptions.IncludeSelf : ContributionQueryOptions.None;
        }));
        hubsContext.HubGroupsUnpinnedByDefault = false;
        ContributionNode contributionNode1;
        if (dictionary.TryGetValue(hubsContext.HubGroupsCollectionContributionId, out contributionNode1))
          hubsContext.HubGroupsUnpinnedByDefault = contributionNode1.GetProperty<bool>(webContext.TfsRequestContext, "hubGroupsUnpinnedByDefault");
        foreach (ContributionNode contributionNode2 in dictionary.Values.Where<ContributionNode>((Func<ContributionNode, bool>) (c => string.Equals(c.Contribution.Type, "ms.vss-web.hub-group", StringComparison.OrdinalIgnoreCase))))
        {
          Contribution contribution = contributionNode2.Contribution;
          HubGroup hubGroup = new HubGroup()
          {
            Id = contribution.Id,
            Name = HubsManagement.GetHubContributionName(contributionNode2, webContext),
            Order = contributionNode2.GetProperty<double>(webContext.TfsRequestContext, "order", 100.0),
            BuiltIn = contribution.IsTargeting("ms.vss-web.builtin-hub-groups"),
            Hidden = contribution.GetProperty<bool>("hidden"),
            NonCollapsible = false
          };
          if (contribution.Id.StartsWith("ms.", StringComparison.OrdinalIgnoreCase))
          {
            string property = contribution.GetProperty<string>("icon", string.Empty);
            if (!string.IsNullOrEmpty(property))
              hubGroup.Icon = property;
            hubGroup.NonCollapsible = contribution.GetProperty<bool>("noncollapsible");
          }
          if (hubsContext.HubGroupsUnpinnedByDefault)
            hubGroup.BuiltIn = contribution.GetProperty<bool>("pinnedByDefault", hubGroup.NonCollapsible);
          hubsContext.HubGroups.Add(hubGroup);
        }
        IEnumerable<ContributionNode> source1 = dictionary.Values.Where<ContributionNode>((Func<ContributionNode, bool>) (c => string.Equals(c.Contribution.Type, "ms.vss-web.hub", StringComparison.OrdinalIgnoreCase)));
        if (hubContributionFilter != null)
          source1 = (IEnumerable<ContributionNode>) source1.Where<ContributionNode>((Func<ContributionNode, bool>) (c => hubContributionFilter(c.Contribution))).ToList<ContributionNode>();
        IContributionRoutingService service2 = webContext.TfsRequestContext.GetService<IContributionRoutingService>();
        if (navigationRelativeRequestUrl == null)
          navigationRelativeRequestUrl = HubsManagement.GetNavigationRelativeRequestUrl(webContext.TfsRequestContext, webContext.NavigationContext);
        if (navigationRootUrl == null)
        {
          string contextRelativeUrl = HubsManagement.GetNavigationContextRelativeUrl(webContext.TfsRequestContext, webContext.NavigationContext);
          navigationRootUrl = Uri.EscapeUriString(webContext.TfsRequestContext.VirtualPath()).TrimEnd('/') + contextRelativeUrl;
        }
        foreach (ContributionNode contributionNode3 in source1)
        {
          IEnumerable<Hub> hubs = HubsManagement.ContributionToHubs(contributionNode3, webContext, hubsContext, service2, service1, navigationRelativeRequestUrl, navigationRootUrl);
          hubsContext.Hubs.AddRange(hubs);
        }
        IList<Hub> list = (IList<Hub>) hubsContext.Hubs.Where<Hub>((Func<Hub, bool>) (h => h.IsSelected)).ToList<Hub>();
        if (list.Count > 0)
        {
          Hub hub = list[0];
          if (list.Count > 1)
          {
            for (int index = 1; index < list.Count; ++index)
            {
              if (list[index].Uri.Length > hub.Uri.Length)
              {
                hub.IsSelected = false;
                hub = list[index];
              }
              else
                list[index].IsSelected = false;
            }
          }
          hubsContext.SelectedHubId = hub.Id;
          hubsContext.SelectedHubGroupId = hub.GroupId;
        }
        foreach (HubGroup hubGroup1 in hubsContext.HubGroups)
        {
          HubGroup hubGroup = hubGroup1;
          IEnumerable<Hub> source2 = hubsContext.Hubs.Where<Hub>((Func<Hub, bool>) (hub => string.Equals(hub.GroupId, hubGroup.Id, StringComparison.OrdinalIgnoreCase)));
          if (source2.Any<Hub>())
          {
            hubGroup.HasHubs = true;
            double num = 0.0;
            foreach (Hub hub in source2)
            {
              if (hubGroup.Uri == null || hub.Order < num)
              {
                num = hub.Order;
                hubGroup.Uri = hub.Uri;
              }
            }
          }
        }
        hubsContext.AllHubs = hubsContext.Hubs.ToList<Hub>();
        hubsContext.Hubs = hubsContext.Hubs.Where<Hub>((Func<Hub, bool>) (hub => string.Equals(hub.GroupId, hubsContext.SelectedHubGroupId, StringComparison.OrdinalIgnoreCase))).ToList<Hub>();
        hubsContext.HubGroups = hubsContext.HubGroups.OrderBy<HubGroup, double>((Func<HubGroup, double>) (hg => hg.Order)).ToList<HubGroup>();
        hubsContext.Hubs = hubsContext.Hubs.OrderBy<Hub, double>((Func<Hub, double>) (hub => hub.Order)).ToList<Hub>();
      }
    }

    private static IEnumerable<Hub> ContributionToHubs(
      ContributionNode contributionNode,
      WebContext webContext,
      HubsContext hubsContext,
      IContributionRoutingService routingService,
      IContributionService contributionService,
      string requestRelativeUrl,
      string navigationContextRootUrl)
    {
      List<Hub> hubs = new List<Hub>();
      Contribution contribution1 = contributionNode.Contribution;
      string property1 = contributionNode.GetProperty<string>(webContext.TfsRequestContext, "name");
      if (string.IsNullOrWhiteSpace(property1))
        return (IEnumerable<Hub>) hubs;
      string[] property2 = contribution1.GetProperty<string[]>("featureFlags");
      if (property2 != null)
      {
        foreach (string featureName in property2)
        {
          if (!webContext.TfsRequestContext.IsFeatureEnabled(featureName))
            return (IEnumerable<Hub>) hubs;
        }
      }
      List<Contribution> contributionsOfType = HubsManagement.GetParentContributionsOfType(contributionNode, "ms.vss-web.hub-group");
      if (contributionsOfType.Count == 0)
        return (IEnumerable<Hub>) hubs;
      bool flag = contribution1.Id.StartsWith("ms.", StringComparison.OrdinalIgnoreCase);
      Hub hub1 = new Hub()
      {
        Id = contribution1.Id,
        Name = contribution1.GetProperty<string>("newHeaderName", property1),
        GroupId = contributionsOfType[0].Id,
        Order = (double) contribution1.GetProperty<int>("order", 100),
        Hidden = contribution1.GetProperty<bool>("hidden"),
        BuiltIn = flag,
        Icon = contribution1.GetProperty<string>("icon")
      };
      if (!string.IsNullOrEmpty(hubsContext.SelectedHubId))
        hub1.IsSelected = string.Equals(contribution1.Id, hubsContext.SelectedHubId, StringComparison.OrdinalIgnoreCase);
      if (routingService != null)
      {
        string property3 = contribution1.GetProperty<string>("defaultRoute");
        if (!string.IsNullOrEmpty(property3))
        {
          hub1.Uri = routingService.RouteUrl(webContext.TfsRequestContext, property3);
          if (!string.IsNullOrEmpty(hub1.Uri) && contribution1.GetProperty<bool>("supportsXHRNavigate", true))
          {
            Contribution contribution2 = contributionService.QueryContribution(webContext.TfsRequestContext, property3);
            if (contribution2 != null)
            {
              string property4 = contribution2.GetProperty<string>("defaults.controller");
              if (property4 == null || string.Equals("Apps", property4, StringComparison.OrdinalIgnoreCase) || string.Equals("ContributedPage", property4, StringComparison.OrdinalIgnoreCase))
                hub1.SupportsXHRNavigate = true;
            }
          }
        }
      }
      if (string.IsNullOrEmpty(hub1.Uri))
      {
        string str;
        if (contribution1.GetProperty<bool>("fullPage"))
        {
          str = contribution1.GetTemplateProperty(webContext.TfsRequestContext, "uri", (object) webContext, PlatformMustacheExtensions.Parser).TrimStart('/');
        }
        else
        {
          str = "_apps/hub/" + Uri.EscapeDataString(contribution1.Id);
          hub1.SupportsXHRNavigate = true;
        }
        hub1.Uri = navigationContextRootUrl;
        if (!string.IsNullOrEmpty(webContext.NavigationContext.Area))
        {
          if (contribution1.GetProperty<bool>("areaAware"))
          {
            requestRelativeUrl = "_" + webContext.NavigationContext.Area.ToLowerInvariant() + "/" + requestRelativeUrl;
          }
          else
          {
            Hub hub2 = hub1;
            hub2.Uri = hub2.Uri + "/_" + Uri.EscapeDataString(webContext.NavigationContext.Area.ToLowerInvariant());
          }
        }
        Hub hub3 = hub1;
        hub3.Uri = hub3.Uri + "/" + (string.IsNullOrEmpty(str) ? "_home" : str);
        if (requestRelativeUrl.StartsWith(str, StringComparison.OrdinalIgnoreCase) && (requestRelativeUrl.Length == str.Length || requestRelativeUrl[str.Length] == '/' || requestRelativeUrl[str.Length] == '?'))
          hub1.IsSelected = true;
      }
      if (contribution1.GetProperty<bool>("expanded"))
        hub1.Hidden = true;
      hubs.Add(hub1);
      if (contributionsOfType.Count > 1)
      {
        string uri = hub1.Uri;
        string str = string.Empty;
        int num = uri.IndexOfAny(new char[2]{ '?', '#' });
        if (num >= 0)
          str = str[num] != '?' ? uri.Substring(num) : "&" + uri.Substring(num + 1);
        hub1.Uri = uri + "?hubGroupId=" + Uri.EscapeDataString(contributionsOfType[0].Id) + str;
        for (int index = 1; index < contributionsOfType.Count; ++index)
        {
          Hub hub4 = new Hub()
          {
            GroupId = contributionsOfType[index].Id,
            Id = hub1.Id,
            Name = hub1.Name,
            Order = hub1.Order,
            Uri = uri + "?hubGroupId=" + Uri.EscapeDataString(contributionsOfType[index].Id) + str,
            BuiltIn = hub1.BuiltIn,
            Icon = hub1.Icon,
            SupportsXHRNavigate = hub1.SupportsXHRNavigate
          };
          hubs.Add(hub4);
        }
        if (hub1.IsSelected)
        {
          string a = webContext.RequestContext.HttpContext.Request.Params["hubGroupId"];
          if (!string.IsNullOrEmpty(a))
          {
            hub1.IsSelected = false;
            foreach (Hub hub5 in hubs)
              hub5.IsSelected = string.Equals(a, hub5.GroupId, StringComparison.OrdinalIgnoreCase);
          }
        }
      }
      return (IEnumerable<Hub>) hubs;
    }

    private static string GetHubContributionName(
      ContributionNode contributionNode,
      WebContext webContext)
    {
      JObject property = contributionNode.Contribution.GetProperty<JObject>("alternateNames");
      if (property != null)
      {
        string contributionName = property[webContext.NavigationContext.TopMostLevel.ToString().ToLowerInvariant()]?.ToString();
        if (contributionName != null)
          return contributionName;
      }
      return contributionNode.GetProperty<string>(webContext.TfsRequestContext, "name");
    }

    private static List<Contribution> GetParentContributionsOfType(
      ContributionNode contribution,
      string contributionType)
    {
      List<Contribution> resultContributions = new List<Contribution>();
      HashSet<string> visitedNodes = new HashSet<string>();
      if (contribution.Parents != null)
      {
        foreach (ContributionNode parent in contribution.Parents)
          HubsManagement.AddParentContributionsOfType(parent, contributionType, resultContributions, visitedNodes);
      }
      return resultContributions;
    }

    private static void AddParentContributionsOfType(
      ContributionNode contributionNode,
      string contributionType,
      List<Contribution> resultContributions,
      HashSet<string> visitedNodes)
    {
      visitedNodes.Add(contributionNode.Id);
      if (string.Equals(contributionNode.Contribution.Type, contributionType, StringComparison.OrdinalIgnoreCase))
      {
        resultContributions.Add(contributionNode.Contribution);
      }
      else
      {
        if (contributionNode.Parents == null)
          return;
        foreach (ContributionNode parent in contributionNode.Parents)
        {
          if (!visitedNodes.Contains(parent.Id))
            HubsManagement.AddParentContributionsOfType(parent, contributionType, resultContributions, visitedNodes);
        }
      }
    }

    private static string GetNavigationRelativeRequestUrl(
      IVssRequestContext requestContext,
      NavigationContext navigationContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(navigationContext.CurrentController) && !string.Equals(navigationContext.CurrentController, "home", StringComparison.OrdinalIgnoreCase))
      {
        stringBuilder.Append('_');
        stringBuilder.Append(Uri.EscapeDataString(navigationContext.CurrentController));
        if (!string.IsNullOrEmpty(navigationContext.CurrentAction) && !string.Equals(navigationContext.CurrentAction, "index", StringComparison.OrdinalIgnoreCase))
        {
          stringBuilder.Append("/");
          stringBuilder.Append(Uri.EscapeDataString(navigationContext.CurrentAction));
          if (!string.IsNullOrEmpty(navigationContext.CurrentParameters))
          {
            stringBuilder.Append("/");
            stringBuilder.Append(Uri.EscapeDataString(navigationContext.CurrentParameters));
          }
        }
      }
      return stringBuilder.ToString();
    }

    private static string GetNavigationContextRelativeUrl(
      IVssRequestContext requestContext,
      NavigationContext navigationContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(navigationContext.Project))
      {
        stringBuilder.Append("/");
        stringBuilder.Append(Uri.EscapeDataString(navigationContext.Project));
      }
      if (!string.IsNullOrEmpty(navigationContext.Team))
      {
        stringBuilder.Append("/");
        stringBuilder.Append(Uri.EscapeDataString(navigationContext.Team));
      }
      return stringBuilder.ToString();
    }

    private static string GetNavigationContextText(NavigationContextLevels topMostLevel)
    {
      switch (topMostLevel)
      {
        case NavigationContextLevels.Deployment:
          return "deployment";
        case NavigationContextLevels.Application:
          return "account";
        case NavigationContextLevels.Collection:
          return "collection";
        case NavigationContextLevels.Project:
        case NavigationContextLevels.Team:
          return "project";
        default:
          return string.Empty;
      }
    }

    private static string GetHubGroupsCollectionContributionId(WebContext webContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(HubsManagement.GetNavigationContextText(webContext.NavigationContext.TopMostLevel));
      return string.Format("ms.vss-web.{0}-hub-groups-collection", (object) stringBuilder);
    }

    public static string GetHubGroupsCollectionContributionId(NavigationContextLevels topMostLevel) => string.Format("ms.vss-web.{0}-hub-groups-collection", (object) HubsManagement.GetNavigationContextText(topMostLevel));
  }
}
