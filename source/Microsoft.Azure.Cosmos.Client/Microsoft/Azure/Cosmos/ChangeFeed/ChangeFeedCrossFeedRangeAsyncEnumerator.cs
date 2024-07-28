// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedCrossFeedRangeAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Serializer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedCrossFeedRangeAsyncEnumerator : 
    IAsyncEnumerator<TryCatch<ChangeFeedPage>>,
    IAsyncDisposable
  {
    private readonly CrossPartitionChangeFeedAsyncEnumerator enumerator;
    private readonly JsonSerializationFormatOptions jsonSerializationFormatOptions;

    public ChangeFeedCrossFeedRangeAsyncEnumerator(
      CrossPartitionChangeFeedAsyncEnumerator enumerator,
      JsonSerializationFormatOptions jsonSerializationFormatOptions)
    {
      this.enumerator = enumerator ?? throw new ArgumentNullException(nameof (enumerator));
      this.jsonSerializationFormatOptions = jsonSerializationFormatOptions;
    }

    public TryCatch<ChangeFeedPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.enumerator.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync()
    {
      TryCatch<CrossFeedRangePage<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage, ChangeFeedState>> tryCatch = await this.enumerator.MoveNextAsync() ? this.enumerator.Current : throw new InvalidOperationException("Change Feed should always be able to move next.");
      if (tryCatch.Failed)
      {
        this.Current = TryCatch<ChangeFeedPage>.FromException(tryCatch.Exception);
        return true;
      }
      CrossFeedRangePage<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage, ChangeFeedState> result1 = tryCatch.Result;
      ChangeFeedCrossFeedRangeState state = new ChangeFeedCrossFeedRangeState(result1.State.Value);
      ChangeFeedPage result2;
      switch (result1.Page)
      {
        case ChangeFeedSuccessPage changeFeedSuccessPage:
          result2 = ChangeFeedPage.CreatePageWithChanges(RestFeedResponseParser.ParseRestFeedResponse(changeFeedSuccessPage.Content, this.jsonSerializationFormatOptions), changeFeedSuccessPage.RequestCharge, changeFeedSuccessPage.ActivityId, state, changeFeedSuccessPage.AdditionalHeaders);
          break;
        case ChangeFeedNotModifiedPage feedNotModifiedPage:
          result2 = ChangeFeedPage.CreateNotModifiedPage(feedNotModifiedPage.RequestCharge, feedNotModifiedPage.ActivityId, state, feedNotModifiedPage.AdditionalHeaders);
          break;
        default:
          throw new InvalidOperationException(string.Format("Unknown type: {0}", (object) result1.Page.GetType()));
      }
      this.Current = TryCatch<ChangeFeedPage>.FromResult(result2);
      return true;
    }
  }
}
