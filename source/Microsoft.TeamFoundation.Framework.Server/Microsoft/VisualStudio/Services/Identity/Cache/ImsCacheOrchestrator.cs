// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheOrchestrator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheOrchestrator : IImsCacheOrchestrator, IVssFrameworkService
  {
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsCacheOrchestrator";
    private static readonly ActionTracer s_tracer = new ActionTracer("Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheOrchestrator));

    public void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public IDictionary<K, V> GetObjects<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      IDictionary<K, V> localObjects = ImsCacheOrchestrator.GetObjectsFromLocalCache<K, V>(context, keys);
      List<K> list = keys.Where<K>((Func<K, bool>) (k => isCacheMiss(localObjects.TryGetOrDefault<K, V>(k)))).ToList<K>();
      if (!list.Any<K>())
        return localObjects;
      IDictionary<K, V> remoteObjects = this.GetObjectsFromRemoteCache<K, V>(context, (ICollection<K>) list);
      return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => ImsCacheObject.GetMostRecent<V>(localObjects.TryGetOrDefault<K, V>(key), remoteObjects.TryGetOrDefault<K, V>(key))));
    }

    public IDictionary<K, V> GetObjectsAndRefreshLocal<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      IDictionary<K, V> localObjects = ImsCacheOrchestrator.GetObjectsFromLocalCache<K, V>(context, keys);
      List<K> list = keys.Where<K>((Func<K, bool>) (k => isCacheMiss(localObjects.TryGetOrDefault<K, V>(k)))).ToList<K>();
      if (!list.Any<K>())
        return localObjects;
      IDictionary<K, V> remoteObjects = this.GetObjectsFromRemoteCache<K, V>(context, (ICollection<K>) list);
      IEnumerable<V> objects = list.Where<K>((Func<K, bool>) (key => ImsCacheObject.IsMoreRecent((ImsCacheObject) remoteObjects.TryGetOrDefault<K, V>(key), (ImsCacheObject) localObjects.TryGetOrDefault<K, V>(key)))).Select<K, V>((Func<K, V>) (key => remoteObjects.TryGetOrDefault<K, V>(key)));
      ImsCacheOrchestrator.AddObjectsToLocalCache<V>(context, objects);
      return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => ImsCacheObject.GetMostRecent<V>(localObjects.TryGetOrDefault<K, V>(key), remoteObjects.TryGetOrDefault<K, V>(key))));
    }

    public IDictionary<K, V> GetObjects<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<V, bool> isCacheMiss,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      IDictionary<K, V> localObjects = ImsCacheOrchestrator.GetObjectsFromLocalCache<K, V>(context, keys);
      List<K> list1 = keys.Where<K>((Func<K, bool>) (k => isCacheMiss(localObjects.TryGetOrDefault<K, V>(k)))).ToList<K>();
      if (!list1.Any<K>())
        return localObjects;
      IDictionary<K, V> remoteObjects = this.GetObjectsFromRemoteCache<K, V>(context, (ICollection<K>) list1);
      IEnumerable<V> objects1 = list1.Where<K>((Func<K, bool>) (key => ImsCacheObject.IsMoreRecent((ImsCacheObject) remoteObjects.TryGetOrDefault<K, V>(key), (ImsCacheObject) localObjects.TryGetOrDefault<K, V>(key)))).Select<K, V>((Func<K, V>) (key => remoteObjects.TryGetOrDefault<K, V>(key)));
      ImsCacheOrchestrator.AddObjectsToLocalCache<V>(context, objects1);
      List<K> list2 = list1.Where<K>((Func<K, bool>) (k => isCacheMiss(remoteObjects.TryGetOrDefault<K, V>(k)))).ToList<K>();
      if (!list2.Any<K>())
        return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => ImsCacheObject.GetMostRecent<V>(localObjects.TryGetOrDefault<K, V>(key), remoteObjects.TryGetOrDefault<K, V>(key))));
      IDictionary<K, V> sourceObjects = ImsCacheOrchestrator.GetObjectsFromSource<K, V>(context, (ICollection<K>) list2, onCacheMiss);
      IEnumerable<V> objects2 = list2.Where<K>((Func<K, bool>) (key => ImsCacheObject.IsMoreRecent((ImsCacheObject) sourceObjects.TryGetOrDefault<K, V>(key), (ImsCacheObject) localObjects.TryGetOrDefault<K, V>(key)))).Select<K, V>((Func<K, V>) (key => sourceObjects.TryGetOrDefault<K, V>(key)));
      ImsCacheOrchestrator.AddObjectsToLocalCache<V>(context, objects2);
      IEnumerable<V> objects3 = list2.Where<K>((Func<K, bool>) (key => ImsCacheObject.IsMoreRecent((ImsCacheObject) sourceObjects.TryGetOrDefault<K, V>(key), (ImsCacheObject) remoteObjects.TryGetOrDefault<K, V>(key)))).Select<K, V>((Func<K, V>) (key => sourceObjects.TryGetOrDefault<K, V>(key)));
      this.AddObjectsToRemoteCache<V>(context, objects3);
      return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => ImsCacheObject.GetMostRecent<V>(localObjects.TryGetOrDefault<K, V>(key), remoteObjects.TryGetOrDefault<K, V>(key), sourceObjects.TryGetOrDefault<K, V>(key))));
    }

    private static IDictionary<K, T> GetObjectsFromLocalCache<K, T>(
      IVssRequestContext context,
      ICollection<K> keys)
      where K : ImsCacheKey
      where T : ImsCacheObject
    {
      return ImsCacheOrchestrator.s_tracer.TraceAction<IDictionary<K, T>>(context, (ActionTracePoints) ImsCacheOrchestrator.TracePoints.GetObjectsFromLocalCache, (Func<IDictionary<K, T>>) (() =>
      {
        try
        {
          ImsCacheOrchestrator.s_tracer.Trace(context, 185410912, TraceLevel.Verbose, (Func<string>) (() => "ImsCacheOrchestrator.GetObjectsFromLocalCache where keys : " + keys.Serialize<ICollection<K>>()), nameof (GetObjectsFromLocalCache));
          return context.GetService<IImsLocalDataCache>().GetObjects<K, T>(context, keys);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          ImsCacheOrchestrator.s_tracer.TraceException(context, ImsCacheOrchestrator.TracePoints.GetObjectsFromLocalCache.Exception, ex, nameof (GetObjectsFromLocalCache));
          return ImsCacheOrchestrator.GenerateEmptyMap<K, T>((IEnumerable<K>) keys);
        }
      }), nameof (GetObjectsFromLocalCache));
    }

    private static void AddObjectsToLocalCache<T>(
      IVssRequestContext context,
      IEnumerable<T> objects)
      where T : ImsCacheObject
    {
      ImsCacheOrchestrator.s_tracer.TraceAction(context, (ActionTracePoints) ImsCacheOrchestrator.TracePoints.AddObjectsToLocalCache, (Action) (() =>
      {
        try
        {
          ImsCacheOrchestrator.s_tracer.Trace(context, 185410922, TraceLevel.Verbose, (Func<string>) (() => "ImsCacheOrchestrator.AddObjectsToLocalCache where objects : " + objects.Serialize<IEnumerable<T>>()), nameof (AddObjectsToLocalCache));
          context.GetService<IImsLocalDataCache>().AddObjects<T>(context, objects);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          ImsCacheOrchestrator.s_tracer.TraceException(context, 185410928, ex, nameof (AddObjectsToLocalCache));
        }
      }), nameof (AddObjectsToLocalCache));
    }

    private IDictionary<K, V> GetObjectsFromRemoteCache<K, V>(
      IVssRequestContext context,
      ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      return ImsCacheOrchestrator.s_tracer.TraceAction<IDictionary<K, V>>(context, (ActionTracePoints) ImsCacheOrchestrator.TracePoints.GetObjectsFromRemoteCache, (Func<IDictionary<K, V>>) (() =>
      {
        try
        {
          ImsCacheOrchestrator.s_tracer.Trace(context, 185410932, TraceLevel.Verbose, (Func<string>) (() => "ImsCacheOrchestrator.GetObjectsFromRemoteCache where keys : " + keys.Serialize<ICollection<K>>()), nameof (GetObjectsFromRemoteCache));
          return this.GetImsRemoteCache(context).GetObjects<K, V>(context, keys);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          ImsCacheOrchestrator.s_tracer.TraceException(context, ImsCacheOrchestrator.TracePoints.GetObjectsFromRemoteCache.Exception, ex, nameof (GetObjectsFromRemoteCache));
          return ImsCacheOrchestrator.GenerateEmptyMap<K, V>((IEnumerable<K>) keys);
        }
      }), nameof (GetObjectsFromRemoteCache));
    }

    private void AddObjectsToRemoteCache<T>(IVssRequestContext context, IEnumerable<T> objects) where T : ImsCacheObject => ImsCacheOrchestrator.s_tracer.TraceAction(context, (ActionTracePoints) ImsCacheOrchestrator.TracePoints.AddObjectsToRemoteCache, (Action) (() =>
    {
      try
      {
        ImsCacheOrchestrator.s_tracer.Trace(context, 185410942, TraceLevel.Verbose, (Func<string>) (() => "ImsCacheOrchestrator.AddObjectsToRemoteCache where objects : " + objects.Serialize<IEnumerable<T>>()), nameof (AddObjectsToRemoteCache));
        this.GetImsRemoteCache(context).AddObjects<T>(context, objects);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        ImsCacheOrchestrator.s_tracer.TraceException(context, ImsCacheOrchestrator.TracePoints.AddObjectsToRemoteCache.Exception, ex, nameof (AddObjectsToRemoteCache));
      }
    }), nameof (AddObjectsToRemoteCache));

    private static IDictionary<K, V> GetObjectsFromSource<K, V>(
      IVssRequestContext context,
      ICollection<K> keys,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      return ImsCacheOrchestrator.s_tracer.TraceAction<IDictionary<K, V>>(context, (ActionTracePoints) ImsCacheOrchestrator.TracePoints.GetObjectsFromSource, (Func<IDictionary<K, V>>) (() =>
      {
        try
        {
          ImsCacheOrchestrator.s_tracer.Trace(context, 185410952, TraceLevel.Verbose, (Func<string>) (() => "ImsCacheOrchestrator.GetObjectsFromSource where keys : " + keys.Serialize<ICollection<K>>()), nameof (GetObjectsFromSource));
          IDictionary<K, V> sourceObjectMap = onCacheMiss((IEnumerable<K>) keys);
          return (IDictionary<K, V>) ((IEnumerable<K>) keys).ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => sourceObjectMap.TryGetOrDefault<K, V>(key)));
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          ImsCacheOrchestrator.s_tracer.TraceException(context, ImsCacheOrchestrator.TracePoints.GetObjectsFromSource.Exception, ex, nameof (GetObjectsFromSource));
          return ImsCacheOrchestrator.GenerateEmptyMap<K, V>((IEnumerable<K>) keys);
        }
      }), nameof (GetObjectsFromSource));
    }

    private static IDictionary<K, V> GenerateEmptyMap<K, V>(IEnumerable<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => default (V)));
    }

    private IImsRemoteCache GetImsRemoteCache(IVssRequestContext context) => context.GetService<IImsRemoteCache>();

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints GetObjectsFromLocalCache = new TimedActionTracePoints(185410911, 185410917, 185410918, 185410919);
      internal static readonly TimedActionTracePoints AddObjectsToLocalCache = new TimedActionTracePoints(185410921, 185410927, 185410928, 185410929);
      internal static readonly TimedActionTracePoints GetObjectsFromRemoteCache = new TimedActionTracePoints(185410931, 185410937, 185410938, 185410939);
      internal static readonly TimedActionTracePoints AddObjectsToRemoteCache = new TimedActionTracePoints(185410941, 185410947, 185410948, 185410949);
      internal static readonly TimedActionTracePoints GetObjectsFromSource = new TimedActionTracePoints(185410951, 185410957, 185410958, 185410959);
    }
  }
}
