// Decompiled with JetBrains decompiler
// Type: Nest.ScriptConditionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System.Collections.Generic;

namespace Nest
{
  internal class ScriptConditionFormatter : IJsonFormatter<IScriptCondition>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "source",
        0
      },
      {
        "id",
        1
      },
      {
        "lang",
        2
      },
      {
        "params",
        3
      }
    };

    public IScriptCondition Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IScriptCondition) null;
      int count = 0;
      IScriptCondition scriptCondition = (IScriptCondition) null;
      string str = (string) null;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
      while (reader.ReadIsInObject(ref count))
      {
        int num;
        if (ScriptConditionFormatter.AutomataDictionary.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
        {
          switch (num)
          {
            case 0:
              scriptCondition = (IScriptCondition) new InlineScriptCondition(reader.ReadString());
              continue;
            case 1:
              scriptCondition = (IScriptCondition) new IndexedScriptCondition(reader.ReadString());
              continue;
            case 2:
              str = reader.ReadString();
              continue;
            case 3:
              dictionary = formatterResolver.GetFormatter<Dictionary<string, object>>().Deserialize(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
      }
      if (scriptCondition == null)
        return (IScriptCondition) null;
      scriptCondition.Lang = str;
      scriptCondition.Params = (IDictionary<string, object>) dictionary;
      return scriptCondition;
    }

    public void Serialize(
      ref JsonWriter writer,
      IScriptCondition value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        bool flag = false;
        if (!(value is IIndexedScriptCondition indexedScriptCondition))
        {
          if (value is IInlineScriptCondition inlineScriptCondition)
          {
            writer.WritePropertyName("source");
            writer.WriteString(inlineScriptCondition.Source);
            flag = true;
          }
        }
        else
        {
          writer.WritePropertyName("id");
          writer.WriteString(indexedScriptCondition.Id);
          flag = true;
        }
        if (value.Lang != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("lang");
          writer.WriteString(value.Lang);
          flag = true;
        }
        if (value.Params != null)
        {
          if (flag)
            writer.WriteValueSeparator();
          writer.WritePropertyName("params");
          formatterResolver.GetFormatter<IDictionary<string, object>>().Serialize(ref writer, value.Params, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }
  }
}
