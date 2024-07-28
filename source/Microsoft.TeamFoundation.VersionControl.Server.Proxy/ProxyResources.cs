// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.ProxyResources
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal static class ProxyResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ProxyResources), typeof (ProxyResources).GetTypeInfo().Assembly);
    public const string FileCacheRootNotAbsolute = "FileCacheRootNotAbsolute";
    public const string RequestExpired = "RequestExpired";
    public const string RequestFileIdMalformed = "RequestFileIdMalformed";
    public const string RequestFileIdMissing = "RequestFileIdMissing";
    public const string RequestServerIdMissing = "RequestServerIdMissing";
    public const string RequestSignatureMissing = "RequestSignatureMissing";
    public const string RequestSignatureValidationFailed = "RequestSignatureValidationFailed";
    public const string RequestTimestampMalformed = "RequestTimestampMalformed";
    public const string RequestTimestampMissing = "RequestTimestampMissing";
    public const string SystemStartMessage = "SystemStartMessage";
    public const string SystemStopMessage = "SystemStopMessage";
    public const string UnknownProxyException = "UnknownProxyException";
    public const string UnknownRepository = "UnknownRepository";
    public const string CommitPathNotSet = "CommitPathNotSet";
    public const string MetadataFormatWrong = "MetadataFormatWrong";
    public const string UserAgent = "UserAgent";
    public const string ServerNotInitialized = "ServerNotInitialized";
    public const string UnknownErrorInGet = "UnknownErrorInGet";
    public const string ErrorCommitingToCache = "ErrorCommitingToCache";
    public const string UnableToStartDownloadThread = "UnableToStartDownloadThread";
    public const string ProxyCacheMissBecameHitException = "ProxyCacheMissBecameHitException";
    public const string ProxyClientRedirectException = "ProxyClientRedirectException";
    public const string StartingCacheCleanup = "StartingCacheCleanup";
    public const string InvalidCacheRoot = "InvalidCacheRoot";
    public const string InvalidCacheLimit = "InvalidCacheLimit";
    public const string InvalidCacheLimitPercent = "InvalidCacheLimitPercent";
    public const string InvalidCacheDeletionPercent = "InvalidCacheDeletionPercent";
    public const string ApplicationTierTimeout = "ApplicationTierTimeout";
    public const string ConfigurationNumericValueOutOfRange = "ConfigurationNumericValueOutOfRange";
    public const string ErrorDownloadingFromAppTier = "ErrorDownloadingFromAppTier";
    public const string ConfigInvalidUriFormat = "ConfigInvalidUriFormat";
    public const string ConfigMissingServerUriNode = "ConfigMissingServerUriNode";
    public const string ConfigValueNotNumeric = "ConfigValueNotNumeric";
    public const string ConfigFileHasNoServers = "ConfigFileHasNoServers";
    public const string ProxyStatsMissingNode = "ProxyStatsMissingNode";
    public const string MissingMetadataVariable = "MissingMetadataVariable";
    public const string ProxyInstanceSuffix = "ProxyInstanceSuffix";
    public const string CacheCleanupComplete = "CacheCleanupComplete";
    public const string DefaultCacheLimit = "DefaultCacheLimit";
    public const string ErrorComputingFolderStatistics = "ErrorComputingFolderStatistics";
    public const string ErrorDeterminingCacheCleanupList = "ErrorDeterminingCacheCleanupList";
    public const string ErrorReadingProxyStats = "ErrorReadingProxyStats";
    public const string DuplicateServerUri = "DuplicateServerUri";
    public const string TamperedProxyStatisticsFile = "TamperedProxyStatisticsFile";
    public const string ReadSchemaFailed = "ReadSchemaFailed";
    public const string ErrorDownloadingFromMidTier = "ErrorDownloadingFromMidTier";
    public const string ErrorProcessingProxyConfig = "ErrorProcessingProxyConfig";
    public const string VerifyCryptoFolderPermission = "VerifyCryptoFolderPermission";
    public const string ErrorLoadingCollection = "ErrorLoadingCollection";
    public const string ErrorInitializingPerformanceCounters = "ErrorInitializingPerformanceCounters";
    public const string DuplicateServer = "DuplicateServer";

    public static ResourceManager Manager => ProxyResources.s_resMgr;

    public static string Get(string resourceName) => ProxyResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? ProxyResources.Get(resourceName) : ProxyResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ProxyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ProxyResources.GetInt(resourceName) : (int) ProxyResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ProxyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ProxyResources.GetBool(resourceName) : (bool) ProxyResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => ProxyResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ProxyResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }
  }
}
