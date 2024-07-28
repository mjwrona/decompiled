// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PlatformPublishedExtensionCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PlatformPublishedExtensionCache : IPublishedExtensionCache, IVssFrameworkService
  {
    private const string s_area = "Gallery";
    private const string s_layer = "PlatformPublishedExtensionCache";
    private ConcurrentDictionary<string, string> m_latestExtensionVersion = new ConcurrentDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void ServiceStart(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      notificationService.RegisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      service.UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
    }

    public Task<Stream> GetExtensionAsset(
      IVssRequestContext requestContext,
      ExtensionFile extensionFile)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return Task.FromResult<Stream>(vssRequestContext.GetService<ITeamFoundationFileService>().RetrieveFile(vssRequestContext, (long) extensionFile.FileId, false, out byte[] _, out long _, out CompressionType _));
    }

    public async Task<Stream> GetExtensionAsset(
      IVssRequestContext requestContext,
      string extensionFileUrl)
    {
      return requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.UseSharedAssetHandlers) ? (Stream) new AssetStream(await AssetHttpClientFactory.CreateClient(requestContext).GetAsset(extensionFileUrl).ConfigureAwait(false)) : await (await new AssetHttpClient(new Uri(extensionFileUrl), requestContext.ActivityId).GetAsset(extensionFileUrl).ConfigureAwait(false)).Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public string GetLatestVersion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string accountToken = null)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Client.UseExtensionCacheOnPrem"))
      {
        PublishedExtension publishedExtension = this.LoadExtension(requestContext, publisherName, extensionName, "latest", accountToken);
        if (publishedExtension == null)
          return (string) null;
        List<ExtensionVersion> versions = publishedExtension.Versions;
        if (versions == null)
          return (string) null;
        return versions.FirstOrDefault<ExtensionVersion>()?.Version;
      }
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      string latestVersion = (string) null;
      if (accountToken != null || !this.m_latestExtensionVersion.TryGetValue(fullyQualifiedName, out latestVersion))
      {
        PublishedExtension publishedExtension = this.GetPublishedExtension(requestContext, publisherName, extensionName, "latest", accountToken);
        if (publishedExtension.Versions != null && publishedExtension.Versions.Count > 0)
          latestVersion = publishedExtension.Versions[0].Version;
        if (accountToken == null)
          this.m_latestExtensionVersion[fullyQualifiedName] = latestVersion;
      }
      return latestVersion;
    }

    public PublishedExtension GetPublishedExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Client.UseExtensionCacheOnPrem"))
        return this.LoadExtension(requestContext, publisherName, extensionName, version, accountToken);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      PublishedExtension publishedExtension;
      if (string.IsNullOrEmpty(accountToken))
      {
        PublishedExtensionMemoryCache service = vssRequestContext.GetService<PublishedExtensionMemoryCache>();
        ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
        if (!service.TryGetValue(vssRequestContext, fullyQualifiedName, out concurrentDictionary))
        {
          concurrentDictionary = new ConcurrentDictionary<string, PublishedExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          service.Set(vssRequestContext, fullyQualifiedName, concurrentDictionary);
        }
        bool extensionFetched = false;
        publishedExtension = concurrentDictionary.GetOrAdd(version, (Func<string, PublishedExtension>) (extensionVersion =>
        {
          extensionFetched = true;
          return this.LoadExtension(requestContext, publisherName, extensionName, version, accountToken);
        }));
        if (extensionFetched && !string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase) && publishedExtension != null && publishedExtension.Versions != null)
          concurrentDictionary.TryAdd(publishedExtension.Versions[0].Version, publishedExtension);
      }
      else
        publishedExtension = this.LoadExtension(requestContext, publisherName, extensionName, version, accountToken);
      return publishedExtension;
    }

    private PublishedExtension LoadExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken = null)
    {
      try
      {
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri;
        string str = version;
        if (!string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
          extensionQueryFlags |= ExtensionQueryFlags.IncludeLatestVersionOnly;
          str = (string) null;
        }
        IVssRequestContext requestContext1 = requestContext;
        string publisherName1 = publisherName;
        string extensionName1 = extensionName;
        string version1 = str;
        int flags = (int) extensionQueryFlags;
        string accountToken1 = accountToken;
        return service.QueryExtension(requestContext1, publisherName1, extensionName1, version1, (ExtensionQueryFlags) flags, accountToken1);
      }
      catch (ExtensionDoesNotExistException ex)
      {
        return (PublishedExtension) null;
      }
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<PublishedExtensionMemoryCache>().Clear(vssRequestContext);
    }

    private void OnPublishedExtensionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(changeNotification.PublisherName, changeNotification.ExtensionName);
        requestContext.Trace(10013409, TraceLevel.Info, "Gallery", nameof (PlatformPublishedExtensionCache), "ExtPublishedExtensionService.OnPublishedExtensionChanged received notification Id: {0} EventType: {1}", (object) fullyQualifiedName, (object) changeNotification.EventType);
        if (changeNotification.EventType == ExtensionEventType.ExtensionShared)
          return;
        requestContext.Trace(10013410, TraceLevel.Info, "Gallery", nameof (PlatformPublishedExtensionCache), "Refreshing extension. Identifer: {0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
        ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
        if (!vssRequestContext.GetService<PublishedExtensionMemoryCache>().TryGetValue(vssRequestContext, fullyQualifiedName, out concurrentDictionary))
          return;
        concurrentDictionary.Clear();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013290, "Gallery", nameof (PlatformPublishedExtensionCache), ex);
      }
    }
  }
}
