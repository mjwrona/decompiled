// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiExtensions
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiExtensions
  {
    public static void PopulateUrls(this WikiV2 wiki, IVssRequestContext requestContext)
    {
      WikiV2 wikiV2_1 = wiki;
      IVssRequestContext requestContext1 = requestContext;
      Guid guid1 = wiki.ProjectId;
      string projectIdentifier1 = guid1.ToString();
      guid1 = wiki.Id;
      string wikiIdentifier1 = guid1.ToString();
      string wikiResourceUrl = WikiUrlHelper.GetWikiResourceUrl(requestContext1, projectIdentifier1, wikiIdentifier1);
      wikiV2_1.Url = wikiResourceUrl;
      WikiV2 wikiV2_2 = wiki;
      IVssRequestContext requestContext2 = requestContext;
      Guid guid2 = wiki.ProjectId;
      string projectIdentifier2 = guid2.ToString();
      guid2 = wiki.Id;
      string wikiIdentifier2 = guid2.ToString();
      string wikiRemoteUrl = WikiUrlHelper.GetWikiRemoteUrl(requestContext2, projectIdentifier2, wikiIdentifier2);
      wikiV2_2.RemoteUrl = wikiRemoteUrl;
    }

    public static void PopulateUrls(
      this WikiPage wikiPage,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId)
    {
      wikiPage.Url = WikiUrlHelper.GetWikiPageResourceUrl(requestContext, projectId.ToString(), wikiId.ToString(), wikiPage.Path);
      wikiPage.RemoteUrl = WikiUrlHelper.GetWikiPageRemoteUrl(requestContext, projectId.ToString(), wikiId.ToString(), wikiPage.Path);
    }
  }
}
