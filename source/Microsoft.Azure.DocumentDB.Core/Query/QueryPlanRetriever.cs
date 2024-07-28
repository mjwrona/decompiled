// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.QueryPlanRetriever
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal static class QueryPlanRetriever
  {
    private static readonly QueryFeatures SupportedQueryFeatures = QueryFeatures.Aggregate | QueryFeatures.Distinct | QueryFeatures.GroupBy | QueryFeatures.MultipleAggregates | QueryFeatures.MultipleOrderBy | QueryFeatures.OffsetAndLimit | QueryFeatures.OrderBy | QueryFeatures.Top | QueryFeatures.NonValueAggregate;
    private static readonly string SupportedQueryFeaturesString = QueryPlanRetriever.SupportedQueryFeatures.ToString();
    private static readonly Version Version = new Version(1, 0);
    private static readonly string VersionString = QueryPlanRetriever.Version.ToString();

    public static PartitionedQueryExecutionInfo GetQueryPlanWithServiceInterop(
      QueryPartitionProvider queryPartitionProvider,
      SqlQuerySpec sqlQuerySpec,
      PartitionKeyDefinition partitionKeyDefinition,
      bool hasLogicalPartitionKey)
    {
      if (queryPartitionProvider == null)
        throw new ArgumentNullException(nameof (queryPartitionProvider));
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      return new QueryPlanHandler(queryPartitionProvider).GetQueryPlan(sqlQuerySpec, partitionKeyDefinition, hasLogicalPartitionKey, QueryPlanRetriever.SupportedQueryFeatures);
    }

    public static async Task<PartitionedQueryExecutionInfo> GetQueryPlanThroughGatewayAsync(
      IDocumentQueryClient documentQueryClient,
      DocumentCollection documentCollection,
      SqlQuerySpec sqlQuerySpec,
      PartitionKeyInfo partitionKeyInfo,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return QueryPlanRetriever.GetQueryPlanFromResponse(await documentQueryClient.ExecuteQueryAsync(QueryPlanRetriever.CreateQueryPlanRequest(documentCollection, sqlQuerySpec, partitionKeyInfo), (IDocumentClientRetryPolicy) null, cancellationToken));
    }

    private static DocumentServiceRequest CreateQueryPlanRequest(
      DocumentCollection documentCollection,
      SqlQuerySpec sqlQuerySpec,
      PartitionKeyInfo partitionKeyInfo)
    {
      DictionaryNameValueCollection headers = new DictionaryNameValueCollection()
      {
        {
          "x-ms-cosmos-is-query-plan-request",
          bool.TrueString
        },
        {
          "Content-Type",
          "application/query+json"
        },
        {
          "x-ms-cosmos-supported-query-features",
          QueryPlanRetriever.SupportedQueryFeaturesString
        },
        {
          "x-ms-cosmos-query-version",
          QueryPlanRetriever.VersionString
        }
      };
      if (partitionKeyInfo != null)
      {
        switch (partitionKeyInfo)
        {
          case LogicalPartitionKey logicalPartitionKey:
            headers["x-ms-documentdb-partitionkey"] = logicalPartitionKey.Value.ToString();
            break;
          case PhysicalPartitionKeyRangeId partitionKeyRangeId:
            headers["x-ms-documentdb-partitionkeyrangeid"] = partitionKeyRangeId.Value;
            break;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0} type: {1}.", (object) "PartitionKeyInfo", (object) partitionKeyInfo.GetType()));
        }
      }
      DocumentServiceRequest queryPlanRequest = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.QueryPlan, documentCollection.ResourceId, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, (INameValueCollection) headers);
      queryPlanRequest.Body = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) sqlQuerySpec)));
      queryPlanRequest.UseGatewayMode = true;
      return queryPlanRequest;
    }

    private static PartitionedQueryExecutionInfo GetQueryPlanFromResponse(
      DocumentServiceResponse documentServiceResponse)
    {
      MemoryStream memoryStream = new MemoryStream();
      documentServiceResponse.ResponseBody.CopyTo((Stream) memoryStream);
      return JsonConvert.DeserializeObject<PartitionedQueryExecutionInfo>(Encoding.UTF8.GetString(CustomTypeExtensions.GetBuffer(memoryStream)));
    }
  }
}
