// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.ExtensionOverviewCacheService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class ExtensionOverviewCacheService : CustomExpiryCache
  {
    private const string c_DetailsAssetTypeName = "Microsoft.VisualStudio.Services.Content.Details";
    private const string OverviewCacheTimeout_RegistryPath = "/Configuration/Service/Gallery/OverviewCacheTimeout";

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.ExtensionChangeCallback), false);
      service.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.VsExtensionUpdateDelete, new SqlNotificationCallback(this.ExtensionChangeCallback), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.ExtensionChangeCallback), false);
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.VsExtensionUpdateDelete, new SqlNotificationCallback(this.ExtensionChangeCallback), false);
    }

    public string GetExtensionOverviewMarkdown(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string assetToken,
      out bool isCacheHit)
    {
      isCacheHit = true;
      string extensionOverviewMD = this.GetOverviewMDFromCache(requestContext, publisherName, extensionName, version);
      if (extensionOverviewMD == null)
      {
        isCacheHit = false;
        extensionOverviewMD = this.GetAssetContent(requestContext, publisherName, extensionName, version, "Microsoft.VisualStudio.Services.Content.Details", assetToken);
        if (extensionOverviewMD != null)
          this.SetExtensionOverviewInCache(requestContext, extensionOverviewMD, publisherName, extensionName);
      }
      return extensionOverviewMD;
    }

    private string GetAssetContent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string assetName,
      string assetToken)
    {
      string assetContent = "";
      IPublisherAssetService service1 = requestContext.GetService<IPublisherAssetService>();
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo(assetName)
      };
      try
      {
        ExtensionFile assetFile = service1.QueryAsset(requestContext, publisherName, extensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, assetToken, true).AssetFile;
        ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
        if (assetFile != null)
        {
          using (StreamReader streamReader = new StreamReader(service2.RetrieveFile(requestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _), true))
            assetContent = streamReader.ReadToEnd();
        }
      }
      catch (ExtensionAssetNotFoundException ex)
      {
        assetContent = "";
      }
      return assetContent;
    }

    private string GetOverviewMDFromCache(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string extVersion)
    {
      string overviewMdFromCache = (string) null;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      this.TryGetValue<string>(requestContext, fullyQualifiedName, out overviewMdFromCache);
      return overviewMdFromCache;
    }

    private void SetExtensionOverviewInCache(
      IVssRequestContext requestContext,
      string extensionOverviewMD,
      string publisherName,
      string extensionName)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      VssCacheExpiryProvider<string, object> expiryProvider = this.OneDayExpiryProvider;
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) "/Configuration/Service/Gallery/OverviewCacheTimeout";
      int num = service.GetValue<int>(requestContext1, in local, 0);
      if (num > 0)
        expiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes((double) num)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.Set(fullyQualifiedName, (object) extensionOverviewMD, expiryProvider);
    }

    private void ExtensionChangeCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableItemDetailsPageOverviewCache"))
        return;
      if (!string.IsNullOrEmpty(eventData))
      {
        string[] source = eventData.Split((char[]) null);
        if (source.Length > 1 && !((IEnumerable<string>) source).Last<string>().IsNullOrEmpty<char>())
          this.ClearExtensionFromCache(requestContext, ((IEnumerable<string>) source).Last<string>());
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "extensionOverviewCacheRefresh");
      properties.Add(nameof (eventData), eventData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ExtensionOverviewCacheFeature", properties);
    }

    private void ClearExtensionFromCache(IVssRequestContext requestContext, string extensionName) => this.Remove(requestContext, extensionName);
  }
}
