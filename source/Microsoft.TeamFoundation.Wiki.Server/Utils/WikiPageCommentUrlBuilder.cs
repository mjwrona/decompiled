// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Utils.WikiPageCommentUrlBuilder
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Wiki.Server.Utils
{
  public class WikiPageCommentUrlBuilder
  {
    public const string CommentPlaceHolder = "3E5FD532-3C38-46A8-BFB6-C39F42053F9A";
    private const string UniqueString = "3E5FD532-3C38-46A8-BFB6-C39F42053F9A";
    private string wikiPageCommentGenericResourceUrl;

    public WikiPageCommentUrlBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      int pageId)
    {
      this.wikiPageCommentGenericResourceUrl = WikiUrlHelper.GetWikiPageCommentResourceUrl(requestContext, projectId.ToString(), wikiId.ToString(), pageId.ToString(), "3E5FD532-3C38-46A8-BFB6-C39F42053F9A");
    }

    public string getPageCommentResourceUrl(int commentId) => this.wikiPageCommentGenericResourceUrl.Replace("3E5FD532-3C38-46A8-BFB6-C39F42053F9A", Uri.EscapeDataString(commentId.ToString()));
  }
}
