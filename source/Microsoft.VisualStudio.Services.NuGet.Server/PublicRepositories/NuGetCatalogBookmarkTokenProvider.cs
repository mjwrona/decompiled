// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetCatalogBookmarkTokenProvider
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Globalization;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public class NuGetCatalogBookmarkTokenProvider : 
    IBookmarkTokenProvider<NullRequest, NuGetCatalogCursor?>
  {
    private readonly IRawBookmarkTokenProvider rawProvider;

    public NuGetCatalogBookmarkTokenProvider(IRawBookmarkTokenProvider rawProvider) => this.rawProvider = rawProvider;

    public NuGetCatalogCursor? GetToken(NullRequest containerId) => this.StringToToken(this.rawProvider.GetToken(Guid.Empty));

    public void StoreToken(NullRequest containerId, NuGetCatalogCursor? token) => this.rawProvider.StoreToken(Guid.Empty, this.TokenToString(token));

    public NuGetCatalogCursor? GetOrStoreToken(NullRequest containerId, NuGetCatalogCursor? token) => this.StringToToken(this.rawProvider.GetOrStoreToken(Guid.Empty, this.TokenToString(token)));

    private string TokenToString(NuGetCatalogCursor? token) => token?.Value.ToString("O", (IFormatProvider) CultureInfo.InvariantCulture) ?? string.Empty;

    private NuGetCatalogCursor? StringToToken(string? str) => !string.IsNullOrWhiteSpace(str) ? new NuGetCatalogCursor(DateTimeOffset.ParseExact(str, "O", (IFormatProvider) CultureInfo.InvariantCulture)) : (NuGetCatalogCursor) null;

    public static NuGetCatalogBookmarkTokenProvider Bootstrap(
      IVssRequestContext requestContext,
      BookmarkTokenKey bookmarkTokenKey)
    {
      return new NuGetCatalogBookmarkTokenProvider((IRawBookmarkTokenProvider) new ItemStoreRawBookmarkTokenProviderFacade(requestContext, (ILegacyBookmarkTokenProvider) new ItemStoreBookmarkTokenProvider(), bookmarkTokenKey));
    }
  }
}
