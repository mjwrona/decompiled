// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.TfsClientThrottlingTracepoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule
{
  public class TfsClientThrottlingTracepoints
  {
    public const int TracepointBase = 34003700;
    public const int CachingTracepointBase = 34003700;
    public const int DelegatingHandlerBase = 34003800;
    private const int Offset = 100;

    public static class CachingTracepoints
    {
      public const string Area = "Caching";
      public const string ServiceTraceLayer = "Service";
      public const int RateLimiterCacheServiceEndEnter = 34003700;
      public const int RateLimiterCacheServiceEndLeave = 34003701;
      public const int RateLimiterCacheServiceEndException = 34003702;
      public const int RateLimiterCacheServiceStartEnter = 34003703;
      public const int RateLimiterCacheServiceStartLeave = 34003704;
      public const int RateLimiterCacheServiceStartException = 34003705;
    }

    public static class DelegatingHandlerTracepoints
    {
      public const string Area = "DelegatingHandler";
      public const int ClientRateLimiterHandlerEnter = 34003800;
      public const int ClientRateLimiterHandlerLeaveNoRetry = 34003801;
      public const int ClientRateLimiterHandlerLeaveWithRetryAfter = 34003802;
      public const int ClientRateLimiterHandlerCacheUpdate = 34003803;
      public const int ClientRateLimiterHandlerConcurrentThrottling = 34003804;
      public const int ClientRateLimiterHandlerMissingRetryAfter = 34003805;
      public const int ClientRateLimiterHandlerThrottling = 34003806;
    }
  }
}
