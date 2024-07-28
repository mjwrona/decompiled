// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RateLimiterCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RateLimiterCacheService : 
    VssMemoryCacheService<string, IDictionary<string, string>>,
    IRateLimiterCacheService,
    IVssFrameworkService
  {
    private static readonly TimeSpan expiryInterval = TimeSpan.FromMinutes(90.0);
    private static readonly TimeSpan cleanupInterval = TimeSpan.FromMinutes(30.0);
    private static readonly TimeSpan inactivityAge = TimeSpan.FromMinutes(90.0);

    public RateLimiterCacheService()
      : base((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase, RateLimiterCacheService.cleanupInterval)
    {
      this.ExpiryInterval.Value = RateLimiterCacheService.expiryInterval;
      this.InactivityInterval.Value = RateLimiterCacheService.inactivityAge;
    }

    public bool TryGetRateLimiterHeaders(
      IVssRequestContext requestContext,
      Guid userId,
      string clientName,
      out IDictionary<string, string> rateLimiterHeaders)
    {
      return this.TryGetValue(requestContext, userId.ToString("N") + "-" + clientName, out rateLimiterHeaders);
    }

    public void UpdateRateLimiterHeaders(
      IVssRequestContext requestContext,
      Guid userId,
      string clientName,
      IDictionary<string, string> rateLimiterHeaders)
    {
      this.Set(requestContext, userId.ToString("N") + "-" + clientName, rateLimiterHeaders);
    }
  }
}
