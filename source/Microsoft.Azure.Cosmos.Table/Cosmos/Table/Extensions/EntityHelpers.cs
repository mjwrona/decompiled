// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.EntityHelpers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class EntityHelpers
  {
    public static Document GetDocumentFromEntity(
      ITableEntity entity,
      OperationContext context,
      TableRequestOptions options)
    {
      if (entity == null)
        throw new ArgumentException("Entity should not be null.");
      TableEntityValidationHelper.ValidatePartitionKey(entity.PartitionKey);
      TableEntityValidationHelper.ValidateRowKey(entity.RowKey);
      return EntityTranslator.GetDocumentFromEntityProperties(entity.WriteEntity(context), entity.PartitionKey, entity.RowKey, false);
    }

    public static TableResult GetTableResultFromResponse(
      ResourceResponse<DocumentCollection> response,
      OperationContext context)
    {
      return new TableResult()
      {
        Etag = response.ResponseHeaders["ETag"],
        HttpStatusCode = (int) response.StatusCode,
        RequestCharge = new double?(response.RequestCharge),
        ActivityId = response.ActivityId
      };
    }

    public static StorageException GetTableResultFromException(
      Exception exception,
      TableOperation tableOperation = null)
    {
      return exception.ToStorageException(tableOperation);
    }

    public static ITableEntity GetEntityFromResourceStream(
      Stream stream,
      OperationContext context,
      IList<string> selectColumns)
    {
      IDictionary<string, EntityProperty> entityProperties;
      IDictionary<string, EntityProperty> documentDBProperties;
      EntityHelpers.GetPropertiesFromResourceStream(stream, selectColumns, out entityProperties, out documentDBProperties);
      EdmConverter.ValidateDocumentDBProperties(documentDBProperties);
      EntityProperty entityProperty1 = documentDBProperties["$pk"];
      EntityProperty entityProperty2 = documentDBProperties["$id"];
      EntityProperty entityProperty3 = documentDBProperties["_etag"];
      EntityProperty entityProperty4 = documentDBProperties["_ts"];
      DynamicTableEntity fromResourceStream = new DynamicTableEntity(entityProperty1.StringValue, entityProperty2.StringValue);
      fromResourceStream.ETag = entityProperty3.StringValue;
      fromResourceStream.Timestamp = (DateTimeOffset) new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(entityProperty4.DoubleValue.Value);
      fromResourceStream.ReadEntity(entityProperties, context);
      return (ITableEntity) fromResourceStream;
    }

    public static ITableEntity GetEntityFromDocument(
      Document document,
      OperationContext context,
      IList<string> selectColumns)
    {
      string partitionKey = (string) null;
      string rowKey = (string) null;
      string eTag = (string) null;
      DateTimeOffset timestamp = new DateTimeOffset();
      IDictionary<string, EntityProperty> entityProperties = (IDictionary<string, EntityProperty>) null;
      EntityTranslator.GetEntityPropertiesFromDocument(document, selectColumns, out partitionKey, out rowKey, out eTag, out timestamp, out entityProperties);
      DynamicTableEntity entityFromDocument = new DynamicTableEntity(partitionKey, rowKey);
      entityFromDocument.ETag = eTag;
      entityFromDocument.Timestamp = timestamp;
      entityFromDocument.PartitionKey = partitionKey;
      entityFromDocument.ReadEntity(entityProperties, context);
      return (ITableEntity) entityFromDocument;
    }

    public static void GetPropertiesFromResourceStream(
      Stream stream,
      IList<string> selectColumns,
      out IDictionary<string, EntityProperty> entityProperties,
      out IDictionary<string, EntityProperty> documentDBProperties)
    {
      using (StreamReader streamReader = new StreamReader(stream))
        EdmConverter.GetPropertiesFromDocument(selectColumns, streamReader.ReadToEnd(), out entityProperties, out documentDBProperties);
    }
  }
}
