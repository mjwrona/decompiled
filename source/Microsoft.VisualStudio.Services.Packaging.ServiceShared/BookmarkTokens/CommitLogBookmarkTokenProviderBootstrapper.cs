// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.CommitLogBookmarkTokenProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  public class CommitLogBookmarkTokenProviderBootstrapper : 
    IBootstrapper<IBookmarkTokenProvider<FeedCore, CommitLogBookmark>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly BookmarkTokenKey bookmarkTokenKey;

    public CommitLogBookmarkTokenProviderBootstrapper(
      IVssRequestContext requestContext,
      BookmarkTokenKey bookmarkTokenKey)
    {
      this.requestContext = requestContext;
      this.bookmarkTokenKey = bookmarkTokenKey;
    }

    public IBookmarkTokenProvider<FeedCore, CommitLogBookmark> Bootstrap() => (IBookmarkTokenProvider<FeedCore, CommitLogBookmark>) new CommitLogBookmarkTokenProvider((IRawBookmarkTokenProvider) new ItemStoreRawBookmarkTokenProviderFacade(this.requestContext, (ILegacyBookmarkTokenProvider) new ItemStoreBookmarkTokenProvider(), this.bookmarkTokenKey));
  }
}
