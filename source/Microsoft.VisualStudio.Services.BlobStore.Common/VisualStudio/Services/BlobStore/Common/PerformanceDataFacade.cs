// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PerformanceDataFacade
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.PerformanceData;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal class PerformanceDataFacade : IPerformanceDataFacade, IDisposable
  {
    protected Dictionary<Guid, PerfCounterSet> setsByGuid;
    protected Dictionary<string, PerfCounterSet> setsByName;
    private static Dictionary<Guid, PerformanceDataFacade> cache = new Dictionary<Guid, PerformanceDataFacade>();
    private static object lockObj = new object();
    private Guid providerGuid;

    protected PerformanceDataFacade(Guid providerGuid)
    {
      this.providerGuid = providerGuid;
      this.setsByGuid = new Dictionary<Guid, PerfCounterSet>();
      this.setsByName = new Dictionary<string, PerfCounterSet>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool IsInitialized { get; private set; }

    public static PerformanceDataFacade GetExistingOrCreate(Guid providerGuid)
    {
      lock (PerformanceDataFacade.lockObj)
      {
        PerformanceDataFacade existingOrCreate;
        if (!PerformanceDataFacade.cache.TryGetValue(providerGuid, out existingOrCreate))
        {
          existingOrCreate = new PerformanceDataFacade(providerGuid);
          PerformanceDataFacade.cache.Add(providerGuid, existingOrCreate);
        }
        return existingOrCreate;
      }
    }

    public virtual void RegisterCounterSet(
      Guid counterSetGuid,
      string counterSetName,
      CounterSetInstanceType instanceType)
    {
      if (this.IsInitialized)
        throw new InvalidOperationException("Initialization phase is already complete");
      PerfCounterSet perfCounterSet = new PerfCounterSet(counterSetGuid, counterSetName, instanceType);
      this.setsByGuid.Add(counterSetGuid, perfCounterSet);
      this.setsByName.Add(counterSetName, perfCounterSet);
      perfCounterSet.RegisterSet(this.providerGuid);
    }

    public virtual void AddCounterToSet(
      Guid counterSetGuid,
      int counterId,
      CounterType counterType,
      string counterName,
      int baseId)
    {
      if (this.IsInitialized)
        throw new InvalidOperationException("Initialization phase is already complete");
      PerfCounterSet counterSet = this.GetCounterSet(counterSetGuid);
      counterSet.Add(new PerfCounter(counterSet, counterId, counterType, counterName, baseId));
    }

    public virtual void CreateCounterSetInstance(Guid counterSetGuid)
    {
      if (this.IsInitialized)
        throw new InvalidOperationException("Initialization phase is already complete");
      this.GetCounterSet(counterSetGuid).CreateSetInstance();
    }

    public void CompleteInitialization() => this.IsInitialized = true;

    public PerfCounter GetCounter(string counterSetName, string counterName)
    {
      if (!this.IsInitialized)
        throw new InvalidOperationException("Initialization phase is not complete yet");
      if (string.IsNullOrEmpty(counterSetName))
        throw new ArgumentNullException(nameof (counterSetName));
      if (!this.setsByName.ContainsKey(counterSetName))
        throw new ArgumentException(string.Format("CounterSet \"{0}\" was not found among {1} sets.", (object) counterSetName, (object) this.setsByName.Count));
      return this.setsByName[counterSetName].GetCounter(counterName);
    }

    public IReadOnlyCollection<PerfCounter> GetCounters()
    {
      if (!this.IsInitialized)
        throw new InvalidOperationException("Initialization phase is not complete yet");
      List<PerfCounter> list = this.setsByGuid.SelectMany<KeyValuePair<Guid, PerfCounterSet>, PerfCounter>((Func<KeyValuePair<Guid, PerfCounterSet>, IEnumerable<PerfCounter>>) (s => s.Value.GetCounters())).ToList<PerfCounter>();
      list.Sort();
      return (IReadOnlyCollection<PerfCounter>) new ReadOnlyCollection<PerfCounter>((IList<PerfCounter>) list);
    }

    public void Dispose()
    {
      lock (PerformanceDataFacade.lockObj)
      {
        PerformanceDataFacade.cache.Remove(this.providerGuid);
        foreach (PerfCounterSet perfCounterSet in this.setsByGuid.Values)
          perfCounterSet.Dispose();
      }
    }

    private PerfCounterSet GetCounterSet(Guid counterSetGuid) => this.setsByGuid.ContainsKey(counterSetGuid) ? this.setsByGuid[counterSetGuid] : throw new InvalidOperationException(string.Format("Counter set \"{0}\" not found. Did you RegisterCounterSet() it?", (object) counterSetGuid));
  }
}
