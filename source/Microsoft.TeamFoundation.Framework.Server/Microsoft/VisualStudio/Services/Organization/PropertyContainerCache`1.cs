// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerCache`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class PropertyContainerCache<T> : PropertyContainerCacheBase<T> where T : ICloneable, IPropertyContainer
  {
    private readonly PropertyContainerMemoryCache<T> m_memoryCache;
    private readonly PropertyContainerRemoteCache<T> m_remoteCache;
    private const string c_area = "Organization";
    private const string c_layer = "PropertyContainerCache";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (PropertyContainerCache<T>));

    public PropertyContainerCache()
    {
      this.m_memoryCache = new PropertyContainerMemoryCache<T>();
      this.m_remoteCache = new PropertyContainerRemoteCache<T>();
    }

    internal override bool TryGet(
      IVssRequestContext context,
      Guid id,
      out PropertyContainerCacheEntry<T> cachedEntry)
    {
      using (PropertyContainerCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerCache<T>.TracePoints.TryGet.Slow, 500, nameof (TryGet)))
      {
        ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>> action = new ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>>(this.TryGet);
        return PropertyContainerCache<T>.s_tracer.TraceAction<bool, PropertyContainerCacheEntry<T>>(context, (ActionTracePoints) PropertyContainerCache<T>.TracePoints.TryGet, action, out cachedEntry, nameof (TryGet), (object) context, (object) id);
      }
    }

    private bool TryGet(out PropertyContainerCacheEntry<T> cachedEntry, params object[] args)
    {
      IVssRequestContext vssRequestContext = (IVssRequestContext) args[0];
      Guid guid = (Guid) args[1];
      if (this.m_memoryCache.TryGet(vssRequestContext, guid, out cachedEntry))
      {
        PropertyContainerCache<T>.s_tracer.TraceCacheHit(vssRequestContext, 6650012, (object) guid, "TryGet-Local");
        PropertyContainerCache<T>.s_tracer.TraceCacheHit(vssRequestContext, 6650013, (object) guid, nameof (TryGet));
        return true;
      }
      if (this.m_remoteCache.TryGet(vssRequestContext, guid, out cachedEntry))
      {
        PropertyContainerCache<T>.s_tracer.TraceCacheHit(vssRequestContext, 6650014, (object) guid, "TryGet-Remote");
        PropertyContainerCache<T>.s_tracer.TraceCacheHit(vssRequestContext, 6650015, (object) guid, nameof (TryGet));
        this.m_memoryCache.Set(vssRequestContext, guid, cachedEntry);
        return true;
      }
      PropertyContainerCache<T>.s_tracer.TraceCacheMiss(vssRequestContext, 6650016, (object) guid, nameof (TryGet));
      return false;
    }

    internal override void Set(
      IVssRequestContext context,
      Guid id,
      PropertyContainerCacheEntry<T> cacheEntry)
    {
      using (PropertyContainerCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerCache<T>.TracePoints.Set.Slow, 200, nameof (Set)))
        PropertyContainerCache<T>.s_tracer.TraceAction(context, (ActionTracePoints) PropertyContainerCache<T>.TracePoints.Set, (Action) (() =>
        {
          this.m_remoteCache.Set(context, id, cacheEntry);
          this.m_memoryCache.Set(context, id, cacheEntry);
        }), nameof (Set));
    }

    public override bool Remove(IVssRequestContext context, Guid id)
    {
      using (PropertyContainerCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerCache<T>.TracePoints.Remove.Slow, 200, nameof (Remove)))
        return PropertyContainerCache<T>.s_tracer.TraceAction<bool>(context, (ActionTracePoints) PropertyContainerCache<T>.TracePoints.Remove, (Func<bool>) (() =>
        {
          int num = this.m_remoteCache.Remove(context, id) | this.m_memoryCache.Remove(context, id) ? 1 : 0;
          PropertyContainerCache<T>.s_tracer.TraceCacheInvalidation(context, 6650032, new Func<string>(((object) id).ToString), nameof (Remove));
          return num != 0;
        }), nameof (Remove));
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints TryGet = new TimedActionTracePoints(6650010, 6650017, 6650018, 6650019);
      internal static readonly TimedActionTracePoints Set = new TimedActionTracePoints(6650020, 6650027, 6650028, 6650029);
      internal static readonly TimedActionTracePoints Remove = new TimedActionTracePoints(6650030, 6650037, 6650038, 6650039);
    }

    private static class MethodNames
    {
      internal const string TryGet = "TryGet";
      internal const string TryGetLocal = "TryGet-Local";
      internal const string TryGetRemote = "TryGet-Remote";
    }
  }
}
