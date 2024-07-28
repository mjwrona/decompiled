// Decompiled with JetBrains decompiler
// Type: Nest.CharFilterFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class CharFilterFormatter : IJsonFormatter<ICharFilter>, IJsonFormatter
  {
    public ICharFilter Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int count = 0;
      string str = (string) null;
      while (reader1.ReadIsInObject(ref count))
      {
        if (reader1.ReadPropertyName() == "type")
        {
          str = reader1.ReadString();
          break;
        }
        reader1.ReadNextBlock();
      }
      if (str == null)
        return (ICharFilter) null;
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      if (str == "html_strip")
        return (ICharFilter) CharFilterFormatter.Deserialize<HtmlStripCharFilter>(ref reader1, formatterResolver);
      if (str == "mapping")
        return (ICharFilter) CharFilterFormatter.Deserialize<MappingCharFilter>(ref reader1, formatterResolver);
      if (str == "pattern_replace")
        return (ICharFilter) CharFilterFormatter.Deserialize<PatternReplaceCharFilter>(ref reader1, formatterResolver);
      if (str == "kuromoji_iteration_mark")
        return (ICharFilter) CharFilterFormatter.Deserialize<KuromojiIterationMarkCharFilter>(ref reader1, formatterResolver);
      return str == "icu_normalizer" ? (ICharFilter) CharFilterFormatter.Deserialize<IcuNormalizationCharFilter>(ref reader1, formatterResolver) : (ICharFilter) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      ICharFilter value,
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
          case "html_strip":
            CharFilterFormatter.Serialize<IHtmlStripCharFilter>(ref writer, value, formatterResolver);
            break;
          case "mapping":
            CharFilterFormatter.Serialize<IMappingCharFilter>(ref writer, value, formatterResolver);
            break;
          case "pattern_replace":
            CharFilterFormatter.Serialize<IPatternReplaceCharFilter>(ref writer, value, formatterResolver);
            break;
          case "kuromoji_iteration_mark":
            CharFilterFormatter.Serialize<IKuromojiIterationMarkCharFilter>(ref writer, value, formatterResolver);
            break;
          case "icu_normalizer":
            CharFilterFormatter.Serialize<IIcuNormalizationCharFilter>(ref writer, value, formatterResolver);
            break;
          default:
            formatterResolver.GetFormatter<object>().Serialize(ref writer, (object) value, formatterResolver);
            break;
        }
      }
    }

    private static void Serialize<TCharFilter>(
      ref JsonWriter writer,
      ICharFilter value,
      IJsonFormatterResolver formatterResolver)
      where TCharFilter : class, ICharFilter
    {
      formatterResolver.GetFormatter<TCharFilter>().Serialize(ref writer, value as TCharFilter, formatterResolver);
    }

    private static TCharFilter Deserialize<TCharFilter>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TCharFilter : ICharFilter
    {
      return formatterResolver.GetFormatter<TCharFilter>().Deserialize(ref reader, formatterResolver);
    }
  }
}
