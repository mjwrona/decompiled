// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitModelExtensions
  {
    private static Dictionary<TfsGitChangeType, VersionControlChangeType> s_changeTypeMap = new Dictionary<TfsGitChangeType, VersionControlChangeType>()
    {
      {
        TfsGitChangeType.Add,
        VersionControlChangeType.Add
      },
      {
        TfsGitChangeType.Delete,
        VersionControlChangeType.Delete
      },
      {
        TfsGitChangeType.Edit,
        VersionControlChangeType.Edit
      },
      {
        TfsGitChangeType.Rename,
        VersionControlChangeType.Rename
      },
      {
        TfsGitChangeType.SourceRename,
        VersionControlChangeType.SourceRename
      }
    };

    public static GitRepository ToWebApiItem(
      this TfsGitRepositoryInfo repo,
      IVssRequestContext rc,
      ProjectInfo projectInfo,
      bool includeLinks = false,
      bool includeAllUrls = false,
      bool includeParent = false)
    {
      return GitModelExtensions.GetWebApiGitRepository(rc, repo.Key, repo.Name, repo.DefaultBranch, repo.Size, repo.IsFork, projectInfo, includeLinks, includeAllUrls, includeParent, repo.IsDisabled, repo.IsInMaintenance);
    }

    public static GitRepository ToWebApiItem(
      this ITfsGitRepository repo,
      IVssRequestContext rc,
      ProjectInfo projectInfo = null,
      bool getDefaultBranch = false,
      bool includeLinks = false,
      bool includeParent = false)
    {
      return GitModelExtensions.GetWebApiGitRepository(rc, repo.Key, repo.Name, getDefaultBranch ? repo.Refs.GetDefault()?.Name : (string) null, repo.Size, repo.IsFork, projectInfo, includeLinks, includeParent: includeParent, isDisabled: repo.IsDisabled, isInMaintenance: repo.IsInMaintenance);
    }

    private static GitRepository GetWebApiGitRepository(
      IVssRequestContext rc,
      RepoKey repoKey,
      string repoName,
      string defaultBranch,
      long compressedSize,
      bool createdByForking,
      ProjectInfo projectInfo,
      bool includeLinks,
      bool includeAllUrls = false,
      bool includeParent = false,
      bool isDisabled = false,
      bool isInMaintenance = false)
    {
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(rc, true);
      if (projectInfo == null)
        projectInfo = rc.GetService<IProjectService>().GetProject(rc, repoKey.ProjectId);
      ISecuredObject securedObject = GitModelExtensions.GetSecuredObject(repoKey);
      string repositoryCloneUrl = GitServerUtils.GetRepositoryCloneUrl(rc, publicBaseUrl, projectInfo.Name, repoName);
      GitRepository apiGitRepository = new GitRepository()
      {
        Id = repoKey.RepoId,
        Name = repoName,
        RemoteUrl = repositoryCloneUrl,
        SshUrl = GitServerUtils.GetSshUrl(rc, repositoryCloneUrl, out bool _),
        ProjectReference = projectInfo.ToTeamProjectReference(rc),
        DefaultBranch = defaultBranch,
        Size = compressedSize == -1L ? new long?() : new long?(compressedSize),
        Url = GitReferenceLinksUtility.GetRepositoryUrl(rc, repoKey),
        WebUrl = GitServerUtils.GetRepositoryWebUrl(rc, publicBaseUrl, projectInfo.Name, repoName),
        IsFork = createdByForking,
        IsDisabled = new bool?(isDisabled),
        IsInMaintenance = new bool?(isInMaintenance)
      };
      if (includeParent)
      {
        GitRepositoryRef parent = rc.GetService<IGitForkService>().GetParent(rc, repoKey);
        if (parent != null)
        {
          GitRepository gitRepository = apiGitRepository;
          GitRepositoryRef gitRepositoryRef = new GitRepositoryRef();
          gitRepositoryRef.Id = parent.Id;
          gitRepositoryRef.Name = parent.Name;
          gitRepositoryRef.RemoteUrl = parent.RemoteUrl;
          gitRepositoryRef.SshUrl = parent.SshUrl;
          gitRepositoryRef.Url = parent.Url;
          gitRepositoryRef.IsFork = parent.IsFork;
          GitForkTeamProjectReference projectReference = new GitForkTeamProjectReference(securedObject);
          projectReference.Id = parent.ProjectReference.Id;
          projectReference.Name = parent.ProjectReference.Name;
          projectReference.Url = parent.ProjectReference.Url;
          gitRepositoryRef.ProjectReference = (TeamProjectReference) projectReference;
          gitRepository.ParentRepository = gitRepositoryRef;
          apiGitRepository.ParentRepository.SetSecuredObject(securedObject);
        }
      }
      apiGitRepository.SetSecuredObject(securedObject);
      if (includeLinks)
        apiGitRepository.Links = apiGitRepository.GetRepositoryReferenceLinks(rc, securedObject);
      if (includeAllUrls)
        apiGitRepository.ValidRemoteUrls = apiGitRepository.GetRepositoryRemoteUrls(rc).ToArray<string>();
      return apiGitRepository;
    }

    public static GitDeletedRepository ToWebApiItem(
      this TfsGitDeletedRepositoryInfo repo,
      IVssRequestContext rc,
      IDictionary<Guid, TeamFoundationIdentity> guidToIdentity,
      ProjectInfo projectInfo)
    {
      GitDeletedRepository webApiItem = new GitDeletedRepository()
      {
        Id = repo.Key.RepoId,
        Name = repo.Name,
        ProjectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(rc) : (TeamProjectReference) null,
        CreatedDate = repo.CreatedDate,
        DeletedDate = repo.DeletedDate
      };
      if (guidToIdentity.ContainsKey(repo.DeletedBy))
        webApiItem.DeletedBy = guidToIdentity[repo.DeletedBy].ToIdentityRef(rc);
      else
        webApiItem.DeletedBy = new IdentityRef()
        {
          Id = repo.DeletedBy.ToString()
        };
      return webApiItem;
    }

    public static IEnumerable<GitRef> ToWebApiItems(
      this IEnumerable<TfsGitRef> gitRefs,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      bool includeLinks,
      ILookup<Sha1Id, GitStatus> statusLookup,
      AnnotatedTagPeeler tagPeeler,
      ISecuredObject securedObject)
    {
      Uri refsUri = GitReferenceLinksUtility.GetRefsUri(requestContext, repoKey);
      string repositoryUrl = GitReferenceLinksUtility.GetRepositoryUrl(requestContext, repoKey);
      return gitRefs.Select<TfsGitRef, GitRef>((Func<TfsGitRef, GitRef>) (r => r.ToWebApiItem(requestContext, refsUri, repositoryUrl, includeLinks, statusLookup, tagPeeler, securedObject)));
    }

    public static GitRef ToWebApiItem(
      this TfsGitRefWithResolvedCommit tfsGitRef,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      bool includeLinks,
      ILookup<Sha1Id, GitStatus> statusLookup,
      ISecuredObject securedObject)
    {
      GitRef webApiItem = tfsGitRef.ToWebApiItem(requestContext, repoKey, includeLinks, statusLookup, (AnnotatedTagPeeler) null, securedObject);
      webApiItem.PeeledObjectId = tfsGitRef.ResolvedCommitId.ToString();
      return webApiItem;
    }

    public static GitRef ToWebApiItem(
      this TfsGitRef tfsGitRef,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      bool includeLinks,
      ILookup<Sha1Id, GitStatus> statusLookup,
      AnnotatedTagPeeler tagPeeler,
      ISecuredObject securedObject)
    {
      Uri refsUri = GitReferenceLinksUtility.GetRefsUri(requestContext, repoKey);
      string repositoryUrl = GitReferenceLinksUtility.GetRepositoryUrl(requestContext, repoKey);
      return tfsGitRef.ToWebApiItem(requestContext, refsUri, repositoryUrl, includeLinks, statusLookup, tagPeeler, securedObject);
    }

    private static GitRef ToWebApiItem(
      this TfsGitRef gitRef,
      IVssRequestContext requestContext,
      Uri refsUri,
      string repositoryUrl,
      bool includeLinks,
      ILookup<Sha1Id, GitStatus> statusLookup,
      AnnotatedTagPeeler tagPeeler,
      ISecuredObject securedObject)
    {
      GitRef webApiItem = new GitRef(gitRef.Name, gitRef.ObjectId.ToString(), gitRef.GetIdentityRef(requestContext));
      webApiItem.Creator = gitRef.GetCreatorIdentityRef(requestContext);
      webApiItem.Url = GitReferenceLinksUtility.BuildRefUrl(refsUri, gitRef.Name);
      if (includeLinks)
      {
        webApiItem.Links = GitReferenceLinksUtility.GetBaseReferenceLinks(webApiItem.Url, repositoryUrl, securedObject);
        if (webApiItem.IsLockedBy != null && webApiItem.IsLockedBy.Url != null)
          webApiItem.Links.AddLink("lockedBy", webApiItem.IsLockedBy.Url, securedObject);
      }
      if (statusLookup != null)
        webApiItem.Statuses = statusLookup[gitRef.ObjectId];
      TfsGitObject peeledObject;
      if (tagPeeler != null && tagPeeler.TryPeelTagRef(gitRef, out peeledObject))
        webApiItem.PeeledObjectId = peeledObject.ObjectId.ToString();
      webApiItem.SetSecuredObject(securedObject);
      return webApiItem;
    }

    public static GitRefFavorite ToWebApiItem(
      this TfsGitRefFavorite tfsFavorite,
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      bool includeLinks = false)
    {
      GitRefFavorite webApiItem = new GitRefFavorite(securedObject)
      {
        Id = tfsFavorite.FavoriteId,
        Name = tfsFavorite.Name,
        IdentityId = tfsFavorite.IdentityId,
        RepositoryId = tfsFavorite.RepoKey.RepoId,
        Type = tfsFavorite.IsFolder ? GitRefFavorite.RefFavoriteType.Folder : GitRefFavorite.RefFavoriteType.Ref
      };
      if (includeLinks)
      {
        webApiItem.Url = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.RefFavoritesLocationId, tfsFavorite.RepoKey.ProjectId, (object) new
        {
          favoriteId = tfsFavorite.FavoriteId
        }).AbsoluteUri;
        webApiItem.Links = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, webApiItem.Url, tfsFavorite.RepoKey);
        webApiItem.Links.AddLink("ref", GitReferenceLinksUtility.BuildRefUrl(requestContext, tfsFavorite.RepoKey, tfsFavorite.Name));
      }
      return webApiItem;
    }

    public static ChangeCountDictionary ToChangeCountDictionary(this ChangeCounts changeCounts)
    {
      ChangeCountDictionary changeCountDictionary = new ChangeCountDictionary();
      changeCountDictionary.Add(VersionControlChangeType.Add, changeCounts.Adds);
      changeCountDictionary.Add(VersionControlChangeType.Edit, changeCounts.Edits);
      changeCountDictionary.Add(VersionControlChangeType.Delete, changeCounts.Deletes);
      return changeCountDictionary;
    }

    public static GitTreeEntryRef ToGitTreeEntryShallow(
      this TfsGitTreeEntry entry,
      IVssRequestContext context,
      RepoKey repoKey,
      UrlHelper urlHelper,
      string relativePath = null)
    {
      Guid locationId = GitWebApiConstants.ItemsLocationId;
      GitObjectType objectType = entry.ObjectType;
      switch (objectType)
      {
        case GitObjectType.Tree:
          locationId = GitWebApiConstants.TreesLocationId;
          break;
        case GitObjectType.Blob:
          locationId = GitWebApiConstants.BlobsLocationId;
          break;
      }
      string sha1Id = entry.ObjectId.ToString();
      GitTreeEntryRef gitTreeEntryRef1 = new GitTreeEntryRef();
      gitTreeEntryRef1.GitObjectType = objectType;
      gitTreeEntryRef1.Mode = Convert.ToString((int) entry.Mode, 8);
      gitTreeEntryRef1.ObjectId = sha1Id;
      GitTreeEntryRef gitTreeEntryRef2 = gitTreeEntryRef1;
      string str;
      if (relativePath == null)
        str = entry.Name;
      else
        str = relativePath.TrimStart('/');
      gitTreeEntryRef2.RelativePath = str;
      GitTreeEntryRef treeEntryShallow = gitTreeEntryRef1;
      if (objectType == GitObjectType.Commit)
      {
        treeEntryShallow.Size = -1L;
        treeEntryShallow.Url = urlHelper.RestLink(context, locationId, (object) new
        {
          project = repoKey.ProjectId,
          repositoryId = repoKey.RepoId,
          path = ".gitmodules"
        });
      }
      else
      {
        treeEntryShallow.Size = entry.Object.GetLength();
        treeEntryShallow.Url = urlHelper.RestLink(context, locationId, RouteValuesFactory.TreeOrBlob(repoKey, sha1Id));
      }
      return treeEntryShallow;
    }

    public static void ParseNameEmail(string nameEmail, out string name, out string email)
    {
      name = (string) null;
      email = (string) null;
      if (nameEmail == null)
        return;
      string str = nameEmail.Trim();
      int num1 = str.LastIndexOf('<');
      if (num1 == -1)
      {
        name = nameEmail;
      }
      else
      {
        if (num1 > 0)
          name = str.Substring(0, num1 - 1);
        int num2 = str.LastIndexOf('>');
        if (num2 - 1 <= num1)
          return;
        int length = num2 - num1 - 1;
        email = str.Substring(num1 + 1, length);
      }
    }

    public static GitBranchStats ToBranchStats(
      this TfsGitCommitLineageDiff lineageDiff,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitCommitTranslator translator,
      ISecuredObject securedObject)
    {
      return new GitBranchStats(translator.ToGitCommitShallow(lineageDiff.Metadata, false), lineageDiff.RefName, lineageDiff.AheadCount, lineageDiff.BehindCount, securedObject);
    }

    public static GitItem ToGitItem(this TfsGitCommitChangeWithId change, string changeItemBaseUrl)
    {
      string stringToEscape = UriUtility.CombinePath(change.ParentPath, change.ChildItem).Trim('/');
      string str = change.CommitId.ToString();
      Uri uri = new Uri(UriUtility.CombinePath(changeItemBaseUrl, Uri.EscapeDataString(stringToEscape))).AppendQuery("versionType", "Commit").AppendQuery("version", str);
      GitItem gitItem = new GitItem();
      gitItem.CommitId = str;
      gitItem.Path = "/" + stringToEscape;
      gitItem.IsFolder = change.ObjectType == GitObjectType.Tree;
      gitItem.GitObjectType = change.ObjectType;
      gitItem.ObjectId = change.ChangedObjectId.ToString();
      gitItem.OriginalObjectId = change.OriginalObjectId.HasValue ? change.OriginalObjectId.ToString() : (string) null;
      gitItem.Url = uri.AbsoluteUri;
      return gitItem;
    }

    public static VersionControlChangeType ToVersionControlChangeType(TfsGitChangeType tfsChangeType)
    {
      VersionControlChangeType controlChangeType = VersionControlChangeType.None;
      foreach (KeyValuePair<TfsGitChangeType, VersionControlChangeType> changeType in GitModelExtensions.s_changeTypeMap)
      {
        if (tfsChangeType.HasFlag((Enum) changeType.Key))
          controlChangeType |= changeType.Value;
      }
      return controlChangeType;
    }

    public static GitCommitChanges ToGitCommitChanges(
      this IEnumerable<TfsGitCommitChangeWithId> commitChanges,
      IVssRequestContext context,
      RepoKey repoKey,
      int maxNumberOfChanges,
      int skipCount,
      ISecuredObject securedObject,
      out bool allChangesIncluded)
    {
      List<GitChange> gitChangeList = new List<GitChange>();
      ChangeCountDictionary changeCounts = new ChangeCountDictionary();
      string absoluteUri = context.GetService<ILocationService>().GetResourceUri(context, "git", GitWebApiConstants.ItemsLocationId, RouteValuesFactory.Repo(repoKey)).AbsoluteUri;
      int num = 0;
      foreach (TfsGitCommitChangeWithId commitChange in commitChanges)
      {
        if (num < skipCount)
          ChangeListHelpers.IncrementChangeFlagCount((Dictionary<VersionControlChangeType, int>) changeCounts, GitModelExtensions.ToVersionControlChangeType(commitChange.ChangeType), 1);
        else if (num < skipCount + maxNumberOfChanges)
        {
          ChangeListHelpers.IncrementChangeFlagCount((Dictionary<VersionControlChangeType, int>) changeCounts, GitModelExtensions.ToVersionControlChangeType(commitChange.ChangeType), 1);
          GitChange gitChange = new GitChange();
          gitChange.ChangeType = GitModelExtensions.ToVersionControlChangeType(commitChange.ChangeType);
          gitChange.Item = commitChange.ToGitItem(absoluteUri);
          gitChange.SourceServerItem = commitChange.RenameSourceItemPath;
          gitChange.SetSecuredObject(securedObject);
          gitChangeList.Add(gitChange);
        }
        else
          ChangeListHelpers.IncrementChangeFlagCount((Dictionary<VersionControlChangeType, int>) changeCounts, GitModelExtensions.ToVersionControlChangeType(commitChange.ChangeType), 1);
        ++num;
      }
      allChangesIncluded = num <= maxNumberOfChanges + skipCount;
      return new GitCommitChanges()
      {
        ChangeCounts = changeCounts,
        Changes = (IEnumerable<GitChange>) gitChangeList
      };
    }

    public static IEnumerable<GitTreeDiffEntry> ToGitTreeDiffEntries(
      this IEnumerable<TfsGitDiffEntry> diffEntries)
    {
      foreach (TfsGitDiffEntry diffEntry in diffEntries)
      {
        GitTreeDiffEntry gitTreeDiffEntry = new GitTreeDiffEntry();
        Sha1Id? nullable = diffEntry.NewObjectId;
        string str1;
        if (!nullable.HasValue)
        {
          str1 = (string) null;
        }
        else
        {
          nullable = diffEntry.NewObjectId;
          str1 = nullable.ToString();
        }
        gitTreeDiffEntry.TargetObjectId = str1;
        nullable = diffEntry.OldObjectId;
        string str2;
        if (!nullable.HasValue)
        {
          str2 = (string) null;
        }
        else
        {
          nullable = diffEntry.OldObjectId;
          str2 = nullable.ToString();
        }
        gitTreeDiffEntry.BaseObjectId = str2;
        nullable = diffEntry.NewObjectId;
        gitTreeDiffEntry.ObjectType = nullable.HasValue ? diffEntry.NewObjectType : diffEntry.OldObjectType;
        gitTreeDiffEntry.Path = diffEntry.RelativePath;
        gitTreeDiffEntry.ChangeType = GitModelExtensions.ToVersionControlChangeType(diffEntry.ChangeType);
        yield return gitTreeDiffEntry;
      }
    }

    public static GitRefUpdate ToRefUpdate(this TfsGitRefLogEntry refLog) => new GitRefUpdate()
    {
      RepositoryId = refLog.RepositoryId,
      Name = refLog.Name,
      OldObjectId = refLog.OldObjectId.ToString(),
      NewObjectId = refLog.NewObjectId.ToString()
    };

    public static IEnumerable<TfsGitCommitMetadata> ToGitCommitMetaData(
      this IEnumerable<Sha1Id> commitIds,
      ITfsGitRepository repo)
    {
      foreach (Sha1Id commitId in commitIds)
        yield return new TfsGitCommitMetadata(repo.LookupObject<TfsGitCommit>(commitId));
    }

    public static GitAnnotatedTag ToGitAnnotatedTags(
      this TfsGitTag gitTag,
      IVssRequestContext rc,
      RepoKey repoKey,
      bool includeUserImageUrl)
    {
      TfsGitObject referencedObject = gitTag.GetReferencedObject();
      IdentityAndDate tagger = gitTag.GetTagger();
      return new GitAnnotatedTag()
      {
        Name = gitTag.GetName(),
        Message = gitTag.GetComment(),
        ObjectId = gitTag.ObjectId.ToString(),
        TaggedObject = new GitObject()
        {
          ObjectId = referencedObject.ObjectId.ToString(),
          ObjectType = referencedObject.ObjectType
        },
        TaggedBy = GitServerUtils.CreateUserDate(rc, tagger.Name, tagger.Email, tagger.LocalTime, includeUserImageUrl),
        Url = rc.GetService<ILocationService>().GetResourceUri(rc, "git", GitWebApiConstants.AnnotatedTagsLocationId, RouteValuesFactory.AnnotatedTag(repoKey, gitTag.ObjectId)).AbsoluteUri
      };
    }

    private static ISecuredObject GetSecuredObject(RepoKey repoKey) => GitSecuredObjectFactory.CreateRepositoryReadOnly(repoKey);
  }
}
