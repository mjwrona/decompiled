// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.AllValueEnumerable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections;
using System.Collections.Generic;

namespace HdrHistogram.Iteration
{
  internal sealed class AllValueEnumerable : IEnumerable<HistogramIterationValue>, IEnumerable
  {
    private readonly HistogramBase _histogram;

    public AllValueEnumerable(HistogramBase histogram) => this._histogram = histogram;

    public IEnumerator<HistogramIterationValue> GetEnumerator() => (IEnumerator<HistogramIterationValue>) new AllValuesEnumerator(this._histogram);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
