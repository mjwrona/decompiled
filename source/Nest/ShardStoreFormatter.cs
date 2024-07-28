// Decompiled with JetBrains decompiler
// Type: Nest.ShardStoreFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class ShardStoreFormatter : IJsonFormatter<ShardStore>, IJsonFormatter
  {
    private AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "allocation",
        0
      },
      {
        "allocation_id",
        1
      }
    };

    public ShardStore Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ShardStore shardStore = (ShardStore) null;
      int count = 0;
      ShardStoreAllocation shardStoreAllocation = ShardStoreAllocation.Primary;
      string str = (string) null;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (this.Fields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              shardStoreAllocation = formatterResolver.GetFormatter<ShardStoreAllocation>().Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              str = reader.ReadString();
              continue;
            default:
              continue;
          }
        }
        else
        {
          shardStore = DynamicObjectResolver.AllowPrivateExcludeNullCamelCase.GetFormatter<ShardStore>().Deserialize(ref reader, formatterResolver);
          shardStore.Id = segment.Utf8String();
        }
      }
      if (shardStore != null)
      {
        shardStore.Allocation = shardStoreAllocation;
        shardStore.AllocationId = str;
      }
      return shardStore;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ShardStore value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }
  }
}
