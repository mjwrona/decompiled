// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzerFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class AnalyzerFormatter : IJsonFormatter<IAnalyzer>, IJsonFormatter
  {
    public IAnalyzer Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int count = 0;
      string str = (string) null;
      bool flag = false;
      while (reader1.ReadIsInObject(ref count))
      {
        switch (reader1.ReadPropertyName())
        {
          case "type":
            str = reader1.ReadString();
            continue;
          case "tokenizer":
            reader1.ReadNext();
            flag = true;
            continue;
          default:
            reader1.ReadNextBlock();
            continue;
        }
      }
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      switch (str)
      {
        case "fingerprint":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<FingerprintAnalyzer>(ref reader1, formatterResolver);
        case "icu_analyzer":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<IcuAnalyzer>(ref reader1, formatterResolver);
        case "keyword":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<KeywordAnalyzer>(ref reader1, formatterResolver);
        case "kuromoji":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<KuromojiAnalyzer>(ref reader1, formatterResolver);
        case "nori":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<NoriAnalyzer>(ref reader1, formatterResolver);
        case "pattern":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<PatternAnalyzer>(ref reader1, formatterResolver);
        case "simple":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<SimpleAnalyzer>(ref reader1, formatterResolver);
        case "snowball":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<SnowballAnalyzer>(ref reader1, formatterResolver);
        case "standard":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<StandardAnalyzer>(ref reader1, formatterResolver);
        case "stop":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<StopAnalyzer>(ref reader1, formatterResolver);
        case "whitespace":
          return (IAnalyzer) AnalyzerFormatter.Deserialize<WhitespaceAnalyzer>(ref reader1, formatterResolver);
        default:
          return flag ? (IAnalyzer) AnalyzerFormatter.Deserialize<CustomAnalyzer>(ref reader1, formatterResolver) : (IAnalyzer) AnalyzerFormatter.Deserialize<LanguageAnalyzer>(ref reader1, formatterResolver);
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IAnalyzer value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        switch (value.Type)
        {
          case "custom":
            AnalyzerFormatter.Serialize<ICustomAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "fingerprint":
            AnalyzerFormatter.Serialize<IFingerprintAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "icu_analyzer":
            AnalyzerFormatter.Serialize<IIcuAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "keyword":
            AnalyzerFormatter.Serialize<IKeywordAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "kuromoji":
            AnalyzerFormatter.Serialize<IKuromojiAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "nori":
            AnalyzerFormatter.Serialize<INoriAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "pattern":
            AnalyzerFormatter.Serialize<IPatternAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "simple":
            AnalyzerFormatter.Serialize<ISimpleAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "snowball":
            AnalyzerFormatter.Serialize<ISnowballAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "standard":
            AnalyzerFormatter.Serialize<IStandardAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "stop":
            AnalyzerFormatter.Serialize<IStopAnalyzer>(ref writer, value, formatterResolver);
            break;
          case "whitespace":
            AnalyzerFormatter.Serialize<IWhitespaceAnalyzer>(ref writer, value, formatterResolver);
            break;
          default:
            AnalyzerFormatter.Serialize<ILanguageAnalyzer>(ref writer, value, formatterResolver);
            break;
        }
      }
    }

    private static void Serialize<TAnalyzer>(
      ref JsonWriter writer,
      IAnalyzer value,
      IJsonFormatterResolver formatterResolver)
      where TAnalyzer : class, IAnalyzer
    {
      formatterResolver.GetFormatter<TAnalyzer>().Serialize(ref writer, value as TAnalyzer, formatterResolver);
    }

    private static TAnalyzer Deserialize<TAnalyzer>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TAnalyzer : IAnalyzer
    {
      return formatterResolver.GetFormatter<TAnalyzer>().Deserialize(ref reader, formatterResolver);
    }
  }
}
