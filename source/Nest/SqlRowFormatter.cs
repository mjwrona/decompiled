// Decompiled with JetBrains decompiler
// Type: Nest.SqlRowFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System.Collections.Generic;

namespace Nest
{
  internal class SqlRowFormatter : IJsonFormatter<SqlRow>, IJsonFormatter
  {
    private static readonly InterfaceListFormatter<SqlValue> SqlValueFormatter = new InterfaceListFormatter<SqlValue>();

    public void Serialize(
      ref JsonWriter writer,
      SqlRow value,
      IJsonFormatterResolver formatterResolver)
    {
      SqlRowFormatter.SqlValueFormatter.Serialize(ref writer, (IList<SqlValue>) value, formatterResolver);
    }

    public SqlRow Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      IList<SqlValue> list = SqlRowFormatter.SqlValueFormatter.Deserialize(ref reader, formatterResolver);
      return list != null ? new SqlRow(list) : (SqlRow) null;
    }
  }
}
