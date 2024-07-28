// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ManifestCounters
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public sealed class ManifestCounters
  {
    private object counterInitializationLock;
    private bool windowsCountersEnabled;
    private InstrumentationManifest manifest;

    public ManifestCounters(
      InstrumentationManifest manifest,
      bool useWindowsCounters,
      string defaultSetName,
      string heartbeatCounterName)
    {
      if (manifest == null)
        throw new ArgumentNullException(nameof (manifest));
      if (!manifest.HasCounters)
        throw new ArgumentException("Expected manifest to contain counters");
      if (defaultSetName != null)
      {
        if (!manifest.CounterSetNames.Contains<string>(defaultSetName))
          throw new ArgumentException(string.Format("Counter set \"{0}\" wasn't among those defined in manifest \"{1}\": {2}", (object) defaultSetName, (object) manifest.ManifestName, (object) string.Join(", ", manifest.CounterSetNames)));
      }
      else if (heartbeatCounterName != null)
        throw new ArgumentNullException(nameof (defaultSetName), "Expected defaultSetName to be specified when heartbeatCounterName is specified");
      this.counterInitializationLock = new object();
      this.manifest = manifest;
      this.DefaultSetName = defaultSetName;
      this.HeartbeatCounterName = heartbeatCounterName;
      this.WindowsCountersEnabled = useWindowsCounters;
      if (heartbeatCounterName == null)
        return;
      this.GetCounter(heartbeatCounterName);
    }

    public IReadOnlyCollection<PerfCounter> Counters => this.Facade.GetCounters();

    public string DefaultSetName { get; private set; }

    public PerfCounter HeartbeatCounter => this.HeartbeatCounterName != null ? this.Facade.GetCounter(this.DefaultSetName, this.HeartbeatCounterName) : (PerfCounter) null;

    public string HeartbeatCounterName { get; private set; }

    public bool WindowsCountersEnabled
    {
      get => this.windowsCountersEnabled;
      set
      {
        this.windowsCountersEnabled = value;
        this.InitializeCounters();
      }
    }

    internal IPerformanceDataFacade Facade { get; private set; }

    public void CountAverageBatchItemDuration(
      string counterSetName,
      string counterName,
      Stopwatch batchDuration,
      int batchItemCount)
    {
      this.GetCounter(counterSetName, counterName).CountAverageBatchItemDuration(batchDuration, batchItemCount);
    }

    public void CountAverageBatchItemDuration(
      string counterName,
      Stopwatch batchDuration,
      int batchItemCount)
    {
      this.CheckDefaultSetName();
      this.CountAverageBatchItemDuration(this.DefaultSetName, counterName, batchDuration, batchItemCount);
    }

    public void Decrement(string counterSetName, string counterName) => this.GetCounter(counterSetName, counterName).Decrement();

    public void Decrement(string counterName)
    {
      this.CheckDefaultSetName();
      this.Decrement(this.DefaultSetName, counterName);
    }

    public void Increment(string counterSetName, string counterName) => this.GetCounter(counterSetName, counterName).Increment();

    public void Increment(string counterName)
    {
      this.CheckDefaultSetName();
      this.Increment(this.DefaultSetName, counterName);
    }

    public PerfCounter GetCounter(string counterSetName, string counterName) => this.Facade.GetCounter(counterSetName, counterName);

    public PerfCounter GetCounter(string counterName)
    {
      this.CheckDefaultSetName();
      return this.GetCounter(this.DefaultSetName, counterName);
    }

    private void CheckDefaultSetName()
    {
      if (this.DefaultSetName == null)
        throw new InvalidOperationException("Expected a DefaultSetName at construction time");
    }

    private void InitializeCounters()
    {
      lock (this.counterInitializationLock)
      {
        if (this.Facade is NoopPerformanceDataFacade && this.windowsCountersEnabled)
        {
          this.Facade = this.manifest.InitializeCounterFacade(this.windowsCountersEnabled);
        }
        else
        {
          IPerformanceDataFacade facade = this.Facade;
          this.Facade = this.manifest.InitializeCounterFacade(this.windowsCountersEnabled);
          facade?.Dispose();
        }
      }
    }
  }
}
