// Decompiled with JetBrains decompiler
// Type: Nest.CatFielddataRecordFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class CatFielddataRecordFormatter : IJsonFormatter<CatFielddataRecord>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "id",
        0
      },
      {
        "node",
        1
      },
      {
        "n",
        1
      },
      {
        "host",
        2
      },
      {
        "ip",
        3
      },
      {
        "field",
        4
      },
      {
        "size",
        5
      }
    };

    public CatFielddataRecord Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      CatFielddataRecord catFielddataRecord = new CatFielddataRecord();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (CatFielddataRecordFormatter.AutomataDictionary.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              catFielddataRecord.Id = reader.ReadString();
              continue;
            case 1:
              catFielddataRecord.Node = reader.ReadString();
              continue;
            case 2:
              catFielddataRecord.Host = reader.ReadString();
              continue;
            case 3:
              catFielddataRecord.Ip = reader.ReadString();
              continue;
            case 4:
              catFielddataRecord.Field = reader.ReadString();
              continue;
            case 5:
              catFielddataRecord.Size = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
      }
      return catFielddataRecord;
    }

    public void Serialize(
      ref JsonWriter writer,
      CatFielddataRecord value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }
  }
}
