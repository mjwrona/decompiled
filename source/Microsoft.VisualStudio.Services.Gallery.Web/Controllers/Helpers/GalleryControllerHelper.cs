// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.GalleryControllerHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class GalleryControllerHelper
  {
    protected const string c_InstalltargetCookie = "targetId";
    private const string c_ABTestExperimentID = "/Configuration/Service/Gallery/ABTestExperimentID";
    private const string c_ABTestAccountID = "/Configuration/Service/Gallery/ABTestAccountID";
    private const string c_DefaultTabRegistryPath = "/Gallery/Web/Settings/DefaultTab";
    private const string SELECTED_TAB_COOKIE_NAME = "Market_SelectedTab";
    private const string c_MarketpPlaceUrlRegistryPath = "/Configuration/Service/Gallery/MarketplaceRootURL";
    private const string c_marketplaceBrowseUrlRegistry = "/Configuration/Service/Gallery/MarketplaceURL";
    private const string c_useVirtualPath = "/Configuration/Service/Gallery/UseVirtualPath";
    private const string c_serverContext = "serverKey";
    private const string c_nullServKey = "null";
    private const string c_PreviouslyRequestedTenantCookie = "previouslyRequestedTenant";
    private const string c_onPremHeaderIcon = "Header/vsonline.png";
    private Dictionary<string, bool> _featureFlags = new Dictionary<string, bool>();
    private bool m_isServerKeyValid = true;
    public const int c_DefaultExtensionQueryPageSize = 18;
    public const int c_DefaultExtensionQueryPageNumber = 1;

    public virtual Microsoft.VisualStudio.Services.Gallery.Web.GalleryController Controller { get; private set; }

    public GalleryControllerHelper(Microsoft.VisualStudio.Services.Gallery.Web.GalleryController controller) => this.Controller = controller;

    public virtual IVssRequestContext TfsRequestContext => this.Controller.TfsRequestContext;

    public virtual WebContext WebContext => this.Controller.WebContext;

    public virtual ViewDataDictionary ViewData => this.Controller.ViewData;

    public virtual HttpContextBase HttpContext => this.Controller.HttpContext;

    public virtual HttpRequestBase Request => this.Controller.Request;

    public virtual HttpResponseBase Response => this.Controller.Response;

    public virtual IPageContextProvider PageContextProvider => this.Controller.PageContextProvider;

    public virtual string AreaName => this.Controller.AreaName;

    public virtual string LayerName => this.Controller.LayerName;

    public PublishedExtensionResult GetPublishedExtension(
      string extensionName,
      bool ensureSharedAccounts,
      bool getExtensionProperties = false,
      bool includeOnlyValidated = true,
      bool useCache = true,
      bool includeLatestVersionsOnly = true)
    {
      PublishedExtensionResult result = new PublishedExtensionResult();
      if (!string.IsNullOrEmpty(extensionName))
      {
        string[] strArray = extensionName.Split(new char[1]
        {
          '.'
        }, 2);
        if (strArray.Length == 2)
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
          IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
          ExtensionQueryFlags flags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeMetadata;
          if (includeOnlyValidated)
            flags |= ExtensionQueryFlags.ExcludeNonValidated;
          if (getExtensionProperties)
            flags |= ExtensionQueryFlags.IncludeVersionProperties;
          if (includeLatestVersionsOnly)
            flags |= ExtensionQueryFlags.IncludeLatestVersionOnly;
          bool useCache1 = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended") & useCache;
          try
          {
            result.Extension = service.QueryExtension(this.TfsRequestContext, strArray[0], strArray[1], (string) null, flags, (string) null, useCache1);
            if (!includeLatestVersionsOnly)
            {
              List<ExtensionVersion> versions = result.Extension.Versions;
              int val2 = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/ItemDetails/MaxPreviousVersionsToBeRetrivedInVersionHistory", 5);
              Dictionary<string, List<ExtensionVersion>> dictionary = result.Extension.Versions.GroupBy<ExtensionVersion, string>((Func<ExtensionVersion, string>) (x => x.Version)).ToDictionary<IGrouping<string, ExtensionVersion>, string, List<ExtensionVersion>>((Func<IGrouping<string, ExtensionVersion>, string>) (x => x.Key), (Func<IGrouping<string, ExtensionVersion>, List<ExtensionVersion>>) (x => x.ToList<ExtensionVersion>()));
              int num1 = Math.Min(dictionary.Count, val2);
              List<ExtensionVersion> extensionVersionList = new List<ExtensionVersion>();
              int num2 = 0;
              foreach (string key in dictionary.Keys)
              {
                if (num2 < val2)
                {
                  extensionVersionList.AddRange((IEnumerable<ExtensionVersion>) dictionary.GetValueOrDefault<string, List<ExtensionVersion>>(key, (List<ExtensionVersion>) null));
                  ++num2;
                }
                else
                  break;
              }
              result.Extension.Versions = extensionVersionList;
              string version1 = result.Extension.Versions == null || !result.Extension.Versions.Any<ExtensionVersion>() ? (string) null : result.Extension.Versions[0].Version;
              for (int index = 1; index < num1; ++index)
              {
                ExtensionVersion version2 = result.Extension.Versions[index];
                if (!(result.Extension.Versions[index].Version == version1))
                  result.Extension.Versions[index] = new ExtensionVersion()
                  {
                    Version = version2.Version,
                    TargetPlatform = version2.TargetPlatform,
                    LastUpdated = version2.LastUpdated,
                    Flags = version2.Flags
                  };
              }
            }
            if (ensureSharedAccounts)
            {
              if (!result.Extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
              {
                flags |= ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations;
                PublishedExtension extension = service.QueryExtension(this.TfsRequestContext.Elevate(), strArray[0], strArray[1], (string) null, flags, (string) null);
                this.GetSharedAccountDetails(service, extension, userIdentity.Id, ensureSharedAccounts, result);
              }
            }
          }
          catch (ExtensionDoesNotExistException ex)
          {
          }
          catch (AccessCheckException ex)
          {
            if (userIdentity == null)
            {
              result.AuthenticationRequired = true;
            }
            else
            {
              PublishedExtension extension = service.QueryExtension(this.TfsRequestContext.Elevate(), strArray[0], strArray[1], (string) null, flags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations, (string) null);
              this.GetSharedAccountDetails(service, extension, userIdentity.Id, ensureSharedAccounts, result);
              if (result.Extension == null)
                result.IsNotAuthenticated = true;
            }
          }
        }
      }
      return result;
    }

    public void PopulateGeneralInfo(bool addFeatureFlags = false, bool isVsipPartner = false)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        if (this.WebContext.Account != null)
          dictionary.Add("galleryUrl", (object) this.WebContext.Account.RelativeUri);
        else
          dictionary.Add("galleryUrl", (object) this.GetAbsolutePath("~/"));
      }
      catch
      {
        dictionary.Add("galleryUrl", (object) this.GetAbsolutePath("~/"));
      }
      dictionary.Add("resourcesPath", (object) this.PageContextProvider.GetResourcesPath(this.TfsRequestContext));
      dictionary.Add("isHosted", (object) this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment);
      if (addFeatureFlags)
      {
        this.setFeatureFlag("ShowVs2017Banner", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ShowVs2017Banner"));
        this.setFeatureFlag("ShowPublishExtensions", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ShowPublishExtensions"));
        this.setFeatureFlag("TileImpressionsHomePage", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.TileImpressionsHomePage"));
        this.setFeatureFlag("VsTrendingHomepage", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.VsTrendingHomepage"));
        this.setFeatureFlag("Vs2019Homepage", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Vs2019Homepage"));
        this.setFeatureFlag("DedupeHomepageExtensions", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions"));
        this.setFeatureFlag("EnableVsForMac", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsForMac"));
        this.setFeatureFlag("EnableCertifiedPublisherUIChanges", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCertifiedPublisherUIChanges"));
        this.setFeatureFlag("MarketplaceBrandingChanges", this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseNewBranding"));
        this.setFeatureFlag("EnableSSRForHomepageVSCode", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSSRForHomepageVSCode"));
        this.setFeatureFlag("EnableItemDetailsAFDCachingForVSCodeHomePage", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsAFDCachingForVSCodeHomePage"));
        this.setFeatureFlag("XamarinUniversityDisable", this.GetCookieOrFeatureFlagStatus("XamarinUniversityDisable", "Microsoft.VisualStudio.Services.Gallery.XamarinUniversityDisable"));
        this.setFeatureFlag("EnableNoFilterSearchHomepageVSIDE", this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableNoFilterSearchHomepageVSIDE"));
        dictionary.Add("featureFlags", (object) this._featureFlags);
        dictionary.Add("registryItems", (object) new Dictionary<string, string>()
        {
          ["DefaultTab"] = this.GetDefaultTab()
        });
      }
      if (isVsipPartner)
        dictionary.Add("isVSIPPartner", (object) isVsipPartner);
      if (!this.CheckIfHostedDeployment())
        this.ViewData["headerIconUrl"] = (object) (this.PageContextProvider.GetResourcesPath(this.TfsRequestContext) + "Header/vsonline.png");
      this.ViewData["generalInfo"] = (object) dictionary;
    }

    public bool GetCookieOrFeatureFlagStatus(
      string cookieName,
      string featureFlagName = "",
      string queryParamName = null)
    {
      bool result = false;
      if (!string.IsNullOrEmpty(queryParamName) && !string.IsNullOrEmpty(this.Request?.Params?[queryParamName]))
      {
        string str = this.Request?.Params?[queryParamName];
        this.Response.Cookies.Set(new HttpCookie(queryParamName, str));
        bool.TryParse(str, out result);
      }
      else if (!string.IsNullOrEmpty(cookieName))
      {
        string str = this.Request?.Cookies?[cookieName]?.Value;
        if (!string.IsNullOrEmpty(str))
          bool.TryParse(str, out result);
      }
      if (result)
        return true;
      return !string.IsNullOrEmpty(featureFlagName) && this.TfsRequestContext.IsFeatureEnabled(featureFlagName);
    }

    public void PopulateABTestingInfo(string experimentName)
    {
      string abTestAccountId = this.GetABTestAccountID();
      string testExperimentId = this.GetABTestExperimentID(experimentName);
      if (!string.IsNullOrEmpty(testExperimentId) && !string.IsNullOrEmpty(abTestAccountId))
        this.ViewData["ABTesting"] = (object) new Dictionary<string, string>()
        {
          {
            "EnableAB",
            "Enabled"
          },
          {
            "ABTestExperimentID",
            testExperimentId
          },
          {
            "ABTestAccountID",
            abTestAccountId
          }
        };
      this.ViewData["ABPageViewEvent"] = (object) new Dictionary<string, string>()
      {
        {
          "ABPageViewEvent",
          "Enabled"
        }
      };
    }

    private string GetABTestExperimentID(string experimentName) => this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/ABTestExperimentID/" + experimentName, "");

    private string GetABTestAccountID() => this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/ABTestAccountID", "");

    private void GetSharedAccountDetails(
      IPublishedExtensionService extensionService,
      PublishedExtension extension,
      Guid userId,
      bool ensureSharedAccounts,
      PublishedExtensionResult result)
    {
      if (extension == null)
        return;
      extension.SharedWith = extensionService.GetAccountsSharedWithUser(this.TfsRequestContext, extension, userId);
      if (extension.SharedWith == null || extension.SharedWith.Count <= 0)
        return;
      if (!ensureSharedAccounts)
        extension.SharedWith = (List<ExtensionShare>) null;
      result.Extension = extension;
      result.ExtensionAssetsToken = this.GetJwtToken(result.Extension);
    }

    private string GetJwtToken(PublishedExtension extension)
    {
      JwtClaims jwtClaims = new JwtClaims()
      {
        Expiration = new DateTime?(DateTime.UtcNow.AddDays(1.0)),
        ExtraClaims = new Dictionary<string, string>()
        {
          {
            "pn",
            extension.Publisher.PublisherName
          },
          {
            "en",
            extension.ExtensionName
          }
        }
      };
      return this.TfsRequestContext.GetService<IGalleryJwtTokenService>().GenerateJwtToken(this.TfsRequestContext, "AssetSigningKey", jwtClaims);
    }

    private void setFeatureFlag(string key, bool value) => this._featureFlags.Add(key, value);

    public string GetProductFromCookieOrDefaultTab()
    {
      string empty = string.Empty;
      HttpCookie cookie = this.HttpContext.Request.Cookies["Market_SelectedTab"];
      string cookieOrDefaultTab;
      if (cookie != null)
      {
        cookieOrDefaultTab = cookie.Value;
      }
      else
      {
        switch (this.GetDefaultTab().ToLower())
        {
          case "vscode":
            cookieOrDefaultTab = "vscode";
            break;
          case "vsts":
            cookieOrDefaultTab = "vsts";
            break;
          case "subscriptons":
            cookieOrDefaultTab = "subscriptions";
            break;
          case "vsformac":
            cookieOrDefaultTab = "vsformac";
            break;
          default:
            cookieOrDefaultTab = "vs";
            break;
        }
      }
      return cookieOrDefaultTab;
    }

    public virtual string GetMarketplaceProductionUrl()
    {
      string marketplaceProductionUrl = this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/MarketplaceRootURL", "https://marketplace.visualstudio.com/");
      if (!marketplaceProductionUrl.EndsWith("/"))
        marketplaceProductionUrl += "/";
      return marketplaceProductionUrl;
    }

    public bool OnPremServerHasInternetAccess(Dictionary<string, string> properties)
    {
      bool flag = false;
      if (properties != null && properties.ContainsKey(CloudConnectedServerConstants.HasInternetAccess) && properties[CloudConnectedServerConstants.HasInternetAccess].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
        flag = true;
      return flag;
    }

    public virtual string GetSignInRedirectUrl() => this.TfsRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(this.TfsRequestContext);

    public virtual string GetMarketBrowseURL() => this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/MarketplaceURL", "https://go.microsoft.com/fwlink/?linkid=821987");

    public virtual string GetGalleryHostName() => this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/VSGallery/HostedUrl");

    public void CheckAndSetConnectedContext()
    {
      if (!this.CheckIfHostedDeployment())
        return;
      this.m_isServerKeyValid = true;
      HttpCookie cookie;
      GalleryControllerHelper.ClearContextDelegate clearContextDelegate = (GalleryControllerHelper.ClearContextDelegate) (() =>
      {
        this.Request.Cookies.Remove("serverKey");
        if (this.Response.Cookies["serverKey"] != null)
        {
          cookie = this.Response.Cookies["serverKey"];
        }
        else
        {
          cookie = new HttpCookie("serverKey");
          this.Response.Cookies.Set(cookie);
        }
        cookie.Value = "";
        cookie.Expires = DateTime.UtcNow.AddDays(-10.0);
        this.m_isServerKeyValid = false;
      });
      try
      {
        string urlParameterValue = this.GetServerKeyUrlParameterValue();
        if (!string.IsNullOrEmpty(urlParameterValue) && !urlParameterValue.Equals("null", StringComparison.InvariantCultureIgnoreCase))
        {
          Dictionary<string, string> properties = CloudConnectedUtilities.DecodeToken(urlParameterValue);
          int num1 = this.IsValidServerKey(properties) ? 1 : 0;
          bool tfsServerInternetConnectivity = this.OnPremServerHasInternetAccess(properties);
          this.PublishTelemetryEventForOnpremToHostedNavigation(tfsServerInternetConnectivity);
          int num2 = tfsServerInternetConnectivity ? 1 : 0;
          if ((num1 & num2) != 0)
          {
            cookie = new HttpCookie("serverKey", urlParameterValue);
            cookie.HttpOnly = true;
            if (this.Request.IsSecureConnection)
              cookie.Secure = true;
            this.Response.Cookies.Set(cookie);
          }
          else
            clearContextDelegate();
        }
        if (urlParameterValue != null && urlParameterValue.Equals("null", StringComparison.InvariantCultureIgnoreCase))
          clearContextDelegate();
        this.PopulateServerContextProperties();
      }
      catch (FormatException ex)
      {
        clearContextDelegate();
      }
    }

    public virtual string GetAADSignInRedirectUrl(string targetTenant, Uri requestedReplyTo)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      string property = userIdentity == null ? (string) null : userIdentity.GetProperty<string>("Account", string.Empty);
      this.TfsRequestContext.Trace(12060001, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Begining to construct redirection url. AccountName: {0}", (object) property));
      SignInContext signInContext = this.ConstructSignInContext();
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      string accessMappingMoniker1 = AccessMappingConstants.ClientAccessMappingMoniker;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker"))
        accessMappingMoniker1 = AccessMappingConstants.PublicAccessMappingMoniker;
      IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      Guid sps = ServiceInstanceTypes.SPS;
      string accessMappingMoniker2 = accessMappingMoniker1;
      Uri uri = new Uri(service.GetLocationServiceUrl(requestContext, sps, accessMappingMoniker2));
      signInContext.QueryString["realm"] = uri.Host;
      signInContext.QueryString["allow_passthrough"] = bool.TrueString;
      signInContext.QueryString["ctpm"] = "MarketPlace";
      requestedReplyTo = requestedReplyTo != (Uri) null ? requestedReplyTo : this.TfsRequestContext.RequestUri();
      NameValueCollection queryString = HttpUtility.ParseQueryString(requestedReplyTo.Query);
      queryString["auth_redirect"] = bool.TrueString;
      if (!string.IsNullOrEmpty(queryString["serverKey"]))
        queryString.Remove("serverKey");
      UriBuilder uriBuilder = new UriBuilder(requestedReplyTo);
      uriBuilder.Query = queryString.ToString();
      signInContext.QueryString["request_silent_aad_profile"] = bool.TrueString;
      if (!string.IsNullOrWhiteSpace(targetTenant))
        signInContext.QueryString["tenant"] = targetTenant;
      signInContext.QueryString["reply_to"] = uriBuilder.Uri.AbsoluteUri;
      string str = uri.AbsoluteUri + "_signedin";
      bool flag = this.CheckIfForceLoginRequired(targetTenant);
      string empty = string.Empty;
      string message;
      if (string.Equals(targetTenant, "live.com", StringComparison.OrdinalIgnoreCase))
        message = new AadAuthUrlUtility.AuthUrlBuilder()
        {
          DomainHint = "live.com",
          Tenant = targetTenant,
          RedirectLocation = str,
          QueryString = ((IDictionary<string, string>) signInContext.QueryString),
          PromptOption = (flag ? AadAuthUrlUtility.PromptOption.Login : AadAuthUrlUtility.PromptOption.NoOption)
        }.Build(this.TfsRequestContext);
      else
        message = new AadAuthUrlUtility.AuthUrlBuilder()
        {
          UserHint = property,
          Tenant = targetTenant,
          RedirectLocation = str,
          QueryString = ((IDictionary<string, string>) signInContext.QueryString),
          PromptOption = (flag ? AadAuthUrlUtility.PromptOption.Login : AadAuthUrlUtility.PromptOption.NoOption)
        }.Build(this.TfsRequestContext);
      this.TfsRequestContext.Trace(12060003, TraceLevel.Info, this.AreaName, this.LayerName, message);
      return message;
    }

    public virtual SignInContext ConstructSignInContext() => SignInContextFactory.Construct(this.TfsRequestContext);

    public virtual Guid GetAuthenticatedTenantId()
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      return userIdentity != null ? userIdentity.GetProperty<Guid>("Domain", Guid.Empty) : Guid.Empty;
    }

    public void InitializeTargetId() => this.SetTargetIdCookie(this.Request.QueryString["targetId"]);

    public void LoadPageMetadata(
      GalleryPages galleryPage,
      PublishedExtension extension,
      string product,
      string category,
      string searchTerm,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = null)
    {
      string str = string.Empty;
      switch (galleryPage)
      {
        case GalleryPages.HomePage:
          str = product.IsNullOrEmpty<char>() ? this.GetProductFromCookieOrDefaultTab() : product;
          break;
        case GalleryPages.SearchPage:
          str = product;
          break;
        case GalleryPages.CategoryPage:
          str = product;
          break;
      }
      this.ViewData["page-metadata"] = (object) new PageMetadataInputs()
      {
        GalleryPage = galleryPage,
        Extension = extension,
        Category = category,
        Product = str,
        SearchTerm = searchTerm,
        Url = this.Request.Url.ToString(),
        Publisher = publisher,
        ResourcesPath = this.PageContextProvider.GetResourcesPath(this.TfsRequestContext)
      };
    }

    public string GetAssetContent(
      string publisherName,
      string extensionName,
      string version,
      string assetName,
      string assetToken)
    {
      string assetContent = "";
      IPublisherAssetService service1 = this.TfsRequestContext.GetService<IPublisherAssetService>();
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo(assetName)
      };
      try
      {
        ExtensionFile assetFile = service1.QueryAsset(this.TfsRequestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, assetToken, true).AssetFile;
        ITeamFoundationFileService service2 = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
        if (assetFile != null)
        {
          using (StreamReader streamReader = new StreamReader(service2.RetrieveFile(this.TfsRequestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _), true))
            assetContent = streamReader.ReadToEnd();
        }
      }
      catch (ExtensionAssetNotFoundException ex)
      {
        assetContent = "";
      }
      return assetContent;
    }

    public virtual bool ContainsAsset(
      string publisherName,
      string extensionName,
      string version,
      string assetName,
      string assetToken)
    {
      bool flag = false;
      IPublisherAssetService service = this.TfsRequestContext.GetService<IPublisherAssetService>();
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo(assetName)
      };
      try
      {
        ExtensionFile assetFile = service.QueryAsset(this.TfsRequestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, assetToken, true).AssetFile;
        this.TfsRequestContext.GetService<ITeamFoundationFileService>();
        if (assetFile != null)
          flag = true;
      }
      catch (ExtensionAssetNotFoundException ex)
      {
        flag = false;
      }
      return flag;
    }

    public virtual string GetManageCookieDropScriptPath()
    {
      string path = this.WebContext.Url.TfsScriptContent("Gallery/Client/Common/ManageCookieDrop.js", this.WebContext.Diagnostics.DebugMode, true);
      string filePath = this.HttpContext.Server.MapPath(path);
      if (this.CheckIfFileExists(filePath))
        return path;
      this.TfsRequestContext.Trace(12062031, TraceLevel.Error, this.AreaName, "CookieCompliance", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to locate the cookie drop manipulation script at fqpath: {0}, relative path: {1}", (object) filePath, (object) path));
      return (string) null;
    }

    public virtual string GetAbsolutePath(string path) => this.ReadRegistryKey(this.TfsRequestContext, "/Configuration/Service/Gallery/UseVirtualPath", false) ? this.TfsRequestContext.VirtualPath() : VirtualPathUtility.ToAbsolute(path);

    public virtual ExtensionFilterResult GetExtensionsResultByPublisherForProduct(
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      string productType,
      bool usePublisherDisplayName = false,
      int pageSize = 18,
      int pageNumber = 1,
      SortByType sortByType = SortByType.InstallCount)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      List<FilterCriteria> filterCriteriaList;
      if (usePublisherDisplayName)
        filterCriteriaList = new List<FilterCriteria>()
        {
          new FilterCriteria()
          {
            FilterType = 19,
            Value = publisher.DisplayName
          }
        };
      else
        filterCriteriaList = new List<FilterCriteria>()
        {
          new FilterCriteria()
          {
            FilterType = 18,
            Value = publisher.PublisherName
          }
        };
      filterCriteriaList.AddRange((IEnumerable<FilterCriteria>) this.GetInstallationTargetFilter(productType));
      PublishedExtensionFlags publishedExtensionFlags = PublishedExtensionFlags.Disabled | PublishedExtensionFlags.BuiltIn | PublishedExtensionFlags.System | PublishedExtensionFlags.Unpublished;
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueriesBasedOnHiddenFlags"))
        publishedExtensionFlags = PublishedExtensionFlags.Disabled | PublishedExtensionFlags.System | PublishedExtensionFlags.Unpublished | PublishedExtensionFlags.Hidden;
      filterCriteriaList.Add(new FilterCriteria()
      {
        FilterType = 12,
        Value = ((int) publishedExtensionFlags).ToString()
      });
      ExtensionQuery extensionQuery1 = new ExtensionQuery()
      {
        Flags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeLatestVersionOnly,
        Filters = new List<QueryFilter>(1)
        {
          new QueryFilter()
          {
            Criteria = filterCriteriaList,
            SortBy = (int) sortByType,
            PageSize = pageSize,
            PageNumber = pageNumber
          }
        },
        AssetTypes = new List<string>()
        {
          "Microsoft.VisualStudio.Services.Icons.Default"
        }
      };
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ExtensionQuery extensionQuery2 = extensionQuery1;
      ExtensionQueryResult extensionQueryResult = service.QueryExtensions(tfsRequestContext, extensionQuery2, (string) null, true);
      return extensionQueryResult.Results != null && extensionQueryResult.Results.Count > 0 ? extensionQueryResult.Results[0] : (ExtensionFilterResult) null;
    }

    public List<FilterCriteria> GetInstallationTargetFilter(string productType)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(productType, nameof (productType));
      List<FilterCriteria> installationTargetFilter = new List<FilterCriteria>();
      switch (productType)
      {
        case "vs":
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Ide"
          });
          break;
        case "vsts":
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Services"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Services.Resource.Cloud"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Services.Cloud"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.TeamFoundation.Server"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Services.Integration"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Services.Cloud.Integration"
          });
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.TeamFoundation.Server.Integration"
          });
          break;
        case "vscode":
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Code"
          });
          break;
        case "vsformac":
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Mac"
          });
          break;
        case "subscriptions":
          installationTargetFilter.Add(new FilterCriteria()
          {
            FilterType = 8,
            Value = "Microsoft.VisualStudio.Offer"
          });
          break;
        default:
          throw new Exception(string.Format("{0} is invalid", (object) productType));
      }
      return installationTargetFilter;
    }

    public bool ExtensionSupportsConnectedTfsVersion(PublishedExtension extension)
    {
      bool flag = false;
      Version result;
      if (this.ViewData.ContainsKey("ConnectedServerVersion") && Version.TryParse((string) this.ViewData["ConnectedServerVersion"], out result) && extension != null && extension.InstallationTargets != null)
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if (installationTarget.Target.Equals("Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) && result.CompareTo(installationTarget.MinVersion) >= (!installationTarget.MinInclusive ? 1 : 0) & result.CompareTo(installationTarget.MaxVersion) <= (installationTarget.MaxInclusive ? 0 : -1))
            flag = true;
        }
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("OnPremConnectedTfsServerVersion", this.ViewData.ContainsKey("ConnectedServerVersion") ? (string) this.ViewData["ConnectedServerVersion"] : "null");
      properties.Add("OnPremConnectedExtensionName", extension != null ? extension.GetFullyQualifiedName() : "null");
      properties.Add("OnPremConnectedTfsInstallSupported", flag);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "OnPremConnected", "OnPremConnectedServerVersionCheck", properties);
      return flag;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetPrimaryMsaIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseNormalGetPrimaryMsaIdentity"))
      {
        this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentity: from identityService.GetPrimaryMsaIdentity");
        return requestContext.GetService<IdentityService>().GetPrimaryMsaIdentity(requestContext, (IReadOnlyVssIdentity) identity);
      }
      this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentity: from AccessToken");
      return this.GetPrimaryMsaIdentityFromAccessToken(requestContext, identity);
    }

    public string GetOnPremRedirectURL(
      string itemName,
      PublishedExtension extension,
      bool freeInstall = false)
    {
      if (extension == null)
        return (string) null;
      string serverContextKey = this.GetServerContextKey();
      if (string.IsNullOrEmpty(serverContextKey))
        return (string) null;
      Dictionary<string, string> dictionary = CloudConnectedUtilities.DecodeToken(serverContextKey);
      Dictionary<string, string> properties = new Dictionary<string, string>();
      properties[GalleryWebConstants.ConnectedInstallContext.ItemName] = itemName;
      properties[GalleryWebConstants.ConnectedInstallContext.ItemDetailsLink] = GalleryHtmlExtensions.GetGalleryItemDetailsUrl(this.TfsRequestContext, itemName);
      if (dictionary != null && dictionary.ContainsKey(CloudConnectedServerConstants.CollectionId))
        properties[CloudConnectedServerConstants.CollectionId] = dictionary[CloudConnectedServerConstants.CollectionId];
      string str = CloudConnectedUtilities.EncodeToken(properties);
      string empty = string.Empty;
      if (dictionary.ContainsKey(CloudConnectedServerConstants.GalleryUrl))
        empty = dictionary[CloudConnectedServerConstants.GalleryUrl];
      if (!empty.EndsWith("/"))
        empty += "/";
      string onPremRedirectUrl = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}items?itemName={1}&install=true&installContext={2}", (object) empty, (object) UriUtility.UrlEncode(itemName), (object) UriUtility.UrlEncode(str));
      if (freeInstall)
        onPremRedirectUrl += "&freeInstall=true";
      return onPremRedirectUrl;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetPrimaryMsaIdentityFromAccessToken(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identity == null)
      {
        this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentityFromAccessToken: identity is null");
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      if (!identity.IsExternalUser || identity.IsContainer)
      {
        this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentityFromAccessToken: identity is not external users or container.");
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string str = identity.GetProperty<string>("PUID", (string) null);
      if (string.IsNullOrWhiteSpace(str))
      {
        string resource = "https://" + vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/GraphApiDomainName", "graph.windows.net");
        Guid property = identity.GetProperty<Guid>("Domain", Guid.Empty);
        str = this.GetAltSecId(vssRequestContext.GetService<IAadTokenService>().AcquireToken(vssRequestContext, resource, property.ToString()));
      }
      if (string.IsNullOrWhiteSpace(str))
      {
        this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentityFromAccessToken: puid is empty.");
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      if (str.StartsWith("aad:"))
      {
        this.TfsRequestContext.Trace(12062064, TraceLevel.Info, this.AreaName, this.LayerName, "GetPrimaryMsaIdentityFromAccessToken: puid is not starting with aad:.");
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      return vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", str + "@Live.com")
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private string GetAltSecId(JwtSecurityToken userAccessToken)
    {
      Claim claim = userAccessToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "altsecid"));
      if (claim != null)
      {
        string str = claim.Value;
        if (!string.IsNullOrEmpty(str) && str.LastIndexOf(':') > str.IndexOf(':'))
          return str.Substring(str.LastIndexOf(':') + 1);
      }
      return string.Empty;
    }

    protected void SetTargetIdCookie(string targetId)
    {
      if (string.IsNullOrEmpty(targetId))
        return;
      Guid result;
      if (!Guid.TryParse(targetId.Split(',')[0], out result))
        return;
      HttpCookie cookie = new HttpCookie(nameof (targetId), result.ToString());
      if (this.Request.IsSecureConnection)
        cookie.Secure = true;
      this.Response.Cookies.Set(cookie);
    }

    protected bool CheckIfHostedDeployment() => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment;

    protected virtual string GetServerContextKey()
    {
      if (this.m_isServerKeyValid)
      {
        string urlParameterValue = this.GetServerKeyUrlParameterValue();
        if (!string.IsNullOrEmpty(urlParameterValue) && !urlParameterValue.Equals("null", StringComparison.OrdinalIgnoreCase))
          return this.AddPaddingToBase64IfNeeded(urlParameterValue);
        HttpCookie cookie = this.Request.Cookies["serverKey"];
        if (cookie != null)
          return this.AddPaddingToBase64IfNeeded(cookie.Value);
      }
      return (string) null;
    }

    protected string GetServerKeyUrlParameterValue()
    {
      string urlParameterValue = this.Request.QueryString["serverKey"];
      if (!string.IsNullOrEmpty(urlParameterValue))
        urlParameterValue = urlParameterValue.Replace(" ", "+");
      return urlParameterValue;
    }

    public string AddPaddingToBase64IfNeeded(string baseValue)
    {
      if (!string.IsNullOrEmpty(baseValue))
      {
        for (int index = 0; baseValue.Length % 4 != 0 && index < 2; ++index)
          baseValue += "=";
      }
      return baseValue;
    }

    protected void PopulateServerContextProperties()
    {
      string serverContextKey = this.GetServerContextKey();
      if (string.IsNullOrEmpty(serverContextKey))
        return;
      Dictionary<string, string> properties = CloudConnectedUtilities.DecodeToken(serverContextKey);
      if (this.OnPremServerHasInternetAccess(properties))
        this.ViewData["server-context"] = (object) properties.Serialize<Dictionary<string, string>>();
      if (!properties.ContainsKey(CloudConnectedServerConstants.ServerVersion))
        return;
      this.ViewData["ConnectedServerVersion"] = (object) properties[CloudConnectedServerConstants.ServerVersion];
    }

    protected virtual string ReadRegistryKey(IVssRequestContext requestContext, string registryPath) => this.ReadRegistryKey(requestContext, registryPath, "");

    protected virtual string ReadRegistryKey(
      IVssRequestContext requestContext,
      string registryPath,
      string defaultValue)
    {
      return this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) registryPath, defaultValue);
    }

    protected virtual bool ReadRegistryKey(
      IVssRequestContext requestContext,
      string registryPath,
      bool defaultValue)
    {
      return this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<bool>(this.TfsRequestContext, (RegistryQuery) registryPath, defaultValue);
    }

    protected virtual bool CheckIfFileExists(string filePath) => File.Exists(filePath);

    private bool IsValidServerKey(Dictionary<string, string> properties)
    {
      if (properties != null && properties.ContainsKey(CloudConnectedServerConstants.ServerName))
      {
        if (properties.ContainsKey(CloudConnectedServerConstants.GalleryUrl))
        {
          try
          {
            string property = properties[CloudConnectedServerConstants.ServerName];
            Uri uri = new Uri(properties[CloudConnectedServerConstants.GalleryUrl]);
            if (string.IsNullOrEmpty(property))
            {
              this.TfsRequestContext.Trace(12061067, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("Bad server name : serverName = {0} ", (object) property));
              return false;
            }
            string[] strArray = new string[4]
            {
              CloudConnectedServerConstants.GalleryUrl,
              CloudConnectedServerConstants.CollectionUrl,
              CloudConnectedServerConstants.ConnectUrl,
              CloudConnectedServerConstants.UserHubUrl
            };
            foreach (string key in strArray)
            {
              string urlString;
              if (properties.TryGetValue(key, out urlString) && !this.IsValidOnPremUrlScheme(urlString))
              {
                this.TfsRequestContext.Trace(12061067, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("Bad gallery url : {0} = {1} ", (object) key, (object) properties[key]));
                return false;
              }
            }
          }
          catch (UriFormatException ex)
          {
            this.TfsRequestContext.Trace(12061067, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("Bad gallery URL:  galleryURL = {0}", (object) properties[CloudConnectedServerConstants.GalleryUrl]));
            return false;
          }
          return true;
        }
      }
      this.TfsRequestContext.Trace(12061067, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("Invalid ServerKey received : {0}", (object) new JavaScriptSerializer().Serialize((object) properties)));
      return false;
    }

    private bool IsValidOnPremUrlScheme(string urlString)
    {
      if (string.IsNullOrEmpty(urlString))
        return true;
      Uri uri = new Uri(urlString);
      return uri.Scheme.Equals(Uri.UriSchemeHttp) || uri.Scheme.Equals(Uri.UriSchemeHttps);
    }

    private string GetDefaultTab() => this.ReadRegistryKey(this.TfsRequestContext, "/Gallery/Web/Settings/DefaultTab", "VS");

    private void PublishTelemetryEventForOnpremToHostedNavigation(bool tfsServerInternetConnectivity)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("InternetAccess", tfsServerInternetConnectivity);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "OnPremConnected", "OnPremConnectedNavigationFeature", properties);
    }

    private bool CheckIfForceLoginRequired(string currentlyRequestedTenant)
    {
      bool flag = false;
      HttpCookie cookie = this.Request.Cookies["previouslyRequestedTenant"];
      if (cookie == null && !string.IsNullOrWhiteSpace(currentlyRequestedTenant))
      {
        this.Response.Cookies.Set(new HttpCookie("previouslyRequestedTenant", currentlyRequestedTenant)
        {
          HttpOnly = true
        });
      }
      else
      {
        Guid authenticatedTenantId = this.GetAuthenticatedTenantId();
        Guid result1;
        Guid result2;
        if (cookie != null && Guid.TryParse(cookie.Value, out result1) && Guid.TryParse(currentlyRequestedTenant, out result2))
        {
          if (result1 != authenticatedTenantId && result1 == result2)
          {
            flag = true;
          }
          else
          {
            cookie.Value = currentlyRequestedTenant;
            this.Response.Cookies.Set(cookie);
          }
        }
      }
      return flag;
    }

    private delegate void ClearContextDelegate();
  }
}
