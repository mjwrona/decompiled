// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PackageController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "package")]
  public class PackageController : AssetByNameController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public virtual HttpResponseMessage GetPackage(
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null,
      bool acceptDefault = true,
      [ClientIgnore] string targetPlatform = null)
    {
      IPublishedExtensionService service1 = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      bool isShared = false;
      string str1 = (string) null;
      bool flag = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseAccountTokenFromHeader");
      if (flag)
      {
        str1 = GalleryServerUtil.GetHeaderValue(this.Request?.Headers, "X-Market-AccountToken");
        if (!string.IsNullOrEmpty(str1))
          accountToken = str1;
      }
      this.TfsRequestContext.Trace(12062060, TraceLevel.Info, this.ActivityLogArea, "PackageController.GetPackage", string.Format("GetExtension: isUseAccountTokenFromHeaderFFEnabled:{0} IsNullOrEmpty(accountToken):{1} ", (object) flag, (object) string.IsNullOrEmpty(accountToken)) + string.Format("IsNullOrEmpty(accountTokenFromHeader):{0}", (object) string.IsNullOrEmpty(str1)));
      ExtensionQueryFlags queryFlags = ExtensionQueryFlags.AllAttributes | ExtensionQueryFlags.IncludeMetadata;
      PublishedExtension publishedExtension = this.QueryPublishedExtension(service1, publisherName, extensionName, queryFlags, ref isShared);
      if (publishedExtension == null)
        throw new ExtensionAssetNotFoundException("Resource not found");
      this.IncrementOnPremDownloadCount(publishedExtension);
      List<AssetInfo> assetTypes = new List<AssetInfo>();
      if (this.Request.Headers.AcceptLanguage != null)
      {
        foreach (StringWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.AcceptLanguage)
          assetTypes.Add(new AssetInfo("Microsoft.VisualStudio.Services.VSIXPackage", qualityHeaderValue.Value));
      }
      assetTypes.Add(new AssetInfo("Microsoft.VisualStudio.Services.VSIXPackage", (string) null));
      IPublisherAssetService service2 = this.TfsRequestContext.GetService<IPublisherAssetService>();
      HttpResponseMessage assetResponse = this.CreateAssetResponse(!isShared ? service2.QueryAsset(this.TfsRequestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, accountToken, (string) null, acceptDefault).AssetFile : service2.QueryAsset(this.TfsRequestContext.Elevate(), publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, accountToken, (string) null, acceptDefault).AssetFile, version, this.GetPackageStreamDelegateForPackage(publishedExtension));
      if (assetResponse.StatusCode == HttpStatusCode.OK)
      {
        string str2 = string.Format("{0}.{1}-{2}.vsix", (object) publisherName, (object) extensionName, (object) version);
        string str3 = str2;
        assetResponse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
        {
          FileName = str2,
          FileNameStar = str3
        };
        assetResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vsix");
        this.TfsRequestContext.UpdateTimeToFirstPage();
        bool isBatchingEnabled = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Publisher360.EnableStatBatchLogging");
        DailyStatsHelper.IncrementWebDownloadCount(this.TfsRequestContext, publishedExtension, isBatchingEnabled);
      }
      return assetResponse;
    }

    protected AssetByNameController.GetPackageStreamDelegate GetPackageStreamDelegateForPackage(
      PublishedExtension extension)
    {
      return (AssetByNameController.GetPackageStreamDelegate) ((ExtensionFile assetFile, out CompressionType compressionType) =>
      {
        using (Stream packageStream = this.GetPackageStream(extension, assetFile, out compressionType))
        {
          PooledMemoryStream delegateForPackage = new PooledMemoryStream((int) packageStream.Length + 20);
          using (Stream destination = (Stream) new GZipStream((Stream) delegateForPackage, CompressionMode.Compress, true))
            packageStream.CopyTo(destination);
          compressionType = CompressionType.GZip;
          delegateForPackage.Seek(0L, SeekOrigin.Begin);
          return (Stream) delegateForPackage;
        }
      });
    }

    protected PublishedExtension QueryPublishedExtension(
      IPublishedExtensionService publishedExtensionService,
      string publisherName,
      string extensionName,
      ExtensionQueryFlags queryFlags,
      ref bool isShared,
      string version = null)
    {
      PublishedExtension extension = (PublishedExtension) null;
      if (publishedExtensionService != null)
      {
        try
        {
          extension = publishedExtensionService.QueryExtension(this.TfsRequestContext, publisherName, extensionName, version, queryFlags, (string) null, true);
        }
        catch (AccessCheckException ex)
        {
          extension = publishedExtensionService.QueryExtension(this.TfsRequestContext.Elevate(), publisherName, extensionName, version, queryFlags, (string) null, true);
          isShared = this.IsSharedWithUser(extension);
          if (!isShared)
            throw;
        }
        catch (ExtensionDoesNotExistException ex)
        {
          return (PublishedExtension) null;
        }
      }
      return extension;
    }

    protected virtual void IncrementOnPremDownloadCount(PublishedExtension publishedExtension) => this.IncrementStatisticCount(this.TfsRequestContext.GetService<IExtensionStatisticService>(), publishedExtension, "onpremDownloads");

    protected void IncrementStatisticCount(
      IExtensionStatisticService statService,
      PublishedExtension publishedExtension,
      string statisticType)
    {
      if (publishedExtension == null)
        return;
      statService.UpdateStatistics(this.TfsRequestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
      {
        new ExtensionStatisticUpdate()
        {
          Statistic = new ExtensionStatistic()
          {
            StatisticName = statisticType,
            Value = 1.0
          },
          Operation = ExtensionStatisticOperation.Increment,
          PublisherName = publishedExtension.Publisher.PublisherName,
          ExtensionName = publishedExtension.ExtensionName
        }
      });
    }

    private bool IsSharedWithUser(PublishedExtension extension)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      return userIdentity != null && GalleryServerUtil.IsSharedWithUser(this.TfsRequestContext, extension, userIdentity.Id);
    }

    private Stream GetPackageStream(
      PublishedExtension extension,
      ExtensionFile assetFile,
      out CompressionType compressionType)
    {
      ITeamFoundationFileService service = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
      Stream tempFile;
      using (Stream stream = service.RetrieveFile(this.TfsRequestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out compressionType))
        tempFile = (Stream) service.CopyStreamToTempFile(this.TfsRequestContext, stream, ref compressionType, false);
      if (!extension.IsVsOrVsCodeOrVsForMacExtension())
        this.UpdateVsixManifest(this.TfsRequestContext, ref tempFile, extension.Publisher.DisplayName, true);
      tempFile.Flush();
      tempFile.Seek(0L, SeekOrigin.Begin);
      return tempFile;
    }
  }
}
