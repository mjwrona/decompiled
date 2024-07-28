// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.QueryExtensionCacheService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class QueryExtensionCacheService : CustomExpiryCache
  {
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

    public PublishedExtension QueryExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string versionQuery,
      Guid validationId,
      ExtensionQueryFlags flags)
    {
      PublishedExtension publishedExtension = (PublishedExtension) null;
      ExtensionQueryFlags flags1 = this.ProcessQueryFlagsForCache(versionQuery);
      Stopwatch stopwatch = new Stopwatch();
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = false;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCache") && validationId == Guid.Empty)
      {
        string extVersion = versionQuery.IsNullOrEmpty<char>() ? "latest" : versionQuery;
        PublishedExtension extension = this.GetExtensionFromCache(requestContext, publisherName, extensionName, extVersion);
        if (extension == null)
        {
          flag2 = false;
          stopwatch.Start();
          using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
            extension = component.QueryExtension(publisherName, extensionName, versionQuery, Guid.Empty, flags1);
          if (extension != null)
          {
            this.SetExtensionInCache(requestContext, extension, publisherName, extensionName, extVersion);
            flag3 = true;
          }
          stopwatch.Stop();
        }
        publishedExtension = GalleryUtil.CloneExtension(extension, flags);
      }
      else
      {
        flag1 = false;
        stopwatch.Start();
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          publishedExtension = component.QueryExtension(publisherName, extensionName, versionQuery, validationId, flags);
        stopwatch.Stop();
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionCacheTelemetry") && stopwatch.ElapsedMilliseconds > 0L)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("eventType", nameof (QueryExtension));
        properties.Add("isCacheUsed", flag1);
        properties.Add(nameof (publisherName), publisherName);
        properties.Add(nameof (extensionName), extensionName);
        properties.Add("extVersion", versionQuery);
        properties.Add("extensionFoundInCache", flag2);
        properties.Add("extensionUpdatedInCache", flag3);
        properties.Add("dbTimeElapsedInMs", (double) stopwatch.ElapsedMilliseconds);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "QueryExtensionCache", properties);
      }
      return publishedExtension;
    }

    private PublishedExtension GetExtensionFromCache(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string extVersion)
    {
      PublishedExtension extensionFromCache = (PublishedExtension) null;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
      if (this.TryGetValue<ConcurrentDictionary<string, PublishedExtension>>(requestContext, fullyQualifiedName, out concurrentDictionary) && concurrentDictionary.Keys.Contains(extVersion))
        extensionFromCache = concurrentDictionary[extVersion];
      return extensionFromCache;
    }

    private void SetExtensionInCache(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string publisherName,
      string extensionName,
      string extVersion)
    {
      if (extension == null || !extension.IsValidated() || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionCacheForPrivate") && !extension.IsPublic())
        return;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      ConcurrentDictionary<string, PublishedExtension> concurrentDictionary;
      if (!this.TryGetValue<ConcurrentDictionary<string, PublishedExtension>>(requestContext, fullyQualifiedName, out concurrentDictionary))
      {
        concurrentDictionary = new ConcurrentDictionary<string, PublishedExtension>();
        this.Set(fullyQualifiedName, (object) concurrentDictionary, this.OneHourExpiryProvider);
      }
      concurrentDictionary[extVersion] = extension;
      if (!extVersion.Equals("latest", StringComparison.OrdinalIgnoreCase))
        return;
      concurrentDictionary[extension.Versions[0].Version] = extension;
    }

    private ExtensionQueryFlags ProcessQueryFlagsForCache(string version)
    {
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeLcids | ExtensionQueryFlags.IncludeSharedOrganizations;
      if (!string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        extensionQueryFlags |= ExtensionQueryFlags.IncludeLatestVersionOnly;
      return extensionQueryFlags;
    }

    private void ExtensionChangeCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!string.IsNullOrEmpty(eventData))
      {
        string[] source = eventData.Split((char[]) null);
        if (source.Length > 1 && !((IEnumerable<string>) source).Last<string>().IsNullOrEmpty<char>())
          this.ClearExtensionFromCache(requestContext, ((IEnumerable<string>) source).Last<string>());
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "queryExtensionCacheRefresh");
      properties.Add(nameof (eventData), eventData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "QueryExtensionCache", properties);
    }

    private void ClearExtensionFromCache(IVssRequestContext requestContext, string extensionName) => this.Remove(requestContext, extensionName);
  }
}
