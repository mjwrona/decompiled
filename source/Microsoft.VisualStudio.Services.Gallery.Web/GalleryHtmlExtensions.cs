// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.GalleryHtmlExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public static class GalleryHtmlExtensions
  {
    private const string c_showFooterViewDataKey = "__gallery.showFooter";
    private const string c_showDetailedFooterViewDataKey = "__gallery.showDetailedFooter";
    private static string WorkFlowID = "workflowId";
    private static string WorkFlowIDMarketplace = "marketplace";
    private static string WorkFlowMCID = "wt.mc_id";
    private static string WorkFlowMCIDSignIn = "o~msft~marketplace~signIn";
    private static string CampaignWorkFlow = GalleryHtmlExtensions.WorkFlowID + "=" + GalleryHtmlExtensions.WorkFlowIDMarketplace;
    private static string DisplayNameCookie = "SpsAuthenticatedUser";
    private static string DisplayNameCookieKey = "DisplayName";
    private static string TestCommerce = "testCommerce";
    private static int DisplayNameMaxLength = 40;
    private const string s_area = "gallery";
    private const string s_layer = "GalleryHtmlExtension";
    private const string RegistryPathForCaptchaPublicKey = "/Configuration/Service/Gallery/CaptchaPublicKey";
    private const string c_area = "Gallery";
    private const string c_layer = "HtmlExtension";
    private const string c_displayNameFormatForCspPartnerUser = "{0} ({1})";

    public static MvcHtmlString RenderHeader(this HtmlHelper htmlHelper)
    {
      UxServicesModel model = new UxServicesModelBuilder(htmlHelper.ViewContext).GetModel();
      string input = "";
      if (model.UseUxServices)
        input = model.UxServicesHeader.ToString();
      return new MvcHtmlString(Regex.Replace(input, "<script [^>]*https://i1.services.social.microsoft.com/search/Widgets/SearchBox.jss?[^>]*></script>", ""));
    }

    public static MvcHtmlString LoadCoreScriptsAsync(this HtmlHelper htmlHelper)
    {
      WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      MvcHtmlString mvcHtmlString = htmlHelper.CoreScripts();
      StringBuilder stringBuilder = new StringBuilder();
      MatchCollection matchCollection = Regex.Matches(mvcHtmlString.ToString(), "script(.*)src(.*)type(.*)</script>");
      if (matchCollection != null)
      {
        foreach (Match match in matchCollection)
          stringBuilder.Append("<script defer=\"true\"" + match.Groups[1].Value + " src" + match.Groups[2].Value + " type" + match.Groups[3].Value + "</script>");
      }
      string input = htmlHelper.PageContext().ToString();
      if (htmlHelper.ViewData["RemoveUserContext"] != null)
        input = new Regex("\"user\":{[^}]+},").Replace(input, "");
      return new MvcHtmlString(input + " " + stringBuilder.ToString());
    }

    public static MvcHtmlString LoadViewBundleAsync(
      this HtmlHelper htmlHelper,
      params string[] defaultModules)
    {
      WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      string input = htmlHelper.ScriptModules(defaultModules).ToString();
      StringBuilder stringBuilder = new StringBuilder();
      MatchCollection matchCollection = Regex.Matches(input, "script(.*)src(.*)type(.*)</script>");
      if (matchCollection != null)
      {
        foreach (Match match in matchCollection)
          stringBuilder.Append("<script defer=\"true\"" + match.Groups[1].Value + " src" + match.Groups[2].Value + " type" + match.Groups[3].Value + "</script>");
      }
      return new MvcHtmlString(stringBuilder.ToString());
    }

    public static string GetPageTitle(
      this HtmlHelper htmlHelper,
      GalleryPages galleryPage,
      PageMetadataInputs metadataInputs)
    {
      IGalleryPageMetadataProvider service = htmlHelper.ViewContext.TfsRequestContext().GetService<IGalleryPageMetadataProvider>();
      string pageTitle = string.Empty;
      switch (galleryPage)
      {
        case GalleryPages.HomePage:
          pageTitle = service.GetHomePageTitle();
          break;
        case GalleryPages.ItemDetailsPage:
          pageTitle = service.GetItemDetailsPageTitle(metadataInputs);
          break;
        case GalleryPages.SearchPage:
          pageTitle = service.GetSearchPageTitle(metadataInputs);
          break;
        case GalleryPages.CategoryPage:
          pageTitle = service.GetCategoryPageTitle(metadataInputs);
          break;
        case GalleryPages.PublisherProfile:
          pageTitle = service.GetPublisherProfilePageTitle(metadataInputs);
          break;
      }
      return pageTitle;
    }

    public static MvcHtmlString RenderPageMetadata(
      this HtmlHelper htmlHelper,
      GalleryPages galleryPage,
      PageMetadataInputs metadataInputs)
    {
      IGalleryPageMetadataProvider service = htmlHelper.ViewContext.TfsRequestContext().GetService<IGalleryPageMetadataProvider>();
      IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsRequestContext();
      string str = string.Empty;
      switch (galleryPage)
      {
        case GalleryPages.HomePage:
          str = service.GetHomePageMetadata(metadataInputs);
          break;
        case GalleryPages.ItemDetailsPage:
          str = service.GetItemDetailsPageMetadata(metadataInputs, tfsRequestContext);
          break;
        case GalleryPages.SearchPage:
          str = service.GetSearchPageMetadata(metadataInputs);
          break;
        case GalleryPages.CategoryPage:
          str = service.GetCategoryPageMetadata(metadataInputs);
          break;
        case GalleryPages.PublisherProfile:
          str = service.GetPublisherProfilePageMetadata(tfsRequestContext, metadataInputs);
          break;
      }
      return new MvcHtmlString(str);
    }

    public static string SignoutUrl(this HtmlHelper htmlHelper, string redirectUrl = null) => GalleryHtmlExtensions.SignoutUrl(htmlHelper.ViewContext.TfsRequestContext(false), redirectUrl);

    public static string SignoutUrl(IVssRequestContext tfsRequestContext, string redirectUrl = null)
    {
      string galleryAbsoluteUrl = GalleryHtmlExtensions.GetGalleryAbsoluteUrl(tfsRequestContext);
      return redirectUrl == null ? GalleryHtmlExtensions.GetSpsAbsoluteUrl(tfsRequestContext, "_signout?redirectUrl=" + Uri.EscapeDataString(galleryAbsoluteUrl)) : GalleryHtmlExtensions.GetSpsAbsoluteUrl(tfsRequestContext, "_signout?redirectUrl=" + redirectUrl);
    }

    public static string SigninUrl(this HtmlHelper htmlHelper)
    {
      IVssRequestContext context = htmlHelper.ViewContext.TfsRequestContext(false);
      if (context.ExecutionEnvironment.IsOnPremisesDeployment)
        return string.Empty;
      ITeamFoundationAuthenticationService service = context.GetService<ITeamFoundationAuthenticationService>();
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary.Add(GalleryHtmlExtensions.WorkFlowID, GalleryHtmlExtensions.WorkFlowIDMarketplace);
      dictionary.Add(GalleryHtmlExtensions.WorkFlowMCID, GalleryHtmlExtensions.WorkFlowMCIDSignIn);
      IVssRequestContext requestContext = context;
      IDictionary<string, string> parameters = dictionary;
      string input = service.GetSignInRedirectLocation(requestContext, parameters: parameters);
      if (input.IndexOf("serverKey", StringComparison.InvariantCultureIgnoreCase) >= 0)
      {
        string pattern = string.Format("{0}{1}[^&]+", (object) Uri.EscapeDataString("&"), (object) "serverKey");
        input = Regex.Replace(input, pattern, "", RegexOptions.IgnoreCase);
      }
      return input;
    }

    public static string ProfileUrl(this HtmlHelper htmlHelper) => GalleryHtmlExtensions.GetSpsAbsoluteUrl(htmlHelper.ViewContext.TfsRequestContext(false), "profile/view?" + GalleryHtmlExtensions.CampaignWorkFlow);

    public static bool isPaidExtension(
      this HtmlHelper htmlHelper,
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails)
    {
      string str = HttpContext.Current.Request.QueryString[GalleryHtmlExtensions.TestCommerce];
      bool result = false;
      if (str != null)
        bool.TryParse(str, out result);
      if (extension.IsFirstPartyAndPaid() && !extension.IsPreview())
        return true;
      if (!extension.IsPaid() || extension.IsPreview())
        return false;
      if (offerDetails.HasPublicPlans)
        return true;
      return result && offerDetails.HasPlans;
    }

    public static bool isByolEnforced(this HtmlHelper htmlHelper, PublishedExtension extension) => extension.IsByolEnforcedExtension();

    public static bool isPaidResource(this HtmlHelper htmlHelper, PublishedExtension extension) => GalleryUtil.InstallationTargetsHasVSTSResource((IEnumerable<InstallationTarget>) extension.InstallationTargets);

    public static bool isPaidOffers(this HtmlHelper htmlHelper, PublishedExtension extension) => GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets);

    public static string UserDisplayNameFromIdentityService(this HtmlHelper htmlHelper)
    {
      IVssRequestContext vssRequestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vssRequestContext.GetUserIdentity();
      if (userIdentity == null)
        return string.Empty;
      if (userIdentity.IsCspPartnerUser)
        return GalleryHtmlExtensions.BuildDisplayNameForCspPartnerUser(vssRequestContext, userIdentity.DisplayName);
      string preferredEmailAddress = IdentityHelper.GetPreferredEmailAddress(vssRequestContext, userIdentity.Id);
      return !string.IsNullOrEmpty(preferredEmailAddress) ? string.Format("{0} ({1})", (object) userIdentity.DisplayName, (object) preferredEmailAddress) : userIdentity.DisplayName;
    }

    public static string UserVsidFromIdentityService(this HtmlHelper htmlHelper)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = htmlHelper.ViewContext.TfsRequestContext(false).GetUserIdentity();
      return userIdentity == null ? string.Empty : userIdentity.Id.ToString();
    }

    public static string GetBannerText(this HtmlHelper htmlHelper)
    {
      IVssRequestContext vssRequestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/BannerText", string.Empty);
    }

    public static string GalleryUrl(this HtmlHelper htmlHelper, string relativeUrl = "") => GalleryHtmlExtensions.GetGalleryAbsoluteUrl(htmlHelper.ViewContext.TfsRequestContext(false), relativeUrl);

    private static string GetSpsAbsoluteUrl(
      IVssRequestContext tfsRequestContext,
      string relativeSpsUrl)
    {
      ILocationService service = tfsRequestContext.GetService<ILocationService>();
      string spsAbsoluteUrl = "";
      if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
        if (tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker"))
          accessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
        string absoluteUri = new Uri(service.GetLocationServiceUrl(tfsRequestContext, ServiceInstanceTypes.SPS, accessMappingMoniker)).AbsoluteUri;
        spsAbsoluteUrl = !absoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? absoluteUri + "/" + relativeSpsUrl : absoluteUri + relativeSpsUrl;
      }
      return spsAbsoluteUrl;
    }

    public static string GetGalleryAbsoluteUrl(
      IVssRequestContext tfsRequestContext,
      string relativeUrl = "")
    {
      string galleryAbsoluteUrl = "";
      Uri galleryUri = GalleryHtmlExtensions.GetGalleryUri(tfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker);
      if (galleryUri != (Uri) null)
      {
        string absoluteUri = galleryUri.AbsoluteUri;
        galleryAbsoluteUrl = !absoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? absoluteUri + "/" + relativeUrl : absoluteUri + relativeUrl;
      }
      return galleryAbsoluteUrl;
    }

    public static Uri GetGalleryUri(
      IVssRequestContext tfsRequestContext,
      string accessMappingMoniker)
    {
      Uri galleryUri = (Uri) null;
      string uriString = (string) null;
      if (tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UsePublicAccessMappingMoniker"))
        accessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
      if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        uriString = tfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(tfsRequestContext, new Guid("00000029-0000-8888-8000-000000000000"), accessMappingMoniker);
        galleryUri = new Uri(uriString);
      }
      tfsRequestContext.Trace(12062025, TraceLevel.Info, "Gallery", "LocationServiceInfo", "Url from location service call: {0}, returnUri:{1}, ServiceOwner:{2}, accessMappingMoniker:{3}", (object) uriString, (object) galleryUri, (object) "00000029-0000-8888-8000-000000000000", (object) accessMappingMoniker);
      return galleryUri;
    }

    public static string GetGalleryItemDetailsUrl(
      IVssRequestContext tfsRequestContext,
      string itemName)
    {
      return GalleryHtmlExtensions.GetGalleryAbsoluteUrl(tfsRequestContext, "items?itemName=" + UriUtility.UrlEncode(itemName));
    }

    public static string GetGalleryItemPurchaseUrl(
      IVssRequestContext tfsRequestContext,
      string itemName)
    {
      return GalleryHtmlExtensions.GetGalleryAbsoluteUrl(tfsRequestContext, "items?itemName=" + UriUtility.UrlEncode(itemName) + "&install=true");
    }

    public static string GetGalleryPublisherProfileUrl(
      IVssRequestContext tfsRequestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      return GalleryHtmlExtensions.GetGalleryAbsoluteUrl(tfsRequestContext, "publishers/" + UriUtility.UrlEncode(publisher.PublisherName));
    }

    public static string UserDisplayName(this HtmlHelper htmlHelper)
    {
      HttpCookie cookie = HttpContext.Current.Request.Cookies[GalleryHtmlExtensions.DisplayNameCookie];
      if (cookie == null)
        return (string) null;
      string stringToUnescape = cookie.Values[GalleryHtmlExtensions.DisplayNameCookieKey];
      if (string.IsNullOrWhiteSpace(stringToUnescape))
        return (string) null;
      string str = Uri.UnescapeDataString(stringToUnescape);
      if (str.Length > GalleryHtmlExtensions.DisplayNameMaxLength)
        str = str.Substring(0, GalleryHtmlExtensions.DisplayNameMaxLength) + "…";
      return str;
    }

    public static string OnPremServerDisplayName(this HtmlHelper htmlHelper)
    {
      string str1 = (string) null;
      if (htmlHelper.ViewData.ContainsKey("server-context"))
      {
        try
        {
          Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>((string) htmlHelper.ViewData["server-context"]);
          string str2 = (string) null;
          string str3 = (string) null;
          if (dictionary.TryGetValue(CloudConnectedServerConstants.ServerName, out str2))
            str1 = !dictionary.TryGetValue(CloudConnectedServerConstants.CollectionName, out str3) ? str2 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, GalleryResources.ServerCollectionHeader, (object) str2, (object) str3);
        }
        catch (JsonException ex)
        {
        }
      }
      return str1;
    }

    public static void ShowFooter(this HtmlHelper htmlHelper, bool value) => htmlHelper.ViewData["__gallery.showFooter"] = (object) value;

    public static bool ShowFooter(this HtmlHelper htmlHelper) => !htmlHelper.ViewData.ContainsKey("__gallery.showFooter") || (bool) htmlHelper.ViewData["__gallery.showFooter"];

    public static void ShowDetailedFooter(this HtmlHelper htmlHelper, bool value) => htmlHelper.ViewData["__gallery.showDetailedFooter"] = (object) value;

    public static bool ShowDetailedFooter(this HtmlHelper htmlHelper) => htmlHelper.ViewData.ContainsKey("__gallery.showDetailedFooter");

    public static bool ShowSupportRequestForm(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext(false).IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSupportRequestFeature");

    public static MvcHtmlString RenderSearchBox(this HtmlHelper htmlHelper)
    {
      MvcHtmlString mvcHtmlString = (MvcHtmlString) null;
      if (htmlHelper.EnableSearchOnPage())
      {
        TagBuilder tagBuilder1 = new TagBuilder("div").AddClass("logo-text search-ext").Attribute("id", "searchDiv");
        TagBuilder tagBuilder2 = new TagBuilder("label").AddClass("search-box-label").Attribute("for", "searchBox");
        TagBuilder tagBuilder3 = new TagBuilder("input").AddClass("search-text").Attribute("id", "searchBox").Attribute("placeholder", GalleryResources.SearchExtensionPlaceHolder);
        TagBuilder tagBuilder4 = new TagBuilder("span").AddClass("bowtie-icon bowtie-search").Attribute("id", "searchButton");
        tagBuilder1.InnerHtml = tagBuilder1.InnerHtml + tagBuilder2.ToString() + tagBuilder3.ToString() + tagBuilder4.ToString();
        mvcHtmlString = tagBuilder1.ToHtmlString();
      }
      return mvcHtmlString;
    }

    private static bool EnableSearchOnPage(this HtmlHelper htmlHelper)
    {
      WebContext webContext = PlatformHelpers.WebContext(htmlHelper.ViewContext);
      return new List<string>()
      {
        "Gallery",
        "Category",
        "Search",
        "Details"
      }.Exists((Predicate<string>) (action => webContext.NavigationContext.CurrentAction.Equals(action, StringComparison.OrdinalIgnoreCase)));
    }

    public static MvcHtmlString PageInitForModuleLoaderConfig(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.PageInitForModuleLoaderConfig"))
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(htmlHelper.GlobalizationConfig().ToString());
        stringBuilder.AppendLine(htmlHelper.ModuleLoaderConfig().ToString());
        return new MvcHtmlString(htmlHelper.PageContext().ToString() + stringBuilder.ToString());
      }
    }

    public static MvcHtmlString EnableABTesting(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.IncludeABTestingScript"))
      {
        if (htmlHelper.ViewData.ContainsKey("ABTesting"))
        {
          Dictionary<string, string> dictionary = (Dictionary<string, string>) htmlHelper.ViewData["ABTesting"];
          string str1 = (string) null;
          ref string local = ref str1;
          if (dictionary.TryGetValue("ABTestExperimentID", out local))
          {
            string str2 = "<script src=\"//www.google-analytics.com/cx/api.js?experiment=" + str1 + "\"></script> ";
            string str3 = "<script> var chosenVariation = cxApi.chooseVariation(); </script>";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(str2);
            stringBuilder.AppendLine(str3);
            return new MvcHtmlString(stringBuilder.ToString());
          }
        }
        return new MvcHtmlString("");
      }
    }

    public static MvcHtmlString setABVariationInCookie(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.IncludeABTestingScript"))
      {
        Dictionary<string, string> dictionary = (Dictionary<string, string>) htmlHelper.ViewData["ABTesting"];
        string str = (string) null;
        ref string local = ref str;
        return dictionary.TryGetValue("ABTestExperimentID", out local) ? new MvcHtmlString(" <script> $(document).ready(function () {document.abTest = chosenVariation;}) </script>") : new MvcHtmlString("");
      }
    }

    private static string BuildDisplayNameForCspPartnerUser(
      IVssRequestContext context,
      string identityDisplayName)
    {
      string defaultDomainName = GalleryHtmlExtensions.GetVerifiedDefaultDomainName(context);
      return string.IsNullOrWhiteSpace(defaultDomainName) ? identityDisplayName : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) identityDisplayName, (object) defaultDomainName);
    }

    public static bool IsReCaptchaEnabledForCreatePublisherProfile(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreatePublisherProfile");

    public static bool IsReCaptchaEnabledForCreateVisualStudioExtension(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreateVisualStudioExtension");

    public static bool IsReCaptchaEnabledForCreateVisualStudioCodeExtension(
      this HtmlHelper htmlHelper)
    {
      return htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreateVisualStudioCodeExtension");
    }

    public static bool IsReCaptchaEnabledForUpdateVisualStudioCodeExtension(
      this HtmlHelper htmlHelper)
    {
      return htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForUpdateVisualStudioCodeExtension");
    }

    public static bool IsReCaptchaEnabledForUpdatePublisherProfile(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForUpdatePublisherProfile");

    public static bool IsReCaptchaEnabledForEditVisualStudioExtension(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForEditVisualStudioExtension");

    public static bool IsReCaptchaEnabledInCreateCSR(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInCreateCSR");

    public static bool IsCaptchaEnabledOnReviewAndRating(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInReviewAndRating");

    public static bool IsCaptchaEnabledOnQnA(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsRequestContext().IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInQnA");

    public static string GetReCaptchaSiteKey(this HtmlHelper htmlHelper)
    {
      IVssRequestContext vssRequestContext = htmlHelper.ViewContext.TfsRequestContext();
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
    }

    public static string GetReCaptchaUrl(this HtmlHelper htmlHelper)
    {
      if (htmlHelper == null)
        throw new ArgumentNullException(nameof (htmlHelper));
      return "https://www.google.com/recaptcha/api.js";
    }

    private static string GetVerifiedDefaultDomainName(IVssRequestContext context)
    {
      AadService service = context.GetService<AadService>();
      AadTenant tenant;
      try
      {
        AadService aadService = service;
        IVssRequestContext context1 = context;
        GetTenantRequest request = new GetTenantRequest();
        request.ToUserTenant = context.ServiceHost.Is(TeamFoundationHostType.Deployment);
        tenant = aadService.GetTenant(context1, request).Tenant;
      }
      catch (Exception ex)
      {
        context.TraceException(1011806, "Gallery", "HtmlExtension", ex);
        return string.Empty;
      }
      if (tenant == null)
      {
        context.Trace(1011801, TraceLevel.Error, "Gallery", "HtmlExtension", GalleryResources.AADTenantRequestFailed);
        return string.Empty;
      }
      if (tenant.VerifiedDomains == null)
      {
        context.TraceAlways(1011802, TraceLevel.Warning, "Gallery", "HtmlExtension", string.Format("Failed to find any verified domains for tenant {0}.", (object) tenant));
        return tenant.DisplayName;
      }
      if (tenant.VerifiedDomains.Count<AadDomain>((Func<AadDomain, bool>) (x => x != null && x.IsDefault)) > 1)
        context.TraceAlways(1011803, TraceLevel.Warning, "Gallery", "HtmlExtension", string.Format("Found multiple a default verified domains for tenant {0}.", (object) tenant));
      AadDomain aadDomain = tenant.VerifiedDomains.FirstOrDefault<AadDomain>((Func<AadDomain, bool>) (x => x != null && x.IsDefault));
      if (aadDomain == null)
      {
        context.TraceAlways(1011804, TraceLevel.Warning, "Gallery", "HtmlExtension", string.Format("Failed to find a default verified domain for tenant {0}.", (object) tenant));
        return tenant.DisplayName;
      }
      string name = aadDomain.Name;
      if (!string.IsNullOrWhiteSpace(name))
        return name;
      context.TraceAlways(1011805, TraceLevel.Warning, "Gallery", "HtmlExtension", string.Format("The verified domain name is empty for tenant {0}.", (object) tenant));
      return tenant.DisplayName;
    }
  }
}
