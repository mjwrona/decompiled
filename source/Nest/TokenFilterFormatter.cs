// Decompiled with JetBrains decompiler
// Type: Nest.TokenFilterFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class TokenFilterFormatter : IJsonFormatter<ITokenFilter>, IJsonFormatter
  {
    private static readonly byte[] TypeField = JsonWriter.GetEncodedPropertyNameWithoutQuotation("type");
    private static readonly AutomataDictionary TokenFilterTypes = new AutomataDictionary()
    {
      {
        "asciifolding",
        0
      },
      {
        "common_grams",
        1
      },
      {
        "delimited_payload",
        2
      },
      {
        "delimited_payload_filter",
        2
      },
      {
        "dictionary_decompounder",
        3
      },
      {
        "edge_ngram",
        4
      },
      {
        "elision",
        5
      },
      {
        "hunspell",
        6
      },
      {
        "hyphenation_decompounder",
        7
      },
      {
        "keep_types",
        8
      },
      {
        "keep",
        9
      },
      {
        "keyword_marker",
        10
      },
      {
        "kstem",
        11
      },
      {
        "length",
        12
      },
      {
        "limit",
        13
      },
      {
        "lowercase",
        14
      },
      {
        "ngram",
        15
      },
      {
        "pattern_capture",
        16
      },
      {
        "pattern_replace",
        17
      },
      {
        "porter_stem",
        18
      },
      {
        "phonetic",
        19
      },
      {
        "reverse",
        20
      },
      {
        "shingle",
        21
      },
      {
        "snowball",
        22
      },
      {
        "stemmer",
        23
      },
      {
        "stemmer_override",
        24
      },
      {
        "stop",
        25
      },
      {
        "synonym",
        26
      },
      {
        "synonym_graph",
        27
      },
      {
        "trim",
        28
      },
      {
        "truncate",
        29
      },
      {
        "unique",
        30
      },
      {
        "uppercase",
        31
      },
      {
        "word_delimiter",
        32
      },
      {
        "word_delimiter_graph",
        33
      },
      {
        "fingerprint",
        34
      },
      {
        "nori_part_of_speech",
        35
      },
      {
        "kuromoji_readingform",
        36
      },
      {
        "kuromoji_part_of_speech",
        37
      },
      {
        "kuromoji_stemmer",
        38
      },
      {
        "icu_collation",
        39
      },
      {
        "icu_folding",
        40
      },
      {
        "icu_normalizer",
        41
      },
      {
        "icu_transform",
        42
      },
      {
        "condition",
        43
      },
      {
        "multiplexer",
        44
      },
      {
        "predicate_token_filter",
        45
      }
    };

    public ITokenFilter Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment1 = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int count = 0;
      ArraySegment<byte> bytes = new ArraySegment<byte>();
      while (reader1.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment2 = reader1.ReadPropertyNameSegmentRaw();
        if (arraySegment2.EqualsBytes(TokenFilterFormatter.TypeField))
        {
          bytes = reader1.ReadStringSegmentUnsafe();
          break;
        }
        reader1.ReadNextBlock();
      }
      if (bytes == new ArraySegment<byte>())
        return (ITokenFilter) null;
      reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int num;
      if (!TokenFilterFormatter.TokenFilterTypes.TryGetValue(bytes, out num))
        return (ITokenFilter) null;
      switch (num)
      {
        case 0:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<AsciiFoldingTokenFilter>(ref reader1, formatterResolver);
        case 1:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<CommonGramsTokenFilter>(ref reader1, formatterResolver);
        case 2:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<DelimitedPayloadTokenFilter>(ref reader1, formatterResolver);
        case 3:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<DictionaryDecompounderTokenFilter>(ref reader1, formatterResolver);
        case 4:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<EdgeNGramTokenFilter>(ref reader1, formatterResolver);
        case 5:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<ElisionTokenFilter>(ref reader1, formatterResolver);
        case 6:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<HunspellTokenFilter>(ref reader1, formatterResolver);
        case 7:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<HyphenationDecompounderTokenFilter>(ref reader1, formatterResolver);
        case 8:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KeepTypesTokenFilter>(ref reader1, formatterResolver);
        case 9:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KeepWordsTokenFilter>(ref reader1, formatterResolver);
        case 10:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KeywordMarkerTokenFilter>(ref reader1, formatterResolver);
        case 11:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KStemTokenFilter>(ref reader1, formatterResolver);
        case 12:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<LengthTokenFilter>(ref reader1, formatterResolver);
        case 13:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<LimitTokenCountTokenFilter>(ref reader1, formatterResolver);
        case 14:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<LowercaseTokenFilter>(ref reader1, formatterResolver);
        case 15:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<NGramTokenFilter>(ref reader1, formatterResolver);
        case 16:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<PatternCaptureTokenFilter>(ref reader1, formatterResolver);
        case 17:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<PatternReplaceTokenFilter>(ref reader1, formatterResolver);
        case 18:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<PorterStemTokenFilter>(ref reader1, formatterResolver);
        case 19:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<PhoneticTokenFilter>(ref reader1, formatterResolver);
        case 20:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<ReverseTokenFilter>(ref reader1, formatterResolver);
        case 21:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<ShingleTokenFilter>(ref reader1, formatterResolver);
        case 22:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<SnowballTokenFilter>(ref reader1, formatterResolver);
        case 23:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<StemmerTokenFilter>(ref reader1, formatterResolver);
        case 24:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<StemmerOverrideTokenFilter>(ref reader1, formatterResolver);
        case 25:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<StopTokenFilter>(ref reader1, formatterResolver);
        case 26:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<SynonymTokenFilter>(ref reader1, formatterResolver);
        case 27:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<SynonymGraphTokenFilter>(ref reader1, formatterResolver);
        case 28:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<TrimTokenFilter>(ref reader1, formatterResolver);
        case 29:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<TruncateTokenFilter>(ref reader1, formatterResolver);
        case 30:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<UniqueTokenFilter>(ref reader1, formatterResolver);
        case 31:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<UppercaseTokenFilter>(ref reader1, formatterResolver);
        case 32:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<WordDelimiterTokenFilter>(ref reader1, formatterResolver);
        case 33:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<WordDelimiterGraphTokenFilter>(ref reader1, formatterResolver);
        case 34:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<FingerprintTokenFilter>(ref reader1, formatterResolver);
        case 35:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<NoriPartOfSpeechTokenFilter>(ref reader1, formatterResolver);
        case 36:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KuromojiReadingFormTokenFilter>(ref reader1, formatterResolver);
        case 37:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KuromojiPartOfSpeechTokenFilter>(ref reader1, formatterResolver);
        case 38:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<KuromojiStemmerTokenFilter>(ref reader1, formatterResolver);
        case 39:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<IcuCollationTokenFilter>(ref reader1, formatterResolver);
        case 40:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<IcuFoldingTokenFilter>(ref reader1, formatterResolver);
        case 41:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<IcuNormalizationTokenFilter>(ref reader1, formatterResolver);
        case 42:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<IcuTransformTokenFilter>(ref reader1, formatterResolver);
        case 43:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<ConditionTokenFilter>(ref reader1, formatterResolver);
        case 44:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<MultiplexerTokenFilter>(ref reader1, formatterResolver);
        case 45:
          return (ITokenFilter) TokenFilterFormatter.Deserialize<PredicateTokenFilter>(ref reader1, formatterResolver);
        default:
          return (ITokenFilter) null;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      ITokenFilter value,
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
          case "asciifolding":
            TokenFilterFormatter.Serialize<IAsciiFoldingTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "common_grams":
            TokenFilterFormatter.Serialize<ICommonGramsTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "condition":
            TokenFilterFormatter.Serialize<IConditionTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "delimited_payload":
            TokenFilterFormatter.Serialize<IDelimitedPayloadTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "dictionary_decompounder":
            TokenFilterFormatter.Serialize<IDictionaryDecompounderTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "edge_ngram":
            TokenFilterFormatter.Serialize<IEdgeNGramTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "elision":
            TokenFilterFormatter.Serialize<IElisionTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "fingerprint":
            TokenFilterFormatter.Serialize<IFingerprintTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "hunspell":
            TokenFilterFormatter.Serialize<IHunspellTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "hyphenation_decompounder":
            TokenFilterFormatter.Serialize<IHyphenationDecompounderTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "icu_collation":
            TokenFilterFormatter.Serialize<IIcuCollationTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "icu_folding":
            TokenFilterFormatter.Serialize<IIcuFoldingTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "icu_normalizer":
            TokenFilterFormatter.Serialize<IIcuNormalizationTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "icu_transform":
            TokenFilterFormatter.Serialize<IIcuTransformTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "keep":
            TokenFilterFormatter.Serialize<IKeepWordsTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "keep_types":
            TokenFilterFormatter.Serialize<IKeepTypesTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "keyword_marker":
            TokenFilterFormatter.Serialize<IKeywordMarkerTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "kstem":
            TokenFilterFormatter.Serialize<IKStemTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "kuromoji_part_of_speech":
            TokenFilterFormatter.Serialize<IKuromojiPartOfSpeechTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "kuromoji_readingform":
            TokenFilterFormatter.Serialize<IKuromojiReadingFormTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "kuromoji_stemmer":
            TokenFilterFormatter.Serialize<IKuromojiStemmerTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "length":
            TokenFilterFormatter.Serialize<ILengthTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "limit":
            TokenFilterFormatter.Serialize<ILimitTokenCountTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "lowercase":
            TokenFilterFormatter.Serialize<ILowercaseTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "multiplexer":
            TokenFilterFormatter.Serialize<IMultiplexerTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "ngram":
            TokenFilterFormatter.Serialize<INGramTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "nori_part_of_speech":
            TokenFilterFormatter.Serialize<INoriPartOfSpeechTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "pattern_capture":
            TokenFilterFormatter.Serialize<IPatternCaptureTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "pattern_replace":
            TokenFilterFormatter.Serialize<IPatternReplaceTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "phonetic":
            TokenFilterFormatter.Serialize<IPhoneticTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "porter_stem":
            TokenFilterFormatter.Serialize<IPorterStemTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "predicate_token_filter":
            TokenFilterFormatter.Serialize<IPredicateTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "reverse":
            TokenFilterFormatter.Serialize<IReverseTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "shingle":
            TokenFilterFormatter.Serialize<IShingleTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "snowball":
            TokenFilterFormatter.Serialize<ISnowballTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "stemmer":
            TokenFilterFormatter.Serialize<IStemmerTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "stemmer_override":
            TokenFilterFormatter.Serialize<IStemmerOverrideTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "stop":
            TokenFilterFormatter.Serialize<IStopTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "synonym":
            TokenFilterFormatter.Serialize<ISynonymTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "synonym_graph":
            TokenFilterFormatter.Serialize<ISynonymGraphTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "trim":
            TokenFilterFormatter.Serialize<ITrimTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "truncate":
            TokenFilterFormatter.Serialize<ITruncateTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "unique":
            TokenFilterFormatter.Serialize<IUniqueTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "uppercase":
            TokenFilterFormatter.Serialize<IUppercaseTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "word_delimiter":
            TokenFilterFormatter.Serialize<IWordDelimiterTokenFilter>(ref writer, value, formatterResolver);
            break;
          case "word_delimiter_graph":
            TokenFilterFormatter.Serialize<IWordDelimiterGraphTokenFilter>(ref writer, value, formatterResolver);
            break;
          default:
            formatterResolver.GetFormatter<object>().Serialize(ref writer, (object) value, formatterResolver);
            break;
        }
      }
    }

    private static void Serialize<TTokenFilter>(
      ref JsonWriter writer,
      ITokenFilter value,
      IJsonFormatterResolver formatterResolver)
      where TTokenFilter : class, ITokenFilter
    {
      formatterResolver.GetFormatter<TTokenFilter>().Serialize(ref writer, value as TTokenFilter, formatterResolver);
    }

    private static TTokenFilter Deserialize<TTokenFilter>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TTokenFilter : ITokenFilter
    {
      return formatterResolver.GetFormatter<TTokenFilter>().Deserialize(ref reader, formatterResolver);
    }
  }
}
