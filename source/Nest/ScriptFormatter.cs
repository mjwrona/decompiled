// Decompiled with JetBrains decompiler
// Type: Nest.ScriptFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System.Collections.Generic;

namespace Nest
{
  internal class ScriptFormatter : IJsonFormatter<IScript>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "inline",
        0
      },
      {
        "source",
        1
      },
      {
        "id",
        2
      },
      {
        "lang",
        3
      },
      {
        "params",
        4
      }
    };

    public IScript Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return (IScript) new InlineScript(reader.ReadString());
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IScript) null;
      }
      int count = 0;
      IScript script = (IScript) null;
      string str = (string) null;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
      while (reader.ReadIsInObject(ref count))
      {
        int num;
        if (ScriptFormatter.AutomataDictionary.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
        {
          switch (num)
          {
            case 0:
            case 1:
              script = (IScript) new InlineScript(reader.ReadString());
              continue;
            case 2:
              script = (IScript) new IndexedScript(reader.ReadString());
              continue;
            case 3:
              str = reader.ReadString();
              continue;
            case 4:
              dictionary = formatterResolver.GetFormatter<Dictionary<string, object>>().Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
      }
      if (script == null)
        return (IScript) null;
      script.Lang = str;
      script.Params = dictionary;
      return script;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IScript value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case null:
          writer.WriteNull();
          break;
        case IInlineScript inlineScript:
          formatterResolver.GetFormatter<IInlineScript>().Serialize(ref writer, inlineScript, formatterResolver);
          break;
        case IIndexedScript indexedScript:
          formatterResolver.GetFormatter<IIndexedScript>().Serialize(ref writer, indexedScript, formatterResolver);
          break;
        default:
          DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IScript>().Serialize(ref writer, value, formatterResolver);
          break;
      }
    }
  }
}
