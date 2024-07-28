// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRequestContextCacheUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitRequestContextCacheUtil : CodeReviewRequestContextCacheUtil
  {
    internal const string c_repoKeyPrefix = "_GitRepository#";
    internal const string c_refKeyPrefix = "_GitRef#";
    internal const string c_defaultRefKeyPrefix = "_GitDefaultRef#";
    internal const string c_defaultRefOrAnyKeyPrefix = "_GitDefaultRefOrAny#";
    internal const string c_pullRequestKeyPrefix = "_GitPullRequest#";
    private const string c_area = "GitRequestContextCacheUtil";

    public static ITfsGitRepository GetRepositoryOrDefault(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryName,
      bool throwOnError = false)
    {
      if (string.IsNullOrEmpty(repositoryName) || projectId == Guid.Empty)
        return (ITfsGitRepository) null;
      ITfsGitRepository cachedRepository = GitRequestContextCacheUtil.GetCachedRepository(requestContext, new Guid?(projectId), repositoryName);
      if (cachedRepository != null)
        return cachedRepository;
      ITfsGitRepository repository = (ITfsGitRepository) null;
      try
      {
        requestContext.GetService<ITeamFoundationGitRepositoryService>().TryFindRepositoryByName(requestContext, projectId.ToString(), repositoryName, out repository);
        GitRequestContextCacheUtil.CacheRepository(requestContext, repository);
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 700345, nameof (GitRequestContextCacheUtil), nameof (GetRepositoryOrDefault), ex, (object) new
        {
          projectId = projectId,
          repositoryName = repositoryName
        }, caller: nameof (GetRepositoryOrDefault));
        if (throwOnError)
          throw;
      }
      return repository;
    }

    public static ITfsGitRepository GetRepositoryOrDefault(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool throwOnError = false,
      bool includeDisabled = false)
    {
      if (repositoryId == Guid.Empty)
        return (ITfsGitRepository) null;
      ITfsGitRepository cachedRepository = GitRequestContextCacheUtil.GetCachedRepository(requestContext, new Guid?(repositoryId));
      if (cachedRepository != null)
        return cachedRepository;
      ITfsGitRepository repository = (ITfsGitRepository) null;
      try
      {
        repository = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: includeDisabled);
        GitRequestContextCacheUtil.CacheRepository(requestContext, repository);
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 700345, nameof (GitRequestContextCacheUtil), nameof (GetRepositoryOrDefault), ex, (object) new
        {
          repositoryId = repositoryId
        }, caller: nameof (GetRepositoryOrDefault));
        if (throwOnError)
          throw;
      }
      return repository;
    }

    public static IEnumerable<TfsGitRepositoryInfo> GetRepositoriesInfoOrDefault(
      IVssRequestContext requestContext,
      IEnumerable<Guid> repositoryIds,
      bool throwOnError = false)
    {
      List<TfsGitRepositoryInfo> source = new List<TfsGitRepositoryInfo>();
      foreach (Guid repositoryId in repositoryIds)
      {
        TfsGitRepositoryInfo cachedRepositoryInfo = GitRequestContextCacheUtil.GetCachedRepositoryInfo(requestContext, new Guid?(repositoryId));
        if (cachedRepositoryInfo != null)
          source.Add(cachedRepositoryInfo);
      }
      if (source.Count<TfsGitRepositoryInfo>() == repositoryIds.Count<Guid>())
        return (IEnumerable<TfsGitRepositoryInfo>) source;
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      IEnumerable<TfsGitRepositoryInfo> repositoriesInfoOrDefault = (IEnumerable<TfsGitRepositoryInfo>) null;
      try
      {
        repositoriesInfoOrDefault = service.QueryRepositoriesAcrossProjects(requestContext, repositoryIds);
        foreach (TfsGitRepositoryInfo repositoryInfo in repositoriesInfoOrDefault)
          GitRequestContextCacheUtil.CacheRepositoryInfo(requestContext, repositoryInfo);
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 700345, nameof (GitRequestContextCacheUtil), nameof (GetRepositoriesInfoOrDefault), ex, (object) new
        {
          repositoryIds = repositoryIds
        }, caller: nameof (GetRepositoriesInfoOrDefault));
        if (throwOnError)
          throw;
      }
      return repositoriesInfoOrDefault;
    }

    public static ITfsGitRepository GetRepositoryFromRepositoryInfo(
      IVssRequestContext requestContext,
      TfsGitRepositoryInfo repoInfo)
    {
      return repoInfo != null ? requestContext.GetService<ITeamFoundationGitRepositoryService>().RepositoryFromRepositoryInfo(requestContext, repoInfo) : (ITfsGitRepository) null;
    }

    internal static ITfsGitRepository GetCachedRepository(
      IVssRequestContext requestContext,
      Guid? projectId,
      string repositoryName)
    {
      TfsGitRepositoryInfo cachedRepositoryInfo = GitRequestContextCacheUtil.GetCachedRepositoryInfo(requestContext, projectId, repositoryName);
      return GitRequestContextCacheUtil.GetRepositoryFromRepositoryInfo(requestContext, cachedRepositoryInfo);
    }

    internal static ITfsGitRepository GetCachedRepository(
      IVssRequestContext requestContext,
      Guid? repositoryId)
    {
      TfsGitRepositoryInfo cachedRepositoryInfo = GitRequestContextCacheUtil.GetCachedRepositoryInfo(requestContext, repositoryId);
      return GitRequestContextCacheUtil.GetRepositoryFromRepositoryInfo(requestContext, cachedRepositoryInfo);
    }

    internal static TfsGitRepositoryInfo GetCachedRepositoryInfo(
      IVssRequestContext requestContext,
      Guid? projectId,
      string repositoryName)
    {
      string repositoryCacheKey = GitRequestContextCacheUtil.GetRepositoryCacheKey(projectId, repositoryName);
      return CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitRepositoryInfo>(requestContext, repositoryCacheKey);
    }

    internal static TfsGitRepositoryInfo GetCachedRepositoryInfo(
      IVssRequestContext requestContext,
      Guid? repositoryId)
    {
      string repositoryCacheKey = GitRequestContextCacheUtil.GetRepositoryCacheKey(repositoryId);
      return CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitRepositoryInfo>(requestContext, repositoryCacheKey);
    }

    public static IEnumerable<ITfsGitRepository> GetCachedRepositories(
      IVssRequestContext requestContext)
    {
      if (!CodeReviewRequestContextCacheUtil.IsCacheAvailable(requestContext, "_GitRepository#"))
        return (IEnumerable<ITfsGitRepository>) null;
      ITeamFoundationGitRepositoryService repositoryService = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      Dictionary<Guid, TfsGitRepositoryInfo> dictionary = new Dictionary<Guid, TfsGitRepositoryInfo>();
      foreach (string key in (IEnumerable<string>) requestContext.RootContext.Items.Keys)
      {
        if (key.StartsWith("_GitRepository#"))
        {
          TfsGitRepositoryInfo cachedValue = CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitRepositoryInfo>(requestContext, key);
          dictionary[cachedValue.Key.RepoId] = cachedValue;
        }
      }
      return dictionary.Values.Select<TfsGitRepositoryInfo, ITfsGitRepository>((Func<TfsGitRepositoryInfo, ITfsGitRepository>) (repoInfo => repositoryService.RepositoryFromRepositoryInfo(requestContext, repoInfo)));
    }

    internal static void CacheRepository(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      if (repository == null || (RepoScope) repository.Key == (RepoScope) null)
        return;
      IVssRequestContext requestContext1 = requestContext;
      string name = repository.Name;
      RepoKey key = repository.Key;
      int num1 = repository.IsFork ? 1 : 0;
      bool isInMaintenance = repository.IsInMaintenance;
      DateTime createdDate = new DateTime();
      DateTime lastMetadataUpdate = new DateTime();
      int num2 = isInMaintenance ? 1 : 0;
      TfsGitRepositoryInfo repositoryInfo = new TfsGitRepositoryInfo(name, key, isFork: num1 != 0, createdDate: createdDate, lastMetadataUpdate: lastMetadataUpdate, isInMaintenance: num2 != 0);
      GitRequestContextCacheUtil.CacheRepositoryInfo(requestContext1, repositoryInfo);
    }

    internal static void CacheRepositoryInfo(
      IVssRequestContext requestContext,
      TfsGitRepositoryInfo repositoryInfo)
    {
      if (repositoryInfo == null || requestContext == null)
        return;
      string repositoryCacheKey1 = GitRequestContextCacheUtil.GetRepositoryCacheKey(new Guid?(repositoryInfo.Key.ProjectId), repositoryInfo.Name);
      string repositoryCacheKey2 = GitRequestContextCacheUtil.GetRepositoryCacheKey(new Guid?(repositoryInfo.Key.RepoId));
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, repositoryCacheKey1, (object) repositoryInfo);
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, repositoryCacheKey2, (object) repositoryInfo);
    }

    public static void CacheRepositoriesInfo(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitRepositoryInfo> repositoriesInfo)
    {
      foreach (TfsGitRepositoryInfo repositoryInfo in repositoriesInfo)
        GitRequestContextCacheUtil.CacheRepositoryInfo(requestContext, repositoryInfo);
    }

    public static IEnumerable<TfsGitRef> GetRefsOrDefault(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<string> refNames)
    {
      if (repository == null || refNames == null)
        return (IEnumerable<TfsGitRef>) null;
      IEnumerable<TfsGitRef> cachedRefs = GitRequestContextCacheUtil.GetCachedRefs(requestContext, repository.Key?.RepoId, refNames);
      if (cachedRefs != null && cachedRefs.All<TfsGitRef>((Func<TfsGitRef, bool>) (cachedRef => cachedRef != null)))
        return cachedRefs;
      IEnumerable<TfsGitRef> tfsGitRefs = (IEnumerable<TfsGitRef>) repository.Refs.MatchingNames(refNames);
      GitRequestContextCacheUtil.CacheRefs(requestContext, repository.Key?.RepoId, tfsGitRefs);
      IDictionary<string, TfsGitRef> refMap = tfsGitRefs != null ? (IDictionary<string, TfsGitRef>) tfsGitRefs.ToDictionary<TfsGitRef, string, TfsGitRef>((Func<TfsGitRef, string>) (r => r.Name), (Func<TfsGitRef, TfsGitRef>) (r => r)) : (IDictionary<string, TfsGitRef>) null;
      return refNames.Select<string, TfsGitRef>((Func<string, TfsGitRef>) (refName =>
      {
        IDictionary<string, TfsGitRef> dictionary = refMap;
        return (dictionary != null ? (dictionary.ContainsKey(refName) ? 1 : 0) : 0) == 0 ? (TfsGitRef) null : refMap[refName];
      }));
    }

    public static TfsGitRef GetDefaultRefOrDefault(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      if (repository == null)
        return (TfsGitRef) null;
      TfsGitRef defaultRef1;
      if (GitRequestContextCacheUtil.TryGetCachedDefaultRef(requestContext, repository.Key?.RepoId, out defaultRef1))
        return defaultRef1;
      TfsGitRef defaultRef2 = repository.Refs.GetDefault();
      GitRequestContextCacheUtil.CacheDefaultRef(requestContext, repository.Key?.RepoId, defaultRef2);
      return defaultRef2;
    }

    public static TfsGitRef GetDefaultRefOrAny(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      if (repository == null)
        return (TfsGitRef) null;
      TfsGitRef cachedValue = CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitRef>(requestContext, GitRequestContextCacheUtil.GetDefaultRefOrAnyCacheKey(repository.Key?.RepoId));
      if (cachedValue != null)
        return cachedValue;
      TfsGitRef defaultOrAny = repository.Refs.GetDefaultOrAny();
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, GitRequestContextCacheUtil.GetDefaultRefOrAnyCacheKey(repository.Key?.RepoId), (object) defaultOrAny);
      return defaultOrAny;
    }

    internal static IEnumerable<TfsGitRef> GetCachedRefs(
      IVssRequestContext requestContext,
      Guid? repositoryId,
      IEnumerable<string> refNames)
    {
      if (refNames == null)
        return (IEnumerable<TfsGitRef>) null;
      List<TfsGitRef> cachedRefs = new List<TfsGitRef>();
      foreach (string refName in refNames)
        cachedRefs.Add(GitRequestContextCacheUtil.GetCachedRef(requestContext, repositoryId, refName));
      return (IEnumerable<TfsGitRef>) cachedRefs;
    }

    internal static TfsGitRef GetCachedRef(
      IVssRequestContext requestContext,
      Guid? repositoryId,
      string refName)
    {
      string refCacheKey = GitRequestContextCacheUtil.GetRefCacheKey(repositoryId, refName);
      return CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitRef>(requestContext, refCacheKey);
    }

    internal static bool TryGetCachedDefaultRef(
      IVssRequestContext requestContext,
      Guid? repositoryId,
      out TfsGitRef defaultRef)
    {
      defaultRef = (TfsGitRef) null;
      string defaultRefCacheKey = GitRequestContextCacheUtil.GetDefaultRefCacheKey(repositoryId);
      if (!CodeReviewRequestContextCacheUtil.IsCachePopulated(requestContext, defaultRefCacheKey))
        return false;
      defaultRef = (TfsGitRef) requestContext.RootContext.Items[defaultRefCacheKey];
      return true;
    }

    internal static void CacheRefs(
      IVssRequestContext requestContext,
      Guid? repositoryId,
      IEnumerable<TfsGitRef> refs)
    {
      if (refs == null)
        return;
      foreach (TfsGitRef tfsGitRef in refs)
      {
        if (tfsGitRef != null)
        {
          string refCacheKey = GitRequestContextCacheUtil.GetRefCacheKey(repositoryId, tfsGitRef.Name);
          CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, refCacheKey, (object) tfsGitRef);
        }
      }
    }

    internal static void CacheDefaultRef(
      IVssRequestContext requestContext,
      Guid? repositoryId,
      TfsGitRef defaultRef)
    {
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, GitRequestContextCacheUtil.GetDefaultRefCacheKey(repositoryId), (object) defaultRef);
      if (defaultRef == null)
        return;
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, GitRequestContextCacheUtil.GetDefaultRefOrAnyCacheKey(repositoryId), (object) defaultRef);
    }

    public static TfsGitPullRequest GetPullRequestOrDefault(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      bool throwOnError = false)
    {
      if (pullRequestId <= 0)
        return (TfsGitPullRequest) null;
      TfsGitPullRequest cachedPullRequest = GitRequestContextCacheUtil.GetCachedPullRequest(requestContext, pullRequestId);
      if (cachedPullRequest != null)
        return cachedPullRequest;
      TfsGitPullRequest pullRequest = (TfsGitPullRequest) null;
      try
      {
        pullRequest = requestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(requestContext, repository, pullRequestId);
        GitRequestContextCacheUtil.CachePullRequest(requestContext, pullRequest);
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 700345, nameof (GitRequestContextCacheUtil), nameof (GetPullRequestOrDefault), ex, (object) new
        {
          RepoId = repository?.Key?.RepoId,
          pullRequestId = pullRequestId
        }, caller: nameof (GetPullRequestOrDefault));
        if (throwOnError)
          throw;
      }
      return pullRequest;
    }

    internal static TfsGitPullRequest GetCachedPullRequest(
      IVssRequestContext requestContext,
      int pullRequestId)
    {
      string pullRequestCacheKey = GitRequestContextCacheUtil.GetPullRequestCacheKey(pullRequestId);
      return CodeReviewRequestContextCacheUtil.GetCachedValue<TfsGitPullRequest>(requestContext, pullRequestCacheKey);
    }

    internal static void CachePullRequest(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      string pullRequestCacheKey = GitRequestContextCacheUtil.GetPullRequestCacheKey(pullRequest.PullRequestId);
      CodeReviewRequestContextCacheUtil.SetCachedValue(requestContext, pullRequestCacheKey, (object) pullRequest);
    }

    internal static string GetRepositoryCacheKey(Guid? projectId, string repositoryName)
    {
      if (!string.IsNullOrEmpty(repositoryName) && projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return string.Format("{0}{1}.{2}", (object) "_GitRepository#", (object) projectId, (object) repositoryName);
      }
      return (string) null;
    }

    internal static string GetRepositoryCacheKey(Guid? repositoryId)
    {
      if (repositoryId.HasValue)
      {
        Guid? nullable = repositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return string.Format("{0}{1}", (object) "_GitRepository#", (object) repositoryId);
      }
      return (string) null;
    }

    internal static string GetRefCacheKey(Guid? repositoryId, string refName)
    {
      if (repositoryId.HasValue)
      {
        Guid? nullable = repositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0 && !string.IsNullOrEmpty(refName))
          return string.Format("{0}{1}{2}", (object) "_GitRef#", (object) repositoryId, (object) refName);
      }
      return (string) null;
    }

    internal static string GetDefaultRefCacheKey(Guid? repositoryId)
    {
      if (repositoryId.HasValue)
      {
        Guid? nullable = repositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return string.Format("{0}{1}", (object) "_GitDefaultRef#", (object) repositoryId);
      }
      return (string) null;
    }

    internal static string GetDefaultRefOrAnyCacheKey(Guid? repositoryId)
    {
      if (repositoryId.HasValue)
      {
        Guid? nullable = repositoryId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return string.Format("{0}{1}", (object) "_GitDefaultRefOrAny#", (object) repositoryId);
      }
      return (string) null;
    }

    internal static string GetPullRequestCacheKey(int pullRequestId) => "_GitPullRequest#" + pullRequestId.ToString();
  }
}
