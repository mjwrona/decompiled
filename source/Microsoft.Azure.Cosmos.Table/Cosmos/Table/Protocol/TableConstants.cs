// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Protocol.TableConstants
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table.Protocol
{
  public static class TableConstants
  {
    public const int TableServiceBatchMaximumOperations = 100;
    public const string TableServicePrefixForTableContinuation = "x-ms-continuation-";
    public const string TableServiceNextPartitionKey = "NextPartitionKey";
    public const string TableServiceNextRowKey = "NextRowKey";
    public const string TableServiceNextTableName = "NextTableName";
    public const int TableServiceMaxResults = 1000;
    public const int TableServiceMaxStringPropertySizeInBytes = 65536;
    public const long TableServiceMaxPayload = 20971520;
    public const int TableServiceMaxStringPropertySizeInChars = 32768;
    public const string TableServiceTablesName = "Tables";
    public const string PartitionKey = "PartitionKey";
    public const string RowKey = "RowKey";
    public const string Timestamp = "Timestamp";
    public const string Etag = "ETag";
    public const string TableName = "TableName";
    public const string Filter = "$filter";
    public const string Top = "$top";
    public const string Select = "$select";
    public const string UserAgentProductVersion = "1.0.7";
    public static readonly DateTimeOffset MinDateTime = new DateTimeOffset(1601, 1, 1, 0, 0, 0, TimeSpan.Zero);
    internal static string Throughput = "CosmosDbTableThroughput";
    internal static string IndexingPolicy = "CosmosDbTableIndexingPolicy";
  }
}
