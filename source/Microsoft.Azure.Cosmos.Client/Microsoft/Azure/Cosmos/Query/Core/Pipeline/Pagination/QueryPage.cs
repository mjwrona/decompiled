// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination.QueryPage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination
{
  internal sealed class QueryPage : Page<QueryState>
  {
    public static readonly ImmutableHashSet<string> BannedHeaders = new HashSet<string>()
    {
      "x-ms-continuation",
      "x-ms-continuationtoken"
    }.Concat<string>((IEnumerable<string>) Page<QueryState>.BannedHeadersBase).ToImmutableHashSet<string>();

    public QueryPage(
      IReadOnlyList<CosmosElement> documents,
      double requestCharge,
      string activityId,
      long responseLengthInBytes,
      Lazy<Microsoft.Azure.Cosmos.Query.Core.QueryClient.CosmosQueryExecutionInfo> cosmosQueryExecutionInfo,
      string disallowContinuationTokenMessage,
      IReadOnlyDictionary<string, string> additionalHeaders,
      QueryState state)
      : base(requestCharge, activityId, additionalHeaders, state)
    {
      this.Documents = documents ?? throw new ArgumentNullException(nameof (documents));
      this.ResponseLengthInBytes = responseLengthInBytes >= 0L ? responseLengthInBytes : throw new ArgumentOutOfRangeException(nameof (responseLengthInBytes));
      this.CosmosQueryExecutionInfo = cosmosQueryExecutionInfo;
      this.DisallowContinuationTokenMessage = disallowContinuationTokenMessage;
    }

    public IReadOnlyList<CosmosElement> Documents { get; }

    public long ResponseLengthInBytes { get; }

    public Lazy<Microsoft.Azure.Cosmos.Query.Core.QueryClient.CosmosQueryExecutionInfo> CosmosQueryExecutionInfo { get; }

    public string DisallowContinuationTokenMessage { get; }

    protected override ImmutableHashSet<string> DerivedClassBannedHeaders => QueryPage.BannedHeaders;
  }
}
