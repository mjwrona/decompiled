// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.FixedSizeQueue`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class FixedSizeQueue<T>
  {
    private readonly int maxSize;
    private object queueLockObj = new object();
    private Queue<T> queue = new Queue<T>();

    internal FixedSizeQueue(int maxSize) => this.maxSize = maxSize;

    internal void Enqueue(T item)
    {
      lock (this.queueLockObj)
      {
        if (this.queue.Count == this.maxSize)
          this.queue.Dequeue();
        this.queue.Enqueue(item);
      }
    }

    internal bool Contains(T item)
    {
      lock (this.queueLockObj)
        return this.queue.Contains(item);
    }
  }
}
