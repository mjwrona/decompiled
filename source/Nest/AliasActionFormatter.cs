// Decompiled with JetBrains decompiler
// Type: Nest.AliasActionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class AliasActionFormatter : IJsonFormatter<IAliasAction>, IJsonFormatter
  {
    private static readonly AutomataDictionary Actions = new AutomataDictionary()
    {
      {
        "add",
        0
      },
      {
        "remove",
        1
      },
      {
        "remove_index",
        2
      }
    };

    public IAliasAction Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (IAliasAction) null;
      }
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      reader1.ReadIsBeginObjectWithVerify();
      ArraySegment<byte> bytes = reader1.ReadPropertyNameSegmentRaw();
      IAliasAction aliasAction = (IAliasAction) null;
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int num;
      if (AliasActionFormatter.Actions.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            aliasAction = (IAliasAction) AliasActionFormatter.Deserialize<AliasAddAction>(ref reader1, formatterResolver);
            break;
          case 1:
            aliasAction = (IAliasAction) AliasActionFormatter.Deserialize<AliasRemoveAction>(ref reader1, formatterResolver);
            break;
          case 2:
            aliasAction = (IAliasAction) AliasActionFormatter.Deserialize<AliasRemoveIndexAction>(ref reader1, formatterResolver);
            break;
        }
      }
      return aliasAction;
    }

    public void Serialize(
      ref JsonWriter writer,
      IAliasAction value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case null:
          writer.WriteNull();
          break;
        case IAliasAddAction action1:
          AliasActionFormatter.Serialize<IAliasAddAction>(ref writer, action1, formatterResolver);
          break;
        case IAliasRemoveAction action2:
          AliasActionFormatter.Serialize<IAliasRemoveAction>(ref writer, action2, formatterResolver);
          break;
        case IAliasRemoveIndexAction action3:
          AliasActionFormatter.Serialize<IAliasRemoveIndexAction>(ref writer, action3, formatterResolver);
          break;
        default:
          writer.WriteNull();
          break;
      }
    }

    private static void Serialize<TAliasAction>(
      ref JsonWriter writer,
      TAliasAction action,
      IJsonFormatterResolver formatterResolver)
      where TAliasAction : IAliasAction
    {
      formatterResolver.GetFormatter<TAliasAction>().Serialize(ref writer, action, formatterResolver);
    }

    private static TAliasAction Deserialize<TAliasAction>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TAliasAction : IAliasAction
    {
      return formatterResolver.GetFormatter<TAliasAction>().Deserialize(ref reader, formatterResolver);
    }
  }
}
