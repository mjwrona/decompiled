// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AadMemberAccessStatus.TracePoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.AadMemberAccessStatus
{
  public static class TracePoints
  {
    public static class AadMemberStatusServiceBase
    {
      public const int GetAadMemberStatusEnter = 9002400;
      public const int GetAadMemberStatusInfo = 9002401;
      public const int GetAadMemberStatusError = 9002402;
      public const int GetAadMemberStatusWarning = 9002403;
      public const int GetAadMemberStatusLeave = 9002404;
      public const int GetAadMemberStatusFromCacheEnter = 90024005;
      public const int GetAadMemberStatusFromCacheInfo = 9002406;
      public const int GetAadMemberStatusFromCacheLeave = 9002409;
      public const int GetCachedStatusEnter = 9002410;
      public const int GetCachedStatusInfo = 9002411;
      public const int GetCachedStatusError = 9002412;
      public const int GetCachedStatusremoteCacheMiss = 9002413;
      public const int GetCachedStatusLocalCacheHit = 9002414;
      public const int GetCachedStatusLocalCacheExpired = 9002415;
      public const int GetCachedStatusLocalCacheMiss = 9002416;
      public const int GetCachedTokenRemoteCacheHit = 9002417;
      public const int GetCachedStatusRemoteCacheExpired = 9002418;
      public const int GetCachedStatusLeave = 9002419;
      public const int UpdateCacheError = 9002422;
      public const int UpdateCacheLeave = 9002421;
      public const int UpdateCacheEnter = 9002420;
      public const int GetAadMemberStatusCacheMiss = 9002423;
      public const int AcquireTokenCacheMiss = 9002424;
      public const int ResolveDeploymentDescriptor = 9002429;
    }

    public static class FrameworkAadMemberStatusService
    {
      public const int GetAadMemberStatusFromRemoteEnter = 9002430;
      public const int GetAadMemberStatusFromRemoteLeave = 9002431;
      public const int GetAadMemberStatusFromRemoteError = 9002432;
    }
  }
}
