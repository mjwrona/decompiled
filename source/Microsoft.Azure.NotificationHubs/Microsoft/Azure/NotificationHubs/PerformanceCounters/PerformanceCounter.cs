// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.PerformanceCounters.PerformanceCounter
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.PerformanceData;

namespace Microsoft.Azure.NotificationHubs.PerformanceCounters
{
  internal abstract class PerformanceCounter
  {
    public ClientPerformanceCounterScope Scope { get; protected set; }

    public abstract List<string> CounterNames { get; }

    public abstract int CounterStart { get; }

    public abstract int CounterEnd { get; }

    public abstract string InstanceName { get; }

    public bool Initialized { get; private set; }

    protected abstract CounterSetInstance CounterSetInstance { get; }

    protected abstract CounterData[] Counters { get; }

    internal static void ThrowInvalidOperationException() => throw new InvalidOperationException("Performance counter is already initialized.");

    protected void Initialize(ClientPerformanceCounterLevel compareLevel)
    {
      try
      {
        if (this.Scope.Level != compareLevel && this.Scope.Level != ClientPerformanceCounterLevel.All)
          return;
        this.OnInitialize();
        this.Initialized = this.CounterSetInstance != null && this.Counters != null;
        MessagingClientEtwProvider.TraceClient((Action) (() => { }));
      }
      catch (Exception ex)
      {
        if (!PerformanceCounter.IsCounterSetException(ex))
          throw;
        else
          MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWritePerformanceCounterCreationFailed(ex.ToString())));
      }
    }

    protected abstract void OnInitialize();

    protected virtual bool IsMinimalCountersEnabled() => this.Initialized;

    protected virtual bool IsVerboseCountersEnabled() => this.Scope.Detail == ClientPerformanceCounterDetail.Verbose && this.Initialized;

    private static bool IsCounterSetException(Exception e)
    {
      Type type = e.GetType();
      return type == typeof (ArgumentException) || type == typeof (InsufficientMemoryException) || type == typeof (Win32Exception) || type == typeof (PlatformNotSupportedException);
    }

    public Hashtable CollectCurrentValueSet()
    {
      if (!this.Initialized)
        throw new ObjectDisposedException(this.GetType().Name);
      CounterSetInstanceCounterDataSet counters = this.CounterSetInstance.Counters;
      Hashtable hashtable = new Hashtable();
      foreach (string counterName in this.CounterNames)
        hashtable.Add((object) counterName, (object) counters[counterName].RawValue);
      return hashtable;
    }
  }
}
