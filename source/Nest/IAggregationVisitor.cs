// Decompiled with JetBrains decompiler
// Type: Nest.IAggregationVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface IAggregationVisitor
  {
    int Depth { get; set; }

    AggregationVisitorScope Scope { get; set; }

    void Visit(IAggregationContainer aggregationContainer);

    void Visit(IAggregation aggregation);

    void Visit(IAverageAggregation aggregation);

    void Visit(IValueCountAggregation aggregation);

    void Visit(IMaxAggregation aggregation);

    void Visit(IMinAggregation aggregation);

    void Visit(IStatsAggregation aggregation);

    void Visit(ISumAggregation aggregation);

    void Visit(IExtendedStatsAggregation aggregation);

    void Visit(IDateHistogramAggregation aggregation);

    void Visit(IPercentilesAggregation aggregation);

    void Visit(IDateRangeAggregation aggregation);

    void Visit(IFilterAggregation aggregation);

    void Visit(IFiltersAggregation aggregation);

    void Visit(IGeoDistanceAggregation aggregation);

    void Visit(IGeoHashGridAggregation aggregation);

    void Visit(IGeoLineAggregation aggregation);

    void Visit(IGeoTileGridAggregation aggregation);

    void Visit(IGeoBoundsAggregation aggregation);

    void Visit(IHistogramAggregation aggregation);

    void Visit(IGlobalAggregation aggregation);

    void Visit(IIpRangeAggregation aggregation);

    void Visit(ICardinalityAggregation aggregation);

    void Visit(IMissingAggregation aggregation);

    void Visit(INestedAggregation aggregation);

    void Visit(INormalizeAggregation aggregation);

    void Visit(IParentAggregation aggregation);

    void Visit(IReverseNestedAggregation aggregation);

    void Visit(IRangeAggregation aggregation);

    void Visit(IRareTermsAggregation aggregation);

    void Visit(IRateAggregation aggregation);

    void Visit(ITermsAggregation aggregation);

    void Visit(ISignificantTermsAggregation aggregation);

    void Visit(ISignificantTextAggregation aggregation);

    void Visit(IPercentileRanksAggregation aggregation);

    void Visit(ITopHitsAggregation aggregation);

    void Visit(IChildrenAggregation aggregation);

    void Visit(IScriptedMetricAggregation aggregation);

    void Visit(IAverageBucketAggregation aggregation);

    void Visit(IDerivativeAggregation aggregation);

    void Visit(IMaxBucketAggregation aggregation);

    void Visit(IMinBucketAggregation aggregation);

    void Visit(ISumBucketAggregation aggregation);

    void Visit(IStatsBucketAggregation aggregation);

    void Visit(IExtendedStatsBucketAggregation aggregation);

    void Visit(IPercentilesBucketAggregation aggregation);

    void Visit(IMovingAverageAggregation aggregation);

    void Visit(IMovingPercentilesAggregation aggregation);

    void Visit(ICumulativeSumAggregation aggregation);

    void Visit(ICumulativeCardinalityAggregation aggregation);

    void Visit(ISerialDifferencingAggregation aggregation);

    void Visit(IBucketScriptAggregation aggregation);

    void Visit(IBucketSelectorAggregation aggregation);

    void Visit(IBucketSortAggregation aggregation);

    void Visit(ISamplerAggregation aggregation);

    void Visit(IDiversifiedSamplerAggregation aggregation);

    void Visit(IGeoCentroidAggregation aggregation);

    void Visit(ICompositeAggregation aggregation);

    void Visit(IMedianAbsoluteDeviationAggregation aggregation);

    void Visit(IAdjacencyMatrixAggregation aggregation);

    void Visit(IAutoDateHistogramAggregation aggregation);

    void Visit(IMatrixStatsAggregation aggregation);

    void Visit(IWeightedAverageAggregation aggregation);

    void Visit(IMovingFunctionAggregation aggregation);

    void Visit(IStringStatsAggregation aggregation);

    void Visit(IBoxplotAggregation aggregation);

    void Visit(ITopMetricsAggregation aggregation);

    void Visit(ITTestAggregation aggregation);

    void Visit(IMultiTermsAggregation aggregation);

    void Visit(IVariableWidthHistogramAggregation aggregation);
  }
}
