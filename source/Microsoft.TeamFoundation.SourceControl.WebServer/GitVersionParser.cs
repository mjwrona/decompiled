// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitVersionParser
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitVersionParser
  {
    public static TfsGitCommit GetCommitById(ITfsGitRepository repository, Sha1Id commitId) => (repository.TryLookupObject(commitId) ?? throw new GitCommitDoesNotExistException(commitId)).TryResolveToCommit() ?? throw new GitUnresolvableToCommitException(commitId);

    public static TfsGitCommit GetCommitFromVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor)
    {
      string str = (string) null;
      return GitVersionParser.GetCommitFromVersion(requestContext, repository, ref str, versionDescriptor);
    }

    public static TfsGitCommit GetCommitFromVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ref string item,
      GitVersionDescriptor versionDescriptor)
    {
      return GitVersionParser.GetCommitFromVersion(requestContext, repository, ref item, versionDescriptor, out string _);
    }

    public static TfsGitCommit GetCommitFromVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ref string path,
      GitVersionDescriptor versionDescriptor,
      out string versionDescription)
    {
      ArgumentUtility.CheckForNull<GitVersionDescriptor>(versionDescriptor, nameof (versionDescriptor));
      GitVersionType versionType = versionDescriptor.VersionType;
      string version = versionDescriptor.Version;
      GitVersionOptions versionOptions = versionDescriptor.VersionOptions;
      TfsGitCommit commitFromVersion;
      switch (versionType)
      {
        case GitVersionType.Branch:
          try
          {
            if (string.IsNullOrEmpty(version))
            {
              TfsGitRef defaultOrAny = repository.Refs.GetDefaultOrAny();
              versionDescription = defaultOrAny != null ? defaultOrAny.Name : throw new ArgumentException(Resources.Format("ErrorNoBranchesFormat", (object) repository.Name)).Expected("git");
              commitFromVersion = GitVersionParser.GetCommitById(repository, defaultOrAny.ObjectId);
              break;
            }
            versionDescription = version;
            commitFromVersion = GitVersionParser.GetCommitById(repository, (GitVersionParser.GetBranchObjectId(repository, version) ?? throw new GitUnresolvableToCommitException(versionDescriptor.ToString(), repository.Name)).Value);
            break;
          }
          catch (GitCommitDoesNotExistException ex)
          {
            requestContext.TraceAlways(1013871, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitVersionParser), "The object a ref pointed to was not found. This most likely indicates a bug in resolving version descriptors after the index was loaded.");
            throw;
          }
        case GitVersionType.Tag:
          versionDescription = version;
          Sha1Id? tagReferenceId = GitVersionParser.GetTagReferenceId(repository, version);
          if (!tagReferenceId.HasValue)
            throw new GitUnresolvableToCommitException(versionDescriptor.ToString(), repository.Name);
          try
          {
            commitFromVersion = repository.LookupObject(tagReferenceId.Value).ResolveToCommit();
            break;
          }
          catch (GitObjectDoesNotExistException ex)
          {
            requestContext.TraceAlways(1013872, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitVersionParser), "The object a tag ref pointed to was not found. This most likely indicates a bug in resolving version descriptors after the index was loaded.");
            throw;
          }
        case GitVersionType.Commit:
          Sha1Id commitIdFromVersion = GitVersionParser.GetCommitIdFromVersion(versionDescriptor.Version);
          versionDescription = Resources.Format("CommitDescriptionFormat", (object) commitIdFromVersion.ToAbbreviatedString());
          commitFromVersion = GitVersionParser.GetCommitById(repository, commitIdFromVersion);
          break;
        default:
          throw new ArgumentException(Resources.Format("ErrorInvalidGitVersionSpec", (object) versionDescriptor.ToString())).Expected("git");
      }
      if (string.IsNullOrEmpty(path))
        return commitFromVersion;
      switch (versionOptions)
      {
        case GitVersionOptions.PreviousChange:
          TfsGitCommit previousVersion = GitVersionParser.GetPreviousVersion(requestContext, repository, ref path, commitFromVersion);
          versionDescription = previousVersion != null ? Resources.Format("CommitDescriptionFormat", (object) previousVersion.ObjectId) : throw new GitNoPreviousChangeException(path, versionDescriptor.VersionType.ToString(), versionDescriptor.Version, commitFromVersion.ObjectId);
          commitFromVersion = previousVersion;
          break;
        case GitVersionOptions.FirstParent:
          TfsGitCommit firstParentVersion = GitVersionParser.GetFirstParentVersion(repository, ref path, commitFromVersion);
          versionDescription = firstParentVersion != null ? Resources.Format("CommitDescriptionFormat", (object) firstParentVersion.ObjectId) : throw new ArgumentException(Resources.Format("GitNoParentCommit", (object) versionDescriptor.ToString(), (object) commitFromVersion.ObjectId)).Expected("git");
          commitFromVersion = firstParentVersion;
          break;
      }
      return commitFromVersion;
    }

    public static TfsGitCommit GetFirstParentVersion(
      ITfsGitRepository repository,
      ref string itemPath,
      TfsGitCommit childCommit)
    {
      string str = "/" + GitItemUtility.TrimPathSeparator(itemPath);
      foreach (TfsGitDiffEntry tfsGitDiffEntry in childCommit.GetManifest(repository, true))
      {
        if (tfsGitDiffEntry.RelativePath.Equals(str))
        {
          if ((tfsGitDiffEntry.ChangeType & TfsGitChangeType.Rename) != (TfsGitChangeType) 0)
          {
            itemPath = tfsGitDiffEntry.RenameSourceItemPath;
            break;
          }
          break;
        }
      }
      return childCommit.GetParents().FirstOrDefault<TfsGitCommit>();
    }

    public static TfsGitCommit GetPreviousVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ref string itemPath,
      TfsGitCommit startCommit)
    {
      TfsGitCommit previousVersion = (TfsGitCommit) null;
      NormalizedGitPath path = new NormalizedGitPath(itemPath);
      TfsGitCommitChangeWithId[] array1 = repository.GetFileHistoryWithChanges(requestContext, startCommit.ObjectId, path.ToString()).Take<TfsGitCommitChangeWithId>(2).ToArray<TfsGitCommitChangeWithId>();
      TfsGitCommitChangeWithId commitChangeWithId1 = array1.Length != 0 ? array1[0] : (TfsGitCommitChangeWithId) null;
      TfsGitCommitChangeWithId commitChangeWithId2 = array1.Length > 1 ? array1[1] : (TfsGitCommitChangeWithId) null;
      if (commitChangeWithId1 == null)
        return (TfsGitCommit) null;
      if ((commitChangeWithId1.ChangeType & TfsGitChangeType.Delete) != (TfsGitChangeType) 0)
        return commitChangeWithId2 != null ? repository.LookupObject<TfsGitCommit>(commitChangeWithId2.CommitId) : (TfsGitCommit) null;
      if (!commitChangeWithId1.CommitId.Equals(startCommit.ObjectId))
      {
        TfsGitCommit tfsGitCommit = repository.LookupObject<TfsGitCommit>(commitChangeWithId1.CommitId);
        if (startCommit.GetParents().Count > 1 && !TfsGitDiffHelper.WalkPath(startCommit.GetTree(), path).ObjectId.Equals(TfsGitDiffHelper.WalkPath(tfsGitCommit.GetTree(), path).ObjectId))
          return startCommit;
      }
      TfsGitCommit tfsGitCommit1 = repository.LookupObject<TfsGitCommit>(commitChangeWithId1.CommitId);
      IReadOnlyList<TfsGitCommit> parents = tfsGitCommit1.GetParents();
      if ((commitChangeWithId1.ChangeType & TfsGitChangeType.Rename) != (TfsGitChangeType) 0)
      {
        itemPath = commitChangeWithId1.RenameSourceItemPath;
        TfsGitCommitChangeWithId commitChangeWithId3 = repository.GetFileHistoryWithChanges(requestContext, parents[0].ObjectId, itemPath).Take<TfsGitCommitChangeWithId>(1).ToArray<TfsGitCommitChangeWithId>()[0];
        previousVersion = repository.LookupObject<TfsGitCommit>(commitChangeWithId3.CommitId);
      }
      else if ((commitChangeWithId1.ChangeType & TfsGitChangeType.Add) != (TfsGitChangeType) 0 && parents.Count > 1)
      {
        TfsGitTree tree = tfsGitCommit1.GetTree();
        for (int index1 = 1; index1 < parents.Count; ++index1)
        {
          IList<TfsGitDiffEntry> tfsGitDiffEntryList = TfsGitDiffHelper.DiffTrees(repository, parents[index1].GetTree(), tree, true);
          bool flag = false;
          for (int index2 = 0; index2 < tfsGitDiffEntryList.Count; ++index2)
          {
            if ((tfsGitDiffEntryList[index2].ChangeType & TfsGitChangeType.Rename) != (TfsGitChangeType) 0 && tfsGitDiffEntryList[index2].RelativePath.Equals(itemPath))
            {
              flag = true;
              itemPath = tfsGitDiffEntryList[index2].RenameSourceItemPath;
              break;
            }
          }
          if (flag)
          {
            TfsGitCommitChangeWithId[] array2 = repository.GetFileHistoryWithChanges(requestContext, parents[index1].ObjectId, itemPath).Take<TfsGitCommitChangeWithId>(1).ToArray<TfsGitCommitChangeWithId>();
            previousVersion = repository.LookupObject<TfsGitCommit>(array2[0].CommitId);
            break;
          }
        }
      }
      else if (commitChangeWithId2 != null)
        previousVersion = repository.LookupObject<TfsGitCommit>(commitChangeWithId2.CommitId);
      return previousVersion;
    }

    public static Sha1Id GetCommitIdFromVersion(string version)
    {
      string sha1Id = version;
      if (!string.IsNullOrEmpty(sha1Id))
      {
        try
        {
          return GitCommitUtility.ParseSha1Id(sha1Id);
        }
        catch (ArgumentException ex) when (ex.ExpectedExceptionFilter("git"))
        {
        }
      }
      throw new ArgumentException(Resources.Format("ErrorInvalidGitVersionSpec", (object) version)).Expected("git");
    }

    public static Sha1Id? GetBranchObjectId(ITfsGitRepository repository, string branchName) => repository.Refs.MatchingName("refs/heads/" + branchName)?.ObjectId;

    public static Sha1Id? GetTagReferenceId(ITfsGitRepository repository, string tagName) => repository.Refs.MatchingName("refs/tags/" + tagName)?.ObjectId;

    public static GitVersionDescriptor VersionDescriptorFromRequest(
      HttpRequestMessage request,
      string prefix = null)
    {
      return GitVersionParser.GetVersionDescriptor(request, prefix + "versionType", prefix + "version", prefix + "versionOptions");
    }

    public static void ValidateVersionDescriptor(GitVersionDescriptor descriptor)
    {
      if (descriptor.VersionType != GitVersionType.Branch && string.IsNullOrEmpty(descriptor.Version))
        throw new ArgumentException(Resources.Get("AmbiguousVersion")).Expected("git");
    }

    private static GitVersionDescriptor GetVersionDescriptor(
      HttpRequestMessage request,
      string versionTypeKey,
      string versionKey,
      string versionOptionsKey)
    {
      GitVersionDescriptor descriptor = new GitVersionDescriptor();
      if (string.IsNullOrEmpty(request.RequestUri.Query))
        return descriptor;
      NameValueCollection queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
      string str1 = queryString[versionKey];
      GitVersionDescriptor versionDescriptor = descriptor;
      string str2;
      if (str1 != null)
        str2 = str1.Split(',')[0];
      else
        str2 = (string) null;
      versionDescriptor.Version = str2;
      GitVersionType result1 = GitVersionType.Branch;
      string str3 = queryString[versionTypeKey];
      if (str3 != null)
      {
        if (Enum.TryParse<GitVersionType>(str3.Split(',')[0], true, out result1))
          descriptor.VersionType = result1;
        else
          throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) str3.Split(',')[0], (object) "versionType", (object) string.Join(",", Enum.GetNames(typeof (GitVersionOptions)))));
      }
      GitVersionOptions result2 = GitVersionOptions.None;
      string str4 = queryString[versionOptionsKey];
      if (str4 != null)
      {
        if (Enum.TryParse<GitVersionOptions>(str4.Split(',')[0], true, out result2))
          descriptor.VersionOptions = result2;
        else
          throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) str4.Split(',')[0], (object) "versionOptions", (object) string.Join(",", Enum.GetNames(typeof (GitVersionOptions)))));
      }
      GitVersionParser.ValidateVersionDescriptor(descriptor);
      return descriptor;
    }
  }
}
