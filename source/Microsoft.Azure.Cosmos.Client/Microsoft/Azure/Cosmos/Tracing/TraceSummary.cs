// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceSummary
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing.TraceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal class TraceSummary
  {
    private int failedRequestCount;
    private readonly HashSet<(string, Uri)> regionContactedInternal = new HashSet<(string, Uri)>();

    public void IncrementFailedCount() => Interlocked.Increment(ref this.failedRequestCount);

    public int GetFailedCount() => this.failedRequestCount;

    public IReadOnlyList<(string, Uri)> RegionsContacted
    {
      get
      {
        lock (this.regionContactedInternal)
          return (IReadOnlyList<(string, Uri)>) this.regionContactedInternal.ToList<(string, Uri)>();
      }
    }

    public void UpdateRegionContacted(TraceDatum traceDatum)
    {
      if (!(traceDatum is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum) || statisticsTraceDatum.RegionsContacted == null || statisticsTraceDatum.RegionsContacted.Count == 0)
        return;
      lock (this.regionContactedInternal)
        this.regionContactedInternal.UnionWith((IEnumerable<(string, Uri)>) statisticsTraceDatum.RegionsContacted);
    }

    public void AddRegionContacted(string regionName, Uri locationEndpoint)
    {
      lock (this.regionContactedInternal)
        this.regionContactedInternal.Add((regionName, locationEndpoint));
    }
  }
}
