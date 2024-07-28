// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.SystemUsageRecorder
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class SystemUsageRecorder
  {
    internal readonly string identifier;
    private readonly int historyLength;
    internal readonly TimeSpan refreshInterval;
    private readonly Queue<SystemUsageLoad> historyQueue;
    private TimeSpan nextTimeStamp;

    public SystemUsageHistory Data { private set; get; }

    internal SystemUsageRecorder(string identifier, int historyLength, TimeSpan refreshInterval)
    {
      this.identifier = !string.IsNullOrEmpty(identifier) ? identifier : throw new ArgumentException("Identifier can not be null.");
      this.historyLength = historyLength > 0 ? historyLength : throw new ArgumentOutOfRangeException("historyLength can not be less than or equal to zero.");
      this.refreshInterval = !(refreshInterval <= TimeSpan.Zero) ? refreshInterval : throw new ArgumentException("refreshInterval timespan can not be zero.");
      this.historyQueue = new Queue<SystemUsageLoad>(this.historyLength);
    }

    internal void RecordUsage(SystemUsageLoad systemUsageLoad, Stopwatch watch)
    {
      this.nextTimeStamp = watch.Elapsed.Add(this.refreshInterval);
      this.Data = new SystemUsageHistory(this.Collect(systemUsageLoad), this.refreshInterval);
    }

    private ReadOnlyCollection<SystemUsageLoad> Collect(SystemUsageLoad loadData)
    {
      if (this.historyQueue.Count == this.historyLength)
        this.historyQueue.Dequeue();
      this.historyQueue.Enqueue(loadData);
      return new ReadOnlyCollection<SystemUsageLoad>((IList<SystemUsageLoad>) this.historyQueue.ToList<SystemUsageLoad>());
    }

    internal bool IsEligibleForRecording(Stopwatch watch) => TimeSpan.Compare(watch.Elapsed, this.nextTimeStamp) >= 0;
  }
}
