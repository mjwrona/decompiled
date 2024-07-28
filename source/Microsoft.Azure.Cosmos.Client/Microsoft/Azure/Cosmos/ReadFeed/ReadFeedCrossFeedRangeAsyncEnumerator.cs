// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.ReadFeedCrossFeedRangeAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ReadFeed
{
  internal sealed class ReadFeedCrossFeedRangeAsyncEnumerator : 
    IAsyncEnumerator<TryCatch<ReadFeedPage>>,
    IAsyncDisposable
  {
    private readonly CrossPartitionReadFeedAsyncEnumerator enumerator;

    public ReadFeedCrossFeedRangeAsyncEnumerator(CrossPartitionReadFeedAsyncEnumerator enumerator) => this.enumerator = enumerator ?? throw new ArgumentNullException(nameof (enumerator));

    public TryCatch<ReadFeedPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.enumerator.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync()
    {
      if (!await this.enumerator.MoveNextAsync())
        return false;
      TryCatch<CrossFeedRangePage<Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage, ReadFeedState>> current = this.enumerator.Current;
      if (current.Failed)
      {
        this.Current = TryCatch<ReadFeedPage>.FromException(current.Exception);
        return true;
      }
      CrossFeedRangePage<Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage, ReadFeedState> result = current.Result;
      CrossFeedRangeState<ReadFeedState> state1 = result.State;
      ReadFeedCrossFeedRangeState? state2 = state1 != null ? new ReadFeedCrossFeedRangeState?(new ReadFeedCrossFeedRangeState(state1.Value)) : new ReadFeedCrossFeedRangeState?();
      this.Current = TryCatch<ReadFeedPage>.FromResult(new ReadFeedPage(CosmosQueryClientCore.ParseElementsFromRestStream(result.Page.Content, ResourceType.Document, (CosmosSerializationFormatOptions) null), result.Page.RequestCharge, result.Page.ActivityId, state2, result.Page.AdditionalHeaders));
      return true;
    }
  }
}
