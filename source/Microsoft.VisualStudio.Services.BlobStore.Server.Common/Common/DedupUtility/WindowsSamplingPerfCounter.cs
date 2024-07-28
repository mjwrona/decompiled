// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.WindowsSamplingPerfCounter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public abstract class WindowsSamplingPerfCounter : ExternalCounter, IDisposable, IResettable
  {
    private PerformanceCounter pc;
    private Timer t;
    private int count;
    private double value;

    protected WindowsSamplingPerfCounter(
      string pcCategory,
      string pcName,
      string pcInstance,
      int samplingInMillesec)
    {
      try
      {
        this.pc = new PerformanceCounter()
        {
          CategoryName = pcCategory,
          CounterName = pcName,
          InstanceName = pcInstance
        };
        double num = (double) this.pc.NextValue();
      }
      catch
      {
      }
      if (this.pc == null)
        return;
      this.t = new Timer(new TimerCallback(this.Sample), (object) null, 0, samplingInMillesec);
    }

    public abstract string Name { get; }

    public int Value
    {
      get
      {
        lock (this)
          return this.count > 0 ? (int) (this.value / (double) this.count) : 0;
      }
    }

    public void Dispose() => this.t?.Dispose();

    public void Reset()
    {
      lock (this)
      {
        this.value = 0.0;
        this.count = 0;
      }
    }

    private void Sample(object state)
    {
      try
      {
        float num = this.pc.NextValue();
        lock (this)
        {
          this.value += (double) num;
          ++this.count;
        }
      }
      catch
      {
      }
    }
  }
}
