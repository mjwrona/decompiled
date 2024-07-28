// Decompiled with JetBrains decompiler
// Type: Nest.AggregationContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AggregationContainerDescriptor<T> : 
    DescriptorBase<AggregationContainerDescriptor<T>, IAggregationContainer>,
    IAggregationContainer
    where T : class
  {
    IAdjacencyMatrixAggregation IAggregationContainer.AdjacencyMatrix { get; set; }

    AggregationDictionary IAggregationContainer.Aggregations { get; set; }

    IAverageAggregation IAggregationContainer.Average { get; set; }

    IAverageBucketAggregation IAggregationContainer.AverageBucket { get; set; }

    IBoxplotAggregation IAggregationContainer.Boxplot { get; set; }

    IBucketScriptAggregation IAggregationContainer.BucketScript { get; set; }

    IBucketSelectorAggregation IAggregationContainer.BucketSelector { get; set; }

    IBucketSortAggregation IAggregationContainer.BucketSort { get; set; }

    ICardinalityAggregation IAggregationContainer.Cardinality { get; set; }

    IChildrenAggregation IAggregationContainer.Children { get; set; }

    ICompositeAggregation IAggregationContainer.Composite { get; set; }

    ICumulativeSumAggregation IAggregationContainer.CumulativeSum { get; set; }

    ICumulativeCardinalityAggregation IAggregationContainer.CumulativeCardinality { get; set; }

    IDateHistogramAggregation IAggregationContainer.DateHistogram { get; set; }

    IAutoDateHistogramAggregation IAggregationContainer.AutoDateHistogram { get; set; }

    IDateRangeAggregation IAggregationContainer.DateRange { get; set; }

    IDerivativeAggregation IAggregationContainer.Derivative { get; set; }

    IDiversifiedSamplerAggregation IAggregationContainer.DiversifiedSampler { get; set; }

    IExtendedStatsAggregation IAggregationContainer.ExtendedStats { get; set; }

    IExtendedStatsBucketAggregation IAggregationContainer.ExtendedStatsBucket { get; set; }

    IFilterAggregation IAggregationContainer.Filter { get; set; }

    IFiltersAggregation IAggregationContainer.Filters { get; set; }

    IGeoBoundsAggregation IAggregationContainer.GeoBounds { get; set; }

    IGeoCentroidAggregation IAggregationContainer.GeoCentroid { get; set; }

    IGeoDistanceAggregation IAggregationContainer.GeoDistance { get; set; }

    IGeoHashGridAggregation IAggregationContainer.GeoHash { get; set; }

    IGeoLineAggregation IAggregationContainer.GeoLine { get; set; }

    IGeoTileGridAggregation IAggregationContainer.GeoTile { get; set; }

    IGlobalAggregation IAggregationContainer.Global { get; set; }

    IHistogramAggregation IAggregationContainer.Histogram { get; set; }

    IIpRangeAggregation IAggregationContainer.IpRange { get; set; }

    IMatrixStatsAggregation IAggregationContainer.MatrixStats { get; set; }

    IMaxAggregation IAggregationContainer.Max { get; set; }

    IMaxBucketAggregation IAggregationContainer.MaxBucket { get; set; }

    IDictionary<string, object> IAggregationContainer.Meta { get; set; }

    IMinAggregation IAggregationContainer.Min { get; set; }

    IMinBucketAggregation IAggregationContainer.MinBucket { get; set; }

    IMissingAggregation IAggregationContainer.Missing { get; set; }

    IMovingAverageAggregation IAggregationContainer.MovingAverage { get; set; }

    IMovingFunctionAggregation IAggregationContainer.MovingFunction { get; set; }

    IMovingPercentilesAggregation IAggregationContainer.MovingPercentiles { get; set; }

    IMultiTermsAggregation IAggregationContainer.MultiTerms { get; set; }

    INestedAggregation IAggregationContainer.Nested { get; set; }

    INormalizeAggregation IAggregationContainer.Normalize { get; set; }

    IParentAggregation IAggregationContainer.Parent { get; set; }

    IPercentileRanksAggregation IAggregationContainer.PercentileRanks { get; set; }

    IPercentilesAggregation IAggregationContainer.Percentiles { get; set; }

    IPercentilesBucketAggregation IAggregationContainer.PercentilesBucket { get; set; }

    IRangeAggregation IAggregationContainer.Range { get; set; }

    IRareTermsAggregation IAggregationContainer.RareTerms { get; set; }

    IRateAggregation IAggregationContainer.Rate { get; set; }

    IReverseNestedAggregation IAggregationContainer.ReverseNested { get; set; }

    ISamplerAggregation IAggregationContainer.Sampler { get; set; }

    IScriptedMetricAggregation IAggregationContainer.ScriptedMetric { get; set; }

    ISerialDifferencingAggregation IAggregationContainer.SerialDifferencing { get; set; }

    ISignificantTermsAggregation IAggregationContainer.SignificantTerms { get; set; }

    ISignificantTextAggregation IAggregationContainer.SignificantText { get; set; }

    IStatsAggregation IAggregationContainer.Stats { get; set; }

    IStatsBucketAggregation IAggregationContainer.StatsBucket { get; set; }

    ISumAggregation IAggregationContainer.Sum { get; set; }

    ISumBucketAggregation IAggregationContainer.SumBucket { get; set; }

    ITermsAggregation IAggregationContainer.Terms { get; set; }

    ITopHitsAggregation IAggregationContainer.TopHits { get; set; }

    ITTestAggregation IAggregationContainer.TTest { get; set; }

    IValueCountAggregation IAggregationContainer.ValueCount { get; set; }

    IWeightedAverageAggregation IAggregationContainer.WeightedAverage { get; set; }

    IMedianAbsoluteDeviationAggregation IAggregationContainer.MedianAbsoluteDeviation { get; set; }

    IStringStatsAggregation IAggregationContainer.StringStats { get; set; }

    ITopMetricsAggregation IAggregationContainer.TopMetrics { get; set; }

    IVariableWidthHistogramAggregation IAggregationContainer.VariableWidthHistogram { get; set; }

    public void Accept(IAggregationVisitor visitor)
    {
      if (visitor.Scope == AggregationVisitorScope.Unknown)
        visitor.Scope = AggregationVisitorScope.Aggregation;
      new AggregationWalker().Walk((IAggregationContainer) this, visitor);
    }

    public AggregationContainerDescriptor<T> Average(
      string name,
      Func<AverageAggregationDescriptor<T>, IAverageAggregation> selector)
    {
      return this._SetInnerAggregation<AverageAggregationDescriptor<T>, IAverageAggregation>(name, selector, (Action<IAggregationContainer, IAverageAggregation>) ((a, d) => a.Average = d));
    }

    public AggregationContainerDescriptor<T> DateHistogram(
      string name,
      Func<DateHistogramAggregationDescriptor<T>, IDateHistogramAggregation> selector)
    {
      return this._SetInnerAggregation<DateHistogramAggregationDescriptor<T>, IDateHistogramAggregation>(name, selector, (Action<IAggregationContainer, IDateHistogramAggregation>) ((a, d) => a.DateHistogram = d));
    }

    public AggregationContainerDescriptor<T> AutoDateHistogram(
      string name,
      Func<AutoDateHistogramAggregationDescriptor<T>, IAutoDateHistogramAggregation> selector)
    {
      return this._SetInnerAggregation<AutoDateHistogramAggregationDescriptor<T>, IAutoDateHistogramAggregation>(name, selector, (Action<IAggregationContainer, IAutoDateHistogramAggregation>) ((a, d) => a.AutoDateHistogram = d));
    }

    public AggregationContainerDescriptor<T> Percentiles(
      string name,
      Func<PercentilesAggregationDescriptor<T>, IPercentilesAggregation> selector)
    {
      return this._SetInnerAggregation<PercentilesAggregationDescriptor<T>, IPercentilesAggregation>(name, selector, (Action<IAggregationContainer, IPercentilesAggregation>) ((a, d) => a.Percentiles = d));
    }

    public AggregationContainerDescriptor<T> PercentileRanks(
      string name,
      Func<PercentileRanksAggregationDescriptor<T>, IPercentileRanksAggregation> selector)
    {
      return this._SetInnerAggregation<PercentileRanksAggregationDescriptor<T>, IPercentileRanksAggregation>(name, selector, (Action<IAggregationContainer, IPercentileRanksAggregation>) ((a, d) => a.PercentileRanks = d));
    }

    public AggregationContainerDescriptor<T> DateRange(
      string name,
      Func<DateRangeAggregationDescriptor<T>, IDateRangeAggregation> selector)
    {
      return this._SetInnerAggregation<DateRangeAggregationDescriptor<T>, IDateRangeAggregation>(name, selector, (Action<IAggregationContainer, IDateRangeAggregation>) ((a, d) => a.DateRange = d));
    }

    public AggregationContainerDescriptor<T> ExtendedStats(
      string name,
      Func<ExtendedStatsAggregationDescriptor<T>, IExtendedStatsAggregation> selector)
    {
      return this._SetInnerAggregation<ExtendedStatsAggregationDescriptor<T>, IExtendedStatsAggregation>(name, selector, (Action<IAggregationContainer, IExtendedStatsAggregation>) ((a, d) => a.ExtendedStats = d));
    }

    public AggregationContainerDescriptor<T> Filter(
      string name,
      Func<FilterAggregationDescriptor<T>, IFilterAggregation> selector)
    {
      return this._SetInnerAggregation<FilterAggregationDescriptor<T>, IFilterAggregation>(name, selector, (Action<IAggregationContainer, IFilterAggregation>) ((a, d) => a.Filter = d));
    }

    public AggregationContainerDescriptor<T> Filters(
      string name,
      Func<FiltersAggregationDescriptor<T>, IFiltersAggregation> selector)
    {
      return this._SetInnerAggregation<FiltersAggregationDescriptor<T>, IFiltersAggregation>(name, selector, (Action<IAggregationContainer, IFiltersAggregation>) ((a, d) => a.Filters = d));
    }

    public AggregationContainerDescriptor<T> GeoDistance(
      string name,
      Func<GeoDistanceAggregationDescriptor<T>, IGeoDistanceAggregation> selector)
    {
      return this._SetInnerAggregation<GeoDistanceAggregationDescriptor<T>, IGeoDistanceAggregation>(name, selector, (Action<IAggregationContainer, IGeoDistanceAggregation>) ((a, d) => a.GeoDistance = d));
    }

    public AggregationContainerDescriptor<T> GeoHash(
      string name,
      Func<GeoHashGridAggregationDescriptor<T>, IGeoHashGridAggregation> selector)
    {
      return this._SetInnerAggregation<GeoHashGridAggregationDescriptor<T>, IGeoHashGridAggregation>(name, selector, (Action<IAggregationContainer, IGeoHashGridAggregation>) ((a, d) => a.GeoHash = d));
    }

    public AggregationContainerDescriptor<T> GeoLine(
      string name,
      Func<GeoLineAggregationDescriptor<T>, IGeoLineAggregation> selector)
    {
      return this._SetInnerAggregation<GeoLineAggregationDescriptor<T>, IGeoLineAggregation>(name, selector, (Action<IAggregationContainer, IGeoLineAggregation>) ((a, d) => a.GeoLine = d));
    }

    public AggregationContainerDescriptor<T> GeoTile(
      string name,
      Func<GeoTileGridAggregationDescriptor<T>, IGeoTileGridAggregation> selector)
    {
      return this._SetInnerAggregation<GeoTileGridAggregationDescriptor<T>, IGeoTileGridAggregation>(name, selector, (Action<IAggregationContainer, IGeoTileGridAggregation>) ((a, d) => a.GeoTile = d));
    }

    public AggregationContainerDescriptor<T> GeoBounds(
      string name,
      Func<GeoBoundsAggregationDescriptor<T>, IGeoBoundsAggregation> selector)
    {
      return this._SetInnerAggregation<GeoBoundsAggregationDescriptor<T>, IGeoBoundsAggregation>(name, selector, (Action<IAggregationContainer, IGeoBoundsAggregation>) ((a, d) => a.GeoBounds = d));
    }

    public AggregationContainerDescriptor<T> Histogram(
      string name,
      Func<HistogramAggregationDescriptor<T>, IHistogramAggregation> selector)
    {
      return this._SetInnerAggregation<HistogramAggregationDescriptor<T>, IHistogramAggregation>(name, selector, (Action<IAggregationContainer, IHistogramAggregation>) ((a, d) => a.Histogram = d));
    }

    public AggregationContainerDescriptor<T> Global(
      string name,
      Func<GlobalAggregationDescriptor<T>, IGlobalAggregation> selector)
    {
      return this._SetInnerAggregation<GlobalAggregationDescriptor<T>, IGlobalAggregation>(name, selector, (Action<IAggregationContainer, IGlobalAggregation>) ((a, d) => a.Global = d));
    }

    public AggregationContainerDescriptor<T> IpRange(
      string name,
      Func<IpRangeAggregationDescriptor<T>, IIpRangeAggregation> selector)
    {
      return this._SetInnerAggregation<IpRangeAggregationDescriptor<T>, IIpRangeAggregation>(name, selector, (Action<IAggregationContainer, IIpRangeAggregation>) ((a, d) => a.IpRange = d));
    }

    public AggregationContainerDescriptor<T> Max(
      string name,
      Func<MaxAggregationDescriptor<T>, IMaxAggregation> selector)
    {
      return this._SetInnerAggregation<MaxAggregationDescriptor<T>, IMaxAggregation>(name, selector, (Action<IAggregationContainer, IMaxAggregation>) ((a, d) => a.Max = d));
    }

    public AggregationContainerDescriptor<T> Min(
      string name,
      Func<MinAggregationDescriptor<T>, IMinAggregation> selector)
    {
      return this._SetInnerAggregation<MinAggregationDescriptor<T>, IMinAggregation>(name, selector, (Action<IAggregationContainer, IMinAggregation>) ((a, d) => a.Min = d));
    }

    public AggregationContainerDescriptor<T> Cardinality(
      string name,
      Func<CardinalityAggregationDescriptor<T>, ICardinalityAggregation> selector)
    {
      return this._SetInnerAggregation<CardinalityAggregationDescriptor<T>, ICardinalityAggregation>(name, selector, (Action<IAggregationContainer, ICardinalityAggregation>) ((a, d) => a.Cardinality = d));
    }

    public AggregationContainerDescriptor<T> Missing(
      string name,
      Func<MissingAggregationDescriptor<T>, IMissingAggregation> selector)
    {
      return this._SetInnerAggregation<MissingAggregationDescriptor<T>, IMissingAggregation>(name, selector, (Action<IAggregationContainer, IMissingAggregation>) ((a, d) => a.Missing = d));
    }

    public AggregationContainerDescriptor<T> MultiTerms(
      string name,
      Func<MultiTermsAggregationDescriptor<T>, IMultiTermsAggregation> selector)
    {
      return this._SetInnerAggregation<MultiTermsAggregationDescriptor<T>, IMultiTermsAggregation>(name, selector, (Action<IAggregationContainer, IMultiTermsAggregation>) ((a, d) => a.MultiTerms = d));
    }

    public AggregationContainerDescriptor<T> Nested(
      string name,
      Func<NestedAggregationDescriptor<T>, INestedAggregation> selector)
    {
      return this._SetInnerAggregation<NestedAggregationDescriptor<T>, INestedAggregation>(name, selector, (Action<IAggregationContainer, INestedAggregation>) ((a, d) => a.Nested = d));
    }

    public AggregationContainerDescriptor<T> Normalize(
      string name,
      Func<NormalizeAggregationDescriptor, INormalizeAggregation> selector)
    {
      return this._SetInnerAggregation<NormalizeAggregationDescriptor, INormalizeAggregation>(name, selector, (Action<IAggregationContainer, INormalizeAggregation>) ((a, d) => a.Normalize = d));
    }

    public AggregationContainerDescriptor<T> Parent<TParent>(
      string name,
      Func<ParentAggregationDescriptor<T, TParent>, IParentAggregation> selector)
      where TParent : class
    {
      return this._SetInnerAggregation<ParentAggregationDescriptor<T, TParent>, IParentAggregation>(name, selector, (Action<IAggregationContainer, IParentAggregation>) ((a, d) => a.Parent = d));
    }

    public AggregationContainerDescriptor<T> ReverseNested(
      string name,
      Func<ReverseNestedAggregationDescriptor<T>, IReverseNestedAggregation> selector)
    {
      return this._SetInnerAggregation<ReverseNestedAggregationDescriptor<T>, IReverseNestedAggregation>(name, selector, (Action<IAggregationContainer, IReverseNestedAggregation>) ((a, d) => a.ReverseNested = d));
    }

    public AggregationContainerDescriptor<T> Range(
      string name,
      Func<RangeAggregationDescriptor<T>, IRangeAggregation> selector)
    {
      return this._SetInnerAggregation<RangeAggregationDescriptor<T>, IRangeAggregation>(name, selector, (Action<IAggregationContainer, IRangeAggregation>) ((a, d) => a.Range = d));
    }

    public AggregationContainerDescriptor<T> RareTerms(
      string name,
      Func<RareTermsAggregationDescriptor<T>, IRareTermsAggregation> selector)
    {
      return this._SetInnerAggregation<RareTermsAggregationDescriptor<T>, IRareTermsAggregation>(name, selector, (Action<IAggregationContainer, IRareTermsAggregation>) ((a, d) => a.RareTerms = d));
    }

    public AggregationContainerDescriptor<T> Rate(
      string name,
      Func<RateAggregationDescriptor<T>, IRateAggregation> selector)
    {
      return this._SetInnerAggregation<RateAggregationDescriptor<T>, IRateAggregation>(name, selector, (Action<IAggregationContainer, IRateAggregation>) ((a, d) => a.Rate = d));
    }

    public AggregationContainerDescriptor<T> Stats(
      string name,
      Func<StatsAggregationDescriptor<T>, IStatsAggregation> selector)
    {
      return this._SetInnerAggregation<StatsAggregationDescriptor<T>, IStatsAggregation>(name, selector, (Action<IAggregationContainer, IStatsAggregation>) ((a, d) => a.Stats = d));
    }

    public AggregationContainerDescriptor<T> Sum(
      string name,
      Func<SumAggregationDescriptor<T>, ISumAggregation> selector)
    {
      return this._SetInnerAggregation<SumAggregationDescriptor<T>, ISumAggregation>(name, selector, (Action<IAggregationContainer, ISumAggregation>) ((a, d) => a.Sum = d));
    }

    public AggregationContainerDescriptor<T> Terms(
      string name,
      Func<TermsAggregationDescriptor<T>, ITermsAggregation> selector)
    {
      return this._SetInnerAggregation<TermsAggregationDescriptor<T>, ITermsAggregation>(name, selector, (Action<IAggregationContainer, ITermsAggregation>) ((a, d) => a.Terms = d));
    }

    public AggregationContainerDescriptor<T> SignificantTerms(
      string name,
      Func<SignificantTermsAggregationDescriptor<T>, ISignificantTermsAggregation> selector)
    {
      return this._SetInnerAggregation<SignificantTermsAggregationDescriptor<T>, ISignificantTermsAggregation>(name, selector, (Action<IAggregationContainer, ISignificantTermsAggregation>) ((a, d) => a.SignificantTerms = d));
    }

    public AggregationContainerDescriptor<T> SignificantText(
      string name,
      Func<SignificantTextAggregationDescriptor<T>, ISignificantTextAggregation> selector)
    {
      return this._SetInnerAggregation<SignificantTextAggregationDescriptor<T>, ISignificantTextAggregation>(name, selector, (Action<IAggregationContainer, ISignificantTextAggregation>) ((a, d) => a.SignificantText = d));
    }

    public AggregationContainerDescriptor<T> ValueCount(
      string name,
      Func<ValueCountAggregationDescriptor<T>, IValueCountAggregation> selector)
    {
      return this._SetInnerAggregation<ValueCountAggregationDescriptor<T>, IValueCountAggregation>(name, selector, (Action<IAggregationContainer, IValueCountAggregation>) ((a, d) => a.ValueCount = d));
    }

    public AggregationContainerDescriptor<T> TopHits(
      string name,
      Func<TopHitsAggregationDescriptor<T>, ITopHitsAggregation> selector)
    {
      return this._SetInnerAggregation<TopHitsAggregationDescriptor<T>, ITopHitsAggregation>(name, selector, (Action<IAggregationContainer, ITopHitsAggregation>) ((a, d) => a.TopHits = d));
    }

    public AggregationContainerDescriptor<T> TTest(
      string name,
      Func<TTestAggregationDescriptor<T>, ITTestAggregation> selector)
    {
      return this._SetInnerAggregation<TTestAggregationDescriptor<T>, ITTestAggregation>(name, selector, (Action<IAggregationContainer, ITTestAggregation>) ((a, d) => a.TTest = d));
    }

    public AggregationContainerDescriptor<T> Children<TChild>(
      string name,
      Func<ChildrenAggregationDescriptor<TChild>, IChildrenAggregation> selector)
      where TChild : class
    {
      return this._SetInnerAggregation<ChildrenAggregationDescriptor<TChild>, IChildrenAggregation>(name, selector, (Action<IAggregationContainer, IChildrenAggregation>) ((a, d) => a.Children = d));
    }

    public AggregationContainerDescriptor<T> ScriptedMetric(
      string name,
      Func<ScriptedMetricAggregationDescriptor<T>, IScriptedMetricAggregation> selector)
    {
      return this._SetInnerAggregation<ScriptedMetricAggregationDescriptor<T>, IScriptedMetricAggregation>(name, selector, (Action<IAggregationContainer, IScriptedMetricAggregation>) ((a, d) => a.ScriptedMetric = d));
    }

    public AggregationContainerDescriptor<T> AverageBucket(
      string name,
      Func<AverageBucketAggregationDescriptor, IAverageBucketAggregation> selector)
    {
      return this._SetInnerAggregation<AverageBucketAggregationDescriptor, IAverageBucketAggregation>(name, selector, (Action<IAggregationContainer, IAverageBucketAggregation>) ((a, d) => a.AverageBucket = d));
    }

    public AggregationContainerDescriptor<T> Derivative(
      string name,
      Func<DerivativeAggregationDescriptor, IDerivativeAggregation> selector)
    {
      return this._SetInnerAggregation<DerivativeAggregationDescriptor, IDerivativeAggregation>(name, selector, (Action<IAggregationContainer, IDerivativeAggregation>) ((a, d) => a.Derivative = d));
    }

    public AggregationContainerDescriptor<T> MaxBucket(
      string name,
      Func<MaxBucketAggregationDescriptor, IMaxBucketAggregation> selector)
    {
      return this._SetInnerAggregation<MaxBucketAggregationDescriptor, IMaxBucketAggregation>(name, selector, (Action<IAggregationContainer, IMaxBucketAggregation>) ((a, d) => a.MaxBucket = d));
    }

    public AggregationContainerDescriptor<T> MinBucket(
      string name,
      Func<MinBucketAggregationDescriptor, IMinBucketAggregation> selector)
    {
      return this._SetInnerAggregation<MinBucketAggregationDescriptor, IMinBucketAggregation>(name, selector, (Action<IAggregationContainer, IMinBucketAggregation>) ((a, d) => a.MinBucket = d));
    }

    public AggregationContainerDescriptor<T> SumBucket(
      string name,
      Func<SumBucketAggregationDescriptor, ISumBucketAggregation> selector)
    {
      return this._SetInnerAggregation<SumBucketAggregationDescriptor, ISumBucketAggregation>(name, selector, (Action<IAggregationContainer, ISumBucketAggregation>) ((a, d) => a.SumBucket = d));
    }

    public AggregationContainerDescriptor<T> StatsBucket(
      string name,
      Func<StatsBucketAggregationDescriptor, IStatsBucketAggregation> selector)
    {
      return this._SetInnerAggregation<StatsBucketAggregationDescriptor, IStatsBucketAggregation>(name, selector, (Action<IAggregationContainer, IStatsBucketAggregation>) ((a, d) => a.StatsBucket = d));
    }

    public AggregationContainerDescriptor<T> ExtendedStatsBucket(
      string name,
      Func<ExtendedStatsBucketAggregationDescriptor, IExtendedStatsBucketAggregation> selector)
    {
      return this._SetInnerAggregation<ExtendedStatsBucketAggregationDescriptor, IExtendedStatsBucketAggregation>(name, selector, (Action<IAggregationContainer, IExtendedStatsBucketAggregation>) ((a, d) => a.ExtendedStatsBucket = d));
    }

    public AggregationContainerDescriptor<T> PercentilesBucket(
      string name,
      Func<PercentilesBucketAggregationDescriptor, IPercentilesBucketAggregation> selector)
    {
      return this._SetInnerAggregation<PercentilesBucketAggregationDescriptor, IPercentilesBucketAggregation>(name, selector, (Action<IAggregationContainer, IPercentilesBucketAggregation>) ((a, d) => a.PercentilesBucket = d));
    }

    public AggregationContainerDescriptor<T> MovingAverage(
      string name,
      Func<MovingAverageAggregationDescriptor, IMovingAverageAggregation> selector)
    {
      return this._SetInnerAggregation<MovingAverageAggregationDescriptor, IMovingAverageAggregation>(name, selector, (Action<IAggregationContainer, IMovingAverageAggregation>) ((a, d) => a.MovingAverage = d));
    }

    public AggregationContainerDescriptor<T> MovingFunction(
      string name,
      Func<MovingFunctionAggregationDescriptor, IMovingFunctionAggregation> selector)
    {
      return this._SetInnerAggregation<MovingFunctionAggregationDescriptor, IMovingFunctionAggregation>(name, selector, (Action<IAggregationContainer, IMovingFunctionAggregation>) ((a, d) => a.MovingFunction = d));
    }

    public AggregationContainerDescriptor<T> MovingPercentiles(
      string name,
      Func<MovingPercentilesAggregationDescriptor, IMovingPercentilesAggregation> selector)
    {
      return this._SetInnerAggregation<MovingPercentilesAggregationDescriptor, IMovingPercentilesAggregation>(name, selector, (Action<IAggregationContainer, IMovingPercentilesAggregation>) ((a, d) => a.MovingPercentiles = d));
    }

    public AggregationContainerDescriptor<T> CumulativeSum(
      string name,
      Func<CumulativeSumAggregationDescriptor, ICumulativeSumAggregation> selector)
    {
      return this._SetInnerAggregation<CumulativeSumAggregationDescriptor, ICumulativeSumAggregation>(name, selector, (Action<IAggregationContainer, ICumulativeSumAggregation>) ((a, d) => a.CumulativeSum = d));
    }

    public AggregationContainerDescriptor<T> CumulativeCardinality(
      string name,
      Func<CumulativeCardinalityAggregationDescriptor, ICumulativeCardinalityAggregation> selector)
    {
      return this._SetInnerAggregation<CumulativeCardinalityAggregationDescriptor, ICumulativeCardinalityAggregation>(name, selector, (Action<IAggregationContainer, ICumulativeCardinalityAggregation>) ((a, d) => a.CumulativeCardinality = d));
    }

    public AggregationContainerDescriptor<T> SerialDifferencing(
      string name,
      Func<SerialDifferencingAggregationDescriptor, ISerialDifferencingAggregation> selector)
    {
      return this._SetInnerAggregation<SerialDifferencingAggregationDescriptor, ISerialDifferencingAggregation>(name, selector, (Action<IAggregationContainer, ISerialDifferencingAggregation>) ((a, d) => a.SerialDifferencing = d));
    }

    public AggregationContainerDescriptor<T> BucketScript(
      string name,
      Func<BucketScriptAggregationDescriptor, IBucketScriptAggregation> selector)
    {
      return this._SetInnerAggregation<BucketScriptAggregationDescriptor, IBucketScriptAggregation>(name, selector, (Action<IAggregationContainer, IBucketScriptAggregation>) ((a, d) => a.BucketScript = d));
    }

    public AggregationContainerDescriptor<T> BucketSelector(
      string name,
      Func<BucketSelectorAggregationDescriptor, IBucketSelectorAggregation> selector)
    {
      return this._SetInnerAggregation<BucketSelectorAggregationDescriptor, IBucketSelectorAggregation>(name, selector, (Action<IAggregationContainer, IBucketSelectorAggregation>) ((a, d) => a.BucketSelector = d));
    }

    public AggregationContainerDescriptor<T> BucketSort(
      string name,
      Func<BucketSortAggregationDescriptor<T>, IBucketSortAggregation> selector)
    {
      return this._SetInnerAggregation<BucketSortAggregationDescriptor<T>, IBucketSortAggregation>(name, selector, (Action<IAggregationContainer, IBucketSortAggregation>) ((a, d) => a.BucketSort = d));
    }

    public AggregationContainerDescriptor<T> Sampler(
      string name,
      Func<SamplerAggregationDescriptor<T>, ISamplerAggregation> selector)
    {
      return this._SetInnerAggregation<SamplerAggregationDescriptor<T>, ISamplerAggregation>(name, selector, (Action<IAggregationContainer, ISamplerAggregation>) ((a, d) => a.Sampler = d));
    }

    public AggregationContainerDescriptor<T> DiversifiedSampler(
      string name,
      Func<DiversifiedSamplerAggregationDescriptor<T>, IDiversifiedSamplerAggregation> selector)
    {
      return this._SetInnerAggregation<DiversifiedSamplerAggregationDescriptor<T>, IDiversifiedSamplerAggregation>(name, selector, (Action<IAggregationContainer, IDiversifiedSamplerAggregation>) ((a, d) => a.DiversifiedSampler = d));
    }

    public AggregationContainerDescriptor<T> GeoCentroid(
      string name,
      Func<GeoCentroidAggregationDescriptor<T>, IGeoCentroidAggregation> selector)
    {
      return this._SetInnerAggregation<GeoCentroidAggregationDescriptor<T>, IGeoCentroidAggregation>(name, selector, (Action<IAggregationContainer, IGeoCentroidAggregation>) ((a, d) => a.GeoCentroid = d));
    }

    public AggregationContainerDescriptor<T> MatrixStats(
      string name,
      Func<MatrixStatsAggregationDescriptor<T>, IMatrixStatsAggregation> selector)
    {
      return this._SetInnerAggregation<MatrixStatsAggregationDescriptor<T>, IMatrixStatsAggregation>(name, selector, (Action<IAggregationContainer, IMatrixStatsAggregation>) ((a, d) => a.MatrixStats = d));
    }

    public AggregationContainerDescriptor<T> AdjacencyMatrix(
      string name,
      Func<AdjacencyMatrixAggregationDescriptor<T>, IAdjacencyMatrixAggregation> selector)
    {
      return this._SetInnerAggregation<AdjacencyMatrixAggregationDescriptor<T>, IAdjacencyMatrixAggregation>(name, selector, (Action<IAggregationContainer, IAdjacencyMatrixAggregation>) ((a, d) => a.AdjacencyMatrix = d));
    }

    public AggregationContainerDescriptor<T> Composite(
      string name,
      Func<CompositeAggregationDescriptor<T>, ICompositeAggregation> selector)
    {
      return this._SetInnerAggregation<CompositeAggregationDescriptor<T>, ICompositeAggregation>(name, selector, (Action<IAggregationContainer, ICompositeAggregation>) ((a, d) => a.Composite = d));
    }

    public AggregationContainerDescriptor<T> WeightedAverage(
      string name,
      Func<WeightedAverageAggregationDescriptor<T>, IWeightedAverageAggregation> selector)
    {
      return this._SetInnerAggregation<WeightedAverageAggregationDescriptor<T>, IWeightedAverageAggregation>(name, selector, (Action<IAggregationContainer, IWeightedAverageAggregation>) ((a, d) => a.WeightedAverage = d));
    }

    public AggregationContainerDescriptor<T> MedianAbsoluteDeviation(
      string name,
      Func<MedianAbsoluteDeviationAggregationDescriptor<T>, IMedianAbsoluteDeviationAggregation> selector)
    {
      return this._SetInnerAggregation<MedianAbsoluteDeviationAggregationDescriptor<T>, IMedianAbsoluteDeviationAggregation>(name, selector, (Action<IAggregationContainer, IMedianAbsoluteDeviationAggregation>) ((a, d) => a.MedianAbsoluteDeviation = d));
    }

    public AggregationContainerDescriptor<T> StringStats(
      string name,
      Func<StringStatsAggregationDescriptor<T>, IStringStatsAggregation> selector)
    {
      return this._SetInnerAggregation<StringStatsAggregationDescriptor<T>, IStringStatsAggregation>(name, selector, (Action<IAggregationContainer, IStringStatsAggregation>) ((a, d) => a.StringStats = d));
    }

    public AggregationContainerDescriptor<T> Boxplot(
      string name,
      Func<BoxplotAggregationDescriptor<T>, IBoxplotAggregation> selector)
    {
      return this._SetInnerAggregation<BoxplotAggregationDescriptor<T>, IBoxplotAggregation>(name, selector, (Action<IAggregationContainer, IBoxplotAggregation>) ((a, d) => a.Boxplot = d));
    }

    public AggregationContainerDescriptor<T> TopMetrics(
      string name,
      Func<TopMetricsAggregationDescriptor<T>, ITopMetricsAggregation> selector)
    {
      return this._SetInnerAggregation<TopMetricsAggregationDescriptor<T>, ITopMetricsAggregation>(name, selector, (Action<IAggregationContainer, ITopMetricsAggregation>) ((a, d) => a.TopMetrics = d));
    }

    public AggregationContainerDescriptor<T> VariableWidthHistogram(
      string name,
      Func<VariableWidthHistogramAggregationDescriptor<T>, IVariableWidthHistogramAggregation> selector)
    {
      return this._SetInnerAggregation<VariableWidthHistogramAggregationDescriptor<T>, IVariableWidthHistogramAggregation>(name, selector, (Action<IAggregationContainer, IVariableWidthHistogramAggregation>) ((a, d) => a.VariableWidthHistogram = d));
    }

    private AggregationContainerDescriptor<T> _SetInnerAggregation<TAggregator, TAggregatorInterface>(
      string key,
      Func<TAggregator, TAggregatorInterface> selector,
      Action<IAggregationContainer, TAggregatorInterface> assignToProperty)
      where TAggregator : IAggregation, TAggregatorInterface, new()
      where TAggregatorInterface : IAggregation
    {
      TAggregatorInterface aggregatorInterface = selector(new TAggregator());
      AggregationContainer aggregationContainer1 = new AggregationContainer()
      {
        Meta = aggregatorInterface.Meta
      };
      assignToProperty((IAggregationContainer) aggregationContainer1, aggregatorInterface);
      IAggregationContainer aggregationContainer2 = (IAggregationContainer) this;
      if (aggregationContainer2.Aggregations == null)
        aggregationContainer2.Aggregations = (AggregationDictionary) new Dictionary<string, IAggregationContainer>();
      if (aggregatorInterface is IBucketAggregation bucketAggregation && bucketAggregation.Aggregations.HasAny<KeyValuePair<string, IAggregationContainer>>())
        aggregationContainer1.Aggregations = bucketAggregation.Aggregations;
      aggregationContainer2.Aggregations[key] = (IAggregationContainer) aggregationContainer1;
      return this;
    }

    public static bool operator false(AggregationContainerDescriptor<T> a) => false;

    public static bool operator true(AggregationContainerDescriptor<T> a) => false;

    public static AggregationContainerDescriptor<T> operator &(
      AggregationContainerDescriptor<T> left,
      AggregationContainerDescriptor<T> right)
    {
      if (right == null)
        return left;
      if (left == null)
        return right;
      if (object.Equals((object) left, (object) right))
        return left;
      AggregationContainerDescriptor<T> containerDescriptor = new AggregationContainerDescriptor<T>();
      IDictionary<string, IAggregationContainer> aggregations = (IDictionary<string, IAggregationContainer>) ((IAggregationContainer) left).Aggregations;
      foreach (KeyValuePair<string, IAggregationContainer> aggregation in (IEnumerable<KeyValuePair<string, IAggregationContainer>>) ((IAggregationContainer) right).Aggregations)
      {
        if (aggregations.ContainsKey(aggregation.Key))
          throw new Exception("Can not merge two AggregationContainerDescriptor's" + " " + aggregation.Key + " is defined in both");
        aggregations.Add(aggregation.Key, aggregation.Value);
      }
      ((IAggregationContainer) containerDescriptor).Aggregations = ((IAggregationContainer) left).Aggregations;
      return containerDescriptor;
    }
  }
}
