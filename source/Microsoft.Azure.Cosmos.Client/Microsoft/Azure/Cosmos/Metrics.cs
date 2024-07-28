// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Metrics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class Metrics
  {
    private ValueStopwatch stopwatch;

    public Metrics() => this.stopwatch = new ValueStopwatch();

    public int Count { get; private set; }

    public long ElapsedMilliseconds => this.stopwatch.ElapsedMilliseconds;

    public double AverageElapsedMilliseconds => (double) this.ElapsedMilliseconds / (double) this.Count;

    public void Start() => this.stopwatch.Start();

    public void Stop()
    {
      this.stopwatch.Stop();
      ++this.Count;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Total time (ms): {0}, Count: {1}, Average Time (ms): {2}", (object) this.ElapsedMilliseconds, (object) this.Count, (object) this.AverageElapsedMilliseconds);
  }
}
