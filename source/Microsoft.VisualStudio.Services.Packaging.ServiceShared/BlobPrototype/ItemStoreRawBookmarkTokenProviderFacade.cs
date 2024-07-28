// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ItemStoreRawBookmarkTokenProviderFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ItemStoreRawBookmarkTokenProviderFacade : IRawBookmarkTokenProvider
  {
    private readonly ILegacyBookmarkTokenProvider tokenProviderWithRequestContext;
    private readonly IVssRequestContext requestContext;
    private readonly BookmarkTokenKey bookmarkTokenKey;

    public ItemStoreRawBookmarkTokenProviderFacade(
      IVssRequestContext requestContext,
      ILegacyBookmarkTokenProvider tokenProviderWithRequestContext,
      BookmarkTokenKey bookmarkTokenKey)
    {
      this.tokenProviderWithRequestContext = tokenProviderWithRequestContext;
      this.bookmarkTokenKey = bookmarkTokenKey;
      this.requestContext = requestContext;
    }

    public string? GetToken(Guid containerId) => this.tokenProviderWithRequestContext.GetToken(this.requestContext, containerId, this.bookmarkTokenKey);

    public void StoreToken(Guid containerId, string token) => this.tokenProviderWithRequestContext.StoreToken(this.requestContext, containerId, token, this.bookmarkTokenKey);

    public string GetOrStoreToken(Guid containerId, string token) => this.tokenProviderWithRequestContext.GetOrStoreToken(this.requestContext, containerId, token, this.bookmarkTokenKey);
  }
}
