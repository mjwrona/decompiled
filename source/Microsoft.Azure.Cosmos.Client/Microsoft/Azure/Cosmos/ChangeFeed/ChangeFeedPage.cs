// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedPage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedPage
  {
    private ChangeFeedPage(
      CosmosArray documents,
      bool notModified,
      double requestCharge,
      string activityId,
      ChangeFeedCrossFeedRangeState state,
      IReadOnlyDictionary<string, string> additionalHeaders)
    {
      this.Documents = documents ?? throw new ArgumentOutOfRangeException(nameof (documents));
      this.NotModified = notModified;
      this.RequestCharge = requestCharge >= 0.0 ? requestCharge : throw new ArgumentOutOfRangeException(nameof (requestCharge));
      this.ActivityId = activityId ?? throw new ArgumentNullException(nameof (activityId));
      this.State = state;
      this.AdditionalHeaders = additionalHeaders;
    }

    public CosmosArray Documents { get; }

    public bool NotModified { get; }

    public double RequestCharge { get; }

    public string ActivityId { get; }

    public ChangeFeedCrossFeedRangeState State { get; }

    public IReadOnlyDictionary<string, string> AdditionalHeaders { get; }

    public static ChangeFeedPage CreateNotModifiedPage(
      double requestCharge,
      string activityId,
      ChangeFeedCrossFeedRangeState state,
      IReadOnlyDictionary<string, string> additionalHeaders)
    {
      return new ChangeFeedPage(CosmosArray.Empty, true, requestCharge, activityId, state, additionalHeaders);
    }

    public static ChangeFeedPage CreatePageWithChanges(
      CosmosArray documents,
      double requestCharge,
      string activityId,
      ChangeFeedCrossFeedRangeState state,
      IReadOnlyDictionary<string, string> additionalHeaders)
    {
      return new ChangeFeedPage(documents, false, requestCharge, activityId, state, additionalHeaders);
    }
  }
}
