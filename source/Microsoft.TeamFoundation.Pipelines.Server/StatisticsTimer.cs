// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.StatisticsTimer
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class StatisticsTimer : IDisposable
  {
    private Stopwatch m_stopwatch;
    private StatisticsBuilder m_statisticsBuilder;

    public StatisticsTimer(StatisticsBuilder statisticsBuilder)
    {
      this.m_statisticsBuilder = statisticsBuilder;
      this.m_stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
      this.m_stopwatch.Stop();
      this.m_statisticsBuilder.AddAnalysisTime(this.m_stopwatch.Elapsed);
    }
  }
}
