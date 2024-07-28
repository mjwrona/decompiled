// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiUrlHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiUrlHelper
  {
    public static string GetWikiRemoteUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier)
    {
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      string part1 = UriUtility.CombinePath("_wiki", "wikis");
      string part2 = UriUtility.CombinePath(Uri.EscapeDataString(projectIdentifier), UriUtility.CombinePath(part1, Uri.EscapeDataString(wikiIdentifier)));
      return UriUtility.CombinePath(publicBaseUrl, part2);
    }

    public static string GetWikiPageRemoteUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier,
      string pagePath)
    {
      return new StringBuilder(WikiUrlHelper.GetWikiRemoteUrl(requestContext, projectIdentifier, wikiIdentifier)).Append("?").Append(nameof (pagePath)).Append("=").Append(Uri.EscapeDataString(pagePath)).ToString();
    }

    internal static string GetWikiPageRemoteUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier,
      int pageId)
    {
      return new StringBuilder(WikiUrlHelper.GetWikiRemoteUrl(requestContext, projectIdentifier, wikiIdentifier)).Append("?").Append(nameof (pageId)).Append("=").Append(pageId.ToString()).ToString();
    }

    public static string GetWikiResourceUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wiki", WikiConstants.WikisLocationId, (object) new
      {
        wikiIdentifier = Uri.EscapeDataString(wikiIdentifier),
        project = Uri.EscapeDataString(projectIdentifier)
      }).AbsoluteUri;
    }

    public static string GetWikiPageResourceUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier,
      string pagePath)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wiki", WikiConstants.WikiPagesLocationId, (object) new
      {
        wikiIdentifier = Uri.EscapeDataString(wikiIdentifier),
        path = Uri.EscapeDataString(pagePath),
        project = Uri.EscapeDataString(projectIdentifier)
      }).AbsoluteUri;
    }

    public static string GetWikiPageCommentResourceUrl(
      IVssRequestContext requestContext,
      string projectIdentifier,
      string wikiIdentifier,
      string pageId,
      string commentId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wiki", WikiConstants.WikiPageCommentsLocationId, (object) new
      {
        wikiIdentifier = Uri.EscapeDataString(wikiIdentifier),
        pageId = Uri.EscapeDataString(pageId),
        project = Uri.EscapeDataString(projectIdentifier),
        id = Uri.EscapeDataString(commentId)
      }).AbsoluteUri;
    }

    public static string GetWikiPageCommentAttachmentsResourceUrl(
      IVssRequestContext requestContext,
      Guid project,
      Guid wikiIdentifier,
      int pageId,
      Guid? attachmentId = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      return service.FindServiceDefinition(requestContext, "wiki", WikiConstants.WikiPageCommentAttachmentsLocationId) != null ? service.GetResourceUri(requestContext, "wiki", WikiConstants.WikiPageCommentAttachmentsLocationId, (object) new
      {
        project = project,
        wikiIdentifier = wikiIdentifier,
        pageId = pageId,
        attachmentId = attachmentId
      }).AbsoluteUri : (string) null;
    }

    public static string GetPageCommitDiffUrl(string wikiPageRemoteUrl, string commitId) => new StringBuilder(wikiPageRemoteUrl).Append("&_a=compare").Append(SpecialChars.Ampersand).Append("version").Append("=").Append(commitId).ToString();

    public static string AppendUrlParam(string url, string paramName, string paramValue) => new StringBuilder(url).Append(SpecialChars.Ampersand).Append(paramName).Append("=").Append(paramValue).ToString();
  }
}
