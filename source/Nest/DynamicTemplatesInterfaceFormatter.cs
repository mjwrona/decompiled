// Decompiled with JetBrains decompiler
// Type: Nest.DynamicTemplatesInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class DynamicTemplatesInterfaceFormatter : 
    IJsonFormatter<IDynamicTemplateContainer>,
    IJsonFormatter
  {
    public IDynamicTemplateContainer Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IDynamicTemplateContainer) formatterResolver.GetFormatter<DynamicTemplateContainer>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      IDynamicTemplateContainer value,
      IJsonFormatterResolver formatterResolver)
    {
      if (!value.HasAny<KeyValuePair<string, IDynamicTemplate>>())
        return;
      writer.WriteBeginArray();
      IJsonFormatter<IDynamicTemplate> formatter = formatterResolver.GetFormatter<IDynamicTemplate>();
      int num = 0;
      foreach (KeyValuePair<string, IDynamicTemplate> keyValuePair in (IEnumerable<KeyValuePair<string, IDynamicTemplate>>) value)
      {
        if (num > 0)
          writer.WriteValueSeparator();
        writer.WriteBeginObject();
        writer.WritePropertyName(keyValuePair.Key);
        formatter.Serialize(ref writer, keyValuePair.Value, formatterResolver);
        writer.WriteEndObject();
        ++num;
      }
      writer.WriteEndArray();
    }
  }
}
