// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination.QueryPaginationOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination
{
  internal sealed class QueryPaginationOptions : PaginationOptions
  {
    public static readonly QueryPaginationOptions Default = new QueryPaginationOptions();
    public static readonly ImmutableHashSet<string> BannedHeaders = new HashSet<string>()
    {
      "x-ms-continuation",
      "x-ms-continuationtoken",
      "x-ms-documentdb-isquery",
      "x-ms-cosmos-is-query-plan-request",
      "x-ms-documentdb-query-iscontinuationexpected",
      "Content-Type"
    }.Concat<string>((IEnumerable<string>) PaginationOptions.bannedAdditionalHeaders).ToImmutableHashSet<string>();

    public QueryPaginationOptions(
      int? pageSizeHint = null,
      Microsoft.Azure.Cosmos.Json.JsonSerializationFormat? jsonSerializationFormat = null,
      Dictionary<string, string> additionalHeaders = null)
      : base(pageSizeHint, jsonSerializationFormat, additionalHeaders)
    {
    }

    protected override ImmutableHashSet<string> BannedAdditionalHeaders => QueryPaginationOptions.BannedHeaders;
  }
}
