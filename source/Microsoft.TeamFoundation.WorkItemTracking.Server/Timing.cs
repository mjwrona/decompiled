// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Timing
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class Timing : IDisposable
  {
    private Stopwatch stopwatch = new Stopwatch();
    private PerformanceScenarioHelper scenarioHelper;

    public string Name { get; set; }

    public TimeSpan Duration { get; set; }

    public Timing(PerformanceScenarioHelper scenario, string name)
    {
      this.scenarioHelper = scenario;
      this.Name = name;
      this.stopwatch.Start();
    }

    public void Dispose()
    {
      this.stopwatch.Stop();
      this.Duration = this.stopwatch.Elapsed;
      this.scenarioHelper.Add(this.Name, this.Duration);
    }
  }
}
