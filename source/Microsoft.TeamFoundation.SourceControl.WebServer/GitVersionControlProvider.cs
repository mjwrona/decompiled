// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitVersionControlProvider
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer.Legacy;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Redis;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitVersionControlProvider : VersionControlProvider
  {
    private const string c_tagsPrefix = "refs/tags/";
    private const string c_headsPrefix = "refs/heads/";
    private const string c_gitModulesFileName = ".gitmodules";
    private const long c_maxAnnotateBytes = 5242880;
    private const string c_userDefaultBranchSettingPrefix = "Git/DefaultUserBranch";
    private const string c_importRequestCacheKey = "_GitActiveImportRequest";
    private static readonly char[] c_pathSeparators = new char[2]
    {
      '/',
      '\\'
    };
    private const int c_maxCommitsForAuthorCountsQuery = 1000;
    private static readonly Regex s_commitRegex = new Regex("^[0-9a-fA-F]{6}[0-9a-fA-F]*$", RegexOptions.Compiled);
    private readonly IEnumerable<ITfsLinkedContentResolver> m_linkedContentResolvers;
    private readonly Guid c_commitItemNamespaceId = new Guid("3154815c-8854-4aff-80fa-f993c6b41ee6");
    private readonly Guid c_lastChangeNamespaceId = new Guid("4c621c01-d477-4df2-9155-b972b57ac137");
    private static readonly string c_CommitItemsExpiry = "/Configuration/Git/CommitItemsExpirySeconds";
    private static readonly RegistryQuery c_CommitItemsCacheExpiryQuery = new RegistryQuery(GitVersionControlProvider.c_CommitItemsExpiry, false);

    public GitVersionControlProvider(TfsWebContext webContext, ITfsGitRepository repository)
      : base(webContext.TfsRequestContext, repository.Name)
    {
      this.Repository = repository;
      this.m_linkedContentResolvers = GitLinkedContentUtility.GetTfsLinkedContentResolvers(webContext.TfsRequestContext, true);
    }

    public GitVersionControlProvider(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
      : base(requestContext, repository.Name)
    {
      this.Repository = repository;
      this.m_linkedContentResolvers = GitLinkedContentUtility.GetTfsLinkedContentResolvers(requestContext, true);
    }

    public ITfsGitRepository Repository { get; private set; }

    public GitRepository GetRepository(bool getDefaultBranch, bool includeParent = false) => this.Repository.ToWebApiItem(this.RequestContext, this.RequestContext.GetService<IRequestProjectService>().GetProject(this.RequestContext), getDefaultBranch, includeParent: this.Repository.IsFork & includeParent);

    public string GetUserDefaultBranchName() => this.RequestContext.GetService<ISettingsService>().GetValue<string>(this.RequestContext, SettingsUserScope.User, "Repository", this.Repository.Key.RepoId.ToString(), "Git/DefaultUserBranch", (string) null);

    private Sha1Id? GetBranchByName(string branchName) => this.Repository.Refs.MatchingName("refs/heads/" + branchName)?.ObjectId;

    private Sha1Id? GetTagByName(string tagName) => this.Repository.Refs.MatchingName("refs/tags/" + tagName)?.ObjectId;

    public override Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      string path,
      string version,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions details)
    {
      return this.GetItem(ref path, version, details, out TfsGitCommit _);
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      ref string path,
      string version,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions details,
      out TfsGitCommit commit)
    {
      path = path.Trim('/');
      string versionDescription;
      this.TryGetCommitFromVersion(ref path, version, out versionDescription, out commit);
      if (commit == null)
        throw new GitUnresolvableToCommitException(this.getVersionDescriptorString(version, GitVersionType.Commit), this.Repository.Name);
      return this.GetItem(ref path, version, versionDescription, details, commit);
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel GetItem(
      ref string path,
      string version,
      string versionDescription,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions details,
      TfsGitCommit commit)
    {
      TfsGitTreeEntry treeEntry = (TfsGitTreeEntry) null;
      TfsGitTree tree1 = commit.GetTree();
      TfsGitObject tree2 = string.IsNullOrEmpty(path) ? (TfsGitObject) tree1 : this.FindMember(tree1, ref path, out treeEntry);
      if (treeEntry != null && treeEntry.ObjectType == Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Commit)
      {
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem gitItem = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(path, treeEntry.ObjectId.ToByteArray(), LegacyGitModelExtensions.ConvertTfsGitObjectType(treeEntry.ObjectType), version, (int) treeEntry.Mode);
        gitItem.ContentMetadata = this.CreateSubmoduleFileContentMetadata();
        return (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) gitItem;
      }
      if (tree2 == null)
        throw new ItemNotFoundException(path);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem gitItem1 = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(path, tree2.ObjectId.ToByteArray(), LegacyGitModelExtensions.ConvertTfsGitObjectType(tree2.ObjectType), version, treeEntry == null ? 0 : (int) treeEntry.Mode);
      if (details.IncludeContentMetadata)
      {
        int encoding = 0;
        if (!gitItem1.IsFolder)
          encoding = this.TryDetectFileEncoding((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) gitItem1, 0, details.ScanBytesForEncoding);
        gitItem1.ContentMetadata = this.GetFileContentMetadata(path, gitItem1.IsFolder, encoding);
        if (!gitItem1.IsFolder)
          gitItem1.ContentMetadata.VisualStudioWebLink = this.GetVisualStudioWebLink(path, version);
      }
      if (details.RecursionLevel != Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.None && tree2 is TfsGitTree)
        this.PopulateChildItems(gitItem1, (TfsGitTree) tree2, details.RecursionLevel);
      gitItem1.VersionDescription = versionDescription;
      return (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) gitItem1;
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContentMetadata CreateSubmoduleFileContentMetadata() => new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContentMetadata()
    {
      IsBinary = false,
      IsImage = false,
      ContentType = "application/octet-stream"
    };

    public string GetVisualStudioWebLinkForRepository() => VisualStudioLinkingUtility.GetRepositoryLink(this.RequestContext, ProjectInfo.GetProjectId(this.Repository.Key.GetProjectUri()), this.Repository.Key.RepoId);

    public string GetVisualStudioWebLink(string path, string version)
    {
      Guid projectId = ProjectInfo.GetProjectId(this.Repository.Key.GetProjectUri());
      ArtifactId artifactId = new ArtifactId("Git", "LaunchLatestVersionedItem", string.Format("/{{{0}}}/{1}", (object) this.Repository.Key.RepoId, (object) path));
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(version) && version.StartsWith("GB", StringComparison.OrdinalIgnoreCase))
        queryParameters["branch"] = version.Substring(2);
      return VisualStudioLinkingUtility.GetProjectArtifactLink(this.RequestContext, projectId, artifactId, (IDictionary<string, string>) queryParameters);
    }

    private void PopulateChildItems(
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem folder,
      TfsGitTree tree,
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType recursionType)
    {
      folder.ChildItems = (IList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>) new List<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel>();
      foreach (TfsGitTreeEntry treeEntry in tree.GetTreeEntries())
      {
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem folder1 = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(folder.ServerItem + "/" + treeEntry.Name, treeEntry.ObjectId.ToByteArray(), LegacyGitModelExtensions.ConvertTfsGitObjectType(treeEntry.ObjectType), folder.VersionString, (int) treeEntry.Mode);
        folder.ChildItems.Add((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) folder1);
        if (recursionType == Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.VersionControlRecursionType.Full && treeEntry.ObjectType == Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Tree)
          this.PopulateChildItems(folder1, (TfsGitTree) treeEntry.Object, recursionType);
      }
    }

    public override Stream GetFileContentStream(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file) => this.GetFileContentStreamWithMetadata(file).Stream;

    public override StoredFile GetFileContentStreamWithMetadata(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem gitItem = (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem) file;
      if (gitItem.GitObjectType == Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Commit)
        return new StoredFile((Stream) new MemoryStream(Encoding.UTF8.GetBytes(gitItem.ObjectId.Full)), gitItem.ObjectId.ObjectId);
      TfsGitBlob var = this.LookupObject<TfsGitBlob>(new Sha1Id(gitItem.ObjectId.ObjectId));
      ArgumentUtility.CheckForNull<TfsGitBlob>(var, "gitObject");
      return new StoredFile(GitLinkedContentUtility.Resolve(this.Repository, var.GetContent(), this.m_linkedContentResolvers), var.ObjectId.ToByteArray());
    }

    public override byte[] GetFileHashValueNoContent(Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel file)
    {
      TfsGitBlob var = this.LookupObject<TfsGitBlob>(new Sha1Id(((Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem) file).ObjectId.ObjectId));
      ArgumentUtility.CheckForNull<TfsGitBlob>(var, "gitObject");
      return var.ObjectId.ToByteArray();
    }

    private TfsGitObject FindMember(
      TfsGitTree tree,
      ref string path,
      out TfsGitTreeEntry treeEntry)
    {
      TfsGitObject member = (TfsGitObject) null;
      treeEntry = (TfsGitTreeEntry) null;
      foreach (char cPathSeparator in GitVersionControlProvider.c_pathSeparators)
      {
        member = this.FindMember(tree, ref path, ref treeEntry, cPathSeparator);
        if (member != null)
          break;
      }
      return member;
    }

    private TfsGitObject FindMember(
      TfsGitTree tree,
      ref string path,
      ref TfsGitTreeEntry treeEntry,
      char pathSeparator)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      string str = (string) null;
      string path1 = (string) null;
      TfsGitObject tree1 = (TfsGitObject) null;
      int num;
      do
      {
        num = path.IndexOf(pathSeparator);
        if (num == 0)
          path = path.Substring(1);
        else if (num < 0)
          str = path;
        else if (num == path.Length - 1)
        {
          path = path.Substring(0, path.Length - 1);
          num = 0;
        }
        else
        {
          str = path.Substring(0, num);
          path1 = path.Substring(num);
        }
      }
      while (num == 0);
      List<TfsGitTreeEntry> tfsGitTreeEntryList = new List<TfsGitTreeEntry>();
      foreach (TfsGitTreeEntry treeEntry1 in tree.GetTreeEntries())
      {
        if (treeEntry1.Name.Equals(str, StringComparison.Ordinal))
        {
          if (treeEntry1.ObjectType != Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Commit)
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
        if (tfsGitTreeEntry.ObjectType == Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Commit)
        {
          path = tfsGitTreeEntry.Name;
        }
        else
        {
          tree1 = tfsGitTreeEntry.Object;
          str = tfsGitTreeEntry.Name;
        }
        treeEntry = tfsGitTreeEntry;
      }
      if (tree1 != null)
      {
        if (path1 == null)
        {
          path = str;
          return tree1;
        }
        if (tree1 is TfsGitTree && path1 != null)
        {
          TfsGitObject member = this.FindMember((TfsGitTree) tree1, ref path1, out treeEntry);
          path = str + "/" + path1;
          return member;
        }
      }
      return (TfsGitObject) null;
    }

    public override HistoryQueryResults QueryHistory(ChangeListSearchCriteria searchCriteria)
    {
      string itemPath1 = searchCriteria.ItemPath;
      bool flag = true;
      TfsGitCommit commit;
      if (!string.IsNullOrEmpty(itemPath1) && !string.Equals(searchCriteria.ItemPath, "/", StringComparison.OrdinalIgnoreCase))
      {
        flag = this.GetItem(ref itemPath1, searchCriteria.ItemVersion, new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemDetailsOptions(), out commit).IsFolder;
        searchCriteria.ItemPath = "/" + itemPath1;
      }
      else
      {
        commit = this.GetCommitFromVersion(searchCriteria.ItemVersion);
        searchCriteria.ItemPath = (string) null;
      }
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitHistoryQueryResults historyQueryResults = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitHistoryQueryResults();
      historyQueryResults.StartingCommitId = commit.ObjectId.ToString();
      List<HistoryEntry> historyEntryList = new List<HistoryEntry>();
      historyQueryResults.Results = (IEnumerable<HistoryEntry>) historyEntryList;
      ITeamFoundationGitCommitService service = this.RequestContext.GetService<ITeamFoundationGitCommitService>();
      int valueOrDefault = searchCriteria.Top.GetValueOrDefault(10000);
      if (valueOrDefault == int.MaxValue)
        --valueOrDefault;
      Sha1Id? nullable1 = new Sha1Id?();
      Sha1Id? nullable2 = new Sha1Id?();
      Sha1Id? nullable3 = new Sha1Id?();
      if (!string.IsNullOrEmpty(searchCriteria.FromVersion))
        nullable1 = new Sha1Id?(new Sha1Id(searchCriteria.FromVersion));
      if (!string.IsNullOrEmpty(searchCriteria.ToVersion))
        nullable2 = new Sha1Id?(new Sha1Id(searchCriteria.ToVersion));
      if (!string.IsNullOrEmpty(searchCriteria.CompareVersion))
        nullable3 = new Sha1Id?(this.GetCommitFromVersion(searchCriteria.CompareVersion).ObjectId);
      IVssRequestContext requestContext = this.RequestContext;
      ITfsGitRepository repository = this.Repository;
      Sha1Id? commitId = new Sha1Id?(commit.ObjectId);
      string itemPath2 = searchCriteria.ItemPath;
      int num1 = flag ? 1 : 0;
      int num2 = searchCriteria.ExcludeDeletes ? 1 : 0;
      string user = searchCriteria.User;
      DateTime? fromDate = this.RequestContext.ParseFromDate(searchCriteria.FromDate);
      DateTime? toDate = this.RequestContext.ParseToDate(searchCriteria.ToDate);
      Sha1Id? fromCommitId = nullable1;
      Sha1Id? toCommitId = nullable2;
      Sha1Id? compareCommitId = nullable3;
      int? skip = searchCriteria.Skip;
      int? maxItemCount = new int?(valueOrDefault + 1);
      IEnumerable<TfsGitCommitHistoryEntry> commitHistoryEntries = service.QueryCommitHistory(requestContext, repository, commitId, itemPath2, num1 != 0, num2 != 0, user, fromDate: fromDate, toDate: toDate, fromCommitId: fromCommitId, toCommitId: toCommitId, compareCommitId: compareCommitId, skip: skip, maxItemCount: maxItemCount);
      ++historyQueryResults.UnpopulatedCount;
      int num3 = 0;
      foreach (TfsGitCommitHistoryEntry commitHistoryEntry in commitHistoryEntries)
      {
        if (num3 == valueOrDefault)
        {
          historyQueryResults.MoreResultsAvailable = true;
          break;
        }
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit legacyGitCommit = commitHistoryEntry.Commit.ToLegacyGitCommit(commitHistoryEntry.ChangeCounts);
        HistoryEntry historyEntry = new HistoryEntry()
        {
          ChangeList = (ChangeList) legacyGitCommit
        };
        if (commitHistoryEntry.Change != null)
        {
          historyEntry.ItemChangeType = LegacyGitModelExtensions.ConvertChangeType(commitHistoryEntry.Change.ChangeType);
          historyEntry.ServerItem = commitHistoryEntry.Change.ParentPath + commitHistoryEntry.Change.ChildItem;
        }
        historyEntryList.Add(historyEntry);
        ++num3;
      }
      return (HistoryQueryResults) historyQueryResults;
    }

    public override ChangeList GetChangeList(string version, int maxNumberOfChanges) => this.GetChangeList(version, maxNumberOfChanges, string.Empty);

    public virtual ChangeList GetChangeList(
      string version,
      int maxNumberOfChanges,
      string filePath)
    {
      TfsGitCommit commitFromVersion = this.GetCommitFromVersion(version);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit legacyGitCommit = this.ToLegacyGitCommit(commitFromVersion);
      CommitMetadataAndChanges metadataAndChanges = this.GetCommitMetadataAndChanges(commitFromVersion.ObjectId);
      TfsGitPushMetadata gitPushMetadata = this.GetGitPushMetadata(metadataAndChanges);
      if (gitPushMetadata != null)
      {
        legacyGitCommit.PushId = gitPushMetadata.PushId;
        legacyGitCommit.PushTime = gitPushMetadata.PushTime;
        IdentityRef pusherIdentity = this.GetPusherIdentity(gitPushMetadata.PusherId);
        legacyGitCommit.PushedByDisplayName = pusherIdentity != null ? pusherIdentity.DisplayName : "DisplayNameForInvalidIdentity";
      }
      if (maxNumberOfChanges > 0)
      {
        bool allChangesIncluded;
        legacyGitCommit.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) this.GetChangeListChangesInternalLegacy((IEnumerable<TfsGitCommitChangeWithId>) metadataAndChanges.Changes, maxNumberOfChanges, 0, this.IsMergeCommit(commitFromVersion), filePath, out allChangesIncluded);
        legacyGitCommit.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers.ComputeChangeCounts(legacyGitCommit.Changes);
        legacyGitCommit.AllChangesIncluded = allChangesIncluded;
      }
      this.SetSecuredObject((VersionControlSecuredObject) legacyGitCommit);
      return (ChangeList) legacyGitCommit;
    }

    public virtual Microsoft.TeamFoundation.SourceControl.WebApi.GitCommit GetGitCommit(
      string version,
      int maxNumberOfChanges,
      string filePath,
      out bool allChangesIncluded)
    {
      TfsGitCommit commitFromVersion = this.GetCommitFromVersion(version);
      Microsoft.TeamFoundation.SourceControl.WebApi.GitCommit gitCommitInternal = this.GetGitCommitInternal(version, commitFromVersion);
      allChangesIncluded = true;
      CommitMetadataAndChanges metadataAndChanges = this.GetCommitMetadataAndChanges(commitFromVersion.ObjectId);
      TfsGitPushMetadata gitPushMetadata = this.GetGitPushMetadata(metadataAndChanges);
      if (gitPushMetadata != null)
        gitCommitInternal.Push = new GitPushRef()
        {
          PushId = gitPushMetadata.PushId,
          Date = gitPushMetadata.PushTime,
          PushedBy = this.GetPusherIdentity(gitPushMetadata.PusherId)
        };
      gitCommitInternal.CommitTooManyChanges = metadataAndChanges.TooManyChanges;
      if (maxNumberOfChanges > 0)
      {
        gitCommitInternal.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitChange>) this.GetChangeListChangesInternal((IEnumerable<TfsGitCommitChangeWithId>) metadataAndChanges.Changes, maxNumberOfChanges, 0, this.IsMergeCommit(commitFromVersion), filePath, out allChangesIncluded);
        gitCommitInternal.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.ChangeListHelpers.ComputeChangeCounts(gitCommitInternal.Changes) as ChangeCountDictionary;
      }
      return gitCommitInternal;
    }

    protected internal virtual Microsoft.TeamFoundation.SourceControl.WebApi.GitCommit GetGitCommitInternal(
      string version,
      TfsGitCommit commit)
    {
      return new GitCommitTranslator(this.RequestContext, this.Repository.Key).ToGitCommit(GitVersionParser.GetCommitById(this.Repository, commit.ObjectId), GitSecuredObjectFactory.CreateRepositoryReadOnly(this.Repository.Key), true);
    }

    private CommitMetadataAndChanges GetCommitMetadataAndChanges(Sha1Id objectId) => this.RequestContext.GetService<ITeamFoundationGitCommitService>().GetCommitManifest(this.RequestContext, this.Repository, objectId, true) ?? throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) objectId.ToString())).Expected(this.RequestContext.ServiceName);

    private TfsGitPushMetadata GetGitPushMetadata(CommitMetadataAndChanges metadata)
    {
      if (!metadata.PushId.HasValue)
        return (TfsGitPushMetadata) null;
      IReadOnlyList<TfsGitPushMetadata> pushDataForPushIds = this.RequestContext.GetService<ITeamFoundationGitCommitService>().GetPushDataForPushIds(this.RequestContext, this.Repository.Key, new int[1]
      {
        metadata.PushId.Value
      });
      return pushDataForPushIds.Count != 0 ? pushDataForPushIds[0] : throw new InvalidDataException(string.Format("ErrorPushNotFound", (object) metadata.PushId));
    }

    private IdentityRef GetPusherIdentity(Guid pusherId)
    {
      string empty = string.Empty;
      IdentityRef pusherIdentity = (IdentityRef) null;
      TeamFoundationIdentity[] foundationIdentityArray = this.RequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(this.RequestContext, new Guid[1]
      {
        pusherId
      });
      if (foundationIdentityArray != null && foundationIdentityArray.Length != 0 && foundationIdentityArray[0] != null)
      {
        pusherIdentity = foundationIdentityArray[0].ToIdentityRef(this.RequestContext);
        pusherIdentity.DisplayName = pusherIdentity.DisplayName ?? string.Empty;
      }
      return pusherIdentity;
    }

    public override ChangeQueryResults GetChangeListChanges(
      string version,
      int maxNumberOfChanges,
      int skipCount)
    {
      TfsGitCommit commitFromVersion = this.GetCommitFromVersion(version);
      Sha1Id objectId = commitFromVersion.ObjectId;
      bool allChangesIncluded;
      ChangeQueryResults securableObject = new ChangeQueryResults()
      {
        Results = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) this.GetChangeListChangesInternalLegacy((IEnumerable<TfsGitCommitChangeWithId>) (this.RequestContext.GetService<ITeamFoundationGitCommitService>().GetCommitManifest(this.RequestContext, this.Repository, objectId) ?? throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) objectId.ToString())).Expected(this.RequestContext.ServiceName)).Changes, maxNumberOfChanges, skipCount, this.IsMergeCommit(commitFromVersion), (string) null, out allChangesIncluded),
        MoreResultsAvailable = !allChangesIncluded
      };
      securableObject.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers.ComputeChangeCounts(securableObject.Results);
      this.SetSecuredObject((VersionControlSecuredObject) securableObject);
      return securableObject;
    }

    public List<GitItemMetadata> GetCommitItems(string version, string path) => this.ComputeLatestChanges(this.GetCommitFromVersion(version), path);

    public GitLastChangeTreeItems GetLastChangeTreeItems(
      string version,
      string path,
      bool allowPartial,
      bool includeCommits)
    {
      TfsGitCommit commit = this.GetCommitFromVersion(version);
      string cacheSubKey = MinimalGitLastChangeTreeItems.GetCacheSubKey(this.Repository);
      allowPartial = false;
      if (this.CanMemoizeQueries() & allowPartial)
      {
        string memoizedKey = this.GetMemoizedKey(commit, path, cacheSubKey, includeCommits: includeCommits);
        MinimalGitLastChangeTreeItems minimal;
        if (this.GetMemoizedCacheContainer<MinimalGitLastChangeTreeItems>("Git.GetLastChangeTreeItems.Minimal", this.c_lastChangeNamespaceId, MinimalGitLastChangeTreeItems.ValueSerializer).TryGet<string, MinimalGitLastChangeTreeItems>(this.RequestContext, memoizedKey, out minimal))
          return this.CreateFullLastChangeTreeItems(minimal, commit.ObjectId, path, includeCommits);
      }
      return this.CreateFullLastChangeTreeItems(this.MemoizedRunQuery<MinimalGitLastChangeTreeItems>(this.GetMemoizedKey(commit, path, cacheSubKey, allowPartial, includeCommits), "Git.GetLastChangeTreeItems.Minimal", this.c_lastChangeNamespaceId, (Func<string, MinimalGitLastChangeTreeItems>) (_ => new MinimalGitLastChangeTreeItems(this.Repository, LatestChangeAlgorithms.GetLatestChanges(this.RequestContext, this.Repository, commit.ObjectId, path, allowPartial ? 0.5 : 1.0, 1000))), MinimalGitLastChangeTreeItems.ValueSerializer), commit.ObjectId, path, includeCommits);
    }

    private GitLastChangeTreeItems CreateFullLastChangeTreeItems(
      MinimalGitLastChangeTreeItems minimal,
      Sha1Id commitId,
      string path,
      bool includeCommits)
    {
      (List<GitLastChangeItem> gitLastChangeItemList, DateTime? lastExploredTime) = minimal.Expand(includeCommits, this.RequestContext, this.Repository, commitId, path);
      return new GitLastChangeTreeItems()
      {
        Items = gitLastChangeItemList,
        LastExploredTime = lastExploredTime,
        Commits = includeCommits ? gitLastChangeItemList.Select<GitLastChangeItem, string>((Func<GitLastChangeItem, string>) (x => x.CommitId)).Distinct<string>().Select<string, GitCommitRef>((Func<string, GitCommitRef>) (x => this.LookupCommitRef(new Sha1Id(x)))).ToList<GitCommitRef>() : new List<GitCommitRef>()
      };
    }

    private GitCommitRef LookupCommitRef(Sha1Id commitId)
    {
      ProjectInfo project = this.RequestContext.GetService<IRequestProjectService>().GetProject(this.RequestContext);
      TfsGitCommit commit = this.Repository.LookupObject<TfsGitCommit>(commitId);
      if (project == null)
        project = this.RequestContext.GetService<IProjectService>().GetProject(this.RequestContext, this.Repository.Key.ProjectId);
      return this.GetCommitRefFromGitCommitObject(commit, project);
    }

    private GitCommitRef GetCommitRefFromGitCommitObject(TfsGitCommit commit, ProjectInfo project)
    {
      GitCommitRef gitCommitShallow = new GitCommitTranslator(this.RequestContext, this.Repository.Key).ToGitCommitShallow(commit, true);
      gitCommitShallow.RemoteUrl = WebLinksUtility.GetCommitRemoteUrl(this.RequestContext, this.RepositoryName, project.Name, gitCommitShallow.CommitId);
      return gitCommitShallow;
    }

    private List<GitItemMetadata> ComputeLatestChanges(TfsGitCommit commit, string path)
    {
      List<LatestChange> changes = LatestChangeAlgorithms.GetLatestChanges(this.RequestContext, this.Repository, commit.ObjectId, path).changes;
      path = new NormalizedGitPath(path).ToString();
      Dictionary<Sha1Id, TfsGitCommit> dictionary = new Dictionary<Sha1Id, TfsGitCommit>();
      List<GitItemMetadata> latestChanges = new List<GitItemMetadata>();
      foreach (LatestChange latestChange in changes)
      {
        TfsGitCommit tfsGitCommit;
        if (!dictionary.TryGetValue(latestChange.CommitId, out tfsGitCommit))
          dictionary[latestChange.CommitId] = tfsGitCommit = this.Repository.LookupObject<TfsGitCommit>(latestChange.CommitId);
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem gitItem1 = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem(latestChange.CommitId.ToByteArray());
        gitItem1.ServerItem = latestChange.ItemName.Length > 0 ? path + "/" + latestChange.ItemName : path;
        gitItem1.IsFolder = latestChange.IsFolder;
        gitItem1.GitObjectType = latestChange.IsFolder ? Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Tree : Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitObjectType.Blob;
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitItem gitItem2 = gitItem1;
        gitItem2.ChangeDate = tfsGitCommit.GetAuthor().Time;
        GitItemMetadata gitItemMetadata = new GitItemMetadata()
        {
          Item = gitItem2,
          Comment = tfsGitCommit.GetComment(),
          OwnerDisplayName = tfsGitCommit.GetAuthor().Name,
          Owner = tfsGitCommit.GetAuthor().Name,
          CommitId = new GitObjectId(latestChange.CommitId.ToByteArray())
        };
        latestChanges.Add(gitItemMetadata);
      }
      return latestChanges;
    }

    public virtual Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit DiffCommits(
      string baseVersion,
      string targetVersion,
      int maxNumberOfChanges,
      int skipCount)
    {
      return this.DiffCommits(baseVersion, targetVersion, maxNumberOfChanges, skipCount, (string) null);
    }

    public virtual Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit DiffCommits(
      string baseVersion,
      string targetVersion,
      int maxNumberOfChanges,
      int skipCount,
      string filePath)
    {
      ITeamFoundationGitCommitService service = this.RequestContext.GetService<ITeamFoundationGitCommitService>();
      TfsGitCommit commitFromVersion = this.GetCommitFromVersion(baseVersion);
      TfsGitCommit targetCommit = this.GetCommitFromVersion(targetVersion);
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit securableObject = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit();
      if (commitFromVersion != null && targetCommit != null && commitFromVersion.ObjectId != targetCommit.ObjectId)
      {
        if (maxNumberOfChanges > 0)
        {
          TfsGitCommit mergeBase = service.GetMergeBase(this.RequestContext, this.Repository, commitFromVersion.ObjectId, targetCommit.ObjectId);
          TfsGitTree oldTree = (TfsGitTree) null;
          TfsGitTree tree = targetCommit.GetTree();
          if (mergeBase != null)
          {
            securableObject.CommitId = new GitObjectId(mergeBase.ObjectId.ToByteArray());
            oldTree = mergeBase.GetTree();
          }
          IEnumerable<TfsGitCommitChangeWithId> commitChanges = TfsGitDiffHelper.DiffTrees(this.Repository, oldTree, tree, true).Select<TfsGitDiffEntry, TfsGitCommitChangeWithId>((Func<TfsGitDiffEntry, TfsGitCommitChangeWithId>) (x => new TfsGitCommitChangeWithId(targetCommit.ObjectId, x)));
          bool allChangesIncluded;
          securableObject.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) this.GetChangeListChangesInternalLegacy(commitChanges, maxNumberOfChanges, skipCount, false, filePath, out allChangesIncluded);
          securableObject.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers.ComputeChangeCounts(securableObject.Changes);
          securableObject.AllChangesIncluded = allChangesIncluded;
        }
      }
      else
      {
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change[] changeArray = Array.Empty<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>();
        securableObject.CommitId = new GitObjectId(commitFromVersion != null ? commitFromVersion.ObjectId.ToByteArray() : Sha1Id.Empty.ToByteArray());
        securableObject.Changes = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.Change>) changeArray;
        securableObject.ChangeCounts = Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers.ComputeChangeCounts(securableObject.Changes);
        securableObject.AllChangesIncluded = true;
      }
      this.SetSecuredObject((VersionControlSecuredObject) securableObject);
      return securableObject;
    }

    private IMutableDictionaryCacheContainer<string, T> GetMemoizedCacheContainer<T>(
      string areaName,
      Guid namespaceId,
      IValueSerializer serializer)
    {
      int num = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, in GitVersionControlProvider.c_CommitItemsCacheExpiryQuery, 900);
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = areaName,
        KeyExpiry = new TimeSpan?(TimeSpan.FromSeconds((double) num))
      };
      if (serializer != null)
        settings.ValueSerializer = serializer;
      return this.RequestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, T, GitVersionControlProvider.MemoizedCacheSecurityToken>(this.RequestContext, namespaceId, settings);
    }

    private string GetMemoizedKey(
      TfsGitCommit commit,
      string path,
      string subkey,
      bool isPartial = false,
      bool includeCommits = true)
    {
      return commit.ObjectId.ToString() + ":" + subkey + ":" + path + (isPartial ? ":partial" : "") + (includeCommits ? ":includeCommits" : "");
    }

    private T MemoizedRunQuery<T>(
      string cacheKey,
      string cacheAreaName,
      Guid cacheNamespaceId,
      Func<string, T> query,
      IValueSerializer serializer)
    {
      return this.CanMemoizeQueries() ? this.GetMemoizedCacheContainer<T>(cacheAreaName, cacheNamespaceId, serializer).Get<string, T>(this.RequestContext, cacheKey, query) : query(string.Empty);
    }

    private bool CanMemoizeQueries() => !this.RequestContext.IsFeatureEnabled("WebAccess.VersionControl.DisableCommitItemCache");

    private GitLastChangeItem ConvertToGitLastChangeItem(LatestChange latestChange) => new GitLastChangeItem()
    {
      Path = latestChange.ItemName,
      CommitId = latestChange.CommitId.ToString()
    };

    protected internal virtual bool IsMergeCommit(TfsGitCommit commit) => commit != null && commit.GetParents().Count > 1;

    protected internal virtual IList<Microsoft.TeamFoundation.SourceControl.WebApi.GitChange> GetChangeListChangesInternal(
      IEnumerable<TfsGitCommitChangeWithId> commitChanges,
      int maxNumberOfChanges,
      int skipCount,
      bool isMergeCommit,
      string filePath,
      out bool allChangesIncluded)
    {
      return (IList<Microsoft.TeamFoundation.SourceControl.WebApi.GitChange>) this.GetGitCommitChangeListInternal(commitChanges, maxNumberOfChanges, skipCount, isMergeCommit, filePath, out allChangesIncluded).Select<TfsGitCommitChangeWithId, Microsoft.TeamFoundation.SourceControl.WebApi.GitChange>((Func<TfsGitCommitChangeWithId, Microsoft.TeamFoundation.SourceControl.WebApi.GitChange>) (change => this.ConvertToGitChange(change))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.GitChange>();
    }

    protected internal virtual IList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange> GetChangeListChangesInternalLegacy(
      IEnumerable<TfsGitCommitChangeWithId> commitChanges,
      int maxNumberOfChanges,
      int skipCount,
      bool isMergeCommit,
      string filePath,
      out bool allChangesIncluded)
    {
      return (IList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange>) this.GetGitCommitChangeListInternal(commitChanges, maxNumberOfChanges, skipCount, isMergeCommit, filePath, out allChangesIncluded).Select<TfsGitCommitChangeWithId, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange>((Func<TfsGitCommitChangeWithId, Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange>) (change => this.ConvertToLegacyGitChange(change))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange>();
    }

    protected internal virtual IList<TfsGitCommitChangeWithId> GetGitCommitChangeListInternal(
      IEnumerable<TfsGitCommitChangeWithId> commitChanges,
      int maxNumberOfChanges,
      int skipCount,
      bool isMergeCommit,
      string filePath,
      out bool allChangesIncluded)
    {
      List<TfsGitCommitChangeWithId> changeListInternal = new List<TfsGitCommitChangeWithId>();
      bool fileIncluded = true;
      string childPath = string.Empty;
      string parentPath = string.Empty;
      int num1 = 0;
      int num2 = 0;
      if (!string.IsNullOrEmpty(filePath))
      {
        int num3 = filePath.LastIndexOf('/');
        childPath = filePath.Substring(num3 + 1);
        parentPath = filePath.Substring(0, num3 + 1);
        fileIncluded = false;
      }
      foreach (TfsGitCommitChangeWithId commitChange in commitChanges)
      {
        if ((commitChange.ObjectType != Microsoft.TeamFoundation.SourceControl.WebApi.GitObjectType.Tree || commitChange.ChangeType != TfsGitChangeType.Edit) && !(!commitChange.ContentChanged & isMergeCommit))
        {
          ++num1;
          if (num1 <= skipCount)
            this.UpdateFileIncluded(commitChange, childPath, parentPath, ref fileIncluded);
          else if (num1 <= skipCount + maxNumberOfChanges)
          {
            this.UpdateFileIncluded(commitChange, childPath, parentPath, ref fileIncluded);
            changeListInternal.Add(commitChange);
            ++num2;
          }
          else if (!fileIncluded)
          {
            this.UpdateFileIncluded(commitChange, childPath, parentPath, ref fileIncluded);
            if (fileIncluded)
            {
              changeListInternal.Add(commitChange);
              ++num2;
            }
          }
          else
            break;
        }
      }
      bool flag = num1 > skipCount + num2;
      allChangesIncluded = !flag;
      return (IList<TfsGitCommitChangeWithId>) changeListInternal;
    }

    protected override void SetSecuredObject(VersionControlSecuredObject securableObject)
    {
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(this.Repository.Key);
      securableObject.SetSecuredObject(repositoryReadOnly);
    }

    protected void SetSecuredObject(
      IEnumerable<VersionControlSecuredObject> securableObjects)
    {
      if (securableObjects == null)
        return;
      securableObjects.SetSecuredObject<VersionControlSecuredObject>(GitSecuredObjectFactory.CreateRepositoryReadOnly(this.Repository.Key));
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.GitChange ConvertToGitChange(
      TfsGitCommitChangeWithId change)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.GitChange gitChange = new Microsoft.TeamFoundation.SourceControl.WebApi.GitChange();
      gitChange.ChangeType = GitModelExtensions.ToVersionControlChangeType(change.ChangeType);
      string absoluteUri = this.RequestContext.GetService<ILocationService>().GetResourceUri(this.RequestContext, "git", GitWebApiConstants.ItemsLocationId, (object) new
      {
        repositoryId = this.Repository.Key.RepoId
      }).AbsoluteUri;
      gitChange.Item = change.ToGitItem(absoluteUri);
      gitChange.SourceServerItem = change.RenameSourceItemPath;
      return gitChange;
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange ConvertToLegacyGitChange(
      TfsGitCommitChangeWithId change)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange legacyGitChange = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitChange(LegacyGitModelExtensions.ConvertChangeType(change.ChangeType));
      legacyGitChange.Item = (Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ItemModel) change.ToLegacyGitItem();
      legacyGitChange.SourceServerItem = change.RenameSourceItemPath;
      return legacyGitChange;
    }

    private void UpdateFileIncluded(
      TfsGitCommitChangeWithId change,
      string childPath,
      string parentPath,
      ref bool fileIncluded)
    {
      if (fileIncluded || !(change.ChildItem == childPath) || !(change.ParentPath == parentPath))
        return;
      fileIncluded = true;
    }

    private bool TryGetCommitById(Sha1Id commitId, out TfsGitCommit commit)
    {
      commit = this.LookupObject<TfsGitCommit>(commitId);
      return commit != null;
    }

    private TfsGitCommit GetCommitById(Sha1Id commitId)
    {
      TfsGitCommit tfsGitCommit = (TfsGitCommit) null;
      TfsGitObject gitObject = this.Repository.TryLookupObject(commitId);
      if (gitObject != null)
        tfsGitCommit = gitObject.TryResolveToCommit();
      return tfsGitCommit != null ? tfsGitCommit : throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) commitId.ToString())).Expected(this.RequestContext.ServiceName);
    }

    private T LookupObject<T>(Sha1Id objectId) where T : TfsGitObject
    {
      TfsGitObject tfsGitObject = this.Repository.TryLookupObject(objectId);
      return tfsGitObject == null || !(tfsGitObject is T obj) ? default (T) : obj;
    }

    private TfsGitCommit GetCommitFromVersion(string version)
    {
      string str = (string) null;
      return this.GetCommitFromVersion(ref str, version);
    }

    private TfsGitCommit GetCommitFromVersion(ref string item, string version)
    {
      TfsGitCommit commit;
      if (this.TryGetCommitFromVersion(ref item, version, out string _, out commit))
        return commit;
      Sha1Id commitId;
      if (this.TryGetCommitIdFromVersion(version, out commitId))
        throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) commitId)).Expected(this.RequestContext.ServiceName);
      throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) version)).Expected(this.RequestContext.ServiceName);
    }

    public string GetVersionDescription(string version)
    {
      string str = (string) null;
      string versionDescription;
      if (this.TryGetCommitFromVersion(ref str, version, out versionDescription, out TfsGitCommit _))
        return versionDescription;
      Sha1Id commitId;
      if (this.TryGetCommitIdFromVersion(version, out commitId))
        throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) commitId)).Expected(this.RequestContext.ServiceName);
      throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) version)).Expected(this.RequestContext.ServiceName);
    }

    protected internal virtual Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit ToLegacyGitCommit(
      TfsGitCommit commit)
    {
      return commit.ToLegacyGitCommit(this.RequestContext);
    }

    public virtual bool TryGetCommitFromVersion(
      ref string item,
      string version,
      out string versionDescription,
      out TfsGitCommit commit)
    {
      if (string.IsNullOrEmpty(version) || string.Equals("T", version, StringComparison.OrdinalIgnoreCase))
      {
        TfsGitRef defaultOrAny = this.Repository.Refs.GetDefaultOrAny();
        string str = defaultOrAny != null ? defaultOrAny.Name : throw new ArgumentException(string.Format("ErrorNoBranchesFormat", (object) this.Repository.Name)).Expected(this.RequestContext.ServiceName);
        if (str.StartsWith("refs/heads/", StringComparison.Ordinal))
          str = str.Substring("refs/heads/".Length);
        versionDescription = str;
        return this.TryGetCommitById(defaultOrAny.ObjectId, out commit);
      }
      if (version.StartsWith("GB", StringComparison.OrdinalIgnoreCase))
      {
        string branchName = version.Substring(2);
        versionDescription = branchName;
        return this.TryGetCommitById((this.GetBranchByName(branchName) ?? throw new GitUnresolvableToCommitException(this.getVersionDescriptorString(version, GitVersionType.Branch), this.Repository.Name)).Value, out commit);
      }
      if (version.StartsWith("GT", StringComparison.OrdinalIgnoreCase))
      {
        string tagName = version.Substring(2);
        versionDescription = tagName;
        commit = this.Repository.LookupObject((this.GetTagByName(tagName) ?? throw new GitUnresolvableToCommitException(this.getVersionDescriptorString(version, GitVersionType.Tag), this.Repository.Name)).Value).ResolveToCommit();
        return true;
      }
      if (version.StartsWith("P", StringComparison.OrdinalIgnoreCase))
      {
        string versionDescription1;
        TfsGitCommit commit1;
        if (!this.TryGetCommitFromVersion(ref item, version.Substring(1), out versionDescription1, out commit1))
        {
          versionDescription = (string) null;
          commit = (TfsGitCommit) null;
          return false;
        }
        TfsGitCommit previousVersion = this.GetPreviousVersion(ref item, commit1);
        if (previousVersion == null)
        {
          versionDescription = versionDescription1;
          commit = commit1;
          return true;
        }
        versionDescription = string.Format("CommitDescriptionFormat", (object) previousVersion.ObjectId.ToAbbreviatedString());
        commit = previousVersion;
        return true;
      }
      Sha1Id commitId;
      if (!this.TryGetCommitIdFromVersion(version, out commitId))
      {
        versionDescription = (string) null;
        commit = (TfsGitCommit) null;
        return false;
      }
      versionDescription = string.Format("CommitDescriptionFormat", (object) commitId.ToAbbreviatedString());
      return this.TryGetCommitById(commitId, out commit);
    }

    private string getVersionDescriptorString(string version, GitVersionType gitVersionType) => new GitVersionDescriptor()
    {
      Version = version,
      VersionType = gitVersionType
    }.ToString();

    private TfsGitCommit GetPreviousVersion(ref string item, TfsGitCommit nextCommit)
    {
      TfsGitCommit nextCommit1 = (TfsGitCommit) null;
      ITeamFoundationGitCommitService service = this.RequestContext.GetService<ITeamFoundationGitCommitService>();
      string str;
      if (item != null)
        str = "/" + item.Trim('/');
      else
        str = "/";
      string path1 = str;
      ITeamFoundationGitCommitService gitCommitService1 = service;
      IVssRequestContext requestContext1 = this.RequestContext;
      ITfsGitRepository repository1 = this.Repository;
      Sha1Id? commitId1 = new Sha1Id?(nextCommit.ObjectId);
      string path2 = path1;
      int? nullable = new int?(2);
      DateTime? fromDate1 = new DateTime?();
      DateTime? toDate1 = new DateTime?();
      Sha1Id? fromCommitId1 = new Sha1Id?();
      Sha1Id? toCommitId1 = new Sha1Id?();
      Sha1Id? compareCommitId1 = new Sha1Id?();
      int? skip1 = new int?();
      int? maxItemCount1 = nullable;
      TfsGitCommitHistoryEntry[] array = gitCommitService1.QueryCommitHistory(requestContext1, repository1, commitId1, path2, false, fromDate: fromDate1, toDate: toDate1, fromCommitId: fromCommitId1, toCommitId: toCommitId1, compareCommitId: compareCommitId1, skip: skip1, maxItemCount: maxItemCount1).ToArray<TfsGitCommitHistoryEntry>();
      if (array.Length != 0 && array[0].Change != null && !string.IsNullOrEmpty(array[0].Change.RenameSourceItemPath))
      {
        item = array[0].Change.RenameSourceItemPath.Trim('/');
        ITeamFoundationGitCommitService gitCommitService2 = service;
        IVssRequestContext requestContext2 = this.RequestContext;
        ITfsGitRepository repository2 = this.Repository;
        Sha1Id? commitId2 = new Sha1Id?(nextCommit.ObjectId);
        string renameSourceItemPath = array[0].Change.RenameSourceItemPath;
        nullable = new int?(1);
        DateTime? fromDate2 = new DateTime?();
        DateTime? toDate2 = new DateTime?();
        Sha1Id? fromCommitId2 = new Sha1Id?();
        Sha1Id? toCommitId2 = new Sha1Id?();
        Sha1Id? compareCommitId2 = new Sha1Id?();
        int? skip2 = new int?();
        int? maxItemCount2 = nullable;
        TfsGitCommitHistoryEntry commitHistoryEntry = gitCommitService2.QueryCommitHistory(requestContext2, repository2, commitId2, renameSourceItemPath, false, true, fromDate: fromDate2, toDate: toDate2, fromCommitId: fromCommitId2, toCommitId: toCommitId2, compareCommitId: compareCommitId2, skip: skip2, maxItemCount: maxItemCount2).FirstOrDefault<TfsGitCommitHistoryEntry>();
        nextCommit1 = commitHistoryEntry == null ? nextCommit.GetParents().FirstOrDefault<TfsGitCommit>() : this.LookupObject<TfsGitCommit>(commitHistoryEntry.Commit.CommitId);
      }
      else if (array.Length == 2)
      {
        nextCommit1 = this.LookupObject<TfsGitCommit>(array[1].Commit.CommitId);
        if (array[1].Change != null && array[1].Change.ChangeType.HasFlag((Enum) TfsGitChangeType.Delete))
          nextCommit1 = this.GetPreviousVersion(ref item, nextCommit1);
      }
      else if (array.Length == 0 || array[0].Change == null)
      {
        TfsGitCommit tfsGitCommit = nextCommit.GetParents().FirstOrDefault<TfsGitCommit>();
        if (tfsGitCommit != null && this.FindMember(tfsGitCommit.GetTree(), ref path1, out TfsGitTreeEntry _) != null)
          nextCommit1 = tfsGitCommit;
      }
      else if (array[0].Change == null || !array[0].Change.ChangeType.HasFlag((Enum) TfsGitChangeType.Add))
        nextCommit1 = nextCommit.GetParents().FirstOrDefault<TfsGitCommit>();
      return nextCommit1;
    }

    private string ParseCommitIdFromVersion(string version)
    {
      string commitIdFromVersion = (string) null;
      if (version.StartsWith("GC", StringComparison.OrdinalIgnoreCase))
        commitIdFromVersion = version.Substring(2);
      else if (GitVersionControlProvider.s_commitRegex.IsMatch(version))
        commitIdFromVersion = version;
      return commitIdFromVersion;
    }

    private Sha1Id GetCommitIdFromVersion(string version)
    {
      string commitIdFromVersion = this.ParseCommitIdFromVersion(version);
      return !string.IsNullOrEmpty(commitIdFromVersion) ? new Sha1Id(commitIdFromVersion) : throw new ArgumentException(string.Format("ErrorInvalidGitVersionSpec", (object) version)).Expected(this.RequestContext.ServiceName);
    }

    private bool TryGetCommitIdFromVersion(string version, out Sha1Id commitId)
    {
      string commitIdFromVersion = this.ParseCommitIdFromVersion(version);
      if (!string.IsNullOrEmpty(commitIdFromVersion))
        return Sha1Id.TryParse(commitIdFromVersion, out commitId);
      commitId = new Sha1Id();
      return false;
    }

    public Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest GetActiveImportRequest()
    {
      if (!this.RequestContext.RootContext.Items.ContainsKey("_GitActiveImportRequest"))
      {
        List<Microsoft.TeamFoundation.Git.Server.GitImportRequest> list1 = this.RequestContext.GetService<ITeamFoundationGitImportService>().QueryImportRequests(this.RequestContext, this.Repository, false).Where<Microsoft.TeamFoundation.Git.Server.GitImportRequest>((Func<Microsoft.TeamFoundation.Git.Server.GitImportRequest, bool>) (x => x.Status != GitAsyncOperationStatus.Completed)).ToList<Microsoft.TeamFoundation.Git.Server.GitImportRequest>();
        IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest> list2 = (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>) list1.Select<Microsoft.TeamFoundation.Git.Server.GitImportRequest, Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>((Func<Microsoft.TeamFoundation.Git.Server.GitImportRequest, Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>) (x => x.ToWebApiImportItem(this.Repository, this.RequestContext, (UrlHelper) null))).ToList<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>();
        Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest gitImportRequest = (Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest) null;
        if (list1.Any<Microsoft.TeamFoundation.Git.Server.GitImportRequest>())
          gitImportRequest = list2.FirstOrDefault<Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest>();
        this.RequestContext.RootContext.Items.Add("_GitActiveImportRequest", (object) gitImportRequest);
      }
      return (Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest) this.RequestContext.RootContext.Items["_GitActiveImportRequest"];
    }

    public string GetDefaultBranchName(bool preferUserLastSelected, string repoDefaultBranch = null)
    {
      string defaultBranchName = (string) null;
      if (preferUserLastSelected)
        defaultBranchName = this.GetUserDefaultBranchName();
      if (string.IsNullOrEmpty(defaultBranchName))
      {
        defaultBranchName = repoDefaultBranch ?? GitRequestContextCacheUtil.GetDefaultRefOrAny(this.RequestContext, this.Repository)?.Name;
        if (!string.IsNullOrEmpty(defaultBranchName) && defaultBranchName.StartsWith("refs/heads/", StringComparison.Ordinal))
          defaultBranchName = defaultBranchName.Substring("refs/heads/".Length);
      }
      return defaultBranchName;
    }

    public List<GitBranchDiff> GetBranchDiffModels(
      string baseVersion,
      IEnumerable<string> versionsToDiff)
    {
      string str1 = (string) null;
      string versionDescription;
      TfsGitCommit commit;
      if (!this.TryGetCommitFromVersion(ref str1, baseVersion, out versionDescription, out commit))
        throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) baseVersion)).Expected(this.RequestContext.ServiceName);
      HashSet<string> stringSet = new HashSet<string>(versionsToDiff);
      HashSet<string> source = new HashSet<string>();
      Dictionary<TfsGitRef, string> dictionary1 = new Dictionary<TfsGitRef, string>();
      string b = string.Format("{0}{1}", (object) "refs/heads/", (object) versionDescription);
      TfsGitRef tfsGitRef = (TfsGitRef) null;
      foreach (TfsGitRef allRefHead in (IEnumerable<TfsGitRef>) this.Repository.Refs.AllRefHeads())
      {
        if (string.Equals(allRefHead.Name, b, StringComparison.Ordinal))
          tfsGitRef = allRefHead;
        else if (stringSet.Contains(allRefHead.Name))
        {
          string str2 = allRefHead.ObjectId.ToString();
          source.Add(str2);
          dictionary1.Add(allRefHead, str2);
        }
      }
      List<GitBranchDiff> branchDiffModels = new List<GitBranchDiff>();
      if (tfsGitRef != null)
      {
        Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit legacyGitCommit = commit.ToLegacyGitCommit(this.RequestContext);
        branchDiffModels.Add(new GitBranchDiff(legacyGitCommit, tfsGitRef.Name, tfsGitRef.GetIdentityRef(this.RequestContext)));
      }
      if (source.Count > 0)
      {
        Dictionary<string, TfsGitCommitLineageDiff> dictionary2 = this.RequestContext.GetService<ITeamFoundationGitCommitService>().DiffCommitLineagesAgainstCommits(this.RequestContext, this.Repository, commit.ObjectId, source.Select<string, Sha1Id>((Func<string, Sha1Id>) (c => new Sha1Id(c)))).ToDictionary<TfsGitCommitLineageDiff, string>((Func<TfsGitCommitLineageDiff, string>) (x => x.Metadata.CommitId.ToString()));
        foreach (KeyValuePair<TfsGitRef, string> keyValuePair in dictionary1)
        {
          TfsGitRef key1 = keyValuePair.Key;
          string key2 = keyValuePair.Value;
          TfsGitCommitLineageDiff commitLineageDiff;
          if (dictionary2.TryGetValue(key2, out commitLineageDiff))
          {
            TfsGitCommitLineageDiff lineageDiff = new TfsGitCommitLineageDiff(commitLineageDiff.Metadata, commitLineageDiff.BehindCount, commitLineageDiff.AheadCount, key1.Name);
            branchDiffModels.Add(lineageDiff.ToLegacyBranchDiff(key1.GetIdentityRef(this.RequestContext)));
          }
        }
      }
      return branchDiffModels;
    }

    public List<GitAnnotateBatchResult> GetAnnotateDiffModels(string path, string[] versions)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(this.RequestContext, "GitVersionControlProvider.GetAnnotateDiffModels"))
      {
        performanceTimer.AddProperty("VersionCount", (object) versions.Length);
        List<GitAnnotateBatchResult> securableObjects = new List<GitAnnotateBatchResult>();
        string path1 = path.Trim('/');
        FileDiffParameters diffParameters = new FileDiffParameters()
        {
          IgnoreTrimmedWhitespace = new bool?(true),
          LineNumbersOnly = true
        };
        foreach (string version in versions)
        {
          GitAnnotateBatchResult annotateBatchResult1 = new GitAnnotateBatchResult();
          annotateBatchResult1.Diffs = (IList<GitAnnotateResult>) new List<GitAnnotateResult>();
          securableObjects.Add(annotateBatchResult1);
          TfsGitCommit commit;
          TfsGitTreeEntry treeEntry;
          TfsGitObject commitItem = this.TryGetCommitFromVersion(ref path1, version, out string _, out commit) ? this.FindMember(commit.GetTree(), ref path1, out treeEntry) : throw new ArgumentException(string.Format("VCErrorCommitNotFound", (object) version)).Expected(this.RequestContext.ServiceName);
          if (commitItem != null)
          {
            GitAnnotateBatchResult annotateBatchResult2 = annotateBatchResult1;
            Sha1Id objectId = commitItem.ObjectId;
            string str1 = objectId.ToString();
            annotateBatchResult2.ModifiedObjectId = str1;
            foreach (TfsGitCommit parent in (IEnumerable<TfsGitCommit>) commit.GetParents())
            {
              TfsGitObject parentCommitItem = this.FindMember(parent.GetTree(), ref path1, out treeEntry);
              if (parentCommitItem != null)
              {
                GitAnnotateResult gitAnnotateResult1 = new GitAnnotateResult();
                GitAnnotateResult gitAnnotateResult2 = gitAnnotateResult1;
                objectId = parentCommitItem.ObjectId;
                string str2 = objectId.ToString();
                gitAnnotateResult2.OriginalObjectId = str2;
                gitAnnotateResult1.Diff = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff();
                this.PopulateFileDiffModelBlocks(gitAnnotateResult1.Diff, diffParameters, (Func<StoredFile>) (() => new StoredFile(parentCommitItem.GetContent(), parentCommitItem.ObjectId.ToByteArray())), (Func<StoredFile>) (() => new StoredFile(commitItem.GetContent(), commitItem.ObjectId.ToByteArray())), Encoding.Default.CodePage, Encoding.Default.CodePage, new long?(5242880L));
                annotateBatchResult1.Diffs.Add(gitAnnotateResult1);
              }
            }
            if (annotateBatchResult1.Diffs.Count == 0)
            {
              GitAnnotateResult gitAnnotateResult = new GitAnnotateResult();
              gitAnnotateResult.Diff = new Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff();
              this.PopulateFileDiffModelBlocks(gitAnnotateResult.Diff, diffParameters, (Func<StoredFile>) (() => (StoredFile) null), (Func<StoredFile>) (() => new StoredFile(commitItem.GetContent(), commitItem.ObjectId.ToByteArray())), Encoding.Default.CodePage, Encoding.Default.CodePage, new long?(5242880L));
              annotateBatchResult1.Diffs.Add(gitAnnotateResult);
            }
          }
        }
        this.SetSecuredObject((IEnumerable<VersionControlSecuredObject>) securableObjects);
        return securableObjects;
      }
    }

    public override IEnumerable<TeamIdentityReference> GetAuthors() => (IEnumerable<TeamIdentityReference>) this.RequestContext.GetService<ITeamFoundationGitCommitService>().QueryCommitAuthorNames(this.RequestContext, this.Repository).Select<string, GitIdentityReference>((Func<string, GitIdentityReference>) (name =>
    {
      return new GitIdentityReference()
      {
        DisplayName = name
      };
    })).ToArray<GitIdentityReference>();

    public override IEnumerable<int> GetLinkedWorkItemIds(string[] versions)
    {
      IEnumerable<int> ints = (IEnumerable<int>) new List<int>();
      this.RequestContext.GetService<TeamFoundationLinkingService>();
      foreach (string version in versions)
      {
        if (!string.IsNullOrEmpty(version))
        {
          string artifactUri = (string) null;
          Sha1Id commitId;
          if (this.TryGetCommitIdFromVersion(version, out commitId))
            artifactUri = LinkingUtilities.EncodeUri(this.GetCommitById(commitId).GetArtifactId(this.Repository.Key));
          else if (version.StartsWith("GB", StringComparison.OrdinalIgnoreCase))
            artifactUri = LinkingUtilities.EncodeUri(new ArtifactId("Git", "Ref", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) this.Repository.Key.ProjectId, (object) this.Repository.Key.RepoId, (object) version)));
          else if (LinkingUtilities.IsUriWellFormed(version))
            artifactUri = version;
          if (artifactUri != null)
          {
            IEnumerable<int> idsFromArtifactUri = this.GetLinkedWorkItemIdsFromArtifactUri(artifactUri);
            ints = ints.Concat<int>(idsFromArtifactUri);
          }
        }
      }
      return ints.Distinct<int>();
    }

    private class MemoizedCacheSecurityToken
    {
    }
  }
}
