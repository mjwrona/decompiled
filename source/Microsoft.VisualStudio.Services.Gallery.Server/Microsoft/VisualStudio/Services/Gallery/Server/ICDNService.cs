// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ICDNService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (CDNService))]
  internal interface ICDNService : IVssFrameworkService
  {
    bool UploadExtensionAssetWithStream(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      Stream stream,
      string assetName,
      string contentType,
      string encoding = "gzip");

    bool UploadPublisherAssetWithStream(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      Stream stream,
      string assetName,
      string contentType,
      string encoding = "gzip");

    string GetExtensionAssetCDNPath(ExtensionAsset extensionAsset);

    string GetPublisherAssetCDNPath(string publisherName, string assetFileName);

    bool DeleteExtensionAsset(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      string assetName);

    bool DeletePublisherAsset(
      IVssRequestContext requestContext,
      CDNPathUtil cdnAssetPath,
      string assetName);
  }
}
