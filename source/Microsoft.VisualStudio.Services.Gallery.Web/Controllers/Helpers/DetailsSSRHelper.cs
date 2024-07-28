// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.DetailsSSRHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Markdig;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Helpers;
using Microsoft.VisualStudio.Services.Gallery.Web.Helpers.ItemDetailsPageHelpers;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class DetailsSSRHelper : GalleryControllerHelper
  {
    private const string PropertyBrandingColor = "Microsoft.VisualStudio.Services.Branding.Color";
    private const string PropertyBrandingTheme = "Microsoft.VisualStudio.Services.Branding.Theme";
    private const string PropertySponsorLink = "Microsoft.VisualStudio.Code.SponsorLink";
    private const string PropertyPriceCalculator = "Microsoft.VisualStudio.Services.Content.Pricing.PriceCalculator";
    private const string VSCodeExtensionInstallLink = "vscode:extension/{0}";
    private const string VSCodeInstallHelpUrl = "https://aka.ms/vscode_extn_install";
    private const string VSCodeMoreInfoLink = "http://go.microsoft.com/fwlink/?LinkID=691811&pub={0}&ext={1}";
    private const string PrunedMDLength_RegistryPath = "/Configuration/Service/Gallery/PruningMDLength";
    private const string OptInCookieForEnableRHSAsyncComponentsEnabled = "RHSAsyncComponentsEnabled";
    private const string OptInCookieForEnableSimpleMarkdownRenderingForSSR = "EnableSimpleMarkdownRenderingForSSR";
    private const string c_ChangelogAssetTypeName = "Microsoft.VisualStudio.Services.Content.Changelog";
    private const string c_PrivacyAssetTypeName = "Microsoft.VisualStudio.Services.Content.PrivacyPolicy";
    private int c_DesiredMDLength = 2800;
    private string _galleryUrl;

    public DetailsSSRHelper(Microsoft.VisualStudio.Services.Gallery.Web.GalleryController controller)
      : base(controller)
    {
    }

    public void InitializeViewData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PublishedExtensionResult extensionResult,
      Dictionary<string, double> blockExecutionTimeMap)
    {
      VSTSExtensionItem vssItem = new VSTSExtensionItem(extension);
      IDictionary<string, string> extensionProperties = GalleryUtil.GetExtensionProperties(extension);
      this.ViewData["extensionProperties"] = (object) extensionProperties;
      this.ViewData["ShowConnectedContext"] = (object) (bool) (!this.WebContext.IsHosted ? 0 : (this.ViewData["server-context"] != null ? 1 : 0));
      requestContext.GetService<IProductService>();
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (InitializeViewData), (object) "GetItemBanner"), blockExecutionTimeMap))
        this.ViewData["BannerDetails"] = (object) this.GetItemBanner(requestContext, extension, vssItem, extensionResult);
      this.ViewData["TabsProps"] = (object) this.GetItemTabs(requestContext, extension, extensionResult, vssItem, extensionProperties);
      this._galleryUrl = this.GetGalleryUrl();
      this.ViewData["galleryUrl"] = (object) this._galleryUrl;
      this.ViewData["moreinfo"] = (object) this.GetMoreInfoData(extension);
      this.ViewData["isUnpublished"] = (object) ((extension.Flags & PublishedExtensionFlags.Unpublished) != 0);
      this.ViewData["isLinux"] = (object) this.IsLinuxBasedBrowser(requestContext);
      this.ViewData["IsCSRFeatureEnabled"] = (object) requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSupportRequestFeature");
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (InitializeViewData), (object) "GetExtensionResources"), blockExecutionTimeMap))
        this.ViewData["resources"] = (object) this.GetExtensionResources(extension);
      this.ViewData["resourcesPath"] = (object) this.PageContextProvider.GetResourcesPath(this.TfsRequestContext);
      this.ViewData["currentUrl"] = (object) this.HttpContext.Request.Url.AbsoluteUri;
      this.ViewData["staticResourceVersion"] = (object) StaticResources.Versioned.Version;
      this.ViewData["itemType"] = (object) vssItem.ItemType;
      bool featureFlagStatus = this.GetCookieOrFeatureFlagStatus("RHSAsyncComponentsEnabled", "Microsoft.VisualStudio.Services.Gallery.EnableRHSAsyncComponents");
      ItemDetailsSSRJsonIslandModel ssrJsonIslandModel = new ItemDetailsSSRJsonIslandModel()
      {
        GitHubLink = this.GetGitHubLinkProperty((IEnumerable<KeyValuePair<string, string>>) extensionProperties),
        ReleaseDateString = extension.IsPublic() ? extension.ReleaseDate.ToString("R") : (string) null,
        LastUpdatedDateString = extension.LastUpdated.ToString("R"),
        GalleryUrl = this._galleryUrl,
        Categories = extension.Categories,
        Tags = extension.Tags,
        ExtensionProperties = extensionProperties,
        Resources = this.GetExtensionResources(extension),
        MoreInfo = this.GetMoreInfoData(extension),
        ResourcesPath = this.GetResourcesPath(),
        AssetUri = DetailsSSRHelper.GetAssetUri(extension),
        VsixManifestAssetType = "Microsoft.VisualStudio.Services.VsixManifest",
        StaticResourceVersion = StaticResources.Versioned.Version,
        AfdIdentifier = (string) this.ViewData["AfdIdentifier"],
        ItemType = (int) vssItem.ItemType,
        VsixId = (string) this.ViewData["vsixId"],
        IsMDPruned = this.ViewData["isMDPruned"] != null && (bool) this.ViewData["isMDPruned"],
        PrunedMDLength = this.ViewData["prunedMDLength"] != null ? (int) this.ViewData["prunedMDLength"] : 0,
        OverviewMDLength = this.ViewData["overviewMDLength"] != null ? (int) this.ViewData["overviewMDLength"] : 0,
        IsRHSAsyncComponentsEnabled = featureFlagStatus,
        OfferDetails = this.ViewData["vss-extension-offer"] != null ? RestApiJsonResult.SerializeRestApiData(this.TfsRequestContext, this.ViewData["vss-extension-offer"]) : (string) null,
        IsDetailsTabsEnabled = this.ViewData["detailsTabsSSREnabled"] != null && (bool) this.ViewData["detailsTabsSSREnabled"],
        ShowVersionHistory = this.ShowVersionHistory(requestContext, vssItem, extension),
        IsSeeMoreButtonOnVersionHistoryTab = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSeeMoreButtonOnVersionHistoryTab"),
        IsReferralLinkRedirectionWarningPopupEnabled = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReferralLinkRedirectionWarningPopup"),
        Versions = this.GetVersions(extension),
        IsCSRFeatureEnabled = (bool) this.ViewData["IsCSRFeatureEnabled"]
      };
      if (extension.IsVsExtension() || extension.IsVSTSExtensionResourceOrIntegration() || extension.IsVsCodeExtension())
        ssrJsonIslandModel.WorksWith = (IEnumerable<string>) this.ViewData["worksWith"];
      if (extension.IsVsCodeExtension())
        ssrJsonIslandModel.TargetPlatforms = (IDictionary<string, string>) this.ViewData["target-platforms"];
      this.ViewData["jiContent"] = (object) ssrJsonIslandModel;
      if (extension.IsVsExtension() && GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extension.Metadata))
        this.ViewData["VsVersionToPayloadDownloadLinkMapping"] = (object) this.GetPayloadDownloadLinkForEachVsVersion(requestContext, extension);
      this.InitializeBreadcrumbMembers(extension);
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (InitializeViewData), (object) "AddOverviewHtml"), blockExecutionTimeMap))
      {
        this.AddOverviewHtml(extension, blockExecutionTimeMap);
        int num = ((string) this.ViewData["vss-item-overview-html"]).Length / 1000;
        blockExecutionTimeMap.Add("overviewSize", (double) num);
      }
      this.ViewData["ShowCertifiedBadge"] = (object) this.ShouldShowCertifiedBadge(extension);
      this.ViewData["ShowVerifiedDomainIcon"] = (object) this.ShouldShowVerifiedDomainIcon(extension);
    }

    private IDictionary<string, string> GetPayloadDownloadLinkForEachVsVersion(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      IDictionary<string, string> payloadDownloadLinkForEachVsVersion = (IDictionary<string, string>) new Dictionary<string, string>();
      IProductService productService = requestContext.GetService<IProductService>();
      IDictionary<string, List<InstallationTarget>> installationTargetPerTargetPlatformMap = (IDictionary<string, List<InstallationTarget>>) new Dictionary<string, List<InstallationTarget>>();
      extension.InstallationTargets.ForEach((Action<InstallationTarget>) (it =>
      {
        if (!installationTargetPerTargetPlatformMap.ContainsKey(it.TargetPlatform))
          installationTargetPerTargetPlatformMap[it.TargetPlatform] = new List<InstallationTarget>();
        installationTargetPerTargetPlatformMap[it.TargetPlatform].Add(it);
      }));
      List<List<InstallationTarget>> installationTargetPerTargetPlatformList = new List<List<InstallationTarget>>();
      installationTargetPerTargetPlatformMap.ForEach<KeyValuePair<string, List<InstallationTarget>>>((Action<KeyValuePair<string, List<InstallationTarget>>>) (it => installationTargetPerTargetPlatformList.Add(it.Value)));
      installationTargetPerTargetPlatformList.Sort((Comparison<List<InstallationTarget>>) ((installationTargets1, installationTargets2) => Version.Parse(installationTargets2[0].ExtensionVersion).CompareTo(Version.Parse(installationTargets1[0].ExtensionVersion))));
      installationTargetPerTargetPlatformList.ForEach((Action<List<InstallationTarget>>) (installationTargets =>
      {
        string extensionVersion = installationTargets[0].ExtensionVersion;
        string targetPlatform = installationTargets[0].TargetPlatform;
        this.GetReleasesList(productService.QueryReleases(requestContext, (IList<InstallationTarget>) installationTargets)).ForEach<ProductRelease>((Action<ProductRelease>) (release =>
        {
          if (payloadDownloadLinkForEachVsVersion.ContainsKey(release.Name))
            return;
          string str = URLHelper.GetVSPackageUrl(this._galleryUrl, extension.Publisher.PublisherName, extension.ExtensionName, extensionVersion) + "?targetPlatform=" + targetPlatform;
          payloadDownloadLinkForEachVsVersion[release.Name] = str;
        }));
      }));
      return payloadDownloadLinkForEachVsVersion;
    }

    private IList<ProductRelease> GetReleasesList(
      IDictionary<InstallationTarget, IList<ProductRelease>> releasesDictionary)
    {
      IList<ProductRelease> listOfReleases = (IList<ProductRelease>) new List<ProductRelease>();
      foreach (KeyValuePair<InstallationTarget, IList<ProductRelease>> releases in (IEnumerable<KeyValuePair<InstallationTarget, IList<ProductRelease>>>) releasesDictionary)
      {
        if (releases.Value != null)
        {
          foreach (ProductRelease productRelease in (IEnumerable<ProductRelease>) releases.Value)
          {
            if (!this.isReleaseExists(listOfReleases, productRelease.Id))
              listOfReleases.Add(productRelease);
          }
        }
      }
      return listOfReleases;
    }

    private bool isReleaseExists(IList<ProductRelease> listOfReleases, string Id)
    {
      foreach (object listOfRelease in (IEnumerable<ProductRelease>) listOfReleases)
      {
        if (listOfRelease.Equals((object) Id))
          return true;
      }
      return false;
    }

    public bool IsQnAEnabled(IDictionary<string, string> extensionProperties) => new QnAUtils("Gallery", nameof (DetailsSSRHelper)).GetQnAMode(extensionProperties) != QnAMode.None;

    private static string GetAssetUri(PublishedExtension extension) => extension.Versions[0].AssetUri;

    public void LoadAcqButtonFields(
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails,
      bool isHostedConnectedContext,
      out string acqButtonLink,
      out string acqButtonText,
      out string actionDescriptionHtml,
      out bool isAcqDisabled,
      out bool acqLinkNewTab)
    {
      acqLinkNewTab = false;
      string galleryUrl = this.GetGalleryUrl();
      actionDescriptionHtml = "";
      isAcqDisabled = false;
      acqButtonText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.GetButtonText;
      acqButtonLink = "#";
      bool flag = this.ExtensionSupportsOnlyOnPrem(extension);
      if (extension.IsVsCodeExtension())
      {
        acqButtonLink = string.Format("vscode:extension/{0}", (object) extension.GetFullyQualifiedName());
        acqButtonText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemInstall;
      }
      else if (extension.IsVsExtension())
      {
        acqButtonText = extension.DeploymentType != ExtensionDeploymentTechnology.ReferralLink ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.DownloadButtonText : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemGetStarted;
        acqButtonLink = URLHelper.GetVSPackageUrl(galleryUrl, extension.Publisher.PublisherName, extension.ExtensionName, extension.Versions[0].Version);
      }
      else if (GalleryUtil.IsVSTSOrTFSIntegrationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets))
      {
        acqLinkNewTab = true;
        string property1 = extension.GetProperty("latest", "Microsoft.VisualStudio.Services.Links.Getstarted");
        string property2 = extension.GetProperty("latest", "Microsoft.VisualStudio.Services.Links.Install");
        if (!string.IsNullOrEmpty(property2))
        {
          acqButtonLink = property2;
          acqButtonText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.InstallText;
        }
        else
        {
          acqButtonLink = string.IsNullOrEmpty(property1) ? "#" : property1;
          acqButtonText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemGetStarted;
          if (extension.IsPublic() && !string.IsNullOrEmpty(property1))
            acqButtonLink = extension.Versions[0].FallbackAssetUri + "/Microsoft.VisualStudio.Services.Links.Getstarted";
        }
        if (string.IsNullOrEmpty(acqButtonLink) || acqButtonLink[0] != '#')
          return;
        acqLinkNewTab = false;
      }
      else
      {
        if (isHostedConnectedContext)
        {
          if (GalleryUtil.IsOnPremSupported((IEnumerable<InstallationTarget>) extension.InstallationTargets))
          {
            bool isTestCommerceEnabled = this._isTestCommerceEnabled();
            bool freeInstall = false;
            if (this._isThirdPartyPaidExtensionWithoutOfferPlans(extension, offerDetails, isTestCommerceEnabled))
              freeInstall = true;
            acqButtonLink = this.GetOnPremRedirectURL(extension.GetFullyQualifiedName(), extension, freeInstall);
          }
        }
        else
          acqButtonLink = URLHelper.GetAzureDevOpsAcqLink(galleryUrl, extension);
        acqButtonText = GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || extension.IsByolExtension() || this._IsPaid(extension, offerDetails) || GalleryUtil.IsHostedResourceInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.GetButtonText : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.GetItFree;
        if (isHostedConnectedContext && !this.ExtensionSupportsConnectedTfsVersion(extension) || !isHostedConnectedContext & flag && extension.ShouldNotDownload())
        {
          isAcqDisabled = true;
          acqButtonLink = "#";
        }
        if (!isHostedConnectedContext)
          return;
        actionDescriptionHtml = this.GetActionDescriptionHtml(extension, acqButtonText, isAcqDisabled);
      }
    }

    private bool ExtensionSupportsOnlyOnPrem(PublishedExtension extension)
    {
      foreach (InstallationTarget installationTarget in extension.InstallationTargets)
      {
        if (!string.Equals(installationTarget.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) && !string.Equals(installationTarget.Target, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    private bool _IsPaid(PublishedExtension extension, ExtensionOfferDetails offerDetails) => extension.IsFirstPartyAndPaid() || this._isThirdPartyAndPaid(extension, offerDetails);

    private bool _isThirdPartyAndPaid(
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || this.ViewData.ContainsKey("server-context"))
        return extension.IsThirdPartyAndPaid();
      bool isTestCommerceEnabled = this._isTestCommerceEnabled();
      bool flag = this._isOfferDetailsPresent(offerDetails, isTestCommerceEnabled);
      return extension.IsThirdPartyAndPaid() & flag && !extension.IsPreview();
    }

    private bool _isTestCommerceEnabled()
    {
      string str = HttpUtility.ParseQueryString(this.HttpContext.Request.Url.Query)["testCommerce"];
      bool result = false;
      if (str != null)
        bool.TryParse(str, out result);
      return result;
    }

    private bool _isThirdPartyPaidExtensionWithoutOfferPlans(
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails,
      bool isTestCommerceEnabled)
    {
      return extension.IsThirdParty() && this._isOfferDetailsPresent(offerDetails, isTestCommerceEnabled);
    }

    private bool _isOfferDetailsPresent(
      ExtensionOfferDetails offerDetails,
      bool isTestCommerceEnabled)
    {
      if (offerDetails == null)
        return false;
      return offerDetails.HasPublicPlans || offerDetails.HasPlans & isTestCommerceEnabled;
    }

    private void AddOverviewHtml(
      PublishedExtension extension,
      Dictionary<string, double> blockExecutionTimeMap)
    {
      string overviewAbsentString = (string) this.ViewData["vss-item-overview"];
      if (string.IsNullOrWhiteSpace(overviewAbsentString))
        overviewAbsentString = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemOverviewAbsentString;
      try
      {
        using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (AddOverviewHtml), (object) "ConvertMarkdownToHtml"), blockExecutionTimeMap))
        {
          string empty = string.Empty;
          string html;
          if (this.GetCookieOrFeatureFlagStatus("EnableSimpleMarkdownRenderingForSSR", "Microsoft.VisualStudio.Services.Gallery.EnableSimpleMarkdownRenderingForSSR"))
          {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAutoLinks().UsePipeTables().UseGridTables().UseAutoIdentifiers().Build();
            html = Markdown.ToHtml(overviewAbsentString, pipeline);
          }
          else
            html = new MarkdownService().ConvertMarkdownToHtml(overviewAbsentString, extension, this.GetResourcesPath(), blockExecutionTimeMap);
          this.ViewData["vss-item-overview-html"] = (object) SafeHtmlWrapper.MakeSafe(html);
        }
      }
      catch (Exception ex)
      {
        this.ViewData["vss-item-overview-html"] = (object) string.Empty;
        this.TfsRequestContext.TraceException(12062072, "gallery", "web", ex);
      }
    }

    public string GetGitHubLinkProperty(
      IEnumerable<KeyValuePair<string, string>> properties)
    {
      if (properties == null)
        return "";
      foreach (KeyValuePair<string, string> property in properties)
      {
        if (string.Compare(property.Key, "Microsoft.VisualStudio.Services.Links.GitHub", true) == 0)
          return property.Value;
      }
      return "";
    }

    public string GetGalleryUrl() => this.ViewData["generalInfo"] == null || (this.ViewData["generalInfo"] as Dictionary<string, object>)["galleryUrl"] == null ? "/" : (this.ViewData["generalInfo"] as Dictionary<string, object>)["galleryUrl"] as string;

    public string GetResourcesPath() => (this.ViewData["generalInfo"] as Dictionary<string, object>)["resourcesPath"] as string;

    public ItemDetailsResourcesSectionModel GetExtensionResources(PublishedExtension extension) => new ItemDetailsResourcesSectionModel()
    {
      LicenseText = this.isAssetPresent(extension, "Microsoft.VisualStudio.Services.Content.License") ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.LicenseText : "",
      ChangelogText = this.isAssetPresent(extension, "Microsoft.VisualStudio.Services.Content.Changelog") ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ChangelogText : "",
      PublisherName = extension.Publisher.PublisherName,
      ExtensionName = extension.ExtensionName,
      Version = extension.Versions[0].Version
    };

    public List<ItemDetailsVersionModel> GetVersions(PublishedExtension extension)
    {
      List<ItemDetailsVersionModel> versions = new List<ItemDetailsVersionModel>();
      foreach (ExtensionVersion version in extension.Versions)
        versions.Add(new ItemDetailsVersionModel()
        {
          version = version.Version,
          targetPlatform = version.TargetPlatform,
          lastUpdated = version.LastUpdated.ToString("R")
        });
      return versions;
    }

    public bool isAssetPresent(PublishedExtension extension, string assetType)
    {
      if (extension.Versions[0].Files == null)
        return false;
      foreach (ExtensionFile file in extension.Versions[0].Files)
      {
        if (string.Equals(file.AssetType, assetType, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public ItemDetailsMoreInfoModel GetMoreInfoData(PublishedExtension extension) => new ItemDetailsMoreInfoModel()
    {
      VersionValue = extension.Versions[0].Version,
      PublisherValue = extension.Publisher.DisplayName,
      UniqueIdentifierValue = extension.GetFullyQualifiedName(),
      IsPublic = extension.IsPublic(),
      TwitterShareContents = Uri.EscapeDataString(string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemShareTwitterContents, (object) this.HttpContext.Request.Url.AbsoluteUri)),
      EmailShareContents = Uri.EscapeDataString(string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemShareEmailContents, (object) Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemShareExtension, (object) this.HttpContext.Request.Url.AbsoluteUri)),
      EmailShareSubject = Uri.EscapeDataString(string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemShareEmailSubject, (object) extension.DisplayName, (object) Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCode))
    };

    public void InitializeBreadcrumbMembers(PublishedExtension extension) => this.ViewData["bread-crumb-props"] = (object) BreadcrumbHelper.GetBreadcrumbProps(extension, BreadcrumbHelper.GetBreadCrumbMembers(extension, this.ViewData["generalInfo"] as Dictionary<string, object>));

    public ItemTabsProps GetItemTabs(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PublishedExtensionResult extensionResult,
      VSTSExtensionItem vssItem,
      IDictionary<string, string> extensionProperties)
    {
      bool flag1;
      bool flag2;
      bool flag3;
      if (GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets))
      {
        flag1 = false;
        flag2 = false;
        flag3 = false;
      }
      else
      {
        flag1 = this.IsQnAEnabled(extensionProperties);
        flag2 = true;
        flag3 = this.ShowVersionHistory(requestContext, vssItem, extension);
      }
      return new ItemTabsProps()
      {
        OverviewTabTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsOverviewTab,
        PricingTabTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsPricingTab,
        QnATabTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsQnaTab,
        RnRTabTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsRnRTab,
        VersionHistoryTabTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsVersionHistoryTab,
        ShowQnA = flag1,
        ShowRnR = flag2,
        ShowVersionHistory = flag3,
        ShowPricing = this.ShouldShowPricingTab(extension, extensionResult, vssItem, extensionProperties)
      };
    }

    private bool ShowVersionHistory(
      IVssRequestContext requestContext,
      VSTSExtensionItem vssItem,
      PublishedExtension extension)
    {
      if ((extension.Flags & PublishedExtensionFlags.Unpublished) != PublishedExtensionFlags.None)
        return false;
      if (vssItem.ItemType == VSSItemType.VSCodeExtension)
        return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVersionHistoryViewForVSCode");
      return vssItem.ItemType == VSSItemType.VSIdeExtension && vssItem.DeploymentTechnology != ExtensionDeploymentTechnology.ReferralLink && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVersionHistoryViewForVS");
    }

    private bool ShouldShowPricingTab(
      PublishedExtension extension,
      PublishedExtensionResult extensionResult,
      VSTSExtensionItem vssItem,
      IDictionary<string, string> extensionProperties)
    {
      if (!extension.IsPaid())
        return false;
      bool flag = this.HasPricingAsset(extension, extensionResult);
      if (extension.IsFirstParty())
      {
        if (flag || !extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.Content.Pricing.PriceCalculator") || !extensionProperties["Microsoft.VisualStudio.Services.Content.Pricing.PriceCalculator"].Equals("false", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      else
      {
        if (extension.IsByolExtension() & flag)
          return true;
        IEnumerable<IOfferMeterPrice> source = this.ViewData["vss-extension-offer-meter-price"] != null ? (IEnumerable<IOfferMeterPrice>) this.ViewData["vss-extension-offer-meter-price"] : (IEnumerable<IOfferMeterPrice>) null;
        ExtensionOfferDetails offerDetails = (ExtensionOfferDetails) this.ViewData["vss-extension-offer"];
        if (this.shouldShowOfferPlans(extension, offerDetails) && source != null && source.Count<IOfferMeterPrice>() > 0)
          return true;
      }
      return false;
    }

    private bool HasPricingAsset(
      PublishedExtension extension,
      PublishedExtensionResult extensionResult)
    {
      ExtensionVersion extensionVersion = GalleryServerUtil.GetLatestValidatedExtensionVersion(extension.Versions);
      return this.ContainsAsset(extension.Publisher.PublisherName, extension.ExtensionName, extensionVersion.Version, "Microsoft.VisualStudio.Services.Content.Pricing", extensionResult.ExtensionAssetsToken);
    }

    public ItemBannerModel GetItemBanner(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      VSTSExtensionItem vssItem,
      PublishedExtensionResult extensionResult)
    {
      IDictionary<string, string> extensionProperties = (IDictionary<string, string>) this.ViewData["extensionProperties"];
      int num = DetailsSSRHelper.IsReferralLinkType(extension) ? 1 : 0;
      bool flag1 = this.ShouldShowPublisherOptionsLinks(extension);
      bool isGetStartedType = num != 0 || GalleryUtil.IsVSTSOrTFSIntegrationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
      string helpText = "";
      bool isHelpTextVisible = false;
      ExtensionOfferDetails offerDetails = (ExtensionOfferDetails) this.ViewData["vss-extension-offer"];
      bool newAcquisitionExperienceEnabled = this.IsNewAcquisitionExperienceEnabled(extension, vssItem, offerDetails);
      Dictionary<string, string> properties = this.ViewData["server-context"] != null ? JsonUtilities.Deserialize<Dictionary<string, string>>((string) this.ViewData["server-context"]) : new Dictionary<string, string>();
      bool isHostedConnectedContext = this.WebContext.IsHosted && this.OnPremServerHasInternetAccess(properties);
      string pricingCategoryText = this.GetPricingCategoryText(vssItem, extension, ref helpText, ref isHelpTextVisible, offerDetails, extensionResult, isHostedConnectedContext, newAcquisitionExperienceEnabled);
      string str = VSTSExtensionItem.GetAssetUrl(extension, "Microsoft.VisualStudio.Services.Icons.Branding");
      if (string.IsNullOrEmpty(str))
        str = VSTSExtensionItem.GetAssetUrl(extension, "Microsoft.VisualStudio.Services.Icons.Default");
      bool flag2 = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ShowLargeThumbnailAsBrandingIcon");
      if (extension.IsVsExtension() && !flag2)
        str = VSTSExtensionItem.GetAssetUrl(extension, "Microsoft.VisualStudio.Services.Icons.Default");
      if (string.IsNullOrWhiteSpace(str))
        str = this.PageContextProvider.GetResourcesPath(this.TfsRequestContext) + "Header/default_icon_128.png";
      return new ItemBannerModel()
      {
        BrandingColor = this.GetBrandingColor(extensionProperties),
        ImageUrl = str,
        ImageAlt = extension.DisplayName,
        BrandingTheme = this.GetBrandingTheme(extensionProperties),
        ItemName = extension.DisplayName,
        PublisherDisplayName = extension.Publisher.DisplayName,
        PublisherLink = this.GetPublisherLink(extension.Publisher),
        PublisherPageLinkDescription = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemDetailsToPublisherPageLink, (object) extension.Publisher.DisplayName),
        BrandingThemeColor = this.GetBrandingThemeColor(this.GetBrandingTheme(extensionProperties)),
        SponsorThemeColor = this.GetSponsorThemeColor(this.GetBrandingTheme(extensionProperties)),
        VerifiedDomainText = this.GetVerifiedDomainText(vssItem),
        DomainUrl = vssItem.PublisherDomain,
        DomainName = this.GetDomain(vssItem),
        InstallsText = this.GetInstallsText(vssItem.InstallCount, isGetStartedType),
        InstallsHoverText = isGetStartedType || vssItem.InstallCount == 0 ? string.Empty : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.InstallsHoverText,
        RnRDetails = this.GetRnRDetails(vssItem, extensionProperties),
        ItemPriceCategoryText = pricingCategoryText,
        SponsorLink = this.GetSponsorLink(extensionProperties),
        IsHelpTextVisible = isHelpTextVisible,
        HelpText = helpText,
        ItemDescription = !string.IsNullOrEmpty(extension.LongDescription) ? extension.LongDescription : extension.ShortDescription,
        ItemAction = this.GetItemAction(requestContext, extension, offerDetails, isHostedConnectedContext),
        ExtensionTitleTag = (extension.Flags & PublishedExtensionFlags.Preview) == PublishedExtensionFlags.None || newAcquisitionExperienceEnabled ? "" : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPreview,
        ReportsLink = flag1 ? URLHelper.GetReportsLink(this._galleryUrl, extension.Publisher.PublisherName, extension.ExtensionName) : (string) null,
        ReportsLinkDisplayName = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ReportsLinkDisplayName,
        GalleryItemEditLink = flag1 ? URLHelper.GetGalleryItemEditLink(this._galleryUrl, extension.Publisher.PublisherName, extension.ExtensionName) : (string) null,
        GalleryItemEditLinkDisplayName = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.GalleryItemManageLinkDisplayName,
        IsGetStartedType = isGetStartedType,
        IsVssExtensionOrResource = extension.IsVSTSExtensionResourceOrIntegration()
      };
    }

    internal string GetPricingCategoryText(
      VSTSExtensionItem vssItem,
      PublishedExtension extension,
      ref string helpText,
      ref bool isHelpTextVisible,
      ExtensionOfferDetails offerDetails,
      PublishedExtensionResult extensionResult,
      bool isHostedConnectedContext,
      bool newAcquisitionExperienceEnabled)
    {
      bool flag1 = extension.IsByolExtension();
      string pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.FreeLabel;
      int num = (extension.Flags & PublishedExtensionFlags.Paid) != PublishedExtensionFlags.None ? 1 : (extension.Tags == null ? 0 : (extension.Tags.Contains("$IsPaid") ? 1 : 0));
      string property = extension.GetProperty("latest", "Microsoft.VisualStudio.Services.GalleryProperties.TrialDays");
      bool flag2 = false;
      bool flag3 = this.shouldShowOfferPlans(extension, offerDetails);
      IEnumerable<IOfferMeterPrice> source = this.ViewData["vss-extension-offer-meter-price"] != null ? this.ViewData["vss-extension-offer-meter-price"] as IEnumerable<IOfferMeterPrice> : (IEnumerable<IOfferMeterPrice>) null;
      bool flag4 = this.HasPricingAsset(extension, extensionResult);
      if (num != 0)
      {
        if (string.Equals(extension.Publisher.DisplayName, "Microsoft", StringComparison.InvariantCultureIgnoreCase))
        {
          if (flag4)
            flag2 = true;
        }
        else if (flag1 || flag3 && source != null && source.Count<IOfferMeterPrice>() > 0)
          flag2 = true;
      }
      if (newAcquisitionExperienceEnabled)
      {
        if (flag2)
        {
          if (extension.IsPreview())
          {
            pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPaidPreview;
          }
          else
          {
            bool flag5 = vssItem.ItemType == VSSItemType.VSSExtension;
            if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !this.ViewData.ContainsKey("server-context"))
            {
              if (flag5 && !flag1)
              {
                helpText = string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemTrialDays, (object) "30");
                isHelpTextVisible = true;
              }
              else if (flag1 && property != null)
              {
                helpText = string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemTrialDays, (object) property);
                isHelpTextVisible = true;
              }
            }
            pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPaid;
          }
        }
        else
          pricingCategoryText = offerDetails == null || !offerDetails.HasPlans ? (!extension.IsPreview() ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.FreeLabel : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPreview) : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPaidPreview;
      }
      else
      {
        switch (vssItem.CostCategory)
        {
          case 0:
            pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.FreeLabel;
            break;
          case 1:
            pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.FreeTrialLabel;
            break;
          case 2:
            pricingCategoryText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemPaid;
            break;
        }
      }
      return pricingCategoryText;
    }

    internal bool IsNewAcquisitionExperienceEnabled(
      PublishedExtension extension,
      VSTSExtensionItem vssItem,
      ExtensionOfferDetails offerDetails)
    {
      return !extension.IsVsCodeExtension() && !extension.IsVsExtension() && extension != null && vssItem != null && (vssItem.ItemType == VSSItemType.VssHostedResource || this.IsPureHosted() && (vssItem.ItemType == VSSItemType.VSSExtension || extension.IsFirstPartyAndPaid() || this._isThirdPartyAndPaid(extension, offerDetails)) || vssItem.ItemType == VSSItemType.VSSOffer && extension.IsPaid() || !this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && GalleryUtil.IsVSTSOrTFSInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets) && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableNewAcquisitionOnPremExperience"));
    }

    private bool shouldShowOfferPlans(
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool onPremServerHasInternetAccess = this.OnPremServerHasInternetAccess(this.ViewData["server-context"] != null ? JsonUtilities.Deserialize<Dictionary<string, string>>((string) this.ViewData["server-context"]) : new Dictionary<string, string>());
      if (GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) || this.IsThirdPartyExtensionAvailableForPurchase(extension, offerDetails, onPremServerHasInternetAccess) && !extension.IsByolEnabledExtension() || extension.IsFirstPartyAndPaid() && this.WebContext.IsHosted | onPremServerHasInternetAccess)
        flag2 = true;
      if (this.WebContext.IsHosted & flag2 && extension.IsVSTSExtensionResourceOrIntegration())
        flag1 = true;
      return flag1;
    }

    private bool IsThirdPartyExtensionAvailableForPurchase(
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails,
      bool onPremServerHasInternetAccess)
    {
      return extension.IsThirdPartyAndPaid() && (onPremServerHasInternetAccess || this.IsPureHosted()) && offerDetails != null && (offerDetails.HasPublicPlans || offerDetails.HasPlans && this._isTestCommerceEnabled());
    }

    private bool IsPureHosted() => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !this.ViewData.ContainsKey("server-context");

    private static bool IsReferralLinkType(PublishedExtension extension) => extension.DeploymentType == ExtensionDeploymentTechnology.ReferralLink;

    private bool ShouldShowPublisherOptionsLinks(PublishedExtension extension)
    {
      if (((this.ViewData["has-publisher-role-reader"] != null ? ((bool) this.ViewData["has-publisher-role-reader"] ? 1 : 0) : 0) | (this.ViewData["can-update-extension"] != null ? ((bool) this.ViewData["can-update-extension"] ? 1 : 0) : (false ? 1 : 0))) == 0)
        return false;
      return extension.IsVsExtension() || extension.IsVSTSExtensionResourceOrIntegration();
    }

    public ItemActionModel GetItemAction(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionOfferDetails offerDetails,
      bool isHostedConnectedContext)
    {
      string str1 = "Ctrl";
      if (requestContext.UserAgent != null && requestContext.UserAgent.Contains("Mac"))
        str1 = "⌘";
      string str2 = string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeInstallInstructions, (object) str1);
      bool acqLinkNewTab = false;
      bool isAcqDisabled = false;
      string acqButtonLink;
      string acqButtonText;
      string actionDescriptionHtml;
      this.LoadAcqButtonFields(extension, offerDetails, isHostedConnectedContext, out acqButtonLink, out acqButtonText, out actionDescriptionHtml, out isAcqDisabled, out acqLinkNewTab);
      return new ItemActionModel()
      {
        ActionHelpLink = "https://aka.ms/vscode_extn_install",
        ActionHelpText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeExtensionHelpText,
        FullyQualifiedName = extension.GetFullyQualifiedName(),
        GalleryUrl = this.GetGalleryUrl(),
        InstallTitle = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeInstallationHeader,
        InstallInstructions = str2,
        CopyCommand = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ext install {0}", (object) extension.GetFullyQualifiedName()),
        CopyButtonTooltip = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeCopyButtonTooptip,
        CopyButtonText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeCopyButtonText,
        CopiedAnimationText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VSCodeCopiedAnimationText,
        MoreInfoLink = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://go.microsoft.com/fwlink/?LinkID=691811&pub={0}&ext={1}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName),
        MoreInfoText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.MoreInfoText,
        UnpublishedText = Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.UnpublishedExtensionNote,
        AcqButtonLink = acqButtonLink,
        AcqButtonText = acqButtonText,
        AcqLinkNewTab = acqLinkNewTab,
        IsAcqDisabled = isAcqDisabled,
        SearchTarget = this.GetSearchTarget(extension),
        ActionDescriptionHtml = actionDescriptionHtml
      };
    }

    private string GetActionDescriptionHtml(
      PublishedExtension extension,
      string AcqButtonText,
      bool isAcqDisabled)
    {
      if (isAcqDisabled)
        return Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.UnsupportedOnPremVersion;
      string onPremRedirectUrl = this.GetOnPremRedirectURL(extension.GetFullyQualifiedName(), extension);
      return string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ConnectedContext_Install_WarningText, (object) ("<strong>" + AcqButtonText + "</strong>"), (object) ("<strong>" + this.GetOnPremServerUrlFromInstallPath(onPremRedirectUrl) + "</strong>"));
    }

    private string GetOnPremServerUrlFromInstallPath(string installPath)
    {
      if (string.IsNullOrEmpty(installPath))
        return "#";
      Uri uri = new Uri(installPath);
      if (uri.Port == 0)
        return uri.Scheme + "://" + uri.Host;
      return uri.Scheme + "://" + uri.Host + ":" + uri.Port.ToString();
    }

    private string GetSearchTarget(PublishedExtension extension)
    {
      string searchTarget = "AzureDevOps";
      if (extension.IsVsCodeExtension())
        searchTarget = "VSCode";
      else if (extension.IsVsExtension())
        searchTarget = "VS&vsVersion=vs2019";
      return searchTarget;
    }

    public bool IsLinuxBasedBrowser(IVssRequestContext requestContext)
    {
      string userAgent = requestContext.UserAgent != null ? requestContext.UserAgent : "";
      return !userAgent.Contains("Windows") && !userAgent.Contains("Mac OS X");
    }

    public RnR GetRnRDetails(
      VSTSExtensionItem vssItem,
      IDictionary<string, string> extensionProperties)
    {
      string brandingTheme = this.GetBrandingTheme(extensionProperties);
      bool flag = !string.IsNullOrEmpty(brandingTheme) && string.Equals(brandingTheme, "dark", StringComparison.OrdinalIgnoreCase);
      int num1 = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/RatingAndReview/RegistryPathForMinimumReviewsToShowStarRatings", 0);
      double num2 = vssItem.RatingCount <= num1 ? 0.0 : vssItem.Rating;
      string str1 = vssItem.RatingCount > 1 ? GalleryCommonResources.RatingPluralText : GalleryCommonResources.RatingSingularText;
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.AverageRatingHoverText, (object) Math.Round(num2, 1), (object) vssItem.RatingCount, (object) str1);
      string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.RatingsScreenReaderText, (object) str2);
      return new RnR()
      {
        RatingsScreenReaderText = str3,
        BannerAverageRatingText = str2,
        RatingCount = vssItem.RatingCount,
        Rating = num2,
        ResourcesPath = this.GetResourcesPath(),
        StarType = flag ? "light" : "dark"
      };
    }

    public string GetVerifiedDomainText(VSTSExtensionItem vssItem)
    {
      if (string.IsNullOrWhiteSpace(vssItem.PublisherDomain))
        return string.Empty;
      string str = Regex.Replace(new Uri(vssItem.PublisherDomain).Host, "^(?i)www.", "");
      return string.Format(Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.VerifiedDomainIconTooltip, (object) vssItem.Author, (object) str);
    }

    public string GetDomain(VSTSExtensionItem vssItem) => string.IsNullOrWhiteSpace(vssItem.PublisherDomain) ? string.Empty : Regex.Replace(new Uri(vssItem.PublisherDomain).Host, "^(?i)www.", "");

    public string GetInstallsText(int installCount, bool isGetStartedType)
    {
      if (installCount == 0)
        return string.Empty;
      string localeString = this.ConvertNumberToLocaleString(installCount);
      string empty = string.Empty;
      return string.Format(installCount != 1 ? (isGetStartedType ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemClicksText : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemInstallsText) : (isGetStartedType ? Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemClickText : Microsoft.VisualStudio.Services.Gallery.Web.GalleryResources.ItemInstallText), (object) localeString);
    }

    public string ConvertNumberToLocaleString(int num)
    {
      NumberFormatInfo numberFormat = new CultureInfo("en-US").NumberFormat;
      return num.ToString("N0", (IFormatProvider) numberFormat);
    }

    public string GetPublisherLink(PublisherFacts publisher)
    {
      string str = this._galleryUrl + "publishers/";
      return Array.IndexOf<string>(new string[2]
      {
        "microsoft",
        "microsoft devlabs"
      }, publisher.DisplayName.ToLower()) < 0 ? str + publisher.PublisherName : str + publisher.DisplayName;
    }

    public string GetBrandingColor(IDictionary<string, string> extensionProperties)
    {
      string brandingColor = (string) null;
      if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.Branding.Color"))
        brandingColor = extensionProperties["Microsoft.VisualStudio.Services.Branding.Color"];
      return brandingColor;
    }

    public string GetBrandingTheme(IDictionary<string, string> extensionProperties)
    {
      string brandingTheme = "light";
      if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Services.Branding.Theme"))
        brandingTheme = extensionProperties["Microsoft.VisualStudio.Services.Branding.Theme"].ToLower();
      return brandingTheme;
    }

    public string GetSponsorLink(IDictionary<string, string> extensionProperties)
    {
      string sponsorLink = "";
      if (extensionProperties.ContainsKey("Microsoft.VisualStudio.Code.SponsorLink"))
        sponsorLink = extensionProperties["Microsoft.VisualStudio.Code.SponsorLink"];
      return sponsorLink;
    }

    public string GetBrandingThemeColor(string brandingTheme)
    {
      string empty = string.Empty;
      return !string.Equals(brandingTheme, "dark", StringComparison.OrdinalIgnoreCase) ? "#000000" : "#ffffff";
    }

    public string GetSponsorThemeColor(string brandingTheme)
    {
      string empty = string.Empty;
      return !string.Equals(brandingTheme, "dark", StringComparison.OrdinalIgnoreCase) ? "#B51E78" : "#D61B8C";
    }

    internal void getPartialOverviewMD(
      string extensionFQN,
      string overviewMD,
      out string prunedMD,
      out bool isMDPruned)
    {
      isMDPruned = false;
      int lengthFromRegistry = this.GetPruningLengthFromRegistry();
      int desiredPruningLength = lengthFromRegistry != 0 ? lengthFromRegistry : this.c_DesiredMDLength;
      if (string.IsNullOrEmpty(overviewMD) || overviewMD.Length <= desiredPruningLength)
      {
        prunedMD = overviewMD;
      }
      else
      {
        int pruningLocation = this.CalculatePruningLocation(extensionFQN, overviewMD, out isMDPruned, desiredPruningLength);
        prunedMD = overviewMD.Substring(0, pruningLocation);
      }
    }

    public int CalculatePruningLocation(
      string extensionFQN,
      string overviewMD,
      out bool isMDPruned,
      int desiredPruningLength)
    {
      if (overviewMD == null || overviewMD.Length == 0)
      {
        isMDPruned = false;
        return 0;
      }
      int length = overviewMD.Length;
      if (length <= desiredPruningLength)
      {
        isMDPruned = false;
        return length;
      }
      try
      {
        int pruningLocation = overviewMD.IndexOf("\n\n", desiredPruningLength);
        if (pruningLocation == -1)
        {
          pruningLocation = overviewMD.IndexOf("\r\n\r\n", desiredPruningLength);
          if (pruningLocation == -1)
          {
            isMDPruned = false;
            return length;
          }
        }
        isMDPruned = true;
        return pruningLocation;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        isMDPruned = false;
        this.TfsRequestContext.Trace(12060022, TraceLevel.Error, this.AreaName, this.LayerName, string.Format("ArgumentOutOfRangeException encountered when calculating Pruned length for extension:{0} with overview length: {1}, desired length: {2}, exception: {3}", (object) extensionFQN, (object) overviewMD.Length, (object) desiredPruningLength, (object) ex.Message));
        return overviewMD.Length;
      }
    }

    private int GetPruningLengthFromRegistry() => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/PruningMDLength", 0);

    private bool ShouldShowCertifiedBadge(PublishedExtension extension) => this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCertifiedPublisherUIChanges") && extension != null && extension.IsVSTSExtensionResourceOrIntegration() && extension.Publisher != null && extension.Publisher.Flags.HasFlag((Enum) PublisherFlags.Certified);

    private bool ShouldShowVerifiedDomainIcon(PublishedExtension extension) => this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVerifiedPublisherDomain") && extension.Publisher.IsDomainVerified;
  }
}
