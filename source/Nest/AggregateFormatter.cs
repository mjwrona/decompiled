// Decompiled with JetBrains decompiler
// Type: Nest.AggregateFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nest
{
  internal class AggregateFormatter : IJsonFormatter<IAggregate>, IJsonFormatter
  {
    private static readonly byte[] BgCountField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("bg_count");
    private static readonly AutomataDictionary BucketFields = new AutomataDictionary()
    {
      {
        "key",
        0
      },
      {
        "from",
        1
      },
      {
        "to",
        2
      },
      {
        "key_as_string",
        3
      },
      {
        "doc_count",
        4
      },
      {
        "min",
        5
      }
    };
    private static readonly byte[] BucketsField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("buckets");
    private static readonly byte[] DocCountErrorUpperBound = JsonWriter.GetEncodedPropertyNameWithoutQuotation("doc_count_error_upper_bound");
    private static readonly byte[] FieldsField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("fields");
    private static readonly AutomataDictionary GeoBoundsFields = new AutomataDictionary()
    {
      {
        "top_left",
        0
      },
      {
        "bottom_right",
        1
      }
    };
    private static readonly AutomataDictionary GeoLineFields = new AutomataDictionary()
    {
      {
        "geometry",
        0
      },
      {
        "properties",
        1
      }
    };
    private static readonly byte[] KeysField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("keys");
    private static readonly byte[] MetaField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("meta");
    private static readonly byte[] MinLengthField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("min_length");
    private static readonly AutomataDictionary RootFields = new AutomataDictionary()
    {
      {
        "values",
        0
      },
      {
        "value",
        1
      },
      {
        "after_key",
        2
      },
      {
        "buckets",
        3
      },
      {
        "doc_count_error_upper_bound",
        4
      },
      {
        "count",
        5
      },
      {
        "doc_count",
        6
      },
      {
        "bounds",
        7
      },
      {
        "hits",
        8
      },
      {
        "location",
        9
      },
      {
        "fields",
        10
      },
      {
        "min",
        11
      },
      {
        "top",
        12
      },
      {
        "type",
        13
      }
    };
    private static readonly byte[] SumOtherDocCount = JsonWriter.GetEncodedPropertyNameWithoutQuotation("sum_other_doc_count");
    private static readonly AutomataDictionary TopHitsFields = new AutomataDictionary()
    {
      {
        "total",
        0
      },
      {
        "max_score",
        1
      },
      {
        "hits",
        2
      }
    };
    private static readonly AutomataDictionary ExtendedStatsFields = new AutomataDictionary()
    {
      {
        "variance",
        0
      },
      {
        "std_deviation",
        1
      },
      {
        "std_deviation_bounds",
        2
      },
      {
        "variance_population",
        3
      },
      {
        "variance_sampling",
        4
      },
      {
        "std_deviation_population",
        5
      },
      {
        "std_deviation_sampling",
        6
      }
    };
    private static readonly byte[] ValueAsStringField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("value_as_string");

    public static string[] AllReservedAggregationNames { get; } = ((IEnumerable<FieldInfo>) typeof (AggregateFormatter.Parser).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)).Where<FieldInfo>((Func<FieldInfo, bool>) (f => f.IsLiteral && !f.IsInitOnly)).Select<FieldInfo, string>((Func<FieldInfo, string>) (f => (string) f.GetValue((object) null))).ToArray<string>();

    public static string UsingReservedAggNameFormat { get; } = "'{0}' is one of the reserved aggregation keywords we use a heuristics based response parser and using these reserved keywords could throw its heuristics off course. We are working on a solution in Elasticsearch itself to make the response parseable. For now these are all the reserved keywords: " + string.Join(", ", AggregateFormatter.AllReservedAggregationNames);

    public IAggregate Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => this.ReadAggregate(ref reader, formatterResolver);

    public void Serialize(
      ref JsonWriter writer,
      IAggregate value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    private IAggregate ReadAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      reader.ReadIsBeginObjectWithVerify();
      IAggregate aggregate = (IAggregate) null;
      if (reader.ReadIsEndObject())
        return aggregate;
      ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
      Dictionary<string, object> meta = (Dictionary<string, object>) null;
      if (bytes.EqualsBytes(AggregateFormatter.MetaField))
      {
        meta = this.GetMetadata(ref reader, formatterResolver);
        reader.ReadIsValueSeparatorWithVerify();
        bytes = reader.ReadPropertyNameSegmentRaw();
      }
      int num;
      if (AggregateFormatter.RootFields.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            aggregate = this.GetPercentilesAggregate(ref reader, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 1:
            aggregate = this.GetValueAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 2:
            CompositeKey compositeKey = formatterResolver.GetFormatter<CompositeKey>().Deserialize(ref reader, formatterResolver);
            reader.ReadNext();
            ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
            if (!arraySegment.EqualsBytes(AggregateFormatter.BucketsField))
              bucketAggregate1 = new BucketAggregate()
              {
                Meta = (IReadOnlyDictionary<string, object>) meta
              };
            else if (!(this.GetMultiBucketAggregate(ref reader, formatterResolver, ref arraySegment, (IReadOnlyDictionary<string, object>) meta) is BucketAggregate bucketAggregate1))
              bucketAggregate1 = new BucketAggregate()
              {
                Meta = (IReadOnlyDictionary<string, object>) meta
              };
            BucketAggregate bucketAggregate2 = bucketAggregate1;
            bucketAggregate2.AfterKey = compositeKey;
            aggregate = (IAggregate) bucketAggregate2;
            break;
          case 3:
          case 4:
            aggregate = this.GetMultiBucketAggregate(ref reader, formatterResolver, ref bytes, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 5:
            aggregate = this.GetStatsAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 6:
            aggregate = this.GetSingleBucketAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 7:
            aggregate = this.GetGeoBoundsAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 8:
            aggregate = this.GetTopHitsAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 9:
            aggregate = this.GetGeoCentroidAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 10:
            aggregate = this.GetMatrixStatsAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 11:
            aggregate = this.GetBoxplotAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 12:
            aggregate = this.GetTopMetricsAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
          case 13:
            aggregate = this.GetGeoLineAggregate(ref reader, formatterResolver, (IReadOnlyDictionary<string, object>) meta);
            break;
        }
      }
      else
        reader.ReadNextBlock();
      reader.ReadIsEndObjectWithVerify();
      return aggregate;
    }

    private IBucket ReadBucket(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IBucket) null;
      reader.ReadNext();
      ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
      IBucket bucket = (IBucket) null;
      int num;
      if (AggregateFormatter.BucketFields.TryGetValue(segment, out num))
      {
        switch (num)
        {
          case 0:
            bucket = this.GetKeyedBucket(ref reader, formatterResolver);
            break;
          case 1:
          case 2:
            bucket = this.GetRangeBucket(ref reader, formatterResolver, (string) null, segment.Utf8String());
            break;
          case 3:
            bucket = this.GetDateHistogramBucket(ref reader, formatterResolver);
            break;
          case 4:
            bucket = this.GetFiltersBucket(ref reader, formatterResolver);
            break;
          case 5:
            bucket = this.GetVariableWidthHistogramBucket(ref reader, formatterResolver);
            break;
        }
      }
      else
        reader.ReadNextBlock();
      reader.ReadNext();
      return bucket;
    }

    private Dictionary<string, object> GetMetadata(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return formatterResolver.GetFormatter<Dictionary<string, object>>().Deserialize(ref reader, formatterResolver);
    }

    private IAggregate GetMatrixStatsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta,
      long? docCount = null)
    {
      MatrixStatsAggregate matrixStatsAggregate1 = new MatrixStatsAggregate();
      matrixStatsAggregate1.DocCount = docCount.GetValueOrDefault();
      matrixStatsAggregate1.Meta = meta;
      MatrixStatsAggregate matrixStatsAggregate2 = matrixStatsAggregate1;
      IJsonFormatter<List<MatrixStatsField>> formatter = formatterResolver.GetFormatter<List<MatrixStatsField>>();
      matrixStatsAggregate2.Fields = formatter.Deserialize(ref reader, formatterResolver);
      return (IAggregate) matrixStatsAggregate2;
    }

    private IAggregate GetBoxplotAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      StringDoubleFormatter stringDoubleFormatter = new StringDoubleFormatter();
      BoxplotAggregate boxplotAggregate1 = new BoxplotAggregate();
      boxplotAggregate1.Min = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      boxplotAggregate1.Meta = meta;
      BoxplotAggregate boxplotAggregate2 = boxplotAggregate1;
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      boxplotAggregate2.Max = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      boxplotAggregate2.Q1 = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      boxplotAggregate2.Q2 = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      boxplotAggregate2.Q3 = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      if (reader.GetCurrentJsonToken() != JsonToken.EndObject)
      {
        reader.ReadNext();
        reader.ReadNext();
        reader.ReadNext();
        boxplotAggregate2.Lower = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
        reader.ReadNext();
        reader.ReadNext();
        reader.ReadNext();
        boxplotAggregate2.Upper = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
      }
      return (IAggregate) boxplotAggregate2;
    }

    private IAggregate GetTopMetricsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      TopMetricsAggregate metricsAggregate1 = new TopMetricsAggregate();
      metricsAggregate1.Meta = meta;
      TopMetricsAggregate metricsAggregate2 = metricsAggregate1;
      IJsonFormatter<List<TopMetric>> formatter = formatterResolver.GetFormatter<List<TopMetric>>();
      metricsAggregate2.Top = (IReadOnlyCollection<TopMetric>) formatter.Deserialize(ref reader, formatterResolver);
      return (IAggregate) metricsAggregate2;
    }

    private IAggregate GetTopHitsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      int count = 0;
      double? nullable = new double?();
      TotalHits totalHits = (TotalHits) null;
      List<LazyDocument> hits = (List<LazyDocument>) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (AggregateFormatter.TopHitsFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              totalHits = formatterResolver.GetFormatter<TotalHits>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              nullable = reader.ReadNullableDouble();
              continue;
            case 2:
              hits = formatterResolver.GetFormatter<List<LazyDocument>>().Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      TopHitsAggregate topHitsAggregate = new TopHitsAggregate((IList<LazyDocument>) hits, formatterResolver);
      topHitsAggregate.Total = totalHits;
      topHitsAggregate.MaxScore = nullable;
      topHitsAggregate.Meta = meta;
      return (IAggregate) topHitsAggregate;
    }

    private IAggregate GetGeoCentroidAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      IJsonFormatter<GeoLocation> formatter = formatterResolver.GetFormatter<GeoLocation>();
      GeoCentroidAggregate centroidAggregate1 = new GeoCentroidAggregate();
      centroidAggregate1.Location = formatter.Deserialize(ref reader, formatterResolver);
      centroidAggregate1.Meta = meta;
      GeoCentroidAggregate centroidAggregate2 = centroidAggregate1;
      if (reader.GetCurrentJsonToken() == JsonToken.EndObject)
        return (IAggregate) centroidAggregate2;
      reader.ReadNext();
      if (reader.ReadPropertyName() == "count")
        centroidAggregate2.Count = reader.ReadInt64();
      return (IAggregate) centroidAggregate2;
    }

    private IAggregate GetGeoBoundsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (IAggregate) null;
      }
      GeoBoundsAggregate geoBoundsAggregate1 = new GeoBoundsAggregate();
      geoBoundsAggregate1.Meta = meta;
      GeoBoundsAggregate geoBoundsAggregate2 = geoBoundsAggregate1;
      IJsonFormatter<LatLon> formatter = formatterResolver.GetFormatter<LatLon>();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (AggregateFormatter.GeoBoundsFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              geoBoundsAggregate2.Bounds.TopLeft = formatter.Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              geoBoundsAggregate2.Bounds.BottomRight = formatter.Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return (IAggregate) geoBoundsAggregate2;
    }

    private IAggregate GetGeoLineAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      GeoLineAggregate geoLineAggregate1 = new GeoLineAggregate();
      geoLineAggregate1.Meta = meta;
      GeoLineAggregate geoLineAggregate2 = geoLineAggregate1;
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (IAggregate) geoLineAggregate2;
      }
      IJsonFormatter<LineStringGeoShape> formatter1 = formatterResolver.GetFormatter<LineStringGeoShape>();
      IJsonFormatter<GeoLineProperties> formatter2 = formatterResolver.GetFormatter<GeoLineProperties>();
      geoLineAggregate2.Type = reader.ReadString();
      reader.ReadNext();
      for (int index = 0; index < 2; ++index)
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (AggregateFormatter.GeoLineFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              geoLineAggregate2.Geometry = formatter1.Deserialize(ref reader, formatterResolver);
              reader.ReadNextBlock();
              continue;
            case 1:
              geoLineAggregate2.Properties = formatter2.Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return (IAggregate) geoLineAggregate2;
    }

    private IAggregate GetPercentilesAggregate(
      ref JsonReader reader,
      IReadOnlyDictionary<string, object> meta)
    {
      PercentilesAggregate percentilesAggregate1 = new PercentilesAggregate();
      percentilesAggregate1.Meta = meta;
      PercentilesAggregate percentilesAggregate2 = percentilesAggregate1;
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      switch (currentJsonToken)
      {
        case JsonToken.BeginObject:
        case JsonToken.BeginArray:
          int count = 0;
          if (currentJsonToken == JsonToken.BeginObject)
          {
            while (reader.ReadIsInObject(ref count))
            {
              string s = reader.ReadPropertyName();
              if (s.Contains("_as_string"))
                reader.ReadNextBlock();
              else
                percentilesAggregate2.Items.Add(new PercentileItem()
                {
                  Percentile = double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture),
                  Value = reader.ReadNullableDouble()
                });
            }
          }
          else
          {
            while (reader.ReadIsInArray(ref count))
            {
              reader.ReadNext();
              reader.ReadNext();
              reader.ReadNext();
              double num = reader.ReadDouble();
              reader.ReadNext();
              reader.ReadNext();
              reader.ReadNext();
              percentilesAggregate2.Items.Add(new PercentileItem()
              {
                Percentile = num,
                Value = reader.ReadNullableDouble()
              });
              reader.ReadNext();
            }
          }
          return (IAggregate) percentilesAggregate2;
        default:
          reader.ReadNextBlock();
          return (IAggregate) percentilesAggregate2;
      }
    }

    private IAggregate GetSingleBucketAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      long num1 = reader.ReadInt64();
      JsonToken currentJsonToken = reader.GetCurrentJsonToken();
      Dictionary<string, IAggregate> aggregations = (Dictionary<string, IAggregate>) null;
      if (currentJsonToken == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        long num2 = 0;
        ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
        if (arraySegment.EqualsBytes(AggregateFormatter.BgCountField))
        {
          num2 = reader.ReadInt64();
          reader.ReadIsValueSeparatorWithVerify();
          arraySegment = reader.ReadPropertyNameSegmentRaw();
        }
        if (arraySegment.EqualsBytes(AggregateFormatter.FieldsField))
          return this.GetMatrixStatsAggregate(ref reader, formatterResolver, meta, new long?(num1));
        if (arraySegment.EqualsBytes(AggregateFormatter.BucketsField))
        {
          BucketAggregate multiBucketAggregate = this.GetMultiBucketAggregate(ref reader, formatterResolver, ref arraySegment, meta) as BucketAggregate;
          return (IAggregate) new BucketAggregate()
          {
            BgCount = num2,
            DocCount = num1,
            Items = (multiBucketAggregate?.Items ?? EmptyReadOnly<IBucket>.Collection),
            Meta = meta
          };
        }
        aggregations = this.GetSubAggregates(ref reader, arraySegment.Utf8String(), formatterResolver);
      }
      SingleBucketAggregate singleBucketAggregate = new SingleBucketAggregate((IReadOnlyDictionary<string, IAggregate>) aggregations);
      singleBucketAggregate.DocCount = num1;
      singleBucketAggregate.Meta = meta;
      return (IAggregate) singleBucketAggregate;
    }

    private IAggregate GetStringStatsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta,
      long count)
    {
      int num1 = reader.ReadInt32();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      int num2 = reader.ReadInt32();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double num3 = reader.ReadDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double num4 = reader.ReadDouble();
      StringStatsAggregate stringStatsAggregate1 = new StringStatsAggregate();
      stringStatsAggregate1.Meta = meta;
      stringStatsAggregate1.Count = count;
      stringStatsAggregate1.MinLength = num1;
      stringStatsAggregate1.MaxLength = num2;
      stringStatsAggregate1.AverageLength = num3;
      stringStatsAggregate1.Entropy = num4;
      StringStatsAggregate stringStatsAggregate2 = stringStatsAggregate1;
      if (reader.ReadIsValueSeparator())
      {
        reader.ReadNext();
        reader.ReadNext();
        IReadOnlyDictionary<string, double> readOnlyDictionary = formatterResolver.GetFormatter<IReadOnlyDictionary<string, double>>().Deserialize(ref reader, formatterResolver);
        stringStatsAggregate2.Distribution = readOnlyDictionary;
      }
      return (IAggregate) stringStatsAggregate2;
    }

    private IAggregate GetStatsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      long valueOrDefault = reader.ReadNullableLong().GetValueOrDefault(0L);
      if (reader.GetCurrentJsonToken() == JsonToken.EndObject)
      {
        GeoCentroidAggregate statsAggregate = new GeoCentroidAggregate();
        statsAggregate.Count = valueOrDefault;
        statsAggregate.Meta = meta;
        return (IAggregate) statsAggregate;
      }
      reader.ReadNext();
      ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
      if (arraySegment.EqualsBytes(AggregateFormatter.MinLengthField))
        return this.GetStringStatsAggregate(ref reader, formatterResolver, meta, valueOrDefault);
      double? nullable1 = reader.ReadNullableDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double? nullable2 = reader.ReadNullableDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double? nullable3 = reader.ReadNullableDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double num = reader.ReadDouble();
      StatsAggregate statsAggregate1 = new StatsAggregate();
      statsAggregate1.Average = nullable3;
      statsAggregate1.Count = valueOrDefault;
      statsAggregate1.Max = nullable2;
      statsAggregate1.Min = nullable1;
      statsAggregate1.Sum = num;
      statsAggregate1.Meta = meta;
      StatsAggregate statsMetric = statsAggregate1;
      if (reader.GetCurrentJsonToken() == JsonToken.EndObject)
        return (IAggregate) statsMetric;
      reader.ReadNext();
      string str = reader.ReadPropertyName();
      while (reader.GetCurrentJsonToken() != JsonToken.EndObject && str.EndsWith("_as_string"))
      {
        reader.ReadNext();
        if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
        {
          reader.ReadNext();
          str = reader.ReadPropertyName();
        }
      }
      return reader.GetCurrentJsonToken() == JsonToken.EndObject ? (IAggregate) statsMetric : this.GetExtendedStatsAggregate(ref reader, formatterResolver, statsMetric, meta);
    }

    private IAggregate GetExtendedStatsAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      StatsAggregate statsMetric,
      IReadOnlyDictionary<string, object> meta)
    {
      ExtendedStatsAggregate extendedStatsAggregate1 = new ExtendedStatsAggregate();
      extendedStatsAggregate1.Average = statsMetric.Average;
      extendedStatsAggregate1.Count = statsMetric.Count;
      extendedStatsAggregate1.Max = statsMetric.Max;
      extendedStatsAggregate1.Min = statsMetric.Min;
      extendedStatsAggregate1.Sum = statsMetric.Sum;
      extendedStatsAggregate1.Meta = statsMetric.Meta;
      ExtendedStatsAggregate extendedStatsAggregate2 = extendedStatsAggregate1;
      extendedStatsAggregate2.SumOfSquares = reader.ReadNullableDouble();
      reader.ReadNext();
      NullableStringDoubleFormatter stringDoubleFormatter = new NullableStringDoubleFormatter();
      while (reader.GetCurrentJsonToken() != JsonToken.EndObject)
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (AggregateFormatter.ExtendedStatsFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              extendedStatsAggregate2.Variance = reader.ReadNullableDouble();
              break;
            case 1:
              extendedStatsAggregate2.StdDeviation = reader.ReadNullableDouble();
              break;
            case 2:
              extendedStatsAggregate2.StdDeviationBounds = formatterResolver.GetFormatter<StandardDeviationBounds>().Deserialize(ref reader, formatterResolver);
              break;
            case 3:
              extendedStatsAggregate2.VariancePopulation = reader.ReadNullableDouble();
              break;
            case 4:
              extendedStatsAggregate2.VarianceSampling = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
              break;
            case 5:
              extendedStatsAggregate2.StdDeviationPopulation = reader.ReadNullableDouble();
              break;
            case 6:
              extendedStatsAggregate2.StdDeviationSampling = stringDoubleFormatter.Deserialize(ref reader, formatterResolver);
              break;
          }
        }
        else
          reader.ReadNextBlock();
        reader.ReadIsValueSeparator();
      }
      return (IAggregate) extendedStatsAggregate2;
    }

    private Dictionary<string, IAggregate> GetSubAggregates(
      ref JsonReader reader,
      string name,
      IJsonFormatterResolver formatterResolver)
    {
      Dictionary<string, IAggregate> subAggregates = new Dictionary<string, IAggregate>();
      IAggregate aggregate1 = this.Deserialize(ref reader, formatterResolver);
      subAggregates.Add(name, aggregate1);
      while (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        name = reader.ReadPropertyName();
        IAggregate aggregate2 = this.Deserialize(ref reader, formatterResolver);
        subAggregates.Add(name, aggregate2);
      }
      return subAggregates;
    }

    private IAggregate GetMultiBucketAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      ref ArraySegment<byte> propertyName,
      IReadOnlyDictionary<string, object> meta)
    {
      BucketAggregate multiBucketAggregate1 = new BucketAggregate()
      {
        Meta = meta
      };
      if (propertyName.EqualsBytes(AggregateFormatter.DocCountErrorUpperBound))
      {
        multiBucketAggregate1.DocCountErrorUpperBound = reader.ReadNullableLong();
        reader.ReadIsValueSeparatorWithVerify();
        propertyName = reader.ReadPropertyNameSegmentRaw();
      }
      if (propertyName.EqualsBytes(AggregateFormatter.SumOtherDocCount))
      {
        multiBucketAggregate1.SumOtherDocCount = reader.ReadNullableLong();
        reader.ReadIsValueSeparatorWithVerify();
        reader.ReadNext();
        reader.ReadNext();
      }
      List<IBucket> bucketList = new List<IBucket>();
      multiBucketAggregate1.Items = (IReadOnlyCollection<IBucket>) bucketList;
      int count = 0;
      if (reader.GetCurrentJsonToken() == JsonToken.BeginObject)
      {
        Dictionary<string, IAggregate> aggregations = new Dictionary<string, IAggregate>();
        while (reader.ReadIsInObject(ref count))
        {
          string key = reader.ReadPropertyName();
          IAggregate aggregate = this.ReadAggregate(ref reader, formatterResolver);
          aggregations[key] = aggregate;
        }
        FiltersAggregate multiBucketAggregate2 = new FiltersAggregate((IReadOnlyDictionary<string, IAggregate>) aggregations);
        multiBucketAggregate2.Meta = meta;
        return (IAggregate) multiBucketAggregate2;
      }
      while (reader.ReadIsInArray(ref count))
      {
        IBucket bucket = this.ReadBucket(ref reader, formatterResolver);
        bucketList.Add(bucket);
      }
      if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        propertyName = reader.ReadPropertyNameSegmentRaw();
        if (propertyName.EqualsBytes(JsonWriter.GetEncodedPropertyNameWithoutQuotation("interval")))
          multiBucketAggregate1.AutoInterval = formatterResolver.GetFormatter<DateMathTime>().Deserialize(ref reader, formatterResolver);
        else
          reader.ReadNextBlock();
      }
      return (IAggregate) multiBucketAggregate1;
    }

    private IAggregate GetValueAggregate(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      IReadOnlyDictionary<string, object> meta)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.Number:
        case JsonToken.Null:
          double? nullable = reader.ReadNullableDouble();
          string str = (string) null;
          JsonToken currentJsonToken = reader.GetCurrentJsonToken();
          if (currentJsonToken != JsonToken.EndObject)
          {
            reader.ReadNext();
            ArraySegment<byte> arraySegment = reader.ReadPropertyNameSegmentRaw();
            if (arraySegment.EqualsBytes(AggregateFormatter.ValueAsStringField))
            {
              str = reader.ReadString();
              currentJsonToken = reader.GetCurrentJsonToken();
              if (currentJsonToken == JsonToken.EndObject)
              {
                ValueAggregate valueAggregate = new ValueAggregate();
                valueAggregate.Value = nullable;
                valueAggregate.ValueAsString = str;
                valueAggregate.Meta = meta;
                return (IAggregate) valueAggregate;
              }
              reader.ReadNext();
              arraySegment = reader.ReadPropertyNameSegmentRaw();
            }
            if (arraySegment.EqualsBytes(AggregateFormatter.KeysField))
            {
              KeyedValueAggregate keyedValueAggregate = new KeyedValueAggregate();
              keyedValueAggregate.Value = nullable;
              keyedValueAggregate.Meta = meta;
              KeyedValueAggregate valueAggregate = keyedValueAggregate;
              IJsonFormatter<List<string>> formatter = formatterResolver.GetFormatter<List<string>>();
              valueAggregate.Keys = (IList<string>) formatter.Deserialize(ref reader, formatterResolver);
              return (IAggregate) valueAggregate;
            }
            for (; currentJsonToken != JsonToken.EndObject; currentJsonToken = reader.GetCurrentJsonToken())
              reader.ReadNextBlock();
          }
          ValueAggregate valueAggregate1 = new ValueAggregate();
          valueAggregate1.Value = nullable;
          valueAggregate1.ValueAsString = str;
          valueAggregate1.Meta = meta;
          return (IAggregate) valueAggregate1;
        default:
          ArraySegment<byte> src = reader.ReadNextBlockSegment();
          ScriptedMetricAggregate valueAggregate2 = new ScriptedMetricAggregate((object) new LazyDocument(BinaryUtil.ToArray(ref src), formatterResolver));
          valueAggregate2.Meta = meta;
          return (IAggregate) valueAggregate2;
      }
    }

    public IBucket GetRangeBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      string key,
      string propertyName)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      long? nullable1 = new long?();
      double? nullable2 = new double?();
      double? nullable3 = new double?();
      bool flag = false;
      while (true)
      {
        switch (propertyName)
        {
          case "from":
            switch (reader.GetCurrentJsonToken())
            {
              case JsonToken.Number:
                nullable3 = new double?(reader.ReadDouble());
                break;
              case JsonToken.String:
                str2 = reader.ReadString();
                break;
              default:
                reader.ReadNext();
                break;
            }
            break;
          case "to":
            switch (reader.GetCurrentJsonToken())
            {
              case JsonToken.Number:
                nullable2 = new double?(reader.ReadDouble());
                break;
              case JsonToken.String:
                str4 = reader.ReadString();
                break;
              default:
                reader.ReadNext();
                break;
            }
            break;
          case nameof (key):
            key = reader.ReadString();
            break;
          case "from_as_string":
            str1 = reader.ReadString();
            break;
          case "to_as_string":
            str3 = reader.ReadString();
            break;
          case "doc_count":
            nullable1 = new long?(reader.ReadNullableLong().GetValueOrDefault(0L));
            break;
          default:
            flag = true;
            break;
        }
        if (!flag && reader.GetCurrentJsonToken() != JsonToken.EndObject)
        {
          reader.ReadNext();
          propertyName = reader.ReadPropertyName();
        }
        else
          break;
      }
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      if (flag)
        dict = this.GetSubAggregates(ref reader, propertyName, formatterResolver);
      IBucket rangeBucket;
      if (str2 != null || str4 != null)
        rangeBucket = (IBucket) new IpRangeBucket((IReadOnlyDictionary<string, IAggregate>) dict)
        {
          Key = key,
          DocCount = nullable1.GetValueOrDefault(),
          From = str2,
          To = str4
        };
      else
        rangeBucket = (IBucket) new RangeBucket((IReadOnlyDictionary<string, IAggregate>) dict)
        {
          Key = key,
          From = nullable3,
          To = nullable2,
          DocCount = nullable1.GetValueOrDefault(),
          FromAsString = str1,
          ToAsString = str3
        };
      return rangeBucket;
    }

    private IBucket GetDateHistogramBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      string str = reader.ReadString();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      long num1 = reader.ReadInt64();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      long num2 = reader.ReadInt64();
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        string name = reader.ReadPropertyName();
        dict = this.GetSubAggregates(ref reader, name, formatterResolver);
      }
      DateHistogramBucket dateHistogramBucket = new DateHistogramBucket((IReadOnlyDictionary<string, IAggregate>) dict);
      dateHistogramBucket.Key = (double) num1;
      dateHistogramBucket.KeyAsString = str;
      dateHistogramBucket.DocCount = new long?(num2);
      return (IBucket) dateHistogramBucket;
    }

    private IBucket GetVariableWidthHistogramBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      double num1 = reader.ReadDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double num2 = reader.ReadDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      double num3 = reader.ReadDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      long num4 = reader.ReadInt64();
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        string name = reader.ReadPropertyName();
        dict = this.GetSubAggregates(ref reader, name, formatterResolver);
      }
      return (IBucket) new VariableWidthHistogramBucket((IReadOnlyDictionary<string, IAggregate>) dict)
      {
        Key = num2,
        Minimum = num1,
        Maximum = num3,
        DocCount = num4
      };
    }

    private IBucket GetKeyedBucket(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      object key1;
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          return this.GetCompositeBucket(ref reader, formatterResolver);
        case JsonToken.BeginArray:
          List<object> objectList = new List<object>();
          int count = 0;
          while (reader.ReadIsInArray(ref count))
          {
            object obj;
            switch (reader.GetCurrentJsonToken())
            {
              case JsonToken.String:
                obj = (object) reader.ReadString();
                break;
              case JsonToken.True:
              case JsonToken.False:
                obj = (object) reader.ReadBoolean();
                break;
              default:
                ArraySegment<byte> arraySegment = reader.ReadNumberSegment();
                int readCount;
                obj = !arraySegment.IsLong() ? (object) NumberConverter.ReadDouble(arraySegment.Array, arraySegment.Offset, out readCount) : (object) NumberConverter.ReadInt64(arraySegment.Array, arraySegment.Offset, out readCount);
                break;
            }
            objectList.Add(obj);
          }
          key1 = (object) objectList;
          break;
        case JsonToken.String:
          key1 = (object) reader.ReadString();
          break;
        default:
          ArraySegment<byte> arraySegment1 = reader.ReadNumberSegment();
          int readCount1;
          key1 = !arraySegment1.IsLong() ? (object) NumberConverter.ReadDouble(arraySegment1.Array, arraySegment1.Offset, out readCount1) : (object) NumberConverter.ReadInt64(arraySegment1.Array, arraySegment1.Offset, out readCount1);
          break;
      }
      reader.ReadNext();
      string propertyName = reader.ReadPropertyName();
      if (propertyName == "from" || propertyName == "to")
      {
        string key2 = !(key1 is double num) ? key1.ToString() : num.ToString("#.#");
        return this.GetRangeBucket(ref reader, formatterResolver, key2, propertyName);
      }
      string str = (string) null;
      if (propertyName == "key_as_string")
      {
        str = reader.ReadString();
        reader.ReadNext();
        reader.ReadNext();
        reader.ReadNext();
      }
      long num1 = reader.ReadInt64();
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      long? nullable = new long?();
      if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        string name1 = reader.ReadPropertyName();
        switch (name1)
        {
          case "score":
            return this.GetSignificantTermsBucket(ref reader, formatterResolver, key1, new long?(num1));
          case "doc_count_error_upper_bound":
            nullable = reader.ReadNullableLong();
            if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
            {
              reader.ReadNext();
              string name2 = reader.ReadPropertyName();
              dict = this.GetSubAggregates(ref reader, name2, formatterResolver);
              break;
            }
            break;
          default:
            dict = this.GetSubAggregates(ref reader, name1, formatterResolver);
            break;
        }
      }
      return (IBucket) new KeyedBucket<object>((IReadOnlyDictionary<string, IAggregate>) dict)
      {
        Key = key1,
        KeyAsString = str,
        DocCount = new long?(num1),
        DocCountErrorUpperBound = nullable
      };
    }

    private IBucket GetCompositeBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      CompositeKey key = new CompositeKey(formatterResolver.GetFormatter<IReadOnlyDictionary<string, object>>().Deserialize(ref reader, formatterResolver));
      long? nullable = new long?();
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      while (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        string name = reader.ReadPropertyName();
        if (name == "doc_count")
          nullable = reader.ReadNullableLong();
        else
          dict = this.GetSubAggregates(ref reader, name, formatterResolver);
      }
      return (IBucket) new CompositeBucket((IReadOnlyDictionary<string, IAggregate>) dict, key)
      {
        DocCount = nullable
      };
    }

    private IBucket GetSignificantTermsBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver,
      object key,
      long? docCount)
    {
      double num1 = reader.ReadDouble();
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      long num2 = reader.ReadInt64();
      Dictionary<string, IAggregate> dict = (Dictionary<string, IAggregate>) null;
      if (reader.GetCurrentJsonToken() == JsonToken.ValueSeparator)
      {
        reader.ReadNext();
        string name = reader.ReadPropertyName();
        dict = this.GetSubAggregates(ref reader, name, formatterResolver);
      }
      return (IBucket) new SignificantTermsBucket<object>((IReadOnlyDictionary<string, IAggregate>) dict)
      {
        Key = key,
        DocCount = docCount.GetValueOrDefault(0L),
        BgCount = num2,
        Score = num1
      };
    }

    private IBucket GetFiltersBucket(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      long valueOrDefault = reader.ReadNullableLong().GetValueOrDefault(0L);
      if (reader.GetCurrentJsonToken() == JsonToken.EndObject)
        return (IBucket) new FiltersBucketItem(EmptyReadOnly<string, IAggregate>.Dictionary)
        {
          DocCount = valueOrDefault
        };
      reader.ReadNext();
      string name = reader.ReadPropertyName();
      return (IBucket) new FiltersBucketItem((IReadOnlyDictionary<string, IAggregate>) this.GetSubAggregates(ref reader, name, formatterResolver))
      {
        DocCount = valueOrDefault
      };
    }

    private static class Parser
    {
      public const string AfterKey = "after_key";
      public const string AsStringSuffix = "_as_string";
      public const string BgCount = "bg_count";
      public const string BottomRight = "bottom_right";
      public const string Bounds = "bounds";
      public const string Buckets = "buckets";
      public const string Count = "count";
      public const string DocCount = "doc_count";
      public const string DocCountErrorUpperBound = "doc_count_error_upper_bound";
      public const string Fields = "fields";
      public const string From = "from";
      public const string Top = "top";
      public const string Type = "type";
      public const string FromAsString = "from_as_string";
      public const string Hits = "hits";
      public const string Key = "key";
      public const string KeyAsString = "key_as_string";
      public const string Keys = "keys";
      public const string Location = "location";
      public const string MaxScore = "max_score";
      public const string Meta = "meta";
      public const string Min = "min";
      public const string MinLength = "min_length";
      public const string Score = "score";
      public const string SumOtherDocCount = "sum_other_doc_count";
      public const string To = "to";
      public const string ToAsString = "to_as_string";
      public const string TopLeft = "top_left";
      public const string Total = "total";
      public const string Value = "value";
      public const string ValueAsString = "value_as_string";
      public const string Values = "values";
      public const string Geometry = "geometry";
      public const string Properties = "properties";
    }
  }
}
