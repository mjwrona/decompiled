// Decompiled with JetBrains decompiler
// Type: Nest.AggregationContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AggregationContainer : IAggregationContainer
  {
    private AggregationDictionary _aggs;

    public IAdjacencyMatrixAggregation AdjacencyMatrix { get; set; }

    [DataMember(Name = "aggregations")]
    private AggregationDictionary AggregationsProxy
    {
      set => this._aggs = value;
    }

    public AggregationDictionary Aggregations
    {
      get => this._aggs;
      set => this._aggs = value;
    }

    public IAverageAggregation Average { get; set; }

    public IAverageBucketAggregation AverageBucket { get; set; }

    public IBoxplotAggregation Boxplot { get; set; }

    public IBucketScriptAggregation BucketScript { get; set; }

    public IBucketSelectorAggregation BucketSelector { get; set; }

    public IBucketSortAggregation BucketSort { get; set; }

    public ICardinalityAggregation Cardinality { get; set; }

    public IChildrenAggregation Children { get; set; }

    public ICompositeAggregation Composite { get; set; }

    public ICumulativeSumAggregation CumulativeSum { get; set; }

    public ICumulativeCardinalityAggregation CumulativeCardinality { get; set; }

    public IDateHistogramAggregation DateHistogram { get; set; }

    public IAutoDateHistogramAggregation AutoDateHistogram { get; set; }

    public IDateRangeAggregation DateRange { get; set; }

    public IDerivativeAggregation Derivative { get; set; }

    public IDiversifiedSamplerAggregation DiversifiedSampler { get; set; }

    public IExtendedStatsAggregation ExtendedStats { get; set; }

    public IExtendedStatsBucketAggregation ExtendedStatsBucket { get; set; }

    public IFilterAggregation Filter { get; set; }

    public IFiltersAggregation Filters { get; set; }

    public IGeoBoundsAggregation GeoBounds { get; set; }

    public IGeoCentroidAggregation GeoCentroid { get; set; }

    public IGeoDistanceAggregation GeoDistance { get; set; }

    public IGeoHashGridAggregation GeoHash { get; set; }

    public IGeoLineAggregation GeoLine { get; set; }

    public IGeoTileGridAggregation GeoTile { get; set; }

    public IGlobalAggregation Global { get; set; }

    public IHistogramAggregation Histogram { get; set; }

    public IIpRangeAggregation IpRange { get; set; }

    public IMatrixStatsAggregation MatrixStats { get; set; }

    public IMaxAggregation Max { get; set; }

    public IMaxBucketAggregation MaxBucket { get; set; }

    public IDictionary<string, object> Meta { get; set; }

    public IMinAggregation Min { get; set; }

    public IMinBucketAggregation MinBucket { get; set; }

    public IMissingAggregation Missing { get; set; }

    public IMovingAverageAggregation MovingAverage { get; set; }

    public IMovingFunctionAggregation MovingFunction { get; set; }

    public IMovingPercentilesAggregation MovingPercentiles { get; set; }

    public INestedAggregation Nested { get; set; }

    public INormalizeAggregation Normalize { get; set; }

    public IParentAggregation Parent { get; set; }

    public IPercentileRanksAggregation PercentileRanks { get; set; }

    public IPercentilesAggregation Percentiles { get; set; }

    public IPercentilesBucketAggregation PercentilesBucket { get; set; }

    public IRangeAggregation Range { get; set; }

    public IRareTermsAggregation RareTerms { get; set; }

    public IRateAggregation Rate { get; set; }

    public IReverseNestedAggregation ReverseNested { get; set; }

    public ISamplerAggregation Sampler { get; set; }

    public IScriptedMetricAggregation ScriptedMetric { get; set; }

    public ISerialDifferencingAggregation SerialDifferencing { get; set; }

    public ISignificantTermsAggregation SignificantTerms { get; set; }

    public ISignificantTextAggregation SignificantText { get; set; }

    public IStatsAggregation Stats { get; set; }

    public IStatsBucketAggregation StatsBucket { get; set; }

    public ISumAggregation Sum { get; set; }

    public ISumBucketAggregation SumBucket { get; set; }

    public ITermsAggregation Terms { get; set; }

    public ITopHitsAggregation TopHits { get; set; }

    public ITTestAggregation TTest { get; set; }

    public IValueCountAggregation ValueCount { get; set; }

    public IWeightedAverageAggregation WeightedAverage { get; set; }

    public IMedianAbsoluteDeviationAggregation MedianAbsoluteDeviation { get; set; }

    public IStringStatsAggregation StringStats { get; set; }

    public ITopMetricsAggregation TopMetrics { get; set; }

    public IMultiTermsAggregation MultiTerms { get; set; }

    public IVariableWidthHistogramAggregation VariableWidthHistogram { get; set; }

    public void Accept(IAggregationVisitor visitor)
    {
      if (visitor.Scope == AggregationVisitorScope.Unknown)
        visitor.Scope = AggregationVisitorScope.Aggregation;
      new AggregationWalker().Walk((IAggregationContainer) this, visitor);
    }

    public static implicit operator AggregationContainer(AggregationBase aggregator)
    {
      if (aggregator == null)
        return (AggregationContainer) null;
      AggregationContainer container = new AggregationContainer();
      aggregator.WrapInContainer(container);
      BucketAggregationBase bucketAggregationBase = aggregator as BucketAggregationBase;
      container.Aggregations = bucketAggregationBase?.Aggregations;
      if ((aggregator is AggregationCombinator aggregationCombinator ? aggregationCombinator.Aggregations : (List<AggregationBase>) null) != null)
      {
        AggregationDictionary aggregationDictionary = new AggregationDictionary();
        foreach (AggregationBase aggregation in aggregationCombinator.Aggregations)
          aggregationDictionary.Add(((IAggregation) aggregation).Name, (AggregationContainer) aggregation);
        container.Aggregations = aggregationDictionary;
      }
      container.Meta = aggregator.Meta;
      return container;
    }
  }
}
