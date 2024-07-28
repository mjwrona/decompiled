// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerMemoryCache`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class PropertyContainerMemoryCache<T> : PropertyContainerCacheBase<T> where T : ICloneable, IPropertyContainer
  {
    private const string c_area = "Organization";
    private const string c_layer = "PropertyContainerMemoryCache";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (PropertyContainerMemoryCache<T>));

    internal override bool TryGet(
      IVssRequestContext context,
      Guid id,
      out PropertyContainerCacheEntry<T> cachedEntry)
    {
      using (PropertyContainerMemoryCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerMemoryCache<T>.TracePoints.TryGet.Slow, 50, nameof (TryGet)))
      {
        ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>> action = new ActionTracer.FunctionWithOutParam<bool, PropertyContainerCacheEntry<T>>(this.TryGet);
        return PropertyContainerMemoryCache<T>.s_tracer.TraceAction<bool, PropertyContainerCacheEntry<T>>(context, (ActionTracePoints) PropertyContainerMemoryCache<T>.TracePoints.TryGet, action, out cachedEntry, nameof (TryGet), (object) context, (object) id);
      }
    }

    private bool TryGet(out PropertyContainerCacheEntry<T> cachedEntry, params object[] args)
    {
      IVssRequestContext vssRequestContext = (IVssRequestContext) args[0];
      Guid guid = (Guid) args[1];
      if (!vssRequestContext.GetService<PropertyContainerMemoryCacheService<T>>().TryGetValue(vssRequestContext, guid, out cachedEntry))
      {
        PropertyContainerMemoryCache<T>.s_tracer.TraceCacheMiss(vssRequestContext, 6670012, (object) guid, nameof (TryGet));
        return false;
      }
      PropertyContainerMemoryCache<T>.s_tracer.TraceCacheHit(vssRequestContext, 6670013, (object) guid, nameof (TryGet));
      return true;
    }

    internal override void Set(
      IVssRequestContext context,
      Guid id,
      PropertyContainerCacheEntry<T> cacheEntry)
    {
      using (PropertyContainerMemoryCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerMemoryCache<T>.TracePoints.Set.Slow, 50, nameof (Set)))
        PropertyContainerMemoryCache<T>.s_tracer.TraceAction(context, (ActionTracePoints) PropertyContainerMemoryCache<T>.TracePoints.Set, (Action) (() => context.GetService<PropertyContainerMemoryCacheService<T>>().Set(context, id, cacheEntry)), nameof (Set));
    }

    public override bool Remove(IVssRequestContext context, Guid id)
    {
      using (PropertyContainerMemoryCache<T>.s_tracer.TraceTimedAction(context, PropertyContainerMemoryCache<T>.TracePoints.Remove.Slow, 50, nameof (Remove)))
        return PropertyContainerMemoryCache<T>.s_tracer.TraceAction<bool>(context, (ActionTracePoints) PropertyContainerMemoryCache<T>.TracePoints.Remove, (Func<bool>) (() =>
        {
          int num = context.GetService<PropertyContainerMemoryCacheService<T>>().Remove(context, id) ? 1 : 0;
          PropertyContainerMemoryCache<T>.s_tracer.TraceCacheInvalidation(context, 6670032, new Func<string>(((object) id).ToString), nameof (Remove));
          return num != 0;
        }), nameof (Remove));
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints TryGet = new TimedActionTracePoints(6670010, 6670017, 6670018, 6670019);
      internal static readonly TimedActionTracePoints Set = new TimedActionTracePoints(6670020, 6670027, 6670028, 6670029);
      internal static readonly TimedActionTracePoints Remove = new TimedActionTracePoints(6670030, 6670037, 6670038, 6670039);
    }

    private static class MethodNames
    {
      internal const string TryGet = "TryGet";
    }
  }
}
