// Decompiled with JetBrains decompiler
// Type: Nest.MovingAverageAggregationFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class MovingAverageAggregationFormatter : 
    IJsonFormatter<IMovingAverageAggregation>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "format",
        0
      },
      {
        "gap_policy",
        1
      },
      {
        "minimize",
        2
      },
      {
        "predict",
        3
      },
      {
        "window",
        4
      },
      {
        "settings",
        5
      },
      {
        "model",
        6
      },
      {
        "buckets_path",
        7
      }
    };
    private static readonly AutomataDictionary ModelDictionary = new AutomataDictionary()
    {
      {
        "linear",
        0
      },
      {
        "simple",
        1
      },
      {
        "ewma",
        2
      },
      {
        "holt",
        3
      },
      {
        "holt_winters",
        4
      }
    };

    public IMovingAverageAggregation Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IMovingAverageAggregation) null;
      }
      int count = 0;
      MovingAverageAggregation averageAggregation = new MovingAverageAggregation();
      ArraySegment<byte> bytes1 = new ArraySegment<byte>();
      ArraySegment<byte> arraySegment = new ArraySegment<byte>();
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes2 = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (MovingAverageAggregationFormatter.AutomataDictionary.TryGetValue(bytes2, out num))
        {
          switch (num)
          {
            case 0:
              averageAggregation.Format = reader.ReadString();
              continue;
            case 1:
              averageAggregation.GapPolicy = formatterResolver.GetFormatter<GapPolicy?>().Deserialize(ref reader, formatterResolver);
              continue;
            case 2:
              averageAggregation.Minimize = new bool?(reader.ReadBoolean());
              continue;
            case 3:
              averageAggregation.Predict = new int?(reader.ReadInt32());
              continue;
            case 4:
              averageAggregation.Window = new int?(reader.ReadInt32());
              continue;
            case 5:
              arraySegment = reader.ReadNextBlockSegment();
              continue;
            case 6:
              bytes1 = reader.ReadStringSegmentUnsafe();
              continue;
            case 7:
              string bucketsPath = reader.ReadString();
              if (!string.IsNullOrEmpty(bucketsPath))
              {
                averageAggregation.BucketsPath = (IBucketsPath) new SingleBucketsPath(bucketsPath);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      int num1;
      if (bytes1 != new ArraySegment<byte>() && MovingAverageAggregationFormatter.ModelDictionary.TryGetValue(bytes1, out num1))
      {
        JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
        switch (num1)
        {
          case 0:
            averageAggregation.Model = (IMovingAverageModel) formatterResolver.GetFormatter<LinearModel>().Deserialize(ref reader1, formatterResolver);
            break;
          case 1:
            averageAggregation.Model = (IMovingAverageModel) formatterResolver.GetFormatter<SimpleModel>().Deserialize(ref reader1, formatterResolver);
            break;
          case 2:
            averageAggregation.Model = (IMovingAverageModel) formatterResolver.GetFormatter<EwmaModel>().Deserialize(ref reader1, formatterResolver);
            break;
          case 3:
            averageAggregation.Model = (IMovingAverageModel) formatterResolver.GetFormatter<HoltLinearModel>().Deserialize(ref reader1, formatterResolver);
            break;
          case 4:
            averageAggregation.Model = (IMovingAverageModel) formatterResolver.GetFormatter<HoltWintersModel>().Deserialize(ref reader1, formatterResolver);
            break;
        }
      }
      return (IMovingAverageAggregation) averageAggregation;
    }

    public void Serialize(
      ref JsonWriter writer,
      IMovingAverageAggregation value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        bool flag = false;
        if (value.BucketsPath != null)
        {
          writer.WritePropertyName("buckets_path");
          formatterResolver.GetFormatter<IBucketsPath>().Serialize(ref writer, value.BucketsPath, formatterResolver);
          flag = true;
        }
        if (value.GapPolicy.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("gap_policy");
          writer.WriteString(((Enum) (System.ValueType) value.GapPolicy).GetStringValue());
          flag = true;
        }
        if (!value.Format.IsNullOrEmpty())
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("format");
          writer.WriteString(value.Format);
          flag = true;
        }
        int? nullable = value.Window;
        if (nullable.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("window");
          ref JsonWriter local = ref writer;
          nullable = value.Window;
          int num = nullable.Value;
          local.WriteInt32(num);
          flag = true;
        }
        bool? minimize = value.Minimize;
        if (minimize.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("minimize");
          ref JsonWriter local = ref writer;
          minimize = value.Minimize;
          int num = minimize.Value ? 1 : 0;
          local.WriteBoolean(num != 0);
          flag = true;
        }
        nullable = value.Predict;
        if (nullable.HasValue)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("predict");
          ref JsonWriter local = ref writer;
          nullable = value.Predict;
          int num = nullable.Value;
          local.WriteInt32(num);
          flag = true;
        }
        if (value.Model != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("model");
          writer.WriteString(value.Model.Name);
          writer.WriteValueSeparator();
          writer.WritePropertyName("settings");
          switch (value.Model.Name)
          {
            case "ewma":
              MovingAverageAggregationFormatter.Serialize<IEwmaModel>(ref writer, (IEwmaModel) value.Model, formatterResolver);
              break;
            case "linear":
              MovingAverageAggregationFormatter.Serialize<ILinearModel>(ref writer, (ILinearModel) value.Model, formatterResolver);
              break;
            case "simple":
              MovingAverageAggregationFormatter.Serialize<ISimpleModel>(ref writer, (ISimpleModel) value.Model, formatterResolver);
              break;
            case "holt":
              MovingAverageAggregationFormatter.Serialize<IHoltLinearModel>(ref writer, (IHoltLinearModel) value.Model, formatterResolver);
              break;
            case "holt_winters":
              MovingAverageAggregationFormatter.Serialize<IHoltWintersModel>(ref writer, (IHoltWintersModel) value.Model, formatterResolver);
              break;
            default:
              MovingAverageAggregationFormatter.Serialize<IMovingAverageModel>(ref writer, value.Model, formatterResolver);
              break;
          }
        }
        writer.WriteEndObject();
      }
    }

    private static void Serialize<TModel>(
      ref JsonWriter writer,
      TModel model,
      IJsonFormatterResolver formatterResolver)
      where TModel : IMovingAverageModel
    {
      formatterResolver.GetFormatter<TModel>().Serialize(ref writer, model, formatterResolver);
    }
  }
}
