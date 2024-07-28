// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.QueryRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Documents;
using System;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  public class QueryRequestOptions : RequestOptions
  {
    public int? ResponseContinuationTokenLimitInKb { get; set; }

    public bool? EnableScanInQuery { get; set; }

    public bool? EnableLowPrecisionOrderBy { get; set; }

    public int? MaxBufferedItemCount { get; set; }

    public int? MaxItemCount { get; set; }

    public int? MaxConcurrency { get; set; }

    public Microsoft.Azure.Cosmos.PartitionKey? PartitionKey { get; set; }

    public bool? PopulateIndexMetrics { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel
    {
      get => this.BaseConsistencyLevel;
      set => this.BaseConsistencyLevel = value;
    }

    public string SessionToken { get; set; }

    public DedicatedGatewayRequestOptions DedicatedGatewayRequestOptions { get; set; }

    internal CosmosElement CosmosElementContinuationToken { get; set; }

    internal string StartId { get; set; }

    internal string EndId { get; set; }

    internal Microsoft.Azure.Documents.EnumerationDirection? EnumerationDirection { get; set; }

    internal CosmosSerializationFormatOptions CosmosSerializationFormatOptions { get; set; }

    internal Microsoft.Azure.Cosmos.Query.Core.Pipeline.ExecutionEnvironment? ExecutionEnvironment { get; set; }

    internal bool? ReturnResultsInDeterministicOrder { get; set; }

    internal TestInjections TestSettings { get; set; }

    internal FeedRange FeedRange { get; set; }

    internal override void PopulateRequestOptions(RequestMessage request)
    {
      if (this.PartitionKey.HasValue && request.ResourceType != ResourceType.Document)
        throw new ArgumentException("PartitionKey can only be set for item operations");
      if (!this.PartitionKey.HasValue && !this.IsEffectivePartitionKeyRouting && request.ResourceType == ResourceType.Document)
        request.Headers.Add("x-ms-documentdb-query-enablecrosspartition", bool.TrueString);
      RequestOptions.SetSessionToken(request, this.SessionToken);
      if (this.MaxItemCount.HasValue)
        request.Headers.CosmosMessageHeaders.PageSize = this.MaxItemCount.ToString();
      int? nullable;
      if (this.MaxConcurrency.HasValue)
      {
        nullable = this.MaxConcurrency;
        int num = 0;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          request.Headers.Add("x-ms-documentdb-query-parallelizecrosspartitionquery", bool.TrueString);
      }
      if (this.EnableScanInQuery.HasValue && this.EnableScanInQuery.Value)
        request.Headers.Add("x-ms-documentdb-query-enable-scan", bool.TrueString);
      if (this.EnableLowPrecisionOrderBy.HasValue)
        request.Headers.Add("x-ms-documentdb-query-enable-low-precision-order-by", this.EnableLowPrecisionOrderBy.ToString());
      nullable = this.ResponseContinuationTokenLimitInKb;
      if (nullable.HasValue)
      {
        Headers headers = request.Headers;
        nullable = this.ResponseContinuationTokenLimitInKb;
        string str = nullable.ToString();
        headers.Add("x-ms-documentdb-responsecontinuationtokenlimitinkb", str);
      }
      if (this.CosmosSerializationFormatOptions != null)
        request.Headers.CosmosMessageHeaders.ContentSerializationFormat = this.CosmosSerializationFormatOptions.ContentSerializationFormat;
      if (this.StartId != null)
        request.Headers.Set("x-ms-start-id", Convert.ToBase64String(Encoding.UTF8.GetBytes(this.StartId)));
      if (this.EndId != null)
        request.Headers.Set("x-ms-end-id", Convert.ToBase64String(Encoding.UTF8.GetBytes(this.EndId)));
      if (this.StartId != null || this.EndId != null)
        request.Headers.Set("x-ms-read-key-type", ReadFeedKeyType.ResourceId.ToString());
      if (this.EnumerationDirection.HasValue)
        request.Headers.Set("x-ms-enumeration-direction", this.EnumerationDirection.Value.ToString());
      if (this.PopulateIndexMetrics.HasValue)
        request.Headers.CosmosMessageHeaders.Add("x-ms-cosmos-populateindexmetrics", this.PopulateIndexMetrics.ToString());
      DedicatedGatewayRequestOptions.PopulateMaxIntegratedCacheStalenessOption(this.DedicatedGatewayRequestOptions, request);
      request.Headers.Add("x-ms-documentdb-populatequerymetrics", bool.TrueString);
      base.PopulateRequestOptions(request);
    }

    internal static void FillContinuationToken(RequestMessage request, string continuationToken)
    {
      if (string.IsNullOrWhiteSpace(continuationToken))
        return;
      request.Headers.ContinuationToken = continuationToken;
    }
  }
}
