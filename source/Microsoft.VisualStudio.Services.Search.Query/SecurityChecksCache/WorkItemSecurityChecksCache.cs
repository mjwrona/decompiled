// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.WorkItemSecurityChecksCache
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public class WorkItemSecurityChecksCache : SecurityChecksCacheBase<WorkItemSecuritySet>
  {
    public WorkItemSecurityChecksCache(
      int SecurityChecksCacheMaxSize,
      TimeSpan securityCheckCacheExpiration,
      TimeSpan securityCheckCacheCleanupTaskInterval,
      TimeSpan securityChecksCacheInactivityExpiration)
      : base(SecurityChecksCacheMaxSize, securityCheckCacheExpiration, securityCheckCacheCleanupTaskInterval, securityChecksCacheInactivityExpiration)
    {
    }
  }
}
