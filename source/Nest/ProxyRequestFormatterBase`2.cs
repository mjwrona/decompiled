// Decompiled with JetBrains decompiler
// Type: Nest.ProxyRequestFormatterBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;

namespace Nest
{
  internal abstract class ProxyRequestFormatterBase<TRequestInterface, TRequest> : 
    IJsonFormatter<TRequestInterface>,
    IJsonFormatter
    where TRequestInterface : IProxyRequest
    where TRequest : TRequestInterface
  {
    public TRequestInterface Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      using (MemoryStream memoryStream = connectionSettings.MemoryStreamFactory.Create(arraySegment.Array, arraySegment.Offset, arraySegment.Count))
      {
        Type genericTypeArgument = typeof (TRequest).GenericTypeArguments[0];
        object obj = connectionSettings.SourceSerializer.Deserialize(genericTypeArgument, (Stream) memoryStream);
        TRequest request;
        if (!typeof (TRequest).IsGenericTypeDefinition)
          request = (TRequest) typeof (TRequest).CreateInstance(obj, null, null);
        else
          request = (TRequest) typeof (TRequest).CreateGenericInstance(genericTypeArgument, obj, null, null);
        return (TRequestInterface) request;
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      TRequestInterface value,
      IJsonFormatterResolver formatterResolver)
    {
      IProxyRequest proxyRequest = (IProxyRequest) value;
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      IElasticsearchSerializer sourceSerializer = connectionSettings.SourceSerializer;
      using (MemoryStream memoryStream = connectionSettings.MemoryStreamFactory.Create())
      {
        proxyRequest.WriteJson(sourceSerializer, (Stream) memoryStream, SerializationFormatting.None);
        writer.WriteRaw(memoryStream);
      }
    }
  }
}
