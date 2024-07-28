// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.ReadFeedCrossFeedRangeAsyncEnumerable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Cosmos.ReadFeed
{
  internal sealed class ReadFeedCrossFeedRangeAsyncEnumerable : 
    IAsyncEnumerable<TryCatch<ReadFeedPage>>
  {
    private readonly IDocumentContainer documentContainer;
    private readonly ReadFeedCrossFeedRangeState state;
    private readonly ReadFeedPaginationOptions readFeedPaginationOptions;

    public ReadFeedCrossFeedRangeAsyncEnumerable(
      IDocumentContainer documentContainer,
      ReadFeedCrossFeedRangeState state,
      ReadFeedPaginationOptions readFeedPaginationOptions)
    {
      this.documentContainer = documentContainer ?? throw new ArgumentNullException(nameof (documentContainer));
      this.state = state;
      this.readFeedPaginationOptions = readFeedPaginationOptions;
    }

    public IAsyncEnumerator<TryCatch<ReadFeedPage>> GetAsyncEnumerator(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (IAsyncEnumerator<TryCatch<ReadFeedPage>>) new ReadFeedCrossFeedRangeAsyncEnumerator(CrossPartitionReadFeedAsyncEnumerator.Create(this.documentContainer, new CrossFeedRangeState<ReadFeedState>(this.state.FeedRangeStates), this.readFeedPaginationOptions, cancellationToken));
    }
  }
}
