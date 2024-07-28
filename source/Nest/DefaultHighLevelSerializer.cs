// Decompiled with JetBrains decompiler
// Type: Nest.DefaultHighLevelSerializer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class DefaultHighLevelSerializer : IElasticsearchSerializer, IInternalSerializer
  {
    public DefaultHighLevelSerializer(IJsonFormatterResolver formatterResolver) => this.FormatterResolver = formatterResolver;

    private IJsonFormatterResolver FormatterResolver { get; }

    bool IInternalSerializer.TryGetJsonFormatter(out IJsonFormatterResolver formatterResolver)
    {
      formatterResolver = this.FormatterResolver;
      return true;
    }

    public T Deserialize<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, this.FormatterResolver);

    public object Deserialize(Type type, Stream stream) => JsonSerializer.NonGeneric.Deserialize(type, stream, this.FormatterResolver);

    public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default (CancellationToken)) => JsonSerializer.DeserializeAsync<T>(stream, this.FormatterResolver);

    public Task<object> DeserializeAsync(
      Type type,
      Stream stream,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return JsonSerializer.NonGeneric.DeserializeAsync(type, stream, this.FormatterResolver);
    }

    public virtual void Serialize<T>(
      T data,
      Stream writableStream,
      SerializationFormatting formatting = SerializationFormatting.None)
    {
      JsonSerializer.Serialize<T>(writableStream, data, this.FormatterResolver);
    }

    public Task SerializeAsync<T>(
      T data,
      Stream stream,
      SerializationFormatting formatting = SerializationFormatting.None,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return JsonSerializer.SerializeAsync<T>(stream, data, this.FormatterResolver);
    }
  }
}
