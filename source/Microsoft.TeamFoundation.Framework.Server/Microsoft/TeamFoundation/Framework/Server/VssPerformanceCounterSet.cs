// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPerformanceCounterSet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.PerformanceData;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssPerformanceCounterSet
  {
    private readonly string _counterSetName;
    private readonly bool _isMultiInstance;
    private readonly object _lockObj = new object();
    private readonly ConcurrentDictionary<VssPerformanceCounterSetKey, CounterSetInstance> _instances = new ConcurrentDictionary<VssPerformanceCounterSetKey, CounterSetInstance>(VssPerformanceCounterSetKey.Comparer);
    private readonly CounterSet _counterSet;
    private readonly ImmutableArray<VssPerformanceCounterInfo> _counters;
    private CounterSetInstance _defaultInstance;

    public VssPerformanceCounterSet(
      Guid providerId,
      Guid counterSetId,
      string counterSetName,
      IReadOnlyCollection<VssPerformanceCounterInfo> counters,
      bool isMultiInstance)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      ArgumentUtility.CheckForEmptyGuid(counterSetId, nameof (counterSetId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(counterSetName, nameof (counterSetName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) counters, nameof (counters));
      this._counterSetName = counterSetName;
      this._isMultiInstance = isMultiInstance;
      this._counters = counters.ToImmutableArray<VssPerformanceCounterInfo>();
      this._counterSet = new CounterSet(providerId, counterSetId, this._isMultiInstance ? CounterSetInstanceType.MultipleAggregate : CounterSetInstanceType.GlobalAggregate);
      foreach (VssPerformanceCounterInfo counter in this._counters)
      {
        try
        {
          this._counterSet.AddCounter(counter.Id, counter.Type, counter.Name);
        }
        catch (ArgumentException ex)
        {
        }
      }
      if (this._isMultiInstance)
        return;
      this.RegisterInstance(new VssPerformanceCounterSetKey(this._counterSetName, (string) null));
    }

    public bool IsMultiInstance => this._isMultiInstance;

    public VssPerformanceCounter this[int counterId]
    {
      get
      {
        if (this._defaultInstance == null)
          this._defaultInstance = this._instances.Single<KeyValuePair<VssPerformanceCounterSetKey, CounterSetInstance>>().Value;
        CounterData counter = this._defaultInstance.Counters[counterId];
        if (counter != null)
          return new VssPerformanceCounter(counter);
        if (VssPerformanceCounterManager.ThrowExceptions())
          throw new Exception(string.Format("Counter set '{0}' does not contain a counter with id {1}.", (object) this._counterSetName, (object) counterId));
        return new VssPerformanceCounter();
      }
    }

    public VssPerformanceCounter this[int counterId, string instanceName, string processName]
    {
      get
      {
        VssPerformanceCounterSetKey key = new VssPerformanceCounterSetKey(instanceName, processName);
        CounterSetInstance counterSetInstance;
        if (this._instances.TryGetValue(key, out counterSetInstance))
          return new VssPerformanceCounter(counterSetInstance.Counters[counterId]);
        this.RegisterInstance(key);
        return this[counterId, instanceName, processName];
      }
    }

    private void RegisterInstance(VssPerformanceCounterSetKey key)
    {
      CounterSetInstance counterSetInstance;
      if (this._instances.TryGetValue(key, out counterSetInstance))
        return;
      lock (this._lockObj)
      {
        if (this._instances.TryGetValue(key, out counterSetInstance))
          return;
        string qualifiedInstanceName = key.GetQualifiedInstanceName();
        counterSetInstance = this._counterSet.CreateCounterSetInstance(qualifiedInstanceName);
        foreach (VssPerformanceCounterInfo counter in this._counters)
          counterSetInstance.Counters[counter.Name].Value = 0L;
        if (!this._instances.TryAdd(key, counterSetInstance) && VssPerformanceCounterManager.ThrowExceptions())
          throw new Exception("Could not add CounterSetInstance '" + qualifiedInstanceName + "' to the collection. It may already exist.");
      }
    }
  }
}
