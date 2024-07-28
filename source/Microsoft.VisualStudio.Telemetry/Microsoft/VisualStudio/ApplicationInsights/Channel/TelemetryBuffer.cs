// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.TelemetryBuffer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class TelemetryBuffer
  {
    public Action OnFull;
    private const int DefaultCapacity = 500;
    private readonly object lockObj = new object();
    private int capacity = 500;
    private List<ITelemetry> items;

    internal TelemetryBuffer() => this.items = new List<ITelemetry>();

    public int Capacity
    {
      get => this.capacity;
      set
      {
        if (value < 1)
          this.capacity = 500;
        else
          this.capacity = value;
      }
    }

    public void Enqueue(ITelemetry item)
    {
      if (item == null)
      {
        CoreEventSource.Log.LogVerbose("item is null in TelemetryBuffer.Enqueue");
      }
      else
      {
        lock (this.lockObj)
        {
          this.items.Add(item);
          if (this.items.Count < this.Capacity)
            return;
          Action onFull = this.OnFull;
          if (onFull == null)
            return;
          onFull();
        }
      }
    }

    public IEnumerable<ITelemetry> Dequeue()
    {
      List<ITelemetry> telemetryList = (List<ITelemetry>) null;
      if (this.items.Count > 0)
      {
        lock (this.lockObj)
        {
          if (this.items.Count > 0)
          {
            telemetryList = this.items;
            this.items = new List<ITelemetry>();
          }
        }
      }
      return (IEnumerable<ITelemetry>) telemetryList;
    }
  }
}
