// Decompiled with JetBrains decompiler
// Type: Nest.FieldValuesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class FieldValuesFormatter : IJsonFormatter<FieldValues>, IJsonFormatter
  {
    private static readonly LazyDocumentFormatter LazyDocumentFormatter = new LazyDocumentFormatter();

    public FieldValues Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (FieldValues) null;
      }
      int count = 0;
      Dictionary<string, LazyDocument> container = new Dictionary<string, LazyDocument>();
      while (reader.ReadIsInObject(ref count))
      {
        string key = reader.ReadPropertyName();
        LazyDocument lazyDocument = FieldValuesFormatter.LazyDocumentFormatter.Deserialize(ref reader, formatterResolver);
        container[key] = lazyDocument;
      }
      return new FieldValues(formatterResolver.GetConnectionSettings().Inferrer, (IDictionary<string, LazyDocument>) container);
    }

    public void Serialize(
      ref JsonWriter writer,
      FieldValues value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        int num = 0;
        foreach (KeyValuePair<string, LazyDocument> keyValuePair in (IEnumerable<KeyValuePair<string, LazyDocument>>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(keyValuePair.Key);
          writer.WriteRaw(keyValuePair.Value.Bytes);
          ++num;
        }
        writer.WriteEndObject();
      }
    }
  }
}
