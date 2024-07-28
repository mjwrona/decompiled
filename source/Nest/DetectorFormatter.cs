// Decompiled with JetBrains decompiler
// Type: Nest.DetectorFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class DetectorFormatter : IJsonFormatter<IDetector>, IJsonFormatter
  {
    private static readonly AutomataDictionary Functions = new AutomataDictionary()
    {
      {
        "count",
        0
      },
      {
        "high_count",
        1
      },
      {
        "low_count",
        2
      },
      {
        "non_zero_count",
        3
      },
      {
        "high_non_zero_count",
        4
      },
      {
        "low_non_zero_count",
        5
      },
      {
        "distinct_count",
        6
      },
      {
        "high_distinct_count",
        7
      },
      {
        "low_distinct_count",
        8
      },
      {
        "lat_long",
        9
      },
      {
        "info_content",
        10
      },
      {
        "high_info_content",
        11
      },
      {
        "low_info_content",
        12
      },
      {
        "min",
        13
      },
      {
        "max",
        14
      },
      {
        "median",
        15
      },
      {
        "high_median",
        16
      },
      {
        "low_median",
        17
      },
      {
        "mean",
        18
      },
      {
        "high_mean",
        19
      },
      {
        "low_mean",
        20
      },
      {
        "metric",
        21
      },
      {
        "varp",
        22
      },
      {
        "high_varp",
        23
      },
      {
        "low_varp",
        24
      },
      {
        "rare",
        25
      },
      {
        "freq_rare",
        26
      },
      {
        "sum",
        27
      },
      {
        "high_sum",
        28
      },
      {
        "low_sum",
        29
      },
      {
        "non_null_sum",
        30
      },
      {
        "high_non_null_sum",
        31
      },
      {
        "low_non_null_sum",
        32
      },
      {
        "time_of_day",
        33
      },
      {
        "time_of_week",
        34
      }
    };

    public IDetector Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (IDetector) null;
      }
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int count = 0;
      ArraySegment<byte> segment = new ArraySegment<byte>();
      while (reader1.ReadIsInObject(ref count))
      {
        if (reader1.ReadPropertyName() == "function")
        {
          segment = reader1.ReadStringSegmentRaw();
          break;
        }
        reader1.ReadNextBlock();
      }
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int num;
      if (DetectorFormatter.Functions.TryGetValue(segment, out num))
      {
        switch (num)
        {
          case 0:
            return (IDetector) DetectorFormatter.Deserialize<CountDetector>(ref reader1, formatterResolver);
          case 1:
            return (IDetector) DetectorFormatter.Deserialize<HighCountDetector>(ref reader1, formatterResolver);
          case 2:
            return (IDetector) DetectorFormatter.Deserialize<LowCountDetector>(ref reader1, formatterResolver);
          case 3:
            return (IDetector) DetectorFormatter.Deserialize<NonZeroCountDetector>(ref reader1, formatterResolver);
          case 4:
            return (IDetector) DetectorFormatter.Deserialize<HighNonZeroCountDetector>(ref reader1, formatterResolver);
          case 5:
            return (IDetector) DetectorFormatter.Deserialize<LowNonZeroCountDetector>(ref reader1, formatterResolver);
          case 6:
            return (IDetector) DetectorFormatter.Deserialize<DistinctCountDetector>(ref reader1, formatterResolver);
          case 7:
            return (IDetector) DetectorFormatter.Deserialize<HighDistinctCountDetector>(ref reader1, formatterResolver);
          case 8:
            return (IDetector) DetectorFormatter.Deserialize<LowDistinctCountDetector>(ref reader1, formatterResolver);
          case 9:
            return (IDetector) DetectorFormatter.Deserialize<LatLongDetector>(ref reader1, formatterResolver);
          case 10:
            return (IDetector) DetectorFormatter.Deserialize<InfoContentDetector>(ref reader1, formatterResolver);
          case 11:
            return (IDetector) DetectorFormatter.Deserialize<HighInfoContentDetector>(ref reader1, formatterResolver);
          case 12:
            return (IDetector) DetectorFormatter.Deserialize<LowInfoContentDetector>(ref reader1, formatterResolver);
          case 13:
            return (IDetector) DetectorFormatter.Deserialize<MinDetector>(ref reader1, formatterResolver);
          case 14:
            return (IDetector) DetectorFormatter.Deserialize<MaxDetector>(ref reader1, formatterResolver);
          case 15:
            return (IDetector) DetectorFormatter.Deserialize<MedianDetector>(ref reader1, formatterResolver);
          case 16:
            return (IDetector) DetectorFormatter.Deserialize<HighMedianDetector>(ref reader1, formatterResolver);
          case 17:
            return (IDetector) DetectorFormatter.Deserialize<LowMedianDetector>(ref reader1, formatterResolver);
          case 18:
            return (IDetector) DetectorFormatter.Deserialize<MeanDetector>(ref reader1, formatterResolver);
          case 19:
            return (IDetector) DetectorFormatter.Deserialize<HighMeanDetector>(ref reader1, formatterResolver);
          case 20:
            return (IDetector) DetectorFormatter.Deserialize<LowMeanDetector>(ref reader1, formatterResolver);
          case 21:
            return (IDetector) DetectorFormatter.Deserialize<MetricDetector>(ref reader1, formatterResolver);
          case 22:
            return (IDetector) DetectorFormatter.Deserialize<VarpDetector>(ref reader1, formatterResolver);
          case 23:
            return (IDetector) DetectorFormatter.Deserialize<HighVarpDetector>(ref reader1, formatterResolver);
          case 24:
            return (IDetector) DetectorFormatter.Deserialize<LowVarpDetector>(ref reader1, formatterResolver);
          case 25:
            return (IDetector) DetectorFormatter.Deserialize<RareDetector>(ref reader1, formatterResolver);
          case 26:
            return (IDetector) DetectorFormatter.Deserialize<FreqRareDetector>(ref reader1, formatterResolver);
          case 27:
            return (IDetector) DetectorFormatter.Deserialize<SumDetector>(ref reader1, formatterResolver);
          case 28:
            return (IDetector) DetectorFormatter.Deserialize<HighSumDetector>(ref reader1, formatterResolver);
          case 29:
            return (IDetector) DetectorFormatter.Deserialize<LowSumDetector>(ref reader1, formatterResolver);
          case 30:
            return (IDetector) DetectorFormatter.Deserialize<NonNullSumDetector>(ref reader1, formatterResolver);
          case 31:
            return (IDetector) DetectorFormatter.Deserialize<HighNonNullSumDetector>(ref reader1, formatterResolver);
          case 32:
            return (IDetector) DetectorFormatter.Deserialize<LowNonNullSumDetector>(ref reader1, formatterResolver);
          case 33:
            return (IDetector) DetectorFormatter.Deserialize<TimeOfDayDetector>(ref reader1, formatterResolver);
          case 34:
            return (IDetector) DetectorFormatter.Deserialize<TimeOfWeekDetector>(ref reader1, formatterResolver);
        }
      }
      throw new Exception("Unknown function " + segment.Utf8String());
    }

    public void Serialize(
      ref JsonWriter writer,
      IDetector value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        string function = value.Function;
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(function))
        {
          case 56500965:
            if (!(function == "low_varp"))
              return;
            goto label_78;
          case 193670821:
            if (!(function == "distinct_count"))
              return;
            goto label_75;
          case 250781252:
            if (!(function == "high_non_zero_count"))
              return;
            goto label_74;
          case 266731151:
            if (!(function == "high_non_null_sum"))
              return;
            goto label_81;
          case 269424939:
            if (!(function == "low_non_null_sum"))
              return;
            goto label_81;
          case 318562538:
            if (!(function == "low_distinct_count"))
              return;
            goto label_75;
          case 319807408:
            if (!(function == "low_non_zero_count"))
              return;
            goto label_74;
          case 349026455:
            if (!(function == "median"))
              return;
            goto label_78;
          case 371815311:
            if (!(function == "info_content"))
              return;
            goto label_77;
          case 462002032:
            if (!(function == "low_info_content"))
              return;
            goto label_77;
          case 483127761:
            if (!(function == "low_count"))
              return;
            break;
          case 967958004:
            if (!(function == "count"))
              return;
            break;
          case 1029165080:
            if (!(function == "freq_rare"))
              return;
            goto label_79;
          case 1202448714:
            if (!(function == "varp"))
              return;
            goto label_78;
          case 1349428199:
            if (!(function == "non_zero_count"))
              return;
            goto label_74;
          case 1504475432:
            if (!(function == "high_median"))
              return;
            goto label_78;
          case 1676374577:
            if (!(function == "time_of_week"))
              return;
            goto label_82;
          case 1925765102:
            if (!(function == "high_distinct_count"))
              return;
            goto label_75;
          case 2005228981:
            if (!(function == "low_sum"))
              return;
            goto label_80;
          case 2408187020:
            if (!(function == "low_median"))
              return;
            goto label_78;
          case 2413851501:
            if (!(function == "lat_long"))
              return;
            DetectorFormatter.Serialize<IGeographicDetector>(ref writer, value, formatterResolver);
            return;
          case 2459073245:
            if (!(function == "rare"))
              return;
            goto label_79;
          case 2665359700:
            if (!(function == "mean"))
              return;
            goto label_78;
          case 2696864057:
            if (!(function == "time_of_day"))
              return;
            goto label_82;
          case 2838550312:
            if (!(function == "non_null_sum"))
              return;
            goto label_81;
          case 3022690485:
            if (!(function == "high_count"))
              return;
            break;
          case 3381609815:
            if (!(function == "min"))
              return;
            goto label_78;
          case 3383706369:
            if (!(function == "metric"))
              return;
            goto label_78;
          case 3519601771:
            if (!(function == "high_mean"))
              return;
            goto label_78;
          case 3617776409:
            if (!(function == "max"))
              return;
            goto label_78;
          case 3670470359:
            if (!(function == "low_mean"))
              return;
            goto label_78;
          case 3689114249:
            if (!(function == "high_varp"))
              return;
            goto label_78;
          case 3712891560:
            if (!(function == "sum"))
              return;
            goto label_80;
          case 3818754636:
            if (!(function == "high_info_content"))
              return;
            goto label_77;
          case 4055015417:
            if (!(function == "high_sum"))
              return;
            goto label_80;
          default:
            return;
        }
        DetectorFormatter.Serialize<ICountDetector>(ref writer, value, formatterResolver);
        return;
