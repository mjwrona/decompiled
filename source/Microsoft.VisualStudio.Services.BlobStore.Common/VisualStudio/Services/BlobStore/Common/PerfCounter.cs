// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PerfCounter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class PerfCounter : IComparable
  {
    internal PerfCounter(PerfCounterSet set, int id, CounterType type, string name, int baseId)
    {
      if (set == null)
        throw new ArgumentNullException(nameof (set));
      if (id <= 0)
        throw new ArgumentException("Expected a positive id", nameof (id));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (baseId < 0)
        throw new ArgumentException("Expected non-negative base id", nameof (baseId));
      this.Set = set;
      this.Id = id;
      this.Type = type;
      this.Name = name;
      this.BaseId = baseId;
    }

    public Guid SetGuid => this.Set.Guid;

    public string SetName => this.Set.Name;

    public CounterSetInstanceType SetInstanceType => this.Set.InstanceType;

    public int Id { get; private set; }

    public CounterType Type { get; private set; }

    public string Name { get; private set; }

    public int BaseId { get; private set; }

    public virtual long RawValue => this.Data.RawValue;

    internal PerfCounterSet Set { get; private set; }

    internal CounterSetInstance SetInstance => this.Set.Instance;

    internal PerfCounter Base
    {
      get
      {
        PerfCounter perfCounter = this.BaseId > 0 ? this.Set.GetCounter(this.BaseId) : throw new InvalidOperationException("Counter \"{0}\" does not have an associated base counter.");
        return this.Type != CounterType.AverageTimer32 || perfCounter.Type == CounterType.AverageBase ? perfCounter : throw new InvalidOperationException(string.Format("Expected counter ID {0} to be an AverageBase per baseID on counter ID {1} in set \"{2}\", but it's {3}.", (object) perfCounter.Id, (object) this.Id, (object) this.Set.Name, (object) perfCounter.Type.ToString()));
      }
    }

    internal virtual CounterData Data
    {
      get
      {
        if (this.SetInstance == null)
          throw new ObjectDisposedException(string.Format("CounterSetInstance ID {0}, Name {1}", (object) this.Id, (object) this.Name));
        return this.SetInstance.Counters[this.Id];
      }
    }

    public virtual void Decrement()
    {
      if (this.Type == CounterType.AverageTimer32)
        throw new InvalidOperationException(string.Format("\"{0}\" is an AverageTimer32 and should not be decremented", (object) this.Name));
      if (this.Type == CounterType.AverageBase)
        throw new InvalidOperationException(string.Format("\"{0}\" is an AverageBase and should not be decremented", (object) this.Name));
      this.Data.Decrement();
    }

    public virtual void Increment()
    {
      if (this.Type == CounterType.AverageTimer32)
        throw new InvalidOperationException(string.Format("\"{0}\" is an AverageTimer32 and should only be incremented via CountAverageBatchItemDuration", (object) this.Name));
      if (this.Type == CounterType.AverageBase)
        throw new InvalidOperationException(string.Format("\"{0}\" is an AverageBase and should only be incremented via CountAverageBatchItemDuration", (object) this.Name));
      this.Data.Increment();
    }

    public virtual void CountAverageBatchItemDuration(TimeSpan batchDuration, int batchItemCount)
    {
      if (this.Type != CounterType.AverageTimer32)
        throw new InvalidOperationException(string.Format("Counter \"{0}\" is not an AverageTimer32 (perf_average_timer)", (object) this.Name));
      this.CountAverageBatchItemDuration((long) (batchDuration.TotalSeconds * (double) Stopwatch.Frequency), 1);
    }

    public virtual void CountAverageBatchItemDuration(Stopwatch batchDuration, int batchItemCount)
    {
      if (this.Type != CounterType.AverageTimer32)
        throw new InvalidOperationException(string.Format("Counter \"{0}\" is not an AverageTimer32 (perf_average_timer)", (object) this.Name));
      if (batchDuration.IsRunning)
        throw new ArgumentException("The batchDuration stopwatch is still running");
      this.CountAverageBatchItemDuration(batchDuration.ElapsedTicks, batchItemCount);
    }

    public int CompareTo(object obj) => !(obj is PerfCounter perfCounter) ? -1 : this.ToString().CompareTo(perfCounter.ToString());

    public override bool Equals(object obj) => obj is PerfCounter perfCounter && this.SetGuid == perfCounter.SetGuid && this.Id == perfCounter.Id;

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override string ToString() => string.Format("Set {0}({1}), Counter {2}({3})", (object) this.SetName, (object) this.SetGuid, (object) this.Name, (object) this.Id);

    private void CountAverageBatchItemDuration(
      long batchItemDurationSystemTicks,
      int batchItemCount)
    {
      this.Data.IncrementBy(batchItemDurationSystemTicks);
      this.Base.Data.IncrementBy((long) batchItemCount);
    }
  }
}
