// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Extensions.TableExtensionQueryHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.ResourceModel;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Interop.Common.Schema.Edm;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.Extensions
{
  internal static class TableExtensionQueryHelper
  {
    private static char[] SelectDelimiter = new char[1]
    {
      ','
    };

    internal static async Task<TableQuerySegment<TResult>> QueryCollectionsAsync<TResult>(
      int? maxItemCount,
      string filterString,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions,
      OperationContext operationContext)
    {
      TableExtensionQueryHelper.ValidateContinuationToken(token);
      FeedOptions defaultFeedOptions = TableExtensionQueryHelper.GetDefaultFeedOptions(requestOptions);
      defaultFeedOptions.RequestContinuation = token != null ? token.NextRowKey : (string) null;
      FeedResponse<DocumentCollection> response;
      if (string.IsNullOrEmpty(filterString))
      {
        response = await client.DocumentClient.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri("TablesDB"), defaultFeedOptions).AsDocumentQuery<DocumentCollection>().ExecuteNextAsync<DocumentCollection>();
      }
      else
      {
        string sqlQuery = QueryTranslator.GetSqlQuery("*", string.IsNullOrEmpty(filterString) ? filterString : ODataV3Translator.TranslateFilter(filterString), false, true, (IList<OrderByItem>) null);
        response = await client.DocumentClient.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri("TablesDB"), sqlQuery, defaultFeedOptions).AsDocumentQuery<object>().ExecuteNextAsync<DocumentCollection>();
      }
      operationContext.RequestResults.Add(response.ToRequestResult<DocumentCollection>());
      List<TResult> result = new List<TResult>();
      foreach (DocumentCollection documentCollection in response)
        result.Add((TResult) new DynamicTableEntity()
        {
          Properties = {
            {
              "TableName",
              new EntityProperty(documentCollection.Id)
            }
          }
        });
      TableQuerySegment<TResult> tableQuerySegment = new TableQuerySegment<TResult>(result);
      if (!string.IsNullOrEmpty(response.ResponseContinuation))
        tableQuerySegment.ContinuationToken = new TableContinuationToken()
        {
          NextRowKey = response.ResponseContinuation
        };
      tableQuerySegment.RequestCharge = new double?(response.RequestCharge);
      tableQuerySegment.ActivityId = response.ActivityId;
      return tableQuerySegment;
    }

    internal static async Task<TableQuerySegment<TResult>> QueryDocumentsAsync<TResult>(
      int? maxItemCount,
      string filterString,
      IList<string> selectColumns,
      TableContinuationToken token,
      CloudTableClient client,
      CloudTable table,
      EntityResolver<TResult> resolver,
      TableRequestOptions requestOptions,
      OperationContext operationContext,
      bool isLinqExpression,
      IList<OrderByItem> orderByItems,
      string tombstoneKey)
    {
      TableExtensionQueryHelper.ValidateContinuationToken(token);
      selectColumns = selectColumns != null ? (IList<string>) new List<string>((IEnumerable<string>) selectColumns) : (IList<string>) null;
      Dictionary<string, bool> selectedSystemProperties = new Dictionary<string, bool>();
      string sqlQuery = QueryTranslator.GetSqlQuery(TableExtensionQueryHelper.GetSelectList(selectColumns, requestOptions, out selectedSystemProperties), string.IsNullOrEmpty(filterString) ? filterString : ODataV3Translator.TranslateFilter(filterString), isLinqExpression, false, orderByItems, tombstoneKey, true);
      FeedOptions defaultFeedOptions = TableExtensionQueryHelper.GetDefaultFeedOptions(requestOptions);
      if (maxItemCount.HasValue)
        defaultFeedOptions.MaxItemCount = maxItemCount;
      defaultFeedOptions.SessionToken = requestOptions.SessionToken;
      defaultFeedOptions.RequestContinuation = token != null ? token.NextRowKey : (string) null;
      FeedResponse<Document> response = await client.DocumentClient.CreateDocumentQuery<Document>(table.GetCollectionUri(), sqlQuery, defaultFeedOptions).AsDocumentQuery<Document>().ExecuteNextAsync<Document>();
      operationContext.RequestResults.Add(response.ToRequestResult<Document>());
      List<TResult> result = new List<TResult>();
      foreach (Document document in response)
      {
        document.ETag = EtagHelper.ConvertFromBackEndETagFormat(document.ETag);
        IDictionary<string, EntityProperty> propertiesFromDocument = EntityTranslator.GetEntityPropertiesFromDocument(document, selectColumns);
        result.Add(resolver(selectedSystemProperties["PartitionKey"] ? document.GetPropertyValue<string>("$pk") : (string) null, selectedSystemProperties["RowKey"] ? document.GetPropertyValue<string>("$id") : (string) null, selectedSystemProperties["Timestamp"] ? (DateTimeOffset) document.Timestamp : new DateTimeOffset(), propertiesFromDocument, selectedSystemProperties["Etag"] ? document.ETag : (string) null));
      }
      TableQuerySegment<TResult> tableQuerySegment = new TableQuerySegment<TResult>(result);
      if (!string.IsNullOrEmpty(response.ResponseContinuation))
        tableQuerySegment.ContinuationToken = new TableContinuationToken()
        {
          NextRowKey = response.ResponseContinuation
        };
      tableQuerySegment.RequestCharge = new double?(response.RequestCharge);
      tableQuerySegment.ActivityId = response.ActivityId;
      return tableQuerySegment;
    }

    private static void ValidateContinuationToken(TableContinuationToken token)
    {
      if (token == null)
        return;
      if (!string.IsNullOrEmpty(token.NextPartitionKey))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Setting the value of the property '{0}' is not supported.", (object) "NextPartitionKey"));
      if (!string.IsNullOrEmpty(token.NextTableName))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Setting the value of the property '{0}' is not supported.", (object) "NextTableName"));
    }

    private static void TryAddIfNotExists(IList<string> list, string key)
    {
      if (list.Contains(key))
        return;
      list.Add(key);
    }

    private static string GetSelectList(
      IList<string> selectColumns,
      TableRequestOptions requestOptions,
      out Dictionary<string, bool> selectedSystemProperties)
    {
      selectedSystemProperties = new Dictionary<string, bool>();
      foreach (string key in EdmSchemaMapping.SystemPropertiesMapping.Keys)
        selectedSystemProperties.Add(key, selectColumns == null || selectColumns.Count == 0);
      if (selectColumns == null || selectColumns.Count == 0)
        return "*";
      string str1 = string.Empty;
      foreach (string selectColumn in (IEnumerable<string>) selectColumns)
      {
        string str2 = selectColumn;
        if (selectedSystemProperties.ContainsKey(selectColumn))
        {
          str2 = EdmSchemaMapping.SystemPropertiesMapping[selectColumn];
          selectedSystemProperties[selectColumn] = true;
        }
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}['{2}'],", (object) str1, (object) EdmSchemaMapping.EntityName, (object) str2);
      }
      if (requestOptions != null)
      {
        bool? systemProperties = requestOptions.ProjectSystemProperties;
        if (systemProperties.HasValue)
        {
          systemProperties = requestOptions.ProjectSystemProperties;
          if (!systemProperties.Value)
            goto label_25;
        }
      }
      foreach (string key in EdmSchemaMapping.SystemPropertiesMapping.Keys)
      {
        if (!selectedSystemProperties[key])
        {
          selectedSystemProperties[key] = true;
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}['{2}'],", (object) str1, (object) EdmSchemaMapping.EntityName, (object) EdmSchemaMapping.SystemPropertiesMapping[key]);
        }
      }
