// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.DocumentEntityCollectionBaseHelpers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal sealed class DocumentEntityCollectionBaseHelpers
  {
    public static async Task<ResourceResponse<Document>> HandleEntityRetrieveAsync(
      string tableName,
      string partitionKey,
      string rowKey,
      IDocumentClient client,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Uri documentUri = UriFactory.CreateDocumentUri("TablesDB", tableName, rowKey);
      requestOptions = DocumentEntityCollectionBaseHelpers.SetPartitionKey(requestOptions, partitionKey);
      requestOptions.JsonSerializerSettings = EntityTranslator.JsonSerializerSettings;
      return await client.ReadDocumentAsync(documentUri.ToString(), requestOptions, cancellationToken);
    }

    public static async Task<ResourceResponse<Document>> HandleEntityReplaceOnlyAsync(
      string tableName,
      string partitionKey,
      string rowKey,
      string ifMatch,
      IDocumentClient client,
      Document document,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      Uri documentUri = UriFactory.CreateDocumentUri("TablesDB", tableName, rowKey);
      requestOptions = DocumentEntityCollectionBaseHelpers.SetPartitionKey(requestOptions, partitionKey);
      requestOptions.JsonSerializerSettings = EntityTranslator.JsonSerializerSettings;
      if (!string.IsNullOrEmpty(ifMatch))
        requestOptions.AccessCondition = new AccessCondition()
        {
          Type = AccessConditionType.IfMatch,
          Condition = ifMatch
        };
      return await client.ReplaceDocumentAsync(documentUri.ToString(), (object) document, requestOptions, cancellationToken);
    }

    public static async Task<ResourceResponse<Document>> HandleEntityDeleteAsync(
      string tableName,
      string partitionKey,
      string rowKey,
      string ifMatch,
      IDocumentClient client,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Uri documentUri = UriFactory.CreateDocumentUri("TablesDB", tableName, rowKey);
      requestOptions = DocumentEntityCollectionBaseHelpers.SetPartitionKey(requestOptions, partitionKey);
      requestOptions.JsonSerializerSettings = EntityTranslator.JsonSerializerSettings;
      if (!string.IsNullOrEmpty(ifMatch))
        requestOptions.AccessCondition = new AccessCondition()
        {
          Type = AccessConditionType.IfMatch,
          Condition = ifMatch
        };
      return await client.DeleteDocumentAsync(documentUri.ToString(), requestOptions);
    }

    public static async Task<StoredProcedureResponse<string>> HandleEntityMergeAsync(
      string tableName,
      TableOperationType operationType,
      string partitionKey,
      string ifMatch,
      IDocumentClient client,
      Document document,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Uri storedProcedureUri = UriFactory.CreateStoredProcedureUri("TablesDB", tableName, "__.sys.tablesBatchOperation");
      List<Document> documentList = new List<Document>();
      documentList.Add(document);
      List<TableOperationType> tableOperationTypeList = new List<TableOperationType>();
      tableOperationTypeList.Add(operationType);
      List<string> stringList = new List<string>();
      stringList.Add(ifMatch);
      AccessCondition accessCondition = (AccessCondition) null;
      if (operationType == TableOperationType.Merge)
        accessCondition = new AccessCondition()
        {
          Type = AccessConditionType.IfMatch,
          Condition = ifMatch
        };
      requestOptions = DocumentEntityCollectionBaseHelpers.SetPartitionKey(requestOptions, partitionKey);
      requestOptions.AccessCondition = accessCondition;
      requestOptions.JsonSerializerSettings = EntityTranslator.JsonSerializerSettings;
      return await client.ExecuteStoredProcedureAsync<string>(storedProcedureUri.ToString(), requestOptions, (object) documentList.ToArray(), (object) tableOperationTypeList.ToArray(), (object) stringList.ToArray());
    }

    public static async Task<ResourceResponse<Document>> HandleEntityFeedInsertAsync(
      string tableName,
      IDocumentClient client,
      Document document,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Uri documentCollectionUri = UriFactory.CreateDocumentCollectionUri("TablesDB", tableName);
      if (requestOptions == null)
        requestOptions = new RequestOptions();
      requestOptions.JsonSerializerSettings = EntityTranslator.JsonSerializerSettings;
      return await client.CreateDocumentAsync(documentCollectionUri.ToString(), (object) document, requestOptions, true);
    }

    private static RequestOptions SetPartitionKey(
      RequestOptions requestOptions,
      string partitionKey)
    {
      if (requestOptions == null)
        requestOptions = new RequestOptions();
      requestOptions.PartitionKey = new PartitionKey((object) partitionKey);
      return requestOptions;
    }
  }
}
