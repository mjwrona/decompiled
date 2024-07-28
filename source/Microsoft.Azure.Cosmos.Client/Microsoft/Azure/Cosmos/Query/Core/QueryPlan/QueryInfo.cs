// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
{
  [JsonObject(MemberSerialization.OptIn)]
  internal sealed class QueryInfo
  {
    [JsonProperty("distinctType")]
    [JsonConverter(typeof (StringEnumConverter))]
    public DistinctQueryType DistinctType { get; set; }

    [JsonProperty("top")]
    public int? Top { get; set; }

    [JsonProperty("offset")]
    public int? Offset { get; set; }

    [JsonProperty("limit")]
    public int? Limit { get; set; }

    [JsonProperty("orderBy", ItemConverterType = typeof (StringEnumConverter))]
    public IReadOnlyList<SortOrder> OrderBy { get; set; }

    [JsonProperty("orderByExpressions")]
    public IReadOnlyList<string> OrderByExpressions { get; set; }

    [JsonProperty("groupByExpressions")]
    public IReadOnlyList<string> GroupByExpressions { get; set; }

    [JsonProperty("groupByAliases")]
    public IReadOnlyList<string> GroupByAliases { get; set; }

    [JsonProperty("aggregates", ItemConverterType = typeof (StringEnumConverter))]
    public IReadOnlyList<AggregateOperator> Aggregates { get; set; }

    [JsonProperty("groupByAliasToAggregateType", ItemConverterType = typeof (StringEnumConverter))]
    public IReadOnlyDictionary<string, AggregateOperator?> GroupByAliasToAggregateType { get; set; }

    [JsonProperty("rewrittenQuery")]
    public string RewrittenQuery { get; set; }

    [JsonProperty("hasSelectValue")]
    public bool HasSelectValue { get; set; }

    [JsonProperty("dCountInfo")]
    public DCountInfo DCountInfo { get; set; }

    public bool HasDCount => this.DCountInfo != null;

    public bool HasDistinct => this.DistinctType != 0;

    public bool HasTop => this.Top.HasValue;

    public bool HasAggregates
    {
      get
      {
        if ((this.Aggregates == null ? 0 : (this.Aggregates.Count > 0 ? 1 : 0)) != 0)
          return true;
        return this.GroupByAliasToAggregateType != null && this.GroupByAliasToAggregateType.Values.Any<AggregateOperator?>((Func<AggregateOperator?, bool>) (aggregateOperator => aggregateOperator.HasValue));
      }
    }

    public bool HasGroupBy => this.GroupByExpressions != null && this.GroupByExpressions.Count > 0;

    public bool HasOrderBy => this.OrderBy != null && this.OrderBy.Count > 0;

    public bool HasOffset => this.Offset.HasValue;

    public bool HasLimit => this.Limit.HasValue;
  }
}
