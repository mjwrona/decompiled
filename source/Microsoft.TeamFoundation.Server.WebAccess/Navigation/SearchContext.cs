// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.SearchContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class SearchContext : HeaderItemContext
  {
    private const string c_conditionalFaultinCompletedKey = "ConditionalFaultInCompleted";
    private const string c_conditionalWikiFaultinCompletedKey = "ConditionalWikiFaultInCompleted";
    private const int SearchControllerConditionalFaultInTracePoint = 1080725;
    private const string SearchConfigurationRegistryItem = "/Service/ALMSearch/Settings/IsSearchConfigured";

    public SearchContext(IVssRequestContext requestContext)
      : base(10)
    {
      this.Available = this.IsAvailable(requestContext);
    }

    public static HeaderAction GetSearchAction(IVssRequestContext requestContext)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      bool flag = SearchContext.IsAccountPage(requestContext);
      return new HeaderAction()
      {
        TargetSelf = true,
        Text = flag ? WACommonResources.SearchInOrganization : WACommonResources.SearchInProject,
        Icon = "ms-icon",
        FabricIconName = "Search",
        Id = flag ? "search-organization" : "search-project",
        GroupKey = "search",
        Rank = 1,
        Url = service.RouteUrl(requestContext, flag ? "ms.vss-search-web.collection-route" : "ms.vss-search-web.project-route")
      };
    }

    private static bool IsAccountPage(IVssRequestContext requestContext)
    {
      string routeValue = requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "project");
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && string.IsNullOrEmpty(routeValue);
    }

    protected bool IsAvailable(IVssRequestContext requestContext)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      string routeValue1 = service.GetRouteValue<string>(requestContext, "routeArea");
      string routeValue2 = service.GetRouteValue<string>(requestContext, "project");
      if (StringComparer.OrdinalIgnoreCase.Equals(routeValue1, "Admin"))
        return false;
      if (!SearchContext.IsAccountPage(requestContext))
        return !string.IsNullOrEmpty(routeValue2);
      return requestContext.IsStakeholder() ? this.IsWorkItemSearchAvailable(requestContext) || this.IsWikiSearchAvailable(requestContext) : this.IsCodeSearchAvailable(requestContext) || this.IsWikiSearchAvailable(requestContext) || this.IsWorkItemSearchAvailable(requestContext);
    }

    protected override IDictionary<string, object> GetExtraProperties(
      IVssRequestContext requestContext)
    {
      return (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["shouldInvokeConditionalFaultInForWIS"] = (object) this.ShouldConditionalFaultInHappenForWISearch(requestContext),
        ["shouldInvokeConditionalFaultInForWikiSearch"] = (object) this.ShouldConditionalFaultInHappenForWikiSearch(requestContext),
        ["codeSearchAvailable"] = (object) this.IsCodeSearchAvailable(requestContext),
        ["workItemSearchAvailable"] = (object) this.IsWorkItemSearchAvailable(requestContext),
        ["wikiSearchAvailable"] = (object) this.IsWikiSearchAvailable(requestContext)
      };
    }

    private bool ShouldConditionalFaultInHappenForWISearch(IVssRequestContext requestContext)
    {
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      string routeValue = service.GetRouteValue<string>(requestContext, "controller");
      string commandName = service.GetCommandName(requestContext);
      bool flag1 = string.Equals(routeValue, "backlogs", StringComparison.OrdinalIgnoreCase) || string.Equals(routeValue, "workitems", StringComparison.OrdinalIgnoreCase) || string.Equals(routeValue, "apps", StringComparison.OrdinalIgnoreCase) && string.Equals(commandName, "ms.vss-work-web.query-route", StringComparison.OrdinalIgnoreCase);
      bool flag2 = requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem.Feature.Toggle");
      int num1 = requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem.ConditionalFaultin") ? 1 : 0;
      bool extensionToggleStatus = SearchContext.GetWorkItemSearchExtensionToggleStatus(requestContext);
      bool accountSetting = TfsWebContextExtensions.GetAccountSetting<bool>(requestContext, SettingsUserScope.AllUsers, "ConditionalFaultInCompleted", throwOnError: false);
      int num2 = flag1 ? 1 : 0;
      return (num1 & num2) != 0 && (!flag2 || flag2 & extensionToggleStatus) && !accountSetting && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
    }

    private bool ShouldConditionalFaultInHappenForWikiSearch(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        string routeValue = service.GetRouteValue<string>(requestContext, "controller");
        string commandName = service.GetCommandName(requestContext);
        if ((string.Equals(routeValue, "wiki", StringComparison.OrdinalIgnoreCase) ? 1 : (!string.Equals(routeValue, "apps", StringComparison.OrdinalIgnoreCase) ? 0 : (string.Equals(commandName, "ms.vss-wiki-web.wiki-overview-route", StringComparison.OrdinalIgnoreCase) ? 1 : 0))) != 0)
        {
          int num = requestContext.IsFeatureEnabled("WebAccess.Search.Wiki.ConditionalFaultin") ? 1 : 0;
          bool accountSetting = TfsWebContextExtensions.GetAccountSetting<bool>(requestContext, SettingsUserScope.AllUsers, "ConditionalWikiFaultInCompleted");
          this.SetWikiConditionalAccountFaultInDoneFlag(requestContext);
          return num != 0 && !accountSetting;
        }
      }
      return false;
    }

    private static bool GetWorkItemSearchExtensionToggleStatus(IVssRequestContext requestContext)
    {
      IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? service.IsFeatureEnabled(requestContext, "ms.vss-workitem-search.enable-workitem-search") : service.IsFeatureEnabled(requestContext, "ms.vss-workitem-searchonprem.enable-workitem-search");
    }

    private bool IsCodeSearchAvailable(IVssRequestContext requestContext)
    {
      bool flag1 = requestContext.IsFeatureEnabled("WebAccess.SearchShell");
      bool flag2 = ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-code-search.code-entity-type");
      return !requestContext.IsStakeholder() && flag2 | flag1 && !SearchContext.IsSearchInUnconfiguredStateForOnPremises(requestContext);
    }

    private bool IsWorkItemSearchAvailable(IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem") ? 1 : 0;
      bool flag1 = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-workitem-searchonprem.workitem-entity-type") : ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-workitem-search.workitem-entity-type");
      bool flag2 = requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem.Feature.Toggle");
      return (num != 0 || flag1 && (flag2 && SearchContext.GetWorkItemSearchExtensionToggleStatus(requestContext) || !flag2)) && !SearchContext.IsSearchInUnconfiguredStateForOnPremises(requestContext);
    }

    private bool IsWikiSearchAvailable(IVssRequestContext requestContext) => (requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? (ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-wiki-searchonprem.wiki-entity-type") ? 1 : 0) : (ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-wiki-search.wiki-entity-type") ? 1 : 0)) != 0 && !SearchContext.IsSearchInUnconfiguredStateForOnPremises(requestContext);

    private void SetWikiConditionalAccountFaultInDoneFlag(IVssRequestContext requestContext)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<ISettingsService>().SetValue(vssRequestContext, SettingsUserScope.AllUsers, "ConditionalWikiFaultInCompleted", (object) true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1080725, "WebAccess", TfsTraceLayers.Controller, ex);
      }
    }

    private static bool IsSearchInUnconfiguredStateForOnPremises(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      return !vssRequestContext.GetService<CachedRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) "/Service/ALMSearch/Settings/IsSearchConfigured", false, false);
    }
  }
}
