// Decompiled with JetBrains decompiler
// Type: Nest.BulkResponseItemFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class BulkResponseItemFormatter : IJsonFormatter<BulkResponseItemBase>, IJsonFormatter
  {
    private static readonly AutomataDictionary Operations = new AutomataDictionary()
    {
      {
        "delete",
        0
      },
      {
        "update",
        1
      },
      {
        "index",
        2
      },
      {
        "create",
        3
      }
    };

    public BulkResponseItemBase Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      BulkResponseItemBase responseItemBase = (BulkResponseItemBase) null;
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (BulkResponseItemBase) null;
      }
      reader.ReadIsBeginObjectWithVerify();
      ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
      int num;
      if (BulkResponseItemFormatter.Operations.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            responseItemBase = (BulkResponseItemBase) formatterResolver.GetFormatter<BulkDeleteResponseItem>().Deserialize(ref reader, formatterResolver);
            break;
          case 1:
            responseItemBase = (BulkResponseItemBase) formatterResolver.GetFormatter<BulkUpdateResponseItem>().Deserialize(ref reader, formatterResolver);
            break;
          case 2:
            responseItemBase = (BulkResponseItemBase) formatterResolver.GetFormatter<BulkIndexResponseItem>().Deserialize(ref reader, formatterResolver);
            break;
          case 3:
            responseItemBase = (BulkResponseItemBase) formatterResolver.GetFormatter<BulkCreateResponseItem>().Deserialize(ref reader, formatterResolver);
            break;
        }
      }
      else
        reader.ReadNextBlock();
      reader.ReadIsEndObjectWithVerify();
      return responseItemBase;
    }

    public void Serialize(
      ref JsonWriter writer,
      BulkResponseItemBase value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }
  }
}
