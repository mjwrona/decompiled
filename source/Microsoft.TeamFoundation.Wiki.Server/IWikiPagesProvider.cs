// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.IWikiPagesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public interface IWikiPagesProvider
  {
    WikiPage GetPage(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Guid wikiId,
      string rootPath,
      WikiPagePath wikiPagePath,
      GitVersionDescriptor wikiVersion,
      IWikiPagesOrderReader pagesOrderReader,
      VersionControlRecursionType recursionType,
      bool includeContent,
      bool includePageId);

    WikiPage GetPageById(
      IVssRequestContext requestContext,
      Guid projectId,
      int pageId,
      IWikiPageIdDetailsProvider pageIdDetailsProvider,
      IWikiPagesOrderReader pagesOrderReader,
      VersionControlRecursionType recursionType,
      bool includeContent);
  }
}
