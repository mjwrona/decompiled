// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CDNService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class CDNService : ICDNService, IVssFrameworkService
  {
    private const string s_layer = "ApiController";
    private const string c_blobStorageConnectionStringOverrideKey = "BlobStorageConnectionStringOverride";
    private const string c_blobStoreProviderType = "Microsoft.TeamFoundation.Framework.Hosting.AzureProvider";
    private const string c_cacheControl = "public, max-age=31536000";
    private GalleryBlobProvider m_cdnBlobProvider;
    private string m_cdnContainerName;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.ConfigureAssetCDNBlobProvider(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment || this.m_cdnBlobProvider == null)
        return;
      this.m_cdnBlobProvider.Unload(systemRequestContext);
    }

    public bool UploadExtensionAssetWithStream(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      Stream stream,
      string assetName,
      string contentType,
      string encoding = "gzip")
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assetName, nameof (assetName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(contentType, nameof (contentType));
      ArgumentUtility.CheckForNull<CDNPathUtil>(cdnAssetPath, nameof (cdnAssetPath));
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      string str = cdnAssetPath.GetExtensionAssetUploadPath() + "/" + assetName;
      return this.UploadAssetWithStream(requestContext, stream, str, str, contentType, encoding);
    }

    public bool UploadPublisherAssetWithStream(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      Stream stream,
      string assetName,
      string contentType,
      string encoding = "gzip")
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assetName, nameof (assetName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(contentType, nameof (contentType));
      ArgumentUtility.CheckForNull<CDNPathUtil>(cdnAssetPath, nameof (cdnAssetPath));
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      string str = cdnAssetPath.GetPublisherAssetUploadPath() + "/" + assetName;
      return this.UploadAssetWithStream(requestContext, stream, str, str, contentType, encoding);
    }

    private bool UploadAssetWithStream(
      IVssRequestContext requestContext,
      Stream stream,
      string assetFullPath,
      string assetName,
      string contentType,
      string encoding)
    {
      bool flag = false;
      if (this.m_cdnBlobProvider != null && this.m_cdnBlobProvider.IsValidBlobProvider())
      {
        if (assetName.EndsWith("/", StringComparison.OrdinalIgnoreCase))
          assetName = assetName.Substring(0, assetName.Length - 1);
        string fileName = Path.GetFileName(assetName);
        string headerValueForAsset = GalleryServerUtil.GetContentDispositionHeaderValueForAsset(requestContext, assetName);
        string str;
        if (headerValueForAsset == null)
          str = headerValueForAsset;
        else
          str = new ContentDispositionHeaderValue(headerValueForAsset)
          {
            FileName = fileName,
            FileNameStar = fileName
          }.ToString();
        string contentDisposition = str;
        requestContext.TraceEnter(12061091, "Gallery", "ApiController", nameof (UploadAssetWithStream));
        this.m_cdnBlobProvider.PutStream(requestContext, this.m_cdnContainerName, assetFullPath, stream, (IDictionary<string, string>) null);
        this.m_cdnBlobProvider.SetBlobHeaders(requestContext, this.m_cdnContainerName, assetFullPath, "public, max-age=31536000", contentType, contentDisposition, encoding, (string) null);
        flag = true;
        requestContext.TraceLeave(12061091, "Gallery", "ApiController", nameof (UploadAssetWithStream));
      }
      return flag;
    }

    public string GetExtensionAssetCDNPath(ExtensionAsset extensionAsset)
    {
      string extensionAssetCdnPath = "";
      if (extensionAsset != null)
      {
        extensionAssetCdnPath = this.m_cdnContainerName + "/" + new CDNPathUtil()
        {
          AssetRoot = extensionAsset.Extension.Versions[0].CdnDirectory,
          ExtensionName = extensionAsset.Extension.ExtensionName,
          ExtensionVersion = extensionAsset.Extension.Versions[0].Version,
          PublisherName = extensionAsset.Extension.Publisher.PublisherName
        }.GetExtensionAssetUploadPath();
        if (!extensionAsset.AssetFile.AssetType.IsNullOrEmpty<char>())
          extensionAssetCdnPath = extensionAssetCdnPath + "/" + extensionAsset.AssetFile.AssetType;
      }
      return extensionAssetCdnPath;
    }

    public string GetPublisherAssetCDNPath(string publisherName, string assetFileName)
    {
      string publisherAssetCdnPath = "";
      if (publisherName != null && assetFileName != null)
        publisherAssetCdnPath = this.m_cdnContainerName + "/" + new CDNPathUtil()
        {
          PublisherName = publisherName
        }.GetPublisherAssetUploadPath() + "/" + assetFileName;
      return publisherAssetCdnPath;
    }

    public bool DeleteExtensionAsset(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      string assetName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assetName, nameof (assetName));
      ArgumentUtility.CheckForNull<CDNPathUtil>(cdnAssetPath, nameof (cdnAssetPath));
      string resourceId = cdnAssetPath.GetExtensionAssetUploadPath() + "/" + assetName;
      return this.m_cdnBlobProvider != null && this.m_cdnBlobProvider.IsValidBlobProvider() && this.m_cdnBlobProvider.DeleteBlob(requestContext, this.m_cdnContainerName, resourceId);
    }

    public bool DeletePublisherAsset(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      string assetName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assetName, nameof (assetName));
      ArgumentUtility.CheckForNull<CDNPathUtil>(cdnAssetPath, nameof (cdnAssetPath));
      string resourceId = cdnAssetPath.GetPublisherAssetUploadPath() + "/" + assetName;
      return this.m_cdnBlobProvider != null && this.m_cdnBlobProvider.IsValidBlobProvider() && this.m_cdnBlobProvider.DeleteBlob(requestContext, this.m_cdnContainerName, resourceId);
    }

    private void ConfigureAssetCDNBlobProvider(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061054, "Gallery", "ApiController", "ConfigureAssetCDNBlobPrvider");
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      try
      {
        this.m_cdnContainerName = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/CDN/DefaultContainer", "extensions");
        this.m_cdnBlobProvider = new GalleryBlobProvider(requestContext, "ConfigurationSecrets", "GalleryAssetCDNStorageConnectionString");
      }
      catch (Exception ex)
      {
        requestContext.Trace(12061054, TraceLevel.Error, "gallery", "ConfigureAssetCDNBlobPrvider", string.Format("Exception: {0}", (object) ex.Message));
        throw;
      }
    }
  }
}
