// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedNotModifiedPage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class ChangeFeedNotModifiedPage : ChangeFeedPage
  {
    private static readonly ImmutableHashSet<string> bannedHeaders = new HashSet<string>().ToImmutableHashSet<string>();

    public ChangeFeedNotModifiedPage(
      double requestCharge,
      string activityId,
      IReadOnlyDictionary<string, string> additionalHeaders,
      ChangeFeedState state)
      : base(requestCharge, activityId, additionalHeaders, state)
    {
    }

    protected override ImmutableHashSet<string> DerivedClassBannedHeaders => ChangeFeedNotModifiedPage.bannedHeaders;
  }
}
