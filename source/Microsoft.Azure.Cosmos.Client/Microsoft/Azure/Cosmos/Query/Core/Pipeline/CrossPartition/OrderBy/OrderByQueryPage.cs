// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByQueryPage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class OrderByQueryPage : Microsoft.Azure.Cosmos.Pagination.Page<QueryState>
  {
    private static readonly ImmutableHashSet<string> bannedHeaders = new HashSet<string>()
    {
      "x-ms-continuation",
      "x-ms-continuationtoken"
    }.ToImmutableHashSet<string>();

    public OrderByQueryPage(QueryPage queryPage)
      : base(queryPage.RequestCharge, queryPage.ActivityId, queryPage.AdditionalHeaders, queryPage.State)
    {
      this.Page = queryPage ?? throw new ArgumentNullException(nameof (queryPage));
      this.Enumerator = queryPage.Documents.GetEnumerator();
    }

    public QueryPage Page { get; }

    public IEnumerator<CosmosElement> Enumerator { get; }

    protected override ImmutableHashSet<string> DerivedClassBannedHeaders => OrderByQueryPage.bannedHeaders;
  }
}
