// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheTeamFoundationFileServiceAsCacheContainer`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V> : 
    IVssFrameworkService,
    IMutableDictionaryCacheContainer<K, V>,
    IDictionaryCacheContainer<K, V>,
    ICacheContainer<K, V>,
    IMutableCacheContainer<K, V>
  {
    private ContainerSettings m_settings;
    private string m_cacheId = "IMSRemoteCache_for_namespaceId";
    private const string AreaName = "ImsCacheTeamFoundationFileServiceAsCacheContainer";
    private const string CacheLayer = "TeamFoundationFileServiceAsImsRemoteCache";
    private static readonly HashSet<Type> m_typesForBlobStore = new HashSet<Type>()
    {
      typeof (ImsCacheDisplayNameSearchIndex),
      typeof (ImsCacheAccountNameSearchIndex),
      typeof (ImsCacheEmailSearchIndex),
      typeof (ImsCacheDomainAccountNameSearchIndex),
      typeof (ImsCacheAppIdSearchIndex)
    };

    public void ReloadSettings(ContainerSettings settings) => this.m_settings = settings;

    public IMutableCacheContainer<K, V> AsMutable() => (IMutableCacheContainer<K, V>) this;

    public IEnumerable<V> Get(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
    {
      requestContext.TraceEnter(9000000, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Get));
      ArgumentUtility.CheckForNull<Func<IEnumerable<K>, IDictionary<K, V>>>(onCacheMiss, nameof (onCacheMiss));
      IList<KeyValuePair<V, bool>> values;
      HashSet<K> missedKeys;
      this.Get(requestContext, keys, out values, out missedKeys);
      if (missedKeys.Any<K>())
      {
        IDictionary<K, V> valuesFromSource = onCacheMiss((IEnumerable<K>) missedKeys);
        try
        {
          this.Set(requestContext, valuesFromSource);
        }
        catch (Exception ex)
        {
          requestContext.TraceCatch(9000001, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", ex);
        }
        IEnumerable<V> vs = keys.Zip<K, KeyValuePair<V, bool>, V>((IEnumerable<KeyValuePair<V, bool>>) values, (Func<K, KeyValuePair<V, bool>, V>) ((k, kvp) =>
        {
          if (kvp.Value)
            return kvp.Key;
          V v;
          return valuesFromSource.TryGetValue(k, out v) ? v : default (V);
        }));
        requestContext.TraceLeave(9000003, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Get));
        return vs;
      }
      requestContext.TraceLeave(9000006, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Get));
      return values.Select<KeyValuePair<V, bool>, V>((Func<KeyValuePair<V, bool>, V>) (x => x.Key));
    }

    public string GetCacheId() => this.m_cacheId;

    public IEnumerable<V> IncrementBy(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<K, V>> items)
    {
      requestContext.TraceEnter(9000010, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (IncrementBy));
      throw new NotImplementedException();
    }

    public virtual void Invalidate(IVssRequestContext requestContext, IEnumerable<K> keys)
    {
      string instanceName = nameof (Invalidate);
      requestContext.TraceEnter(9000020, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Invalidate));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        foreach (K key in keys)
        {
          if (key is ImsCacheIdKey imsCacheIdKey)
          {
            using (VssRequestContextHolder collection = requestContext.ToCollection(imsCacheIdKey.Id))
            {
              ITeamFoundationFileService service = collection.RequestContext.GetService<ITeamFoundationFileService>();
              string cacheKey = this.GetCacheKey(key);
              IVssRequestContext requestContext1 = collection.RequestContext;
              string[] fileNames = new string[1]{ cacheKey };
              service.DeleteNamedFiles(requestContext1, OwnerId.ImsRemoteCache, (IEnumerable<string>) fileNames);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(9000021, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", ex);
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTime", instanceName).IncrementTicks(stopwatch);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTimeBase", instanceName).Increment();
      }
      requestContext.TraceLeave(9000022, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Invalidate));
    }

    public virtual bool Set(IVssRequestContext requestContext, IDictionary<K, V> items)
    {
      requestContext.TraceEnter(9000030, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Set));
      string instanceName = nameof (Set);
      Stopwatch stopwatch = Stopwatch.StartNew();
      bool flag;
      try
      {
        KeyValuePair<string, byte[]>[] serializedItems = items.Select<KeyValuePair<K, V>, KeyValuePair<string, byte[]>>((Func<KeyValuePair<K, V>, KeyValuePair<string, byte[]>>) (item => new KeyValuePair<string, byte[]>(this.GetCacheKey(item.Key), this.Serialize(item.Value)))).ToArray<KeyValuePair<string, byte[]>>();
        requestContext.TraceConditionally(9000031, TraceLevel.Info, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", (Func<string>) (() => string.Format("Set() by {0}, count={1}, totalSize={2}", (object) nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), (object) serializedItems.Length, (object) ((IEnumerable<KeyValuePair<string, byte[]>>) serializedItems).Sum<KeyValuePair<string, byte[]>>((Func<KeyValuePair<string, byte[]>, int>) (x =>
        {
          byte[] numArray = x.Value;
          return numArray == null ? 0 : numArray.Length;
        })))));
        List<K> list = items.Keys.ToList<K>();
        for (int index = 0; index < serializedItems.Length; ++index)
        {
          if (list[index] is ImsCacheIdKey imsCacheIdKey)
          {
            using (VssRequestContextHolder collection = requestContext.ToCollection(imsCacheIdKey.Id))
            {
              ITeamFoundationFileService service = collection.RequestContext.GetService<ITeamFoundationFileService>();
              service.DeleteNamedFiles(collection.RequestContext, OwnerId.ImsRemoteCache, (IEnumerable<string>) new string[1]
              {
                serializedItems[index].Key
              });
              service.UploadFile64(collection.RequestContext, (Stream) new MemoryStream(serializedItems[index].Value), OwnerId.ImsRemoteCache, Guid.Empty, serializedItems[index].Key);
            }
          }
        }
        flag = true;
      }
      catch (Exception ex)
      {
        flag = false;
        requestContext.TraceCatch(9000032, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", ex);
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTime", instanceName).IncrementTicks(stopwatch);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTimeBase", instanceName).Increment();
      }
      requestContext.TraceLeave(9000033, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (Set));
      return flag;
    }

    public IEnumerable<TimeSpan?> TimeToLive(IVssRequestContext requestContext, IEnumerable<K> keys)
    {
      requestContext.TraceEnter(9000050, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", nameof (TimeToLive));
      throw new NotImplementedException();
    }

    private void Get(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      out IList<KeyValuePair<V, bool>> values,
      out HashSet<K> missedKeys)
    {
      requestContext.Trace(9000040, TraceLevel.Info, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", "Get() by {0}, count={1}", (object) nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), (object) keys.Count<K>());
      string instanceName = nameof (Get);
      values = (IList<KeyValuePair<V, bool>>) new List<KeyValuePair<V, bool>>();
      missedKeys = new HashSet<K>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      foreach (K key in keys)
      {
        try
        {
          string cacheKey = this.GetCacheKey(key);
          if (key is ImsCacheIdKey imsCacheIdKey)
          {
            using (VssRequestContextHolder collection = requestContext.ToCollection(imsCacheIdKey.Id))
            {
              byte[] numArray;
              using (Stream stream = collection.RequestContext.GetService<ITeamFoundationFileService>().RetrieveNamedFile(collection.RequestContext, OwnerId.ImsRemoteCache, cacheKey, false, out byte[] _, out long _, out CompressionType _))
              {
                numArray = new byte[stream.Length];
                stream.Read(numArray, 0, (int) stream.Length);
              }
              if (numArray != null && numArray.Length != 0)
              {
                values.Add(new KeyValuePair<V, bool>(this.Deserialize(numArray), true));
              }
              else
              {
                missedKeys.Add(key);
                values.Add(new KeyValuePair<V, bool>(default (V), false));
              }
            }
          }
        }
        catch (Exception ex)
        {
          missedKeys.Add(key);
          values.Add(new KeyValuePair<V, bool>(default (V), false));
          requestContext.TraceCatch(9000041, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", ex);
        }
      }
      stopwatch.Stop();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTime", instanceName).IncrementTicks(stopwatch);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_TeamFoundationFileServiceAsCacheContainer_AverageCallTimeBase", instanceName).Increment();
      int valuesCount = values.Count;
      requestContext.TraceConditionally(9000042, TraceLevel.Info, nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), "TeamFoundationFileServiceAsImsRemoteCache", (Func<string>) (() => string.Format("EndTryGet() by {0}, totalSize={1}", (object) nameof (ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>), (object) valuesCount)));
    }

    public virtual IEnumerable<KeyValuePair<V, bool>> TryGet(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      IList<KeyValuePair<V, bool>> values;
      this.Get(requestContext, keys, out values, out HashSet<K> _);
      return (IEnumerable<KeyValuePair<V, bool>>) values;
    }

    private string GetCacheKey(string item) => string.Format("{0}_{1}_{2}", (object) this.m_cacheId, (object) item, (object) typeof (V));

    private string GetCacheKey(K key) => this.GetCacheKey(this.m_settings.KeySerializer.Serialize<K>(key));

    private byte[] Serialize(V value) => this.m_settings.ValueSerializer.Serialize<V>(value);

    private V Deserialize(byte[] data) => this.m_settings.ValueSerializer.Deserialize<V>(data);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!ImsCacheTeamFoundationFileServiceAsCacheContainer<K, V>.m_typesForBlobStore.Contains(typeof (V)))
        throw new ArgumentException("This type of cache container supports only these types: ImsCacheDisplayNameSearchIndex, ImsCacheAccountNameSearchIndex, ImsCacheEmailSearchIndex, ImsCacheDomainAccountNameSearchIndex, ImsCacheAppIdSearchIndex");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
