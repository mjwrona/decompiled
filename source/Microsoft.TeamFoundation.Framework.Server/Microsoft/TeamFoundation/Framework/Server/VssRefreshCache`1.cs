// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRefreshCache`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssRefreshCache<T> where T : class
  {
    private VssRefreshCache<T>.ValueContainer m_instance;
    private readonly TimeSpan m_refreshInterval;
    private readonly Func<IVssRequestContext, T> m_refreshFunction;
    private readonly bool m_allowStaleValues;
    private readonly ITimeProvider m_timeProvider;
    private const string c_area = "Caching";
    private static readonly string c_layer = "VssRefreshCache<" + typeof (T).Name + ">";

    public VssRefreshCache(
      TimeSpan refreshInterval,
      Func<IVssRequestContext, T> refreshFunction,
      bool allowStaleValues = false)
      : this(refreshInterval, refreshFunction, allowStaleValues, (ITimeProvider) new DefaultTimeProvider())
    {
    }

    internal VssRefreshCache(
      TimeSpan refreshInterval,
      Func<IVssRequestContext, T> refreshFunction,
      bool allowStaleValues,
      ITimeProvider timeProvider)
    {
      this.m_refreshInterval = refreshInterval;
      this.m_refreshFunction = refreshFunction;
      this.m_allowStaleValues = allowStaleValues;
      this.m_timeProvider = timeProvider;
    }

    public T Get(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(10007001, "Caching", VssRefreshCache<T>.c_layer, nameof (Get));
      try
      {
        VssRefreshCache<T>.ValueContainer instance = this.m_instance;
        T obj1 = instance != null ? instance.Value : default (T);
        DateTime dateTime = instance != null ? instance.Timestamp : DateTime.MinValue;
        if (dateTime + this.m_refreshInterval <= this.m_timeProvider.Now)
        {
          if (instance != null && Interlocked.CompareExchange(ref instance.RefreshInProgress, 1, 0) != 0 && this.m_allowStaleValues)
          {
            requestContext.Trace(10007008, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Returning expired value since refresh is currently in progress, existingValue={0}, valueTime={1:o}", (object) obj1, (object) dateTime);
            return obj1;
          }
          requestContext.Trace(10007004, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Refreshing cache, existingValue={0}, valueTime={1:o}", (object) obj1, (object) dateTime);
          T obj2;
          DateTime now;
          try
          {
            obj2 = this.m_refreshFunction(requestContext);
            now = this.m_timeProvider.Now;
          }
          catch (Exception ex)
          {
            if (instance != null)
              instance.RefreshInProgress = 0;
            requestContext.TraceException(10007009, TraceLevel.Error, "Caching", VssRefreshCache<T>.c_layer, ex);
            throw;
          }
          requestContext.Trace(10007005, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Updating cache, newValue={0}, valueTime={1:o}", (object) obj2, (object) now);
          VssRefreshCache<T>.ValueContainer source = new VssRefreshCache<T>.ValueContainer(obj2, now);
          VssRefreshCache<T>.ValueContainer valueContainer = this.InterlockedCompareExchangeGreater(ref this.m_instance, source);
          obj1 = valueContainer.Value;
          DateTime timestamp = valueContainer.Timestamp;
          if (source == valueContainer)
            requestContext.Trace(10007006, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Cache updated, newValue={0}, valueTime={1:o}", (object) obj1, (object) timestamp);
          else
            requestContext.Trace(10007007, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Cache preserved, existingValue={0}, valueTime={1:o}", (object) obj1, (object) timestamp);
        }
        else
          requestContext.Trace(10007003, TraceLevel.Info, "Caching", VssRefreshCache<T>.c_layer, "Cache is up to date, existingValue={0}, valueTime={1:o}", (object) obj1, (object) dateTime);
        return obj1;
      }
      finally
      {
        requestContext.TraceLeave(10007002, "Caching", VssRefreshCache<T>.c_layer, nameof (Get));
      }
    }

    private VssRefreshCache<T>.ValueContainer InterlockedCompareExchangeGreater(
      ref VssRefreshCache<T>.ValueContainer target,
      VssRefreshCache<T>.ValueContainer source)
    {
      VssRefreshCache<T>.ValueContainer comparand = target;
      if (comparand != null && !(source.Timestamp > comparand.Timestamp))
        return comparand;
      VssRefreshCache<T>.ValueContainer valueContainer = Interlocked.CompareExchange<VssRefreshCache<T>.ValueContainer>(ref target, source, comparand);
      return valueContainer == comparand ? source : valueContainer;
    }

    private class ValueContainer
    {
      public int RefreshInProgress;

      public ValueContainer(T value, DateTime timestamp)
      {
        this.Value = value;
        this.Timestamp = timestamp;
      }

      public T Value { get; }

      public DateTime Timestamp { get; }
    }
  }
}
