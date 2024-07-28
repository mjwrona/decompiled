// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.HistogramIterationValue
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram.Iteration
{
  internal sealed class HistogramIterationValue
  {
    public long ValueIteratedTo { get; private set; }

    public long ValueIteratedFrom { get; private set; }

    public long CountAtValueIteratedTo { get; private set; }

    public long CountAddedInThisIterationStep { get; private set; }

    public long TotalCountToThisValue { get; private set; }

    public long TotalValueToThisValue { get; private set; }

    public double Percentile { get; private set; }

    public double PercentileLevelIteratedTo { get; private set; }

    public bool IsLastValue() => Math.Abs(this.PercentileLevelIteratedTo - 100.0) < double.Epsilon;

    internal void Set(
      long valueIteratedTo,
      long valueIteratedFrom,
      long countAtValueIteratedTo,
      long countInThisIterationStep,
      long totalCountToThisValue,
      long totalValueToThisValue,
      double percentile,
      double percentileLevelIteratedTo)
    {
      this.ValueIteratedTo = valueIteratedTo;
      this.ValueIteratedFrom = valueIteratedFrom;
      this.CountAtValueIteratedTo = countAtValueIteratedTo;
      this.CountAddedInThisIterationStep = countInThisIterationStep;
      this.TotalCountToThisValue = totalCountToThisValue;
      this.TotalValueToThisValue = totalValueToThisValue;
      this.Percentile = percentile;
      this.PercentileLevelIteratedTo = percentileLevelIteratedTo;
    }

    public override string ToString() => "ValueIteratedTo:" + this.ValueIteratedTo.ToString() + ", ValueIteratedFrom:" + this.ValueIteratedFrom.ToString() + ", CountAtValueIteratedTo:" + this.CountAtValueIteratedTo.ToString() + ", CountAddedInThisIterationStep:" + this.CountAddedInThisIterationStep.ToString() + ", TotalCountToThisValue:" + this.TotalCountToThisValue.ToString() + ", TotalValueToThisValue:" + this.TotalValueToThisValue.ToString() + ", Percentile:" + this.Percentile.ToString() + ", PercentileLevelIteratedTo:" + this.PercentileLevelIteratedTo.ToString();
  }
}
