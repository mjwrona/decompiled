// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IQueryOptimizationCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (QueryOptimizationCacheService))]
  public interface IQueryOptimizationCacheService : IVssFrameworkService
  {
    QueryOptimizationInstance GetQueryOptimizationInstance(
      IVssRequestContext requestContext,
      Guid? queryId,
      string queryhash,
      QueryOptimizationStrategy strategyToMatchForFuzzMatchOnId,
      bool exactMatch = false);

    void AddOrUpdate(IVssRequestContext requestContext, QueryOptimizationInstance instance);

    void AddOrUpdateFromDB(
      IVssRequestContext requestContext,
      IEnumerable<QueryOptimizationInstance> queryOptimizationInstances);

    DateTime MostRecentUpdatedFromDBTime { get; }
  }
}
