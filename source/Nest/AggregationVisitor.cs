// Decompiled with JetBrains decompiler
// Type: Nest.AggregationVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class AggregationVisitor : IAggregationVisitor
  {
    public int Depth { get; set; }

    public AggregationVisitorScope Scope { get; set; }

    public virtual void Visit(IValueCountAggregation aggregation)
    {
    }

    public virtual void Visit(IMinAggregation aggregation)
    {
    }

    public virtual void Visit(ISumAggregation aggregation)
    {
    }

    public virtual void Visit(IDateHistogramAggregation aggregation)
    {
    }

    public virtual void Visit(IDateRangeAggregation aggregation)
    {
    }

    public virtual void Visit(IFiltersAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoHashGridAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoLineAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoTileGridAggregation aggregation)
    {
    }

    public virtual void Visit(IHistogramAggregation aggregation)
    {
    }

    public virtual void Visit(IIpRangeAggregation aggregation)
    {
    }

    public virtual void Visit(IMissingAggregation aggregation)
    {
    }

    public virtual void Visit(IReverseNestedAggregation aggregation)
    {
    }

    public virtual void Visit(ITermsAggregation aggregation)
    {
    }

    public virtual void Visit(ISignificantTextAggregation aggregation)
    {
    }

    public virtual void Visit(IPercentileRanksAggregation aggregation)
    {
    }

    public virtual void Visit(IChildrenAggregation aggregation)
    {
    }

    public virtual void Visit(IAverageBucketAggregation aggregation)
    {
    }

    public virtual void Visit(IMaxBucketAggregation aggregation)
    {
    }

    public virtual void Visit(ISumBucketAggregation aggregation)
    {
    }

    public virtual void Visit(IStatsBucketAggregation aggregation)
    {
    }

    public virtual void Visit(IExtendedStatsBucketAggregation aggregation)
    {
    }

    public virtual void Visit(IPercentilesBucketAggregation aggregation)
    {
    }

    public virtual void Visit(ICumulativeSumAggregation aggregation)
    {
    }

    public virtual void Visit(ICumulativeCardinalityAggregation aggregation)
    {
    }

    public virtual void Visit(IBucketScriptAggregation aggregation)
    {
    }

    public virtual void Visit(ISamplerAggregation aggregation)
    {
    }

    public virtual void Visit(IDiversifiedSamplerAggregation aggregation)
    {
    }

    public virtual void Visit(IBucketSelectorAggregation aggregation)
    {
    }

    public virtual void Visit(IBucketSortAggregation aggregation)
    {
    }

    public virtual void Visit(ISerialDifferencingAggregation aggregation)
    {
    }

    public virtual void Visit(IMovingAverageAggregation aggregation)
    {
    }

    public virtual void Visit(IMovingPercentilesAggregation aggregation)
    {
    }

    public virtual void Visit(IMinBucketAggregation aggregation)
    {
    }

    public virtual void Visit(IDerivativeAggregation aggregation)
    {
    }

    public virtual void Visit(IScriptedMetricAggregation aggregation)
    {
    }

    public virtual void Visit(ITopHitsAggregation aggregation)
    {
    }

    public virtual void Visit(ISignificantTermsAggregation aggregation)
    {
    }

    public virtual void Visit(IRangeAggregation aggregation)
    {
    }

    public virtual void Visit(IRareTermsAggregation aggregation)
    {
    }

    public virtual void Visit(IRateAggregation aggregation)
    {
    }

    public virtual void Visit(INestedAggregation aggregation)
    {
    }

    public virtual void Visit(INormalizeAggregation aggregation)
    {
    }

    public virtual void Visit(IParentAggregation aggregation)
    {
    }

    public virtual void Visit(ICardinalityAggregation aggregation)
    {
    }

    public virtual void Visit(IGlobalAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoBoundsAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoDistanceAggregation aggregation)
    {
    }

    public virtual void Visit(IFilterAggregation aggregation)
    {
    }

    public virtual void Visit(IPercentilesAggregation aggregation)
    {
    }

    public virtual void Visit(IExtendedStatsAggregation aggregation)
    {
    }

    public virtual void Visit(IStatsAggregation aggregation)
    {
    }

    public virtual void Visit(IMaxAggregation aggregation)
    {
    }

    public virtual void Visit(IAverageAggregation aggregation)
    {
    }

    public virtual void Visit(IGeoCentroidAggregation aggregation)
    {
    }

    public virtual void Visit(ICompositeAggregation aggregation)
    {
    }

    public virtual void Visit(IMedianAbsoluteDeviationAggregation aggregation)
    {
    }

    public virtual void Visit(IAdjacencyMatrixAggregation aggregation)
    {
    }

    public virtual void Visit(IAutoDateHistogramAggregation aggregation)
    {
    }

    public virtual void Visit(IMatrixStatsAggregation aggregation)
    {
    }

    public virtual void Visit(IWeightedAverageAggregation aggregation)
    {
    }

    public virtual void Visit(IMovingFunctionAggregation aggregation)
    {
    }

    public virtual void Visit(IStringStatsAggregation aggregation)
    {
    }

    public virtual void Visit(IBoxplotAggregation aggregation)
    {
    }

    public virtual void Visit(ITopMetricsAggregation aggregation)
    {
    }

    public virtual void Visit(ITTestAggregation aggregation)
    {
    }

    public virtual void Visit(IMultiTermsAggregation aggregationContainer)
    {
    }

    public virtual void Visit(IAggregation aggregation)
    {
    }

    public virtual void Visit(IAggregationContainer aggregationContainer)
    {
    }

    public virtual void Visit(
      IVariableWidthHistogramAggregation aggregationContainer)
    {
    }
  }
}
