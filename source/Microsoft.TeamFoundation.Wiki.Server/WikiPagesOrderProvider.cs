// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPagesOrderProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPagesOrderProvider : IWikiPagesOrderProvider
  {
    public void PopulateOrderInWikiPages(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      Dictionary<GitPath, GitItem> orderFilesMap,
      Dictionary<WikiPagePath, WikiPage> wikiPagesMap,
      string rootPath,
      WikiPagePath wikiPagePath)
    {
      Dictionary<GitPath, Dictionary<string, int>> orderFilePathToTitlesOrderMap = new Dictionary<GitPath, Dictionary<string, int>>();
      if (!wikiPagePath.IsWikiRootPage())
        this.PopulateOrderTitleMap(requestContext, repository, versionDescriptor, pagesOrderReader, GitPath.ForParentOrderFile(wikiPagePath, rootPath), orderFilePathToTitlesOrderMap);
      foreach (KeyValuePair<GitPath, GitItem> orderFiles in orderFilesMap)
      {
        if (orderFiles.Value != null && orderFiles.Value.Path != null)
          this.PopulateOrderTitleMap(requestContext, repository, versionDescriptor, pagesOrderReader, GitPath.FromGitItem(orderFiles.Value), orderFilePathToTitlesOrderMap);
      }
      foreach (KeyValuePair<WikiPagePath, WikiPage> wikiPages in wikiPagesMap)
      {
        WikiPage wikiPage = wikiPages.Value;
        WikiPagePath key1 = wikiPages.Key;
        if (!key1.IsWikiRootPage())
        {
          GitPath key2 = GitPath.ForParentOrderFile(key1, rootPath);
          Dictionary<string, int> dictionary;
          if (orderFilePathToTitlesOrderMap.TryGetValue(key2, out dictionary))
          {
            int num;
            wikiPage.Order = !dictionary.TryGetValue(key1.GetGitFileName(), out num) ? int.MaxValue : num;
          }
        }
      }
    }

    public string AddPageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath pageFilePath,
      int pageOrder,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew)
    {
      return this.AddOrModifyPageOrder(requestContext, repository, versionDescriptor, pagesOrderReader, pageFilePath, pageOrder, mappedPath, out orderingFilePath, out createNew);
    }

    public string RemovePageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath wikiPagePath,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew)
    {
      string gitFileName = wikiPagePath.GetGitFileName();
      orderingFilePath = GitPath.ForParentOrderFile(wikiPagePath, mappedPath);
      IList<string> stringList = (IList<string>) pagesOrderReader.GetOrderedPageTitles(requestContext, repository, versionDescriptor, orderingFilePath, out createNew);
      if (!createNew)
        stringList = this.RemovePageFromOrderedList(stringList, gitFileName);
      return this.GetOrderedContent(stringList);
    }

    public string ModifyPageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath pagePath,
      int pageOrder,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew)
    {
      return this.AddOrModifyPageOrder(requestContext, repository, versionDescriptor, pagesOrderReader, pagePath, pageOrder, mappedPath, out orderingFilePath, out createNew);
    }

    public string ModifyPageTitle(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath newWikiPagePath,
      WikiPagePath originalWikiPagePath,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew)
    {
      string str = newWikiPagePath.GetParentPagePath().Equals((object) originalWikiPagePath.GetParentPagePath()) ? newWikiPagePath.GetGitFileName() : throw new InvalidArgumentValueException(nameof (newWikiPagePath), Resources.InvalidParametersOrNull);
      string gitFileName = originalWikiPagePath.GetGitFileName();
      orderingFilePath = GitPath.ForParentOrderFile(newWikiPagePath, mappedPath);
      IList<string> orderedPageTitles;
      IList<string> orderedPaths = orderedPageTitles = (IList<string>) pagesOrderReader.GetOrderedPageTitles(requestContext, repository, versionDescriptor, orderingFilePath, out createNew);
      if (createNew)
      {
        orderedPaths.Add(str);
      }
      else
      {
        int index = orderedPaths.IndexOf(gitFileName);
        if (index >= 0)
          orderedPaths[index] = str;
        else
          orderedPaths.Add(str);
      }
      return this.GetOrderedContent(orderedPaths);
    }

    private string GetOrderedContent(IList<string> orderedPaths) => orderedPaths == null ? string.Empty : string.Join(Environment.NewLine, orderedPaths.ToArray<string>());

    private string AddOrModifyPageOrder(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      WikiPagePath wikiPagePath,
      int pageOrder,
      string mappedPath,
      out GitPath orderingFilePath,
      out bool createNew)
    {
      string gitFileName = wikiPagePath.GetGitFileName();
      orderingFilePath = GitPath.ForParentOrderFile(wikiPagePath, mappedPath);
      return this.GetOrderedContent(this.AddPageToOrderedList(this.RemovePageFromOrderedList((IList<string>) pagesOrderReader.GetOrderedPageTitles(requestContext, repository, versionDescriptor, orderingFilePath, out createNew), gitFileName), gitFileName, pageOrder));
    }

    private IList<string> AddPageToOrderedList(
      IList<string> orderedPageTitles,
      string pageTitleToBeAdded,
      int order)
    {
      if (order < 0)
        throw new InvalidArgumentValueException(nameof (order), Resources.WikiPageOrderNegative);
      if (orderedPageTitles != null && orderedPageTitles.Count > 0 && orderedPageTitles.Count > order)
        orderedPageTitles.Insert(order, pageTitleToBeAdded);
      else if (orderedPageTitles != null && order >= orderedPageTitles.Count)
        orderedPageTitles.Add(pageTitleToBeAdded);
      return orderedPageTitles;
    }

    private IList<string> RemovePageFromOrderedList(
      IList<string> orderedPageTitles,
      string pageTitleToBeRemoved)
    {
      if (orderedPageTitles != null && orderedPageTitles.Count > 0)
        orderedPageTitles.Remove(pageTitleToBeRemoved);
      return orderedPageTitles;
    }

    private void PopulateOrderTitleMap(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      GitPath orderFilePath,
      Dictionary<GitPath, Dictionary<string, int>> orderFilePathToTitlesOrderMap)
    {
      if (orderFilePathToTitlesOrderMap.ContainsKey(orderFilePath))
        return;
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      List<string> orderedPageTitles = pagesOrderReader.GetOrderedPageTitles(requestContext, repository, versionDescriptor, orderFilePath, out bool _);
      for (int index = 0; index < orderedPageTitles.Count; ++index)
      {
        try
        {
          dictionary.Add(orderedPageTitles[index], index);
        }
        catch (Exception ex)
        {
        }
      }
      orderFilePathToTitlesOrderMap.Add(orderFilePath, dictionary);
    }
  }
}
