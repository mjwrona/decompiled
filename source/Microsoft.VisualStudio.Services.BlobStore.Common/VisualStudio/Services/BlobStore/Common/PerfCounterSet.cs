// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PerfCounterSet
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal class PerfCounterSet : IDisposable
  {
    private Dictionary<int, PerfCounter> countersById;
    private Dictionary<string, PerfCounter> countersByName;

    internal PerfCounterSet(Guid guid, string name, CounterSetInstanceType type)
    {
      if (guid == Guid.Empty)
        throw new ArgumentException("Expected a non-zero Guid");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      this.Guid = guid;
      this.Name = name;
      this.InstanceType = type;
      this.countersById = new Dictionary<int, PerfCounter>();
      this.countersByName = new Dictionary<string, PerfCounter>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    internal Guid Guid { get; private set; }

    internal string Name { get; private set; }

    internal CounterSet Set { get; private set; }

    internal CounterSetInstance Instance { get; private set; }

    internal CounterSetInstanceType InstanceType { get; private set; }

    public void Dispose()
    {
      if (this.Instance != null)
      {
        this.Instance.Dispose();
        this.Instance = (CounterSetInstance) null;
      }
      if (this.Set == null)
        return;
      this.Set.Dispose();
      this.Set = (CounterSet) null;
    }

    internal void RegisterSet(Guid providerGuid) => this.Set = new CounterSet(providerGuid, this.Guid, this.InstanceType);

    internal void CreateSetInstance()
    {
      if (this.Set == null)
        throw new InvalidOperationException(string.Format("Counter set \"{0}\" hasn't been registered yet. Did you call RegisterSet()?", (object) this.Name));
      this.Instance = this.Instance == null ? this.Set.CreateCounterSetInstance(this.Name) : throw new InvalidOperationException(string.Format("An instance of counter set \"{0}\" has already been created.", (object) this.Name));
    }

    internal void Add(PerfCounter counter, bool addRealCounter = true)
    {
      if (this.countersById.ContainsKey(counter.Id))
        throw new ArgumentException(string.Format("Counter set \"{0}\" already contains counter ID {1}.", (object) this.Name, (object) counter.Id));
      if (this.countersByName.ContainsKey(counter.Name))
        throw new ArgumentException(string.Format("Counter set \"{0}\" already contains counter ID {1}.", (object) this.Name, (object) counter.Id));
      if (this.Instance != null)
        throw new InvalidOperationException(string.Format("An instance of counter set \"{0}\" has already been created. Therefore, no more counters may be added.", (object) this.Name));
      if (addRealCounter)
      {
        if (this.Set == null)
          throw new InvalidOperationException(string.Format("Counter set \"{0}\" hasn't been registered yet. Did you call RegisterSet()?", (object) this.Name));
        this.Set.AddCounter(counter.Id, counter.Type, counter.Name);
      }
      this.countersById.Add(counter.Id, counter);
      this.countersByName.Add(counter.Name, counter);
    }

    internal PerfCounter GetCounter(int counterId) => this.countersById.ContainsKey(counterId) ? this.countersById[counterId] : throw new ArgumentException(string.Format("Counter ID \"{0}\" was not found in set \"{1}\" which contains {2} counters.", (object) counterId, (object) this.Name, (object) this.countersByName.Count));

    internal PerfCounter GetCounter(string counterName) => this.countersByName.ContainsKey(counterName) ? this.countersByName[counterName] : throw new ArgumentException(string.Format("Counter \"{0}\" was not found in set \"{1}\" which contains {2} counters.", (object) counterName, (object) this.Name, (object) this.countersByName.Count));

    internal IEnumerable<PerfCounter> GetCounters() => (IEnumerable<PerfCounter>) this.countersByName.Values;
  }
}
