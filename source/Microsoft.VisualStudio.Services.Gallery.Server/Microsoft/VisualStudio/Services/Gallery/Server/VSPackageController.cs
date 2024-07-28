// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.VSPackageController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "vspackage")]
  public class VSPackageController : PackageController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientIgnore]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenFromHeader", "Header to pass the account token", true, true)]
    public override HttpResponseMessage GetPackage(
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null,
      bool acceptDefault = true,
      [ClientIgnore] string targetPlatform = null)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      ExtensionQueryFlags queryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeMetadata;
      bool isShared = false;
      accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "VSPackageController.GetPackage", accountToken);
      PublishedExtension publishedExtension = this.QueryPublishedExtension(service, publisherName, extensionName, queryFlags, ref isShared, version);
      bool flag1 = publishedExtension != null && publishedExtension.IsVsCodeExtension();
      bool flag2 = publishedExtension != null && publishedExtension.IsVsExtension();
      bool flag3 = publishedExtension != null && publishedExtension.IsVsForMacExtension();
      if (publishedExtension == null || !(flag2 | flag1 | flag3))
        throw new ExtensionAssetNotFoundException("Resource not found");
      return this.ServePackage(publishedExtension, version, publisherName, extensionName, accountToken, acceptDefault, targetPlatform);
    }

    private HttpResponseMessage ServePackage(
      PublishedExtension publishedExtension,
      string version,
      string publisherName,
      string extensionName,
      string accountToken,
      bool acceptDefault,
      string targetPlatform = null)
    {
      IExtensionStatisticService service = this.TfsRequestContext.GetService<IExtensionStatisticService>();
      bool isBatchingEnabled = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableStatBatchLogging");
      if (publishedExtension.IsVsExtension())
      {
        HttpResponseMessage httpResponseMessage = this.ServeVsPackage(publishedExtension, version, targetPlatform);
        bool flag = service.ShouldUpdateStatCount(this.TfsRequestContext, publishedExtension.ExtensionId, version, "install", targetPlatform);
        if (((httpResponseMessage.StatusCode == HttpStatusCode.Found ? 1 : (httpResponseMessage.StatusCode == HttpStatusCode.OK ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        {
          this.IncrementStatisticCount(service, publishedExtension, "install");
          DailyStatsHelper.IncrementWebDownloadCount(this.TfsRequestContext, publishedExtension, isBatchingEnabled, version, targetPlatform);
        }
        return httpResponseMessage;
      }
      List<AssetInfo> assetTypes = new List<AssetInfo>();
      if (this.Request.Headers.AcceptLanguage != null)
      {
        foreach (StringWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.AcceptLanguage)
          assetTypes.Add(new AssetInfo("Microsoft.VisualStudio.Services.VSIXPackage", qualityHeaderValue.Value));
      }
      assetTypes.Add(new AssetInfo("Microsoft.VisualStudio.Services.VSIXPackage", (string) null));
      ExtensionFile assetFile = this.TfsRequestContext.GetService<IPublisherAssetService>().QueryAsset(this.TfsRequestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, accountToken, (string) null, acceptDefault, targetPlatform).AssetFile;
      HttpResponseMessage assetResponse = this.CreateAssetResponse(assetFile, version, this.GetPackageStreamDelegateForPackage(publishedExtension));
      if (assetResponse.StatusCode == HttpStatusCode.OK)
      {
        string empty = string.Empty;
        string str1;
        if (string.IsNullOrEmpty(assetFile.TargetPlatform))
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}-{2}.vsix", (object) publisherName, (object) extensionName, (object) version);
        else
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}-{2}@{3}.vsix", (object) publisherName, (object) extensionName, (object) version, (object) assetFile.TargetPlatform);
        string str2 = str1;
        assetResponse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
        {
          FileName = str1,
          FileNameStar = str2
        };
        assetResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vsix");
        this.TfsRequestContext.UpdateTimeToFirstPage();
        if (service.ShouldUpdateStatCount(this.TfsRequestContext, publishedExtension.ExtensionId, version, "download", targetPlatform))
        {
          this.IncrementStatisticCount(service, publishedExtension, "downloadCount");
          DailyStatsHelper.IncrementWebDownloadCount(this.TfsRequestContext, publishedExtension, isBatchingEnabled, version, targetPlatform);
        }
      }
      return assetResponse;
    }

    private HttpResponseMessage ServeVsPackage(
      PublishedExtension publishedExtension,
      string version,
      string targetPlatform)
    {
      string uriString = this.GetRedirectionURLString();
      if (publishedExtension.Metadata == null)
        return this.Request.CreateResponse(HttpStatusCode.InternalServerError);
      if (!this.IsReferralLink(publishedExtension.Metadata))
      {
        int index1 = 0;
        if (!string.IsNullOrWhiteSpace(targetPlatform))
        {
          for (int index2 = 0; index2 < publishedExtension.Versions.Count; ++index2)
          {
            if (string.Equals(targetPlatform, publishedExtension.Versions[index2].TargetPlatform, StringComparison.OrdinalIgnoreCase))
            {
              index1 = index2;
              break;
            }
          }
        }
        List<ExtensionFile> files = publishedExtension.Versions[index1].Files;
        int num = 0;
        foreach (ExtensionFile extensionFile in files)
        {
          if (extensionFile.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload"))
            num = extensionFile.FileId;
        }
        foreach (ExtensionFile assetFile in files)
        {
          if (!assetFile.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload") && assetFile.FileId == num)
          {
            if (this.TfsRequestContext.GetService<IPublisherAssetService>().DirectPackageRequestSupported(this.TfsRequestContext, publishedExtension))
            {
              HttpResponseMessage assetResponse = this.CreateAssetResponse(assetFile, version, this.GetPackageStreamDelegateForPackage(publishedExtension));
              if (assetResponse.StatusCode == HttpStatusCode.OK)
              {
                string nameFromAssetType;
                string str = nameFromAssetType = this.GetPackageFileNameFromAssetType(assetFile.AssetType);
                assetResponse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                  FileName = nameFromAssetType,
                  FileNameStar = str
                };
                assetResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(assetFile.ContentType);
                this.TfsRequestContext.UpdateTimeToFirstPage();
              }
              return assetResponse;
            }
            uriString = assetFile.Source;
          }
        }
      }
      else
      {
        foreach (ExtensionMetadata extensionMetadata in publishedExtension.Metadata)
        {
          if (extensionMetadata.Key.Equals("ReferralUrl"))
            uriString = extensionMetadata.Value;
        }
      }
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Found);
      response.Headers.Location = new Uri(uriString);
      return response;
    }

    private string GetPackageFileNameFromAssetType(string assetType)
    {
      if (assetType == null)
        return string.Empty;
      int num = assetType.LastIndexOf('/');
      return num < 0 || num == assetType.Length - 1 ? assetType : assetType.Substring(num + 1);
    }

    private string GetRedirectionURLString()
    {
      IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
      string redirectionUrlString = (string) null;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) "/Configuration/Service/Gallery/VSGallery/HostedUrl";
      IEnumerable<RegistryItem> registryItems = service.Read(tfsRequestContext, in local);
      if (registryItems != null)
      {
        foreach (RegistryItem registryItem in registryItems)
        {
          if (registryItem.Path.Equals("/Configuration/Service/Gallery/VSGallery/HostedUrl"))
            redirectionUrlString = registryItem.Value;
        }
      }
      return redirectionUrlString;
    }

    private bool IsReferralLink(List<ExtensionMetadata> metadatas)
    {
      bool flag = false;
      foreach (ExtensionMetadata metadata in metadatas)
      {
        if (metadata.Key.Equals("DeploymentTechnology") && metadata.Value.Equals("Referral Link"))
          flag = true;
      }
      return flag;
    }
  }
}
