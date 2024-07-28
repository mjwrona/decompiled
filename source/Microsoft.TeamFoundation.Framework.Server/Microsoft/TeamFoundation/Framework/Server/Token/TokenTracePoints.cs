// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Token.TokenTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Token
{
  internal static class TokenTracePoints
  {
    internal static class FrameworkTokenCacheServiceTracePoints
    {
      internal const int SetAccessTokenEnter = 1048250;
      internal const int SetAccessTokenCachingLocally = 1048251;
      internal const int SetAccessTokenSkippingLocalCache = 1048252;
      internal const int SetAccessTokenSkippingremoteCahce = 1048258;
      internal const int SetAccessTokenLeave = 1048259;
      internal const int SetTokenCachingToRedis = 1048253;
      internal const int SetTokenRedisIgnoredSetRequest = 1048254;
      internal const int SetTokenRedisNotEnabled = 1048257;
      internal const int GetAccessTokenEnter = 1048260;
      internal const int GetAccessTokenFoundInLocalCache = 1048261;
      internal const int GetAccessTokenFoundInRemoteCache = 1048262;
      internal const int GetAccessTokenRedisDisabled = 1048265;
      internal const int GetAccessTokenSkippingRemoteLookup = 1048266;
      internal const int AddNoAccessTokenEntry = 1048266;
      internal const int GetAccessTokenLeave = 1048269;
      internal const int InvalidateAccessTokenEnter = 1048270;
      internal const int InvalidateAccessTokenRemovingLocal = 1048271;
      internal const int InvalidateAccessTokenSkippingRemote = 1048277;
      internal const int InvalidateAccessTokenLeave = 1048279;
      internal const int InvalidateRedisTokenRemote = 1048272;
      internal const int InvalidateRedisNotEnabled = 1048278;
      internal const int SqlNotificationInvalidatingAccessToken = 1048275;
      internal const int SqlNotificationCalledInalidateAccessTokens = 1048276;
      internal const int InvalidateAccessTokensEnter = 1048280;
      internal const int InvalidateAccessTokensLeave = 1048289;
      internal const int AddNoAccessTokenEntryToLocalCacheEnter = 1048290;
      internal const int AddNoAccessTokenEntryToLocalCacheLeave = 1048299;
    }
  }
}
