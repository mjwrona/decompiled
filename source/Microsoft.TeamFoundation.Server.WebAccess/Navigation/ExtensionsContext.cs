// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.ExtensionsContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class ExtensionsContext : MenuBarHeaderItemContext
  {
    private static readonly Guid GalleryServiceId = new Guid("00000029-0000-8888-8000-000000000000");
    private static RegistryQuery s_marketplaceUrlRegistryQuery = new RegistryQuery("/Configuration/Service/Gallery/MarketplaceURL");
    private bool m_adminSettingsEnabled;
    private const string GalleryLocationTimeout = "/Service/Gallery/GalleryLocation/Timeout";
    private const string CircuitBreakerGetGalleryLocationCommandKey = "GetGalleryLocation";
    private const string TraceArea = "ExtensionsContext";
    private const string TraceLayer = "WebFramework";
    private static readonly int defaultGalleryLocationTimeoutMiliseconds = 1000;

    public ExtensionsContext(bool adminSettingsEnabled)
      : base(40)
    {
      this.m_adminSettingsEnabled = adminSettingsEnabled;
    }

    public override void AddServerContribution(
      HtmlHelper htmlHelper,
      IDictionary<string, IHtmlString> contributions)
    {
      MenuBar menuBar = ControlFactory.Create<MenuBar>();
      menuBar.EnhancementCssClass = (string) null;
      menuBar.CssClass<MenuBar>("bowtie-menus extensions-menu l1-menubar");
      this.PopulateMenuItem(htmlHelper.ViewContext.TfsWebContext(), menuBar.AddMenuItem());
      contributions["ms.vss-tfs-web.header-level1-extensions-menu"] = (IHtmlString) menuBar.ToHtml(htmlHelper);
    }

    public override void PopulateMenuItem(TfsWebContext webContext, MenuItem menuItem)
    {
      string str = webContext.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? WACommonResources.BrowseForExtensions : WACommonResources.BrowseTheMarketplace;
      menuItem.ItemId("l1-extensions").Icon("bowtie-icon bowtie-shop").ShowText(false).AriaLabel(str).HideDrop().AddExtraOptions((object) new
      {
        align = "right-bottom"
      }).AddMenuItem();
    }

    public override void AddActions(IVssRequestContext requestContext)
    {
      foreach (HeaderAction allAction in ExtensionsContext.GetAllActions(requestContext, this.m_adminSettingsEnabled))
        this.AddAction(allAction.Id, allAction);
    }

    public static IEnumerable<HeaderAction> GetAllActions(
      IVssRequestContext requestContext,
      bool adminSettingsEnabled)
    {
      List<HeaderAction> source = new List<HeaderAction>();
      bool premisesDeployment = requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      if (premisesDeployment)
        source.Add(ExtensionsContext.GetMarketPlaceActionForOnPrem(requestContext));
      if (!premisesDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        source.Add(ExtensionsContext.GetGalleryAction(requestContext));
      if (adminSettingsEnabled)
      {
        HeaderAction extensionsAction = ExtensionsContext.GetExtensionsAction(requestContext);
        if (extensionsAction != null && (!premisesDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection)))
          source.Add(extensionsAction);
      }
      return source.Where<HeaderAction>((Func<HeaderAction, bool>) (a => a != null));
    }

    public static HeaderAction GetGalleryAction(IVssRequestContext requestContext)
    {
      HeaderAction galleryAction = new HeaderAction();
      bool premisesDeployment = requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      galleryAction.Text = WACommonResources.BrowseTheMarketplace;
      galleryAction.TargetSelf = false;
      galleryAction.Id = "gallery";
      string stringToEscape = ExtensionsContext.GetGalleryUrl(requestContext);
      if (string.IsNullOrEmpty(stringToEscape))
        return (HeaderAction) null;
      if (!stringToEscape.EndsWith("/"))
        stringToEscape += "/";
      if (premisesDeployment)
      {
        galleryAction.TargetSelf = true;
        galleryAction.Text = WACommonResources.BrowseForExtensions;
        stringToEscape += "_gallery";
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          stringToEscape = stringToEscape + "?targetId=" + requestContext.ServiceHost.InstanceId.ToString();
      }
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) || requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        stringToEscape += string.Format("vsts?utm_source=vstsproduct&utm_medium=L1BrowseMarketplace&targetId={0}", (object) requestContext.ServiceHost.InstanceId);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableIdentityNavigation"))
        {
          string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
          if (!string.IsNullOrEmpty(locationServiceUrl))
            stringToEscape = locationServiceUrl.TrimEnd('/') + "/_redirect?target=" + Uri.EscapeDataString(stringToEscape);
        }
      }
      galleryAction.Url = stringToEscape;
      return galleryAction;
    }

    public static HeaderAction GetMarketPlaceActionForOnPrem(IVssRequestContext requestContext) => new HeaderAction()
    {
      TargetSelf = false,
      Text = WACommonResources.BrowseTheMarketplace,
      Id = "market",
      Url = ExtensionsContext.GetOnPremMarketplaceUrl(requestContext)
    };

    private static string GetOnPremMarketplaceUrl(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string str = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, in ExtensionsContext.s_marketplaceUrlRegistryQuery, "https://go.microsoft.com/fwlink/?linkid=821987");
      string token = requestContext.GetService<IConnectedServerContextKeyService>().GetToken(requestContext, (Dictionary<string, string>) null);
      return str.IndexOf("?") <= -1 ? str + "?serverKey=" + token : str + "&serverKey=" + token;
    }

    public static HeaderAction GetExtensionsAction(IVssRequestContext requestContext)
    {
      HeaderAction extensionsAction = (HeaderAction) null;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
        extensionsAction = new HeaderAction()
        {
          TargetSelf = true,
          Text = WACommonResources.ManageExtensions,
          Id = "extensions",
          Url = service.RouteUrl(requestContext, "ms.vss-extmgmt-web.manageExtensions-collection-route", new RouteValueDictionary()
          {
            {
              "tab",
              (object) "manage"
            },
            {
              "status",
              (object) "active"
            }
          })
        };
      }
      return extensionsAction;
    }

    public static string GetGallerySearchUrl(IVssRequestContext requestContext, string searchTerm)
    {
      string stringToEscape = !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? ExtensionsContext.GetGalleryUrl(requestContext) : ExtensionsContext.GetOnPremMarketplaceUrl(requestContext);
      if (string.IsNullOrEmpty(stringToEscape))
        return (string) null;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        stringToEscape += string.Format("&term={0}", (object) searchTerm);
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) || requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        if (!stringToEscape.EndsWith("/"))
          stringToEscape += "/";
        stringToEscape += string.Format("search?term={0}&target=VSTS&category=All%20categories&sortBy=Relevance&targetId={1}", (object) searchTerm, (object) requestContext.ServiceHost.InstanceId);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableIdentityNavigation"))
        {
          string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
          if (!string.IsNullOrEmpty(locationServiceUrl))
            stringToEscape = locationServiceUrl.TrimEnd('/') + "/_redirect?target=" + Uri.EscapeDataString(stringToEscape);
        }
      }
      return stringToEscape;
    }

    private static string GetGalleryUrl(IVssRequestContext requestContext)
    {
      string galleryUrl = (string) null;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) "GetGalleryLocation").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(ExtensionsContext.GetGalleryLocationTimeout(requestContext)));
      CommandService commandService = new CommandService(requestContext, setter, (Action) (() =>
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        using (vssRequestContext.CreateAsyncTimeOutScope(ExtensionsContext.GetGalleryLocationTimeout(vssRequestContext)))
          galleryUrl = vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ExtensionsContext.GalleryServiceId, AccessMappingConstants.ClientAccessMappingMoniker);
      }));
      try
      {
        commandService.Execute();
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        requestContext.Trace(100136320, TraceLevel.Error, nameof (ExtensionsContext), "WebFramework", "Gallery service is currently unreachable");
        requestContext.TraceException(100136321, TraceLevel.Error, nameof (ExtensionsContext), "WebFramework", (Exception) ex);
      }
      catch (TaskCanceledException ex)
      {
        requestContext.TraceException(100136325, TraceLevel.Error, nameof (ExtensionsContext), "WebFramework", (Exception) ex);
      }
      return galleryUrl;
    }

    private static TimeSpan GetGalleryLocationTimeout(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return TimeSpan.FromMilliseconds((double) vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Gallery/GalleryLocation/Timeout", ExtensionsContext.defaultGalleryLocationTimeoutMiliseconds));
    }
  }
}
