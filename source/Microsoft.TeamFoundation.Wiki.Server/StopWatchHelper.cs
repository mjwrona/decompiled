// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.StopWatchHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class StopWatchHelper : IDisposable
  {
    private readonly Stopwatch stopwatch;
    private readonly TimedCiEvent ciEvent;
    private readonly string kpiPrefix;

    public StopWatchHelper(TimedCiEvent ciEvent, string kpiPrefix)
    {
      if (ciEvent == null)
        return;
      this.ciEvent = ciEvent;
      this.kpiPrefix = kpiPrefix;
      this.stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
      if (this.ciEvent == null)
        return;
      this.stopwatch.Stop();
      this.ciEvent.Properties.AddOrIncrement(this.kpiPrefix + "TimeInms", this.stopwatch.ElapsedMilliseconds);
      this.ciEvent.Properties.AddOrIncrement(this.kpiPrefix + "Count", 1L);
    }
  }
}
