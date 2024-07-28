// Decompiled with JetBrains decompiler
// Type: Nest.AggregateDictionary
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  [JsonFormatter(typeof (AggregateDictionaryFormatter))]
  public class AggregateDictionary : IsAReadOnlyDictionaryBase<string, IAggregate>
  {
    internal static readonly char[] TypedKeysSeparator = new char[1]
    {
      '#'
    };

    [SerializationConstructor]
    public AggregateDictionary(
      IReadOnlyDictionary<string, IAggregate> backingDictionary)
      : base(backingDictionary)
    {
    }

    public static AggregateDictionary Default { get; } = new AggregateDictionary(EmptyReadOnly<string, IAggregate>.Dictionary);

    protected override string Sanitize(string key)
    {
      string[] strArray = AggregateDictionary.TypedKeyTokens(key);
      return strArray.Length <= 1 ? strArray[0] : strArray[1];
    }

    internal static string[] TypedKeyTokens(string key) => key.Split(AggregateDictionary.TypedKeysSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

    public ValueAggregate Min(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate Max(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate Sum(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate Cardinality(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate Average(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate ValueCount(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate AverageBucket(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate Derivative(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate SumBucket(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate MovingAverage(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate CumulativeSum(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate CumulativeCardinality(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate BucketScript(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate SerialDifferencing(string key) => this.TryGet<ValueAggregate>(key);

    public ValueAggregate WeightedAverage(string key) => this.TryGet<ValueAggregate>(key);

    public KeyedValueAggregate MaxBucket(string key) => this.TryGet<KeyedValueAggregate>(key);

    public KeyedValueAggregate MinBucket(string key) => this.TryGet<KeyedValueAggregate>(key);

    public ScriptedMetricAggregate ScriptedMetric(string key)
    {
      ValueAggregate valueAggregate = this.TryGet<ValueAggregate>(key);
      if (valueAggregate == null)
        return this.TryGet<ScriptedMetricAggregate>(key);
      ScriptedMetricAggregate scriptedMetricAggregate = new ScriptedMetricAggregate((object) valueAggregate.Value);
      scriptedMetricAggregate.Meta = valueAggregate.Meta;
      return scriptedMetricAggregate;
    }

    public StatsAggregate Stats(string key) => this.TryGet<StatsAggregate>(key);

    public StringStatsAggregate StringStats(string key) => this.TryGet<StringStatsAggregate>(key);

    public TopMetricsAggregate TopMetrics(string key) => this.TryGet<TopMetricsAggregate>(key);

    public StatsAggregate StatsBucket(string key) => this.TryGet<StatsAggregate>(key);

    public ExtendedStatsAggregate ExtendedStats(string key) => this.TryGet<ExtendedStatsAggregate>(key);

    public ExtendedStatsAggregate ExtendedStatsBucket(string key) => this.TryGet<ExtendedStatsAggregate>(key);

    public GeoBoundsAggregate GeoBounds(string key) => this.TryGet<GeoBoundsAggregate>(key);

    public GeoLineAggregate GeoLine(string key) => this.TryGet<GeoLineAggregate>(key);

    public PercentilesAggregate Percentiles(string key) => this.TryGet<PercentilesAggregate>(key);

    public PercentilesAggregate PercentilesBucket(string key) => this.TryGet<PercentilesAggregate>(key);

    public PercentilesAggregate MovingPercentiles(string key) => this.TryGet<PercentilesAggregate>(key);

    public PercentilesAggregate PercentileRanks(string key) => this.TryGet<PercentilesAggregate>(key);

    public TopHitsAggregate TopHits(string key) => this.TryGet<TopHitsAggregate>(key);

    public FiltersAggregate Filters(string key)
    {
      FiltersAggregate filtersAggregate1 = this.TryGet<FiltersAggregate>(key);
      if (filtersAggregate1 != null)
        return filtersAggregate1;
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (FiltersAggregate) null;
      FiltersAggregate filtersAggregate2 = new FiltersAggregate();
      filtersAggregate2.Buckets = (IReadOnlyCollection<FiltersBucketItem>) bucketAggregate.Items.OfType<FiltersBucketItem>().ToList<FiltersBucketItem>();
      filtersAggregate2.Meta = bucketAggregate.Meta;
      return filtersAggregate2;
    }

    public SingleBucketAggregate Global(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Filter(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Missing(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Nested(string key) => this.TryGet<SingleBucketAggregate>(key);

    public ValueAggregate Normalize(string key) => this.TryGet<ValueAggregate>(key);

    public SingleBucketAggregate ReverseNested(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Children(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Parent(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate Sampler(string key) => this.TryGet<SingleBucketAggregate>(key);

    public SingleBucketAggregate DiversifiedSampler(string key) => this.TryGet<SingleBucketAggregate>(key);

    public GeoCentroidAggregate GeoCentroid(string key) => this.TryGet<GeoCentroidAggregate>(key);

    public SignificantTermsAggregate<TKey> SignificantTerms<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (SignificantTermsAggregate<TKey>) null;
      SignificantTermsAggregate<TKey> significantTermsAggregate = new SignificantTermsAggregate<TKey>();
      significantTermsAggregate.BgCount = new long?(bucketAggregate.BgCount);
      significantTermsAggregate.DocCount = bucketAggregate.DocCount;
      significantTermsAggregate.Buckets = (IReadOnlyCollection<SignificantTermsBucket<TKey>>) this.GetSignificantTermsBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<SignificantTermsBucket<TKey>>();
      significantTermsAggregate.Meta = bucketAggregate.Meta;
      return significantTermsAggregate;
    }

    public SignificantTermsAggregate<string> SignificantTerms(string key) => this.SignificantTerms<string>(key);

    public SignificantTermsAggregate<TKey> SignificantText<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (SignificantTermsAggregate<TKey>) null;
      SignificantTermsAggregate<TKey> significantTermsAggregate = new SignificantTermsAggregate<TKey>();
      significantTermsAggregate.BgCount = new long?(bucketAggregate.BgCount);
      significantTermsAggregate.DocCount = bucketAggregate.DocCount;
      significantTermsAggregate.Buckets = (IReadOnlyCollection<SignificantTermsBucket<TKey>>) this.GetSignificantTermsBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<SignificantTermsBucket<TKey>>();
      significantTermsAggregate.Meta = bucketAggregate.Meta;
      return significantTermsAggregate;
    }

    public SignificantTermsAggregate<string> SignificantText(string key) => this.SignificantText<string>(key);

    public TermsAggregate<TKey> Terms<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (TermsAggregate<TKey>) null;
      TermsAggregate<TKey> termsAggregate = new TermsAggregate<TKey>();
      termsAggregate.DocCountErrorUpperBound = bucketAggregate.DocCountErrorUpperBound;
      termsAggregate.SumOtherDocCount = bucketAggregate.SumOtherDocCount;
      termsAggregate.Buckets = (IReadOnlyCollection<KeyedBucket<TKey>>) this.GetKeyedBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<KeyedBucket<TKey>>();
      termsAggregate.Meta = bucketAggregate.Meta;
      return termsAggregate;
    }

    public TermsAggregate<string> Terms(string key) => this.Terms<string>(key);

    public MultiBucketAggregate<KeyedBucket<double>> Histogram(string key) => this.GetMultiKeyedBucketAggregate<double>(key);

    public MultiBucketAggregate<KeyedBucket<string>> GeoHash(string key) => this.GetMultiKeyedBucketAggregate<string>(key);

    public MultiBucketAggregate<KeyedBucket<string>> GeoTile(string key) => this.GetMultiKeyedBucketAggregate<string>(key);

    public MultiBucketAggregate<KeyedBucket<string>> AdjacencyMatrix(string key) => this.GetMultiKeyedBucketAggregate<string>(key);

    public MultiBucketAggregate<RareTermsBucket<TKey>> RareTerms<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (MultiBucketAggregate<RareTermsBucket<TKey>>) null;
      return new MultiBucketAggregate<RareTermsBucket<TKey>>()
      {
        Buckets = (IReadOnlyCollection<RareTermsBucket<TKey>>) this.GetRareTermsBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<RareTermsBucket<TKey>>(),
        Meta = bucketAggregate.Meta
      };
    }

    public ValueAggregate Rate(string key) => this.TryGet<ValueAggregate>(key);

    public MultiBucketAggregate<RareTermsBucket<string>> RareTerms(string key) => this.RareTerms<string>(key);

    public MultiBucketAggregate<RangeBucket> Range(string key) => this.GetMultiBucketAggregate<RangeBucket>(key);

    public MultiBucketAggregate<RangeBucket> DateRange(string key) => this.GetMultiBucketAggregate<RangeBucket>(key);

    public MultiBucketAggregate<IpRangeBucket> IpRange(string key) => this.GetMultiBucketAggregate<IpRangeBucket>(key);

    public MultiBucketAggregate<RangeBucket> GeoDistance(string key) => this.GetMultiBucketAggregate<RangeBucket>(key);

    public MultiBucketAggregate<DateHistogramBucket> DateHistogram(string key) => this.GetMultiBucketAggregate<DateHistogramBucket>(key);

    public MultiBucketAggregate<VariableWidthHistogramBucket> VariableWidthHistogram(string key) => this.GetMultiBucketAggregate<VariableWidthHistogramBucket>(key);

    public MultiTermsAggregate<string> MultiTerms(string key) => this.MultiTerms<string>(key);

    public MultiTermsAggregate<TKey> MultiTerms<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (MultiTermsAggregate<TKey>) null;
      MultiTermsAggregate<TKey> multiTermsAggregate = new MultiTermsAggregate<TKey>();
      multiTermsAggregate.DocCountErrorUpperBound = bucketAggregate.DocCountErrorUpperBound;
      multiTermsAggregate.SumOtherDocCount = bucketAggregate.SumOtherDocCount;
      multiTermsAggregate.Buckets = (IReadOnlyCollection<MultiTermsBucket<TKey>>) AggregateDictionary.GetMultiTermsBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<MultiTermsBucket<TKey>>();
      multiTermsAggregate.Meta = bucketAggregate.Meta;
      return multiTermsAggregate;
    }

    public AutoDateHistogramAggregate AutoDateHistogram(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (AutoDateHistogramAggregate) null;
      AutoDateHistogramAggregate histogramAggregate = new AutoDateHistogramAggregate();
      histogramAggregate.Buckets = (IReadOnlyCollection<DateHistogramBucket>) bucketAggregate.Items.OfType<DateHistogramBucket>().ToList<DateHistogramBucket>();
      histogramAggregate.Meta = bucketAggregate.Meta;
      histogramAggregate.AutoInterval = bucketAggregate.AutoInterval;
      return histogramAggregate;
    }

    public CompositeBucketAggregate Composite(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (CompositeBucketAggregate) null;
      CompositeBucketAggregate compositeBucketAggregate = new CompositeBucketAggregate();
      compositeBucketAggregate.Buckets = (IReadOnlyCollection<CompositeBucket>) bucketAggregate.Items.OfType<CompositeBucket>().ToList<CompositeBucket>();
      compositeBucketAggregate.Meta = bucketAggregate.Meta;
      compositeBucketAggregate.AfterKey = bucketAggregate.AfterKey;
      return compositeBucketAggregate;
    }

    public MatrixStatsAggregate MatrixStats(string key) => this.TryGet<MatrixStatsAggregate>(key);

    public ValueAggregate MedianAbsoluteDeviation(string key) => this.TryGet<ValueAggregate>(key);

    public BoxplotAggregate Boxplot(string key) => this.TryGet<BoxplotAggregate>(key);

    public ValueAggregate TTest(string key) => this.TryGet<ValueAggregate>(key);

    private TAggregate TryGet<TAggregate>(string key) where TAggregate : class, IAggregate
    {
      IAggregate aggregate;
      return !this.BackingDictionary.TryGetValue(key, out aggregate) ? default (TAggregate) : aggregate as TAggregate;
    }

    private MultiBucketAggregate<TBucket> GetMultiBucketAggregate<TBucket>(string key) where TBucket : IBucket
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (MultiBucketAggregate<TBucket>) null;
      return new MultiBucketAggregate<TBucket>()
      {
        Buckets = (IReadOnlyCollection<TBucket>) bucketAggregate.Items.OfType<TBucket>().ToList<TBucket>(),
        Meta = bucketAggregate.Meta
      };
    }

    private MultiBucketAggregate<KeyedBucket<TKey>> GetMultiKeyedBucketAggregate<TKey>(string key)
    {
      BucketAggregate bucketAggregate = this.TryGet<BucketAggregate>(key);
      if (bucketAggregate == null)
        return (MultiBucketAggregate<KeyedBucket<TKey>>) null;
      return new MultiBucketAggregate<KeyedBucket<TKey>>()
      {
        Buckets = (IReadOnlyCollection<KeyedBucket<TKey>>) this.GetKeyedBuckets<TKey>((IEnumerable<IBucket>) bucketAggregate.Items).ToList<KeyedBucket<TKey>>(),
        Meta = bucketAggregate.Meta
      };
    }

    private IEnumerable<KeyedBucket<TKey>> GetKeyedBuckets<TKey>(IEnumerable<IBucket> items)
    {
      foreach (KeyedBucket<object> keyedBucket in items.Cast<KeyedBucket<object>>())
        yield return new KeyedBucket<TKey>(keyedBucket.BackingDictionary)
        {
          Key = AggregateDictionary.GetKeyFromBucketKey<TKey>(keyedBucket.Key),
          KeyAsString = keyedBucket.KeyAsString,
          DocCount = keyedBucket.DocCount,
          DocCountErrorUpperBound = keyedBucket.DocCountErrorUpperBound
        };
    }

    private IEnumerable<SignificantTermsBucket<TKey>> GetSignificantTermsBuckets<TKey>(
      IEnumerable<IBucket> items)
    {
      foreach (SignificantTermsBucket<object> significantTermsBucket in items.Cast<SignificantTermsBucket<object>>())
        yield return new SignificantTermsBucket<TKey>(significantTermsBucket.BackingDictionary)
        {
          Key = AggregateDictionary.GetKeyFromBucketKey<TKey>(significantTermsBucket.Key),
          BgCount = significantTermsBucket.BgCount,
          DocCount = significantTermsBucket.DocCount,
          Score = significantTermsBucket.Score
        };
    }

    private IEnumerable<RareTermsBucket<TKey>> GetRareTermsBuckets<TKey>(IEnumerable<IBucket> items)
    {
      foreach (KeyedBucket<object> keyedBucket in items.Cast<KeyedBucket<object>>())
        yield return new RareTermsBucket<TKey>(keyedBucket.BackingDictionary)
        {
          Key = AggregateDictionary.GetKeyFromBucketKey<TKey>(keyedBucket.Key),
          DocCount = keyedBucket.DocCount.GetValueOrDefault(0L)
        };
    }

    private static IEnumerable<MultiTermsBucket<TKey>> GetMultiTermsBuckets<TKey>(
      IEnumerable<IBucket> items)
    {
      foreach (KeyedBucket<object> keyedBucket in items.Cast<KeyedBucket<object>>())
      {
        MultiTermsBucket<TKey> multiTermsBucket = new MultiTermsBucket<TKey>(keyedBucket.BackingDictionary)
        {
          DocCount = keyedBucket.DocCount,
          DocCountErrorUpperBound = keyedBucket.DocCountErrorUpperBound,
          KeyAsString = keyedBucket.KeyAsString
        };
        if (keyedBucket.Key is List<object> key)
          multiTermsBucket.Key = (IEnumerable<TKey>) key.Select<object, TKey>(new Func<object, TKey>(AggregateDictionary.GetKeyFromBucketKey<TKey>)).ToList<TKey>();
        else
          multiTermsBucket.Key = (IEnumerable<TKey>) new TKey[1]
          {
            AggregateDictionary.GetKeyFromBucketKey<TKey>(keyedBucket.Key)
          };
        yield return multiTermsBucket;
      }
    }

    private static TKey GetKeyFromBucketKey<TKey>(object key) => !typeof (TKey).IsEnum ? (TKey) Convert.ChangeType(key, typeof (TKey)) : (TKey) Enum.Parse(typeof (TKey), key.ToString(), true);
  }
}
