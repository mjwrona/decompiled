// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.CommitLogBookmarkTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Newtonsoft.Json;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  public class CommitLogBookmarkTokenProvider : IBookmarkTokenProvider<FeedCore, CommitLogBookmark>
  {
    private readonly IRawBookmarkTokenProvider untypedProvider;

    public CommitLogBookmarkTokenProvider(IRawBookmarkTokenProvider untypedProvider) => this.untypedProvider = untypedProvider;

    public CommitLogBookmark GetToken(FeedCore feed) => CommitLogBookmarkTokenProvider.StringToToken(this.untypedProvider.GetToken(feed.Id));

    public void StoreToken(FeedCore feed, CommitLogBookmark token) => this.untypedProvider.StoreToken(feed.Id, CommitLogBookmarkTokenProvider.TokenToString(token));

    public CommitLogBookmark GetOrStoreToken(FeedCore feed, CommitLogBookmark token) => CommitLogBookmarkTokenProvider.StringToToken(this.untypedProvider.GetOrStoreToken(feed.Id, CommitLogBookmarkTokenProvider.TokenToString(token)));

    private static CommitLogBookmark StringToToken(string? value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return CommitLogBookmark.Empty;
      return value.StartsWith("{", StringComparison.Ordinal) ? JsonConvert.DeserializeObject<CommitLogBookmark>(value) : new CommitLogBookmark(PackagingCommitId.Parse(value), new long?());
    }

    private static string TokenToString(CommitLogBookmark token) => JsonConvert.SerializeObject((object) token);
  }
}
