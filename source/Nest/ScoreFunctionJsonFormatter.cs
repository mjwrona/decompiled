// Decompiled with JetBrains decompiler
// Type: Nest.ScoreFunctionJsonFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class ScoreFunctionJsonFormatter : IJsonFormatter<IScoreFunction>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "filter",
        0
      },
      {
        "weight",
        1
      },
      {
        "exp",
        2
      },
      {
        "gauss",
        2
      },
      {
        "linear",
        2
      },
      {
        "random_score",
        3
      },
      {
        "field_value_factor",
        4
      },
      {
        "script_score",
        5
      }
    };

    public IScoreFunction Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      QueryContainer queryContainer = (QueryContainer) null;
      double? nullable1 = new double?();
      IScoreFunction scoreFunction = (IScoreFunction) null;
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (ScoreFunctionJsonFormatter.AutomataDictionary.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              queryContainer = formatterResolver.GetFormatter<QueryContainer>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              nullable1 = new double?(reader.ReadDouble());
              continue;
            case 2:
              int count2 = 0;
              MultiValueMode? nullable2 = new MultiValueMode?();
              IDecayFunction decayFunction = (IDecayFunction) null;
              while (reader.ReadIsInObject(ref count2))
              {
                string str = reader.ReadPropertyName();
                if (str == "multi_value_mode")
                {
                  nullable2 = new MultiValueMode?(formatterResolver.GetFormatter<MultiValueMode>().Deserialize(ref reader, formatterResolver));
                }
                else
                {
                  string type = segment.Utf8String();
                  decayFunction = ScoreFunctionJsonFormatter.ReadDecayFunction(ref reader, type, formatterResolver);
                  decayFunction.Field = (Field) str;
                }
              }
              if (decayFunction != null)
              {
                decayFunction.MultiValueMode = nullable2;
                scoreFunction = (IScoreFunction) decayFunction;
                continue;
              }
              continue;
            case 3:
              scoreFunction = (IScoreFunction) formatterResolver.GetFormatter<RandomScoreFunction>().Deserialize(ref reader, formatterResolver);
              continue;
            case 4:
              scoreFunction = (IScoreFunction) formatterResolver.GetFormatter<FieldValueFactorFunction>().Deserialize(ref reader, formatterResolver);
              continue;
            case 5:
              scoreFunction = (IScoreFunction) formatterResolver.GetFormatter<ScriptScoreFunction>().Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
      }
      if (scoreFunction == null)
      {
        if (!nullable1.HasValue)
          return (IScoreFunction) null;
        scoreFunction = (IScoreFunction) new WeightFunction();
      }
      scoreFunction.Weight = nullable1;
      scoreFunction.Filter = queryContainer;
      return scoreFunction;
    }

    public void Serialize(
      ref JsonWriter writer,
      IScoreFunction value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        return;
      bool flag = false;
      writer.WriteBeginObject();
      if (value.Filter != null)
      {
        writer.WritePropertyName("filter");
        formatterResolver.GetFormatter<QueryContainer>().Serialize(ref writer, value.Filter, formatterResolver);
        flag = true;
      }
      switch (value)
      {
        case IDecayFunction decay:
          if (flag)
            writer.WriteValueSeparator();
          this.WriteDecay(ref writer, decay, formatterResolver);
          flag = true;
          goto label_18;
        case IFieldValueFactorFunction valueFactorFunction:
          if (flag)
            writer.WriteValueSeparator();
          ScoreFunctionJsonFormatter.WriteFieldValueFactor(ref writer, valueFactorFunction, formatterResolver);
          flag = true;
          goto label_18;
        case IRandomScoreFunction randomScoreFunction:
          if (flag)
            writer.WriteValueSeparator();
          ScoreFunctionJsonFormatter.WriteRandomScore(ref writer, randomScoreFunction, formatterResolver);
          flag = true;
          goto label_18;
        case IScriptScoreFunction scriptScoreFunction:
          if (flag)
            writer.WriteValueSeparator();
          ScoreFunctionJsonFormatter.WriteScriptScore(ref writer, scriptScoreFunction, formatterResolver);
          flag = true;
          goto label_18;
        case IWeightFunction _:
label_18:
          if (value.Weight.HasValue)
          {
            if (flag)
              writer.WriteValueSeparator();
            writer.WritePropertyName("weight");
            writer.WriteDouble(value.Weight.Value);
          }
          writer.WriteEndObject();
          break;
        default:
          throw new Exception("Can not write function score json for " + value.GetType().Name);
      }
    }

    private static void WriteScriptScore(
      ref JsonWriter writer,
      IScriptScoreFunction value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WritePropertyName("script_score");
      writer.WriteBeginObject();
      writer.WritePropertyName("script");
      formatterResolver.GetFormatter<IScript>().Serialize(ref writer, value?.Script, formatterResolver);
      writer.WriteEndObject();
    }

    private static void WriteRandomScore(
      ref JsonWriter writer,
      IRandomScoreFunction value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WritePropertyName("random_score");
      writer.WriteBeginObject();
      if (value.Seed != null)
      {
        writer.WritePropertyName("seed");
        formatterResolver.GetFormatter<Union<long, string>>().Serialize(ref writer, value.Seed, formatterResolver);
      }
      if (value.Field != (Field) null)
      {
        if (value.Seed != null)
          writer.WriteValueSeparator();
        writer.WritePropertyName("field");
        formatterResolver.GetFormatter<Field>().Serialize(ref writer, value.Field, formatterResolver);
      }
      writer.WriteEndObject();
    }

    private static void WriteFieldValueFactor(
      ref JsonWriter writer,
      IFieldValueFactorFunction value,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WritePropertyName("field_value_factor");
      writer.WriteBeginObject();
      writer.WritePropertyName("field");
      writer.WriteString(formatterResolver.GetConnectionSettings().Inferrer.Field(value.Field));
      double? nullable;
      if (value.Factor.HasValue)
      {
        writer.WriteValueSeparator();
        writer.WritePropertyName("factor");
        ref JsonWriter local = ref writer;
        nullable = value.Factor;
        double num = nullable.Value;
        local.WriteDouble(num);
      }
      if (value.Modifier.HasValue)
      {
        writer.WriteValueSeparator();
        writer.WritePropertyName("modifier");
        formatterResolver.GetFormatter<FieldValueFactorModifier>().Serialize(ref writer, value.Modifier.Value, formatterResolver);
      }
      nullable = value.Missing;
      if (nullable.HasValue)
      {
        writer.WriteValueSeparator();
        writer.WritePropertyName("missing");
        ref JsonWriter local = ref writer;
        nullable = value.Missing;
        double num = nullable.Value;
        local.WriteDouble(num);
      }
      writer.WriteEndObject();
    }

    private void WriteDecay(
      ref JsonWriter writer,
      IDecayFunction decay,
      IJsonFormatterResolver formatterResolver)
    {
      writer.WritePropertyName(decay.DecayType);
      writer.WriteBeginObject();
      writer.WritePropertyName(formatterResolver.GetConnectionSettings().Inferrer.Field(decay.Field));
      writer.WriteBeginObject();
      bool written = false;
      switch (decay)
      {
        case IDecayFunction<double?, double?> decayFunction1:
          ScoreFunctionJsonFormatter.WriteNumericDecay(ref writer, ref written, decayFunction1);
          break;
        case IDecayFunction<DateMath, Time> decayFunction2:
          ScoreFunctionJsonFormatter.WriteDateDecay(ref writer, ref written, decayFunction2, formatterResolver);
          break;
        case IDecayFunction<GeoLocation, Distance> decayFunction3:
          ScoreFunctionJsonFormatter.WriteGeoDecay(ref writer, ref written, decayFunction3, formatterResolver);
          break;
        default:
          throw new Exception("Can not write decay function json for " + decay.GetType().Name);
      }
      double? decay1 = decay.Decay;
      if (decay1.HasValue)
      {
        if (written)
          writer.WriteValueSeparator();
        writer.WritePropertyName(nameof (decay));
        ref JsonWriter local = ref writer;
        decay1 = decay.Decay;
        double num = decay1.Value;
        local.WriteDouble(num);
      }
      writer.WriteEndObject();
      MultiValueMode? multiValueMode = decay.MultiValueMode;
      if (multiValueMode.HasValue)
      {
        writer.WriteValueSeparator();
        writer.WritePropertyName("multi_value_mode");
        IJsonFormatter<MultiValueMode> formatter = formatterResolver.GetFormatter<MultiValueMode>();
        ref JsonWriter local = ref writer;
        multiValueMode = decay.MultiValueMode;
        int num = (int) multiValueMode.Value;
        IJsonFormatterResolver formatterResolver1 = formatterResolver;
        formatter.Serialize(ref local, (MultiValueMode) num, formatterResolver1);
      }
      writer.WriteEndObject();
    }

    private static void WriteNumericDecay(
      ref JsonWriter writer,
      ref bool written,
      IDecayFunction<double?, double?> value)
    {
      if (value.Origin.HasValue)
      {
        writer.WritePropertyName("origin");
        writer.WriteDouble(value.Origin.Value);
        written = true;
      }
      if (value.Scale.HasValue)
      {
        if (written)
          writer.WriteValueSeparator();
        writer.WritePropertyName("scale");
        writer.WriteDouble(value.Scale.Value);
        written = true;
      }
      if (!value.Offset.HasValue)
        return;
      if (written)
        writer.WriteValueSeparator();
      writer.WritePropertyName("offset");
      writer.WriteDouble(value.Offset.Value);
    }

    private static void WriteDateDecay(
      ref JsonWriter writer,
      ref bool written,
      IDecayFunction<DateMath, Time> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Field.IsConditionless())
        return;
      if (value.Origin != null)
      {
        writer.WritePropertyName("origin");
        formatterResolver.GetFormatter<DateMath>().Serialize(ref writer, value.Origin, formatterResolver);
        written = true;
      }
      if (written)
        writer.WriteValueSeparator();
      writer.WritePropertyName("scale");
      IJsonFormatter<Time> formatter = formatterResolver.GetFormatter<Time>();
      formatter.Serialize(ref writer, value.Scale, formatterResolver);
      written = true;
      if (!(value.Offset != (Time) null))
        return;
      writer.WriteValueSeparator();
      writer.WritePropertyName("offset");
      formatter.Serialize(ref writer, value.Offset, formatterResolver);
    }

    private static void WriteGeoDecay(
      ref JsonWriter writer,
      ref bool written,
      IDecayFunction<GeoLocation, Distance> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Field.IsConditionless())
        return;
      written = true;
      writer.WritePropertyName("origin");
      formatterResolver.GetFormatter<GeoLocation>().Serialize(ref writer, value.Origin, formatterResolver);
      writer.WriteValueSeparator();
      writer.WritePropertyName("scale");
      IJsonFormatter<Distance> formatter = formatterResolver.GetFormatter<Distance>();
      formatter.Serialize(ref writer, value.Scale, formatterResolver);
      if (value.Offset == null)
        return;
      writer.WriteValueSeparator();
      writer.WritePropertyName("offset");
      formatter.Serialize(ref writer, value.Offset, formatterResolver);
    }

    private static IDecayFunction ReadDecayFunction(
      ref JsonReader reader,
      string type,
      IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      int count = 0;
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      string str = "numeric";
      while (reader1.ReadIsInObject(ref count))
      {
        if (reader1.ReadPropertyName() == "origin")
        {
          switch (reader1.GetCurrentJsonToken())
          {
            case JsonToken.BeginObject:
              str = "geo";
              goto label_6;
            case JsonToken.String:
              str = "date";
              goto label_6;
            default:
              goto label_6;
          }
        }
      }
label_6:
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      switch (type)
      {
        case "exp":
          switch (str)
          {
            case "numeric":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<ExponentialDecayFunction>(ref reader1, formatterResolver);
            case "date":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<ExponentialDateDecayFunction>(ref reader1, formatterResolver);
            case "geo":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<ExponentialGeoDecayFunction>(ref reader1, formatterResolver);
            default:
              return (IDecayFunction) null;
          }
        case "gauss":
          switch (str)
          {
            case "numeric":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<GaussDecayFunction>(ref reader1, formatterResolver);
            case "date":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<GaussDateDecayFunction>(ref reader1, formatterResolver);
            case "geo":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<GaussGeoDecayFunction>(ref reader1, formatterResolver);
            default:
              return (IDecayFunction) null;
          }
        case "linear":
          switch (str)
          {
            case "numeric":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<LinearDecayFunction>(ref reader1, formatterResolver);
            case "date":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<LinearDateDecayFunction>(ref reader1, formatterResolver);
            case "geo":
              return (IDecayFunction) ScoreFunctionJsonFormatter.Deserialize<LinearGeoDecayFunction>(ref reader1, formatterResolver);
            default:
              return (IDecayFunction) null;
          }
        default:
          return (IDecayFunction) null;
      }
    }

    private static TDecayFunction Deserialize<TDecayFunction>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TDecayFunction : IDecayFunction
    {
      return formatterResolver.GetFormatter<TDecayFunction>().Deserialize(ref reader, formatterResolver);
    }
  }
}
