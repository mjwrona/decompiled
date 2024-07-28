// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.DailyStatsHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class DailyStatsHelper
  {
    public static void IncrementConnectedInstallCount(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      requestContext.GetService<IExtensionDailyStatsService>().IncrementStatCount(requestContext, extension, DailyStatType.ConnectedInstall);
    }

    public static void IncrementWebDownloadCount(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isBatchingEnabled = false,
      string version = null,
      string targetPlatform = null)
    {
      IExtensionDailyStatsService service = requestContext.GetService<IExtensionDailyStatsService>();
      IVssRequestContext requestContext1 = requestContext;
      PublishedExtension extension1 = extension;
      bool flag = isBatchingEnabled;
      string str1 = version;
      string str2 = targetPlatform;
      DateTime? statDate = new DateTime?();
      int num = flag ? 1 : 0;
      string version1 = str1;
      string targetPlatform1 = str2;
      service.IncrementStatCount(requestContext1, extension1, DailyStatType.WebDownload, statDate, num != 0, version1, targetPlatform1);
    }

    public static void IncrementInstallCount(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isBatchingEnabled = false,
      string version = null,
      string targetPlatform = null)
    {
      IExtensionDailyStatsService service = requestContext.GetService<IExtensionDailyStatsService>();
      IVssRequestContext requestContext1 = requestContext;
      PublishedExtension extension1 = extension;
      bool flag = isBatchingEnabled;
      string str1 = version;
      string str2 = targetPlatform;
      DateTime? statDate = new DateTime?();
      int num = flag ? 1 : 0;
      string version1 = str1;
      string targetPlatform1 = str2;
      service.IncrementStatCount(requestContext1, extension1, DailyStatType.Install, statDate, num != 0, version1, targetPlatform1);
    }

    public static void IncrementUnInstallCount(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isBatchingEnabled = false)
    {
      IExtensionDailyStatsService service = requestContext.GetService<IExtensionDailyStatsService>();
      IVssRequestContext requestContext1 = requestContext;
      PublishedExtension extension1 = extension;
      bool flag = isBatchingEnabled;
      DateTime? statDate = new DateTime?();
      int num = flag ? 1 : 0;
      service.IncrementStatCount(requestContext1, extension1, DailyStatType.Uninstall, statDate, num != 0);
    }

    public static void IncrementInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      PublishedExtension extension = DailyStatsHelper.GetExtension(requestContext, publisherName, extensionName);
      requestContext.GetService<IExtensionDailyStatsService>().IncrementStatCount(requestContext, extension, DailyStatType.Install);
    }

    public static void RefreshAverageRatingStat(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      requestContext.GetService<IExtensionDailyStatsService>().RefreshAverageRatingStat(requestContext, extension);
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Parameters are validated using ArgumentUtility")]
    public static PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      bool getExtensionProperties = false)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly;
      if (getExtensionProperties)
        extensionQueryFlags |= ExtensionQueryFlags.IncludeVersionProperties;
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      bool flag = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      int flags = (int) extensionQueryFlags;
      int num = flag ? 1 : 0;
      return service.QueryExtension(requestContext1, publisherName1, extensionName1, (string) null, (ExtensionQueryFlags) flags, (string) null, num != 0);
    }

    public static PublishedExtension GetExtensionVersion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets;
      IPublishedExtensionService service = vssRequestContext.GetService<IPublishedExtensionService>();
      bool flag = vssRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
      IVssRequestContext requestContext1 = vssRequestContext;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      string version1 = version;
      int flags = (int) extensionQueryFlags;
      int num = flag ? 1 : 0;
      return service.QueryExtension(requestContext1, publisherName1, extensionName1, version1, (ExtensionQueryFlags) flags, (string) null, num != 0);
    }
  }
}
