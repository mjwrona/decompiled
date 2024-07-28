// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.DynamicTableEntityExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class DynamicTableEntityExtensions
  {
    public static string GetCosmosTableName(this TableOperation operation) => ((DynamicTableEntity) operation.Entity).Properties["TableName"].StringValue;

    public static string GetCosmosTableName(this DynamicTableEntity tblEntity) => tblEntity.Properties["TableName"].StringValue;

    public static void SetCosmosTableName(this DynamicTableEntity tblEntity, string value) => tblEntity.Properties.Add("TableName", new EntityProperty(value));

    public static int? GetCosmosTableThroughput(this DynamicTableEntity tblEntity)
    {
      int? cosmosTableThroughput = new int?();
      if (tblEntity.Properties.ContainsKey(TableConstants.Throughput))
        cosmosTableThroughput = tblEntity.Properties[TableConstants.Throughput].Int32Value;
      return cosmosTableThroughput;
    }

    public static void SetCosmosTableThroughput(this DynamicTableEntity tblEntity, int? throughput)
    {
      if (!throughput.HasValue)
        return;
      tblEntity.Properties.Add(TableConstants.Throughput, new EntityProperty(new int?(throughput.Value)));
    }

    public static void SetCosmosTableIndexingPolicy(
      this DynamicTableEntity tblEntity,
      string serializedIndexingPolicy)
    {
      if (string.IsNullOrEmpty(serializedIndexingPolicy))
        return;
      JsonConvert.DeserializeObject<IndexingPolicy>(serializedIndexingPolicy);
      tblEntity.Properties.Add(TableConstants.IndexingPolicy, new EntityProperty(serializedIndexingPolicy));
    }

    public static IndexingPolicy GetCosmosTableIndexingPolicy(this DynamicTableEntity tblEntity) => tblEntity.Properties.ContainsKey(TableConstants.IndexingPolicy) ? JsonConvert.DeserializeObject<IndexingPolicy>(tblEntity.Properties[TableConstants.IndexingPolicy].StringValue) : (IndexingPolicy) null;
  }
}
