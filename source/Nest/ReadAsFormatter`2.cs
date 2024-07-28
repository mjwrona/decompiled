// Decompiled with JetBrains decompiler
// Type: Nest.ReadAsFormatter`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;

namespace Nest
{
  internal class ReadAsFormatter<TRead, T> : IJsonFormatter<T>, IJsonFormatter where TRead : T
  {
    public virtual T Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver) => (T) formatterResolver.GetFormatter<TRead>().Deserialize(ref reader, formatterResolver);

    public virtual void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      T value,
      IJsonFormatterResolver formatterResolver)
    {
      this.SerializeInternal(ref writer, value, formatterResolver);
    }

    public virtual void SerializeInternal(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      T value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<T>().Serialize(ref writer, value, formatterResolver);
    }
  }
}
