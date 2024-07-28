// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.PublishedExtensionCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  internal class PublishedExtensionCache : 
    VssBaseService,
    IPublishedExtensionCache,
    IVssFrameworkService
  {
    private const int c_defaultRefreshTaskInSeconds = 60;
    private static readonly string s_area = "Gallery";
    private static readonly string s_layer = nameof (PublishedExtensionCache);
    private ConcurrentDictionary<string, string> m_latestExtensionVersion = new ConcurrentDictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private HashSet<string> m_extensionRefreshNeeded = new HashSet<string>();
    private ILockName m_extensionRefreshLockName;
    private TimeSpan m_refreshTaskDelay;
    private INotificationRegistration m_extensionChangedRegistration;
    private INotificationRegistration m_cacheRefreshRegistration;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.LoadConfiguration(requestContext);
      this.m_extensionRefreshLockName = this.CreateLockName(requestContext, "extensionRefresh");
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), GallerySdkRegistryConstants.GallerySettingsRoot + "/...");
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_extensionChangedRegistration = service.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false, false);
      this.m_cacheRefreshRegistration = service.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false, false);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_extensionChangedRegistration.Unregister(requestContext);
      this.m_cacheRefreshRegistration.Unregister(requestContext);
      requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void LoadConfiguration(IVssRequestContext requestContext) => this.m_refreshTaskDelay = TimeSpan.FromSeconds((double) requestContext.GetService<CachedRegistryService>().GetValue<uint>(requestContext, (RegistryQuery) GallerySdkRegistryConstants.ExtensionRefreshDelay, true, 60U));

    public async Task<Stream> GetExtensionAsset(
      IVssRequestContext requestContext,
      ExtensionFile extensionFile)
    {
      return await this.GetExtensionAsset(requestContext, extensionFile.Source).ConfigureAwait(false);
    }

    public async Task<Stream> GetExtensionAsset(
      IVssRequestContext requestContext,
      string extensionFileUrl)
    {
      if (requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.UseSharedAssetHandlers))
      {
        requestContext.Trace(100136256, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Fetching asset with shared handlers: {0}", (object) extensionFileUrl);
        return (Stream) new AssetStream(await AssetHttpClientFactory.CreateClient(requestContext).GetAsset(extensionFileUrl).ConfigureAwait(false));
      }
      requestContext.Trace(100136257, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Fetching asset without shared handlers: {0}", (object) extensionFileUrl);
      return await (await new AssetHttpClient(new Uri(extensionFileUrl), requestContext.ActivityId).GetAsset(extensionFileUrl).ConfigureAwait(false)).Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public string GetLatestVersion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string accountToken = null)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      string latestVersion = (string) null;
      if (accountToken != null || !this.m_latestExtensionVersion.TryGetValue(fullyQualifiedName, out latestVersion))
      {
        PublishedExtension publishedExtension = this.GetPublishedExtension(requestContext, publisherName, extensionName, (string) null, accountToken);
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
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      PublishedExtension publishedExtension1;
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
        publishedExtension1 = concurrentDictionary.GetOrAdd(version, (Func<string, PublishedExtension>) (extensionVersion =>
        {
          extensionFetched = true;
          PublishedExtension publishedExtension2 = this.LoadExtension(requestContext, publisherName, extensionName, version, accountToken);
          if (publishedExtension2 == null || publishedExtension2.Versions == null)
            this.QueueRefreshCheck(requestContext, publisherName, extensionName, version);
          return publishedExtension2;
        }));
        if (extensionFetched && !string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase) && publishedExtension1 != null && publishedExtension1.Versions != null)
        {
          concurrentDictionary.TryAdd(publishedExtension1.Versions[0].Version, publishedExtension1);
          if (publishedExtension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) || requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.PersistPrivateExtension))
            this.StorePublishedExtensionObject(requestContext, publisherName, extensionName, publishedExtension1.Versions[0].Version, publishedExtension1);
        }
      }
      else
        publishedExtension1 = this.LoadExtension(requestContext, publisherName, extensionName, version, accountToken);
      return publishedExtension1;
    }

    protected internal virtual void StorePublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      PublishedExtension publishedExtension)
    {
      requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((requestContextTask, taskArgs) => requestContextTask.GetService<IPublishedExtensionStorageService>().StorePublishedExtensionObject(requestContextTask, publisherName, extensionName, version, publishedExtension)));
    }

    private void QueueRefreshCheck(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      string str = string.Format("{0}.{1}.{2}", (object) publisherName, (object) extensionName, (object) version);
      requestContext.Trace(100136255, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Attempting to queue refresh for extension: {0}", (object) str);
      IVssRequestContext context = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      bool flag = false;
      using (context.AcquireReaderLock(this.m_extensionRefreshLockName))
        flag = this.m_extensionRefreshNeeded.Contains(str);
      if (!flag)
      {
        ITeamFoundationTaskService service = context.GetService<ITeamFoundationTaskService>();
        using (context.AcquireWriterLock(this.m_extensionRefreshLockName))
        {
          if (!this.m_extensionRefreshNeeded.Add(str))
            return;
          DateTime startTime = DateTime.UtcNow + this.m_refreshTaskDelay;
          service.AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContextTask, args) => this.RemoveExtension(requestContextTask, publisherName, extensionName, version)), (object) null, startTime, 0));
          requestContext.Trace(100136260, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Refresh queued for extension: {0}", (object) str);
        }
      }
      else
        requestContext.Trace(100136254, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Refresh already queued for extension: {0}", (object) str);
    }

    private void RemoveExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      string str = string.Format("{0}.{1}.{2}", (object) publisherName, (object) extensionName, (object) version);
      requestContext.Trace(100136253, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Attempting to refresh version of extension: {0}", (object) str);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      PublishedExtensionMemoryCache service = vssRequestContext.GetService<PublishedExtensionMemoryCache>();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      IVssRequestContext requestContext1 = vssRequestContext;
      string key = fullyQualifiedName;
      ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
      ref ConcurrentDictionary<string, PublishedExtension> local = ref concurrentDictionary;
      if (!service.TryGetValue(requestContext1, key, out local))
        return;
      PublishedExtension extension;
      if (concurrentDictionary.TryGetValue(version, out extension))
      {
        try
        {
          if (extension == null)
            extension = vssRequestContext.GetService<IGalleryService>().GetExtension(vssRequestContext, publisherName, extensionName, version, ExtensionQueryFlags.None);
          if (extension == null || !extension.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
            return;
          requestContext.Trace(100136251, TraceLevel.Error, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Unable to load version of built in extension: {0}", (object) str);
          using (requestContext.AcquireWriterLock(this.m_extensionRefreshLockName))
            this.m_extensionRefreshNeeded.Remove(str);
          requestContext.Trace(100136252, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Extension removed from refresh cache: {0}", (object) str);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(100136253, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, ex);
          throw;
        }
        finally
        {
          concurrentDictionary.TryRemove(version, out extension);
          requestContext.Trace(100136254, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Extension removed from cache: {0}", (object) str);
        }
      }
      else
      {
        using (requestContext.AcquireWriterLock(this.m_extensionRefreshLockName))
          this.m_extensionRefreshNeeded.Remove(str);
      }
    }

    protected internal virtual PublishedExtension LoadExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken)
    {
      requestContext.Trace(10013400, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Cache miss. Loading extension from gallery. Identifier: {0}.{1}{2}", (object) publisherName, (object) extensionName, accountToken == null ? (object) string.Empty : (object) string.Format("with token '{0}'", (object) accountToken));
      PublishedExtension publishedExtension = (PublishedExtension) null;
      try
      {
        if (this.ShouldBypassGalleryCalls(requestContext))
        {
          requestContext.Trace(100136316, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "LoadExtension: Account token is null and BypassGalleryCalls FF is enabled, serving PubilshedExtension object from file service");
          requestContext.GetService<IPublishedExtensionStorageService>().TryGetPublishedExtensionObject(requestContext, publisherName, extensionName, version, out publishedExtension);
        }
        else
        {
          publishedExtension = this.FetchExtensionFromGallery(requestContext, publisherName, extensionName, version, accountToken);
          if (publishedExtension != null)
          {
            if (!publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
            {
              if (!requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.PersistPrivateExtension))
                goto label_8;
            }
            this.StorePublishedExtensionObject(requestContext, publisherName, extensionName, version, publishedExtension);
          }
        }
      }
      catch (ExtensionDoesNotExistException ex)
      {
        requestContext.TraceException(10013081, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, (Exception) ex);
      }
label_8:
      return publishedExtension;
    }

    protected internal virtual PublishedExtension FetchExtensionFromGallery(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken)
    {
      GalleryHttpClient client = requestContext.GetClient<GalleryHttpClient>();
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri;
      if (!requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.UseCdnAssetUri))
        extensionQueryFlags |= ExtensionQueryFlags.UseFallbackAssetUri;
      string str = version;
      if (!string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase))
      {
        extensionQueryFlags |= ExtensionQueryFlags.IncludeLatestVersionOnly;
        str = (string) null;
      }
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string version1 = str;
      ExtensionQueryFlags? flags = new ExtensionQueryFlags?(extensionQueryFlags);
      string accountTokenHeader = accountToken;
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetExtensionAsync(publisherName1, extensionName1, version1, flags, accountTokenHeader: accountTokenHeader, cancellationToken: cancellationToken).SyncResult<PublishedExtension>();
    }

    protected internal virtual bool ShouldBypassGalleryCalls(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.BypassGalleryCalls);

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
        requestContext.Trace(10013409, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "ExtPublishedExtensionService.OnPublishedExtensionChanged received notification Id: {0} EventType: {1}", (object) fullyQualifiedName, (object) changeNotification.EventType);
        if (changeNotification.EventType == ExtensionEventType.ExtensionShared)
          return;
        requestContext.Trace(10013410, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "Refreshing extension. Identifer: {0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
        ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
        if (!vssRequestContext.GetService<PublishedExtensionMemoryCache>().TryGetValue(vssRequestContext, fullyQualifiedName, out concurrentDictionary))
          return;
        concurrentDictionary.Clear();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013290, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, ex);
      }
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<PublishedExtensionMemoryCache>().Clear(vssRequestContext);
      requestContext.Trace(10013098, TraceLevel.Info, PublishedExtensionCache.s_area, PublishedExtensionCache.s_layer, "ExtPublishedExtensionService.OnForceFlush processed");
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConfiguration(requestContext);
    }
  }
}
