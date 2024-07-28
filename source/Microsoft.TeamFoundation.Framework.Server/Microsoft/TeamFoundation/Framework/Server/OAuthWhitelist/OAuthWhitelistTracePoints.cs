// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.OAuthWhitelistTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  public static class OAuthWhitelistTracePoints
  {
    public const int TracePointStart = 33000000;
    public const int AddOAuthWhitelistEntryEnter = 33000000;
    public const int AddOAuthWhitelistEntryLeave = 33000001;
    public const int AddOAuthWhitelistEntryException = 33000002;
    public const int DeleteOAuthWhitelistEntryEnter = 33000003;
    public const int DeleteOAuthWhitelistEntryLeave = 33000004;
    public const int DeleteOAuthWhitelistEntryException = 33000005;
    public const int GetOAuthWhitelistEntriesEnter = 33000006;
    public const int GetOAuthWhitelistEntriesLeave = 33000007;
    public const int GetOAuthWhitelistEntriesException = 33000008;
    public const int InitializeCacheEnter = 33000009;
    public const int InitializeCacheLeave = 33000010;
    public const int InitializePlatformCacheData = 33000011;
    public const int InitializePlatformCacheExpiryData = 33000012;
    public const int InitializeFrameworkCacheData = 33000013;
    public const int InitializeFrameworkCacheExpiryData = 33000014;
    public const int GetOAuthWhitelistEntriesRestApiEnter = 33000015;
    public const int GetOAuthWhitelistEntriesRestApiLeave = 33000016;
    public const int IsWhitelistedEnter = 33000019;
    public const int IsWhitelistedLeave = 33000020;
    public const int ServiceStartEnter = 33000021;
    public const int ServiceStartLeave = 33000022;
    public const int ServiceEndEnter = 33000023;
    public const int ServiceEndLeave = 33000024;
    public const int OnRegistryChangedEnter = 33000025;
    public const int OnRegistryChangedLeave = 33000026;
    public const int OnOAuthWhitelistChangedEnter = 33000027;
    public const int OnOAuthWhitelistChangedLeave = 33000028;
    public const int OAuthWhitelistCacheEnter = 33000029;
    public const int OAuthWhitelistCacheLeave = 33000030;
    public const int AddWhitelistCacheData = 33000031;
    public const int DeleteWhitelistCacheData = 33000032;
    public const int AddWhitelistData = 33000033;
    public const int DeleteWhitelistData = 33000034;
    public const int WhitelistCacheCtorData = 33000035;
    public const int WhitelistCacheGetData = 33000036;
    public const int RegistryChangedData = 33000037;
    public const int WhitelistChangedData = 33000038;
    public const int WhitelistCacheBaseGetData = 33000039;
    public const int WhitelistCacheBaseIsWhitelistedData = 33000040;
    public const int WhitelistServiceBaseGetData = 33000041;
    public const int WhitelistServiceBaseIsWhitelistedData = 33000042;
    public const int OAuthWhitelistMessageSubscriberReceiveEnter = 33000043;
    public const int OAuthWhitelistMessageSubscriberReceiveLeave = 33000044;
    public const int NotifyWhitelistChangedEnter = 33000045;
    public const int NotifyWhitelistChangedLeave = 33000046;
    public const int AuthenticationMessageSubscriberReceiveEnter = 33000047;
    public const int AuthenticationMessageSubscriberReceiveLeave = 33000048;
  }
}