label_74:
        DetectorFormatter.Serialize<INonZeroCountDetector>(ref writer, value, formatterResolver);
        return;
label_75:
        DetectorFormatter.Serialize<IDistinctCountDetector>(ref writer, value, formatterResolver);
        return;
label_77:
        DetectorFormatter.Serialize<IInfoContentDetector>(ref writer, value, formatterResolver);
        return;
label_78:
        DetectorFormatter.Serialize<IMetricDetector>(ref writer, value, formatterResolver);
        return;
label_79:
        DetectorFormatter.Serialize<IRareDetector>(ref writer, value, formatterResolver);
        return;
label_80:
        DetectorFormatter.Serialize<ISumDetector>(ref writer, value, formatterResolver);
        return;
label_81:
        DetectorFormatter.Serialize<INonNullSumDetector>(ref writer, value, formatterResolver);
        return;
label_82:
        DetectorFormatter.Serialize<ITimeDetector>(ref writer, value, formatterResolver);
      }
    }

    private static void Serialize<TDetector>(
      ref JsonWriter writer,
      IDetector value,
      IJsonFormatterResolver formatterResolver)
      where TDetector : class, IDetector
    {
      formatterResolver.GetFormatter<TDetector>().Serialize(ref writer, value as TDetector, formatterResolver);
    }

    private static TDetector Deserialize<TDetector>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TDetector : IDetector
    {
      return formatterResolver.GetFormatter<TDetector>().Deserialize(ref reader, formatterResolver);
    }
  }
}
