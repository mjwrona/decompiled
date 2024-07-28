// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Resources;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class GalleryAreaRegistration : AreaRegistration
  {
    private bool? m_isHosted;

    public override string AreaName => "Gallery";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptArea scriptArea = ScriptRegistration.RegisterBundledArea("Gallery", "Gallery/Scripts/Gallery/Resources", "VSS");
      scriptArea.RegisterResource("Gallery", (Func<ResourceManager>) (() => GalleryResources.ResourceManager));
      scriptArea.RegisterResource("GalleryCommon", (Func<ResourceManager>) (() => GalleryCommonResources.ResourceManager));
      this.RegisterRoutes(context.Routes);
    }

    private void RegisterRoutes(RouteCollection routes)
    {
      this.AddRoute(routes, "sitemap.xml", "Sitemap", "sitemap");
      this.AddRoute(routes, "robots.txt", "Robots");
      this.AddRoute(routes, "manage", "Publisher", "GalleryManage");
      this.AddRoute(routes, "manage/publishers/{publisherName}", "Publisher", "GalleryPublisher", new RouteValueDictionary()
      {
        {
          "publisherName",
          (object) UrlParameter.Optional
        }
      });
      this.AddRoute(routes, "manage/publishers/{publisherName}/extensions/{extensionname}/hub", "PublisherReports");
      this.AddRoute(routes, "subscribe", "subscribe");
      this.AddRoute(routes, "unsubscribe", "unsubscribe");
      this.AddRoute(routes, "vsts/{categoryName}", "Category", "GalleryVSSCategories", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vsts"
        }
      });
      this.AddRoute(routes, "vss/{categoryName}", "Category", "GalleryVSSCategoriesOld", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vsts"
        }
      });
      this.AddRoute(routes, "vscode/{categoryName}", "Category", "GalleryVSCodeCategories", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vscode"
        }
      });
      this.AddRoute(routes, "install/{itemName}", "Install");
      this.AddRoute(routes, "publish/{itemName}", "Publish");
      this.AddRoute(routes, "items", "Details");
      this.AddRoute(routes, "acquisition", "Acquisition");
      this.AddRoute(routes, "subscription", "Subscription");
      this.AddRoute(routes, "buy/{itemName}", "Install", "GalleryBuy");
      this.AddRoute(routes, "items/{itemName}/license", "Eula");
      this.AddRoute(routes, "itemsssr", "DetailsSSR");
      this.AddRoute(routes, "itemdetails", "DetailsSSR1");
      this.AddRoute(routes, "search", "Search");
      this.AddRoute(routes, "publishers/{publisherName}", "PublisherProfile", "PublisherProfile");
      this.AddRoute(routes, "manage/publishers/{publisherName}/newvsextension", "VSExtensionCreate");
      this.AddRoute(routes, "manage/publishers/{publisherName}/extensions/{extensionName}/edit", "VSExtensionEdit");
      this.AddRoute(routes, "manage/createpublisher", "PublisherCreate", "PublisherCreate");
      this.AddRoute(routes, "getextensionspercategory", "GetExtensionsPerCategory");
      this.AddRoute(routes, "avatar", "Avatar");
      this.AddRoute(routes, "gettoken", "GetToken");
      this.AddRoute(routes, "gettenants", "GetTenants");
      this.AddRoute(routes, "tenantRedirect", "TenantRedirect");
      this.AddRoute(routes, "getSubscriptionId", "GetSubscriptionId");
      this.AddRoute(routes, "getPurchaseQuantityDetails", "GetPurchaseQuantityDetails");
      this.AddRoute(routes, "publishers/{publisherName}/extensions/{extensionName}/support/reportAbuse", "Support", "Support", new RouteValueDictionary()
      {
        {
          "controller",
          (object) "CustomerSupportRequest"
        }
      });
      this.AddRoute(routes, "manage/publishers/{publisherName}/support/publisherManagement", "PublisherSupport", "PublisherSupport", new RouteValueDictionary()
      {
        {
          "controller",
          (object) "CustomerSupportRequest"
        }
      });
      this.AddRoute(routes, "manage/publishers/{publisherName}/extensions/{extensionName}/reviewer/{reviewerId}/support/appealReview", "AppealReview", "AppealReview", new RouteValueDictionary()
      {
        {
          "controller",
          (object) "CustomerSupportRequest"
        }
      });
      this.AddRoute(routes, "support/contactus", "FooterSupport", "FooterSupport", new RouteValueDictionary()
      {
        {
          "controller",
          (object) "CustomerSupportRequest"
        }
      });
      this.AddRoute(routes, "items/{itemName}", "DetailsOld");
      this.AddRoute(routes, "", "Gallery", "GalleryHomePage", new RouteValueDictionary());
      this.AddRoute(routes, "vs", "Gallery", "GalleryHomePageVS", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vs"
        }
      });
      this.AddRoute(routes, "vsformac", "Gallery", "GalleryHomePageVSForMac", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vsformac"
        }
      });
      this.AddRoute(routes, "vsts", "Gallery", "GalleryHomePageVSTS", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vsts"
        }
      });
      this.AddRoute(routes, "AzureDevOps", "Gallery", "GalleryHomePageAzureDevOps", new RouteValueDictionary()
      {
        {
          "product",
          (object) "azuredevops"
        }
      });
      this.AddRoute(routes, "vscode", "Gallery", "GalleryHomePageVSCode", new RouteValueDictionary()
      {
        {
          "product",
          (object) "vscode"
        }
      });
      this.AddRoute(routes, "subscriptions", "Gallery", "GalleryHomePageVSSubs", new RouteValueDictionary()
      {
        {
          "product",
          (object) "subscriptions"
        }
      });
      if (!this.IsHosted)
      {
        this.AddRoute(routes, "server/connect", "ConnectServer", hostType: TeamFoundationHostType.ProjectCollection);
        this.AddRoute(routes, "server/getConnectedServerContext", "GetConnectedServerContext");
        this.AddRoute(routes, "getextensionscopes", "GetExtensionScopes");
        this.AddRoute(routes, "getimportoperation", "GetImportOperation");
      }
      else
        this.AddRoute(routes, "vsgallery/{extensionId}", "VsGallery");
      this.AddRoute(routes, "items/{itemName}/changelog", "Changelog");
      this.AddRoute(routes, "items/{itemName}/privacy", "Privacy");
      this.AddRoute(routes, "_signout", "Signout");
    }

    private void AddRoute(
      RouteCollection routes,
      string routeTemplate,
      string action,
      string routeName = null,
      RouteValueDictionary defaults = null,
      TeamFoundationHostType hostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment)
    {
      if (!this.IsHosted)
        routeTemplate = "_gallery/" + routeTemplate;
      if (string.IsNullOrEmpty(routeName))
        routeName = "Gallery" + action;
      if (defaults == null)
        defaults = new RouteValueDictionary();
      if (!defaults.ContainsKey("controller"))
        defaults.Add("controller", (object) "Gallery");
      if (!defaults.ContainsKey(nameof (action)))
        defaults[nameof (action)] = (object) action;
      routes.MapTfsRoute(routeName, hostType, routeTemplate, (object) defaults);
    }

    private bool IsHosted
    {
      get
      {
        if (!this.m_isHosted.HasValue)
        {
          using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
            this.m_isHosted = new bool?(systemContext.ExecutionEnvironment.IsHostedDeployment);
        }
        return this.m_isHosted.Value;
      }
    }
  }
}
