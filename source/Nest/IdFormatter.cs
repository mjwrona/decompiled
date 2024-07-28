// Decompiled with JetBrains decompiler
// Type: Nest.IdFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class IdFormatter : IJsonFormatter<Id>, IJsonFormatter
  {
    public Id Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => reader.GetCurrentJsonToken() != JsonToken.Number ? new Id(reader.ReadString()) : new Id(reader.ReadInt64());

    public void Serialize(
      ref JsonWriter writer,
      Id value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (Id) null)
        writer.WriteNull();
      else if (value.Document != null)
      {
        string str = formatterResolver.GetConnectionSettings().Inferrer.Id(value.Document.GetType(), value.Document);
        writer.WriteString(str);
      }
      else
      {
        long? longValue = value.LongValue;
        if (longValue.HasValue)
        {
          ref JsonWriter local = ref writer;
          longValue = value.LongValue;
          long num = longValue.Value;
          local.WriteInt64(num);
        }
        else
          writer.WriteString(value.StringValue);
      }
    }
  }
}
