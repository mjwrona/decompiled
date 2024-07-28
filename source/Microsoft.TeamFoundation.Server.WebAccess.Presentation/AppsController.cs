// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.AppsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [SupportedRouteArea(null, NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  [ValidateInput(false)]
  public class AppsController : TfsController
  {
    private const string AppsControllerPublishTimerData = "AppsControllerPublishTimerData";

    static AppsController()
    {
      NavigationExtensions.RegisterGetTargetRouteParamsCallback("Apps", "Hub", new GetTargetRouteParametersDelegate(AppsController.GetContributionIdFromParameters));
      NavigationExtensions.RegisterGetTargetRouteParamsCallback("Apps", "Index", new GetTargetRouteParametersDelegate(AppsController.GetContributionIdFromParameters));
    }

    public AppsController() => this.m_executeContributedRequestHandlers = true;

    private static string GetContributionIdFromParameters(
      TfsWebContext webContext,
      string parameters)
    {
      if (!string.IsNullOrEmpty(parameters))
      {
        string[] strArray = parameters.Split(new char[1]
        {
          '/'
        }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 1)
          return strArray[0];
      }
      return (string) null;
    }

    [HttpGet]
    public ActionResult Index() => (ActionResult) this.Redirect(Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions.RouteUrl(this.TfsWebContext.Url, TfsRouteHelpers.GetControllerActionRouteName(this.NavigationContext.TopMostLevel, (string) null, true), "hub", "apps", new RouteValueDictionary()
    {
      {
        "parameters",
        (object) this.NavigationContext.CurrentParameters
      }
    }));

    [PublicProjectRequestRestrictions]
    [HttpGet]
    public ActionResult Hub()
    {
      Contribution hubContribution = (Contribution) null;
      string idFromParameters = AppsController.GetContributionIdFromParameters(this.TfsWebContext, this.NavigationContext.CurrentParameters);
      if (idFromParameters != null)
        hubContribution = this.TfsRequestContext.GetService<IContributionService>().QueryContribution(this.TfsRequestContext, idFromParameters);
      if (hubContribution != null)
      {
        RouteValueDictionary levelRouteValues = NavigationHelpers.GetCurrentNavigationLevelRouteValues(this.TfsWebContext.NavigationContext);
        string hubDefaultUrl = NavigationHelpers.GetHubDefaultUrl((WebContext) this.TfsWebContext, hubContribution, levelRouteValues);
        if (!string.IsNullOrEmpty(hubDefaultUrl))
        {
          if (!string.IsNullOrEmpty(this.Request.Url.Query))
            hubDefaultUrl += this.Request.Url.Query;
          return (ActionResult) this.Redirect(hubDefaultUrl);
        }
      }
      return this.GetResultForContribution(hubContribution);
    }

    [PublicContributedHubRequestRestrictions("project")]
    [HttpGet]
    public ActionResult ContributedHub()
    {
      IContributionNavigationService service1 = this.TfsRequestContext.GetService<IContributionNavigationService>();
      IContributionService service2 = this.TfsRequestContext.GetService<IContributionService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ContributedNavigation selectedElementByType = service1.GetSelectedElementByType(tfsRequestContext, "ms.vss-web.hub");
      return this.GetResultForContribution(selectedElementByType == null ? (Contribution) null : service2.QueryContribution(this.TfsRequestContext, selectedElementByType.Id));
    }

    private ActionResult GetResultForContribution(Contribution hubContribution)
    {
      bool flag1 = false;
      if (hubContribution != null && !string.IsNullOrEmpty(WebContextFactory.GetPageContext(this.Request.RequestContext).HubsContext.SelectedHubGroupId))
        flag1 = true;
      if (!flag1)
        throw new HttpException(404, WACommonResources.PageNotFound);
      bool flag2 = (hubContribution.GetProperty<int>("::Attributes") & 1) == 1;
      bool flag3 = string.Equals("true", this.Request["_xhr"]);
      bool flag4 = false;
      ContributedNavigation selectedElementByType = this.TfsRequestContext.GetService<IContributionNavigationService>().GetSelectedElementByType(this.TfsRequestContext, "ms.vss-web.page");
      if (selectedElementByType != null)
      {
        Contribution contribution = this.TfsRequestContext.GetService<IContributionService>().QueryContribution(this.TfsRequestContext, selectedElementByType.Id);
        if ((contribution.GetProperty<int>("::Attributes") & 1) == 1)
          flag4 = contribution.GetProperty<string>("viewName", string.Empty).IndexOf("Mobile", StringComparison.OrdinalIgnoreCase) >= 0;
      }
      PageContext pageContext = WebContextFactory.GetPageContext(this.Request.RequestContext);
      this.Request.RequestContext.HttpContext.UseNewPlatformHost(true);
      GeneralHtmlExtensions.UseCommonCSS(this.Request.RequestContext.HttpContext, "jQueryUI-Modified", "Core");
      FontRegistration.RegisterFonts(this.Request.RequestContext);
      GeneralHtmlExtensions.UseCommonScriptModules(this.Request.RequestContext.HttpContext, "VSS/Error", "VSS/Controls/ExternalHub", "VSS/Contributions/PageEvents");
      if (pageContext.WebContext.ProjectContext != null)
        GeneralHtmlExtensions.UseScriptModules(this.Request.RequestContext.HttpContext, "TfsCommon/Scripts/ProjectTelemetry");
      if (flag4)
      {
        GeneralHtmlExtensions.UseCSS(this.Request.RequestContext.HttpContext, "Mobile/Mobile");
        GeneralHtmlExtensions.UseScriptModules(this.Request.RequestContext.HttpContext, "TfsCommon/Scripts/MobileNavigation/Bundle");
      }
      else if (flag3)
      {
        NavigationExtensions.ContributedHeaderInit(this.Request.RequestContext);
        if (!this.TfsWebContext.IsHosted)
          GeneralHtmlExtensions.UseCommonScriptModules(this.Request.RequestContext.HttpContext, "Admin/Scripts/TFS.Admin.Registration.HostPlugins");
      }
      if (flag3 && this.TfsWebContext.Diagnostics.TracePointCollectionEnabled)
        GeneralHtmlExtensions.UseScriptModules(this.Request.RequestContext.HttpContext, "VSS/Controls/PerfBar");
      if (!flag2)
      {
        CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
        ContributionIdentifier contributionIdentifier = new ContributionIdentifier(hubContribution.Id);
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(CustomerIntelligenceProperty.Action, "Execute");
        intelligenceData.Add("PublisherName", contributionIdentifier.PublisherName);
        intelligenceData.Add("ExtensionName", contributionIdentifier.ExtensionName);
        intelligenceData.Add("ContributionType", hubContribution.Type);
        intelligenceData.Add("ContributionId", hubContribution.Id);
        intelligenceData.Add("ContributionData", this.Request.Url.PathAndQuery);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string contributions = CustomerIntelligenceArea.Contributions;
        string contributionUsage = CustomerIntelligenceFeature.ContributionUsage;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(tfsRequestContext, contributions, contributionUsage, properties);
      }
      pageContext.Navigation.UpdateResolvedRoute();
      if (flag3)
      {
        this.TfsRequestContext.Items.Add("AppsControllerPublishTimerData", (object) true);
        DataProviderResult dataProviderData = ContributionsHtmlExtensions.GetDataProviderData(pageContext, out IList<Contribution> _);
        PageXHRData data = new PageXHRData()
        {
          DataProviderData = dataProviderData,
          ContributionsData = ContributionsHtmlExtensions.GetContributionsPageData(pageContext),
          FeatureAvailability = pageContext.FeatureAvailability,
          Navigation = pageContext.Navigation,
          ServiceLocations = pageContext.ServiceLocations,
          ActivityId = this.TfsRequestContext.ActivityId,
          StaticContentVersion = pageContext.WebAccessConfiguration.Paths.StaticContentVersion
        };
        if (pageContext.Diagnostics.BundlingEnabled)
        {
          IList<BundledCSSFile> bundledCssFiles = GeneralHtmlExtensions.GetBundledCSSFiles(pageContext);
          IList<BundledScriptFile> bundledScriptFiles = GeneralHtmlExtensions.GetBundledScriptFiles(pageContext);
          IVssRequestContext deploymentContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          ICdnLocationService cdnLocationService = deploymentContext.GetService<ICdnLocationService>();
          data.Bundles = new DynamicBundlesCollection()
          {
            Scripts = bundledScriptFiles.Where<BundledScriptFile>((Func<BundledScriptFile, bool>) (s => !s.IsEmpty)).Select<BundledScriptFile, DynamicScriptBundle>((Func<BundledScriptFile, DynamicScriptBundle>) (s =>
            {
              DynamicScriptBundle resultForContribution = new DynamicScriptBundle()
              {
                ClientId = s.Name,
                ContentLength = s.ContentLength,
                Integrity = s.Integrity,
                Uri = BundlingHelper.GetLocalBundleScriptUrl(deploymentContext, s)
              };
              if (pageContext.Diagnostics.CdnEnabled && !string.IsNullOrEmpty(s.CDNRelativeUri))
              {
                string cdnUrl = cdnLocationService.GetCdnUrl(deploymentContext, s.CDNRelativeUri);
                if (!string.IsNullOrEmpty(cdnUrl))
                  resultForContribution.Uri = cdnUrl;
              }
              return resultForContribution;
            })).ToList<DynamicScriptBundle>(),
            Styles = bundledCssFiles.Where<BundledCSSFile>((Func<BundledCSSFile, bool>) (c => !c.IsEmpty)).Select<BundledCSSFile, DynamicCSSBundle>((Func<BundledCSSFile, DynamicCSSBundle>) (c =>
            {
              DynamicCSSBundle resultForContribution = new DynamicCSSBundle()
              {
                ClientId = c.Name,
                ContentLength = c.ContentLength,
                CssFiles = c.IncludedCssFiles.ToList<string>(),
                FallbackThemeUri = BundlingHelper.GetFallbackThemeLocalUri((WebContext) this.TfsWebContext, c),
                Uri = BundlingHelper.GetLocalBundleCssUrl(deploymentContext, c)
              };
              if (pageContext.Diagnostics.CdnEnabled && !string.IsNullOrEmpty(c.CDNRelativeUri))
              {
                string cdnUrl = cdnLocationService.GetCdnUrl(deploymentContext, c.CDNRelativeUri);
                if (!string.IsNullOrEmpty(cdnUrl))
                {
                  resultForContribution.Uri = cdnUrl;
                  resultForContribution.FallbackThemeUri = cdnLocationService.GetCdnUrl(deploymentContext, c.FallbackThemeCDNRelativeUri);
                }
              }
              return resultForContribution;
            })).ToList<DynamicCSSBundle>()
          };
        }
        if (pageContext.Diagnostics.TracePointCollectionEnabled)
          data.PerformanceTimings = PerformanceTimer.GetAllTimings(this.TfsRequestContext);
        return (ActionResult) new RestApiJsonResult((object) data);
      }
      this.TfsRequestContext.Items.Add("AppsControllerPublishTimerData", (object) true);
      GeneralHtmlExtensions.UseCommonScriptModules(this.Request.RequestContext.HttpContext, "VSSPreview/Flux/Components/ExternalHubShim");
      MRUNavigationContextEntryManager.UpdateMRUNavigationContextAsync(this.TfsWebContext);
      return (ActionResult) new ContributedPageActionResult(this.TfsRequestContext, (Action<IDictionary<string, object>>) (templateContext =>
      {
        HtmlHelper htmlHelper = new HtmlHelper(new ViewContext(this.ControllerContext, (IView) new AppsController.BackCompatView(), this.ViewData, this.TempData, TextWriter.Null), (IViewDataContainer) new ViewPage());
        templateContext["pageTitle"] = (object) htmlHelper.HtmlPageTitle();
        templateContext["bodyClass"] = (object) htmlHelper.BodyClasses();
      }));
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      object obj;
      bool result;
      if (((this.TfsRequestContext == null || !this.TfsRequestContext.Items.TryGetValue("AppsControllerPublishTimerData", out obj) ? 0 : (bool.TryParse(obj.ToString(), out result) ? 1 : 0)) & (result ? 1 : 0)) == 0)
        return;
      WebPerformanceTimerHelpers.SendCustomerIntelligenceData((WebContext) this.TfsWebContext);
    }

    private class BackCompatView : IView
    {
      public void Render(ViewContext viewContext, TextWriter writer)
      {
      }
    }
  }
}
