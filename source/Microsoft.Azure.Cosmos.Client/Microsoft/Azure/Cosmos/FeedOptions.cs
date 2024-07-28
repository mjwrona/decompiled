// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedOptions
  {
    public FeedOptions()
    {
    }

    internal FeedOptions(FeedOptions options)
    {
      this.MaxItemCount = options != null ? options.MaxItemCount : throw new ArgumentNullException(nameof (options));
      this.RequestContinuationToken = options.RequestContinuationToken;
      this.SessionToken = options.SessionToken;
      this.EnableScanInQuery = options.EnableScanInQuery;
      this.EnableCrossPartitionQuery = options.EnableCrossPartitionQuery;
      this.EnableLowPrecisionOrderBy = options.EnableLowPrecisionOrderBy;
      this.MaxBufferedItemCount = options.MaxBufferedItemCount;
      this.MaxDegreeOfParallelism = options.MaxDegreeOfParallelism;
      this.PartitionKeyRangeId = options.PartitionKeyRangeId;
      this.PopulateQueryMetrics = options.PopulateQueryMetrics;
      this.ResponseContinuationTokenLimitInKb = options.ResponseContinuationTokenLimitInKb;
      this.DisableRUPerMinuteUsage = options.DisableRUPerMinuteUsage;
      this.PartitionKey = options.PartitionKey != null ? Microsoft.Azure.Documents.PartitionKey.FromInternalKey(options.PartitionKey.InternalKey) : (Microsoft.Azure.Documents.PartitionKey) null;
      this.EmitVerboseTracesInQuery = options.EmitVerboseTracesInQuery;
      this.FilterBySchemaResourceId = options.FilterBySchemaResourceId;
      this.RequestContinuationToken = options.RequestContinuationToken;
      this.ConsistencyLevel = options.ConsistencyLevel;
      this.JsonSerializerSettings = options.JsonSerializerSettings;
      this.ForceQueryScan = options.ForceQueryScan;
      this.EnumerationDirection = options.EnumerationDirection;
      this.ReadFeedKeyType = options.ReadFeedKeyType;
      this.StartId = options.StartId;
      this.EndId = options.EndId;
      this.StartEpk = options.StartEpk;
      this.EndEpk = options.EndEpk;
      this.ContentSerializationFormat = options.ContentSerializationFormat;
      this.EnableGroupBy = options.EnableGroupBy;
      this.MergeStaticId = options.MergeStaticId;
      this.Properties = options.Properties;
    }

    public int? MaxItemCount { get; set; }

    public string RequestContinuationToken { get; set; }

    public string SessionToken { get; set; }

    public bool? EnableScanInQuery { get; set; }

    public bool EnableCrossPartitionQuery { get; set; }

    public bool? EnableLowPrecisionOrderBy { get; set; }

    public Microsoft.Azure.Documents.PartitionKey PartitionKey { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public int MaxDegreeOfParallelism { get; set; }

    public int MaxBufferedItemCount { get; set; }

    internal bool? EmitVerboseTracesInQuery { get; set; }

    internal string FilterBySchemaResourceId { get; set; }

    public bool PopulateQueryMetrics { get; set; }

    public int? ResponseContinuationTokenLimitInKb { get; set; }

    public bool DisableRUPerMinuteUsage { get; set; }

    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel { get; set; }

    internal bool ForceQueryScan { get; set; }

    internal Microsoft.Azure.Documents.EnumerationDirection? EnumerationDirection { get; set; }

    internal Microsoft.Azure.Cosmos.ReadFeedKeyType? ReadFeedKeyType { get; set; }

    internal string StartId { get; set; }

    internal string EndId { get; set; }

    internal string StartEpk { get; set; }

    internal string EndEpk { get; set; }

    internal Microsoft.Azure.Documents.ContentSerializationFormat? ContentSerializationFormat { get; set; }

    internal bool EnableGroupBy { get; set; }

    internal string MergeStaticId { get; set; }

    internal CosmosSerializationFormatOptions CosmosSerializationFormatOptions { get; set; }

    internal IDictionary<string, object> Properties { get; set; }
  }
}
