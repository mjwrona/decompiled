// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPaginationOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class ChangeFeedPaginationOptions : PaginationOptions
  {
    public static readonly ChangeFeedPaginationOptions Default = new ChangeFeedPaginationOptions(ChangeFeedMode.Incremental);
    public static readonly ImmutableHashSet<string> BannedHeaders = new HashSet<string>()
    {
      "A-IM",
      "If-Modified-Since",
      "If-None-Match"
    }.Concat<string>((IEnumerable<string>) PaginationOptions.bannedAdditionalHeaders).ToImmutableHashSet<string>();

    public ChangeFeedPaginationOptions(
      ChangeFeedMode mode,
      int? pageSizeHint = null,
      Microsoft.Azure.Cosmos.Json.JsonSerializationFormat? jsonSerializationFormat = null,
      Dictionary<string, string> additionalHeaders = null,
      ChangeFeedQuerySpec changeFeedQuerySpec = null)
      : base(pageSizeHint, jsonSerializationFormat, additionalHeaders)
    {
      this.Mode = mode ?? throw new ArgumentNullException(nameof (mode));
      this.ChangeFeedQuerySpec = changeFeedQuerySpec;
    }

    public ChangeFeedMode Mode { get; }

    public ChangeFeedQuerySpec ChangeFeedQuerySpec { get; }

    protected override ImmutableHashSet<string> BannedAdditionalHeaders => ChangeFeedPaginationOptions.BannedHeaders;
  }
}
