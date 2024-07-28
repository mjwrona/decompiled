// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.QueryInfo
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Query
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
    public SortOrder[] OrderBy { get; set; }

    [JsonProperty("orderByExpressions")]
    public string[] OrderByExpressions { get; set; }

    [JsonProperty("groupByExpressions")]
    public string[] GroupByExpressions { get; set; }

    [JsonProperty("groupByAliases")]
    public string[] GroupByAliases { get; set; }

    [JsonProperty("aggregates", ItemConverterType = typeof (StringEnumConverter))]
    public AggregateOperator[] Aggregates { get; set; }

    [JsonProperty("groupByAliasToAggregateType", ItemConverterType = typeof (StringEnumConverter))]
    public Dictionary<string, AggregateOperator?> GroupByAliasToAggregateType { get; set; }

    [JsonProperty("rewrittenQuery")]
    public string RewrittenQuery { get; set; }

    [JsonProperty("hasSelectValue")]
    public bool HasSelectValue { get; set; }

    public bool HasDistinct => this.DistinctType != 0;

    public bool HasTop => this.Top.HasValue;

    public bool HasAggregates => ((this.Aggregates == null ? 0 : (this.Aggregates.Length != 0 ? 1 : 0)) | (this.GroupByAliasToAggregateType == null ? (false ? 1 : 0) : (this.GroupByAliasToAggregateType.Values.Any<AggregateOperator?>((Func<AggregateOperator?, bool>) (aggregateOperator => aggregateOperator.HasValue)) ? 1 : 0))) != 0;

    public bool HasGroupBy => this.GroupByExpressions != null && this.GroupByExpressions.Length != 0;

    public bool HasOrderBy => this.OrderBy != null && this.OrderBy.Length != 0;

    public bool HasOffset => this.Offset.HasValue;

    public bool HasLimit => this.Limit.HasValue;
  }
}
