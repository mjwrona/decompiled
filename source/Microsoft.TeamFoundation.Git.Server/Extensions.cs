// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Extensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class Extensions
  {
    private static readonly ByteArrayEqualityComparer s_byteArrayComparer = new ByteArrayEqualityComparer();
    private const int c_defaultMaxChanges = 1000000;

    public static GitObjectType GetResolvableObjectType(this TfsGitObject gitObject)
    {
      if (gitObject == null)
        return GitObjectType.Bad;
      return gitObject.ObjectType == GitObjectType.Tag ? ((TfsGitTag) gitObject).GetReferencedObject().GetResolvableObjectType() : gitObject.ObjectType;
    }

    public static TfsGitCommit TryResolveToCommit(this TfsGitObject gitObject)
    {
      while (gitObject != null && gitObject.ObjectType == GitObjectType.Tag)
        gitObject = ((TfsGitTag) gitObject).GetReferencedObject();
      return gitObject as TfsGitCommit;
    }

    public static TfsGitCommit ResolveToCommit(this TfsGitObject gitObject) => gitObject.TryResolveToCommit() ?? throw new GitUnresolvableToCommitException(gitObject.ObjectId);

    public static bool TryPeelToNonTag(this TfsGitTag tag, out TfsGitObject peeledObject) => Extensions.TryPeelToNonTagImpl(tag, out peeledObject, (List<TfsGitObject>) null);

    public static bool TryPeelToNonTag(this TfsGitTag tag, out List<TfsGitObject> seenObjects)
    {
      seenObjects = new List<TfsGitObject>();
      return Extensions.TryPeelToNonTagImpl(tag, out TfsGitObject _, seenObjects);
    }

    private static bool TryPeelToNonTagImpl(
      TfsGitTag tag,
      out TfsGitObject peeledObject,
      List<TfsGitObject> seenObjects)
    {
      TfsGitObject referencedObject = tag.GetReferencedObject();
      if (referencedObject == null)
      {
        peeledObject = (TfsGitObject) null;
        return false;
      }
      seenObjects?.Add(referencedObject);
      if (referencedObject.ObjectType == GitObjectType.Tag)
        return Extensions.TryPeelToNonTagImpl((TfsGitTag) referencedObject, out peeledObject, seenObjects);
      peeledObject = referencedObject;
      return true;
    }

    public static IEnumerable<TfsGitDiffEntry> GetManifest(
      this TfsGitCommit commit,
      ITfsGitRepository repository,
      bool detectRenames,
      bool includeTrees = true,
      int maxChanges = 1000000,
      bool preventGitTreeOverflow = false)
    {
      return commit.GetManifestLazyish(repository.ObjectMetadata, detectRenames ? repository.OdbSettings.MaxRenameDetectionFileSize : 0, includeTrees, preventGitTreeOverflow).Take<TfsGitDiffEntry>(maxChanges);
    }

    internal static IEnumerable<TfsGitDiffEntry> GetManifestFromOdb(
      this TfsGitCommit commit,
      Odb odb,
      bool detectRenames,
      bool includeTrees = true,
      int maxChanges = 1000000,
      bool preventGitTreeOverflow = false)
    {
      return commit.GetManifestLazyish(odb.ObjectMetadata, detectRenames ? odb.Settings.MaxRenameDetectionFileSize : 0, includeTrees, preventGitTreeOverflow).Take<TfsGitDiffEntry>(maxChanges);
    }

    internal static IEnumerable<TfsGitDiffEntry> GetManifestLazyish(
      this TfsGitCommit commit,
      IObjectMetadata objectMetadata,
      int maxRenameDetectionFileSize,
      bool includeTrees,
      bool isEntriesCountLimited = false,
      int maxEntriesCount = 2147483647)
    {
      TfsGitTree tree = commit.GetTree();
      List<TfsGitCommit> tfsGitCommitList = new List<TfsGitCommit>((IEnumerable<TfsGitCommit>) commit.GetParents());
      if (tfsGitCommitList.Count == 0)
      {
        foreach (TreeEntryAndPath newEntry in tree.GetTreeEntriesRecursive(preventGitTreeOverflow: isEntriesCountLimited))
        {
          if (includeTrees || newEntry.Entry.ObjectType != GitObjectType.Tree)
            yield return new TfsGitDiffEntry((TreeEntryAndPath) null, newEntry);
        }
      }
      else if (1 == tfsGitCommitList.Count)
      {
        foreach (TfsGitDiffEntry tfsGitDiffEntry in maxRenameDetectionFileSize > 0 ? (IEnumerable<TfsGitDiffEntry>) TfsGitDiffHelper.DiffTrees(objectMetadata, maxRenameDetectionFileSize, tfsGitCommitList[0].GetTree(), tree, isEntriesCountLimited: isEntriesCountLimited, maxEntriesCount: maxEntriesCount) : TfsGitDiffHelper.DiffTreesLazy(tfsGitCommitList[0].GetTree(), tree, isEntriesCountLimited: isEntriesCountLimited, maxEntriesCount: maxEntriesCount))
        {
          if (includeTrees || tfsGitDiffEntry.OldObjectType != GitObjectType.Tree && tfsGitDiffEntry.NewObjectType != GitObjectType.Tree)
            yield return tfsGitDiffEntry;
        }
      }
      else
      {
        SortedDictionary<string, TfsGitDiffEntry> sortedDictionary = new SortedDictionary<string, TfsGitDiffEntry>((IComparer<string>) StringComparer.Ordinal);
        Dictionary<string, int> dictionary = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (TfsGitDiffEntry tfsGitDiffEntry in maxRenameDetectionFileSize > 0 ? (IEnumerable<TfsGitDiffEntry>) TfsGitDiffHelper.DiffTrees(objectMetadata, maxRenameDetectionFileSize, tfsGitCommitList[0].GetTree(), tree, isEntriesCountLimited: isEntriesCountLimited) : TfsGitDiffHelper.DiffTreesLazy(tfsGitCommitList[0].GetTree(), tree, isEntriesCountLimited: isEntriesCountLimited))
        {
          tfsGitDiffEntry.ContentChanged = false;
          sortedDictionary[tfsGitDiffEntry.RelativePath] = tfsGitDiffEntry;
          dictionary[tfsGitDiffEntry.RelativePath] = 1;
        }
        for (int index = 1; index < tfsGitCommitList.Count; ++index)
        {
          foreach (TfsGitDiffEntry tfsGitDiffEntry in TfsGitDiffHelper.DiffTreesLazy(tfsGitCommitList[index].GetTree(), tree, isEntriesCountLimited: isEntriesCountLimited))
          {
            if (dictionary.ContainsKey(tfsGitDiffEntry.RelativePath) && ++dictionary[tfsGitDiffEntry.RelativePath] == tfsGitCommitList.Count)
              sortedDictionary[tfsGitDiffEntry.RelativePath].ContentChanged = true;
          }
        }
        foreach (TfsGitDiffEntry tfsGitDiffEntry in sortedDictionary.Values)
        {
          if (includeTrees || tfsGitDiffEntry.NewObjectType != GitObjectType.Tree && tfsGitDiffEntry.OldObjectType != GitObjectType.Tree)
            yield return tfsGitDiffEntry;
        }
      }
    }

    public static bool IsManifestEmpty(this TfsGitCommit commit)
    {
      Sha1Id objectId = commit.GetTree().ObjectId;
      TfsGitTree tree = commit.GetParents().FirstOrDefault<TfsGitCommit>()?.GetTree();
      Sha1Id sha1Id = tree != null ? tree.ObjectId : TfsGitTree.EmptyTreeId;
      return objectId == sha1Id;
    }

    public static ArtifactId GetArtifactId(this TfsGitCommit commit, RepoKey repoKey) => GitCommitArtifactId.GetArtifactIdForCommit(repoKey, commit.ObjectId);

    public static string GetNameToDisplay(this IVssRequestContext requestContext)
    {
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<ITeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      return foundationIdentity == null || string.IsNullOrEmpty(foundationIdentity.DisplayName) ? requestContext.AuthenticatedUserName : foundationIdentity.DisplayName;
    }

    public static IdentityRef GetIdentityRef(
      this TfsGitRef tfsGitRef,
      IVssRequestContext requestContext)
    {
      IdentityRef identityRef = (IdentityRef) null;
      if (tfsGitRef.IsLockedById.HasValue)
      {
        ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
        TeamFoundationIdentity userIdentity = IdentityHelper.Instance.GetUserIdentity(requestContext, service, tfsGitRef.IsLockedById.Value);
        if (userIdentity != null)
          identityRef = userIdentity.ToIdentityRef(requestContext);
      }
      return identityRef;
    }

    public static IdentityRef GetCreatorIdentityRef(
      this TfsGitRef tfsGitRef,
      IVssRequestContext requestContext)
    {
      IdentityRef creatorIdentityRef = (IdentityRef) null;
      if (tfsGitRef.CreatorId.HasValue)
      {
        ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
        TeamFoundationIdentity userIdentity = IdentityHelper.Instance.GetUserIdentity(requestContext, service, tfsGitRef.CreatorId.Value);
        if (userIdentity != null)
          creatorIdentityRef = userIdentity.ToIdentityRef(requestContext);
      }
      return creatorIdentityRef;
    }

    public static string GetCurrentFragmentForResource(this GitPermissionScope permissionScope)
    {
      switch (permissionScope)
      {
        case GitPermissionScope.Project:
          return Resources.Get("TheCurrentProject");
        case GitPermissionScope.Repository:
          return Resources.Get("TheCurrentRepository");
        case GitPermissionScope.Project | GitPermissionScope.Repository:
          return Resources.Get("TheCurrentProjectOrRepository");
        case GitPermissionScope.Branch:
          return Resources.Get("TheCurrentBranch");
        case GitPermissionScope.NonBranchRef:
          return Resources.Get("TheCurrentNonBranchRef");
        case GitPermissionScope.BranchesRoot:
          return Resources.Get("TheCurrentBranchesRoot");
        default:
          throw new ArgumentException(nameof (permissionScope));
      }
    }

    public static string GetRepositoryWebUri(
      this TfsGitRepositoryInfo repositoryInfo,
      IVssRequestContext requestContext)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, repositoryInfo.Key.ProjectId);
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      return GitServerUtils.GetRepositoryWebUrl(requestContext, publicBaseUrl, project.Name, repositoryInfo.Name);
    }

    public static string GetRepositoryCloneUri(
      this TfsGitRepositoryInfo repositoryInfo,
      IVssRequestContext requestContext)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, repositoryInfo.Key.ProjectId);
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      return GitServerUtils.GetRepositoryCloneUrl(requestContext, publicBaseUrl, project.Name, repositoryInfo.Name);
    }

    public static List<TfsGitRefLogEntry> GetNextRefLogEntries(
      this ITfsGitRepository repo,
      IVssRequestContext requestContext,
      string refName,
      int? afterPushId,
      int? take)
    {
      ArgumentUtility.CheckForNull<string>(refName, nameof (refName));
      using (requestContext.TimeRegion(nameof (GetNextRefLogEntries), nameof (GetNextRefLogEntries), 1013853))
      {
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          return gitCoreComponent.GetNextRefLogEntries(repo.Key, refName, afterPushId, take);
      }
    }
  }
}
