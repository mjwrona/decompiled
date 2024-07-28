// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Metrics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class Metrics
  {
    private readonly Stopwatch stopwatch;

    public Metrics() => this.stopwatch = new Stopwatch();

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
