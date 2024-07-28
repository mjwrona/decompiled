// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Iteration.AllValuesEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace HdrHistogram.Iteration
{
  internal sealed class AllValuesEnumerator : AbstractHistogramEnumerator
  {
    private int _visitedSubBucketIndex;
    private int _visitedBucketIndex;

    public AllValuesEnumerator(HistogramBase histogram)
      : base(histogram)
    {
      this._visitedSubBucketIndex = -1;
      this._visitedBucketIndex = -1;
    }

    protected override void IncrementIterationLevel()
    {
      this._visitedSubBucketIndex = this.CurrentSubBucketIndex;
      this._visitedBucketIndex = this.CurrentBucketIndex;
    }

    protected override bool ReachedIterationLevel() => this._visitedSubBucketIndex != this.CurrentSubBucketIndex || this._visitedBucketIndex != this.CurrentBucketIndex;
  }
}
