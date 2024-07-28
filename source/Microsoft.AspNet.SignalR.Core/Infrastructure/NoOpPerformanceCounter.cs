// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.NoOpPerformanceCounter
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal class NoOpPerformanceCounter : IPerformanceCounter
  {
    public string CounterName => this.GetType().Name;

    public long Decrement() => 0;

    public long Increment() => 0;

    public long IncrementBy(long value) => 0;

    public long RawValue
    {
      get => 0;
      set
      {
      }
    }

    public void Close()
    {
    }

    public void RemoveInstance()
    {
    }

    public CounterSample NextSample() => CounterSample.Empty;
  }
}
