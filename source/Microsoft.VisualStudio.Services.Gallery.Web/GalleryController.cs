// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.VsGalleryMigration;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using Microsoft.VisualStudio.Services.Gallery.Server.Sitemap;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  [SupportedRouteArea("", NavigationContextLevels.All)]
  [LogUserInfoFilter]
  [SetUserTokenFilter]
  [IncludeCspHeader]
  [SetXFrameOptions]
  public class GalleryController : GalleryAreaController
  {
    private const int c_QueryPageSize = 100;
    private const int defaultNoOfDaysReport = 30;
    private const int DefaultNumberOfCarousels = 3;
    private const int DefaultAFDCacheExpiryTimeInMinutes = 120;
    private const string DefaultOrgPublisherNamesMapString = "Microsoft=ms;Microsoft DevLabs=ms-devlabs";
    private const string NumberOfCarousel_RegistryPath = "/Configuration/Service/Gallery/CarouselNumber";
    private const string OrgPublisherNamesMap_RegistryPath = "/Configuration/Service/Gallery/OrgPublisherNamesMap";
    private const string ServerSideRenderedItems_RegistryPath = "/Configuration/Service/Gallery/SSRItems";
    private const string SSRExcludedItems_RegistryPath = "/Configuration/Service/Gallery/SSRExcludedItems";
    private const string ServerSideRenderedCachedItems_RegistryPath = "/Configuration/Service/Gallery/SSRCachedItems";
    private const string SSR_AFDCache_ExpiryTime = "/Configuration/Service/Gallery/SSRCacheExpiryTime";
    private const string RegistryPathForReCaptchaPublicKey = "/Configuration/Service/Gallery/CaptchaPublicKey";
    private const string c_LastPublisherRegistryPath = "/Gallery/Web/Settings/LastPublisher";
    private const string c_commerceMarketplaceSubscriptionCookie = "CommerceMarketplaceSubscriptionCookie";
    private const string c_serverContext = "serverKey";
    private const string c_xamarinUniversityItemName = "ms.xamarin-university";
    private const string c_VsMigrationCookie = "ShowVsMigrationExp";
    private const string c_ShowNewAcquisitionExpCookie = "ShowNewAcquisitionExperience";
    private const string c_reportsPageCookie = "EnableReportsPage";
    private const string c_microsoft_account = "Microsoft Account";
    private const string c_serverName = "serverName";
    private const string c_detailsActionName = "Details";
    private const string c_searchActionName = "Search";
    private const string c_InstalltargetCookie = "targetId";
    private const string c_searchPage_searchTermQueryParam = "term";
    private const string c_searchPage_searchTargetQueryParam = "target";
    private const string c_searchPage_searchCategoryQueryParam = "category";
    private const string c_detailsPage_referrerQueryParam = "referrer";
    private const string c_TFS_AccountDomain = "https://dev.azure.com/";
    private const string c_product_vsts = "vsts";
    private const string c_product_azuredevops = "azuredevops";
    private const string c_ChangelogAssetTypeName = "Microsoft.VisualStudio.Services.Content.Changelog";
    private const string c_PrivacyAssetTypeName = "Microsoft.VisualStudio.Services.Content.PrivacyPolicy";
    private const string c_DetailsAssetTypeName = "Microsoft.VisualStudio.Services.Content.Details";
    private IGalleryDataProvider m_galleryDataProvider;
    private IProductExtensionsDataProvider m_productExtensionsProvider;
    private IVSDataProvider m_vsDataProvider;
    private IGalleryPageMetadataProvider m_pageMetadataProvider;
    private readonly IRemoteServiceClientFactory m_clientFactory;
    internal const string FEATURED_CATEGORY_NAME = "Featured";
    internal const string MOST_POPULAR_ITEMS_NAME = "Most Popular";
    internal const string TRENDINGWEEKLY = "TrendingWeekly";
    internal const string TRENDING_DAILY = "TrendingDaily";
    internal const string TRENDING_MONTHLY = "TrendingMonthly";
    private const string c_targetIdParameter = "targetId";
    private const string c_itemNameParameter = "itemName";
    private const string c_referrer = "referrer";
    private const string MSADirectory = "Microsoft Account";
    private const string OptInCookieForEnableItemDetailsPageServerSideRendering = "EnableItemDetailsPageServerSideRendering";
    private const string OptInCookieForEnableItemDetailsAFDCaching = "EnableItemDetailsAFDCaching";
    private const string OptInCookieForEnableItemDetailsAFDCachingForVSCode = "EnableItemDetailsAFDCachingForAllVSCode";
    private const string OptInCookieForEnableItemDetailsSSRForVsCode = "EnableItemDetailsSSRForVsCode";
    private const string OptInCookieForEnableItemDetailsSSRForVsIDE = "EnableItemDetailsSSRForVsIDE";
    private const string OptInCookieForEnableItemDetailsSSRForAzDev = "EnableItemDetailsSSRForAzDev";
    private const string OptInCookieForEnableSSRForHomepageVSIDE = "EnableSSRForHomepageVSIDE";
    private const string OptInCookieForEnableSSRForHomepageAzDev = "EnableSSRForHomepageAzDev";
    private const string OptInCookieForEnableSSRForPaidAzDev = "EnableSSRForPaidAzDev";
    private const string OptInCookieForEnableSSRForConnectedContext = "EnableSSRForConnectedContext";
    private const string OptInCookieForEnableItemDetailsPageOverviewCache = "EnableItemDetailsPageOverviewCache";
    private const string OptInCookieForEnableDelayOverviewSSR = "EnableDelayOverviewSSR";
    private const string OptInCookieForEnableSSRForPrivateExtensions = "EnableSSRForPrivateExtensions";
    private const string OptInCookieForEnableSSRForDetailsTabs = "EnableSSRForDetailsTabs";
    private const string VsProfessionalAnnualItem = "ms.vs-professional-annual";
    private const string c_afdHeader = "X-FD-REF";
    private const string c_refererHeader = "referer";
    private const string refererFromMarketplaceKey = "refererFromMarketplace";
    private const string c_redirectionPropertyName = "redirection";
    private const string VsEnterpriseAnnualItem = "ms.vs-enterprise-annual";
    public const int c_DefaultExtensionQueryPageSize = 18;
    public const string BlockTelemetryNameFormat = "{0}.{1}";
    private GalleryControllerHelper m_controllerHelper;
    private GalleryDetailsActionHelper m_detailsActionHelper;
    private GalleryImportOperationHelper m_importOperationHelper;
    private GalleryConnectServerActionHelper m_connectServerHelper;
    private DetailsSSRHelper m_detailsSsrHelper;

    public string LayerName => nameof (GalleryController);

    private int AFDCacheExpiryTime => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/SSRCacheExpiryTime", 120);

    public IPageContextProvider PageContextProvider { get; private set; }

    public ICommerceDataProvider CommerceDataProvider { get; private set; }

    public GalleryController() => this.m_clientFactory = (IRemoteServiceClientFactory) new RemoteServiceClientFactory();

    public GalleryController(IRemoteServiceClientFactory clientFactory) => this.m_clientFactory = clientFactory;

    public virtual GalleryControllerHelper ControllerHelper
    {
      get => this.m_controllerHelper ?? (this.m_controllerHelper = new GalleryControllerHelper(this));
      set => this.m_controllerHelper = value;
    }

    public virtual GalleryDetailsActionHelper DetailsHelper
    {
      get => this.m_detailsActionHelper ?? (this.m_detailsActionHelper = new GalleryDetailsActionHelper(this));
      set => this.m_detailsActionHelper = value;
    }

    public virtual DetailsSSRHelper DetailsSSRHelper
    {
      get => this.m_detailsSsrHelper ?? (this.m_detailsSsrHelper = new DetailsSSRHelper(this));
      set => this.m_detailsSsrHelper = value;
    }

    public virtual GalleryImportOperationHelper ImportHelper
    {
      get => this.m_importOperationHelper ?? (this.m_importOperationHelper = new GalleryImportOperationHelper(this));
      set => this.m_importOperationHelper = value;
    }

    public virtual GalleryConnectServerActionHelper ConnectServerHelper
    {
      get => this.m_connectServerHelper ?? (this.m_connectServerHelper = new GalleryConnectServerActionHelper(this));
      set => this.m_connectServerHelper = value;
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);
      this.ControllerHelper.InitializeTargetId();
      this.ControllerHelper.CheckAndSetConnectedContext();
      this.ViewData["CookieConsentEnabled"] = (object) false;
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCookieConsentBanner"))
      {
        this.ViewData["CookieConsentEnabled"] = (object) true;
        IDictionary<string, bool> complianceConsent = this.TfsRequestContext.GetExtension<IGalleryCookieConsentClient>().GetCookieComplianceConsent(this.HttpContext.Request);
        foreach (string key in (IEnumerable<string>) complianceConsent.Keys)
          this.ViewData[key] = (object) complianceConsent[key];
        string cookieDropScriptPath = this.ControllerHelper.GetManageCookieDropScriptPath();
        if (!string.IsNullOrWhiteSpace(cookieDropScriptPath))
          this.ViewData["CookieConsentJavaScript"] = (object) cookieDropScriptPath;
      }
      this.ViewData["EnableJsll"] = (object) false;
      TeamFoundationExecutionEnvironment executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableJsll"))
        this.ViewData["EnableJsll"] = (object) true;
      this.ViewData["EnableSurveyInfoBanner"] = (object) false;
      executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSurveyInfoBanner"))
        this.ViewData["EnableSurveyInfoBanner"] = (object) true;
      this.ViewData["EnableGTMForAllPages"] = (object) false;
      executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableGTMForAllPages"))
        this.ViewData["EnableGTMForAllPages"] = (object) true;
      this.ViewData["DisableGreenID"] = (object) false;
      executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableGreenID"))
        this.ViewData["DisableGreenID"] = (object) true;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      this.m_galleryDataProvider = vssRequestContext.GetService<IGalleryDataProvider>();
      this.m_productExtensionsProvider = this.m_galleryDataProvider.GetProductExtensionsProvider(vssRequestContext);
      this.CommerceDataProvider = this.m_galleryDataProvider.GetCommerceDataProvider(vssRequestContext);
      this.m_vsDataProvider = this.m_galleryDataProvider.GetVSDataProvider(vssRequestContext);
      this.PageContextProvider = this.m_galleryDataProvider.GetPageContextProvider(this.WebContext.RequestContext);
      this.m_pageMetadataProvider = vssRequestContext.GetService<IGalleryPageMetadataProvider>();
      this.RedirectIfHostIsDifferent();
    }

    public bool RedirectIfHostIsDifferent()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        Uri galleryUri = GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker);
        string relativeUri = this.TfsRequestContext.RelativeUrl();
        string host = this.TfsRequestContext.RequestUri().Host;
        if (galleryUri.Host != null && host != null && !host.Equals(galleryUri.Host, StringComparison.OrdinalIgnoreCase))
        {
          this.Response.RedirectPermanent(new Uri(galleryUri, relativeUri).AbsoluteUri, true);
          return true;
        }
      }
      return false;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(Duration = 604800, Location = OutputCacheLocation.Any, VaryByParam = "")]
    public ActionResult Avatar(string userId)
    {
      Guid result;
      if (!Guid.TryParse(userId, out result))
        throw new HttpException(400, GalleryResources.InvalidUserId);
      byte[] image;
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        image = vssRequestContext.GetService<IUserService>().GetAvatar(vssRequestContext, result).Image;
      }
      catch (Exception ex) when (ex is UserDoesNotExistException || ex is IdentityNotFoundException)
      {
        this.TfsRequestContext.TraceException(12062063, this.AreaName, this.LayerName, ex);
        throw new HttpException(404, GalleryResources.UserAvatarDoesNotExist);
      }
      return (ActionResult) this.File(image, "image/png");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Subscribe() => (ActionResult) this.RedirectPermanent(GalleryHtmlExtensions.GetGalleryAbsoluteUrl(this.TfsRequestContext, "unsubscribe"));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Unsubscribe()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, GalleryResources.OnPremUnsupportedText);
      try
      {
        this.ViewData["userSettings"] = (object) this.TfsRequestContext.GetService<IGalleryUserSettingsService>().GetGalleryUserSettings(this.TfsRequestContext, "me", (string) null);
        this.SetMailAddress();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(12061059, this.AreaName, this.LayerName, ex);
        throw;
      }
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (Unsubscribe));
    }

    private void SetMailAddress()
    {
      IMailNotification mailNotification = (IMailNotification) new MailNotification();
      Guid userId = this.TfsRequestContext.GetUserId();
      if (userId.Equals(Guid.Empty))
        return;
      this.ViewData["UserMailAddress"] = (object) mailNotification.GetUserMailAddress(this.TfsRequestContext, userId);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Signout(string redirectUrl = null) => (ActionResult) this.Redirect(GalleryHtmlExtensions.SignoutUrl(this.TfsRequestContext, redirectUrl));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => this.Publisher();

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PublisherProfile(string publisherName)
    {
      if (string.IsNullOrEmpty(publisherName) || this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment || !this.GetCookieOrFeatureFlagStatus("EnablePublisherProfilePage", "Microsoft.VisualStudio.Services.Gallery.EnablePublisherProfilePage"))
        throw new HttpException(404, WACommonResources.PageNotFound);
      IDictionary<string, string> publisherNamesMap = this.GetOrgPublisherNamesMap();
      string primaryPublisherName;
      if (publisherNamesMap.TryGetValue(publisherName, out primaryPublisherName))
        return this.GetPublisherOrgProfilePage(publisherName, primaryPublisherName);
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      try
      {
        publisher = this.TfsRequestContext.GetService<IPublisherService>().QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.None, true);
        if (publisherNamesMap.TryGetValue(publisher.DisplayName, out primaryPublisherName))
        {
          this.Response.RedirectPermanent(this.GetPublisherProfileRedirectUrl(publisher.DisplayName), true);
        }
        else
        {
          this.PopulateExtensionsResultInPublisherProfileViewData(publisher, false);
          this.ViewData["publisher-details"] = (object) publisher;
          this.ViewData["publisher-profile-query-with-display-name"] = (object) false;
          this.ViewData["publisher-profile-page-size"] = (object) 18;
        }
      }
      catch (PublisherDoesNotExistException ex)
      {
        throw new HttpException(404, WACommonResources.PageNotFound);
      }
      this.PopulateCommonPublisherProfilePageViewData(publisher);
      return (ActionResult) this.View(nameof (PublisherProfile));
    }

    private ActionResult GetPublisherOrgProfilePage(
      string publisherOrgName,
      string primaryPublisherName)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      try
      {
        publisher = this.TfsRequestContext.GetService<IPublisherService>().QueryPublisher(this.TfsRequestContext, primaryPublisherName, PublisherQueryFlags.None, true);
        this.PopulateExtensionsResultInPublisherProfileViewData(publisher, true);
        this.ViewData["publisher-details"] = (object) publisher;
        this.ViewData["publisher-profile-query-with-display-name"] = (object) true;
        this.ViewData["publisher-profile-page-size"] = (object) 18;
      }
      catch (PublisherDoesNotExistException ex)
      {
        throw new HttpException(404, WACommonResources.PageNotFound);
      }
      this.PopulateCommonPublisherProfilePageViewData(publisher);
      return (ActionResult) this.View("PublisherProfile");
    }

    private void PopulateExtensionsResultInPublisherProfileViewData(
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisherSelected,
      bool usePublisherDisplayName)
    {
      this.ViewData["vs-extensions-result"] = (object) this.ControllerHelper.GetExtensionsResultByPublisherForProduct(publisherSelected, "vs", usePublisherDisplayName);
      this.ViewData["vscode-extensions-result"] = (object) this.ControllerHelper.GetExtensionsResultByPublisherForProduct(publisherSelected, "vscode", usePublisherDisplayName);
      this.ViewData["vsts-extensions-result"] = (object) this.ControllerHelper.GetExtensionsResultByPublisherForProduct(publisherSelected, "vsts", usePublisherDisplayName);
      this.ViewData["vsformac-extensions-result"] = (object) this.ControllerHelper.GetExtensionsResultByPublisherForProduct(publisherSelected, "vsformac", usePublisherDisplayName);
    }

    private void PopulateCommonPublisherProfilePageViewData(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      this.ControllerHelper.PopulateGeneralInfo();
      this.ControllerHelper.LoadPageMetadata(GalleryPages.PublisherProfile, (PublishedExtension) null, (string) null, (string) null, (string) null, publisher);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Publisher(string publisherName = null) => this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? this.OnPremPublisherRevamp(publisherName) : this.PublisherRevamp(publisherName);

    private ActionResult PublisherRevamp(string publisherName)
    {
      IPublisherService service = this.TfsRequestContext.GetService<IPublisherService>();
      if (!string.IsNullOrEmpty(publisherName))
      {
        try
        {
          Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisherSelected = service.QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.IncludeExtensions);
          GalleryPublisherViewData publisherViewData = this.GetPublisherViewData(publisherSelected, service);
          if (!publisherViewData.HasPublisherPrivateReadPermission && !publisherViewData.HasViewPublisherPermissionsPermission && !GallerySecurity.IsPartOfGalleryAdminSecurityGroup(this.TfsRequestContext))
          {
            PublisherFoundInTenantResult publisherInTenant = this.FindPublisherInTenant(publisherViewData?.PublisherTenants, publisherName);
            if (publisherInTenant == null)
              throw new HttpException(403, GalleryResources.ForbiddenMessage);
            return (ActionResult) this.Redirect(this.ControllerHelper.GetAADSignInRedirectUrl(publisherInTenant.Tenant.Id == Guid.Empty.ToString() ? "live.com" : publisherInTenant.Tenant.Id, new Uri(this.GetManagePagePublisherRedirectUrl(publisherInTenant.Publisher.PublisherName))));
          }
          if (publisherSelected.Extensions != null && publisherSelected.Extensions.Count > 0)
          {
            this.TfsRequestContext.Trace(12060033, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Before Filter | Selected publisher:{0}, Extension count: {1}, Extension Names:{2}", (object) publisherSelected.PublisherName, (object) publisherSelected.Extensions.Count, (object) this.GetExtensionNames(publisherSelected.Extensions)));
            this.FillPublisherExtensionsDetails(publisherSelected);
            this.TfsRequestContext.Trace(12060033, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("After Filter | Selected publisher:{0}, Extension count: {1}, Extension Names:{2}", (object) publisherSelected.PublisherName, (object) publisherSelected.Extensions.Count, (object) this.GetExtensionNames(publisherSelected.Extensions)));
          }
          this.RecordLastPublisherUsed(publisherName);
          this.ViewData["GalleryPublisherViewData"] = (object) publisherViewData;
          this.ViewData["ReservedPublisherDisplayNames"] = (object) service.GetReservedPublisherDisplayName();
          this.SetMailAddress();
        }
        catch (PublisherDoesNotExistException ex)
        {
          throw new HttpException(404, WACommonResources.PageNotFound);
        }
      }
      else
      {
        List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher> myPublishersList1 = this.GetMyPublishersList(service);
        if (myPublishersList1 != null && myPublishersList1.Count > 0)
        {
          string publisherName1 = myPublishersList1[0].PublisherName;
          string lastPublisher = this.GetLastPublisherUsed();
          if (!string.IsNullOrEmpty(lastPublisher) && myPublishersList1.Exists((Predicate<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher>) (p => string.Equals(p.PublisherName, lastPublisher, StringComparison.OrdinalIgnoreCase))))
            publisherName1 = lastPublisher;
          return (ActionResult) this.Redirect(this.GetManagePagePublisherRedirectUrl(publisherName1));
        }
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
        service.LinkPendingProfile(this.TfsRequestContext, userIdentity);
        List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher> myPublishersList2 = this.GetMyPublishersList(service);
        if (myPublishersList2 != null && myPublishersList2.Count > 0)
          return (ActionResult) this.Redirect(this.GetManagePagePublisherRedirectUrl(myPublishersList2[0].PublisherName));
        GalleryPublisherViewData publisherViewData = this.GetPublisherViewData((Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null, service);
        if (this.FindPublisherInTenant(publisherViewData?.PublisherTenants) == null)
          return (ActionResult) this.Redirect(this.GetPublisherCreateRedirectUrl());
        this.ViewData["GalleryPublisherViewData"] = (object) publisherViewData;
      }
      this.ViewData["ReCaptchaPublicKey"] = (object) this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
      this.ViewData["gallery-browse-url"] = (object) this.ControllerHelper.GetGalleryHostName();
      this.ViewData["target-platforms"] = (object) this.PopulateTargetPlatforms();
      this.ControllerHelper.PopulateGeneralInfo();
      this.ViewData["delete-prevent-min-acquisition-count"] = (object) GalleryServerUtil.GetDeletePreventMinAcquisitionCount(this.TfsRequestContext);
      this.ViewData["delete-prevent-min-days-count"] = (object) GalleryServerUtil.GetDeletePreventMinDaysCount(this.TfsRequestContext);
      return (ActionResult) this.View("Publisher");
    }

    private ActionResult OnPremPublisherRevamp(string publisherName)
    {
      IPublisherService service = this.TfsRequestContext.GetService<IPublisherService>();
      GalleryPublisherViewData publisherViewData1 = new GalleryPublisherViewData();
      GalleryPublisherViewData publisherViewData2;
      if (publisherName != null)
      {
        try
        {
          Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisherSelected = service.QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.IncludeExtensions);
          publisherViewData2 = this.GetPublisherViewData(publisherSelected, service);
          if (publisherSelected.Extensions != null && publisherSelected.Extensions.Count > 0)
          {
            this.TfsRequestContext.Trace(12060033, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Before Filter | Selected publisher:{0}, Extension count: {1}, Extension Names:{2}", (object) publisherSelected.PublisherName, (object) publisherSelected.Extensions.Count, (object) this.GetExtensionNames(publisherSelected.Extensions)));
            this.FillPublisherExtensionsDetails(publisherSelected);
            this.TfsRequestContext.Trace(12060033, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("After Filter | Selected publisher:{0}, Extension count: {1}, Extension Names:{2}", (object) publisherSelected.PublisherName, (object) publisherSelected.Extensions.Count, (object) this.GetExtensionNames(publisherSelected.Extensions)));
          }
          this.ViewData["GalleryPublisherViewData"] = (object) publisherViewData2;
        }
        catch (PublisherDoesNotExistException ex)
        {
          throw new HttpException(404, WACommonResources.PageNotFound);
        }
      }
      else
        publisherViewData2 = this.GetPublisherViewData((Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null, service);
      this.ViewData["GalleryPublisherViewData"] = (object) publisherViewData2;
      this.PopulateValidCategories("vsts");
      this.ViewData["gallery-browse-url"] = (object) this.ControllerHelper.GetGalleryHostName();
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View("Publisher");
    }

    private PublisherFoundInTenantResult FindPublisherInTenant(
      List<PublisherTenant> tenants,
      string publisherName = null)
    {
      if (tenants != null && tenants.Count > 0)
      {
        foreach (PublisherTenant tenant in tenants)
        {
          if (tenant.Publishers != null && tenant.Publishers.Count > 0)
          {
            if (string.IsNullOrWhiteSpace(publisherName))
              return new PublisherFoundInTenantResult()
              {
                Tenant = tenant,
                Publisher = tenant.Publishers.First<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher>()
              };
            foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher in tenant.Publishers)
            {
              if (publisher.PublisherName.Equals(publisherName, StringComparison.OrdinalIgnoreCase))
                return new PublisherFoundInTenantResult()
                {
                  Tenant = tenant,
                  Publisher = publisher
                };
            }
          }
        }
      }
      return (PublisherFoundInTenantResult) null;
    }

    private GalleryPublisherViewData GetPublisherViewData(
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisherSelected,
      IPublisherService publisherService)
    {
      IGalleryDomainInfo extension = this.TfsRequestContext.GetExtension<IGalleryDomainInfo>();
      string str1 = "";
      string str2 = "";
      Guid guid = Guid.Empty;
      int num = this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0;
      List<string> stringList = (List<string>) null;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      if (num != 0)
      {
        if (extension != null)
        {
          GalleryUserDomainInfo domainInfo = extension.GetDomainInfo(this.TfsRequestContext);
          str1 = domainInfo.UserAddress;
          str2 = domainInfo.UserDomain;
          guid = domainInfo.UserDomainId;
        }
        if (publisherSelected != null)
        {
          stringList = this.GetMigratedExtensionList(publisherSelected?.Extensions);
          flag1 = GallerySecurity.HasPublisherPermission(this.TfsRequestContext, publisherSelected, PublisherPermissions.PrivateRead);
          flag3 = GallerySecurity.HasPublisherPermission(this.TfsRequestContext, publisherSelected, PublisherPermissions.EditSettings);
          flag2 = GallerySecurity.HasPublisherPermission(this.TfsRequestContext, publisherSelected, PublisherPermissions.ViewPermissions);
          flag4 = GallerySecurity.HasPublisherPermission(this.TfsRequestContext, publisherSelected, PublisherPermissions.PublishExtension);
        }
        return new GalleryPublisherViewData()
        {
          PublisherSelected = publisherSelected,
          PublisherTenants = this.GetMyPublishersListAccrossDomains(this.TfsRequestContext, publisherService),
          MigratedExtensions = stringList,
          AccountsDomain = this.GetTfsAccountsDomain(),
          UserDomain = str2,
          UserDomainId = guid,
          UserAddress = str1,
          HasPublisherPrivateReadPermission = flag1,
          HasViewPublisherPermissionsPermission = flag2,
          HasEditPublisherSettingsPermission = flag3,
          HasPublisherPublishExtensionPermission = flag4
        };
      }
      return new GalleryPublisherViewData()
      {
        PublisherSelected = publisherSelected,
        PublisherTenants = (List<PublisherTenant>) null,
        MigratedExtensions = stringList,
        AccountsDomain = this.GetTfsAccountsDomain(),
        UserDomain = str2,
        UserDomainId = guid,
        UserAddress = str1,
        HasCreatePublisherPermission = GallerySecurity.HasRootPermission(this.TfsRequestContext, PublisherPermissions.CreatePublisher),
        HasCertificateDownloadPermission = GallerySecurity.HasRootPermission(this.TfsRequestContext, PublisherPermissions.UpdateExtension)
      };
    }

    private string GetExtensionNames(List<PublishedExtension> extensions)
    {
      string extensionNames = "";
      foreach (PublishedExtension extension in extensions)
        extensionNames = extensionNames + extension.ExtensionName + ";";
      return extensionNames;
    }

    private void RecordLastPublisherUsed(string publisherName) => this.TfsRequestContext.GetService<IVssRegistryService>().WriteEntries(this.TfsRequestContext, this.TfsRequestContext.GetUserIdentity(), (IEnumerable<RegistryEntry>) new RegistryEntry[1]
    {
      new RegistryEntry("/Gallery/Web/Settings/LastPublisher", publisherName)
    });

    private string GetLastPublisherUsed()
    {
      RegistryEntryCollection source = this.TfsRequestContext.GetService<IVssRegistryService>().ReadEntries(this.TfsRequestContext, this.TfsRequestContext.GetUserIdentity(), "/Gallery/Web/Settings/LastPublisher");
      return (source != null ? source.FirstOrDefault<RegistryEntry>() : (RegistryEntry) null)?.Value;
    }

    private IDictionary<string, string> GetOrgPublisherNamesMap()
    {
      string str1 = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/OrgPublisherNamesMap", "Microsoft=ms;Microsoft DevLabs=ms-devlabs");
      IDictionary<string, string> publisherNamesMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!string.IsNullOrEmpty(str1))
      {
        string str2 = str1;
        char[] chArray1 = new char[1]{ ';' };
        foreach (string str3 in str2.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ '=' };
          string[] strArray = str3.Split(chArray2);
          publisherNamesMap.Add(strArray[0], strArray[1]);
        }
      }
      return publisherNamesMap;
    }

    private int SetDefaultCarouselNumber() => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CarouselNumber", 3);

    private string GetTfsAccountsDomain()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return string.Empty;
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseNewDomainUrlInShareDropdown"))
      {
        string tfsAccountsDomain = (string) null;
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        ILocationDataProvider locationData = vssRequestContext.GetService<ILocationService>().GetLocationData(vssRequestContext, ServiceInstanceTypes.SPS);
        if (locationData != null)
        {
          ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(vssRequestContext, "VsService", ServiceInstanceTypes.TFS);
          if (serviceDefinition != null)
            tfsAccountsDomain = serviceDefinition.GetLocationMapping(AccessMappingConstants.ServicePathMappingMoniker)?.Location;
        }
        if (tfsAccountsDomain == null)
        {
          this.TfsRequestContext.Trace(12062038, TraceLevel.Warning, this.AreaName, this.LayerName, "BaseUri Returned as null");
          tfsAccountsDomain = "https://dev.azure.com/";
        }
        return tfsAccountsDomain;
      }
      Uri serviceBaseUri = LocationServiceHelper.GetServiceBaseUri(this.TfsRequestContext, ServiceInstanceTypes.TFS, false);
      if ((object) serviceBaseUri == null)
        serviceBaseUri = LocationServiceHelper.GetServiceBaseUri(this.TfsRequestContext);
      return serviceBaseUri?.Host;
    }

    private List<string> GetMigratedExtensionList(List<PublishedExtension> extensions) => extensions != null ? extensions.Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => extension.IsMigratedFromVSGallery())).Select<PublishedExtension, string>((Func<PublishedExtension, string>) (extension => extension.GetFullyQualifiedName())).ToList<string>() : (List<string>) null;

    private void FillPublisherExtensionsDetails(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisherSelected)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      List<FilterCriteria> list = publisherSelected.Extensions.Select<PublishedExtension, FilterCriteria>((Func<PublishedExtension, FilterCriteria>) (extension => new FilterCriteria()
      {
        FilterType = 4,
        Value = extension.ExtensionId.ToString()
      })).ToList<FilterCriteria>();
      ExtensionQuery extensionQuery1 = new ExtensionQuery()
      {
        Flags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeSharedOrganizations,
        Filters = new List<QueryFilter>(1)
        {
          new QueryFilter() { Criteria = list, DoNotUseCache = true }
        },
        AssetTypes = new List<string>()
        {
          "Microsoft.VisualStudio.Services.Icons.Default"
        }
      };
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableForcePageSizeOnManagePublisherPage"))
      {
        extensionQuery1.Filters[0].ForcePageSize = true;
        extensionQuery1.Filters[0].PageSize = 2000;
      }
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ExtensionQuery extensionQuery2 = extensionQuery1;
      ExtensionQueryResult extensionQueryResult = service.QueryExtensions(tfsRequestContext, extensionQuery2, (string) null, true);
      if (extensionQueryResult.Results == null || extensionQueryResult.Results.Count <= 0)
        return;
      publisherSelected.Extensions = extensionQueryResult.Results[0].Extensions;
    }

    private List<PublisherTenant> GetMyPublishersListAccrossDomains(
      IVssRequestContext requestContext,
      IPublisherService publisherService,
      int pageSize = 100)
    {
      using (requestContext.TraceBlock(12062038, 12062038, this.AreaName, this.LayerName, nameof (GetMyPublishersListAccrossDomains)))
      {
        Dictionary<Guid, Tenant> vsiDsAndTenants = this.GetVSIDsAndTenants(requestContext);
        List<QueryFilter> queryFilterList = new List<QueryFilter>();
        List<Guid> guidList = new List<Guid>((IEnumerable<Guid>) vsiDsAndTenants.Keys.ToArray<Guid>());
        guidList.ForEach((Action<Guid>) (vsid => queryFilterList.Add(new QueryFilter()
        {
          Criteria = new List<FilterCriteria>(1)
          {
            new FilterCriteria()
            {
              FilterType = 3,
              Value = vsid.ToString()
            }
          },
          Direction = PagingDirection.Forward,
          PageSize = pageSize
        })));
        PublisherQueryResult publisherQueryResult = publisherService.QueryPublishers(this.TfsRequestContext, new PublisherQuery()
        {
          Filters = queryFilterList,
          Flags = PublisherQueryFlags.None
        }, true);
        if (publisherQueryResult.Results == null || publisherQueryResult.Results.Count != guidList.Count)
        {
          this.TfsRequestContext.Trace(12062038, TraceLevel.Warning, this.AreaName, this.LayerName, string.Format("Query results count ({0}) is not equals vsid count ({1})", (object) (publisherQueryResult.Results == null ? 0 : publisherQueryResult.Results.Count), (object) guidList.Count));
          return (List<PublisherTenant>) null;
        }
        List<PublisherTenant> listAccrossDomains = new List<PublisherTenant>();
        for (int index = 0; index < guidList.Count; ++index)
        {
          Guid guid = guidList[index];
          Tenant tenant = vsiDsAndTenants[guid];
          List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher> publishers = publisherQueryResult.Results[index].Publishers;
          Dictionary<string, PublisherRoleData> dictionary = new Dictionary<string, PublisherRoleData>();
          if (publishers != null)
          {
            foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher in publishers)
            {
              SecurityRole roleForGivenVsid = GallerySecurity.GetPublisherRoleForGivenVSID(requestContext, publisher, guid);
              dictionary[publisher.PublisherName] = new PublisherRoleData()
              {
                Name = roleForGivenVsid != null ? roleForGivenVsid.DisplayName : "Other",
                Permission = roleForGivenVsid != null ? roleForGivenVsid.AllowPermissions : 1
              };
            }
          }
          listAccrossDomains.Add(new PublisherTenant()
          {
            Id = tenant.Id.ToString(),
            Name = tenant.Name,
            Publishers = publishers,
            PublisherRoles = dictionary
          });
        }
        return listAccrossDomains;
      }
    }

    public Dictionary<Guid, Tenant> GetVSIDsAndTenants(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(12062038, 12062038, this.AreaName, this.LayerName, nameof (GetVSIDsAndTenants)))
      {
        if (requestContext == null)
          throw new ArgumentNullException(nameof (requestContext));
        List<AadTenant> tenants = new List<AadTenant>();
        Dictionary<Guid, Tenant> tenantsDict = new Dictionary<Guid, Tenant>();
        Dictionary<Guid, Tenant> retTenants = new Dictionary<Guid, Tenant>();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        try
        {
          IVssRequestContext context1 = requestContext.Elevate();
          AadService service1 = context1.GetService<AadService>();
          IVssRequestContext context2 = context1;
          GetTenantsRequest request = new GetTenantsRequest();
          request.ToMicrosoftServicesTenant = true;
          tenants = service1.GetTenants(context2, request).Tenants.ToList<AadTenant>();
          if (tenants != null)
          {
            if (tenants.Count<AadTenant>() > 0)
            {
              tenants = tenants.Where<AadTenant>((Func<AadTenant, bool>) (x => x != null)).ToList<AadTenant>();
              tenants.ForEach((Action<AadTenant>) (tenant => tenantsDict[tenant.ObjectId] = new Tenant()
              {
                Id = tenant.ObjectId,
                Name = tenant.DisplayName
              }));
              IdentityService service2 = requestContext.GetService<IdentityService>();
              string mailAddress = userIdentity?.GetProperty<string>("Mail", string.Empty);
              IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>();
              if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseIdentityDescriptorsToReadIdentities"))
              {
                if (tenants.Count >= 1)
                {
                  List<IdentityDescriptor> list = tenants.Select<AadTenant, IdentityDescriptor>((Func<AadTenant, IdentityDescriptor>) (x => new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}\\{1}", (object) x.ObjectId, (object) mailAddress)))).ToList<IdentityDescriptor>();
                  collection = service2.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null);
                }
                else if (userIdentity != null)
                {
                  string property = userIdentity.GetProperty<string>("Domain", string.Empty);
                  Guid empty = Guid.Empty;
                  ref Guid local = ref empty;
                  if (Guid.TryParse(property, out local))
                  {
                    List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
                    {
                      new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}\\{1}", (object) empty, (object) mailAddress))
                    };
                    collection = service2.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null);
                  }
                }
              }
              else
                collection = service2.ReadIdentities(requestContext, IdentitySearchFilter.MailAddress, mailAddress, QueryMembership.None, (IEnumerable<string>) null);
              if (collection != null)
                collection.ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (identity =>
                {
                  if (identity == null)
                    return;
                  string property = identity.GetProperty<string>("Domain", string.Empty);
                  if ("Windows Live ID".Equals(property, StringComparison.OrdinalIgnoreCase))
                  {
                    retTenants[identity.Id] = new Tenant()
                    {
                      Id = Guid.Empty,
                      Name = "Microsoft Account"
                    };
                  }
                  else
                  {
                    Guid result;
                    if (Guid.TryParse(property, out result) && tenantsDict.ContainsKey(result))
                      retTenants[identity.Id] = tenantsDict[result];
                    else
                      this.TfsRequestContext.Trace(12062038, TraceLevel.Warning, this.AreaName, this.LayerName, string.Format("This should not happen. No tenant found for VSID = {0} and domain = {1}. List of tenants = ({2})", (object) identity.Id, (object) property, (object) string.Join<char>(",", tenants.SelectMany<AadTenant, char>((Func<AadTenant, IEnumerable<char>>) (t => (IEnumerable<char>) t.ObjectId.ToString())))));
                  }
                }));
            }
          }
        }
        catch (AadException ex)
        {
          this.TfsRequestContext.TraceException(12062038, this.AreaName, this.LayerName, (Exception) ex);
        }
        catch (VssServiceResponseException ex)
        {
          this.TfsRequestContext.TraceException(12061131, this.AreaName, this.LayerName, (Exception) ex);
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(12062038, this.AreaName, this.LayerName, ex);
          throw;
        }
        this.TfsRequestContext.Trace(12062038, TraceLevel.Info, this.AreaName, this.LayerName, "Continuing after try block");
        this.TfsRequestContext.Trace(12062038, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("retTenants.Count is {0} and authenticatedIdentity == null is {1}", (object) retTenants.Count, (object) (userIdentity == null)));
        if (retTenants.Count == 0 && userIdentity != null)
        {
          string str = "";
          if (tenants != null && tenants.Count<AadTenant>() > 0)
            str = string.Join(",", tenants.Select<AadTenant, string>((Func<AadTenant, string>) (x => x.ObjectId.ToString())));
          this.TfsRequestContext.Trace(12062038, TraceLevel.Error, this.AreaName, this.LayerName, "No VSID and tenants found for the user. Current VSID = " + userIdentity.Id.ToString() + " List of tenants = (" + str + ")");
          string property = userIdentity.GetProperty<string>("Domain", string.Empty);
          Guid result;
          if (Guid.TryParse(property, out result))
          {
            if (tenantsDict.ContainsKey(result))
            {
              retTenants[userIdentity.Id] = tenantsDict[result];
            }
            else
            {
              this.TfsRequestContext.Trace(12062038, TraceLevel.Error, this.AreaName, this.LayerName, "Domain id " + property + " is not found in list of tenants = (" + str + ")");
              retTenants[userIdentity.Id] = new Tenant()
              {
                Id = result,
                Name = string.Empty
              };
            }
          }
          else if ("Windows Live ID".Equals(property, StringComparison.OrdinalIgnoreCase))
          {
            retTenants[userIdentity.Id] = new Tenant()
            {
              Id = Guid.Empty,
              Name = "Microsoft Account"
            };
          }
          else
          {
            this.TfsRequestContext.Trace(12062038, TraceLevel.Error, this.AreaName, this.LayerName, "Domain id " + property + " is not GUID");
            retTenants[userIdentity.Id] = new Tenant()
            {
              Id = Guid.Empty,
              Name = property
            };
          }
        }
        else if (userIdentity == null)
          this.TfsRequestContext.Trace(12062038, TraceLevel.Error, this.AreaName, this.LayerName, "Couldn't detect the logged in identity.");
        return retTenants;
      }
    }

    private List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher> GetMyPublishersList(
      IPublisherService publisherService,
      int pageSize = 100)
    {
      List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>(1)
      {
        new FilterCriteria() { FilterType = 3 }
      };
      PublisherQueryResult publisherQueryResult = publisherService.QueryPublishers(this.TfsRequestContext, new PublisherQuery()
      {
        Filters = new List<QueryFilter>(1)
        {
          new QueryFilter()
          {
            Criteria = filterCriteriaList,
            Direction = PagingDirection.Forward,
            PageSize = pageSize
          }
        },
        Flags = PublisherQueryFlags.None
      });
      return publisherQueryResult.Results == null || publisherQueryResult.Results.Count == 0 ? (List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher>) null : publisherQueryResult.Results[0].Publishers;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Gallery(string product = "")
    {
      int numCarousel = this.SetDefaultCarouselNumber();
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseNewBranding"))
      {
        if (string.Compare(product, "vsts", StringComparison.OrdinalIgnoreCase) == 0)
        {
          NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
          return (ActionResult) this.RedirectPermanent(new UriBuilder(new Uri(GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker), string.Format("/{0}", (object) "azuredevops", (object) StringComparison.InvariantCultureIgnoreCase)))
          {
            Query = queryString.ToString()
          }.ToString());
        }
        if (string.Compare(product, "azuredevops", StringComparison.OrdinalIgnoreCase) == 0)
          product = "vsts";
      }
      this.ControllerHelper.LoadPageMetadata(GalleryPages.HomePage, (PublishedExtension) null, product, (string) null, (string) null);
      this.PopulateViewDataKO(product, numCarousel);
      return (ActionResult) this.View("HomePageVNext");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult VSSSearch()
    {
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View("Search");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult VSSearch()
    {
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View("Search");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult DetailsOld(string itemName) => (ActionResult) this.RedirectPermanent(this.GetDetailsPageUrl(itemName));

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Sitemap()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotFound);
      int fileIdOfSitemap = this.TfsRequestContext.GetService<ISitemapService>().GetFileIdOfSitemap(this.TfsRequestContext);
      return fileIdOfSitemap == 0 ? (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotFound) : (ActionResult) new FileStreamResult(this.TfsRequestContext.GetService<IStorageService>().RetrieveFile(this.TfsRequestContext, fileIdOfSitemap), "application/xml");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Robots()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotFound);
      StringBuilder stringBuilder = new StringBuilder();
      if (this.TfsRequestContext.RequestUri().Scheme == Uri.UriSchemeHttps && this.TfsRequestContext.RequestUri().Host.Equals("marketplace.visualstudio.com", StringComparison.OrdinalIgnoreCase))
      {
        stringBuilder.AppendLine("User-agent: *");
        stringBuilder.AppendLine("Disallow: /_apis/");
        stringBuilder.AppendLine("Disallow: /publishers/");
        stringBuilder.AppendLine();
        stringBuilder.Append("sitemap: ");
        stringBuilder.AppendLine(this.Url.RouteUrl("sitemap", (object) null, this.Request.Url.Scheme).TrimEnd('/'));
      }
      else
      {
        stringBuilder.AppendLine("user-agent: *");
        stringBuilder.AppendLine("disallow: /");
      }
      return (ActionResult) this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Acquisition(
      string itemName,
      bool auth_redirect = false,
      string installContext = null,
      string targetId = null)
    {
      bool isAuthenticated = this.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      PublishedExtensionResult extensionResult = new PublishedExtensionResult();
      ActionResult actionResult1 = this.CheckCommonSteps(itemName, out extensionResult);
      if (actionResult1 != null)
        return actionResult1;
      if ((extensionResult == null || extensionResult.Extension == null) && (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment || installContext == null))
        throw new HttpException(404, GalleryResources.PageNotFoundError);
      PublishedExtension extension = extensionResult.Extension;
      HttpCookie cookie = this.Request?.Cookies?[nameof (targetId)];
      if (cookie != null && !string.IsNullOrEmpty(cookie.Value) && targetId == null)
        targetId = cookie.Value;
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string redirectionUrl = (string) null;
        this.LoadAccountsData(extension, targetId, isAuthenticated, installContext, ref redirectionUrl);
        if (!string.IsNullOrEmpty(redirectionUrl))
          return (ActionResult) this.Redirect(redirectionUrl);
      }
      ActionResult actionResult2 = this.LoadCommonData(extensionResult, itemName, true, auth_redirect, installContext);
      if (actionResult2 != null)
        return actionResult2;
      this.DetailsHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (Acquisition));
    }

    public virtual bool LoadAccountsData(
      PublishedExtension extension,
      string accountId,
      bool isAuthenticated,
      string installContext,
      ref string redirectionUrl)
    {
      bool flag1 = false;
      bool flag2 = this.ViewData.ContainsKey("server-context");
      if (extension != null && (!flag2 && GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || GalleryUtil.IsHostedResourceInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets)))
      {
        GalleryDetailsActionHelper.AccountContext accountsData = this.DetailsHelper.GetAccountsData(extension, accountId);
        ISubscriptionAccount selectedAccount = accountsData.selectedAccount;
        IDictionary<string, AcquisitionOptions> dictionary = (IDictionary<string, AcquisitionOptions>) new Dictionary<string, AcquisitionOptions>();
        if (selectedAccount != null)
        {
          if (this.IsRedirectionRequired(extension, selectedAccount, ref redirectionUrl))
            return false;
          AcquisitionOptions andAccountDetails = this.DetailsHelper.GetDefaultOptionsAndAccountDetails(extension, ref selectedAccount);
          if (andAccountDetails != null)
            dictionary[selectedAccount.AccountId.ToString()] = andAccountDetails;
        }
        if (accountsData != null)
        {
          this.ViewData["accounts"] = (object) accountsData.accounts;
          this.ViewData["selected-account"] = (object) selectedAccount;
          this.ViewData["default-options"] = (object) dictionary;
          flag1 = true;
        }
      }
      return flag1;
    }

    private bool IsRedirectionRequired(
      PublishedExtension extension,
      ISubscriptionAccount selectedAccount,
      ref string redirectionUrl)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      string property = userIdentity.GetProperty<string>("Domain", string.Empty);
      Guid result;
      if (selectedAccount.AccountTenantId != Guid.Empty && (Guid.TryParse(property, out result) && result != selectedAccount.AccountTenantId || !Guid.TryParse(property, out Guid _)))
      {
        string journeyId = this.GetJourneyId();
        this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("LoadAccountData account = {0}, currentId= {1}, userDomain ={2}, accountDomain = {3},  journeyId={4}", (object) selectedAccount.AccountId, (object) userIdentity.Id, (object) property, (object) selectedAccount.AccountTenantId, (object) journeyId));
        TokenRequestResult tokenRequestResult = this.FetchTokenResult(extension.GetFullyQualifiedName(), selectedAccount);
        if (tokenRequestResult != null && tokenRequestResult.requiresRedirection)
        {
          this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("LoadAccountData RedirectionUri = {0} journeyId={1}", (object) tokenRequestResult.redirectionUri, (object) journeyId));
          redirectionUrl = tokenRequestResult.redirectionUri;
          return true;
        }
      }
      return false;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult DetailsSSR1(string itemName)
    {
      this.ViewData["RemoveUserContext"] = (object) true;
      this.ViewData["DontShowLoggedInContext"] = (object) true;
      ActionResult actionResult = this.DetailsSSRInternal(this.TfsRequestContext, itemName, (PublishedExtension) null, (PublishedExtensionResult) null);
      this.Response.Cache.SetCacheability(HttpCacheability.Public);
      this.Response.Cache.SetExpires(DateTime.Now.Add(TimeSpan.FromMinutes((double) this.AFDCacheExpiryTime)));
      List<string> stringList = new List<string>();
      HttpCookieCollection cookies = this.Response?.Cookies;
      if (cookies != null)
      {
        foreach (string str in (NameObjectCollectionBase) cookies)
        {
          if (!string.IsNullOrEmpty(str))
            stringList.Add(str);
        }
      }
      foreach (string name in stringList)
        this.Response?.Cookies?.Remove(name);
      return actionResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult DetailsSSR(string itemName) => this.DetailsSSRInternal(this.TfsRequestContext, itemName, (PublishedExtension) null, (PublishedExtensionResult) null);

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Details(
      string itemName,
      bool install = false,
      bool auth_redirect = false,
      string installContext = null)
    {
      Dictionary<string, double> blockExecutionTimeMap = new Dictionary<string, double>();
      try
      {
        using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "GalleryController.Details"))
        {
          using (new InstrumentBlock(nameof (Details), blockExecutionTimeMap))
          {
            PublishedExtensionResult extensionResult;
            ActionResult actionResult;
            using (new InstrumentBlock("CheckCommonSteps", blockExecutionTimeMap))
              actionResult = this.CheckCommonSteps(itemName, out extensionResult);
            PublishedExtension extension = extensionResult.Extension;
            if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableItemDetailsForUnpublishedOrLockedExtensions") && extension != null && extension.IsVsCodeExtension() && (extension.Flags.HasFlag((System.Enum) PublishedExtensionFlags.Unpublished) || extension.Flags.HasFlag((System.Enum) PublishedExtensionFlags.Locked)))
              throw new HttpException(404, GalleryResources.PageNotFoundError);
            if (actionResult != null)
              return actionResult;
            if (this.ShouldItemBeReturnedFromAFDCache(itemName, extension))
              return (ActionResult) this.Redirect(this.GetNewItemDetailsCachedUrl(itemName));
            if (this.ShouldItemBeServerSideRendered(itemName, extension))
              return this.DetailsSSRInternal(this.TfsRequestContext, itemName, extension, extensionResult);
            if (this.ViewData.ContainsKey("server-context"))
              this.ViewData["onprem-version-supported"] = (object) this.ControllerHelper.ExtensionSupportsConnectedTfsVersion(extension);
            TeamFoundationExecutionEnvironment executionEnvironment;
            if (extension == null)
            {
              if (installContext != null)
              {
                executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
                if (!executionEnvironment.IsOnPremisesDeployment)
                  goto label_43;
              }
              else
                goto label_43;
            }
            if (install)
            {
              if (this.ShouldRedirectToNewExperience(extension, installContext))
                return (ActionResult) this.Redirect(this.GetNewAcquisitionExperienceRedirectUrl(extension));
              if (!extension.IsPaid())
              {
                executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
                if (!executionEnvironment.IsOnPremisesDeployment)
                  return (ActionResult) this.Redirect(this.GetRedirectUrlToDetailsPage());
              }
            }
            if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.VsAnnualDisable") && this.ShouldRedirectForAnnualSubscription(itemName))
              return (ActionResult) this.Redirect("https://go.microsoft.com/fwlink/?linkid=2053014");
            executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
            if (executionEnvironment.IsOnPremisesDeployment && !install && extension != null && extension.IsPaid())
            {
              string hostedRedirectUrl = this.DetailsHelper.GetOnPremToHostedRedirectURL(extension, itemName);
              if (hostedRedirectUrl != null)
                return (ActionResult) this.Redirect(hostedRedirectUrl);
            }
            GalleryUtil.LoadExtensionDeploymentType(extension);
            if (extension != null && extension.Versions != null)
            {
              using (new InstrumentBlock("LoadBadges", blockExecutionTimeMap))
                this.DetailsHelper.LoadBadges(extension);
              using (new InstrumentBlock("LoadUserReviews", blockExecutionTimeMap))
                this.DetailsHelper.LoadUserReviews(extension);
            }
label_43:
            using (new InstrumentBlock("LoadCommonData", blockExecutionTimeMap))
              actionResult = this.LoadCommonData(extensionResult, itemName, install, auth_redirect, installContext, true, blockExecutionTimeMap);
            if (actionResult != null)
              return actionResult;
            using (new InstrumentBlock("PopulateGeneralInfo", blockExecutionTimeMap))
              this.DetailsHelper.PopulateGeneralInfo(isVsipPartner: this.DetailsHelper.IsExtensionVsipPartner(extension));
            using (new InstrumentBlock("LoadWorksWithString", blockExecutionTimeMap))
              this.DetailsHelper.LoadWorksWithString(extension);
            using (new InstrumentBlock("GetPresentTargetPlatformsPairs", blockExecutionTimeMap))
              this.DetailsHelper.GetPresentTargetPlatformsPairs(this.TfsRequestContext, extension);
            if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
            {
              if (this.GetCookieOrFeatureFlagStatus("EnablePublisherProfilePage", "Microsoft.VisualStudio.Services.Gallery.EnablePublisherProfilePage"))
                this.ViewData["org-publisher-display-names"] = (object) this.GetOrgPublisherNamesMap().Keys.ToArray<string>();
            }
          }
        }
        PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", this.TfsRequestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", this.TfsRequestContext.LastTracedBlockElapsedMilliseconds());
          foreach (KeyValuePair<string, double> keyValuePair in blockExecutionTimeMap)
            ciData.Add(keyValuePair.Key, keyValuePair.Value);
        }));
        return (ActionResult) this.View("VSSItemDetails");
      }
      catch (FileIdNotFoundException ex)
      {
        throw new HttpException(404, WACommonResources.PageNotFound);
      }
    }

    private string GetNewItemDetailsCachedUrl(string itemName)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
      return this.GetRedirectUrlWithParams(this.DetailsSSRHelper.GetGalleryUrl() + "itemdetails", queryString);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult VsGallery(string extensionId)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IVsGalleryMigrationService service = vssRequestContext.GetService<IVsGalleryMigrationService>();
      PublishedExtension extension;
      try
      {
        Guid extensionId1 = new Guid(extensionId);
        extension = service.QueryExtensionById(vssRequestContext, extensionId1, (string) null, ExtensionQueryFlags.IncludeInstallationTargets, Guid.Empty);
        if (!extension.IsVsExtension())
          throw new HttpException(404, WACommonResources.PageNotFound);
      }
      catch (Exception ex)
      {
        throw new HttpException(404, ex.Message);
      }
      return (ActionResult) this.RedirectPermanent(this.GetDetailsPageUrl(extension.GetFullyQualifiedName()));
    }

    private ActionResult CheckCommonSteps(
      string itemName,
      out PublishedExtensionResult extensionResult)
    {
      if (!this.IsItemNameValid(itemName))
        throw new HttpException(404, GalleryResources.PageNotFoundError);
      bool ensureSharedAccounts = this.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      extensionResult = new PublishedExtensionResult();
      if (this.GetCookieOrFeatureFlagStatus("XamarinUniversityDisable", "Microsoft.VisualStudio.Services.Gallery.XamarinUniversityDisable") && itemName.Equals("ms.xamarin-university", StringComparison.OrdinalIgnoreCase))
        return (ActionResult) this.Redirect("http://aka.ms/learn-xamarin");
      try
      {
        extensionResult = this.DetailsHelper.GetPublishedExtension(itemName, ensureSharedAccounts, true, includeLatestVersionsOnly: false);
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(404, ex.Message);
      }
      if (extensionResult.AuthenticationRequired)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      PublishedExtension extension = extensionResult.Extension;
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        ITeamFoundationPropertyService service = this.TfsRequestContext.GetService<ITeamFoundationPropertyService>();
        ArtifactSpec redirectionArtifactSpec = GalleryServerUtil.GetExtensionRedirectionArtifactSpec(itemName);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        ArtifactSpec artifactSpec = redirectionArtifactSpec;
        using (TeamFoundationDataReader properties = service.GetProperties(tfsRequestContext, artifactSpec, (IEnumerable<string>) null))
        {
          if (properties != null)
          {
            foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
            {
              foreach (PropertyValue propertyValue in current.PropertyValues)
              {
                if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.PropertyName) && propertyValue.PropertyName.Equals("redirection", StringComparison.OrdinalIgnoreCase))
                {
                  if (propertyValue.Value == null)
                  {
                    this.TfsRequestContext.Trace(12062073, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("Value cannot be null for redirection property, itemName: {0}", (object) itemName));
                  }
                  else
                  {
                    string detailsPageUrl = propertyValue.Value.ToString();
                    if (!detailsPageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !detailsPageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                      detailsPageUrl = this.GetDetailsPageUrl(detailsPageUrl);
                    this.TfsRequestContext.Trace(12062073, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Redirecting to {1} , itemName: {0}", (object) itemName, (object) detailsPageUrl));
                    return (ActionResult) this.RedirectPermanent(detailsPageUrl);
                  }
                }
              }
            }
          }
        }
        if (extension == null && !extensionResult.IsNotAuthenticated)
          throw new HttpException(404, GalleryResources.PageNotFoundError);
      }
      return (ActionResult) null;
    }

    private ActionResult DetailsSSRInternal(
      IVssRequestContext requestContext,
      string itemName,
      PublishedExtension extension,
      PublishedExtensionResult extensionResult)
    {
      if (!this.GetCookieOrFeatureFlagStatus("EnableItemDetailsPageServerSideRendering", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageServerSideRendering"))
        throw new HttpException(404, GalleryResources.PageNotFoundError);
      ViewResult viewResult = (ViewResult) null;
      Dictionary<string, double> blockExecutionTimeMap = new Dictionary<string, double>();
      bool isCacheHit = false;
      bool overviewCacheEnabled = this.GetCookieOrFeatureFlagStatus("EnableItemDetailsPageOverviewCache", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageOverviewCache");
      bool featureFlagStatus1 = this.GetCookieOrFeatureFlagStatus("EnableDelayOverviewSSR", "Microsoft.VisualStudio.Services.Gallery.EnableDelayOverviewSSR");
      bool featureFlagStatus2 = this.GetCookieOrFeatureFlagStatus("EnableSSRForDetailsTabs", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForDetailsTabs");
      this.AddAfdHeaderIdentifierToViewData();
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "GalleryController.DetailsSSR"))
      {
        using (new InstrumentBlock(nameof (DetailsSSRInternal), blockExecutionTimeMap))
        {
          using (new InstrumentBlock("getExtensionTime", blockExecutionTimeMap))
          {
            if (extension == null)
            {
              this.CheckCommonSteps(itemName, out extensionResult);
              extension = extensionResult.Extension;
            }
          }
          if (extension == null)
            throw new HttpException(404, GalleryResources.PageNotFoundError);
          using (new InstrumentBlock("getExtensionPropertiesTime", blockExecutionTimeMap))
            this.DetailsHelper.LoadExtensionProperties(extension);
          if (featureFlagStatus2 && extension != null && extension.Versions != null)
          {
            using (new InstrumentBlock("LoadUserReviews", blockExecutionTimeMap))
              this.DetailsHelper.LoadUserReviews(extension);
          }
          using (new InstrumentBlock("loadPageMetadataTime", blockExecutionTimeMap))
            this.DetailsHelper.LoadPageMetadata(GalleryPages.ItemDetailsPage, extension, (string) null, (string) null, (string) null);
          using (new InstrumentBlock("getItemOverviewTime", blockExecutionTimeMap))
          {
            ExtensionVersion extensionVersion = GalleryServerUtil.GetLatestValidatedExtensionVersion(extension.Versions);
            string overviewMD = !overviewCacheEnabled ? this.DetailsHelper.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, extensionVersion.Version, "Microsoft.VisualStudio.Services.Content.Details", extensionResult.ExtensionAssetsToken) : this.TfsRequestContext.GetService<ExtensionOverviewCacheService>().GetExtensionOverviewMarkdown(this.TfsRequestContext, extension.Publisher.PublisherName, extension.ExtensionName, extensionVersion.Version, extensionResult.ExtensionAssetsToken, out isCacheHit);
            bool isMDPruned = false;
            string prunedMD = "";
            this.ViewData["overviewMDLength"] = (object) overviewMD.Length;
            string str = overviewMD;
            if (featureFlagStatus1)
            {
              this.DetailsSSRHelper.getPartialOverviewMD(extension.GetFullyQualifiedName(), overviewMD, out prunedMD, out isMDPruned);
              str = prunedMD;
            }
            this.ViewData["prunedMDLength"] = (object) (isMDPruned ? prunedMD.Length : 0);
            this.ViewData["isMDPruned"] = (object) isMDPruned;
            this.ViewData["vss-item-overview"] = (object) str;
          }
          using (new InstrumentBlock("getGeneralInfoTime", blockExecutionTimeMap))
          {
            this.ControllerHelper.PopulateGeneralInfo();
            this.ViewData["user-login-url"] = (object) this.ControllerHelper.GetSignInRedirectUrl();
            this.ViewData["detailsTabsSSREnabled"] = (object) featureFlagStatus2;
          }
          if (extension.IsVsCodeExtension() || extension.IsVsExtension() || extension.IsVSTSExtensionResourceOrIntegration())
            this.DetailsHelper.LoadWorksWithString(extension);
          if (extension.IsVsExtension() || extension.IsVSTSExtensionResourceOrIntegration())
          {
            using (new InstrumentBlock("VSOrVSTSAdditionalData", blockExecutionTimeMap))
            {
              this.DetailsHelper.LoadUserHasPublisherRoleReader(extension);
              this.DetailsHelper.LoadCanUpdateExtension(extension);
              if (extension.IsVsExtension())
              {
                GalleryUtil.LoadExtensionDeploymentType(extension);
                this.DetailsHelper.LoadExtensionVsixId(extension);
              }
            }
          }
          if (extension.IsVSTSExtensionResourceOrIntegration())
          {
            using (new InstrumentBlock("LoadCommerceData", blockExecutionTimeMap))
              this.DetailsHelper.LoadCommerceData(extension, itemName);
          }
          using (new InstrumentBlock("GetPresentTargetPlatformsPairs", blockExecutionTimeMap))
            this.DetailsHelper.GetPresentTargetPlatformsPairs(this.TfsRequestContext, extension);
          using (new InstrumentBlock("initializeViewDataTime", blockExecutionTimeMap))
          {
            this.DetailsHelper.LoadCspData();
            this.DetailsSSRHelper.InitializeViewData(requestContext, extension, extensionResult, blockExecutionTimeMap);
          }
          using (new InstrumentBlock("viewCreationTime", blockExecutionTimeMap))
            viewResult = this.View("VSSItemDetailsSSR");
        }
      }
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext, (Action<CustomerIntelligenceData>) (ciData =>
      {
        ciData.Add("Timings", this.TfsRequestContext.GetTraceTimingAsString());
        ciData.Add("ElapsedMillis", this.TfsRequestContext.LastTracedBlockElapsedMilliseconds());
        ciData.Add("overviewCacheEnabled", overviewCacheEnabled);
        ciData.Add("isCacheHit", isCacheHit);
        ciData.Add("ExtensionName", itemName);
        foreach (KeyValuePair<string, double> keyValuePair in blockExecutionTimeMap)
          ciData.Add(keyValuePair.Key, keyValuePair.Value);
        foreach (KeyValuePair<string, string> refererDetail in this.GetRefererDetails())
          ciData.Add(refererDetail.Key, refererDetail.Value);
      }));
      return (ActionResult) viewResult;
    }

    private Dictionary<string, string> GetRefererDetails()
    {
      string refererHeader = this.GetRefererHeader();
      Dictionary<string, string> refererDetails = new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(refererHeader))
      {
        try
        {
          Uri uri = new Uri(refererHeader);
          bool? nullable1 = uri.Host?.ToLower(CultureInfo.InvariantCulture).StartsWith("marketplace");
          if (nullable1.HasValue)
          {
            bool? nullable2 = nullable1;
            bool flag = true;
            if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
            {
              refererDetails.Add("refererFromMarketplace", "true");
              refererDetails.Add("path", uri.AbsolutePath);
              refererDetails.Add("query", uri.Query);
              goto label_7;
            }
          }
          refererDetails.Add("refererFromMarketplace", "false");
        }
        catch (Exception ex)
        {
          refererDetails.Add("failedToGetRefererDetails", "true");
          return refererDetails;
        }
      }
      else
        refererDetails.Add("refererFromMarketplace", "false");
