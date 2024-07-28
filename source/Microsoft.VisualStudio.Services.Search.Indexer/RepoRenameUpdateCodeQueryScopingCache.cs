// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.RepoRenameUpdateCodeQueryScopingCache
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class RepoRenameUpdateCodeQueryScopingCache : IQueryScopingCacheUpdater
  {
    private IQueryScopingCache m_queryScopingCache;

    internal RepoRenameUpdateCodeQueryScopingCache(IQueryScopingCache queryScopingCache) => this.m_queryScopingCache = queryScopingCache;

    public void UpdateQueryScopingCache(
      IVssRequestContext requestContext,
      QueryScopingCacheUpdateData changeData)
    {
      if (changeData == null)
        throw new ArgumentNullException(nameof (changeData));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081464, "Query Pipeline", "DocumentContractTypeService", nameof (UpdateQueryScopingCache));
      try
      {
        if (requestContext.IsFeatureEnabled("Search.Server.ScopedQuery"))
        {
          IDictionary<string, string> parentHierarchy = changeData.ParentHierarchy;
          string valueOrDefault = parentHierarchy != null ? parentHierarchy.GetValueOrDefault<string, string>("ProjectFilters") : (string) null;
          QueryingUnit chilQueryingUnit1 = this.m_queryScopingCache.GetCacheRoot().GetChilQueryingUnit(valueOrDefault);
          QueryingUnit queryingUnit1;
          if (chilQueryingUnit1 == null)
          {
            queryingUnit1 = (QueryingUnit) null;
          }
          else
          {
            IDictionary<string, QueryingUnit> childRoutingUnits = chilQueryingUnit1.ChildRoutingUnits;
            queryingUnit1 = childRoutingUnits != null ? childRoutingUnits.GetValueOrDefault<string, QueryingUnit>(changeData.OldEntityName) : (QueryingUnit) null;
          }
          QueryingUnit queryingUnit2 = queryingUnit1;
          if (queryingUnit2 != null && !string.IsNullOrEmpty(changeData.NewEntityName))
          {
            QueryingUnit queryingUnit3 = (QueryingUnit) queryingUnit2.Clone();
            queryingUnit3.EntityName = changeData.NewEntityName;
            QueryingUnit chilQueryingUnit2 = this.m_queryScopingCache.GetCacheRoot().GetChilQueryingUnit(valueOrDefault);
            chilQueryingUnit2.RemoveChildQueryingUnit(changeData.OldEntityName);
            chilQueryingUnit2.AddChildRoutingUnit(queryingUnit3);
          }
        }
        else
          this.m_queryScopingCache.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheDisabled;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Final Status of cache {0}", (object) this.m_queryScopingCache.QueryScopingCacheStatus.ToString())));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081465, "Query Pipeline", "DocumentContractTypeService", nameof (UpdateQueryScopingCache));
      }
    }
  }
}
