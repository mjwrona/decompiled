// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ContributionManifestService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ContributionManifestService : 
    VssMemoryCacheService<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ExtensionManifest>>>,
    IContributionManifestService,
    IVssFrameworkService
  {
    private const string s_useCdnAssetUriFlag = "Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri";
    private const string s_area = "ContributionManifestService";
    private const string s_layer = "Service";
    private const string DefaultLanguage = "default";
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(12.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(30.0);
    private static readonly TimeSpan s_cleanupInterval = new TimeSpan(0, 5, 0);

    public ContributionManifestService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, ContributionManifestService.s_cleanupInterval)
    {
      this.ExpiryInterval.Value = ContributionManifestService.s_maxCacheLife;
      this.InactivityInterval.Value = ContributionManifestService.s_maxCacheInactivityAge;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      ITeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
      notificationService.RegisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
      service.UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      base.ServiceEnd(requestContext);
    }

    public virtual bool TryGetManifest(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string accountToken,
      out ExtensionManifest extensionManifest)
    {
      requestContext.TraceEnter(10013080, nameof (ContributionManifestService), "Service", "TryGetExtension");
      try
      {
        string requestCulture;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          requestCulture = "default";
        }
        else
        {
          using (new RequestLanguage(requestContext))
            requestCulture = Thread.CurrentThread.CurrentUICulture.Name;
        }
        ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
        ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
        ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
        try
        {
          if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/ExtensionService/Cache", true, true))
          {
            string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
            ConcurrentDictionary<string, ConcurrentDictionary<string, ExtensionManifest>> concurrentDictionary;
            if (!this.TryGetValue(requestContext, fullyQualifiedName, out concurrentDictionary))
            {
              concurrentDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, ExtensionManifest>>();
              this.Set(requestContext, fullyQualifiedName, concurrentDictionary);
            }
            ConcurrentDictionary<string, ExtensionManifest> languageCache = concurrentDictionary.GetOrAdd(version, (Func<string, ConcurrentDictionary<string, ExtensionManifest>>) (k => new ConcurrentDictionary<string, ExtensionManifest>()));
            extensionManifest = languageCache.GetOrAdd(requestCulture, (Func<string, ExtensionManifest>) (requestedCulture =>
            {
              ExtensionManifest manifest = this.LoadManifest(requestContext, publisherName, extensionName, version, requestCulture, accountToken);
              string str = requestCulture;
              if (manifest != null)
                manifest.Language = str;
              ExtensionManifest orAdd = languageCache.GetOrAdd(str, manifest);
              if (!string.Equals(str, requestedCulture))
                languageCache.TryAdd(requestedCulture, orAdd);
              return manifest;
            }));
          }
          else
            extensionManifest = this.LoadManifest(requestContext, publisherName, extensionName, version, requestCulture, accountToken);
        }
        catch (Exception ex)
        {
          extensionManifest = (ExtensionManifest) null;
          requestContext.TraceException(10013082, nameof (ContributionManifestService), "Service", ex);
        }
        return extensionManifest != null;
      }
      finally
      {
        requestContext.TraceLeave(10013085, nameof (ContributionManifestService), "Service", "TryGetExtension");
      }
    }

    private ExtensionManifest LoadManifest(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string language,
      string accountToken)
    {
      ExtensionManifest extensionManifest = (ExtensionManifest) null;
      requestContext.TraceEnter(10013090, nameof (ContributionManifestService), "Service", "LoadExtension");
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        PublishedExtension publishedExtension = vssRequestContext.GetService<IPublishedExtensionCache>().GetPublishedExtension(vssRequestContext, publisherName, extensionName, version, accountToken);
        ExtensionFile manifestFile = (ExtensionFile) null;
        string assetUri = (string) null;
        string fallbackAssetUri = (string) null;
        if (publishedExtension != null && publishedExtension.Versions != null)
        {
          foreach (ExtensionVersion version1 in publishedExtension.Versions)
          {
            if (version1.Version == version)
            {
              assetUri = version1.AssetUri;
              fallbackAssetUri = version1.FallbackAssetUri;
              if (version1.Files != null)
              {
                int num = int.MaxValue;
                ExtensionFile extensionFile1 = (ExtensionFile) null;
                ExtensionFile extensionFile2 = (ExtensionFile) null;
                CultureInfo cultureInfo;
                try
                {
                  cultureInfo = CultureInfo.GetCultureInfo(language);
                }
                catch (CultureNotFoundException ex)
                {
                  cultureInfo = (CultureInfo) null;
                }
                foreach (ExtensionFile file in version1.Files)
                {
                  if (file.AssetType.Equals("Microsoft.VisualStudio.Services.Manifest", StringComparison.OrdinalIgnoreCase))
                  {
                    if (extensionFile2 == null)
                      extensionFile2 = file;
                    if (extensionFile1 == null && file.Language == null)
                    {
                      extensionFile1 = file;
                      if (cultureInfo == null)
                        break;
                    }
                    if (cultureInfo != null)
                    {
                      try
                      {
                        if (language != null)
                        {
                          if (file.Language != null)
                          {
                            int matchingDistance = GalleryUtil.GetCultureMatchingDistance(CultureInfo.GetCultureInfo(language), CultureInfo.GetCultureInfo(file.Language));
                            if (matchingDistance != -1)
                            {
                              if (matchingDistance < num)
                              {
                                manifestFile = file;
                                num = matchingDistance;
                              }
                            }
                          }
                        }
                      }
                      catch (CultureNotFoundException ex)
                      {
                      }
                    }
                  }
                }
                if (manifestFile == null)
                {
                  if (extensionFile1 == null && extensionFile2 != null)
                    extensionFile1 = extensionFile2;
                  manifestFile = extensionFile1;
                  break;
                }
                break;
              }
              break;
            }
          }
        }
        if (manifestFile != null)
        {
          IDictionary<string, object> extensionProperties = (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(publishedExtension.Flags);
          extensionProperties.Add("::Version", (object) version);
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            IManifestStorageService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IManifestStorageService>();
            if (!service.TryGetManifest(requestContext, publisherName, extensionName, version, language, extensionProperties, out extensionManifest))
            {
              extensionManifest = this.FetchManifest(requestContext, manifestFile, publisherName, extensionName, version, assetUri, fallbackAssetUri, extensionProperties);
              service.StoreManifest(requestContext, publisherName, extensionName, version, language, manifestFile);
            }
          }
          else
            extensionManifest = this.FetchManifest(requestContext, manifestFile, publisherName, extensionName, version, assetUri, fallbackAssetUri, extensionProperties);
          if (string.IsNullOrEmpty(extensionManifest.BaseUri))
          {
            extensionManifest.BaseUri = assetUri;
            if (!string.IsNullOrEmpty(assetUri))
            {
              if (!string.IsNullOrEmpty(fallbackAssetUri))
              {
                if (!assetUri.Equals(fallbackAssetUri, StringComparison.OrdinalIgnoreCase))
                  extensionManifest.FallbackBaseUri = fallbackAssetUri;
              }
            }
          }
        }
      }
      catch (ExtensionDoesNotExistException ex)
      {
        requestContext.TraceException(10013081, nameof (ContributionManifestService), "Service", (Exception) ex);
      }
      catch (ExtensionVersionNotFoundException ex)
      {
        requestContext.TraceException(10013081, nameof (ContributionManifestService), "Service", (Exception) ex);
      }
      catch (ExtensionAssetNotFoundException ex)
      {
        requestContext.TraceException(10013081, nameof (ContributionManifestService), "Service", (Exception) ex);
      }
      finally
      {
        requestContext.TraceLeave(10013095, nameof (ContributionManifestService), "Service", "LoadExtension");
      }
      return extensionManifest;
    }

    private ExtensionManifest FetchManifest(
      IVssRequestContext requestContext,
      ExtensionFile manifestFile,
      string publisherName,
      string extensionName,
      string version,
      string assetUri,
      string fallbackAssetUri,
      IDictionary<string, object> extraProperties)
    {
      IPublishedExtensionCache service = requestContext.Elevate().GetService<IPublishedExtensionCache>();
      try
      {
        using (Stream manifestStream = service.GetExtensionAsset(requestContext, manifestFile).SyncResult<Stream>())
          return ExtensionUtil.LoadManifest(publisherName, extensionName, version, manifestStream, extraProperties, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013096, nameof (ContributionManifestService), "Service", ex);
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri") && !string.IsNullOrEmpty(fallbackAssetUri))
        {
          string extensionFileUrl = manifestFile.Source.Replace(assetUri, fallbackAssetUri);
          if (!string.IsNullOrEmpty(extensionFileUrl) && !extensionFileUrl.Equals(manifestFile.Source, StringComparison.OrdinalIgnoreCase))
          {
            using (Stream manifestStream = service.GetExtensionAsset(requestContext, extensionFileUrl).SyncResult<Stream>())
              return ExtensionUtil.LoadManifest(publisherName, extensionName, version, manifestStream, extraProperties, true);
          }
        }
        throw;
      }
    }

    private void OnPublishedExtensionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(changeNotification.PublisherName, changeNotification.ExtensionName);
        requestContext.Trace(10013096, TraceLevel.Info, nameof (ContributionManifestService), "Service", "ContributionManifestService.OnPublishedExtensionChanged received notification Id: {0}.{1} EventType: {2}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName, (object) changeNotification.EventType);
        if (changeNotification.EventType == ExtensionEventType.ExtensionDeleted || string.IsNullOrEmpty(changeNotification.Version))
        {
          this.Remove(requestContext, fullyQualifiedName);
        }
        else
        {
          ConcurrentDictionary<string, ConcurrentDictionary<string, ExtensionManifest>> concurrentDictionary;
          if (changeNotification.EventType == ExtensionEventType.ExtensionShared || !this.TryGetValue(requestContext, fullyQualifiedName, out concurrentDictionary))
            return;
          concurrentDictionary.TryRemove(changeNotification.Version, out ConcurrentDictionary<string, ExtensionManifest> _);
          if (concurrentDictionary.Count != 0)
            return;
          this.Remove(requestContext, fullyQualifiedName);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013290, nameof (ContributionManifestService), "Service", ex);
      }
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData)
    {
      requestContext.Trace(10013097, TraceLevel.Info, nameof (ContributionManifestService), "Service", "ContributionManifestService.OnForFlush processed");
      this.Clear(requestContext);
    }
  }
}
