// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRefreshCacheDictionary`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssRefreshCacheDictionary<K, T>
  {
    private readonly VssMemoryCacheList<K, T> m_memoryCacheList;
    private readonly ConcurrentDictionary<K, byte> m_updatesInProgress = new ConcurrentDictionary<K, byte>();
    private readonly TimeSpan m_refreshInterval;
    private readonly Func<IVssRequestContext, K, T> m_refreshFunction;
    private readonly bool m_allowStaleValues;
    private readonly bool m_returnDefaultValueInInitialRefresh;
    private readonly bool m_cacheFailuresWithDefaultValue;
    private readonly T m_defaultValue;
    private readonly ITimeProvider m_timeProvider;
    private const string c_area = "Caching";
    private static readonly string c_layer = "VssRefreshCacheDictionary<" + typeof (T).Name + ">";

    public VssRefreshCacheDictionary(
      TimeSpan refreshInterval,
      Func<IVssRequestContext, K, T> refreshFunction,
      VssMemoryCacheList<K, T> memoryCacheList,
      bool allowStaleValues = false,
      bool returnDefaultValueInInitialRefresh = false,
      bool cacheFailuresWithDefaultValue = false,
      T defaultValue = null)
      : this(refreshInterval, refreshFunction, memoryCacheList, allowStaleValues, returnDefaultValueInInitialRefresh, cacheFailuresWithDefaultValue, defaultValue, (ITimeProvider) new DefaultTimeProvider())
    {
    }

    internal VssRefreshCacheDictionary(
      TimeSpan refreshInterval,
      Func<IVssRequestContext, K, T> refreshFunction,
      VssMemoryCacheList<K, T> memoryCacheList,
      bool allowStaleValues,
      bool returnDefaultValueInInitialRefresh,
      bool cacheFailuresWithDefaultValue,
      T defaultValue,
      ITimeProvider timeProvider)
    {
      this.m_refreshInterval = refreshInterval;
      this.m_refreshFunction = refreshFunction;
      this.m_memoryCacheList = memoryCacheList;
      this.m_allowStaleValues = allowStaleValues;
      this.m_timeProvider = timeProvider;
      this.m_returnDefaultValueInInitialRefresh = returnDefaultValueInInitialRefresh;
      this.m_cacheFailuresWithDefaultValue = cacheFailuresWithDefaultValue;
      this.m_defaultValue = defaultValue;
    }

    public T Get(IVssRequestContext requestContext, K key)
    {
      requestContext.TraceEnter(10009001, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, nameof (Get));
      try
      {
        T obj;
        DateTime modifiedOn;
        bool flag = this.m_memoryCacheList.TryGetValue(key, out obj, out modifiedOn, out DateTime _, false);
        if (!flag)
          modifiedOn = DateTime.MinValue;
        if (modifiedOn + this.m_refreshInterval <= this.m_timeProvider.Now)
        {
          if ((flag || this.m_returnDefaultValueInInitialRefresh) && !this.m_updatesInProgress.TryAdd(key, (byte) 1))
          {
            if (!flag && this.m_returnDefaultValueInInitialRefresh)
              return this.m_defaultValue;
            if (this.m_allowStaleValues)
            {
              requestContext.Trace(10009008, TraceLevel.Info, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, "Returning expired value since refresh is currently in progress, existingValue={0}, modifiedDate={1:o}", (object) obj, (object) modifiedOn);
              return obj;
            }
          }
          requestContext.Trace(10009004, TraceLevel.Info, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, "Refreshing cache, existingValue={0}, modifiedDate={1:o}", (object) obj, (object) modifiedOn);
          try
          {
            try
            {
              obj = this.m_refreshFunction(requestContext, key);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10009009, TraceLevel.Error, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, ex);
              if (this.m_cacheFailuresWithDefaultValue)
                obj = this.m_defaultValue;
              else
                throw;
            }
            requestContext.Trace(10009005, TraceLevel.Info, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, "Updating cache, newValue={0}, valueTime={1:o}", (object) obj, (object) modifiedOn);
            DateTime now = this.m_timeProvider.Now;
            this.m_memoryCacheList.Add(key, obj, now, true);
            requestContext.Trace(10009006, TraceLevel.Info, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, "Cache updated, newValue={0}, valueTime={1:o}", (object) obj, (object) now);
          }
          finally
          {
            this.m_updatesInProgress.TryRemove(key, out byte _);
          }
        }
        else
          requestContext.Trace(10009003, TraceLevel.Info, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, "Cache is up to date, existingValue={0}, valueTime={1:o}", (object) obj, (object) modifiedOn);
        return obj;
      }
      finally
      {
        requestContext.TraceLeave(10009002, "Caching", VssRefreshCacheDictionary<K, T>.c_layer, nameof (Get));
      }
    }
  }
}
