// Decompiled with JetBrains decompiler
// Type: Nest.BulkRequestFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class BulkRequestFormatter : IJsonFormatter<IBulkRequest>, IJsonFormatter
  {
    private const byte Newline = 10;

    private static SourceWriteFormatter<object> SourceWriter { get; } = new SourceWriteFormatter<object>();

    public IBulkRequest Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => throw new NotSupportedException();

    public void Serialize(
      ref JsonWriter writer,
      IBulkRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Operations == null)
        return;
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      Inferrer inferrer = connectionSettings.Inferrer;
      IJsonFormatter<object> formatter = formatterResolver.GetFormatter<object>();
      for (int index = 0; index < value.Operations.Count; ++index)
      {
        IBulkOperation operation = value.Operations[index];
        IBulkOperation bulkOperation1 = operation;
        if ((object) bulkOperation1.Index == null)
        {
          IBulkOperation bulkOperation2 = bulkOperation1;
          IndexName indexName = value.Index;
          if ((object) indexName == null)
            indexName = (IndexName) operation.ClrType;
          bulkOperation2.Index = indexName;
        }
        if (operation.Index.Equals((object) value.Index))
          operation.Index = (IndexName) null;
        operation.Id = operation.GetIdForOperation(inferrer);
        operation.Routing = operation.GetRoutingForOperation(inferrer);
        writer.WriteBeginObject();
        writer.WritePropertyName(operation.Operation);
        formatter.Serialize(ref writer, (object) operation, formatterResolver);
        writer.WriteEndObject();
        writer.WriteRaw((byte) 10);
        object body = operation.GetBody();
        if (body != null)
        {
          if (operation.Operation == "update" || body is ILazyDocument)
            connectionSettings.RequestResponseSerializer.SerializeUsingWriter<object>(ref writer, body, (IConnectionConfigurationValues) connectionSettings, SerializationFormatting.None);
          else
            BulkRequestFormatter.SourceWriter.Serialize(ref writer, body, formatterResolver);
          writer.WriteRaw((byte) 10);
        }
      }
    }
  }
}
