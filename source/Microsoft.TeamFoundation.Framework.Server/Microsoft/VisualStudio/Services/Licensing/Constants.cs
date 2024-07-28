// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Constants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class Constants
  {
    internal static class RegistryKeys
    {
      internal const string Root = "/BusinessPolicy/";
      internal const string CacheConfigurationRefreshInterval = "/BusinessPolicy/CacheConfigurationRefreshInterval";
      internal const string CacheEntryStateIdleLifetime = "/BusinessPolicy/CacheEntryStateIdleLifetime";
      internal const string NotificationFilter = "/BusinessPolicy/...";
    }

    internal class RegistrySettings
    {
      internal static readonly TimeSpan DefaultCacheConfigurationRefreshInterval = TimeSpan.FromMinutes(15.0);
      internal static readonly TimeSpan DefaultCacheEntryStateIdleLifetime = TimeSpan.FromHours(3.0);

      internal TimeSpan CacheConfigurationRefreshInterval { get; set; }

      internal TimeSpan CacheEntryStateIdleLifetime { get; set; }
    }

    internal class LicenseClaimConstants
    {
      internal static readonly string VssClaimUriPrefix = "vss:";
      internal static readonly string AccountRightsStateKeyPrefix = "AccountRights/";
      internal static readonly string AccountEntitlementStateKeyPrefix = "AccountEntitlement/";
      internal static readonly string AccountRightsClaimTypePrefix = Constants.LicenseClaimConstants.VssClaimUriPrefix + Constants.LicenseClaimConstants.AccountRightsStateKeyPrefix;
      internal static readonly string AccountEntitlementClaimTypePrefix = Constants.LicenseClaimConstants.VssClaimUriPrefix + Constants.LicenseClaimConstants.AccountEntitlementStateKeyPrefix;
      internal static readonly char KeyPathSeparator = '/';
    }
  }
}
