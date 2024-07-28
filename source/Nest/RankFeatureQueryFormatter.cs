// Decompiled with JetBrains decompiler
// Type: Nest.RankFeatureQueryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;

namespace Nest
{
  internal class RankFeatureQueryFormatter : IJsonFormatter<IRankFeatureQuery>, IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "_name",
        0
      },
      {
        "boost",
        1
      },
      {
        "field",
        2
      },
      {
        "saturation",
        3
      },
      {
        "log",
        4
      },
      {
        "sigmoid",
        5
      },
      {
        "linear",
        6
      }
    };

    public void Serialize(
      ref JsonWriter writer,
      IRankFeatureQuery value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        if (!string.IsNullOrEmpty(value.Name))
        {
          writer.WritePropertyName("_name");
          writer.WriteString(value.Name);
          writer.WriteValueSeparator();
        }
        double? boost = value.Boost;
        if (boost.HasValue)
        {
          writer.WritePropertyName("boost");
          ref JsonWriter local = ref writer;
          boost = value.Boost;
          double num = boost.Value;
          local.WriteDouble(num);
          writer.WriteValueSeparator();
        }
        writer.WritePropertyName("field");
        formatterResolver.GetFormatter<Field>().Serialize(ref writer, value.Field, formatterResolver);
        if (value.Function != null)
        {
          writer.WriteValueSeparator();
          switch (value.Function)
          {
            case IRankFeatureSigmoidFunction scoreFunction1:
              RankFeatureQueryFormatter.SerializeScoreFunction<IRankFeatureSigmoidFunction>(ref writer, "sigmoid", scoreFunction1, formatterResolver);
              break;
            case IRankFeatureSaturationFunction scoreFunction2:
              RankFeatureQueryFormatter.SerializeScoreFunction<IRankFeatureSaturationFunction>(ref writer, "saturation", scoreFunction2, formatterResolver);
              break;
            case IRankFeatureLogarithmFunction scoreFunction3:
              RankFeatureQueryFormatter.SerializeScoreFunction<IRankFeatureLogarithmFunction>(ref writer, "log", scoreFunction3, formatterResolver);
              break;
            case IRankFeatureLinearFunction scoreFunction4:
              RankFeatureQueryFormatter.SerializeScoreFunction<IRankFeatureLinearFunction>(ref writer, "linear", scoreFunction4, formatterResolver);
              break;
          }
        }
        writer.WriteEndObject();
      }
    }

    private static void SerializeScoreFunction<TScoreFunction>(
      ref JsonWriter writer,
      string name,
      TScoreFunction scoreFunction,
      IJsonFormatterResolver formatterResolver)
      where TScoreFunction : IRankFeatureFunction
    {
      writer.WritePropertyName(name);
      formatterResolver.GetFormatter<TScoreFunction>().Serialize(ref writer, scoreFunction, formatterResolver);
    }

    private static IRankFeatureFunction DeserializeScoreFunction<TScoreFunction>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TScoreFunction : IRankFeatureFunction
    {
      return (IRankFeatureFunction) formatterResolver.GetFormatter<TScoreFunction>().Deserialize(ref reader, formatterResolver);
    }

    public IRankFeatureQuery Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return (IRankFeatureQuery) null;
      RankFeatureQuery rankFeatureQuery = new RankFeatureQuery();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        int num;
        if (RankFeatureQueryFormatter.Fields.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
        {
          switch (num)
          {
            case 0:
              rankFeatureQuery.Name = reader.ReadString();
              continue;
            case 1:
              rankFeatureQuery.Boost = new double?(reader.ReadDouble());
              continue;
            case 2:
              rankFeatureQuery.Field = formatterResolver.GetFormatter<Field>().Deserialize(ref reader, formatterResolver);
              continue;
            case 3:
              rankFeatureQuery.Function = RankFeatureQueryFormatter.DeserializeScoreFunction<RankFeatureSaturationFunction>(ref reader, formatterResolver);
              continue;
            case 4:
              rankFeatureQuery.Function = RankFeatureQueryFormatter.DeserializeScoreFunction<RankFeatureLogarithmFunction>(ref reader, formatterResolver);
              continue;
            case 5:
              rankFeatureQuery.Function = RankFeatureQueryFormatter.DeserializeScoreFunction<RankFeatureSigmoidFunction>(ref reader, formatterResolver);
              continue;
            case 6:
              rankFeatureQuery.Function = RankFeatureQueryFormatter.DeserializeScoreFunction<RankFeatureLinearFunction>(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      return (IRankFeatureQuery) rankFeatureQuery;
    }
  }
}
