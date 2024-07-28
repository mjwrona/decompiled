// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
{
  internal sealed class PartitionedQueryExecutionInfo
  {
    public PartitionedQueryExecutionInfo() => this.Version = 2;

    [JsonProperty("partitionedQueryExecutionInfoVersion")]
    public int Version { get; private set; }

    [JsonProperty("queryInfo")]
    public QueryInfo QueryInfo { get; set; }

    [JsonProperty("queryRanges")]
    public List<Range<string>> QueryRanges { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public static bool TryParse(
      string serializedQueryPlan,
      out PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (serializedQueryPlan == null)
        throw new ArgumentNullException(nameof (serializedQueryPlan));
      try
      {
        partitionedQueryExecutionInfo = JsonConvert.DeserializeObject<PartitionedQueryExecutionInfo>(serializedQueryPlan);
        return true;
      }
      catch (JsonException ex)
      {
        partitionedQueryExecutionInfo = (PartitionedQueryExecutionInfo) null;
        return false;
      }
    }
  }
}
