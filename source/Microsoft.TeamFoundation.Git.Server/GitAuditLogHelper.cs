// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAuditLogHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitAuditLogHelper
  {
    public static void RepositoryCreated(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string repoName,
      MinimalGlobalRepoKey? forkSourceRepo)
    {
      string actionId = !forkSourceRepo.HasValue ? "Git.RepositoryCreated" : "Git.RepositoryForked";
      GitAuditLogHelper.RepositoryCreated(requestContext, actionId, repoKey, repoName, forkSourceRepo, false);
    }

    internal static void RepositoriesUndeleted(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      GitAuditLogHelper.RepositoryCreated(requestContext, "Git.RepositoryUndeleted", repository.Key, repository.Name, new MinimalGlobalRepoKey?(), true);
    }

    private static void RepositoryCreated(
      IVssRequestContext requestContext,
      string actionId,
      RepoKey repoKey,
      string repoName,
      MinimalGlobalRepoKey? forkSourceRepo,
      bool isUndelete)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      string projectName1 = service.GetProjectName(requestContext, repoKey.ProjectId);
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        ["ProjectName"] = (object) projectName1,
        ["RepoId"] = (object) repoKey.RepoId,
        ["RepoName"] = (object) repoName
      };
      if (forkSourceRepo.HasValue)
      {
        RepoKey repoKey1;
        string str;
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          str = gitCoreComponent.RepositoryNameFromId(forkSourceRepo.Value.RepositoryId, false, out repoKey1, out bool _, out long _, out bool _, out bool _);
        string projectName2;
        service.TryGetProjectName(requestContext, repoKey1.ProjectId, out projectName2);
        dictionary["ParentProjectId"] = (object) repoKey.ProjectId;
        dictionary["ParentProjectName"] = (object) projectName2;
        dictionary["ParentRepoId"] = (object) repoKey1.RepoId;
        dictionary["ParentRepoName"] = (object) str;
      }
      IVssRequestContext requestContext1 = requestContext;
      string actionId1 = actionId;
      Dictionary<string, object> data = dictionary;
      Guid projectId1 = repoKey.ProjectId;
      Guid targetHostId = new Guid();
      Guid projectId2 = projectId1;
      requestContext1.LogAuditEvent(actionId1, data, targetHostId, projectId2);
    }

    public static void RepositoryModified(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string repositoryName,
      string newRepositoryName,
      string defaultBranch,
      string newDefaultBranch)
    {
      string projectName;
      requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, repoKey.ProjectId, out projectName);
      newRepositoryName = newRepositoryName ?? repositoryName;
      defaultBranch = defaultBranch ?? "";
      newDefaultBranch = newDefaultBranch ?? defaultBranch;
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
      {
        ["ProjectName"] = (object) projectName,
        ["RepoId"] = (object) repoKey.RepoId,
        ["RepoName"] = (object) newRepositoryName
      };
      if (newRepositoryName != repositoryName)
      {
        Dictionary<string, object> dictionary2 = new Dictionary<string, object>((IDictionary<string, object>) dictionary1)
        {
          ["PreviousRepoName"] = (object) repositoryName
        };
        IVssRequestContext requestContext1 = requestContext;
        Dictionary<string, object> data = dictionary2;
        Guid projectId1 = repoKey.ProjectId;
        Guid targetHostId = new Guid();
        Guid projectId2 = projectId1;
        requestContext1.LogAuditEvent("Git.RepositoryRenamed", data, targetHostId, projectId2);
      }
      if (!(newDefaultBranch != defaultBranch))
        return;
      Dictionary<string, object> dictionary3 = new Dictionary<string, object>((IDictionary<string, object>) dictionary1)
      {
        ["DefaultBranch"] = (object) newDefaultBranch,
        ["PreviousDefaultBranch"] = (object) defaultBranch
      };
      IVssRequestContext requestContext2 = requestContext;
      Dictionary<string, object> data1 = dictionary3;
      Guid projectId3 = repoKey.ProjectId;
      Guid targetHostId1 = new Guid();
      Guid projectId4 = projectId3;
      requestContext2.LogAuditEvent("Git.RepositoryDefaultBranchChanged", data1, targetHostId1, projectId4);
    }

    public static void RepositoriesDeleted(
      IVssRequestContext requestContext,
      ICollection<TfsGitRepositoryInfo> repositoryInfos,
      bool destroyed)
    {
      Guid projectId1 = Guid.Empty;
      string str1 = (string) null;
      string str2 = destroyed ? "Git.RepositoryDestroyed" : "Git.RepositoryDeleted";
      foreach (TfsGitRepositoryInfo repositoryInfo in (IEnumerable<TfsGitRepositoryInfo>) repositoryInfos)
      {
        if (repositoryInfo.Key.ProjectId != projectId1)
        {
          projectId1 = repositoryInfo.Key.ProjectId;
          str1 = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId1);
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>()
        {
          ["ProjectName"] = (object) str1,
          ["RepoId"] = (object) repositoryInfo.Key.RepoId,
          ["RepoName"] = (object) repositoryInfo.Name
        };
        IVssRequestContext requestContext1 = requestContext;
        string actionId = str2;
        Dictionary<string, object> data = dictionary;
        Guid guid = projectId1;
        Guid targetHostId = new Guid();
        Guid projectId2 = guid;
        requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId2);
      }
    }

    public static void RepositoryIsDisabledModified(
      IVssRequestContext requestContext,
      RepoScope repoScope,
      bool isDisabled)
    {
      string str1 = isDisabled ? "Git.RepositoryDisabled" : "Git.RepositoryEnabled";
      string str2;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        str2 = gitCoreComponent.RepositoryNameFromId(repoScope.RepoId, true, out RepoKey _, out bool _, out long _, out bool _, out bool _);
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        ["RepoName"] = (object) str2
      };
      IVssRequestContext requestContext1 = requestContext;
      string actionId = str1;
      Dictionary<string, object> data = dictionary;
      Guid projectId1 = repoScope.ProjectId;
      Guid targetHostId = new Guid();
      Guid projectId2 = projectId1;
      requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId2);
    }

    public static void RefUpdatePoliciesBypassed(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitRefUpdateRequest refUpdate,
      Dictionary<string, object> auditDetails)
    {
      Guid projectId1 = repository.Key.ProjectId;
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId1);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IDictionary<string, object>) auditDetails)
      {
        ["ProjectName"] = (object) projectName,
        ["RepoId"] = (object) repository.Key.RepoId,
        ["RepoName"] = (object) repository.Name,
        ["Name"] = (object) (refUpdate.Name ?? ""),
        ["FriendlyName"] = (object) (GitUtils.GetFriendlyBranchName(refUpdate.Name) ?? ""),
        ["OldObjectId"] = (object) refUpdate.OldObjectId.ToString(),
        ["NewObjectId"] = (object) refUpdate.NewObjectId.ToString()
      };
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, object> data = dictionary;
      Guid guid = projectId1;
      Guid targetHostId = new Guid();
      Guid projectId2 = guid;
      requestContext1.LogAuditEvent("Git.RefUpdatePoliciesBypassed", data, targetHostId, projectId2);
    }
  }
}
