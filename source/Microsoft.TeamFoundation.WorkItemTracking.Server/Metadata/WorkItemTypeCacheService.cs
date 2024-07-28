// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTypeCacheService : VssMemoryCacheService<Guid, ProjectWorkItemTypes>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private const int c_maxCacheSize = 1019;

    public WorkItemTypeCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, ProjectWorkItemTypes>().WithMaxElements(1019).WithCleanupInterval(WorkItemTypeCacheService.s_cacheCleanupInterval))
    {
      this.InactivityInterval.Value = WorkItemTypeCacheService.s_maxCacheInactivityAge;
    }

    public void RemoveProjectWorkItemTypes(IVssRequestContext requestContext, Guid projectId) => this.Remove(requestContext, projectId);
  }
}
