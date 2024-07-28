// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ServiceInteropQueryPlanHandler
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class ServiceInteropQueryPlanHandler
  {
    private readonly QueryPlanHandler queryPlanHandler;
    private readonly QueryFeatures supportedQueryFeatures;

    public ServiceInteropQueryPlanHandler(
      QueryPartitionProvider queryPartitionProvider,
      QueryFeatures supportedQueryFeatures)
    {
      this.queryPlanHandler = new QueryPlanHandler(queryPartitionProvider);
      this.supportedQueryFeatures = supportedQueryFeatures;
    }

    public PartitionedQueryExecutionInfo GetPlanForQuery(
      SqlQuerySpec sqlQuerySpec,
      PartitionKeyDefinition partitionKeyDefinition,
      PartitionKey partitionKey)
    {
      return this.queryPlanHandler.GetQueryPlan(sqlQuerySpec, partitionKeyDefinition, partitionKey != null, this.supportedQueryFeatures);
    }
  }
}
