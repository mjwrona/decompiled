// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphIdentifierConversionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class GraphIdentifierConversionCacheService : 
    VssCacheBase,
    IGraphIdentifierConversionCacheService,
    IVssFrameworkService
  {
    private VssMemoryCacheList<Guid, IdentityKeyMap> identityKeyMapByStorageKey;
    private IVssMemoryCacheGrouping<Guid, IdentityKeyMap, Guid> identityKeyMapByCuid;
    private INotificationRegistration m_identityKeyRegistration;
    private const string Area = "Graph";
    private const string Layer = "GraphIdentifierConversionCacheService";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(GraphIdentifierConversionPerfCounters.StandardSet, "Graph", nameof (GraphIdentifierConversionCacheService));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.TraceEnter(10007050, "Graph", nameof (GraphIdentifierConversionCacheService), nameof (ServiceStart));
        this.identityKeyMapByStorageKey = new VssMemoryCacheList<Guid, IdentityKeyMap>((IVssCachePerformanceProvider) this, systemRequestContext.GetService<IGraphCachePolicyService>().GetCachePolicies(systemRequestContext).CacheSize);
        this.identityKeyMapByCuid = VssMemoryCacheGroupingFactory.Create<Guid, IdentityKeyMap, Guid>(systemRequestContext, this.identityKeyMapByStorageKey, (Func<Guid, IdentityKeyMap, IEnumerable<Guid>>) ((k, v) => (IEnumerable<Guid>) new Guid[1]
        {
          v.Cuid
        }), groupingBehavior: VssMemoryCacheGroupingBehavior.Replace);
        this.m_identityKeyRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.IdentityKeyMapChanged, new SqlNotificationCallback(this.OnIdentityKeyMapChangedNotification), false, false);
      }
      finally
      {
        systemRequestContext.TraceLeave(10007051, "Graph", nameof (GraphIdentifierConversionCacheService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      try
      {
        systemRequestContext.TraceEnter(10007060, "Graph", nameof (GraphIdentifierConversionCacheService), nameof (ServiceEnd));
        this.identityKeyMapByStorageKey.Clear();
        this.m_identityKeyRegistration.Unregister(systemRequestContext);
      }
      finally
      {
        systemRequestContext.TraceLeave(10007061, "Graph", nameof (GraphIdentifierConversionCacheService), nameof (ServiceEnd));
      }
    }

    public bool TryGetIdentityKeyMapByStorageKey(
      IVssRequestContext context,
      Guid storageKey,
      out IdentityKeyMap identityKeyMap)
    {
      int num = this.identityKeyMapByStorageKey.TryGetValue(storageKey, out identityKeyMap) ? 1 : 0;
      if (num != 0)
      {
        GraphIdentifierConversionCacheService.s_tracer.TraceCacheHit(context, 10007062, (object) storageKey, nameof (TryGetIdentityKeyMapByStorageKey));
        return num != 0;
      }
      GraphIdentifierConversionCacheService.s_tracer.TraceCacheMiss(context, 10007063, (object) storageKey, nameof (TryGetIdentityKeyMapByStorageKey));
      return num != 0;
    }

    public bool TryGetIdentityKeyMapByCuid(
      IVssRequestContext context,
      Guid cuid,
      out IdentityKeyMap identityKeyMap)
    {
      IEnumerable<Guid> keys;
      if (this.identityKeyMapByCuid.TryGetKeys(cuid, out keys))
      {
        Guid storageKey = keys.SingleOrDefault<Guid>();
        if (storageKey != Guid.Empty)
          return this.TryGetIdentityKeyMapByStorageKey(context, storageKey, out identityKeyMap);
      }
      identityKeyMap = (IdentityKeyMap) null;
      return false;
    }

    public void AddIdentityKeyMap(IVssRequestContext context, IdentityKeyMap identityKeyMap) => this.identityKeyMapByStorageKey.Add(identityKeyMap.StorageKey, identityKeyMap, true);

    public bool InvalidateKeyMapCacheByStorageKey(IVssRequestContext context, Guid storageKey) => this.identityKeyMapByStorageKey.Remove(storageKey);

    public bool InvalidateKeyMapCacheByCuid(IVssRequestContext requestContext, Guid cuid)
    {
      IdentityKeyMap identityKeyMap;
      return this.TryGetIdentityKeyMapByCuid(requestContext, cuid, out identityKeyMap) && this.InvalidateKeyMapCacheByStorageKey(requestContext, identityKeyMap.StorageKey);
    }

    private void OnIdentityKeyMapChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      IdentityKeyMapChangeData keyMapChangeData = TeamFoundationSerializationUtility.Deserialize<IdentityKeyMapChangeData>(eventData);
      if (keyMapChangeData == null)
        requestContext.Trace(10007082, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionCacheService), "Null storage key change data received");
      else if (keyMapChangeData.StorageKeyChangeType == IdentityKeyMapChangeType.None)
      {
        requestContext.Trace(10007082, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionCacheService), "Storage key change type received as None");
      }
      else
      {
        requestContext.Trace(10007081, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionCacheService), "Identity storage keys changed");
        if (keyMapChangeData.StorageKeyChangeType == IdentityKeyMapChangeType.Updated)
        {
          if (keyMapChangeData.KeyMapChanges == null)
            return;
          requestContext.Trace(10007083, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionCacheService), "InvalidateGraphMemberIdCache - new entries are being added.");
          foreach (IdentityKeyMapChange keyMapChange in keyMapChangeData.KeyMapChanges)
          {
            this.InvalidateKeyMapCacheByStorageKey(requestContext, keyMapChange.StorageKey);
            this.InvalidateKeyMapCacheByCuid(requestContext, keyMapChange.Cuid);
          }
        }
        else
        {
          if (keyMapChangeData.StorageKeyChangeType != IdentityKeyMapChangeType.BulkChange)
            return;
          requestContext.Trace(10007084, TraceLevel.Info, "Graph", nameof (GraphIdentifierConversionCacheService), "InvalidateGraphMemberIdCache - bulk change, clearing cache.");
          this.identityKeyMapByStorageKey.Clear();
        }
      }
    }
  }
}
