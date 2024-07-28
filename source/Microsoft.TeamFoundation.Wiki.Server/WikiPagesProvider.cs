// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPagesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPagesProvider : IWikiPagesProvider
  {
    private WikiPageIdProvider m_wikiPageIdProvider;

    public WikiPagesProvider() => this.m_wikiPageIdProvider = new WikiPageIdProvider();

    internal WikiPagesProvider(WikiPageIdProvider wikiPageIdProvider) => this.m_wikiPageIdProvider = wikiPageIdProvider;

    public WikiPage GetPage(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Guid wikiId,
      string rootPath,
      WikiPagePath wikiPagePath,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      VersionControlRecursionType recursionType = VersionControlRecursionType.None,
      bool includeContent = false,
      bool includePageId = false)
    {
      return this.GetPageInternal(requestContext, repository, wikiId, rootPath, wikiPagePath, versionDescriptor, pagesOrderReader, recursionType, includeContent, includePageId);
    }

    public WikiPage GetPageById(
      IVssRequestContext requestContext,
      Guid projectId,
      int pageId,
      IWikiPageIdDetailsProvider pageIdDetailsProvider = null,
      IWikiPagesOrderReader pagesOrderReader = null,
      VersionControlRecursionType recursionType = VersionControlRecursionType.None,
      bool includeContent = false)
    {
      if (pageIdDetailsProvider == null)
        pageIdDetailsProvider = (IWikiPageIdDetailsProvider) new WikiPageIdDetailsProvider();
      WikiV2 wiki;
      WikiPageIdDetails pageIdDetails = pageIdDetailsProvider.GetPageIdDetails(requestContext, projectId, pageId, out wiki);
      using (ITfsGitRepository wikiRepository = WikiV2Helper.GetWikiRepository(requestContext, wiki.RepositoryId))
      {
        TfsGitCommit commit = this.ResolveBranchToCommit(requestContext, wikiRepository, pageIdDetails.WikiVersion);
        IVssRequestContext requestContext1 = requestContext;
        ITfsGitRepository repository = wikiRepository;
        Guid id = wiki.Id;
        string mappedPath = wiki.MappedPath;
        WikiPagePath givenWikiPagePath = WikiPagePath.FromWikiPageIdDetails(pageIdDetails);
        GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
        versionDescriptor.Version = commit.ObjectId.ToString();
        versionDescriptor.VersionType = GitVersionType.Commit;
        IWikiPagesOrderReader pagesOrderReader1 = pagesOrderReader;
        int recursionType1 = (int) recursionType;
        int num = includeContent ? 1 : 0;
        WikiPage pageInternal = this.GetPageInternal(requestContext1, repository, id, mappedPath, givenWikiPagePath, versionDescriptor, pagesOrderReader1, (VersionControlRecursionType) recursionType1, num != 0, false);
        pageInternal.Id = new int?(pageId);
        pageInternal.RemoteUrl = WikiUrlHelper.GetWikiPageRemoteUrl(requestContext, projectId.ToString(), wiki.Id.ToString(), pageId);
        return pageInternal;
      }
    }

    public Dictionary<WikiPagePath, WikiPage> GetIndigenousWikiPagesFromGitItems(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Guid wikiId,
      string rootPath,
      WikiPagePath wikiPagePath,
      Dictionary<GitPath, GitItem> gitItemsMap,
      out Dictionary<GitPath, GitItem> orderFilesMap)
    {
      Dictionary<GitPath, GitItem> folderItemsMap = new Dictionary<GitPath, GitItem>();
      Dictionary<GitPath, GitItem> dictionary1 = new Dictionary<GitPath, GitItem>();
      Dictionary<WikiPagePath, WikiPage> pagesFromGitItems = new Dictionary<WikiPagePath, WikiPage>();
      Dictionary<GitPath, bool> dictionary2 = new Dictionary<GitPath, bool>();
      orderFilesMap = new Dictionary<GitPath, GitItem>();
      foreach (KeyValuePair<GitPath, GitItem> gitItems in gitItemsMap)
      {
        GitPath key = gitItems.Key;
        GitItem gitItem = gitItems.Value;
        if (gitItem.IsFolder)
        {
          folderItemsMap.Add(key, gitItem);
          dictionary2.Add(key, false);
        }
        else if (Path.GetExtension(gitItem.Path).Equals(".md", StringComparison.InvariantCultureIgnoreCase))
        {
          if (!string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(gitItem.Path)))
            dictionary1.Add(key, gitItem);
        }
        else if (Path.GetExtension(gitItem.Path).Equals(".order", StringComparison.InvariantCultureIgnoreCase))
          orderFilesMap.Add(key, gitItem);
      }
      this.GetReservedFolders(requestContext).ForEach((Action<string>) (reservedPath =>
      {
        if (rootPath == "/")
          folderItemsMap.Remove(GitPath.FromMappedPath("/" + reservedPath));
        else
          folderItemsMap.Remove(GitPath.FromMappedPath(rootPath + "/" + reservedPath));
      }));
      GitPath key1 = GitPath.FromMappedPath(rootPath);
      GitItem gitItem1;
      if (!folderItemsMap.TryGetValue(key1, out gitItem1))
        gitItem1 = (GitItem) null;
      WikiPageUrlBuilder wikiPageUrlBuilder1 = new WikiPageUrlBuilder(requestContext, repository.Key.ProjectId, wikiId, Guid.NewGuid().ToString());
      if (wikiPagePath.IsWikiRootPage())
      {
        pagesFromGitItems.Add(wikiPagePath, this.CreateWikiPage(wikiPagePath, true, 0, gitItem1, wikiPageUrlBuilder1));
        dictionary2[key1] = true;
      }
      foreach (KeyValuePair<GitPath, GitItem> keyValuePair in dictionary1)
      {
        GitItem gitItem2 = keyValuePair.Value;
        GitPath key2 = GitPath.FromGitItem(gitItem2).GitEquivalentFolderPath();
        bool flag1 = folderItemsMap.ContainsKey(key2);
        if (flag1)
          dictionary2[key2] = true;
        WikiPagePath key3 = WikiPagePath.FromGitItem(gitItem2, rootPath);
        bool flag2 = PathHelper.IsPageFileNameNonConformant(gitItem2.Path, this.IsTemplateFolderWhiteListed(requestContext));
        if (!pagesFromGitItems.ContainsKey(key3) || !flag2)
        {
          Dictionary<WikiPagePath, WikiPage> dictionary3 = pagesFromGitItems;
          WikiPagePath key4 = key3;
          WikiPagePath wikiPagePath1 = key3;
          int num1 = flag1 ? 1 : 0;
          GitItem gitItem3 = gitItem2;
          bool flag3 = flag2;
          WikiPageUrlBuilder wikiPageUrlBuilder2 = wikiPageUrlBuilder1;
          int num2 = flag3 ? 1 : 0;
          WikiPage wikiPage = this.CreateWikiPage(wikiPagePath1, num1 != 0, int.MaxValue, gitItem3, wikiPageUrlBuilder2, num2 != 0);
          dictionary3[key4] = wikiPage;
        }
      }
      foreach (KeyValuePair<GitPath, GitItem> keyValuePair in folderItemsMap)
      {
        GitItem gitItem4 = keyValuePair.Value;
        GitPath key5 = keyValuePair.Key;
        if (!dictionary2[key5])
        {
          WikiPagePath key6 = WikiPagePath.FromGitItem(gitItem4, rootPath);
          if (pagesFromGitItems.ContainsKey(key6))
          {
            requestContext.Trace(15250404, TraceLevel.Error, "Wiki", "Service", "Key already added in the dictionary");
          }
          else
          {
            bool flag4 = PathHelper.IsPageFileNameNonConformant(gitItem4.Path, this.IsTemplateFolderWhiteListed(requestContext));
            if (!pagesFromGitItems.ContainsKey(key6) || !flag4)
            {
              Dictionary<WikiPagePath, WikiPage> dictionary4 = pagesFromGitItems;
              WikiPagePath key7 = key6;
              WikiPagePath wikiPagePath2 = WikiPagePath.FromGitItem(gitItem4, rootPath);
              bool flag5 = flag4;
              WikiPageUrlBuilder wikiPageUrlBuilder3 = wikiPageUrlBuilder1;
              int num = flag5 ? 1 : 0;
              WikiPage wikiPage = this.CreateWikiPage(wikiPagePath2, true, int.MaxValue, (GitItem) null, wikiPageUrlBuilder3, num != 0);
              dictionary4[key7] = wikiPage;
            }
          }
        }
      }
      return pagesFromGitItems;
    }

    public List<string> GetFlattenedPagePathList(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string rootPath,
      GitVersionDescriptor versionDescriptor,
      VersionControlRecursionType recursionType = VersionControlRecursionType.None,
      TimedCiEvent ciData = null)
    {
      using (new StopWatchHelper(ciData, nameof (GetFlattenedPagePathList)))
      {
        GitItemsCollection filePathsForWiki;
        try
        {
          filePathsForWiki = this.GetUnorderedGitFilePathsForWiki(requestContext, repository, GitPath.FromMappedPath(rootPath), versionDescriptor, recursionType);
        }
        catch (GitItemNotFoundException ex)
        {
          requestContext.TraceAlways(15250700, TraceLevel.Info, "Wiki", nameof (WikiPagesProvider), string.Format("ProjectId:{0}, version:{1} rootPath does not exist", (object) repository.Key.ProjectId, (object) versionDescriptor.Version));
          return new List<string>();
        }
        List<string> wikiPageList = new List<string>();
        if (filePathsForWiki.Count > 0)
          filePathsForWiki.ForEach((Action<GitItem>) (gitItem =>
          {
            string path = gitItem.Path;
            if (!(path != rootPath) || !path.IsMdFile(requestContext) || !gitItem.GitObjectType.Equals((object) GitObjectType.Blob))
              return;
            string pathFromGitFilePath = path.GetWikiPathFromGitFilePath(rootPath);
            if (pathFromGitFilePath == null)
              return;
            wikiPageList.Add(pathFromGitFilePath);
          }));
        return wikiPageList;
      }
    }

    private TfsGitCommit ResolveBranchToCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor wikiVersion)
    {
      return GitVersionParser.GetCommitFromVersion(requestContext, repository, new GitVersionDescriptor()
      {
        Version = wikiVersion.Version,
        VersionType = wikiVersion.VersionType,
        VersionOptions = wikiVersion.VersionOptions
      });
    }

    private WikiPage GetPageInternal(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Guid wikiId,
      string mappedPath,
      WikiPagePath givenWikiPagePath,
      GitVersionDescriptor versionDescriptor,
      IWikiPagesOrderReader pagesOrderReader,
      VersionControlRecursionType recursionType,
      bool includeContent,
      bool includePageId)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      if (givenWikiPagePath == null)
        throw new ArgumentException("Is null", nameof (givenWikiPagePath));
      if (versionDescriptor == null)
        throw new ArgumentException("Is null or empty", "wikiVersion");
      GitItem gitItem1 = (GitItem) null;
      GitItemsCollection enumerable = (GitItemsCollection) null;
      GitPath gitPath = GitPath.GitMdPathFromWikiPagePath(givenWikiPagePath, mappedPath);
      if (!givenWikiPagePath.IsWikiRootPage())
      {
        try
        {
          gitItem1 = this.GetUnorderedGitFilePathsForWiki(requestContext, repository, gitPath, versionDescriptor, includeContent: includeContent, scanBytes: long.MaxValue).FirstOrDefault<GitItem>();
        }
        catch (GitItemNotFoundException ex)
        {
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15250401, "Wiki", "Service", ex);
          throw;
        }
      }
      try
      {
        enumerable = this.GetUnorderedGitFilePathsForWiki(requestContext, repository, gitPath.GitEquivalentFolderPath(), versionDescriptor, recursionType);
      }
      catch (GitItemNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15250402, "Wiki", "Service", ex);
        throw;
      }
      if (gitItem1 == null && enumerable.IsNullOrEmpty<GitItem>())
        throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) givenWikiPagePath));
      Dictionary<GitPath, GitItem> gitItemsMap = new Dictionary<GitPath, GitItem>();
      if (gitItem1 != null)
        gitItemsMap.Add(GitPath.FromGitItem(gitItem1), gitItem1);
      // ISSUE: explicit non-virtual call
      if (enumerable != null && __nonvirtual (enumerable.Count) > 0)
        enumerable.ForEach((Action<GitItem>) (gitItem => gitItemsMap.Add(GitPath.FromGitItem(gitItem), gitItem)));
      if (givenWikiPagePath.IsWikiRootPage() && !gitItemsMap.ContainsKey(GitPath.FromMappedPath(mappedPath)))
        throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) GitPath.GitMdPathFromWikiPagePath(givenWikiPagePath, mappedPath)));
      Dictionary<GitPath, GitItem> orderFilesMap;
      Dictionary<WikiPagePath, WikiPage> pagesFromGitItems = this.GetIndigenousWikiPagesFromGitItems(requestContext, repository, wikiId, mappedPath, givenWikiPagePath, gitItemsMap, out orderFilesMap);
      if (pagesOrderReader != null)
        new WikiPagesOrderProvider().PopulateOrderInWikiPages(requestContext, repository, versionDescriptor, pagesOrderReader, orderFilesMap, pagesFromGitItems, mappedPath, givenWikiPagePath);
      WikiPage pageInternal = PageHelper.ConstructWikiPage(pagesFromGitItems, givenWikiPagePath);
      if (pageInternal == null)
        throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) givenWikiPagePath));
      if (includeContent && pageInternal.InternalItem?.ContentMetadata != null)
      {
        using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repository, new Sha1Id(pageInternal.InternalItem.ObjectId)))
          pageInternal.Content = VersionControlFileReader.ReadFileContent(fileContentStream, pageInternal.InternalItem.ContentMetadata.Encoding);
      }
      else
        pageInternal.Content = string.Empty;
      if (includePageId && this.isPermaIdFeatureAvailable(requestContext) && pageInternal.GitItemPath.IsMdFile())
      {
        using (TimedCiEvent ciEvent = new TimedCiEvent(requestContext, "Microsoft.TeamFoundation.Wiki.Server", "GetWikiUniquePageIds"))
        {
          if (versionDescriptor.VersionType != GitVersionType.Branch)
            throw new WikiInvalidVersionType(string.Format(Resources.InvalidWikiVersionType, (object) versionDescriptor.VersionType));
          if (gitItem1 != null)
          {
            if (gitItem1.Path != null)
            {
              pageInternal.Id = this.m_wikiPageIdProvider.GetPageIdByPath(requestContext, repository.Key.ProjectId, wikiId, versionDescriptor, gitItem1.Path.GetWikiPathFromGitFilePath(mappedPath), ciEvent);
              ciEvent?.Properties.Add("PageIdFound", (object) pageInternal.Id.HasValue);
            }
          }
        }
      }
      return pageInternal;
    }

    private WikiPage CreateWikiPage(
      WikiPagePath wikiPagePath,
      bool isParentPage,
      int order,
      GitItem item,
      WikiPageUrlBuilder wikiPageUrlBuilder,
      bool isNonConformant = false)
    {
      WikiPage wikiPage = new WikiPage(wikiPagePath.Path, isParentPage, order, item, isNonConformant);
      wikiPage.Url = wikiPageUrlBuilder.getPageResourceUrl(wikiPage.Path);
      wikiPage.RemoteUrl = wikiPageUrlBuilder.getPageRemoteUrl(wikiPage.Path);
      return wikiPage;
    }

    public GitItemsCollection GetUnorderedGitFilePathsForWiki(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPath gitPath,
      GitVersionDescriptor versionDescriptor,
      VersionControlRecursionType recursionType = VersionControlRecursionType.None,
      bool includeContent = false,
      long scanBytes = 0)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      if (versionDescriptor == null)
        throw new ArgumentNullException(nameof (versionDescriptor));
      GitItemDescriptor itemDescriptor = new GitItemDescriptor()
      {
        Path = gitPath.Path,
        RecursionLevel = recursionType,
        Version = versionDescriptor.Version,
        VersionType = versionDescriptor.VersionType,
        VersionOptions = versionDescriptor.VersionOptions
      };
      return GitItemUtility.RetrieveItemModels(requestContext, (UrlHelper) null, repository, itemDescriptor, includeContent, false, scanBytes) ?? new GitItemsCollection();
    }

    private bool IsTemplateFolderWhiteListed(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Wiki.EnableTemplateEditing") && requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-wiki-web.wiki-templates-editing");

    private bool isPermaIdFeatureAvailable(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.Wiki.PermanentLinks");

    private List<string> GetReservedFolders(IVssRequestContext requestContext)
    {
      List<string> reservedFolders = new List<string>()
      {
        ".attachments"
      };
      if (!this.IsTemplateFolderWhiteListed(requestContext))
        reservedFolders.Add(".templates");
      return reservedFolders;
    }
  }
}
