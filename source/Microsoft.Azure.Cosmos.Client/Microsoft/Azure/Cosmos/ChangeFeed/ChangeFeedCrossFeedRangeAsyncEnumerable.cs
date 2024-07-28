// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedCrossFeedRangeAsyncEnumerable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Serializer;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedCrossFeedRangeAsyncEnumerable : 
    IAsyncEnumerable<TryCatch<ChangeFeedPage>>
  {
    private readonly IDocumentContainer documentContainer;
    private readonly ChangeFeedPaginationOptions changeFeedPaginationOptions;
    private readonly ChangeFeedCrossFeedRangeState state;
    private readonly JsonSerializationFormatOptions jsonSerializationFormatOptions;

    public ChangeFeedCrossFeedRangeAsyncEnumerable(
      IDocumentContainer documentContainer,
      ChangeFeedCrossFeedRangeState state,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      JsonSerializationFormatOptions jsonSerializationFormatOptions = null)
    {
      this.documentContainer = documentContainer ?? throw new ArgumentNullException(nameof (documentContainer));
      this.changeFeedPaginationOptions = changeFeedPaginationOptions ?? ChangeFeedPaginationOptions.Default;
      this.state = state;
      this.jsonSerializationFormatOptions = jsonSerializationFormatOptions;
    }

    public IAsyncEnumerator<TryCatch<ChangeFeedPage>> GetAsyncEnumerator(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (IAsyncEnumerator<TryCatch<ChangeFeedPage>>) new ChangeFeedCrossFeedRangeAsyncEnumerator(CrossPartitionChangeFeedAsyncEnumerator.Create(this.documentContainer, new CrossFeedRangeState<ChangeFeedState>(this.state.FeedRangeStates), this.changeFeedPaginationOptions, cancellationToken), this.jsonSerializationFormatOptions);
    }
  }
}
