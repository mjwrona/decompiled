// Decompiled with JetBrains decompiler
// Type: Nest.IAggregationContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (AggregationContainer))]
  public interface IAggregationContainer
  {
    [DataMember(Name = "adjacency_matrix")]
    IAdjacencyMatrixAggregation AdjacencyMatrix { get; set; }

    [DataMember(Name = "aggs")]
    AggregationDictionary Aggregations { get; set; }

    [DataMember(Name = "avg")]
    IAverageAggregation Average { get; set; }

    [DataMember(Name = "avg_bucket")]
    IAverageBucketAggregation AverageBucket { get; set; }

    [DataMember(Name = "boxplot")]
    IBoxplotAggregation Boxplot { get; set; }

    [DataMember(Name = "bucket_script")]
    IBucketScriptAggregation BucketScript { get; set; }

    [DataMember(Name = "bucket_selector")]
    IBucketSelectorAggregation BucketSelector { get; set; }

    [DataMember(Name = "bucket_sort")]
    IBucketSortAggregation BucketSort { get; set; }

    [DataMember(Name = "cardinality")]
    ICardinalityAggregation Cardinality { get; set; }

    [DataMember(Name = "children")]
    IChildrenAggregation Children { get; set; }

    [DataMember(Name = "composite")]
    ICompositeAggregation Composite { get; set; }

    [DataMember(Name = "cumulative_sum")]
    ICumulativeSumAggregation CumulativeSum { get; set; }

    [DataMember(Name = "cumulative_cardinality")]
    ICumulativeCardinalityAggregation CumulativeCardinality { get; set; }

    [DataMember(Name = "date_histogram")]
    IDateHistogramAggregation DateHistogram { get; set; }

    [DataMember(Name = "auto_date_histogram")]
    IAutoDateHistogramAggregation AutoDateHistogram { get; set; }

    [DataMember(Name = "date_range")]
    IDateRangeAggregation DateRange { get; set; }

    [DataMember(Name = "derivative")]
    IDerivativeAggregation Derivative { get; set; }

    [DataMember(Name = "diversified_sampler")]
    IDiversifiedSamplerAggregation DiversifiedSampler { get; set; }

    [DataMember(Name = "extended_stats")]
    IExtendedStatsAggregation ExtendedStats { get; set; }

    [DataMember(Name = "extended_stats_bucket")]
    IExtendedStatsBucketAggregation ExtendedStatsBucket { get; set; }

    [DataMember(Name = "filter")]
    IFilterAggregation Filter { get; set; }

    [DataMember(Name = "filters")]
    IFiltersAggregation Filters { get; set; }

    [DataMember(Name = "geo_bounds")]
    IGeoBoundsAggregation GeoBounds { get; set; }

    [DataMember(Name = "geo_centroid")]
    IGeoCentroidAggregation GeoCentroid { get; set; }

    [DataMember(Name = "geo_distance")]
    IGeoDistanceAggregation GeoDistance { get; set; }

    [DataMember(Name = "geohash_grid")]
    IGeoHashGridAggregation GeoHash { get; set; }

    [DataMember(Name = "geo_line")]
    IGeoLineAggregation GeoLine { get; set; }

    [DataMember(Name = "geotile_grid")]
    IGeoTileGridAggregation GeoTile { get; set; }

    [DataMember(Name = "global")]
    IGlobalAggregation Global { get; set; }

    [DataMember(Name = "histogram")]
    IHistogramAggregation Histogram { get; set; }

    [DataMember(Name = "ip_range")]
    IIpRangeAggregation IpRange { get; set; }

    [DataMember(Name = "matrix_stats")]
    IMatrixStatsAggregation MatrixStats { get; set; }

    [DataMember(Name = "max")]
    IMaxAggregation Max { get; set; }

    [DataMember(Name = "max_bucket")]
    IMaxBucketAggregation MaxBucket { get; set; }

    [DataMember(Name = "meta")]
    [JsonFormatter(typeof (VerbatimDictionaryInterfaceKeysFormatter<string, object>))]
    IDictionary<string, object> Meta { get; set; }

    [DataMember(Name = "min")]
    IMinAggregation Min { get; set; }

    [DataMember(Name = "min_bucket")]
    IMinBucketAggregation MinBucket { get; set; }

    [DataMember(Name = "missing")]
    IMissingAggregation Missing { get; set; }

    [DataMember(Name = "moving_avg")]
    IMovingAverageAggregation MovingAverage { get; set; }

    [DataMember(Name = "moving_fn")]
    IMovingFunctionAggregation MovingFunction { get; set; }

    [DataMember(Name = "moving_percentiles")]
    IMovingPercentilesAggregation MovingPercentiles { get; set; }

    [DataMember(Name = "nested")]
    INestedAggregation Nested { get; set; }

    [DataMember(Name = "normalize")]
    INormalizeAggregation Normalize { get; set; }

    [DataMember(Name = "parent")]
    IParentAggregation Parent { get; set; }

    [DataMember(Name = "percentile_ranks")]
    IPercentileRanksAggregation PercentileRanks { get; set; }

    [DataMember(Name = "percentiles")]
    IPercentilesAggregation Percentiles { get; set; }

    [DataMember(Name = "percentiles_bucket")]
    IPercentilesBucketAggregation PercentilesBucket { get; set; }

    [DataMember(Name = "range")]
    IRangeAggregation Range { get; set; }

    [DataMember(Name = "rare_terms")]
    IRareTermsAggregation RareTerms { get; set; }

    [DataMember(Name = "rate")]
    IRateAggregation Rate { get; set; }

    [DataMember(Name = "reverse_nested")]
    IReverseNestedAggregation ReverseNested { get; set; }

    [DataMember(Name = "sampler")]
    ISamplerAggregation Sampler { get; set; }

    [DataMember(Name = "scripted_metric")]
    IScriptedMetricAggregation ScriptedMetric { get; set; }

    [DataMember(Name = "serial_diff")]
    ISerialDifferencingAggregation SerialDifferencing { get; set; }

    [DataMember(Name = "significant_terms")]
    ISignificantTermsAggregation SignificantTerms { get; set; }

    [DataMember(Name = "significant_text")]
    ISignificantTextAggregation SignificantText { get; set; }

    [DataMember(Name = "stats")]
    IStatsAggregation Stats { get; set; }

    [DataMember(Name = "stats_bucket")]
    IStatsBucketAggregation StatsBucket { get; set; }

    [DataMember(Name = "sum")]
    ISumAggregation Sum { get; set; }

    [DataMember(Name = "sum_bucket")]
    ISumBucketAggregation SumBucket { get; set; }

    [DataMember(Name = "terms")]
    ITermsAggregation Terms { get; set; }

    [DataMember(Name = "top_hits")]
    ITopHitsAggregation TopHits { get; set; }

    [DataMember(Name = "t_test")]
    ITTestAggregation TTest { get; set; }

    [DataMember(Name = "value_count")]
    IValueCountAggregation ValueCount { get; set; }

    [DataMember(Name = "weighted_avg")]
    IWeightedAverageAggregation WeightedAverage { get; set; }

    [DataMember(Name = "median_absolute_deviation")]
    IMedianAbsoluteDeviationAggregation MedianAbsoluteDeviation { get; set; }

    [DataMember(Name = "string_stats")]
    IStringStatsAggregation StringStats { get; set; }

    [DataMember(Name = "top_metrics")]
    ITopMetricsAggregation TopMetrics { get; set; }

    [DataMember(Name = "multi_terms")]
    IMultiTermsAggregation MultiTerms { get; set; }

    [DataMember(Name = "variable_width_histogram")]
    IVariableWidthHistogramAggregation VariableWidthHistogram { get; set; }

    void Accept(IAggregationVisitor visitor);
  }
}
