// Decompiled with JetBrains decompiler
// Type: Nest.SourceWriteFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class SourceWriteFormatter<T> : SourceFormatter<T>
  {
    public override void Serialize(
      ref JsonWriter writer,
      T value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
        writer.WriteNull();
      else if (value.GetType().IsNestType())
        formatterResolver.GetFormatter<T>().Serialize(ref writer, value, formatterResolver);
      else
        base.Serialize(ref writer, value, formatterResolver);
    }
  }
}
