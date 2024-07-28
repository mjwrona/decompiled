// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.IRecentActivityCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public interface IRecentActivityCacheService : IVssFrameworkService
  {
    RecentActivityDetails GetRecentActivitiesFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid userId,
      bool isSearchRequest);

    void UpdateRecentActivitiesInCache(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId,
      DateTime activityDate,
      int artifactId,
      int areaId);
  }
}
