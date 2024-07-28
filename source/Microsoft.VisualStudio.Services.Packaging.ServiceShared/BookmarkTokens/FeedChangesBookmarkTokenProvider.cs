// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.FeedChangesBookmarkTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  public class FeedChangesBookmarkTokenProvider : IBookmarkTokenProvider<CollectionId, long>
  {
    private readonly IRawBookmarkTokenProvider untypedProvider;

    public FeedChangesBookmarkTokenProvider(IRawBookmarkTokenProvider untypedProvider) => this.untypedProvider = untypedProvider;

    public long GetToken(CollectionId collectionId) => FeedChangesBookmarkTokenProvider.StringToToken(this.untypedProvider.GetToken(collectionId.Guid));

    public void StoreToken(CollectionId collectionId, long token) => this.untypedProvider.StoreToken(collectionId.Guid, FeedChangesBookmarkTokenProvider.TokenToString(token));

    public long GetOrStoreToken(CollectionId collectionId, long token) => FeedChangesBookmarkTokenProvider.StringToToken(this.untypedProvider.GetOrStoreToken(collectionId.Guid, FeedChangesBookmarkTokenProvider.TokenToString(token)));

    private static long StringToToken(string? token) => Convert.ToInt64(token);

    private static string TokenToString(long token) => Convert.ToString(token);

    public static IBookmarkTokenProvider<CollectionId, long> Bootstrap(
      IVssRequestContext requestContext,
      BookmarkTokenKey bookmarkTokenKey)
    {
      return (IBookmarkTokenProvider<CollectionId, long>) new FeedChangesBookmarkTokenProvider((IRawBookmarkTokenProvider) new ItemStoreRawBookmarkTokenProviderFacade(requestContext, (ILegacyBookmarkTokenProvider) new ItemStoreBookmarkTokenProvider(), bookmarkTokenKey));
    }
  }
}
