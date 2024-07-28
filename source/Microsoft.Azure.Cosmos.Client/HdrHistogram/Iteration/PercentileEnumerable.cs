// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.PercentileEnumerable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections;
using System.Collections.Generic;

namespace HdrHistogram.Iteration
{
  internal sealed class PercentileEnumerable : IEnumerable<HistogramIterationValue>, IEnumerable
  {
    private readonly HistogramBase _histogram;
    private readonly int _percentileTicksPerHalfDistance;

    public PercentileEnumerable(HistogramBase histogram, int percentileTicksPerHalfDistance)
    {
      this._histogram = histogram;
      this._percentileTicksPerHalfDistance = percentileTicksPerHalfDistance;
    }

    public IEnumerator<HistogramIterationValue> GetEnumerator() => (IEnumerator<HistogramIterationValue>) new PercentileEnumerator(this._histogram, this._percentileTicksPerHalfDistance);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
