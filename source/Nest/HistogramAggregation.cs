// Decompiled with JetBrains decompiler
// Type: Nest.HistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class HistogramAggregation : 
    BucketAggregationBase,
    IHistogramAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal HistogramAggregation()
    {
    }

    public HistogramAggregation(string name)
      : base(name)
    {
    }

    public Nest.ExtendedBounds<double> ExtendedBounds { get; set; }

    public Nest.HardBounds<double> HardBounds { get; set; }

    public Field Field { get; set; }

    public double? Interval { get; set; }

    public int? MinimumDocumentCount { get; set; }

    public double? Missing { get; set; }

    public double? Offset { get; set; }

    public HistogramOrder Order { get; set; }

    public IScript Script { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Histogram = (IHistogramAggregation) this;
  }
}
