// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AssetByNameController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class AssetByNameController : GalleryController
  {
    private HashSet<string> AssetTypeForStatIncrementSet = new HashSet<string>((IEnumerable<string>) new List<string>()
    {
      "Microsoft.VisualStudio.Services.VSIXPackage",
      "Microsoft.VisualStudio.Services.CommonVSIXPackage",
      "Microsoft.VisualStudio.Ide.Payload"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string s_area = "AssetByNameController";
    private const string s_layer = "ApiController";
    private const string s_itemId = "ItemId";
    private const double c_defaultAssetCacheDurationHours = 8760.0;
    private static readonly RegistryQuery s_customCacheDurationRegistryPath = new RegistryQuery("/Configuration/Gallery/Assets/ClientCacheControlMaxAgeHours");

    protected override bool AllowAssetDomain => true;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Track refactor with a backlog item")]
    protected HttpResponseMessage GetAsset(
      string publisherName,
      string extensionName,
      string version,
      string assetType,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      bool install = false,
      bool onPremDownload = false,
      bool redirect = false,
      bool update = false,
      string targetPlatform = null)
    {
      List<AssetInfo> assetTypes = new List<AssetInfo>();
      if (assetType == null)
        throw new ArgumentException(GalleryResources.AssetNotFound());
      if (!redirect)
      {
        if (this.Request.Headers.UserAgent != null && this.Request.Headers.UserAgent.ToString().StartsWith("VSCode", StringComparison.OrdinalIgnoreCase))
          this.SendVSCodeFallbackCiEvent(this.TfsRequestContext, publisherName, extensionName, assetType, version, targetPlatform);
        if (this.Request.Headers.UserAgent != null && this.Request.Headers.UserAgent.ToString().StartsWith("VSCode", StringComparison.OrdinalIgnoreCase))
        {
          if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockDirectAssetRequestsFromVSCode "))
            throw new ExtensionAssetAccessNotSupportedException(GalleryResources.ExtensionAssetDirectAccessNotSupportedText());
          if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.RedirectDirectAssetRequestsFromVSCode"))
            redirect = true;
        }
      }
      if (this.Request.Headers.AcceptLanguage != null)
      {
        foreach (StringWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.AcceptLanguage)
          assetTypes.Add(new AssetInfo(assetType, qualityHeaderValue.Value));
      }
      assetTypes.Add(new AssetInfo(assetType, (string) null));
      IPublisherAssetService service1 = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IPublisherAssetService>();
      Uri redirectionUrl = (Uri) null;
      bool forceRedirect = false;
      PublishedExtension extension = new PublishedExtension();
      HttpResponseMessage response;
      if (assetType.Equals("Microsoft.VisualStudio.Services.Links.Getstarted", StringComparison.OrdinalIgnoreCase) && this.IsRequestForVSTSIntegrationGetStarted(this.TfsRequestContext, publisherName, extensionName, ref extension, version))
      {
        this.FetchRedirectionUrlAndForceRedirectForVstsIntegration(extension, version, assetType, ref redirectionUrl, ref forceRedirect);
        response = this.Request.CreateResponse(HttpStatusCode.Found);
        response.Headers.Location = redirectionUrl;
        this.AddCacheControlHeader(this.TfsRequestContext, response);
        if (install)
          this.TfsRequestContext.GetService<IExtensionStatisticService>().UpdateStatistics(this.TfsRequestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
          {
            new ExtensionStatisticUpdate()
            {
              Statistic = new ExtensionStatistic()
              {
                StatisticName = nameof (install),
                Value = 1.0
              },
              Operation = ExtensionStatisticOperation.Increment,
              PublisherName = publisherName,
              ExtensionName = extensionName
            }
          });
      }
      else
      {
        ExtensionAsset requestedAsset = service1.QueryAsset(this.TfsRequestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, accountToken, assetToken, acceptDefault, targetPlatform);
        if (this.IsRequestForVsExtensionPayload(requestedAsset.Extension, assetType))
          this.FetchRedirectionUrlAndForceRedirectForVsExtension(requestedAsset.Extension, ref redirectionUrl, ref forceRedirect);
        if (requestedAsset.AssetFile != null && !requestedAsset.AssetFile.AssetType.Equals(assetType, StringComparison.Ordinal))
          this.SendGetAssetDifferentCaseCiEvent(this.TfsRequestContext, publisherName, extensionName, requestedAsset.AssetFile.AssetType, assetType);
        bool flag1 = false;
        bool bModifyVsixManifest = false;
        if (!forceRedirect && service1.DirectAssetRequestSupported(this.TfsRequestContext, requestedAsset, redirect))
        {
          if (this.IsRequestForVsExtensionPayload(requestedAsset.Extension, assetType))
            throw new ExtensionAssetAccessNotSupportedException(GalleryResources.ExtensionAssetDirectAccessNotSupportedText());
          flag1 = true;
          bool flag2 = onPremDownload || GalleryUtil.IsVSTSOrTFSInstallationTargets((IEnumerable<InstallationTarget>) requestedAsset.Extension?.InstallationTargets) && this.TfsRequestContext.UserAgent != null && this.TfsRequestContext.UserAgent.Contains("TfsJobAgent.exe");
          bModifyVsixManifest = assetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage", StringComparison.OrdinalIgnoreCase) & flag2;
          response = this.CreateAssetResponse(requestedAsset.AssetFile, version, (AssetByNameController.GetPackageStreamDelegate) ((ExtensionFile file, out CompressionType compType) =>
          {
            ITeamFoundationFileService service2 = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
            Stream packageStream;
            if (bModifyVsixManifest)
            {
              using (Stream stream = service2.RetrieveFile(this.TfsRequestContext, (long) file.FileId, false, out byte[] _, out long _, out compType))
                packageStream = (Stream) service2.CopyStreamToTempFile(this.TfsRequestContext, stream, ref compType, false);
              this.UpdateVsixManifest(this.TfsRequestContext, ref packageStream, requestedAsset.Extension.Publisher.DisplayName, false);
            }
            else
              packageStream = service2.RetrieveFile(this.TfsRequestContext, (long) file.FileId, out compType);
            return packageStream;
          }));
        }
        else
        {
          response = this.Request.CreateResponse(HttpStatusCode.Found);
          HttpResponseHeaders headers = response.Headers;
          Uri uri = redirectionUrl;
          if ((object) uri == null)
            uri = service1.GetAssetUri(this.TfsRequestContext, requestedAsset, redirect);
          headers.Location = uri;
          this.AddCacheControlHeader(this.TfsRequestContext, response);
        }
        bool flag3 = true;
        if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.IncrementDownloadCountForSpecificAssetType"))
          flag3 = this.AssetTypeForStatIncrementSet.Contains(assetType);
        if (flag3)
        {
          if (install | update)
          {
            ExtensionFile assetFile = requestedAsset.AssetFile;
            if (assetFile != null)
            {
              version = assetFile.Version;
              targetPlatform = assetFile.TargetPlatform;
            }
            this.IncrementStatsAndPublishTelemetryForInstallOrUpdate(requestedAsset.Extension, version, update, targetPlatform);
          }
          if (onPremDownload)
          {
            try
            {
              this.TfsRequestContext.GetService<IExtensionStatisticService>().UpdateStatistics(this.TfsRequestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
              {
                new ExtensionStatisticUpdate()
                {
                  Statistic = new ExtensionStatistic()
                  {
                    StatisticName = "onpremDownloads",
                    Value = 1.0
                  },
                  Operation = ExtensionStatisticOperation.Increment,
                  PublisherName = publisherName,
                  ExtensionName = extensionName
                }
              });
              DailyStatsHelper.IncrementConnectedInstallCount(this.TfsRequestContext, requestedAsset.Extension);
            }
            catch (Exception ex)
            {
              this.TfsRequestContext.TraceException(12061061, nameof (AssetByNameController), "ApiController", ex);
            }
          }
        }
        if (flag1)
          this.TfsRequestContext.UpdateTimeToFirstPage();
      }
      if (redirect)
      {
        if (response.StatusCode == HttpStatusCode.Found)
          this.SendRedirectFlagCiEvent(this.TfsRequestContext, publisherName, extensionName, assetType, version, response.Headers.Location.AbsoluteUri);
        else
          this.SendRedirectFlagCiEvent(this.TfsRequestContext, publisherName, extensionName, assetType, version, "");
      }
      return response;
    }

    private void FetchRedirectionUrlAndForceRedirectForVstsIntegration(
      PublishedExtension extension,
      string version,
      string assetType,
      ref Uri redirectionUrl,
      ref bool forceRedirect)
    {
      string property = extension.GetProperty(version, assetType);
      if (property == null)
        return;
      redirectionUrl = new Uri(property);
      forceRedirect = true;
    }

    private bool IsRequestForVSTSIntegrationGetStarted(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ref PublishedExtension extension,
      string version)
    {
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      extension = service.QueryExtension(requestContext, publisherName, extensionName, version, ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly, (string) null, true);
      return extension != null && extension.InstallationTargets != null && GalleryUtil.IsVSTSOrTFSIntegrationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
    }

    private bool IsRequestForVsExtensionPayload(PublishedExtension extension, string assetType) => extension.IsVsExtension() && assetType.Equals("Microsoft.VisualStudio.Ide.Payload");

    protected HttpResponseMessage CreateAssetResponse(
      ExtensionFile assetFile,
      string version,
      AssetByNameController.GetPackageStreamDelegate getPackageStream)
    {
      CompressionType compressionType = CompressionType.None;
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Stream content = getPackageStream(assetFile, out compressionType);
      response.Content = (HttpContent) new StreamContent(content);
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      if (!string.IsNullOrEmpty(assetFile.Language))
        response.Content.Headers.ContentLanguage.Add(assetFile.Language);
      string assetType1 = assetFile.AssetType;
      string assetType2 = assetFile.AssetType;
      response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
      {
        FileName = assetType1,
        FileNameStar = assetType2
      };
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(assetFile.ContentType);
      if (!string.IsNullOrEmpty(version) && !string.Equals("latest", version, StringComparison.OrdinalIgnoreCase))
        this.AddCacheControlHeader(this.TfsRequestContext, response);
      response.Headers.Add("Cross-Origin-Resource-Policy", "cross-origin");
      return response;
    }

    protected void SendGetAssetDifferentCaseCiEvent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string assetType,
      string requestedAssetType)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("assettype", assetType);
      intelligenceData.Add("ItemId", publisherName + "." + extensionName);
      intelligenceData.Add(nameof (requestedAssetType), requestedAssetType);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "AsssetTypeCaseMismatch", properties);
    }

    protected void SendRedirectFlagCiEvent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string assetType,
      string assetVersion,
      string redirectURI)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("assettype", assetType);
      intelligenceData.Add("ItemId", publisherName + "." + extensionName);
      intelligenceData.Add(nameof (assetVersion), assetVersion);
      intelligenceData.Add(nameof (redirectURI), redirectURI);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "GetAssestRedirect", properties);
    }

    protected void SendVSCodeFallbackCiEvent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string assetType,
      string assetVersion,
      string targetPlatform = null)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("assettype", assetType);
      intelligenceData.Add("ItemId", publisherName + "." + extensionName);
      intelligenceData.Add(nameof (assetVersion), assetVersion);
      intelligenceData.Add("assetTargetPlatform", targetPlatform);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "VSCodeFallback", properties);
    }

    internal virtual void UpdateVsixManifest(
      IVssRequestContext requestContext,
      ref Stream packageStream,
      string publisherDisplayName,
      bool addMarketTag)
    {
      GalleryServerUtil.UpdateVsixManifest(requestContext, ref packageStream, publisherDisplayName, addMarketTag);
    }

    private void AddCacheControlHeader(
      IVssRequestContext requestContext,
      HttpResponseMessage response)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      double num = vssRequestContext.GetService<IVssRegistryService>().GetValue<double>(vssRequestContext, in AssetByNameController.s_customCacheDurationRegistryPath, 8760.0);
      if (num <= 0.0)
        return;
      response.Headers.CacheControl = new CacheControlHeaderValue()
      {
        Public = true,
        MaxAge = new TimeSpan?(TimeSpan.FromHours(num))
      };
    }

    private void IncrementStatsAndPublishTelemetryForInstallOrUpdate(
      PublishedExtension extension,
      string version,
      bool isUpdate = false,
      string targetPlatform = null)
    {
      string statisticType;
      string str;
      if (!isUpdate)
      {
        statisticType = "install";
        str = !extension.IsVsExtension() ? "VsCodeInstall" : "VsIdeInstall";
      }
      else
      {
        statisticType = "updateCount";
        str = "VsIdeUpdate";
      }
      try
      {
        CustomerIntelligenceService service1 = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("ItemId", extension.Publisher.PublisherName + "." + extension.ExtensionName);
        if (!string.IsNullOrEmpty(version))
          intelligenceData.Add(nameof (version), version);
        if (!string.IsNullOrEmpty(targetPlatform))
          intelligenceData.Add(nameof (targetPlatform), targetPlatform);
        string headerValue = GalleryServerUtil.GetHeaderValue(this.Request.Headers, "X-Market-Search-Activity-Id");
        if (!string.IsNullOrWhiteSpace(headerValue))
          intelligenceData.Add("SearchActivityId", headerValue);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string feature = str;
        CustomerIntelligenceData properties = intelligenceData;
        service1.Publish(tfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", feature, properties);
        IExtensionStatisticService service2 = this.TfsRequestContext.GetService<IExtensionStatisticService>();
        bool isBatchingEnabled = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableStatBatchLogging");
        bool flag = true;
        if (isUpdate)
        {
          if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCacheCheckForStatUpdatesFromAssetController"))
            flag = service2.ShouldUpdateStatCount(this.TfsRequestContext, extension.ExtensionId, version, "update", targetPlatform);
          if (!flag)
            return;
          service2.IncrementStatCount(this.TfsRequestContext, extension, statisticType);
        }
        else
        {
          if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableCacheCheckForStatUpdatesFromAssetController"))
            flag = service2.ShouldUpdateStatCount(this.TfsRequestContext, extension.ExtensionId, version, "install", targetPlatform);
          if (!flag)
            return;
          service2.IncrementStatCount(this.TfsRequestContext, extension, statisticType);
          DailyStatsHelper.IncrementInstallCount(this.TfsRequestContext, extension, isBatchingEnabled, version, targetPlatform);
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(10013475, nameof (AssetByNameController), "ApiController", ex);
      }
    }

    private void FetchRedirectionUrlAndForceRedirectForVsExtension(
      PublishedExtension extension,
      ref Uri redirectionUrl,
      ref bool forceRedirect)
    {
      string str = "";
      string uriString = "";
      foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
      {
        if (extensionMetadata.Key.Equals("DeploymentTechnology", StringComparison.OrdinalIgnoreCase))
          str = extensionMetadata.Value;
        if (extensionMetadata.Key.Equals("ReferralUrl", StringComparison.OrdinalIgnoreCase))
          uriString = extensionMetadata.Value;
      }
      switch (str)
      {
        case "Referral Link":
          redirectionUrl = new Uri(uriString);
          forceRedirect = true;
          break;
        case "EXE":
          break;
        case "MSI":
          break;
        default:
          int num = str == "VSIX" ? 1 : 0;
          break;
      }
    }

    protected delegate Stream GetPackageStreamDelegate(
      ExtensionFile file,
      out CompressionType compressionType);
  }
}