label_7:
      return refererDetails;
    }

    private string GetRefererHeader()
    {
      string refererHeader = string.Empty;
      NameObjectCollectionBase.KeysCollection keys = this.Request?.Headers?.Keys;
      if (keys != null)
      {
        for (int index = 0; index < keys.Count; ++index)
        {
          if (string.Equals(keys[index], "referer", StringComparison.OrdinalIgnoreCase))
          {
            refererHeader = this.Request?.Headers[keys[index]];
            break;
          }
        }
      }
      return refererHeader;
    }

    private bool ShouldItemBeServerSideRendered(string itemName, PublishedExtension extension)
    {
      if (extension == null || !this.WebContext.IsHosted || !this.GetCookieOrFeatureFlagStatus("EnableItemDetailsPageServerSideRendering", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageServerSideRendering") || this.IsItemExcludedFromSSR(itemName) || !this.GetCookieOrFeatureFlagStatus("EnableSSRForPrivateExtensions", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForPrivateExtensions") && !extension.IsPublic() || this.ViewData.ContainsKey("server-context") && !this.IsOnPremServerVersionSupported())
        return false;
      try
      {
        NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
        if (queryString["ssr"] != null)
          return string.Compare(queryString["ssr"], "true", StringComparison.OrdinalIgnoreCase) == 0;
      }
      catch (Exception ex)
      {
        return false;
      }
      return this.GetCookieOrFeatureFlagStatus("EnableItemDetailsSSRForVsCode", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForVsCode") && extension.IsVsCodeExtension() || this.GetCookieOrFeatureFlagStatus("EnableItemDetailsSSRForVsIDE", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForVsIDE") && extension.IsVsExtension() || this.GetCookieOrFeatureFlagStatus("EnableSSRForHomepageVSIDE", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageVSIDE") && extension.IsVsExtension() && this.IsRefererFromMarketplaceNonSearchPage() || this.GetCookieOrFeatureFlagStatus("EnableItemDetailsSSRForAzDev", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsSSRForAzDev") && extension.IsVSTSExtensionResourceOrIntegration() && this.ShouldEnableSSRForAzDevExtension(extension) || this.GetCookieOrFeatureFlagStatus("EnableSSRForHomepageAzDev", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageAzDev") && extension.IsVSTSExtensionResourceOrIntegration() && this.IsRefererFromMarketplaceNonSearchPage() && this.ShouldEnableSSRForAzDevExtension(extension) || this.IsItemNamePresentInSSRRegistry(itemName);
    }

    private bool IsOnPremServerVersionSupported()
    {
      Dictionary<string, string> dictionary = this.ViewData["server-context"] != null ? JsonUtilities.Deserialize<Dictionary<string, string>>((string) this.ViewData["server-context"]) : new Dictionary<string, string>();
      int result = 0;
      if (dictionary.ContainsKey(CloudConnectedServerConstants.EnabledFeatures) && dictionary[CloudConnectedServerConstants.EnabledFeatures] != null)
        int.TryParse(dictionary[CloudConnectedServerConstants.EnabledFeatures], out result);
      return (result & 2) != 0;
    }

    private bool IsRefererFromMarketplaceNonSearchPage()
    {
      Dictionary<string, string> refererDetails = this.GetRefererDetails();
      return refererDetails.ContainsKey("refererFromMarketplace") && refererDetails["refererFromMarketplace"].Equals("true", StringComparison.OrdinalIgnoreCase) && refererDetails.ContainsKey("path") && !refererDetails["path"].ToLower(CultureInfo.InvariantCulture).Contains("search");
    }

    private bool ShouldEnableSSRForAzDevExtension(PublishedExtension extension) => (!this.ViewData.ContainsKey("server-context") || this.GetCookieOrFeatureFlagStatus("EnableSSRForConnectedContext", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForConnectedContext")) && (this.GetCookieOrFeatureFlagStatus("EnableSSRForPaidAzDev", "Microsoft.VisualStudio.Services.Gallery.EnableSSRForPaidAzDev") || !extension.IsPaid() && !extension.IsTrial());

    private void AddAfdHeaderIdentifierToViewData()
    {
      NameObjectCollectionBase.KeysCollection keys = this.Request?.Headers?.Keys;
      if (keys == null)
        return;
      for (int index = 0; index < keys.Count; ++index)
      {
        if (string.Equals(keys[index], "X-FD-REF", StringComparison.OrdinalIgnoreCase))
          this.ViewData["AfdIdentifier"] = (object) this.Request?.Headers[keys[index]];
      }
    }

    private bool IsItemNamePresentInSSRRegistry(string itemName)
    {
      string itemList = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/SSRItems", "");
      return GalleryController.IsItemPresentInItemList(itemName, itemList);
    }

    private bool IsItemExcludedFromSSR(string itemName)
    {
      string itemList = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/SSRExcludedItems", "");
      return GalleryController.IsItemPresentInItemList(itemName, itemList);
    }

    private bool ShouldItemBeReturnedFromAFDCache(string itemName, PublishedExtension extension)
    {
      if (!this.GetCookieOrFeatureFlagStatus("EnableItemDetailsAFDCaching", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCaching") && !this.GetCookieOrFeatureFlagStatus("EnableItemDetailsAFDCachingForAllVSCode", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCachingForAllVSCode"))
        return false;
      try
      {
        NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
        if (queryString["ssr"] != null)
        {
          if (string.Compare(queryString["ssr"], "false", StringComparison.OrdinalIgnoreCase) == 0)
            return false;
        }
      }
      catch
      {
        return false;
      }
      if (this.DetailsSSRHelper.IsLinuxBasedBrowser(this.TfsRequestContext))
        return false;
      if (extension.IsVsCodeExtension() && extension.IsPublic() && this.GetCookieOrFeatureFlagStatus("EnableItemDetailsAFDCachingForAllVSCode", "Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCachingForAllVSCode"))
        return true;
      string itemList = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/SSRCachedItems", "");
      return GalleryController.IsItemPresentInItemList(itemName, itemList);
    }

    private static bool IsItemPresentInItemList(string itemName, string itemList)
    {
      if (string.IsNullOrEmpty(itemList))
        return false;
      string[] strArray = itemList.Split(';');
      if (strArray.Length != 0)
      {
        foreach (string a in strArray)
        {
          if (string.Equals(a, itemName, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private bool IsItemNameValid(string itemName)
    {
      if (itemName == null)
        return false;
      string[] strArray = itemName.Split(new char[1]{ '.' }, 2);
      if (strArray.Length != 2)
        return false;
      try
      {
        GalleryUtil.CheckPublisherName(strArray[0]);
        GalleryUtil.CheckExtensionName(strArray[1]);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private Uri GetRedirectUrl(string itemName, string accountId)
    {
      RouteValueDictionary routeValues = new RouteValueDictionary();
      routeValues[nameof (itemName)] = (object) itemName;
      routeValues["targetId"] = (object) accountId;
      if (!string.IsNullOrEmpty(this.Request.QueryString["referrer"]))
        routeValues["referrer"] = (object) this.Request.QueryString["referrer"];
      return new Uri(this.Url.Action("Acquisition", (string) null, routeValues, this.Request.Url.Scheme, this.Request.Url.Host));
    }

    private ActionResult LoadCommonData(
      PublishedExtensionResult extensionResult,
      string itemName,
      bool install = false,
      bool auth_redirect = false,
      string installContext = null,
      bool isDetailsEndpoint = false,
      Dictionary<string, double> blocksExecutionTimes = null)
    {
      PublishedExtension extension = extensionResult.Extension;
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadDefaultViewData"), blocksExecutionTimes))
      {
        this.DetailsHelper.LoadDefaultViewData(extensionResult);
        this.DetailsHelper.LoadCspData();
      }
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadCommerceData"), blocksExecutionTimes))
        this.DetailsHelper.LoadCommerceData(extension, itemName, isDetailsEndpoint);
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadMsdnData"), blocksExecutionTimes))
        this.DetailsHelper.LoadMsdnData(extension);
      if (extension != null && extension.Versions != null)
      {
        ActionResult actionResult = this.LoadModalInstallData(extension, auth_redirect, install, installContext, isDetailsEndpoint);
        if (actionResult != null)
          return actionResult;
        using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadExtensionProperties"), blocksExecutionTimes))
        {
          this.DetailsHelper.LoadExtensionProperties(extension);
          this.DetailsHelper.LoadIsMigratedProperty(extension);
        }
        using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadPageMetadata"), blocksExecutionTimes))
        {
          this.DetailsHelper.LoadPageMetadata(GalleryPages.ItemDetailsPage, extension, (string) null, (string) null, (string) null);
          this.DetailsHelper.LoadExtensionVsixId(extension);
        }
        using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadMiscellaneousItemData"), blocksExecutionTimes))
          this.DetailsHelper.LoadMiscellaneousItemData(extensionResult);
      }
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (LoadCommonData), (object) "LoadExecutionEnvironmentData"), blocksExecutionTimes))
        this.DetailsHelper.LoadExecutionEnvironmentData(extension, itemName, installContext);
      return (ActionResult) null;
    }

    private ActionResult LoadModalInstallData(
      PublishedExtension extension,
      bool auth_redirect,
      bool install,
      string installContext,
      bool isDetailsEndpoint = false)
    {
      if (isDetailsEndpoint && this.ShouldRedirectToNewExperience(extension, installContext))
        return (ActionResult) null;
      bool flag = this.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      int num = GalleryUtil.HasAcquisitionExperience(extension) ? 1 : 0;
      this.ViewData["needs-aad-auth"] = (object) false;
      if (num != 0)
      {
        RedirectResult aadRedirectResult = this.GetAADRedirectResult(extension, auth_redirect);
        if (install)
        {
          if (!auth_redirect)
          {
            string str = this.Request.UrlReferrer?.AbsoluteUri ?? "";
            if (this.Request.Cookies["CommerceMarketplaceSubscriptionCookie"] == null)
            {
              this.Response.Cookies.Add(new HttpCookie("CommerceMarketplaceSubscriptionCookie", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "referrer", (object) str)));
            }
            else
            {
              HttpCookie cookie = this.Request.Cookies["CommerceMarketplaceSubscriptionCookie"];
              cookie.Values["referrer"] = str;
              this.Response.Cookies["CommerceMarketplaceSubscriptionCookie"].Value = cookie.Value;
            }
          }
          if (aadRedirectResult != null)
            return (ActionResult) aadRedirectResult;
          if (!flag)
            return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
        }
        else if (aadRedirectResult != null)
          this.ViewData["needs-aad-auth"] = (object) true;
        if (flag)
        {
          this.ViewData["is-modal-install"] = (object) true;
          this.ViewData["authenticated-tenant-id"] = (object) this.DetailsHelper.GetAuthenticatedTenantId();
          this.DetailsHelper.LoadViewDataForInstall(extension);
        }
      }
      return (ActionResult) null;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PublisherCreate()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, WACommonResources.PageNotFound);
      IEnumerable<Tenant> tenants = this.TfsRequestContext.GetExtension<IAadTenantProvider>().GetTenants(this.TfsRequestContext);
      IGalleryDomainInfo extension = this.TfsRequestContext.GetExtension<IGalleryDomainInfo>();
      string str1 = "";
      if (extension != null)
        str1 = extension.GetDomainInfo(this.TfsRequestContext).UserDomain;
      PublisherDirectoryData publisherDirectoryData = new PublisherDirectoryData()
      {
        UserDomain = str1,
        PublisherTenants = tenants
      };
      string str2 = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
      this.ViewData["directoryInfo"] = (object) publisherDirectoryData;
      this.ViewData["ReCaptchaPublicKey"] = (object) str2;
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (PublisherCreate));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult VSExtensionCreate(string publisherName)
    {
      IPublisherService service1 = this.TfsRequestContext.GetService<IPublisherService>();
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, GalleryResources.OnPremUnsupportedText);
      if (!string.IsNullOrEmpty(publisherName))
      {
        try
        {
          publisher = service1.QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.None);
          GallerySecurity.CheckPublisherPermission(this.TfsRequestContext, publisher, PublisherPermissions.PublishExtension);
          this.ViewData["GalleryPublisherData"] = (object) publisher;
          this.PopulateValidCategories("vs");
        }
        catch (PublisherDoesNotExistException ex)
        {
          throw new HttpException(404, WACommonResources.PageNotFound);
        }
        catch (AccessCheckException ex)
        {
          throw new HttpException(401, ex.Message);
        }
      }
      string str = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
      IFirstPartyPublisherAccessService service2 = this.TfsRequestContext.GetService<IFirstPartyPublisherAccessService>();
      this.ViewData["ReCaptchaPublicKey"] = (object) str;
      this.ViewData["VSExtensionPublishScenario"] = (object) "Create";
      this.ViewData["IsMSPublisher"] = (object) service2.IsMicrosoftEmployee(this.TfsRequestContext, publisher, (PublishedExtension) null);
      this.ViewData["MaxPackageSize"] = (object) GalleryServerUtil.GetMaxPackageSizeInBytes(this.TfsRequestContext, 482344960L);
      this.ViewData["MaxAssetSize"] = (object) GalleryServerUtil.GetMaxPackageSizeInBytes(this.TfsRequestContext, 2097152L);
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View("VSExtensionPublish");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult VSExtensionEdit(string publisherName, string extensionName)
    {
      IPublisherService service1 = this.TfsRequestContext.GetService<IPublisherService>();
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, GalleryResources.OnPremUnsupportedText);
      if (!string.IsNullOrEmpty(publisherName))
      {
        try
        {
          publisher = service1.QueryPublisher(this.TfsRequestContext, publisherName, PublisherQueryFlags.None);
          GallerySecurity.CheckPublisherPermission(this.TfsRequestContext, publisher, PublisherPermissions.PublishExtension);
          this.ViewData["GalleryPublisherData"] = (object) publisher;
          this.PopulateValidCategories("vs");
        }
        catch (PublisherDoesNotExistException ex)
        {
          throw new HttpException(404, WACommonResources.PageNotFound);
        }
        catch (AccessCheckException ex)
        {
          throw new HttpException(401, ex.Message);
        }
      }
      PublishedExtensionResult publishedExtensionResult = new PublishedExtensionResult();
      PublishedExtensionResult publishedExtension1;
      try
      {
        publishedExtension1 = this.DetailsHelper.GetPublishedExtension(GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName), true, true, false, false);
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(404, ex.Message);
      }
      PublishedExtension publishedExtension2 = publishedExtension1 != null && publishedExtension1.Extension != null ? publishedExtension1.Extension : throw new HttpException(404, GalleryResources.PageNotFoundError);
      this.ViewData["Extension"] = (object) publishedExtension2;
      List<ExtensionMetadata> metadata = new List<ExtensionMetadata>();
      if (publishedExtension2.Metadata != null)
        publishedExtension2.Metadata.ForEach((Action<ExtensionMetadata>) (a =>
        {
          if (!(a.Key == "VsixId") && !(a.Key == "SourceCodeUrl") && !(a.Key == "ReferralUrl") && !(a.Key == "ConvertedToMarkdown") && !(a.Key == "MigratedFromVSGallery") && !(a.Key == "OriginalExtensionSource"))
            return;
          metadata.Add(a);
        }));
      this.ViewData["ExtensionMetadata"] = (object) metadata;
      this.DetailsHelper.LoadExtensionProperties(publishedExtension2);
      IFirstPartyPublisherAccessService service2 = this.TfsRequestContext.GetService<IFirstPartyPublisherAccessService>();
      this.ViewData["ReCaptchaPublicKey"] = (object) this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
      this.ViewData["VSExtensionPublishScenario"] = (object) "Edit";
      this.ViewData["IsMSPublisher"] = (object) service2.IsMicrosoftEmployee(this.TfsRequestContext, publisher, publishedExtension2);
      this.ViewData["MaxPackageSize"] = (object) GalleryServerUtil.GetMaxPackageSizeInBytes(this.TfsRequestContext, 482344960L);
      this.ViewData["MaxAssetSize"] = (object) GalleryServerUtil.GetMaxPackageSizeInBytes(this.TfsRequestContext, 2097152L);
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View("VSExtensionPublish");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Eula(string itemName, string version = null)
    {
      PublishedExtensionResult publishedExtension = this.ControllerHelper.GetPublishedExtension(itemName, false);
      if (publishedExtension.AuthenticationRequired)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      PublishedExtension extension = publishedExtension.Extension != null ? publishedExtension.Extension : throw new HttpException(404, WACommonResources.PageNotFound);
      if (extension != null)
      {
        string str = !extension.IsFirstParty() || !this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? this.GetEULA(extension, publishedExtension.ExtensionAssetsToken, version) : this.GetMsEULA(extension, publishedExtension.ExtensionAssetsToken);
        this.ViewData["vss-item-license"] = !str.Equals(string.Empty) ? (object) str : throw new HttpException(404, WACommonResources.PageNotFound);
      }
      this.ViewData["vss-extension"] = (object) extension;
      this.ViewData["vss-extension-token"] = (object) publishedExtension.ExtensionAssetsToken;
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (Eula));
    }

    protected virtual RedirectResult GetAADRedirectResult(
      PublishedExtension publishedExtension,
      bool auth_redirect)
    {
      return this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && publishedExtension.IsPaid() && !this.IsFreeInstallWorkflow(publishedExtension) && !publishedExtension.IsPreview() && !auth_redirect && !this.IsAuthenticatedWithAAD() ? this.Redirect(this.ControllerHelper.GetAADSignInRedirectUrl(string.Empty, (Uri) null)) : (RedirectResult) null;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsBypassAntiForgeryValidation]
    [ValidateConnectedAuthToken]
    public ActionResult ConnectServer()
    {
      this.TfsRequestContext.CheckOnPremisesDeployment();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      string str1 = this.Request.Form["accountName"];
      string str2 = this.Request.Form["accountId"];
      string str3 = this.Request.Form["spsUrl"];
      string str4 = this.Request.Form["installAction"];
      string str5 = this.Request.Form["authorizationUrl"];
      ArgumentUtility.CheckStringForNullOrEmpty(str1, "accountName");
      ArgumentUtility.CheckStringForNullOrEmpty(str2, "accountId");
      ArgumentUtility.CheckStringForNullOrEmpty(str5, "authorizationUrl");
      ArgumentUtility.CheckStringForNullOrEmpty(str3, "spsUrl");
      ArgumentUtility.CheckStringForNullOrEmpty(str4, "installAction");
      return (ActionResult) this.Redirect(this.ConnectServerHelper.SaveConnectedServerSettings(str1, str2, str3, str4, str5));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetConnectedServerContext(string collectionId)
    {
      this.TfsRequestContext.CheckOnPremisesDeployment();
      string token = (string) null;
      Guid result;
      if (Guid.TryParse(collectionId, out result))
      {
        using (IVssRequestContext requestContext = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(this.TfsRequestContext, result, RequestContextType.UserContext))
        {
          IConnectedServerContextKeyService service = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>();
          token = service.GetToken(requestContext, (Dictionary<string, string>) null);
          Dictionary<string, string> dictionary = CloudConnectedUtilities.DecodeToken(token);
          service.SaveAuthToken(requestContext, dictionary[CloudConnectedServerConstants.AuthenticationToken]);
        }
      }
      JsonNetResult connectedServerContext = new JsonNetResult();
      connectedServerContext.Data = (object) token;
      return (ActionResult) connectedServerContext;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Install(
      string itemName,
      bool auth_redirect = false,
      bool freeInstall = false,
      Guid? accountId = null)
    {
      this.ControllerHelper.GetPublishedExtension(itemName, true);
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
      routeValueDictionary[nameof (itemName)] = (object) itemName;
      routeValueDictionary["install"] = (object) true;
      routeValueDictionary[nameof (auth_redirect)] = !auth_redirect ? (object) false : (object) true;
      routeValueDictionary[nameof (freeInstall)] = !freeInstall ? (object) false : (object) true;
      if (accountId.HasValue)
        routeValueDictionary[nameof (accountId)] = (object) accountId;
      if (this.Request.Params["subscriptionId"] != null)
        routeValueDictionary["subscriptionId"] = (object) this.Request.Params["subscriptionId"];
      return this.Request.Cookies["CommerceMarketplaceSubscriptionCookie"] != null ? (ActionResult) this.Redirect(this.GetActionUri("Details", Guid.Empty, routeValueDictionary).ToString()) : (ActionResult) this.RedirectToAction("Details", routeValueDictionary);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(12060020, 12060029)]
    public ActionResult Subscription(Guid subscriptionId, Guid? tenantId, bool auth_redirect = false)
    {
      Guid guid = tenantId ?? Guid.Empty;
      Guid authenticatedTenantId = this.ControllerHelper.GetAuthenticatedTenantId();
      if (authenticatedTenantId != guid && guid != Guid.Empty)
      {
        if (auth_redirect)
        {
          this.TfsRequestContext.Trace(12060021, TraceLevel.Warning, this.AreaName, this.LayerName, string.Format("Redirecting user back to home page. Failed to authenticate in tenant which contains the subscription. User is autheticated in tenant: {0}. Subscription (targetTenant) was created in {1}. SubscriptionId: {2}", (object) authenticatedTenantId, (object) guid, (object) subscriptionId));
          return (ActionResult) this.RedirectToAction("Gallery");
        }
        this.TfsRequestContext.Trace(12060022, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Sending user to log in with AAD, user is autheticated in tenant: {0}. Subscription (targetTenant) was created in {1}. SubscriptionId: {2}", (object) authenticatedTenantId, (object) guid, (object) subscriptionId));
        return (ActionResult) this.Redirect(this.ControllerHelper.GetAADSignInRedirectUrl(guid.ToString(), (Uri) null));
      }
      Uri actionUri = this.GetActionUri("Details", subscriptionId, true);
      this.TfsRequestContext.Trace(12060027, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Sending the user back to the install page. TenantId: {0}, SubscriptionId: {1}, url: {2}", (object) authenticatedTenantId, (object) subscriptionId, (object) actionUri.ToString()));
      return (ActionResult) this.Redirect(actionUri.ToString());
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Category(string product, string categoryName)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        this.ViewData["product-categories"] = this.m_productExtensionsProvider.GetProductExtensions(this.TfsRequestContext, product) is ProductExtensions productExtensions ? (object) productExtensions.Categories : (object) (string[]) null;
        Uri galleryUri = GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker);
        string str = this.TfsRequestContext.RelativeUrl();
        string relativeUri = "/search?&target=" + product + "&category=" + categoryName;
        if (str.Contains("?"))
          relativeUri = relativeUri + "&" + str.Split('?')[1];
        return (ActionResult) this.RedirectPermanent(new Uri(galleryUri, relativeUri).AbsoluteUri);
      }
      this.ViewData["product-categories"] = this.m_productExtensionsProvider.GetProductExtensions(this.TfsRequestContext, product) is ProductExtensions productExtensions1 ? (object) productExtensions1.Categories : (object) (string[]) null;
      this.PopulateValidCategories(product);
      this.ControllerHelper.PopulateGeneralInfo();
      this.ControllerHelper.LoadPageMetadata(GalleryPages.CategoryPage, (PublishedExtension) null, product, categoryName, (string) null);
      return (ActionResult) this.View(nameof (Category));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Search(string product)
    {
      this.PopulateValidCategories(product);
      this.ViewData["target-platforms"] = (object) this.PopulateTargetPlatforms();
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseNewBranding") && this.Request.QueryString["target"] != null && string.Compare(this.Request.QueryString["target"], "vsts", StringComparison.OrdinalIgnoreCase) == 0)
      {
        NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
        queryString.Set("target", "AzureDevOps");
        return (ActionResult) this.RedirectPermanent(new UriBuilder(this.Request.Url)
        {
          Query = queryString.ToString()
        }.ToString());
      }
      if (this.Request.QueryString["category"] != null)
      {
        string key = this.Request.QueryString["category"];
        string str = "";
        if (GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.TryGetValue(key, out str))
        {
          NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.QueryString.ToString());
          queryString.Set("category", str);
          return (ActionResult) this.RedirectPermanent(new UriBuilder(this.Request.Url)
          {
            Query = queryString.ToString()
          }.ToString());
        }
      }
      this.ControllerHelper.PopulateGeneralInfo();
      if (this.Request.QueryString["term"] == null)
        this.ControllerHelper.LoadPageMetadata(GalleryPages.CategoryPage, (PublishedExtension) null, this.Request.QueryString["target"], this.Request.QueryString["category"], (string) null);
      else
        this.ControllerHelper.LoadPageMetadata(GalleryPages.SearchPage, (PublishedExtension) null, this.Request.QueryString["target"], (string) null, this.Request.QueryString["term"]);
      return (ActionResult) this.View("Category");
    }

    private IReadOnlyDictionary<string, string> PopulateTargetPlatforms() => GalleryServerUtil.GetAllVSCodeTargetPlatformPairs(this.TfsRequestContext);

    private void PopulateValidCategories(string product)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      string language = "en-us";
      IEnumerable<string> languages = (IEnumerable<string>) new string[1]
      {
        language
      };
      CategoriesResult categoriesResult = service.QueryAvailableCategories(this.TfsRequestContext, languages, product: product);
      Dictionary<string, List<ValidCategory>> dictionary = new Dictionary<string, List<ValidCategory>>();
      if (categoriesResult != null && categoriesResult.Categories != null)
      {
        foreach (ExtensionCategory category in categoriesResult.Categories)
        {
          if (category.AssociatedProducts != null)
          {
            foreach (string associatedProduct in category.AssociatedProducts)
            {
              List<ValidCategory> validCategoryList = (List<ValidCategory>) null;
              if (!dictionary.TryGetValue(associatedProduct, out validCategoryList))
              {
                validCategoryList = new List<ValidCategory>();
                dictionary[associatedProduct] = validCategoryList;
              }
              ValidCategory validCategory = new ValidCategory(category.GetCategoryTitleForLanguage(language));
              if (category.Parent != null)
                validCategory.ParentCategoryName = category.Parent.GetCategoryTitleForLanguage(language);
              validCategory.InternalCategoryName = category.CategoryName;
              validCategoryList.Add(validCategory);
            }
          }
        }
      }
      this.ViewData["valid-categories"] = (object) dictionary;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetExtensionsPerCategory(string product, bool removeFirstSetCategories = false)
    {
      int num = this.SetDefaultCarouselNumber();
      if (string.IsNullOrEmpty(product))
        throw new HttpException(400, "Argument product cannot be null");
      object productExtensions1 = this.m_productExtensionsProvider.GetProductExtensions(this.TfsRequestContext, product);
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions") & removeFirstSetCategories && !product.Equals("subscriptions", StringComparison.OrdinalIgnoreCase))
      {
        ProductExtensions productExtensions2 = productExtensions1 as ProductExtensions;
        productExtensions2.ExtensionsPerCategory.RemoveRange(0, num < productExtensions2.ExtensionsPerCategory.Count ? num : productExtensions2.ExtensionsPerCategory.Count);
      }
      JsonNetResult extensionsPerCategory = new JsonNetResult();
      extensionsPerCategory.Data = productExtensions1;
      return (ActionResult) extensionsPerCategory;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetExtensionScopes(string itemName)
    {
      PublishedExtensionResult publishedExtension = this.ControllerHelper.GetPublishedExtension(itemName, false);
      if (publishedExtension.AuthenticationRequired)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      PublishedExtension extension = publishedExtension.Extension;
      JsonNetResult extensionScopes = new JsonNetResult();
      extensionScopes.Data = (object) this.DetailsHelper.GetExtensionScopes(extension);
      return (ActionResult) extensionScopes;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetImportOperation(Guid jobId)
    {
      JsonNetResult importOperation = new JsonNetResult();
      importOperation.Data = (object) this.ImportHelper.GetImportOperation(jobId);
      return (ActionResult) importOperation;
    }

    public virtual TokenRequestResult FetchTokenResult(
      string itemName,
      ISubscriptionAccount selectedAccount)
    {
      Uri redirectUrl = this.GetRedirectUrl(itemName, selectedAccount.AccountId.ToString());
      return this.GetTokenResult(new Guid?(selectedAccount.AccountTenantId), redirectUrl);
    }

    public TokenRequestResult GetTokenResult(
      Guid? requestTenant,
      Uri actionUri = null,
      bool useNewTokenAcquisitionExperience = true)
    {
      this.TfsRequestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      Guid property = userIdentity.GetProperty<Guid>("Domain", Guid.Empty);
      TokenRequestResult tokenResult = new TokenRequestResult()
      {
        requiresRedirection = false
      };
      Guid guid = Guid.Empty;
      SessionTokenResult sessionTokenResult = (SessionTokenResult) null;
      if (requestTenant.HasValue && requestTenant.Value != property)
      {
        if (requestTenant.Value == Guid.Empty)
        {
          Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = this.DetailsHelper.GetPrimaryMsaIdentity(this.TfsRequestContext, userIdentity);
          if (primaryMsaIdentity != null)
          {
            this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("MSAPrimary tenantId = {0} id = {1}", (object) property, (object) primaryMsaIdentity.Id));
            guid = primaryMsaIdentity.Id;
          }
          else
          {
            this.TfsRequestContext.Trace(12061076, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("MSARedirect tenantId = {0}", (object) property));
            if (!this.GetCookieOrFeatureFlagStatus("SupportNativeAADUserInMSAOrg", "Microsoft.VisualStudio.Services.Gallery.SupportNativeAADUserInMSAOrg"))
            {
              tokenResult.requiresRedirection = true;
              TokenRequestResult tokenRequestResult = tokenResult;
              GalleryControllerHelper controllerHelper = this.ControllerHelper;
              Uri requestedReplyTo = actionUri;
              if ((object) requestedReplyTo == null)
                requestedReplyTo = this.GetActionUri("Details", Guid.Empty);
              string signInRedirectUrl = controllerHelper.GetAADSignInRedirectUrl("live.com", requestedReplyTo);
              tokenRequestResult.redirectionUri = signInRedirectUrl;
            }
          }
        }
        else if (this.ShouldUseNewTokenAcquisitionExperience(useNewTokenAcquisitionExperience))
        {
          sessionTokenResult = this.GetSessionTokenWithoutRedirection(requestTenant, "vso.commerce.write vso.gallery_acquire vso.licensing vso.extension_manage");
          if (sessionTokenResult == null)
          {
            tokenResult.requiresRedirection = true;
            TokenRequestResult tokenRequestResult = tokenResult;
            GalleryControllerHelper controllerHelper = this.ControllerHelper;
            string targetTenant = requestTenant.Value.ToString();
            Uri requestedReplyTo = actionUri;
            if ((object) requestedReplyTo == null)
              requestedReplyTo = this.GetActionUri("Details", Guid.Empty);
            string signInRedirectUrl = controllerHelper.GetAADSignInRedirectUrl(targetTenant, requestedReplyTo);
            tokenRequestResult.redirectionUri = signInRedirectUrl;
          }
        }
        else
        {
          this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("AAD tenantId = {0} Requestedtenantid = {1}", (object) property, (object) requestTenant.Value));
          tokenResult.requiresRedirection = true;
          TokenRequestResult tokenRequestResult = tokenResult;
          GalleryControllerHelper controllerHelper = this.ControllerHelper;
          string targetTenant = requestTenant.Value.ToString();
          Uri requestedReplyTo = actionUri;
          if ((object) requestedReplyTo == null)
            requestedReplyTo = this.GetActionUri("Details", Guid.Empty);
          string signInRedirectUrl = controllerHelper.GetAADSignInRedirectUrl(targetTenant, requestedReplyTo);
          tokenRequestResult.redirectionUri = signInRedirectUrl;
        }
      }
      if (sessionTokenResult == null)
      {
        if (guid == Guid.Empty)
        {
          IDelegatedAuthorizationService service = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          DateTime? nullable1 = new DateTime?(DateTime.UtcNow.AddHours(1.0));
          Guid? nullable2 = new Guid?();
          Guid? clientId = new Guid?();
          Guid? userId = nullable2;
          DateTime? validTo = nullable1;
          Guid? authorizationId = new Guid?();
          Guid? accessId = new Guid?();
          sessionTokenResult = service.IssueSessionToken(tfsRequestContext, clientId, userId, validTo: validTo, scope: "vso.commerce.write vso.gallery_acquire vso.licensing vso.extension_manage", authorizationId: authorizationId, accessId: accessId);
        }
        else
        {
          IVssRequestContext context = this.TfsRequestContext.Elevate();
          IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
          IVssRequestContext requestContext = context;
          DateTime? nullable3 = new DateTime?(DateTime.UtcNow.AddHours(1.0));
          Guid? nullable4 = new Guid?(guid);
          Guid? clientId = new Guid?();
          Guid? userId = nullable4;
          DateTime? validTo = nullable3;
          Guid? authorizationId = new Guid?();
          Guid? accessId = new Guid?();
          sessionTokenResult = service.IssueSessionToken(requestContext, clientId, userId, validTo: validTo, scope: "vso.commerce.write vso.gallery_acquire vso.licensing vso.extension_manage", authorizationId: authorizationId, accessId: accessId);
        }
      }
      tokenResult.sessionToken = new GallerySessionToken()
      {
        TokenKey = sessionTokenResult.SessionToken.Token,
        ValidTo = sessionTokenResult.SessionToken.ValidTo
      };
      return tokenResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetToken(
      Guid? requestTenant,
      Uri actionUri = null,
      bool useNewTokenAcquisitionExperience = true)
    {
      TokenRequestResult tokenResult = this.GetTokenResult(requestTenant, actionUri, useNewTokenAcquisitionExperience);
      JsonNetResult token = new JsonNetResult();
      token.Data = (object) tokenResult;
      return (ActionResult) token;
    }

    private bool ShouldUseNewTokenAcquisitionExperience(bool useNewTokenAcquisitionExperience)
    {
      bool flag1 = false;
      List<string> source = new List<string>()
      {
        "ms.vs-professional-annual",
        "ms.vs-professional-monthly",
        "ms.vs-enterprise-annual",
        "ms.vs-enterprise-monthly",
        "ms.xamarin-university"
      };
      bool flag2 = false;
      string localPath = this.Request?.UrlReferrer?.LocalPath;
      string query = this.Request?.UrlReferrer?.Query;
      if (!string.IsNullOrEmpty(localPath) && !string.IsNullOrEmpty(query))
      {
        NameValueCollection queryString = HttpUtility.ParseQueryString(query);
        if (queryString != null)
        {
          string a = queryString["itemName"];
          if (source.Contains<string>(a, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            flag1 = true;
          if (string.Equals(a, "ms.vss-testmanager-web", StringComparison.OrdinalIgnoreCase))
            flag2 = true;
        }
      }
      int num1;
      if ((this.GetCookieOrFeatureFlagStatus("EnableNewTokenAcquisitionExperience", "Microsoft.VisualStudio.Services.Gallery.EnableNewTokenAcquisitionExperience") ? 1 : (this.GetCookieOrFeatureFlagStatus("EnableNewTokenAcquisitionExperienceForOffers", "Microsoft.VisualStudio.Services.Gallery.EnableNewTokenAcquisitionExperienceForOffers") & flag1 ? 1 : (this.GetCookieOrFeatureFlagStatus("EnableNewTokenAcquisitionExperienceForTestManager", "Microsoft.VisualStudio.Services.Gallery.EnableNewTokenAcquisitionExperienceForTestManager") & flag2 ? 1 : 0))) != 0)
      {
        Uri urlReferrer = this.Request.UrlReferrer;
        if ((object) urlReferrer == null)
        {
          num1 = 0;
        }
        else
        {
          int? nullable = urlReferrer.LocalPath?.IndexOf("acquisition", StringComparison.OrdinalIgnoreCase);
          int num2 = 0;
          num1 = nullable.GetValueOrDefault() >= num2 & nullable.HasValue ? 1 : 0;
        }
      }
      else
        num1 = 0;
      int num3 = useNewTokenAcquisitionExperience ? 1 : 0;
      return (num1 & num3) != 0;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetTenants()
    {
      IAadTenantProvider extension = this.TfsRequestContext.GetExtension<IAadTenantProvider>();
      JsonNetResult tenants = new JsonNetResult();
      tenants.Data = (object) extension.GetTenants(this.TfsRequestContext);
      return (ActionResult) tenants;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult TenantRedirect(
      Guid requestedTenantId,
      string redirectActionName = null,
      string redirectRouteName = null,
      string routeData = null)
    {
      Guid property = this.TfsRequestContext.GetUserIdentity().GetProperty<Guid>("Domain", Guid.Empty);
      if (requestedTenantId != property)
      {
        string targetTenant = requestedTenantId == Guid.Empty ? "live.com" : requestedTenantId.ToString();
        RouteValueDictionary routeData1 = (RouteValueDictionary) null;
        if (routeData != null)
        {
          routeData1 = new RouteValueDictionary();
          string str1 = routeData;
          char[] chArray1 = new char[1]{ ';' };
          foreach (string str2 in str1.Split(chArray1))
          {
            char[] chArray2 = new char[1]{ '=' };
            string[] strArray = str2.Split(chArray2);
            if (strArray.Length == 2)
            {
              string key = strArray[0];
              string str3 = strArray[1];
              if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(str3))
                routeData1[key] = (object) str3;
            }
          }
        }
        JsonNetResult jsonNetResult = new JsonNetResult();
        jsonNetResult.Data = (object) this.ControllerHelper.GetAADSignInRedirectUrl(targetTenant, string.IsNullOrEmpty(redirectActionName) ? this.ConstructRouteUrl(redirectRouteName, routeData1) : this.ConstructActionUrl(redirectActionName, routeData1));
        return (ActionResult) jsonNetResult;
      }
      JsonNetResult jsonNetResult1 = new JsonNetResult();
      jsonNetResult1.Data = (object) string.Empty;
      return (ActionResult) jsonNetResult1;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetSubscriptionId(string hiddenAccountId)
    {
      Guid? nullable = new Guid?();
      ISubscriptionAccount subscriptionAccount = this.CommerceDataProvider.GetSubscriptionAccount(this.TfsRequestContext.Elevate(), new Guid(hiddenAccountId), AccountProviderNamespace.OnPremise);
      nullable = subscriptionAccount == null ? new Guid?(Guid.Empty) : subscriptionAccount.SubscriptionId;
      JsonNetResult subscriptionId = new JsonNetResult();
      subscriptionId.Data = (object) nullable;
      return (ActionResult) subscriptionId;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetPurchaseQuantityDetails(
      string accountId,
      string extensionName,
      bool getNextMonthQuantity = true)
    {
      PurchaseQuantityDetails purchaseQuantityDetails1 = new PurchaseQuantityDetails();
      IOfferSubscription subscriptionDetails1 = this.CommerceDataProvider.GetOfferSubscriptionDetails(this.TfsRequestContext.Elevate(), extensionName, new Guid(accountId), this.m_clientFactory);
      if (subscriptionDetails1 != null)
      {
        purchaseQuantityDetails1.CurrentQuantity = (long) (subscriptionDetails1.CommittedQuantity - subscriptionDetails1.IncludedQuantity);
        if (subscriptionDetails1.IsTrialOrPreview)
          purchaseQuantityDetails1.TrialEndDate = subscriptionDetails1.TrialExpiryDate;
        else
          purchaseQuantityDetails1.RenewalDate = new DateTime?(subscriptionDetails1.ResetDate);
        purchaseQuantityDetails1.IncludedQuantity = (long) subscriptionDetails1.IncludedQuantity;
        purchaseQuantityDetails1.MaximumQuantity = (long) subscriptionDetails1.MaximumQuantity;
      }
      if (getNextMonthQuantity)
      {
        IOfferSubscription subscriptionDetails2 = this.CommerceDataProvider.GetOfferSubscriptionDetails(this.TfsRequestContext.Elevate(), extensionName, new Guid(accountId), this.m_clientFactory, true);
        if (subscriptionDetails2 != null)
          purchaseQuantityDetails1.NextMonthQuantity = (long) (subscriptionDetails2.CommittedQuantity - subscriptionDetails2.IncludedQuantity);
      }
      JsonNetResult purchaseQuantityDetails2 = new JsonNetResult();
      purchaseQuantityDetails2.Data = (object) purchaseQuantityDetails1;
      return (ActionResult) purchaseQuantityDetails2;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PublisherReports(string publisherName, string extensionName)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, GalleryResources.OnPremUnsupportedText);
      PublishedExtensionResult publishedExtensionResult = new PublishedExtensionResult();
      PublishedExtensionResult publishedExtension;
      try
      {
        publishedExtension = this.DetailsHelper.GetPublishedExtension(GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName), true, true);
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(404, ex.Message);
      }
      if (publishedExtension != null)
      {
        if (publishedExtension.Extension != null)
        {
          try
          {
            IExtensionDailyStatsService service = this.TfsRequestContext.GetService<IExtensionDailyStatsService>();
            DateTime dateTime = DateTime.UtcNow.AddDays(-30.0);
            IVssRequestContext tfsRequestContext = this.TfsRequestContext;
            string publisherName1 = publisherName;
            string extensionName1 = extensionName;
            int? count = new int?();
            DateTime? afterDate = new DateTime?(dateTime);
            string lastContactDetails = GalleryServiceConstants.LastContactDetails;
            IEnumerable<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent> source;
            if (service.GetExtensionEvents(tfsRequestContext, publisherName1, extensionName1, count, afterDate, "uninstall", lastContactDetails).Events.TryGetValue("uninstall", out source))
            {
              if (source != null)
              {
                if (source.Count<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent>() > 0)
                  this.ViewData["uninstallEvents"] = (object) source;
              }
            }
          }
          catch (ExtensionDailyStatsAccessDeniedException ex)
          {
            throw new HttpException(401, ex.Message);
          }
          this.ViewData["disablecontactoption"] = (object) this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<bool>(this.TfsRequestContext, (RegistryQuery) ("/Configuration/Service/Gallery/DisableContactOption/" + publisherName), false, false);
          PublishedExtension extension = publishedExtension.Extension;
          GalleryUtil.LoadExtensionDeploymentType(extension);
          this.ViewData["vss-extension"] = (object) extension;
          this.ViewData["can-update-extension"] = (object) GallerySecurity.HasExtensionPermission(this.TfsRequestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
          if (extension != null)
          {
            if (extension.Versions != null)
              this.DetailsHelper.LoadExtensionProperties(extension);
            this.DetailsHelper.LoadCommerceData(extension, extension.GetFullyQualifiedName());
          }
          this.ViewData["target-platforms"] = (object) this.PopulateTargetPlatforms();
          this.ViewData["manage-vscode-records-per-page"] = (object) GalleryServerUtil.GetManageVSCodeReportsRecordsCountPerPage(this.TfsRequestContext);
          return (ActionResult) this.View(nameof (PublisherReports));
        }
      }
      throw new HttpException(404, GalleryResources.PageNotFoundError);
    }

    protected ViewResult ComingSoonView() => this.View("ComingSoon");

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Changelog(string itemName)
    {
      PublishedExtensionResult publishedExtension = this.ControllerHelper.GetPublishedExtension(itemName, false);
      if (publishedExtension.AuthenticationRequired)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      PublishedExtension extension = publishedExtension.Extension != null ? publishedExtension.Extension : throw new HttpException(404, WACommonResources.PageNotFound);
      if (extension != null)
      {
        string changelog = this.GetChangelog(extension, publishedExtension.ExtensionAssetsToken);
        this.ViewData["vss-item-changelog"] = !changelog.Equals(string.Empty) ? (object) changelog : throw new HttpException(404, WACommonResources.PageNotFound);
      }
      this.ViewData["vss-extension"] = (object) extension;
      this.ViewData["vss-extension-token"] = (object) publishedExtension.ExtensionAssetsToken;
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (Changelog));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Privacy(string itemName)
    {
      PublishedExtensionResult publishedExtension = this.ControllerHelper.GetPublishedExtension(itemName, false);
      if (publishedExtension.AuthenticationRequired)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      PublishedExtension extension = publishedExtension.Extension != null ? publishedExtension.Extension : throw new HttpException(404, WACommonResources.PageNotFound);
      if (extension != null)
      {
        string privacy = this.GetPrivacy(extension, publishedExtension.ExtensionAssetsToken);
        this.ViewData["vss-item-privacy"] = !string.IsNullOrEmpty(privacy) ? (object) privacy : throw new HttpException(404, WACommonResources.PageNotFound);
      }
      this.ViewData["vss-extension"] = (object) extension;
      this.ViewData["vss-extension-token"] = (object) publishedExtension.ExtensionAssetsToken;
      this.ControllerHelper.PopulateGeneralInfo();
      return (ActionResult) this.View(nameof (Privacy));
    }

    private void PopulateViewDataKO(string product, int numCarousel)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        product = "vsts";
        string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
        this.ViewData["market-browse-url"] = (object) (this.ControllerHelper.GetMarketBrowseURL() + "&serverKey=" + token);
        this.ViewData["gallery-onprem-homepage"] = (object) true;
      }
      else
        this.ViewData["gallery-onprem-homepage"] = (object) false;
      if (string.IsNullOrWhiteSpace(product))
        product = this.ControllerHelper.GetProductFromCookieOrDefaultTab();
      Dictionary<string, object> result = new Dictionary<string, object>();
      if (product.Equals("subscriptions", StringComparison.OrdinalIgnoreCase))
        result.Add("subscriptions", this.m_productExtensionsProvider.GetProductExtensions(this.TfsRequestContext, product));
      else
        this.ManipulateExtensionsResult(this.m_productExtensionsProvider.GetProductExtensions(this.TfsRequestContext, product) as ProductExtensions, result, product, numCarousel);
      this.ViewData["TabData"] = (object) result;
      this.SetMailAddress();
      this.ControllerHelper.PopulateGeneralInfo(true);
    }

    private void ManipulateExtensionsResult(
      ProductExtensions extensions,
      Dictionary<string, object> result,
      string product,
      int numCarousel)
    {
      if (extensions == null)
        return;
      int count = 6;
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions") && extensions != null)
        extensions.ExtensionsPerCategory?.RemoveAll((Predicate<ExtensionPerCategory>) (extensionsPerCategory => extensionsPerCategory.CategoryName == "TrendingDaily" || extensionsPerCategory.CategoryName == "TrendingMonthly"));
      if (numCarousel < extensions.ExtensionsPerCategory.Count)
        extensions.ExtensionsPerCategory.RemoveRange(numCarousel, extensions.ExtensionsPerCategory.Count - numCarousel);
      for (int index = 0; extensions != null && index < extensions.ExtensionsPerCategory.Count; ++index)
      {
        if (extensions.ExtensionsPerCategory[index].Extensions != null)
        {
          List<BaseExtensionItem> list = extensions.ExtensionsPerCategory[index].Extensions.Take<BaseExtensionItem>(count).ToList<BaseExtensionItem>();
          extensions.ExtensionsPerCategory[index].Extensions = list;
        }
      }
      result.Add(product, (object) extensions);
    }

    private async Task<VSTabViewDataKO> GetVSTabData(
      bool removeFirstSetCategories,
      bool sendOnlyFirstSetCategories,
      int numCarousel)
    {
      GalleryController galleryController = this;
      VSTabViewDataKO vsTabData1;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (galleryController.TfsRequestContext).ExecutionEnvironment.IsHostedDeployment)
      {
        // ISSUE: explicit non-virtual call
        VSTabData vsTabData2 = await galleryController.m_vsDataProvider.GetVSTabData(__nonvirtual (galleryController.TfsRequestContext));
        List<ExtensionPerCategory> extensionsPerCategory = new List<ExtensionPerCategory>()
        {
          galleryController.CreateExtensionPerCategory(vsTabData2.FeaturedExtensions, GalleryResources.FeaturedItems_Header),
          galleryController.CreateExtensionPerCategory(vsTabData2.MostPopularExtensions, GalleryCommonResources.MostPopular_Items_Header, true, "https://visualstudiogallery.msdn.microsoft.com/site/search?f%5B0%5D.Type=VisualStudioVersion&f%5B0%5D.Value=14.0&f%5B0%5D.Text=Visual%20Studio%202015&sortBy=Popularity"),
          galleryController.CreateExtensionPerCategory(vsTabData2.TopRatedExtensions, GalleryResources.TopRated_Items_Header, true, "https://visualstudiogallery.msdn.microsoft.com/site/search?f%5B0%5D.Type=VisualStudioVersion&f%5B0%5D.Value=14.0&f%5B0%5D.Text=Visual%20Studio%202015&sortBy=Ratings")
        };
        if (numCarousel < extensionsPerCategory.Count)
        {
          if (sendOnlyFirstSetCategories)
            extensionsPerCategory.RemoveRange(numCarousel, extensionsPerCategory.Count - numCarousel);
          else if (removeFirstSetCategories)
            extensionsPerCategory.RemoveRange(0, numCarousel);
        }
        else if (removeFirstSetCategories)
          extensionsPerCategory = new List<ExtensionPerCategory>();
        vsTabData1 = new VSTabViewDataKO(extensionsPerCategory, galleryController.PopulateCategories(vsTabData2.Categories));
      }
      else
        vsTabData1 = new VSTabViewDataKO(new List<ExtensionPerCategory>(), Array.Empty<VSCategoryKO>());
      return vsTabData1;
    }

    private VSCategoryKO[] PopulateCategories(VSCategory[] categories) => new List<VSCategoryKO>()
    {
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_Coding_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=RootCategory&f[0].Value=tools&f[0].Text=Tools&f[1].Type=SubCategory&f[1].Value=coding"
      },
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_Framework_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=RootCategory&f[0].Value=controls&f[0].Text=Controls&f[1].Type=SubCategory&f[1].Value=framework"
      },
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_Language_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=RootCategory&f[0].Value=tools&f[0].Text=Tools&f[1].Type=SubCategory&f[1].Value=languages"
      },
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_WinForms_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=RootCategory&f[0].Value=controls&f[0].Text=Controls&f[1].Type=SubCategory&f[1].Value=windowsformscontrols"
      },
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_TeamDevelopment_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?f[0].Type=RootCategory&f[0].Value=tools&f[0].Text=Tools&f[1].Type=SubCategory&f[1].Value=teamdevelopment"
      },
      new VSCategoryKO()
      {
        Title = GalleryResources.VSCategories_SeeAll_Title,
        Link = "https://visualstudiogallery.msdn.microsoft.com/site/search?query=&f[0].Value=&f[0].Type=SearchText"
      }
    }.ToArray();

    private string GetDetailsPageUrl(string itemName) => GalleryHtmlExtensions.GetGalleryAbsoluteUrl(this.TfsRequestContext, "items") + "?itemName=" + itemName;

    private ExtensionPerCategory CreateExtensionPerCategory(
      List<VSSearchResult> searchResult,
      string categoryName,
      bool hasMoreExtensions = false,
      string seeMoreLink = "")
    {
      return new ExtensionPerCategory()
      {
        CategoryName = categoryName,
        Extensions = searchResult.Where<VSSearchResult>((Func<VSSearchResult, bool>) (ex => ex != null)).Select<VSSearchResult, BaseExtensionItem>((Func<VSSearchResult, BaseExtensionItem>) (ex => (BaseExtensionItem) new VSExtensionItem(ex))).ToList<BaseExtensionItem>(),
        HasMoreExtensions = hasMoreExtensions,
        SeeMoreLink = seeMoreLink
      };
    }

    private bool IsAuthenticatedWithAAD() => this.ControllerHelper.GetAuthenticatedTenantId() != Guid.Empty;

    private string GetEULA(
      PublishedExtension extension,
      string assetToken,
      string requestedVersion)
    {
      string eula = string.Empty;
      string str;
      if (requestedVersion == null)
      {
        List<ExtensionVersion> versions = extension.Versions;
        str = versions != null ? versions.First<ExtensionVersion>().Version : (string) null;
      }
      else
        str = requestedVersion;
      string version = str;
      if (extension != null && version != null)
        eula = this.ControllerHelper.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.License", assetToken);
      return eula;
    }

    private string GetMsEULA(PublishedExtension extension, string assetToken)
    {
      string msEula = string.Empty;
      string path = this.HttpContext.Server.MapPath("~/_views/Shared/MsDefaultEula.md");
      if (System.IO.File.Exists(path))
        msEula = System.IO.File.ReadAllText(path);
      return msEula;
    }

    private Uri GetActionUri(
      string actionName,
      Guid subscriptionId,
      RouteValueDictionary exitingRoutes = null,
      bool isFreeInstallFlow = false)
    {
      return this.GetActionUri(actionName, subscriptionId, false, exitingRoutes, isFreeInstallFlow);
    }

    private Uri GetActionUri(
      string actionName,
      Guid subscriptionId,
      bool isNewAzureSubscription,
      RouteValueDictionary exitingRoutes = null,
      bool isFreeInstallFlow = false)
    {
      HttpCookie cookie = this.Request.Cookies["CommerceMarketplaceSubscriptionCookie"];
      if (cookie == null)
      {
        this.TfsRequestContext.Trace(12060023, TraceLevel.Warning, this.AreaName, this.LayerName, string.Format("The purchase cookie is null, SubscriptionId: {0}", (object) subscriptionId));
        return new Uri(this.Url.Action("Gallery", (string) null, (RouteValueDictionary) null, this.Request.Url.Scheme, this.Request.Url.Host));
      }
      string str1 = cookie["itemName"];
      if (str1 == null)
      {
        this.TfsRequestContext.Trace(12060024, TraceLevel.Warning, this.AreaName, this.LayerName, string.Format("The item name is missing from the cookie, SubscriptionId: {0}", (object) subscriptionId));
        return new Uri(this.Url.Action("Gallery", (string) null, (RouteValueDictionary) null, this.Request.Url.Scheme, this.Request.Url.Host));
      }
      RouteValueDictionary routeData = new RouteValueDictionary();
      routeData["itemName"] = (object) str1;
      routeData["install"] = (object) bool.TrueString;
      if (subscriptionId == Guid.Empty)
      {
        string input = cookie[nameof (subscriptionId)];
        if (input != null)
          Guid.TryParse(input, out subscriptionId);
      }
      if (subscriptionId != Guid.Empty)
        routeData[nameof (subscriptionId)] = (object) subscriptionId;
      string str2 = cookie["freeInstall"];
      bool result1;
      if (!string.IsNullOrEmpty("freeInstall") && bool.TryParse(str2, out result1))
        routeData["freeInstall"] = (object) result1;
      string str3 = cookie["skipBuy"];
      bool result2;
      if (!string.IsNullOrEmpty("skipBuy") && bool.TryParse(str3, out result2))
        routeData["skipBuy"] = (object) result2;
      string str4 = cookie["changeQuantity"];
      bool result3;
      if (!string.IsNullOrEmpty("changeQuantity") && bool.TryParse(str4, out result3))
        routeData["changeQuantity"] = (object) result3;
      string a = cookie["testCommerce"];
      if (a != null && string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase))
        routeData["testCommerce"] = (object) bool.TrueString;
      string str5 = cookie["accountId"];
      if (str5 != null && (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || !this.ViewData.ContainsKey("server-context")))
      {
        this.TfsRequestContext.Trace(12060026, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("The accountId is present in the cookie. The user should be purchasing an extension, SubscriptionId: {0}, GalleryId: {1}, AccountId: {2}", (object) subscriptionId, (object) str1, (object) str5));
        routeData["accountId"] = (object) str5;
      }
      else
        this.TfsRequestContext.Trace(12060026, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("The accountId is not present in the cookie. This should only happen when purchasing a VSSubscription/Bundle, SubscriptionId: {0}, GalleryId: {1}", (object) subscriptionId, (object) str1));
      string str6 = cookie.Values["newAzureSub"];
      if (isNewAzureSubscription || str6 != null)
        routeData["newAzureSub"] = (object) "true";
      string str7 = cookie.Values["referrer"];
      if (!string.IsNullOrEmpty(str7))
        routeData["referrer"] = (object) str7;
      if (exitingRoutes != null)
      {
        foreach (KeyValuePair<string, object> exitingRoute in exitingRoutes)
        {
          if (string.Equals(exitingRoute.Key, "itemName", StringComparison.OrdinalIgnoreCase) && exitingRoute.Value != null)
            str1 = exitingRoute.Value.ToString();
          if (string.Equals(exitingRoute.Key, nameof (subscriptionId), StringComparison.OrdinalIgnoreCase) && exitingRoute.Value != null)
            Guid.TryParse(exitingRoute.Value.ToString(), out subscriptionId);
          routeData[exitingRoute.Key] = exitingRoute.Value;
        }
      }
      this.TfsRequestContext.Trace(12060027, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Sending the user back to the install page. SubscriptionId: {0}, GalleryId: {1}", (object) subscriptionId, (object) str1));
      return this.ConstructActionUrl(actionName, routeData);
    }

    private string GetChangelog(PublishedExtension extension, string assetToken)
    {
      string changelog = string.Empty;
      if (extension != null && extension.Versions != null)
      {
        string version = extension.Versions.First<ExtensionVersion>().Version;
        changelog = this.ControllerHelper.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.Changelog", assetToken);
      }
      return changelog;
    }

    private string GetPrivacy(PublishedExtension extension, string assetToken)
    {
      string privacy = string.Empty;
      if (extension != null && extension.Versions != null)
      {
        string version = extension.Versions.First<ExtensionVersion>().Version;
        privacy = this.ControllerHelper.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.PrivacyPolicy", assetToken);
      }
      return privacy;
    }

    protected virtual Uri ConstructActionUrl(string actionName, RouteValueDictionary routeData) => new Uri(this.Url.Action(actionName, (string) null, routeData, this.Request.Url.Scheme, this.Request.Url.Host));

    protected virtual Uri ConstructRouteUrl(string routeName, RouteValueDictionary routeData) => new Uri(this.Url.RouteUrl(routeName, routeData, this.Request.Url.Scheme, this.Request.Url.Host));

    private bool IsFreeInstallWorkflow(PublishedExtension extension)
    {
      bool result1 = false;
      bool result2 = false;
      return extension != null && GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) && !extension.IsPaid() || (bool.TryParse(this.Request?.QueryString?["freeInstall"], out result1) || bool.TryParse(this.Request?.QueryString?["skipBuy"], out result2)) && result1 | result2;
    }

    private bool ShouldRedirectInConnectedContext(PublishedExtension extension)
    {
      bool flag = false;
      if (this.ViewData.ContainsKey("server-context"))
      {
        if (GalleryUtil.IsOnlyVSTSInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || GalleryUtil.InstallationTargetsHasVSTSResource((IEnumerable<InstallationTarget>) extension.InstallationTargets) || extension.IsPaid())
          flag = true;
      }
      else
        flag = true;
      return flag;
    }

    private bool ShouldRedirectToNewExperience(PublishedExtension extension, string installContext) => this.ShouldRedirectToNewExperienceInOnPrem(extension, installContext) || this.ShouldRedirectToNewExperienceInHosted(extension);

    private bool ShouldRedirectForAnnualSubscription(string itemName) => itemName.Equals("ms.vs-professional-annual", StringComparison.OrdinalIgnoreCase) || itemName.Equals("ms.vs-enterprise-annual", StringComparison.OrdinalIgnoreCase);

    private bool ShouldRedirectToNewExperienceInHosted(PublishedExtension extension) => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.ShouldRedirectInConnectedContext(extension) && (GalleryUtil.InstallationTargetsHasVSTSResource((IEnumerable<InstallationTarget>) extension.InstallationTargets) || extension.IsFirstPartyAndPaidVSTS() || GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || this.IsThirdPartyExtensionCurrent(extension) || GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) extension.InstallationTargets));

    private bool ShouldRedirectToNewExperienceInOnPrem(
      PublishedExtension extension,
      string installContext)
    {
      return this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && (installContext != null || extension != null && GalleryUtil.IsVSSExtensionInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets)) && this.IsNewExperienceEnabledForOnPrem();
    }

    private bool IsThirdPartyExtensionCurrent(PublishedExtension extension)
    {
      if (string.Equals(extension.Publisher.PublisherName, "spartez", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "agile-cards", StringComparison.OrdinalIgnoreCase) || string.Equals(extension.Publisher.PublisherName, "Berichthaus", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "TfsTimetracker", StringComparison.OrdinalIgnoreCase) || string.Equals(extension.Publisher.PublisherName, "mskold", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "mskold-PRO-EnhancedExport", StringComparison.OrdinalIgnoreCase) || string.Equals(extension.Publisher.PublisherName, "agile-extensions", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "backlog-essentials", StringComparison.OrdinalIgnoreCase) || string.Equals(extension.Publisher.PublisherName, "MathewAn", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "smartwit", StringComparison.OrdinalIgnoreCase) || string.Equals(extension.Publisher.PublisherName, "ndepend", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "ndependextension", StringComparison.OrdinalIgnoreCase))
        return true;
      return string.Equals(extension.Publisher.PublisherName, "ripplerock", StringComparison.OrdinalIgnoreCase) && string.Equals(extension.ExtensionName, "senseadaptvsts", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsNewExperienceEnabledForOnPrem() => this.GetCookieOrFeatureFlagStatus("EnableNewAcquisitionOnPremExperience", "Microsoft.VisualStudio.Services.Gallery.EnableNewAcquisitionOnPremExperience");

    private bool IsCookieSetToFalse(string cookieName)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(cookieName))
      {
        string str = this.Request?.Cookies?[cookieName]?.Value;
        bool result;
        if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out result))
          flag = !result;
      }
      return flag;
    }

    private bool GetCookieOrFeatureFlagStatus(
      string cookieName,
      string featureFlagName = "",
      string queryParamName = null)
    {
      return this.DetailsHelper.GetCookieOrFeatureFlagStatus(cookieName, featureFlagName, queryParamName);
    }

    private string GetNewAcquisitionExperienceRedirectUrl(PublishedExtension extension)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
      this.RemoveParamIfPresent(queryString, "install");
      if (!string.IsNullOrEmpty(queryString["accountId"]))
      {
        queryString["targetId"] = queryString["accountId"];
        queryString.Remove("accountId");
      }
      if (this.IsFreeInstallWorkflow(extension))
        this.RemoveParamIfPresent(queryString, "acquisitionOption");
      else if (string.IsNullOrEmpty(queryString["acquisitionOption"]))
        queryString.Add("acquisitionOption", "2");
      this.RemoveParamIfPresent(queryString, "serverKey");
      this.RemoveParamIfPresent(queryString, "skipBuy");
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.RemoveParamIfPresent(queryString, "freeInstall");
      return this.GetRedirectUrlWithParams(!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? "_gallery/acquisition" : "/acquisition", queryString);
    }

    private string GetRedirectUrlWithParams(string relativeUrl, NameValueCollection queryParams) => new UriBuilder((!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? (object) new Uri(this.TfsRequestContext.GetClient<GalleryHttpClient>().BaseAddress, relativeUrl) : (object) new Uri(GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker), relativeUrl)).ToString())
    {
      Query = queryParams.ToString()
    }.ToString();

    private string GetRedirectUrlToDetailsPage()
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
      this.RemoveParamIfPresent(queryString, "install");
      this.RemoveParamIfPresent(queryString, "serverKey");
      return this.GetRedirectUrlWithParams("/items", queryString);
    }

    private string GetManagePagePublisherRedirectUrl(string publisherName)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
      return new UriBuilder(this.ConstructRouteUrl("GalleryPublisher", new RouteValueDictionary()
      {
        [nameof (publisherName)] = (object) publisherName
      }).ToString())
      {
        Query = queryString.ToString()
      }.ToString();
    }

    private string GetPublisherProfileRedirectUrl(string publisherName)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.Query);
      return new UriBuilder(this.ConstructRouteUrl("PublisherProfile", new RouteValueDictionary()
      {
        [nameof (publisherName)] = (object) publisherName
      }).ToString())
      {
        Query = queryString.ToString()
      }.ToString();
    }

    private string GetPublisherCreateRedirectUrl() => new UriBuilder(this.ConstructRouteUrl("PublisherCreate", (RouteValueDictionary) null).ToString())
    {
      Query = "managePageRedirect=true"
    }.ToString();

    private void RemoveParamIfPresent(NameValueCollection queryParams, string paramName)
    {
      if (queryParams == null || string.IsNullOrEmpty(queryParams[paramName]))
        return;
      queryParams.Remove(paramName);
    }

    private void PublishTelemetryForAcquireToken(string featureName, ClientTraceData ctData = null)
    {
      ClientTraceService service = this.TfsRequestContext.GetService<ClientTraceService>();
      if (ctData == null)
        ctData = new ClientTraceData();
      string journeyId = this.GetJourneyId();
      if (!string.IsNullOrEmpty(journeyId))
        ctData.Add("JourneyId", (object) journeyId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string marketPlace = MarketPlaceCustomerIntelligenceArea.MarketPlace;
      string feature = featureName;
      ClientTraceData properties = ctData;
      service.Publish(tfsRequestContext, marketPlace, feature, properties);
    }

    private SessionTokenResult GetSessionTokenWithoutRedirection(
      Guid? requestTenant,
      string tokenScopes)
    {
      SessionTokenResult withoutRedirection = (SessionTokenResult) null;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IAadTokenService service1 = this.TfsRequestContext.GetService<IAadTokenService>();
      try
      {
        string journeyId = this.GetJourneyId();
        if (string.IsNullOrEmpty(journeyId))
          journeyId = string.Empty;
        this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Verbose, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Begin acquire token request. Journey id = {0}", (object) journeyId)));
        this.PublishTelemetryForAcquireToken("AcquireTokenRequest");
        JwtSecurityToken aadToken = service1.AcquireToken(vssRequestContext, service1.DefaultResource, requestTenant.Value.ToString(), this.TfsRequestContext.GetAuthenticatedDescriptor());
        this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Verbose, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Acquire Token response received. Journey id = {0}", (object) journeyId)));
        this.PublishTelemetryForAcquireToken("AcquireTokenResponse");
        IdentityDescriptor identityDescriptor = AuthenticationHelpers.GetAadIdentityDescriptorFromAadToken(this.TfsRequestContext, aadToken);
        if (identityDescriptor != (IdentityDescriptor) null)
        {
          this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Identity type = {0}. Identity identifier = {1}. JourneyId = {2}", (object) identityDescriptor.IdentityType, (object) identityDescriptor.Identifier, (object) journeyId)));
          Microsoft.VisualStudio.Services.Identity.Identity resolvedIdentity = vssRequestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActor(vssRequestContext, identityDescriptor);
          if (resolvedIdentity != null)
          {
            this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Identity resolved. Identity info = {0}", (object) resolvedIdentity)));
            if (this.IsRefreshTokenPresent(service1, vssRequestContext, resolvedIdentity, requestTenant))
            {
              this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Verbose, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Refresh token present. JourneyId = {0}", (object) journeyId)));
              IDelegatedAuthorizationService service2 = this.TfsRequestContext.GetService<IDelegatedAuthorizationService>();
              IVssRequestContext requestContext = vssRequestContext;
              DateTime? nullable1 = new DateTime?(DateTime.UtcNow.AddHours(1.0));
              string str = tokenScopes;
              Guid? nullable2 = new Guid?(resolvedIdentity.Id);
              Guid? clientId = new Guid?();
              Guid? userId = nullable2;
              DateTime? validTo = nullable1;
              string scope = str;
              Guid? authorizationId = new Guid?();
              Guid? accessId = new Guid?();
              withoutRedirection = service2.IssueSessionToken(requestContext, clientId, userId, validTo: validTo, scope: scope, authorizationId: authorizationId, accessId: accessId);
              if (withoutRedirection != null)
              {
                this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Verbose, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Acquired session token. JourneyId = {0}", (object) journeyId)));
                this.PublishTelemetryForAcquireToken("AcquireTokenSuccess");
              }
              else
                this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Acquired session token for resolved identity is null. JourneyId = {0}", (object) journeyId)));
            }
            else
              this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Refresh token is not present. Identity = {0}. JourneyId = {1}", (object) resolvedIdentity, (object) journeyId)));
          }
          else
            this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Resolved identity is null. JourneyId = {0}", (object) journeyId)));
        }
        else
          this.TfsRequestContext.TraceConditionally(12060054, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetSessionTokenWithoutRedirection: Identity descriptor is null. JourneyId = {0}", (object) journeyId)));
        if (withoutRedirection == null)
          this.PublishTelemetryForAcquireToken("AcquireTokenNoSessionToken");
      }
      catch (Exception ex)
      {
        ClientTraceData ctData = new ClientTraceData();
        ctData.Add(MarketPlaceIntelligencePropertyName.ErrorDetails, (object) ex.ToString());
        ctData.Add(MarketPlaceIntelligencePropertyName.ErrorType, (object) ex.GetType().ToString());
        this.PublishTelemetryForAcquireToken("AcquireTokenException", ctData);
        this.TfsRequestContext.TraceConditionally(12060055, TraceLevel.Warning, this.AreaName, this.LayerName, (Func<string>) (() => ex.Message));
      }
      return withoutRedirection;
    }

    private bool IsRefreshTokenPresent(
      IAadTokenService tokenService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity resolvedIdentity,
      Guid? requestTenant)
    {
      bool isRefreshTokenPresent = false;
      string journeyId = string.Empty;
      requestContext.TraceEnter(12060050, "Gallery", this.LayerName, nameof (IsRefreshTokenPresent));
      try
      {
        journeyId = this.GetJourneyId();
        if (string.IsNullOrEmpty(journeyId))
          journeyId = string.Empty;
        tokenService.AcquireToken(requestContext, tokenService.DefaultResource, requestTenant.Value.ToString(), resolvedIdentity.Descriptor);
        isRefreshTokenPresent = true;
      }
      catch (Exception ex)
      {
        if (ex != null)
          requestContext.TraceConditionally(12060053, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "JourneyId = {0}. Exception type = {1}. Exception message = {2}", (object) journeyId, (object) ex.GetType().ToString(), (object) ex.Message)));
        isRefreshTokenPresent = false;
      }
      finally
      {
        requestContext.TraceConditionally(12060052, TraceLevel.Info, "Gallery", this.LayerName, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User context = {0}. Request tenant = {1}. Resolved identity = {2}. Journey id = {3}. IsRefreshTokenPresent = {4}", (object) requestContext.UserContext, (object) requestTenant.Value, (object) resolvedIdentity.Descriptor, (object) journeyId, (object) isRefreshTokenPresent)));
      }
      return isRefreshTokenPresent;
    }

    private string GetJourneyId() => this.Request?.Cookies?["Gallery-Service-NewJourneyId"]?.Value;
  }
}
