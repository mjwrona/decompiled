// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitItemUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitItemUtility
  {
    private static readonly char[] c_pathSeparators = new char[2]
    {
      '/',
      '\\'
    };
    private static readonly string c_separator = "/";
    private static readonly int c_maxFilePathsCount = 200000;
    private const int c_MaxRecursionDepth = 100;

    public static TfsGitObject FindMember(
      IVssRequestContext requestContext,
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry)
    {
      return GitItemUtility.FindMember(requestContext, tree, ref path, out treeEntry, 0);
    }

    private static TfsGitObject FindMember(
      IVssRequestContext requestContext,
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry,
      int recursionDepth)
    {
      if (recursionDepth > 100)
        throw new GitRecursionLimitReachedException(100);
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      string str = (string) null;
      string path1 = (string) null;
      TfsGitObject tree1 = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      int length;
      do
      {
        length = path.IndexOfAny(GitItemUtility.c_pathSeparators);
        if (length == 0)
          path = path.Substring(1);
        else if (length < 0)
          str = path;
        else if (length == path.Length - 1)
        {
          path = path.Substring(0, path.Length - 1);
          length = 0;
        }
        else
        {
          str = path.Substring(0, length);
          path1 = path.Substring(length + 1);
        }
      }
      while (length == 0);
      List<TfsGitTreeEntry> tfsGitTreeEntryList = new List<TfsGitTreeEntry>();
      foreach (TfsGitTreeEntry treeEntry1 in tree.GetTreeEntries())
      {
        if (treeEntry1.Name.Equals(str, StringComparison.Ordinal))
        {
          if (treeEntry1.ObjectType != GitObjectType.Commit)
            tree1 = treeEntry1.Object;
          treeEntry = treeEntry1;
          break;
        }
        if (treeEntry1.Name.Equals(str, StringComparison.CurrentCultureIgnoreCase))
          tfsGitTreeEntryList.Add(treeEntry1);
      }
      if (treeEntry == null && tfsGitTreeEntryList.Count == 1)
      {
        TfsGitTreeEntry tfsGitTreeEntry = tfsGitTreeEntryList[0];
        if (tfsGitTreeEntry.ObjectType == GitObjectType.Commit)
          path = tfsGitTreeEntry.Name;
        else
          tree1 = tfsGitTreeEntry.Object;
        str = tfsGitTreeEntry.Name;
        treeEntry = tfsGitTreeEntry;
      }
      if (path1 == null)
      {
        path = str;
        return tree1;
      }
      TfsGitObject member = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      if (tree1 is TfsGitTree)
        member = GitItemUtility.FindMember(requestContext, (TfsGitTree) tree1, ref path1, out treeEntry, recursionDepth + 1);
      path = str + "/" + path1;
      return member;
    }

    public static GitItem RetrieveItemModel(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      ITfsGitRepository repository,
      GitItemDescriptor itemDescriptor)
    {
      return GitItemUtility.RetrieveItemModels(requestContext, urlHelper, repository, itemDescriptor, false, false).FirstOrDefault<GitItem>();
    }

    public static GitItemsCollection RetrieveItemModels(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      ITfsGitRepository repository,
      GitItemDescriptor itemDescriptor,
      bool includeContentMetadata,
      bool includeLinks,
      long scanBytes = 0,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers = null,
      TfsGitCommit resolvedCommit = null)
    {
      GitItem gitItem1;
      TfsGitObject gitItem2 = GitItemUtility.FindGitItem(requestContext, repository, itemDescriptor, resolvedCommit, out gitItem1);
      if (urlHelper != null)
      {
        var routeValues = new
        {
          project = repository.Key.ProjectId,
          repositoryId = repository.Key.RepoId,
          path = gitItem1.Path,
          versionType = itemDescriptor.VersionType,
          version = itemDescriptor.Version,
          versionOptions = itemDescriptor.VersionOptions
        };
        ILocationService service = requestContext.GetService<ILocationService>();
        gitItem1.Url = service.GetResourceUri(requestContext, "git", GitWebApiConstants.ItemsLocationId, (object) routeValues, true, false, true).AbsoluteUri;
      }
      if (includeContentMetadata)
      {
        string branch = (string) null;
        if (itemDescriptor.VersionType == GitVersionType.Branch && !string.IsNullOrEmpty(itemDescriptor.Version))
          branch = itemDescriptor.Version;
        gitItem1.ContentMetadata = GitItemUtility.GetContentMetadata(requestContext, repository, gitItem1, true, branch, scanBytes, linkedContentResolvers);
      }
      gitItem1.Links = includeLinks ? gitItem1.GetItemReferenceLinks(requestContext, urlHelper, repository.Key) : (ReferenceLinks) null;
      GitItemsCollection itemList = new GitItemsCollection();
      itemList.Add(gitItem1);
      if (gitItem2 is TfsGitTree && itemDescriptor.RecursionLevel != VersionControlRecursionType.None)
        GitItemUtility.PopulateItemsCollection(requestContext, urlHelper, repository.Key, gitItem1, gitItem2 as TfsGitTree, itemList, itemDescriptor, 0);
      return itemList;
    }

    public static TfsGitObject FindGitItem(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitItemDescriptor itemDescriptor,
      TfsGitCommit resolvedCommit,
      out GitItem gitItem)
    {
      if (itemDescriptor.RecursionLevel != VersionControlRecursionType.None && itemDescriptor.VersionOptions == GitVersionOptions.PreviousChange)
        throw new ArgumentException(Resources.Get("ErrorPreviousChangeRecursive"));
      string path = GitItemUtility.TrimPathSeparator(itemDescriptor.Path);
      GitVersionDescriptor versionDescriptor = new GitVersionDescriptor()
      {
        Version = itemDescriptor.Version,
        VersionType = itemDescriptor.VersionType,
        VersionOptions = itemDescriptor.VersionOptions
      };
      GitVersionParser.ValidateVersionDescriptor(versionDescriptor);
      TfsGitCommit tfsGitCommit = resolvedCommit ?? GitVersionParser.GetCommitFromVersion(requestContext, repository, ref path, versionDescriptor);
      string commitId = tfsGitCommit.ObjectId.ToString();
      TfsGitTree tree = tfsGitCommit.GetTree();
      if (string.IsNullOrEmpty(path))
      {
        gitItem = new GitItem(path, tree.ObjectId.ToString(), tree.ObjectType, commitId, 0);
        return (TfsGitObject) tree;
      }
      TfsGitTreeEntry treeEntry;
      TfsGitObject member = GitItemUtility.FindMember(requestContext, tree, ref path, out treeEntry);
      if (treeEntry == null)
        throw new GitItemNotFoundException(path, repository.Name, versionDescriptor.ToString(), commitId);
      gitItem = new GitItem(path, treeEntry.ObjectId.ToString(), treeEntry.ObjectType, commitId, (int) treeEntry.Mode);
      return member;
    }

    public static GitFilePathsCollection RetrieveFilePaths(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      ITfsGitRepository repository,
      string scopePath,
      GitVersionDescriptor versionDescriptor)
    {
      GitItemDescriptor itemDescriptor = new GitItemDescriptor()
      {
        Path = scopePath,
        RecursionLevel = VersionControlRecursionType.Full
      };
      if (versionDescriptor != null)
      {
        itemDescriptor.Version = versionDescriptor.Version;
        itemDescriptor.VersionType = versionDescriptor.VersionType;
        itemDescriptor.VersionOptions = versionDescriptor.VersionOptions;
      }
      GitItem gitItem1;
      TfsGitObject gitItem2 = GitItemUtility.FindGitItem(requestContext, repository, itemDescriptor, (TfsGitCommit) null, out gitItem1);
      GitFilePathsCollection pathList = new GitFilePathsCollection();
      if (urlHelper != null)
        pathList.Url = urlHelper.RestLink(requestContext, GitWebApiConstants.FilePathsLocationId, (object) new
        {
          repositoryId = repository.Key.RepoId,
          scopepath = GitItemUtility.TrimPathSeparator(gitItem1.Path),
          versionType = itemDescriptor.VersionType,
          version = itemDescriptor.Version,
          versionOptions = itemDescriptor.VersionOptions
        });
      pathList.CommitId = gitItem1.CommitId;
      switch (gitItem2)
      {
        case TfsGitTree _:
          string folderPath = GitItemUtility.TrimPathSeparator(gitItem1.Path);
          if (folderPath != string.Empty)
            folderPath += GitItemUtility.c_separator;
          int maxFilePaths = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/GitRest/Settings/maxFilePaths", GitItemUtility.c_maxFilePathsCount);
          try
          {
            GitItemUtility.PopulateGitFilePaths(folderPath, gitItem2 as TfsGitTree, pathList, maxFilePaths);
            break;
          }
          catch (GitFilePathsMaxCountExceededException ex)
          {
            requestContext.Trace(1013659, TraceLevel.Error, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "File path count exceeded limit of {0} for repositoryId:{1}", (object) maxFilePaths, (object) repository.Key.RepoId.ToString());
            throw;
          }
        case TfsGitBlob _:
          pathList.Paths.Add(GitItemUtility.TrimPathSeparator(gitItem1.Path));
          break;
      }
      return pathList;
    }

    private static void PopulateGitFilePaths(
      string folderPath,
      TfsGitTree tree,
      GitFilePathsCollection pathList,
      int maxFilePaths)
    {
      foreach (TfsGitTreeEntry treeEntry in tree.GetTreeEntries())
      {
        string str = folderPath + treeEntry.Name;
        if (treeEntry.ObjectType == GitObjectType.Blob)
        {
          pathList.Paths.Add(str);
          if (pathList.Paths.Count > maxFilePaths)
            throw new GitFilePathsMaxCountExceededException(maxFilePaths);
        }
        else if (treeEntry.ObjectType == GitObjectType.Tree)
          GitItemUtility.PopulateGitFilePaths(str + GitItemUtility.c_separator, (TfsGitTree) treeEntry.Object, pathList, maxFilePaths);
      }
    }

    public static string GetItemPath(
      IVssRequestContext TfsRequestContext,
      ITfsGitRepository repository,
      GitQueryCommitsCriteria searchCriteria,
      UrlHelper url,
      ref bool recursive)
    {
      return GitItemUtility.GetItemPath(TfsRequestContext, repository, searchCriteria.ItemPath, searchCriteria.ItemVersion, url, ref recursive);
    }

    public static string GetItemPath(
      IVssRequestContext TfsRequestContext,
      ITfsGitRepository repository,
      string itemPath,
      GitVersionDescriptor itemVersion,
      UrlHelper Url,
      ref bool recursive)
    {
      if (!string.IsNullOrEmpty(itemPath) && !string.Equals(itemPath, "/", StringComparison.Ordinal))
      {
        GitItemDescriptor itemDescriptor = new GitItemDescriptor()
        {
          Path = itemPath,
          RecursionLevel = VersionControlRecursionType.None
        };
        if (itemVersion != null)
        {
          itemDescriptor.Version = itemVersion.Version;
          itemDescriptor.VersionType = itemVersion.VersionType;
          itemDescriptor.VersionOptions = itemVersion.VersionOptions;
        }
        GitItem gitItem = GitItemUtility.RetrieveItemModels(TfsRequestContext, Url, repository, itemDescriptor, false, false).First<GitItem>();
        recursive = gitItem.IsFolder;
        itemPath = itemPath.StartsWith("/", StringComparison.Ordinal) ? gitItem.Path : "/" + gitItem.Path;
      }
      return itemPath;
    }

    public static string GetGitRepositoryNameFromUri(Uri pageUri)
    {
      char[] separators = "/".ToCharArray();
      return HttpUtility.UrlDecode(((IEnumerable<string>) pageUri.Segments).Select<string, string>((Func<string, string>) (x => x.Trim(separators))).SkipUntilAfter<string>((Predicate<string>) (x => string.Equals(x, "_git", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<string>());
    }

    internal static FileContentMetadata GetContentMetadata(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitItem item)
    {
      return GitItemUtility.GetContentMetadata(context, repository, item, false, (string) null);
    }

    internal static FileContentMetadata GetContentMetadata(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitItem item,
      bool includeVsLink,
      string branch)
    {
      return GitItemUtility.GetContentMetadata(context, repository, item, includeVsLink, branch, 0L);
    }

    internal static FileContentMetadata GetContentMetadata(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitItem item,
      bool includeVsLink,
      string branch,
      long scanBytes)
    {
      return GitItemUtility.GetContentMetadata(context, repository, item, includeVsLink, branch, scanBytes, (IEnumerable<ITfsLinkedContentResolver>) null);
    }

    internal static FileContentMetadata GetContentMetadata(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitItem item,
      bool includeVsLink,
      string branch,
      long scanBytes,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers)
    {
      int encoding = 0;
      bool containsByteOrderMark = false;
      if (!item.IsFolder && item.GitObjectType != GitObjectType.Commit)
        encoding = GitFileUtility.TryDetectFileEncoding(repository, GitCommitUtility.ParseSha1Id(item.ObjectId), 0, scanBytes, linkedContentResolvers, out containsByteOrderMark);
      FileContentMetadata fileContentMetadata = VersionControlFileUtility.GetFileContentMetadata(GitFileUtility.GetFileNameFromPath(item.Path), item.IsFolder, encoding);
      if (includeVsLink && !item.IsFolder)
        fileContentMetadata.VisualStudioWebLink = GitItemUtility.GetVisualStudioWebLink(context, repository, item.Path, branch);
      return fileContentMetadata;
    }

    private static string GetVisualStudioWebLink(
      IVssRequestContext context,
      ITfsGitRepository repository,
      string path,
      string branch)
    {
      ArtifactId artifactId = new ArtifactId("Git", "LaunchLatestVersionedItem", string.Format("/{{{0}}}/{1}", (object) repository.Key.RepoId, (object) path));
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(branch))
        queryParameters[nameof (branch)] = branch;
      return VisualStudioLinkingUtility.GetProjectArtifactLink(context, ProjectInfo.GetProjectId(repository.Key.GetProjectUri()), artifactId, (IDictionary<string, string>) queryParameters);
    }

    private static void PopulateItemsCollection(
      IVssRequestContext context,
      UrlHelper urlHelper,
      RepoKey repoKey,
      GitItem folder,
      TfsGitTree tree,
      GitItemsCollection itemList,
      GitItemDescriptor itemDescriptor,
      int recursionDepth)
    {
      if (recursionDepth > 100)
      {
        if (itemDescriptor.RecursionLevel != VersionControlRecursionType.OneLevelPlusNestedEmptyFolders)
          throw new GitRecursionLimitReachedException(100);
      }
      else
      {
        if (itemDescriptor.RecursionLevel == VersionControlRecursionType.None)
          return;
        foreach (TfsGitTreeEntry treeEntry in tree.GetTreeEntries())
        {
          string itemPath = GitItemUtility.c_separator + treeEntry.Name;
          if (!string.Equals(folder.Path, GitItemUtility.c_separator))
            itemPath = folder.Path + itemPath;
          GitItem folder1 = new GitItem(itemPath, treeEntry.ObjectId.ToString(), treeEntry.ObjectType, folder.CommitId, (int) treeEntry.Mode);
          if (urlHelper != null)
            folder1.Url = urlHelper.RestLink(context, GitWebApiConstants.ItemsLocationId, (object) new
            {
              repositoryId = repoKey.RepoId,
              path = folder1.Path,
              versionType = itemDescriptor.VersionType,
              version = itemDescriptor.Version,
              versionOptions = itemDescriptor.VersionOptions
            });
          itemList.Add(folder1);
          if (treeEntry.ObjectType == GitObjectType.Tree)
          {
            if (itemDescriptor.RecursionLevel == VersionControlRecursionType.Full)
              GitItemUtility.PopulateItemsCollection(context, urlHelper, repoKey, folder1, (TfsGitTree) treeEntry.Object, itemList, itemDescriptor, recursionDepth + 1);
            else if (itemDescriptor.RecursionLevel == VersionControlRecursionType.OneLevelPlusNestedEmptyFolders)
            {
              IEnumerable<TfsGitObject> objects = ((TfsGitTree) treeEntry.Object).GetObjects();
              if (objects.Count<TfsGitObject>() == 1 && objects.ElementAt<TfsGitObject>(0).ObjectType == GitObjectType.Tree)
                GitItemUtility.PopulateItemsCollection(context, urlHelper, repoKey, folder1, (TfsGitTree) treeEntry.Object, itemList, itemDescriptor, recursionDepth + 1);
            }
          }
        }
      }
    }

    public static void PopulateLastChangedCommits(
      IVssRequestContext context,
      UrlHelper urlHelper,
      ITfsGitRepository repository,
      GitItemsCollection items,
      VersionControlRecursionType recursion)
    {
      ITeamFoundationGitCommitService service = context.GetService<ITeamFoundationGitCommitService>();
      if (!items.Any<GitItem>())
        return;
      GitItem gitItem1 = items[0];
      int num;
      switch (recursion)
      {
        case VersionControlRecursionType.None:
          num = 0;
          break;
        case VersionControlRecursionType.OneLevel:
          num = 1;
          break;
        default:
          num = 2;
          break;
      }
      QueryCommitItemsRecursionLevel recursionLevel = (QueryCommitItemsRecursionLevel) num;
      IEnumerable<TfsGitCommitHistoryEntry> commitHistoryEntries = service.QueryCommitItems(context, repository, GitCommitUtility.ParseSha1Id(gitItem1.CommitId), gitItem1.Path, recursionLevel);
      Dictionary<string, GitItem> dictionary = items.ToDictionary<GitItem, string>((Func<GitItem, string>) (item => item.Path));
      GitCommitTranslator commitTranslator = new GitCommitTranslator(context, repository.Key, urlHelper);
      foreach (TfsGitCommitHistoryEntry historyEntry in commitHistoryEntries)
      {
        string key = historyEntry.Change.ParentPath + historyEntry.Change.ChildItem;
        GitItem gitItem2;
        if (dictionary.TryGetValue(key, out gitItem2))
          gitItem2.LatestProcessedChange = commitTranslator.ToGitCommitShallow(historyEntry, false, false);
      }
    }

    public static string TrimPathSeparator(string path) => string.IsNullOrEmpty(path) ? string.Empty : path.Trim(GitItemUtility.c_pathSeparators);

    public static GitBlobRef CreateMinimalGitBlobRef(
      IVssRequestContext requestContext,
      Sha1Id blobId,
      RepoKey repoKey,
      UrlHelper urlHelper = null)
    {
      if (blobId.IsEmpty)
        return (GitBlobRef) null;
      string blobId1 = blobId.ToString();
      return new GitBlobRef()
      {
        ObjectId = blobId1,
        Url = GitReferenceLinksUtility.GetBlobRefUrl(requestContext, repoKey, blobId1, urlHelper)
      };
    }

    public static GitTreeRef CreateMinimalGitTreeRef(
      IVssRequestContext requestContext,
      Sha1Id treeId,
      RepoKey repoKey,
      UrlHelper urlHelper = null)
    {
      if (treeId.IsEmpty)
        return (GitTreeRef) null;
      string treeId1 = treeId.ToString();
      return new GitTreeRef()
      {
        ObjectId = treeId1,
        Url = GitReferenceLinksUtility.GetTreeRefUrl(requestContext, repoKey, treeId1, urlHelper)
      };
    }

    internal static IEnumerable<GitItem> RetrievePagedItemModels(
      IVssRequestContext requestContext,
      TfsGitTree rootTree,
      NormalizedGitPath normPath,
      GitItemsContinuationToken continuationToken = null)
    {
      NormalizedGitPath normalizedGitPath = normPath;
      GitObjectType gitObjectType = GitObjectType.Tree;
      bool skipFirstEntry = false;
      if (continuationToken != null)
      {
        normalizedGitPath = continuationToken.ContinuationPath;
        gitObjectType = continuationToken.ContinuationObjectType;
        skipFirstEntry = true;
      }
      TfsGitTreeDepthFirstEnumerator rootEnum = new TfsGitTreeDepthFirstEnumerator(rootTree);
      if ((rootEnum == null ? 0 : (rootEnum.SkipEntriesUntilPath(normalizedGitPath, gitObjectType) ? 1 : 0)) != 0)
        return GitItemUtility.GetAllTreeEntriesFromPath(rootEnum, normPath, normalizedGitPath, gitObjectType, skipFirstEntry);
      if (continuationToken == null)
        return (IEnumerable<GitItem>) new List<GitItem>();
      throw new InvalidArgumentValueException(Resources.Format("InvalidContinuationToken", (object) continuationToken.ToString()));
    }

    private static IEnumerable<GitItem> GetAllTreeEntriesFromPath(
      TfsGitTreeDepthFirstEnumerator rootEnum,
      NormalizedGitPath scopePath,
      NormalizedGitPath startingItemPath,
      GitObjectType startingItemObjectType,
      bool skipFirstEntry)
    {
      bool flag = rootEnum != null;
      int scopeDepth = scopePath.Depth;
      for (; flag; flag = rootEnum.MoveNext())
      {
        TreeEntryAndPath current = rootEnum.Current;
        if (skipFirstEntry)
        {
          skipFirstEntry = false;
        }
        else
        {
          if (rootEnum.CurrentDepth <= scopeDepth && startingItemPath != new NormalizedGitPath(current.RelativePath))
            break;
          GitItem gitItem = new GitItem();
          gitItem.Path = current.RelativePath;
          gitItem.ObjectId = current.Entry.ObjectId.ToString();
          gitItem.GitObjectType = current.Entry.ObjectType;
          yield return gitItem;
        }
      }
    }
  }
}
