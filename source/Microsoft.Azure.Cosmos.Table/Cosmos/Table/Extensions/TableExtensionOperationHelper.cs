// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionOperationHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.ResourceModel;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class TableExtensionOperationHelper
  {
    internal static async Task<TResult> ExecuteOperationAsync<TResult>(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TResult : class
    {
      TableResult tableResult;
      try
      {
        switch (operation.OperationType)
        {
          case TableOperationType.Insert:
            tableResult = await TableExtensionOperationHelper.HandleInsertAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          case TableOperationType.Delete:
            tableResult = await TableExtensionOperationHelper.HandleDeleteAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          case TableOperationType.Replace:
            tableResult = await TableExtensionOperationHelper.HandleReplaceAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          case TableOperationType.Merge:
          case TableOperationType.InsertOrMerge:
            tableResult = await TableExtensionOperationHelper.HandleMergeAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          case TableOperationType.InsertOrReplace:
            tableResult = await TableExtensionOperationHelper.HandleUpsertAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          case TableOperationType.Retrieve:
            tableResult = await TableExtensionOperationHelper.HandleReadAsync(operation, client, table, requestOptions, operationContext, cancellationToken);
            break;
          default:
            throw new NotSupportedException();
        }
      }
      catch (Exception ex)
      {
        TableOperation tableOperation = operation;
        StorageException resultFromException = EntityHelpers.GetTableResultFromException(ex, tableOperation);
        operationContext.RequestResults.Add(resultFromException.RequestInformation);
        throw resultFromException;
      }
      operationContext.FireRequestCompleted(new RequestEventArgs(operationContext.LastResult));
      return tableResult as TResult;
    }

    internal static async Task<TResult> ExecuteBatchOperationAsync<TResult>(
      TableBatchOperation batch,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
      where TResult : class
    {
      TableOperationType currentOperationType = batch.First<TableOperation>().OperationType;
      batch.First<TableOperation>();
      try
      {
        List<Document> documentList = new List<Document>();
        List<TableOperationType> tableOperationTypeList = new List<TableOperationType>();
        List<string> stringList = new List<string>();
        foreach (TableOperation tableOperation in batch)
        {
          currentOperationType = tableOperation.OperationType;
          Document document = tableOperation.OperationType != TableOperationType.Retrieve ? EntityHelpers.GetDocumentFromEntity(tableOperation.Entity, operationContext, requestOptions) : EntityTranslator.GetDocumentWithPartitionAndRowKey(tableOperation.RetrievePartitionKey, tableOperation.RetrieveRowKey);
          documentList.Add(document);
          tableOperationTypeList.Add(tableOperation.OperationType);
          stringList.Add(tableOperation.Entity == null ? string.Empty : EtagHelper.ConvertToBackEndETagFormat(tableOperation.Entity.ETag));
        }
        RequestOptions requestOptions1 = TableExtensionOperationHelper.GetRequestOptions(batch.batchPartitionKey, requestOptions);
        Uri storedProcedureUri = UriFactory.CreateStoredProcedureUri("TablesDB", table.Name, "__.sys.tablesBatchOperation");
        StoredProcedureResponse<string> procedureResponse = await table.ServiceClient.DocumentClient.ExecuteStoredProcedureAsync<string>(storedProcedureUri, requestOptions1, (object) documentList.ToArray(), (object) tableOperationTypeList.ToArray(), (object) stringList.ToArray());
        JArray jarray = JArray.Parse(procedureResponse.Response);
        TableBatchResult tableBatchResult = new TableBatchResult();
        tableBatchResult.RequestCharge = new double?(procedureResponse.RequestCharge);
        tableBatchResult.ActivityId = procedureResponse.ActivityId;
        for (int index = 0; index < jarray.Count; ++index)
          tableBatchResult.Add(TableExtensionOperationHelper.GetTableResultFromDocument(batch[index], jarray[index].ToObject<Document>(), operationContext, requestOptions, procedureResponse.SessionToken, 0.0, procedureResponse.ActivityId));
        return tableBatchResult as TResult;
      }
      catch (Exception ex)
      {
        if (ex is DocumentClientException documentClientException)
        {
          HttpStatusCode? statusCode = documentClientException.StatusCode;
          HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
          if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue && documentClientException.Message.Contains("Resource Not Found") && currentOperationType == TableOperationType.Retrieve)
          {
            TableBatchResult tableBatchResult = new TableBatchResult();
            tableBatchResult.Add(new TableResult()
            {
              Etag = (string) null,
              HttpStatusCode = 404,
              Result = (object) null
            });
            tableBatchResult.RequestCharge = new double?(documentClientException.RequestCharge);
            tableBatchResult.ActivityId = documentClientException.ActivityId;
            return tableBatchResult as TResult;
          }
        }
        TableErrorResult tableErrorResult = ex.TranslateDocumentErrorForStoredProcs(batchOperationCount: batch.Count);
        RequestResult requestResult = TableExtensionOperationHelper.GenerateRequestResult(tableErrorResult.ExtendedErroMessage, tableErrorResult.HttpStatusCode, tableErrorResult.ExtendedErrorCode, tableErrorResult.ExtendedErroMessage, tableErrorResult.ServiceRequestID, new double?(tableErrorResult.RequestCharge));
        StorageException storageException = new StorageException(requestResult, requestResult.ExtendedErrorInformation.ErrorMessage, ex);
        if (documentClientException != null)
          TableExtensionOperationHelper.PopulateOperationContextForBatchOperations(operationContext, storageException, documentClientException.ActivityId);
        throw storageException;
      }
    }

    internal static RequestResult GenerateRequestResult(
      string httpStatusMessage,
      int httpStatusCode,
      string extendedInfoErrorCode,
      string extendedInfoErrorMessage,
      string serviceRequestId,
      double? requestCharge)
    {
      return new RequestResult()
      {
        HttpStatusMessage = httpStatusMessage,
        HttpStatusCode = httpStatusCode,
        ExtendedErrorInformation = new StorageExtendedErrorInformation()
        {
          ErrorCode = extendedInfoErrorCode,
          ErrorMessage = extendedInfoErrorMessage
        },
        ServiceRequestID = serviceRequestId,
        RequestCharge = requestCharge
      };
    }

    private static void PopulateOperationContextForBatchOperations(
      OperationContext operationContext,
      StorageException storageException,
      string serviceRequestId)
    {
      operationContext?.RequestResults.Add(storageException.ToRequestResult(serviceRequestId));
    }

    private static async Task<TableResult> HandleMergeAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      Document documentFromEntity = EntityHelpers.GetDocumentFromEntity(operation.Entity, context, options);
      RequestOptions requestOptions = TableExtensionOperationHelper.GetRequestOptions(operation, options);
      StoredProcedureResponse<string> procedureResponse = await DocumentEntityCollectionBaseHelpers.HandleEntityMergeAsync(table.Name, operation.OperationType, operation.PartitionKey, EtagHelper.ConvertToBackEndETagFormat(operation.ETag), client.DocumentClient, documentFromEntity, requestOptions, cancellationToken);
      return TableExtensionOperationHelper.GetTableResultFromDocument(operation, JsonConvert.DeserializeObject<List<Document>>(procedureResponse.Response).FirstOrDefault<Document>(), context, options, procedureResponse.SessionToken, procedureResponse.RequestCharge, procedureResponse.ActivityId);
    }

    private static async Task<TableResult> HandleInsertAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      if (operation.IsTableEntity)
      {
        DynamicTableEntity entity = (DynamicTableEntity) operation.Entity;
        string cosmosTableName = entity.GetCosmosTableName();
        int? cosmosTableThroughput = entity.GetCosmosTableThroughput();
        IndexingPolicy tableIndexingPolicy = entity.GetCosmosTableIndexingPolicy();
        RequestOptions defaultRequestOptions = TableExtensionOperationHelper.GetDefaultRequestOptions((string) null, cosmosTableThroughput);
        return EntityHelpers.GetTableResultFromResponse(await DocumentCollectionBaseHelpers.HandleCollectionFeedInsertAsync(client.DocumentClient, cosmosTableName, tableIndexingPolicy, defaultRequestOptions, cancellationToken), context);
      }
      ResourceResponse<Document> response = await DocumentEntityCollectionBaseHelpers.HandleEntityFeedInsertAsync(table.Name, client.DocumentClient, EntityHelpers.GetDocumentFromEntity(operation.Entity, context, options), TableExtensionOperationHelper.GetRequestOptions(operation, options), cancellationToken);
      return TableExtensionOperationHelper.GetTableResultFromResponse(operation, response, context, options, operation.SelectColumns, response.SessionToken);
    }

    private static async Task<TableResult> HandleUpsertAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      Document documentFromEntity = EntityHelpers.GetDocumentFromEntity(operation.Entity, context, options);
      RequestOptions requestOptions = TableExtensionOperationHelper.GetRequestOptions(operation, options);
      ResourceResponse<Document> response = await client.DocumentClient.UpsertDocumentAsync(table.GetCollectionUri(), (object) documentFromEntity, requestOptions, true, cancellationToken);
      return TableExtensionOperationHelper.GetTableResultFromResponse(operation, response, context, options, operation.SelectColumns, response.SessionToken);
    }

    private static async Task<TableResult> HandleReplaceAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      Document documentFromEntity = EntityHelpers.GetDocumentFromEntity(operation.Entity, context, options);
      RequestOptions requestOptions = TableExtensionOperationHelper.GetRequestOptions(operation, options);
      ResourceResponse<Document> response = await DocumentEntityCollectionBaseHelpers.HandleEntityReplaceOnlyAsync(table.Name, operation.PartitionKey, operation.RowKey, EtagHelper.ConvertToBackEndETagFormat(operation.ETag), client.DocumentClient, documentFromEntity, requestOptions, cancellationToken);
      return TableExtensionOperationHelper.GetTableResultFromResponse(operation, response, context, options, operation.SelectColumns, response.SessionToken);
    }

    private static async Task<TableResult> HandleDeleteAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      if (operation.IsTableEntity)
        return EntityHelpers.GetTableResultFromResponse(await client.DocumentClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("TablesDB", ((DynamicTableEntity) operation.Entity).Properties["TableName"].StringValue)), context);
      RequestOptions requestOptions = TableExtensionOperationHelper.GetRequestOptions(operation, options);
      ResourceResponse<Document> response = await DocumentEntityCollectionBaseHelpers.HandleEntityDeleteAsync(table.Name, operation.PartitionKey, operation.RowKey, EtagHelper.ConvertToBackEndETagFormat(operation.ETag), client.DocumentClient, requestOptions, cancellationToken);
      return TableExtensionOperationHelper.GetTableResultFromResponse(operation, response, context, options, operation.SelectColumns, response.SessionToken);
    }

    private static async Task<TableResult> HandleReadAsync(
      TableOperation operation,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions options,
      OperationContext context,
      CancellationToken cancellationToken)
    {
      try
      {
        if (operation.IsTableEntity)
          return EntityHelpers.GetTableResultFromResponse(await DocumentCollectionBaseHelpers.HandleDocumentCollectionRetrieveAsync(operation.GetCosmosTableName(), client.DocumentClient), context);
        ResourceResponse<Document> response = await DocumentEntityCollectionBaseHelpers.HandleEntityRetrieveAsync(table.Name, operation.PartitionKey, operation.RowKey, client.DocumentClient, TableExtensionOperationHelper.GetRequestOptions(operation, options), cancellationToken);
        return TableExtensionOperationHelper.GetTableResultFromResponse(operation, response, context, options, operation.SelectColumns, response.SessionToken);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
          return new TableResult()
          {
            HttpStatusCode = 404,
            RequestCharge = new double?(ex.RequestCharge),
            ActivityId = ex.ActivityId
          };
        throw;
      }
    }

    private static TableResult GetTableResultFromResponse(
      TableOperation operation,
      ResourceResponse<Document> response,
      OperationContext context,
      TableRequestOptions options,
      List<string> selectColumns,
      string sessionToken)
    {
      TableResult resultFromResponse = new TableResult();
      resultFromResponse.Etag = EtagHelper.ConvertFromBackEndETagFormat(response.ResponseHeaders["ETag"]);
      resultFromResponse.HttpStatusCode = (int) response.StatusCode;
      resultFromResponse.SessionToken = sessionToken;
      resultFromResponse.RequestCharge = new double?(response.RequestCharge);
      resultFromResponse.ActivityId = response.ActivityId;
      if (operation.Entity != null && !string.IsNullOrEmpty(resultFromResponse.Etag))
        operation.Entity.ETag = resultFromResponse.Etag;
      if (operation.OperationType == TableOperationType.InsertOrReplace || operation.OperationType == TableOperationType.Replace || !operation.EchoContent)
        resultFromResponse.HttpStatusCode = 204;
      if (operation.OperationType != TableOperationType.Retrieve)
      {
        if (operation.OperationType != TableOperationType.Delete)
        {
          resultFromResponse.Result = (object) operation.Entity;
          if (resultFromResponse.Result is ITableEntity result)
          {
            if (resultFromResponse.Etag != null)
              result.ETag = resultFromResponse.Etag;
            result.Timestamp = (DateTimeOffset) response.Resource.Timestamp;
          }
        }
      }
      else if (response.Resource != null)
      {
        if (operation.RetrieveResolver == null)
        {
          resultFromResponse.Result = (object) EntityHelpers.GetEntityFromResourceStream(response.ResponseStream, context, (IList<string>) operation.SelectColumns);
        }
        else
        {
          IDictionary<string, EntityProperty> entityProperties;
          IDictionary<string, EntityProperty> documentDBProperties;
          EntityHelpers.GetPropertiesFromResourceStream(response.ResponseStream, (IList<string>) operation.SelectColumns, out entityProperties, out documentDBProperties);
          if (options.ProjectSystemProperties.Value)
          {
            EdmConverter.ValidateDocumentDBProperties(documentDBProperties);
            EntityProperty entityProperty1 = documentDBProperties["$pk"];
            EntityProperty entityProperty2 = documentDBProperties["$id"];
            EntityProperty entityProperty3 = documentDBProperties["_etag"];
            EntityProperty entityProperty4 = documentDBProperties["_ts"];
            string str = EtagHelper.ConvertFromBackEndETagFormat(entityProperty3.StringValue);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(entityProperty4.DoubleValue.Value);
            resultFromResponse.Result = operation.RetrieveResolver(entityProperty1.StringValue, entityProperty2.StringValue, (DateTimeOffset) dateTime, entityProperties, str);
          }
          else
            resultFromResponse.Result = operation.RetrieveResolver((string) null, (string) null, new DateTimeOffset(), entityProperties, (string) null);
        }
      }
      return resultFromResponse;
    }

    private static TableResult GetTableResultFromDocument(
      TableOperation operation,
      Document response,
      OperationContext context,
      TableRequestOptions requestOptions,
      string sessionToken,
      double requestCharge,
      string activityId)
    {
      response.ETag = EtagHelper.ConvertFromBackEndETagFormat(response.ETag);
      TableResult resultFromDocument = new TableResult();
      resultFromDocument.Etag = response.ETag;
      resultFromDocument.HttpStatusCode = TableExtensionOperationHelper.GetSuccessStatusCodeFromOperationType(operation.OperationType);
      resultFromDocument.SessionToken = sessionToken;
      resultFromDocument.RequestCharge = new double?(requestCharge);
      resultFromDocument.ActivityId = activityId;
      if (operation.OperationType != TableOperationType.Retrieve)
      {
        operation.Entity.ETag = response.ETag;
        resultFromDocument.Result = (object) operation.Entity;
      }
      else
        resultFromDocument.Result = operation.RetrieveResolver == null ? (object) EntityHelpers.GetEntityFromDocument(response, context, (IList<string>) operation.SelectColumns) : operation.RetrieveResolver(response.GetPropertyValue<string>("$pk"), response.Id, (DateTimeOffset) response.Timestamp, EntityTranslator.GetEntityPropertiesFromDocument(response, (IList<string>) operation.SelectColumns), response.ETag);
      return resultFromDocument;
    }

    private static int GetSuccessStatusCodeFromOperationType(TableOperationType operationType)
    {
      switch (operationType)
      {
        case TableOperationType.Insert:
          return 201;
        case TableOperationType.Delete:
        case TableOperationType.Replace:
        case TableOperationType.Merge:
        case TableOperationType.InsertOrReplace:
        case TableOperationType.InsertOrMerge:
          return 204;
        case TableOperationType.Retrieve:
          return 200;
        default:
          return 200;
      }
    }

    private static RequestOptions GetDefaultRequestOptions(string partitionKey, int? throughput = null) => new RequestOptions()
    {
      PartitionKey = string.IsNullOrEmpty(partitionKey) ? (PartitionKey) null : new PartitionKey((object) partitionKey),
      OfferThroughput = throughput
    };

    private static RequestOptions GetRequestOptions(
      TableOperation operation,
      TableRequestOptions options)
    {
      return TableExtensionOperationHelper.GetRequestOptions(operation.PartitionKey, options);
    }

    private static RequestOptions GetRequestOptions(
      string partitionKey,
      TableRequestOptions options)
    {
      RequestOptions defaultRequestOptions = TableExtensionOperationHelper.GetDefaultRequestOptions(partitionKey);
      if (options != null)
      {
        defaultRequestOptions.SessionToken = options.SessionToken;
        defaultRequestOptions.ConsistencyLevel = CloudTableClient.ToDocDbConsistencyLevel(options.ConsistencyLevel);
      }
      return defaultRequestOptions;
    }
  }
}
