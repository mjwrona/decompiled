// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Cosmos.ReadFeed.Pagination
{
  internal sealed class ReadFeedPage : Page<ReadFeedState>
  {
    public static readonly ImmutableHashSet<string> BannedHeaders = new HashSet<string>()
    {
      "x-ms-continuation",
      "x-ms-continuationtoken"
    }.Concat<string>((IEnumerable<string>) Page<ReadFeedState>.BannedHeadersBase).ToImmutableHashSet<string>();

    public ReadFeedPage(
      Stream content,
      double requestCharge,
      string activityId,
      IReadOnlyDictionary<string, string> additionalHeaders,
      ReadFeedState state)
      : base(requestCharge, activityId, additionalHeaders, state)
    {
      this.Content = content ?? throw new ArgumentNullException(nameof (content));
    }

    public Stream Content { get; }

    protected override ImmutableHashSet<string> DerivedClassBannedHeaders => ReadFeedPage.BannedHeaders;
  }
}
