// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.PartitionedQueryExecutionInfo
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Query
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
  }
}
