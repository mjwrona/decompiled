// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadLocalCacheContainer`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadLocalCacheContainer<T> where T : AadCacheObject
  {
    private readonly int containerSize;
    private readonly int sampleSize;
    private readonly ConcurrentDictionary<AadCacheKey, T> dictionary;
    private readonly object writeLock = new object();
    private readonly Random random = new Random();
    private readonly AadCacheKey[] allKeys;

    internal AadLocalCacheContainer(int containerSize, int sampleSize)
    {
      if (containerSize <= 0)
        throw new ArgumentException(string.Format("Container size ({0}) must be a positive value.", (object) containerSize), nameof (containerSize));
      if (sampleSize <= 0)
        throw new ArgumentException(string.Format("Sample size ({0}) must be a positive value.", (object) sampleSize), nameof (sampleSize));
      this.containerSize = sampleSize <= containerSize ? containerSize : throw new ArgumentException(string.Format("Sample size ({0}) must be less than or equal to container size ({1}).", (object) sampleSize, (object) containerSize), nameof (sampleSize));
      this.sampleSize = sampleSize;
      this.dictionary = new ConcurrentDictionary<AadCacheKey, T>(Environment.ProcessorCount, containerSize);
      this.allKeys = new AadCacheKey[containerSize];
    }

    internal IEnumerable<AadCacheLookup<T>> GetObjects(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
    {
      AadCacheKey[] aadCacheKeyArray = keys != null ? keys.ToArray<AadCacheKey>() : Array.Empty<AadCacheKey>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.GetObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.GetObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.GetObjects.Objects").IncrementBy((long) aadCacheKeyArray.Length);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.GetObjects.ObjectsPerSecond").IncrementBy((long) aadCacheKeyArray.Length);
      AadCacheLookup<T>[] objects = new AadCacheLookup<T>[aadCacheKeyArray.Length];
      for (int index = 0; index < aadCacheKeyArray.Length; ++index)
      {
        AadCacheKey key = aadCacheKeyArray[index];
        if (key == null)
        {
          if (context == null || !context.ServiceHost.IsProduction)
            throw new AadException("Attempt to get object with null key.");
          context.Trace(19191901, TraceLevel.Warning, "VisualStudio.Services.Aad", "Cache", "Attempt to get object with null key.");
          objects[index] = new AadCacheLookup<T>(key, AadCacheLookupStatus.Miss, (Exception) null);
        }
        else
        {
          T result;
          objects[index] = this.dictionary.TryGetValue(key, out result) ? new AadCacheLookup<T>(key, result) : new AadCacheLookup<T>(key, AadCacheLookupStatus.Miss, (Exception) null);
        }
      }
      return (IEnumerable<AadCacheLookup<T>>) objects;
    }

    internal void AddObjects(IVssRequestContext context, IEnumerable<T> objects)
    {
      T[] objArray = objects != null ? objects.ToArray<T>() : Array.Empty<T>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.AddObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.AddObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.AddObjects.Objects").IncrementBy((long) objArray.Length);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.AddObjects.ObjectsPerSecond").IncrementBy((long) objArray.Length);
      for (int index1 = 0; index1 < objArray.Length; ++index1)
      {
        T newValue = objArray[index1];
        if ((object) newValue == null)
        {
          if (context == null || !context.ServiceHost.IsProduction)
            throw new AadException("Attempt to add null object.");
          context.Trace(19191902, TraceLevel.Warning, "VisualStudio.Services.Aad", "Cache", "Attempt to add null object.");
        }
        else if (newValue.Key == null)
        {
          if (context == null || !context.ServiceHost.IsProduction)
            throw new AadException("Attempt to add object with null key.");
          context.Trace(19191903, TraceLevel.Warning, "VisualStudio.Services.Aad", "Cache", "Attempt to add object with null key.");
        }
        else
        {
          T comparisonValue;
          if (this.dictionary.TryGetValue(newValue.Key, out comparisonValue))
          {
            this.dictionary.TryUpdate(newValue.Key, newValue, comparisonValue);
          }
          else
          {
            lock (this.writeLock)
            {
              int count = this.dictionary.Count;
              int index2;
              if (count < this.containerSize)
              {
                index2 = count;
              }
              else
              {
                int num = -1;
                T obj1 = default (T);
                for (int index3 = 0; index3 < this.sampleSize; ++index3)
                {
                  int index4 = this.random.Next(this.containerSize);
                  T obj2 = this.dictionary[this.allKeys[index4]];
                  if ((object) obj1 == null || obj2.Time < obj1.Time)
                  {
                    num = index4;
                    obj1 = obj2;
                  }
                }
                if (!this.dictionary.TryRemove(obj1.Key, out comparisonValue))
                {
                  if (context != null)
                  {
                    if (context.ServiceHost.IsProduction)
                      continue;
                  }
                  throw new AadInternalException("Failed to evict key: " + obj1.Key?.ToString());
                }
                index2 = num;
              }
              if (this.dictionary.TryAdd(newValue.Key, newValue))
                this.allKeys[index2] = newValue.Key;
            }
          }
        }
      }
    }

    internal void RemoveObjects(IVssRequestContext context, IEnumerable<AadCacheKey> keys)
    {
      AadCacheKey[] aadCacheKeyArray = keys != null ? keys.ToArray<AadCacheKey>() : Array.Empty<AadCacheKey>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.RemoveObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.RemoveObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.RemoveObjects.Objects").IncrementBy((long) aadCacheKeyArray.Length);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadLocalCache.RemoveObjects.ObjectsPerSecond").IncrementBy((long) aadCacheKeyArray.Length);
      for (int index = 0; index < aadCacheKeyArray.Length; ++index)
      {
        AadCacheKey key = aadCacheKeyArray[index];
        if (key == null)
        {
          if (context == null || !context.ServiceHost.IsProduction)
            throw new AadException("Attempt to remove object with null key.");
          context.Trace(19191904, TraceLevel.Warning, "VisualStudio.Services.Aad", "Cache", "Attempt to remove object with null key.");
        }
        else
        {
          lock (this.writeLock)
          {
            if (!this.dictionary.TryRemove(key, out T _))
              break;
            for (int destinationIndex = 0; destinationIndex < this.allKeys.Length; ++destinationIndex)
            {
              if (this.allKeys[destinationIndex] == key)
              {
                Array.Copy((Array) this.allKeys, destinationIndex + 1, (Array) this.allKeys, destinationIndex, this.dictionary.Count - destinationIndex);
                break;
              }
            }
          }
        }
      }
    }
  }
}
