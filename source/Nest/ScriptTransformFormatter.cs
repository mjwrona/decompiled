// Decompiled with JetBrains decompiler
// Type: Nest.ScriptTransformFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System.Collections.Generic;

namespace Nest
{
  internal class ScriptTransformFormatter : IJsonFormatter<IScriptTransform>, IJsonFormatter
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

    public IScriptTransform Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        return (IScriptTransform) null;
      int count = 0;
      IScriptTransform scriptTransform = (IScriptTransform) null;
      string str = (string) null;
      Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
      while (reader.ReadIsInObject(ref count))
      {
        int num;
        if (ScriptTransformFormatter.AutomataDictionary.TryGetValue(reader.ReadPropertyNameSegmentRaw(), out num))
        {
          switch (num)
          {
            case 0:
              scriptTransform = (IScriptTransform) new InlineScriptTransform(reader.ReadString());
              continue;
            case 1:
              scriptTransform = (IScriptTransform) new IndexedScriptTransform(reader.ReadString());
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
      if (scriptTransform == null)
        return (IScriptTransform) null;
      scriptTransform.Lang = str;
      scriptTransform.Params = dictionary;
      return scriptTransform;
    }

    public void Serialize(
      ref JsonWriter writer,
      IScriptTransform value,
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
        if (!(value is IIndexedScriptTransform indexedScriptTransform))
        {
          if (value is IInlineScriptTransform inlineScriptTransform)
          {
            writer.WritePropertyName("source");
            writer.WriteString(inlineScriptTransform.Source);
            flag = true;
          }
        }
        else
        {
          writer.WritePropertyName("id");
          writer.WriteString(indexedScriptTransform.Id);
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
          formatterResolver.GetFormatter<Dictionary<string, object>>().Serialize(ref writer, value.Params, formatterResolver);
        }
        writer.WriteEndObject();
      }
    }
  }
}
