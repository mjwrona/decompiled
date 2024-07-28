// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSExtensionScanScheduler
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal class CVSExtensionScanScheduler
  {
    private Stream m_packageStream;
    private const string s_layer = "CVSExtensionScanScheduler";

    public PublishedExtension Extension { get; private set; }

    public Guid ScanId { get; private set; }

    public IEnumerable<ScanItem> ScanItems { get; private set; }

    public IdentityPuid PublisherPuid { get; private set; }

    public CVSExtensionScanScheduler(
      PublishedExtension extension,
      Guid scanId,
      IdentityPuid publisherPuid = null,
      Stream packageStream = null)
    {
      this.Extension = extension;
      this.ScanId = scanId == Guid.Empty ? Guid.NewGuid() : scanId;
      this.m_packageStream = packageStream;
      this.PublisherPuid = publisherPuid;
    }

    public IEnumerable<ScanItem> ScheduleScan(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSExtensionScanScheduler), "SubmitExtensionForScan");
      this.ScanItems = (IEnumerable<ScanItem>) null;
      if (this.Extension?.Versions?.Count.GetValueOrDefault() == 1)
      {
        try
        {
          if (!this.Extension.IsVsExtension())
          {
            bool flag = false;
            if (this.m_packageStream == null)
            {
              flag = true;
              this.m_packageStream = this.GetPackageFileStream(requestContext, this.Extension);
            }
            if (this.m_packageStream != null)
            {
              try
              {
                DateTime lastUpdated = this.Extension.Versions[0].LastUpdated;
                this.ScanItems = this.SubmitPackageForScan(requestContext, this.ScanId, this.m_packageStream, new DateTime?(lastUpdated));
              }
              finally
              {
                if (flag)
                  this.m_packageStream.Dispose();
              }
            }
            else
              requestContext.TraceAlways(12061123, TraceLevel.Error, "gallery", nameof (CVSExtensionScanScheduler), "PackageStream Not found for extension id: {0}", (object) this.Extension.ExtensionId);
          }
          else
            this.ScanItems = this.SubmitVSExtensionForScan(requestContext, this.ScanId, this.Extension);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061123, "gallery", nameof (CVSExtensionScanScheduler), ex);
          requestContext.GetService<ICVSService>().DeleteScanItemsByScanId(requestContext, this.ScanId);
          throw;
        }
      }
      else
        requestContext.TraceAlways(12061123, TraceLevel.Error, "gallery", nameof (CVSExtensionScanScheduler), "Didnt find proper extension object to submit for scan");
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSExtensionScanScheduler), "SubmitExtensionForScan");
      return this.ScanItems;
    }

    private IEnumerable<ScanItem> SubmitPackageForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Stream packageStream,
      DateTime? updateTime)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSExtensionScanScheduler), nameof (SubmitPackageForScan));
      ConcurrentBag<ScanItem> cvsScanItems = new ConcurrentBag<ScanItem>();
      if (packageStream != null)
      {
        IVssDeploymentServiceHost deploymentHost = requestContext.ServiceHost.DeploymentServiceHost;
        bool isServicingContext = requestContext.IsServicingContext;
        VSIXPackage.Parse(packageStream, (IImageResizeUtility) new DefaultImageResizeUtility((Func<Exception, bool>) null, false), requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MultithreadExtensionsAssetUpload"), (Func<ManifestFile, Stream, bool>) ((manifestFile, stream) =>
        {
          using (IVssRequestContext vssRequestContext = isServicingContext ? deploymentHost.CreateServicingContext() : deploymentHost.CreateSystemContext())
          {
            if (stream != null)
            {
              if (manifestFile != null)
              {
                ICVSService service = vssRequestContext.GetService<ICVSService>();
                ContentType contentType = this.GetContentType(manifestFile.ContentType);
                if (contentType != null)
                {
                  stream.Seek(0L, SeekOrigin.Begin);
                  if (stream.Length > 0L)
                  {
                    ScanItem scanItem = service.SubmitStreamForScan(vssRequestContext, scanId, stream, manifestFile.AssetType, contentType, this.GetExtensionIdVersionString(this.Extension), this.PublisherPuid, updateTime, false);
                    if (scanItem != null)
                      cvsScanItems.Add(scanItem);
                  }
                }
              }
            }
          }
          return false;
        }));
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSExtensionScanScheduler), nameof (SubmitPackageForScan));
      return (IEnumerable<ScanItem>) cvsScanItems;
    }

    private IEnumerable<ScanItem> SubmitVSExtensionForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      PublishedExtension extension)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSExtensionScanScheduler), nameof (SubmitVSExtensionForScan));
      List<ScanItem> scanItemList = new List<ScanItem>();
      if (extension != null && extension.Versions != null && extension.Versions.Count == 1)
      {
        ICVSService service1 = requestContext.GetService<ICVSService>();
        DateTime lastUpdated = extension.Versions[0].LastUpdated;
        if (extension.Versions[0].Files != null && extension.Versions[0].Files.Count > 0)
        {
          ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
          foreach (ExtensionFile file in extension.Versions[0].Files)
          {
            ContentType contentType = this.GetContentType(file.ContentType);
            if (contentType != null)
            {
              using (Stream stream = service2.RetrieveFile(requestContext, (long) file.FileId, out CompressionType _))
              {
                ScanItem scanItem = service1.SubmitStreamForScan(requestContext, scanId, stream, file.AssetType, contentType, this.GetExtensionIdVersionString(extension), this.PublisherPuid, new DateTime?(lastUpdated), false);
                if (scanItem != null)
                  scanItemList.Add(scanItem);
              }
            }
          }
        }
        using (Stream extensionMetadataStream = this.GetVSExtensionMetadataStream(extension))
        {
          if (extensionMetadataStream != null)
          {
            ScanItem scanItem = service1.SubmitStreamForScan(requestContext, scanId, extensionMetadataStream, GalleryResources.CVSMetadataStreamDescription(), (ContentType) 1, this.GetExtensionIdVersionString(extension), this.PublisherPuid, new DateTime?(lastUpdated), false);
            if (scanItem != null)
              scanItemList.Add(scanItem);
          }
          else
            requestContext.TraceAlways(12061123, TraceLevel.Error, "gallery", nameof (CVSExtensionScanScheduler), "Metadata Stream Not found for extension: {0}", (object) extension.ExtensionId);
        }
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSExtensionScanScheduler), nameof (SubmitVSExtensionForScan));
      return (IEnumerable<ScanItem>) scanItemList;
    }

    private Stream GetVSExtensionMetadataStream(PublishedExtension extension)
    {
      Stream extensionMetadataStream = (Stream) null;
      if (extension != null)
      {
        List<string> values = new List<string>();
        values.Add(extension.DisplayName);
        values.Add(extension.ExtensionName);
        values.Add(extension.LongDescription);
        values.Add(extension.ShortDescription);
        if (!extension.Tags.IsNullOrEmpty<string>())
          values.AddRange((IEnumerable<string>) extension.Tags);
        extensionMetadataStream = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, (IEnumerable<string>) values)));
      }
      return extensionMetadataStream;
    }

    private ContentType GetContentType(string contentType)
    {
      ContentType contentType1 = (ContentType) 0;
      if (contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        contentType1 = (ContentType) 3;
      else if (contentType.StartsWith("text", StringComparison.OrdinalIgnoreCase))
        contentType1 = (ContentType) 1;
      else if (contentType != null)
      {
        switch (contentType.Length)
        {
          case 10:
            if (contentType == "plain/text")
              break;
            goto label_15;
          case 15:
            if (!(contentType == "application/xml"))
              goto label_15;
            else
              break;
          case 16:
            if (contentType == "application/json")
              break;
            goto label_15;
          case 20:
            if (contentType == "application/xaml+xml")
              break;
            goto label_15;
          case 21:
            if (contentType == "application/xhtml+xml")
              break;
            goto label_15;
          case 22:
            switch (contentType[12])
            {
              case 'j':
                if (contentType == "application/javascript")
                  break;
                goto label_15;
              case 't':
                if (contentType == "application/typescript")
                  break;
                goto label_15;
              default:
                goto label_15;
            }
            break;
          default:
            goto label_15;
        }
        contentType1 = (ContentType) 1;
      }
label_15:
      return contentType1;
    }

    private Stream GetPackageFileStream(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      requestContext.TraceEnter(12061123, "Gallery", nameof (CVSExtensionScanScheduler), "GePackageFileStream");
      Stream packageFileStream = (Stream) null;
      if (extension != null && !extension.IsVsExtension())
      {
        List<ExtensionVersion> versions = extension.Versions;
        ExtensionVersion extensionVersion = versions != null ? versions.FirstOrDefault<ExtensionVersion>() : (ExtensionVersion) null;
        string version = extensionVersion.Version;
        if (extensionVersion.Files != null)
        {
          foreach (ExtensionFile file in extensionVersion.Files)
          {
            if (file.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage"))
            {
              packageFileStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) file.FileId, out CompressionType _);
              break;
            }
          }
        }
      }
      requestContext.TraceLeave(12061123, "Gallery", nameof (CVSExtensionScanScheduler), "GePackageFileStream");
      return packageFileStream;
    }

    private string GetExtensionIdVersionString(PublishedExtension extension)
    {
      string extensionIdVersionString = string.Empty;
      ExtensionVersion extensionVersion;
      if (extension == null)
      {
        extensionVersion = (ExtensionVersion) null;
      }
      else
      {
        List<ExtensionVersion> versions = extension.Versions;
        extensionVersion = versions != null ? versions.FirstOrDefault<ExtensionVersion>() : (ExtensionVersion) null;
      }
      if (extensionVersion != null)
        extensionIdVersionString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) extension.ExtensionId.ToString(), (object) extension.Versions[0].Version);
      return extensionIdVersionString;
    }
  }
}
