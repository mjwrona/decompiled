// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionAssetUploadService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ExtensionAssetUploadService : 
    IExtensionAssetUploadService,
    IPostUploadExtensionProcessorService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We handle the exception to gracefully terminate the job with proper")]
    public bool ProcessExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream,
      Guid validationId,
      Guid vsid,
      out string resultMessage)
    {
      requestContext.TraceEnter(12061093, "Gallery", "UploadExtensionAssetsService", nameof (ProcessExtension));
      bool flag = true;
      resultMessage = string.Empty;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UploadAssetsAsync"))
      {
        if (!extension.IsVsExtension())
        {
          try
          {
            this.UploadExtensionAssets(requestContext, extension, packageStream, validationId);
          }
          catch (Exception ex)
          {
            flag = false;
            resultMessage = ex.ToString();
            requestContext.TraceException(12061093, "Gallery", nameof (ProcessExtension), ex);
          }
        }
      }
      requestContext.TraceLeave(12061093, "Gallery", "UploadExtensionAssetsService", nameof (ProcessExtension));
      return flag;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is comming for tracepoints so ignoring")]
    [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Warning for string in trace")]
    private void UploadExtensionAssets(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream,
      Guid validationId)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
        throw new ExtensionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found"));
      if (packageStream == null)
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to find vsix for extension: {0}.{1} version {2} validation id:{3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions[0].Version, (object) validationId));
      requestContext.Trace(12061093, TraceLevel.Info, "Gallery", "ProcessExtension", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Uploading Assets for extension: {0}.{1} version {2} validation id:{3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions[0].Version, (object) validationId));
      HashSet<string> existingAssets = new HashSet<string>();
      extension.Versions[0].Files.ForEach((Action<ExtensionFile>) (x => existingAssets.Add(x.AssetType)));
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      PackageDetails packageDetails1 = VSIXPackage.Parse(packageStream);
      packageDetails1.Manifest.AssetCDNRoot = extension.Versions[0].CdnDirectory;
      IVssRequestContext requestContext1 = requestContext;
      Stream extensionPackageStream = packageStream;
      PackageDetails packageDetails2 = packageDetails1;
      HashSet<string> ExcludeAssetTypes = existingAssets;
      IEnumerable<ExtensionFile> assets = (IEnumerable<ExtensionFile>) service.UploadAssets(requestContext1, extensionPackageStream, packageDetails2, ExcludeAssetTypes: ExcludeAssetTypes).Manifest.Assets;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
      {
        try
        {
          component.AddAssetsForExtensionVersion(extension, validationId, assets);
        }
        catch (NotImplementedException ex)
        {
          requestContext.Trace(12061093, TraceLevel.Info, "Gallery", "ProcessExtension", "extensionComponent.AddAssetsForExtensionVersion not implemented. Updating DB with extensionComponent11.AddAssetForExtensionVersion");
          foreach (ExtensionFile extensionFile in assets)
            component.AddAssetForExtensionVersion(extension.ExtensionId, extension.Versions[0].Version, extensionFile.AssetType, extensionFile.ContentType, extensionFile.FileId, extensionFile.ShortDescription);
        }
      }
    }
  }
}
