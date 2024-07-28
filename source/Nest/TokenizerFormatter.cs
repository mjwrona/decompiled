// Decompiled with JetBrains decompiler
// Type: Nest.TokenizerFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class TokenizerFormatter : IJsonFormatter<ITokenizer>, IJsonFormatter
  {
    private static byte[] TypeField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("type");
    private static readonly AutomataDictionary TokenizerTypes = new AutomataDictionary()
    {
      {
        "char_group",
        0
      },
      {
        "edgengram",
        1
      },
      {
        "edge_ngram",
        1
      },
      {
        "ngram",
        2
      },
      {
        "path_hierarchy",
        3
      },
      {
        "pattern",
        4
      },
      {
        "standard",
        5
      },
      {
        "uax_url_email",
        6
      },
      {
        "whitespace",
        7
      },
      {
        "kuromoji_tokenizer",
        8
      },
      {
        "icu_tokenizer",
        9
      },
      {
        "nori_tokenizer",
        10
      }
    };

    public ITokenizer Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment1 = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int count = 0;
      ArraySegment<byte> bytes = new ArraySegment<byte>();
      while (reader1.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment2 = reader1.ReadPropertyNameSegmentRaw();
        if (arraySegment2.EqualsBytes(TokenizerFormatter.TypeField))
        {
          bytes = reader1.ReadStringSegmentUnsafe();
          break;
        }
        reader1.ReadNextBlock();
      }
      if (bytes == new ArraySegment<byte>())
        return (ITokenizer) null;
      reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int num;
      if (!TokenizerFormatter.TokenizerTypes.TryGetValue(bytes, out num))
        return (ITokenizer) null;
      switch (num)
      {
        case 0:
          return (ITokenizer) TokenizerFormatter.Deserialize<CharGroupTokenizer>(ref reader1, formatterResolver);
        case 1:
          return (ITokenizer) TokenizerFormatter.Deserialize<EdgeNGramTokenizer>(ref reader1, formatterResolver);
        case 2:
          return (ITokenizer) TokenizerFormatter.Deserialize<NGramTokenizer>(ref reader1, formatterResolver);
        case 3:
          return (ITokenizer) TokenizerFormatter.Deserialize<PathHierarchyTokenizer>(ref reader1, formatterResolver);
        case 4:
          return (ITokenizer) TokenizerFormatter.Deserialize<PatternTokenizer>(ref reader1, formatterResolver);
        case 5:
          return (ITokenizer) TokenizerFormatter.Deserialize<StandardTokenizer>(ref reader1, formatterResolver);
        case 6:
          return (ITokenizer) TokenizerFormatter.Deserialize<UaxEmailUrlTokenizer>(ref reader1, formatterResolver);
        case 7:
          return (ITokenizer) TokenizerFormatter.Deserialize<WhitespaceTokenizer>(ref reader1, formatterResolver);
        case 8:
          return (ITokenizer) TokenizerFormatter.Deserialize<KuromojiTokenizer>(ref reader1, formatterResolver);
        case 9:
          return (ITokenizer) TokenizerFormatter.Deserialize<IcuTokenizer>(ref reader1, formatterResolver);
        case 10:
          return (ITokenizer) TokenizerFormatter.Deserialize<NoriTokenizer>(ref reader1, formatterResolver);
        default:
          return (ITokenizer) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      ITokenizer value,
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
          case "char_group":
            TokenizerFormatter.Serialize<ICharGroupTokenizer>(ref writer, value, formatterResolver);
            break;
          case "edge_ngram":
            TokenizerFormatter.Serialize<IEdgeNGramTokenizer>(ref writer, value, formatterResolver);
            break;
          case "icu_tokenizer":
            TokenizerFormatter.Serialize<IIcuTokenizer>(ref writer, value, formatterResolver);
            break;
          case "kuromoji_tokenizer":
            TokenizerFormatter.Serialize<IKuromojiTokenizer>(ref writer, value, formatterResolver);
            break;
          case "ngram":
            TokenizerFormatter.Serialize<INGramTokenizer>(ref writer, value, formatterResolver);
            break;
          case "nori_tokenizer":
            TokenizerFormatter.Serialize<INoriTokenizer>(ref writer, value, formatterResolver);
            break;
          case "path_hierarchy":
            TokenizerFormatter.Serialize<IPathHierarchyTokenizer>(ref writer, value, formatterResolver);
            break;
          case "pattern":
            TokenizerFormatter.Serialize<IPatternTokenizer>(ref writer, value, formatterResolver);
            break;
          case "standard":
            TokenizerFormatter.Serialize<IStandardTokenizer>(ref writer, value, formatterResolver);
            break;
          case "uax_url_email":
            TokenizerFormatter.Serialize<IUaxEmailUrlTokenizer>(ref writer, value, formatterResolver);
            break;
          case "whitespace":
            TokenizerFormatter.Serialize<IWhitespaceTokenizer>(ref writer, value, formatterResolver);
            break;
          default:
            formatterResolver.GetFormatter<object>().Serialize(ref writer, (object) value, formatterResolver);
            break;
        }
      }
    }

    private static TTokenizer Deserialize<TTokenizer>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TTokenizer : ITokenizer
    {
      return formatterResolver.GetFormatter<TTokenizer>().Deserialize(ref reader, formatterResolver);
    }

    private static void Serialize<TTokenizer>(
      ref JsonWriter writer,
      ITokenizer value,
      IJsonFormatterResolver formatterResolver)
      where TTokenizer : class, ITokenizer
    {
      formatterResolver.GetFormatter<TTokenizer>().Serialize(ref writer, value as TTokenizer, formatterResolver);
    }
  }
}
