// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PypI.Server.PublicRepositories.PyPiChangelogBookmarkTokenProvider
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.PypI.Server.PublicRepositories
{
  public class PyPiChangelogBookmarkTokenProvider : 
    IBookmarkTokenProvider<NullRequest, PyPiChangelogCursor?>
  {
    private readonly IRawBookmarkTokenProvider rawProvider;

    public PyPiChangelogBookmarkTokenProvider(IRawBookmarkTokenProvider rawProvider) => this.rawProvider = rawProvider;

    public PyPiChangelogCursor? GetToken(NullRequest containerId) => PyPiChangelogBookmarkTokenProvider.StringToToken(this.rawProvider.GetToken(Guid.Empty));

    public void StoreToken(NullRequest containerId, PyPiChangelogCursor? token) => this.rawProvider.StoreToken(Guid.Empty, PyPiChangelogBookmarkTokenProvider.TokenToString(token));

    public PyPiChangelogCursor? GetOrStoreToken(NullRequest containerId, PyPiChangelogCursor? token) => PyPiChangelogBookmarkTokenProvider.StringToToken(this.rawProvider.GetOrStoreToken(Guid.Empty, PyPiChangelogBookmarkTokenProvider.TokenToString(token)));

    private static string TokenToString(PyPiChangelogCursor? token) => token?.SinceSerial.ToString() ?? string.Empty;

    private static PyPiChangelogCursor? StringToToken(string? str) => !string.IsNullOrWhiteSpace(str) ? PyPiChangelogCursor.Parse(str) : (PyPiChangelogCursor) null;

    public static PyPiChangelogBookmarkTokenProvider Bootstrap(
      IVssRequestContext requestContext,
      BookmarkTokenKey bookmarkTokenKey)
    {
      return new PyPiChangelogBookmarkTokenProvider((IRawBookmarkTokenProvider) new ItemStoreRawBookmarkTokenProviderFacade(requestContext, (ILegacyBookmarkTokenProvider) new ItemStoreBookmarkTokenProvider(), bookmarkTokenKey));
    }
  }
}
