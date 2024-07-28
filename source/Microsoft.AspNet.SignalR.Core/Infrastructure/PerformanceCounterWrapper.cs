// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterWrapper
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class PerformanceCounterWrapper : IPerformanceCounter
  {
    private readonly PerformanceCounter _counter;

    public PerformanceCounterWrapper(PerformanceCounter counter) => this._counter = counter;

    public string CounterName => this._counter.CounterName;

    public long RawValue
    {
      get => this._counter.RawValue;
      set => this._counter.RawValue = value;
    }

    public long Decrement() => this._counter.Decrement();

    public long Increment() => this._counter.Increment();

    public long IncrementBy(long value) => this._counter.IncrementBy(value);

    public void Close() => this._counter.Close();

    public void RemoveInstance()
    {
      try
      {
        this._counter.RemoveInstance();
      }
      catch (NotImplementedException ex)
      {
      }
    }

    public CounterSample NextSample() => this._counter.NextSample();
  }
}
