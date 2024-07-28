// Decompiled with JetBrains decompiler
// Type: Nest.AggregationWalker
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AggregationWalker
  {
    private void Accept(IAggregationVisitor visitor, AggregationDictionary aggregations)
    {
      if (!aggregations.HasAny<KeyValuePair<string, IAggregationContainer>>())
        return;
      foreach (KeyValuePair<string, IAggregationContainer> aggregation in (IEnumerable<KeyValuePair<string, IAggregationContainer>>) aggregations)
        this.Accept(visitor, aggregation.Value, AggregationVisitorScope.Bucket);
    }

    private void Accept(
      IAggregationVisitor visitor,
      IAggregationContainer aggregation,
      AggregationVisitorScope scope = AggregationVisitorScope.Aggregation)
    {
      if (aggregation == null)
        return;
      visitor.Scope = scope;
      aggregation.Accept(visitor);
    }

    private static void AcceptAggregation<T>(
      T aggregation,
      IAggregationVisitor visitor,
      Action<IAggregationVisitor, T> scoped)
      where T : class, IAggregation
    {
      if ((object) aggregation == null)
        return;
      ++visitor.Depth;
      visitor.Visit((IAggregation) aggregation);
      scoped(visitor, aggregation);
      --visitor.Depth;
    }

    public void Walk(IAggregationContainer aggregation, IAggregationVisitor visitor)
    {
      visitor.Visit(aggregation);
      AggregationWalker.AcceptAggregation<IAverageAggregation>(aggregation.Average, visitor, (Action<IAggregationVisitor, IAverageAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IAverageBucketAggregation>(aggregation.AverageBucket, visitor, (Action<IAggregationVisitor, IAverageBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IBoxplotAggregation>(aggregation.Boxplot, visitor, (Action<IAggregationVisitor, IBoxplotAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IBucketScriptAggregation>(aggregation.BucketScript, visitor, (Action<IAggregationVisitor, IBucketScriptAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IBucketSortAggregation>(aggregation.BucketSort, visitor, (Action<IAggregationVisitor, IBucketSortAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IBucketSelectorAggregation>(aggregation.BucketSelector, visitor, (Action<IAggregationVisitor, IBucketSelectorAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ICardinalityAggregation>(aggregation.Cardinality, visitor, (Action<IAggregationVisitor, ICardinalityAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IChildrenAggregation>(aggregation.Children, visitor, (Action<IAggregationVisitor, IChildrenAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<ICumulativeSumAggregation>(aggregation.CumulativeSum, visitor, (Action<IAggregationVisitor, ICumulativeSumAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ICumulativeCardinalityAggregation>(aggregation.CumulativeCardinality, visitor, (Action<IAggregationVisitor, ICumulativeCardinalityAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IDateHistogramAggregation>(aggregation.DateHistogram, visitor, (Action<IAggregationVisitor, IDateHistogramAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IDateRangeAggregation>(aggregation.DateRange, visitor, (Action<IAggregationVisitor, IDateRangeAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IDerivativeAggregation>(aggregation.Derivative, visitor, (Action<IAggregationVisitor, IDerivativeAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IExtendedStatsAggregation>(aggregation.ExtendedStats, visitor, (Action<IAggregationVisitor, IExtendedStatsAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IFilterAggregation>(aggregation.Filter, visitor, (Action<IAggregationVisitor, IFilterAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IFiltersAggregation>(aggregation.Filters, visitor, (Action<IAggregationVisitor, IFiltersAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IGeoBoundsAggregation>(aggregation.GeoBounds, visitor, (Action<IAggregationVisitor, IGeoBoundsAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IGeoDistanceAggregation>(aggregation.GeoDistance, visitor, (Action<IAggregationVisitor, IGeoDistanceAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IGeoHashGridAggregation>(aggregation.GeoHash, visitor, (Action<IAggregationVisitor, IGeoHashGridAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IGeoLineAggregation>(aggregation.GeoLine, visitor, (Action<IAggregationVisitor, IGeoLineAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IGeoTileGridAggregation>(aggregation.GeoTile, visitor, (Action<IAggregationVisitor, IGeoTileGridAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IGlobalAggregation>(aggregation.Global, visitor, (Action<IAggregationVisitor, IGlobalAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IHistogramAggregation>(aggregation.Histogram, visitor, (Action<IAggregationVisitor, IHistogramAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IIpRangeAggregation>(aggregation.IpRange, visitor, (Action<IAggregationVisitor, IIpRangeAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IMaxAggregation>(aggregation.Max, visitor, (Action<IAggregationVisitor, IMaxAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMaxBucketAggregation>(aggregation.MaxBucket, visitor, (Action<IAggregationVisitor, IMaxBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMinAggregation>(aggregation.Min, visitor, (Action<IAggregationVisitor, IMinAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMinBucketAggregation>(aggregation.MinBucket, visitor, (Action<IAggregationVisitor, IMinBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMissingAggregation>(aggregation.Missing, visitor, (Action<IAggregationVisitor, IMissingAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IMovingAverageAggregation>(aggregation.MovingAverage, visitor, (Action<IAggregationVisitor, IMovingAverageAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMovingPercentilesAggregation>(aggregation.MovingPercentiles, visitor, (Action<IAggregationVisitor, IMovingPercentilesAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMultiTermsAggregation>(aggregation.MultiTerms, visitor, (Action<IAggregationVisitor, IMultiTermsAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<INestedAggregation>(aggregation.Nested, visitor, (Action<IAggregationVisitor, INestedAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<INormalizeAggregation>(aggregation.Normalize, visitor, (Action<IAggregationVisitor, INormalizeAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IParentAggregation>(aggregation.Parent, visitor, (Action<IAggregationVisitor, IParentAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IPercentileRanksAggregation>(aggregation.PercentileRanks, visitor, (Action<IAggregationVisitor, IPercentileRanksAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IPercentilesAggregation>(aggregation.Percentiles, visitor, (Action<IAggregationVisitor, IPercentilesAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IRangeAggregation>(aggregation.Range, visitor, (Action<IAggregationVisitor, IRangeAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IRareTermsAggregation>(aggregation.RareTerms, visitor, (Action<IAggregationVisitor, IRareTermsAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IRateAggregation>(aggregation.Rate, visitor, (Action<IAggregationVisitor, IRateAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IReverseNestedAggregation>(aggregation.ReverseNested, visitor, (Action<IAggregationVisitor, IReverseNestedAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<ISamplerAggregation>(aggregation.Sampler, visitor, (Action<IAggregationVisitor, ISamplerAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IScriptedMetricAggregation>(aggregation.ScriptedMetric, visitor, (Action<IAggregationVisitor, IScriptedMetricAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ISerialDifferencingAggregation>(aggregation.SerialDifferencing, visitor, (Action<IAggregationVisitor, ISerialDifferencingAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ISignificantTermsAggregation>(aggregation.SignificantTerms, visitor, (Action<IAggregationVisitor, ISignificantTermsAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<ISignificantTextAggregation>(aggregation.SignificantText, visitor, (Action<IAggregationVisitor, ISignificantTextAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<IStatsAggregation>(aggregation.Stats, visitor, (Action<IAggregationVisitor, IStatsAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ISumAggregation>(aggregation.Sum, visitor, (Action<IAggregationVisitor, ISumAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ISumBucketAggregation>(aggregation.SumBucket, visitor, (Action<IAggregationVisitor, ISumBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IStatsBucketAggregation>(aggregation.StatsBucket, visitor, (Action<IAggregationVisitor, IStatsBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IExtendedStatsBucketAggregation>(aggregation.ExtendedStatsBucket, visitor, (Action<IAggregationVisitor, IExtendedStatsBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IPercentilesBucketAggregation>(aggregation.PercentilesBucket, visitor, (Action<IAggregationVisitor, IPercentilesBucketAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ITermsAggregation>(aggregation.Terms, visitor, (Action<IAggregationVisitor, ITermsAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
      AggregationWalker.AcceptAggregation<ITopHitsAggregation>(aggregation.TopHits, visitor, (Action<IAggregationVisitor, ITopHitsAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IValueCountAggregation>(aggregation.ValueCount, visitor, (Action<IAggregationVisitor, IValueCountAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IGeoCentroidAggregation>(aggregation.GeoCentroid, visitor, (Action<IAggregationVisitor, IGeoCentroidAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ICompositeAggregation>(aggregation.Composite, visitor, (Action<IAggregationVisitor, ICompositeAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IMedianAbsoluteDeviationAggregation>(aggregation.MedianAbsoluteDeviation, visitor, (Action<IAggregationVisitor, IMedianAbsoluteDeviationAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<ITTestAggregation>(aggregation.TTest, visitor, (Action<IAggregationVisitor, ITTestAggregation>) ((v, d) => v.Visit(d)));
      AggregationWalker.AcceptAggregation<IVariableWidthHistogramAggregation>(aggregation.VariableWidthHistogram, visitor, (Action<IAggregationVisitor, IVariableWidthHistogramAggregation>) ((v, d) =>
      {
        v.Visit(d);
        this.Accept(v, d.Aggregations);
      }));
    }
  }
}
