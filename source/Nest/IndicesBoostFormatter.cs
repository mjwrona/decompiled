// Decompiled with JetBrains decompiler
// Type: Nest.IndicesBoostFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class IndicesBoostFormatter : 
    IJsonFormatter<IDictionary<IndexName, double>>,
    IJsonFormatter
  {
    public void Serialize(
      ref JsonWriter writer,
      IDictionary<IndexName, double> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        writer.WriteBeginArray();
        int num = 0;
        foreach (KeyValuePair<IndexName, double> keyValuePair in (IEnumerable<KeyValuePair<IndexName, double>>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WriteBeginObject();
          string propertyName = connectionSettings.Inferrer.IndexName(keyValuePair.Key);
          writer.WritePropertyName(propertyName);
          writer.WriteDouble(keyValuePair.Value);
          writer.WriteEndObject();
          ++num;
        }
        writer.WriteEndArray();
      }
    }

    public IDictionary<IndexName, double> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          return (IDictionary<IndexName, double>) formatterResolver.GetFormatter<Dictionary<IndexName, double>>().Deserialize(ref reader, formatterResolver);
        case JsonToken.BeginArray:
          Dictionary<IndexName, double> dictionary = new Dictionary<IndexName, double>();
          int count = 0;
          IJsonFormatter<IndexName> formatter = formatterResolver.GetFormatter<IndexName>();
          while (reader.ReadIsInArray(ref count))
          {
            reader.ReadIsBeginObjectWithVerify();
            IndexName key = formatter.Deserialize(ref reader, formatterResolver);
            reader.ReadIsNameSeparatorWithVerify();
            dictionary.Add(key, reader.ReadDouble());
            reader.ReadIsEndObjectWithVerify();
          }
          return (IDictionary<IndexName, double>) dictionary;
        default:
          reader.ReadNextBlock();
          return (IDictionary<IndexName, double>) null;
      }
    }
  }
}