label_25:
      return str1.Trim(TableExtensionQueryHelper.SelectDelimiter);
    }

    private static FeedOptions GetDefaultFeedOptions(TableRequestOptions options)
    {
      if (options != null && !TableExtensionSettings.EnableAppSettingsBasedOptions)
      {
        FeedOptions defaultFeedOptions = new FeedOptions();
        defaultFeedOptions.MaxItemCount = options.TableQueryMaxItemCount;
        defaultFeedOptions.EnableScanInQuery = options.TableQueryEnableScan;
        defaultFeedOptions.EnableCrossPartitionQuery = true;
        int? nullable = options.TableQueryMaxBufferedItemCount;
        defaultFeedOptions.MaxBufferedItemCount = nullable.Value;
        nullable = options.TableQueryMaxDegreeOfParallelism;
        defaultFeedOptions.MaxDegreeOfParallelism = nullable.Value;
        defaultFeedOptions.ResponseContinuationTokenLimitInKb = options.TableQueryContinuationTokenLimitInKb;
        defaultFeedOptions.ConsistencyLevel = CloudTableClient.ToDocDbConsistencyLevel(options.ConsistencyLevel);
        return defaultFeedOptions;
      }
      return new FeedOptions()
      {
        MaxItemCount = TableExtensionSettings.MaxItemCount,
        EnableScanInQuery = new bool?(TableExtensionSettings.EnableScan),
        EnableCrossPartitionQuery = true,
        MaxBufferedItemCount = TableExtensionSettings.TableQueryMaxBufferedItemCount,
        MaxDegreeOfParallelism = TableExtensionSettings.MaxDegreeOfParallelism,
        ResponseContinuationTokenLimitInKb = TableExtensionSettings.ContinuationTokenLimitInKb
      };
    }
  }
}
