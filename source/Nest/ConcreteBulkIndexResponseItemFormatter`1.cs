// Decompiled with JetBrains decompiler
// Type: Nest.ConcreteBulkIndexResponseItemFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class ConcreteBulkIndexResponseItemFormatter<T> : IJsonFormatter<T>, IJsonFormatter where T : BulkResponseItemBase
  {
    public void Serialize(ref Elasticsearch.Net.Utf8Json.JsonWriter writer, T value, IJsonFormatterResolver formatterResolver) => throw new NotImplementedException();

    public T Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver) => DynamicObjectResolver.AllowPrivateExcludeNullCamelCase.GetFormatter<T>().Deserialize(ref reader, formatterResolver);
  }
}
