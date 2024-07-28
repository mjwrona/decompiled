// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DelegatedAuthorization.DelegatedAuthorizationTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.DelegatedAuthorization
{
  internal static class DelegatedAuthorizationTracePoints
  {
    internal static class FrameworkDelegatedAuthorizationCacheServiceTracePoints
    {
      internal const int SetAccessTokenEnter = 1048050;
      internal const int SetAccessTokenCachingLocally = 1048051;
      internal const int SetAccessTokenSkippingLocalCache = 1048052;
      internal const int SetAccessTokenSkippingRemoteCache = 1048058;
      internal const int SetAccessTokenLeave = 1048059;
      internal const int SetTokenCachingToRedis = 1048053;
      internal const int SetTokenRedisIgnoredSetRequest = 1048054;
      internal const int SetTokenRedisNotEnabled = 1048057;
      internal const int GetAccessTokenEnter = 1048060;
      internal const int GetAccessTokenFoundInLocalCache = 1048061;
      internal const int GetAccessTokenFoundInRemoteCache = 1048062;
      internal const int GetAccessTokenRedisDisabled = 1048065;
      internal const int GetAccessTokenSkippingRemoteLookup = 1048066;
      internal const int AddNoAccessTokenEntry = 1048066;
      internal const int GetAccessTokenLeave = 1048069;
      internal const int InvalidateAccessTokenEnter = 1048070;
      internal const int InvalidateAccessTokenRemovingLocal = 1048071;
      internal const int InvalidateAccessTokenSkippingRemote = 1048077;
      internal const int InvalidateAccessTokenLeave = 1048079;
      internal const int InvalidateRedisTokenRemote = 1048072;
      internal const int InvalidateRedisNotEnabled = 1048078;
      internal const int SqlNotificationInvalidatingAccessToken = 1048075;
      internal const int SqlNotificationCalledInalidateAccessTokens = 1048076;
      internal const int InvalidateAccessTokensEnter = 1048080;
      internal const int InvalidateAccessTokensLeave = 1048089;
      internal const int AddNoAccessTokenEntryToLocalCacheEnter = 1048090;
      internal const int AddNoAccessTokenEntryToLocalCacheLeave = 1048099;
    }
  }
}
