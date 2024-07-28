// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.IWikiPagesOrderProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public interface IWikiPagesOrderProvider
  {
    void PopulateOrderInWikiPages(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      Dictionary<GitPath, GitItem> orderFilesMap,
      Dictionary<WikiPagePath, WikiPage> wikiPagesMap,
      string rootPath,
      WikiPagePath wikiPagePath);

    string AddPageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath pagePath,
      int pageOrder,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew);

    string RemovePageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath wikiPagePath,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew);

    string ModifyPageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath pagePath,
      int pageOrder,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew);

    string ModifyPageTitle(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath newWikiPagePath,
      WikiPagePath originalWikiPagePath,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew);
  }
}
