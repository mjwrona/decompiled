// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.FeedOptions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class FeedOptions
  {
    public FeedOptions()
    {
    }

    internal FeedOptions(FeedOptions options)
    {
      this.MaxItemCount = options != null ? options.MaxItemCount : throw new ArgumentNullException(nameof (options));
      this.RequestContinuation = options.RequestContinuation;
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
      this.PartitionKey = options.PartitionKey != null ? PartitionKey.FromInternalKey(options.PartitionKey.InternalKey) : (PartitionKey) null;
      this.EmitVerboseTracesInQuery = options.EmitVerboseTracesInQuery;
      this.FilterBySchemaResourceId = options.FilterBySchemaResourceId;
      this.RequestContinuation = options.RequestContinuation;
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
      this.MergeStaticId = options.MergeStaticId;
    }

    public int? MaxItemCount { get; set; }

    public string RequestContinuation { get; set; }

    public string SessionToken { get; set; }

    public bool? EnableScanInQuery { get; set; }

    public bool EnableCrossPartitionQuery { get; set; }

    public bool? EnableLowPrecisionOrderBy { get; set; }

    public PartitionKey PartitionKey { get; set; }

    public string PartitionKeyRangeId { get; set; }

    public int MaxDegreeOfParallelism { get; set; }

    public int MaxBufferedItemCount { get; set; }

    internal bool? EmitVerboseTracesInQuery { get; set; }

    internal string FilterBySchemaResourceId { get; set; }

    public bool PopulateQueryMetrics { get; set; }

    public int? ResponseContinuationTokenLimitInKb { get; set; }

    public bool DisableRUPerMinuteUsage { get; set; }

    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    public Microsoft.Azure.Documents.ConsistencyLevel? ConsistencyLevel { get; set; }

    internal bool ForceQueryScan { get; set; }

    internal Microsoft.Azure.Documents.EnumerationDirection? EnumerationDirection { get; set; }

    internal Microsoft.Azure.Documents.ReadFeedKeyType? ReadFeedKeyType { get; set; }

    internal string StartId { get; set; }

    internal string EndId { get; set; }

    internal string StartEpk { get; set; }

    internal string EndEpk { get; set; }

    internal Microsoft.Azure.Documents.ContentSerializationFormat? ContentSerializationFormat { get; set; }

    internal string MergeStaticId { get; set; }
  }
}
