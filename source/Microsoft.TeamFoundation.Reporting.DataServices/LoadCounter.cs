// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.LoadCounter
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class LoadCounter : ICountLoad
  {
    private Stopwatch m_processingStopwatch;
    private Stopwatch m_userStopwatch;

    public LoadCounter()
    {
      this.CountedIterations = 0;
      this.m_processingStopwatch = new Stopwatch();
      this.m_userStopwatch = Stopwatch.StartNew();
    }

    public void Start() => this.m_processingStopwatch.Start();

    public void Stop()
    {
      this.m_processingStopwatch.Stop();
      ++this.CountedIterations;
    }

    public int CountedIterations { get; private set; }

    public TimeSpan ElapsedProcessingTime => this.m_processingStopwatch.Elapsed;

    public TimeSpan ElapsedUserTime => this.m_userStopwatch.Elapsed;
  }
}
