// Decompiled with JetBrains decompiler
// Type: Nest.SqlValueFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class SqlValueFormatter : IJsonFormatter<SqlValue>, IJsonFormatter
  {
    public SqlValue Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.Null)
      {
        reader.ReadNext();
        return (SqlValue) null;
      }
      ArraySegment<byte> src = reader.ReadNextBlockSegment();
      return new SqlValue(BinaryUtil.ToArray(ref src), formatterResolver);
    }

    public void Serialize(
      ref JsonWriter writer,
      SqlValue value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        for (int index = 0; index < value.Bytes.Length; ++index)
          writer.WriteByte(value.Bytes[index]);
      }
    }
  }
}
