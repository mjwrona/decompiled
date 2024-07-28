// Decompiled with JetBrains decompiler
// Type: Nest.LazyDocument
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
  [JsonFormatter(typeof (LazyDocumentFormatter))]
  public class LazyDocument : ILazyDocument
  {
    private readonly IElasticsearchSerializer _sourceSerializer;
    private readonly IElasticsearchSerializer _requestResponseSerializer;
    private readonly IMemoryStreamFactory _memoryStreamFactory;

    internal LazyDocument(byte[] bytes, IJsonFormatterResolver formatterResolver)
    {
      this.Bytes = bytes;
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      this._sourceSerializer = connectionSettings.SourceSerializer;
      this._requestResponseSerializer = connectionSettings.RequestResponseSerializer;
      this._memoryStreamFactory = connectionSettings.MemoryStreamFactory;
    }

    internal byte[] Bytes { get; }

    internal T AsUsingRequestResponseSerializer<T>()
    {
      using (MemoryStream memoryStream = this._memoryStreamFactory.Create(this.Bytes))
        return this._requestResponseSerializer.Deserialize<T>((Stream) memoryStream);
    }

    public T As<T>()
    {
      using (MemoryStream memoryStream = this._memoryStreamFactory.Create(this.Bytes))
        return this._sourceSerializer.Deserialize<T>((Stream) memoryStream);
    }

    public object As(Type objectType)
    {
      using (MemoryStream memoryStream = this._memoryStreamFactory.Create(this.Bytes))
        return this._sourceSerializer.Deserialize(objectType, (Stream) memoryStream);
    }

    public Task<T> AsAsync<T>(CancellationToken ct = default (CancellationToken))
    {
      using (MemoryStream memoryStream = this._memoryStreamFactory.Create(this.Bytes))
        return this._sourceSerializer.DeserializeAsync<T>((Stream) memoryStream, ct);
    }

    public Task<object> AsAsync(Type objectType, CancellationToken ct = default (CancellationToken))
    {
      using (MemoryStream memoryStream = this._memoryStreamFactory.Create(this.Bytes))
        return this._sourceSerializer.DeserializeAsync(objectType, (Stream) memoryStream, ct);
    }
  }
}
