// Decompiled with JetBrains decompiler
// Type: Nest.SimilarityFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class SimilarityFormatter : IJsonFormatter<ISimilarity>, IJsonFormatter
  {
    private static readonly AutomataDictionary Similarities = new AutomataDictionary()
    {
      {
        "BM25",
        0
      },
      {
        "LMDirichlet",
        1
      },
      {
        "DFR",
        2
      },
      {
        "DFI",
        3
      },
      {
        "IB",
        4
      },
      {
        "LMJelinekMercer",
        5
      },
      {
        "scripted",
        6
      }
    };
    private static readonly byte[] Type = JsonWriter.GetEncodedPropertyNameWithoutQuotation("type");

    public ISimilarity Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment1 = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int count = 0;
      ArraySegment<byte> bytes = new ArraySegment<byte>();
      while (reader1.ReadIsInObject(ref count))
      {
        ArraySegment<byte> arraySegment2 = reader1.ReadPropertyNameSegmentRaw();
        if (arraySegment2.EqualsBytes(SimilarityFormatter.Type))
        {
          bytes = reader1.ReadStringSegmentUnsafe();
          break;
        }
        reader1.ReadNextBlock();
      }
      reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      int num;
      if (SimilarityFormatter.Similarities.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            return (ISimilarity) SimilarityFormatter.Deserialize<BM25Similarity>(ref reader1, formatterResolver);
          case 1:
            return (ISimilarity) SimilarityFormatter.Deserialize<LMDirichletSimilarity>(ref reader1, formatterResolver);
          case 2:
            return (ISimilarity) SimilarityFormatter.Deserialize<DFRSimilarity>(ref reader1, formatterResolver);
          case 3:
            return (ISimilarity) SimilarityFormatter.Deserialize<DFISimilarity>(ref reader1, formatterResolver);
          case 4:
            return (ISimilarity) SimilarityFormatter.Deserialize<IBSimilarity>(ref reader1, formatterResolver);
          case 5:
            return (ISimilarity) SimilarityFormatter.Deserialize<LMJelinekMercerSimilarity>(ref reader1, formatterResolver);
          case 6:
            return (ISimilarity) SimilarityFormatter.Deserialize<ScriptedSimilarity>(ref reader1, formatterResolver);
        }
      }
      return (ISimilarity) new CustomSimilarity(formatterResolver.GetFormatter<Dictionary<string, object>>().Deserialize(ref reader1, formatterResolver));
    }

    public void Serialize(
      ref JsonWriter writer,
      ISimilarity value,
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
          case "BM25":
            SimilarityFormatter.Serialize<IBM25Similarity>(ref writer, value, formatterResolver);
            break;
          case "DFI":
            SimilarityFormatter.Serialize<IDFISimilarity>(ref writer, value, formatterResolver);
            break;
          case "DFR":
            SimilarityFormatter.Serialize<IDFRSimilarity>(ref writer, value, formatterResolver);
            break;
          case "IB":
            SimilarityFormatter.Serialize<IIBSimilarity>(ref writer, value, formatterResolver);
            break;
          case "LMDirichlet":
            SimilarityFormatter.Serialize<ILMDirichletSimilarity>(ref writer, value, formatterResolver);
            break;
          case "LMJelinekMercer":
            SimilarityFormatter.Serialize<ILMJelinekMercerSimilarity>(ref writer, value, formatterResolver);
            break;
          case "scripted":
            SimilarityFormatter.Serialize<IScriptedSimilarity>(ref writer, value, formatterResolver);
            break;
          default:
            SimilarityFormatter.Serialize<ICustomSimilarity>(ref writer, value, formatterResolver);
            break;
        }
      }
    }

    private static void Serialize<TSimilarity>(
      ref JsonWriter writer,
      ISimilarity value,
      IJsonFormatterResolver formatterResolver)
      where TSimilarity : class, ISimilarity
    {
      formatterResolver.GetFormatter<TSimilarity>().Serialize(ref writer, value as TSimilarity, formatterResolver);
    }

    private static TSimilarity Deserialize<TSimilarity>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TSimilarity : ISimilarity
    {
      return formatterResolver.GetFormatter<TSimilarity>().Deserialize(ref reader, formatterResolver);
    }
  }
}
