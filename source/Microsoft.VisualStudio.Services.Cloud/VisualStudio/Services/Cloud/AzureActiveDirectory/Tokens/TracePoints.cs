// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  public static class TracePoints
  {
    public static class AadTokenServiceBase
    {
      public const int AcquireTokenEnter = 9002011;
      public const int AcquireTokenLeave = 9001012;
      public const int AcquireTokenInfo = 9002013;
      public const int AcquireTokenError = 9002014;
      public const int AcquireTokenWarn = 9002035;
      public const int GetCachedTokenEnter = 9002015;
      public const int GetCachedTokenLeave = 9002016;
      public const int GetCachedTokenLocalCacheHit = 9002017;
      public const int GetCachedTokenLocalCacheMiss = 9002018;
      public const int GetCachedTokenLocalCacheExpired = 9002019;
      public const int GetCachedTokenLocalCacheInfo = 9002020;
      public const int GetCachedTokenRemoteCacheHit = 9002021;
      public const int GetCachedTokenRemoteCacheMiss = 9002022;
      public const int GetCachedTokenRemoteCacheExpired = 9002023;
      public const int GetCachedTokenRemoteCacheInfo = 9002024;
      public const int LocalCacheUpdate = 9002025;
      public const int RemoteCacheUpdate = 9002026;
      public const int AcquireAppTokenEnter = 9002027;
      public const int AcquireAppTokenLeave = 9002028;
      public const int AcquireAppTokenInfo = 9002029;
      public const int AcquireAppTokenError = 9002030;
      public const int AcquireTokenFromAuthCodeEnter = 9002031;
      public const int AcquireTokenFromAuthCodeLeave = 9002032;
      public const int AcquireTokenFromAuthCodeInfo = 9002033;
      public const int AcquireTokenFromAuthCodeError = 9002034;
      public const int EvictExpiredTokensException = 9002036;
      public const int AcquireTokenFromCacheEnter = 9002037;
      public const int AcquireTokenFromCacheLeave = 9002038;
      public const int AcquireTokenFromCacheInfo = 9002039;
      public const int AcquireTokenFromCacheError = 9002040;
      public const int AcquireTokenFromCacheWarn = 9002041;
    }

    public static class FrameworkAadTokenService
    {
      public const int GetUserAccessTokenEnter = 9002101;
      public const int GetUserAccessTokenLeave = 9002102;
      public const int GetUserAccessTokenInfo = 9002103;
      public const int GetUserAccessTokenSuccess = 9002104;
      public const int GetUserAccessTokenFailed = 9002105;
      public const int GetAppAccessTokenEnter = 9002106;
      public const int GetAppAccessTokenLeave = 9002107;
      public const int GetAppAccessTokenInfo = 9002108;
      public const int GetAppAccessTokenSuccess = 9002109;
      public const int GetAppAccessTokenFailed = 9002110;
      public const int UpdateRefreshTokenEnter = 9002111;
      public const int UpdateRefreshTokenLeave = 9002112;
      public const int UpdateRefreshTokenInfo = 9002113;
      public const int UpdateRefreshTokenSuccess = 9002114;
      public const int UpdateRefreshTokenFailed = 9002115;
      public const int GetUserAccessTokenFromAuthCodeEnter = 9002116;
      public const int GetUserAccessTokenFromAuthCodeLeave = 9002117;
      public const int GetUserAccessTokenFromAuthCodeInfo = 9002118;
      public const int GetUserAccessTokenFromAuthCodeSuccess = 9002119;
      public const int GetUserAccessTokenFromAuthCodeFailed = 9002120;
    }

    public static class AadOnBehalfOfHandlerTracepoints
    {
      public const int TryGetRefreshTokenOnBehalfOfUserStart = 9002200;
      public const int TryGetRefreshTokenOnBehalfOfUserInfo = 9002201;
      public const int TryGetRefreshTokenOnBehalfOfUserError = 9002202;
      public const int TryGetRefreshTokenOnBehalfOfUserEnd = 9002209;
    }
  }
}
