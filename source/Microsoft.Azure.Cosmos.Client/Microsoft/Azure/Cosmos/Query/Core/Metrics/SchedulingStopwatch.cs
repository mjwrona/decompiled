// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.SchedulingStopwatch
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class SchedulingStopwatch
  {
    private ValueStopwatch turnaroundTimeStopwatch;
    private ValueStopwatch responseTimeStopwatch;
    private ValueStopwatch runTimeStopwatch;
    private long numPreemptions;
    private bool responded;

    public SchedulingStopwatch()
    {
      this.turnaroundTimeStopwatch = new ValueStopwatch();
      this.responseTimeStopwatch = new ValueStopwatch();
      this.runTimeStopwatch = new ValueStopwatch();
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
