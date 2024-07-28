// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IPublisherAssetService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (PublisherAssetService))]
  internal interface IPublisherAssetService : IVssFrameworkService
  {
    PublisherAssetConfiguration GetConfiguration(IVssRequestContext requestContext);

    bool DirectAssetRequestSupported(
      IVssRequestContext requestContext,
      ExtensionAsset assetQuery,
      bool redirect = false);

    bool DirectPackageRequestSupported(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    void GetPublisherFromToken(
      IVssRequestContext requestContext,
      string assetToken,
      out string publisherName,
      out string extensionName);

    string GetTokenForPublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    PackageDetails UploadAssets(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      PackageDetails packageDetails,
      HashSet<string> IncludeAssetTypes = null,
      HashSet<string> ExcludeAssetTypes = null);

    ExtensionAsset QueryAsset(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      Guid validationId,
      IEnumerable<AssetInfo> assetTypes,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      string targetPlatform = null);

    ExtensionAsset QueryAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      Guid validationId,
      IEnumerable<AssetInfo> assetTypes,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      string targetPlatform = null);

    Uri GetAssetUri(
      IVssRequestContext requestContext,
      ExtensionAsset extensionAsset,
      bool cdnSupported = false);

    Uri GetAssetUri(IVssRequestContext requestContext, string baseUri, string assetType);

    void DeleteExtensionAssets(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version = null);

    int UploadVsixSignatureToFileService(
      IVssRequestContext requestContext,
      Stream signatureAssetStream);

    bool UploadVsixSignatureToBlobStorage(
      IVssRequestContext requestContext,
      Stream signatureAssetStream,
      PublishedExtension publishedExtension,
      string cdnRootDirectory,
      string assetType);
  }
}
