// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SchedulingStopwatch
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Diagnostics;

namespace Microsoft.Azure.Documents
{
  internal sealed class SchedulingStopwatch
  {
    private readonly Stopwatch turnaroundTimeStopwatch;
    private readonly Stopwatch responseTimeStopwatch;
    private readonly Stopwatch runTimeStopwatch;
    private long numPreemptions;
    private bool responded;

    public SchedulingStopwatch()
    {
      this.turnaroundTimeStopwatch = new Stopwatch();
      this.responseTimeStopwatch = new Stopwatch();
      this.runTimeStopwatch = new Stopwatch();
    }

    public SchedulingTimeSpan Elapsed => new SchedulingTimeSpan(this.turnaroundTimeStopwatch.Elapsed, this.responseTimeStopwatch.Elapsed, this.runTimeStopwatch.Elapsed, this.turnaroundTimeStopwatch.Elapsed - this.runTimeStopwatch.Elapsed, this.numPreemptions);

    public void Ready()
    {
      this.turnaroundTimeStopwatch.Start();
      this.responseTimeStopwatch.Start();
    }

    public void Start()
    {
      if (this.runTimeStopwatch.IsRunning)
        return;
      if (!this.responded)
      {
        this.responseTimeStopwatch.Stop();
        this.responded = true;
      }
      this.runTimeStopwatch.Start();
    }

    public void Stop()
    {
      if (!this.runTimeStopwatch.IsRunning)
        return;
      this.runTimeStopwatch.Stop();
      ++this.numPreemptions;
    }

    public void Terminate()
    {
      this.turnaroundTimeStopwatch.Stop();
      this.responseTimeStopwatch.Stop();
    }

    public override string ToString() => this.Elapsed.ToString();
  }
}
