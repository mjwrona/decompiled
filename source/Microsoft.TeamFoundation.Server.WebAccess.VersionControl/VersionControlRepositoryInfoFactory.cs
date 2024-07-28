// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlRepositoryInfoFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public class VersionControlRepositoryInfoFactory : IDisposable
  {
    public const string c_defaultRepositoryPath = "Git/DefaultRepository";
    private const string c_TfvcRepositoryPrefix = "$/";
    private const string c_gitDefaultRepositoryInformation = "_GitDefaultRepositoryInformation";
    private const string c_gitDefaultRepositoryValidated = "_GitDefaultRepositoryValidated";
    private const string c_defaultRepositoryIsGit = "_DefaultRepositoryIsGit";
    private string m_defaultRepositoryName;
    private Guid m_defaultRepositoryId;
    private TfsRepositoryInfo m_tfsRepositoryInfo;
    private VersionControlProjectInfo m_versionControlProjectInfo;
    private IList<GitRepository> m_gitRepositories;
    private Dictionary<string, GitRepositoryInfo> m_gitRepositoriesByName = new Dictionary<string, GitRepositoryInfo>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
    private Dictionary<Guid, GitRepositoryInfo> m_gitRepositoriesById = new Dictionary<Guid, GitRepositoryInfo>();
    private bool? m_hasTfvcReadPermissions;
    private bool m_allowLoadingRepoSettings;
    private ProjectInfo m_project;

    public VersionControlRepositoryInfoFactory(
      IVssRequestContext requestContext,
      IEnumerable<ITfsGitRepository> cachedRepositories = null,
      bool allowLoadingRepoSettings = false)
    {
      this.RequestContext = requestContext;
      this.m_allowLoadingRepoSettings = allowLoadingRepoSettings;
      if (cachedRepositories == null)
        return;
      this.CacheRepositories(cachedRepositories);
    }

    public VersionControlRepositoryInfoFactory(
      TfsWebContext webContext,
      IEnumerable<ITfsGitRepository> cachedRepositories = null,
      bool allowLoadingRepoSettings = false)
    {
      this.RequestContext = webContext.TfsRequestContext;
      this.m_allowLoadingRepoSettings = allowLoadingRepoSettings;
      if (cachedRepositories == null)
        return;
      this.CacheRepositories(cachedRepositories);
    }

    public void Dispose()
    {
      foreach (GitRepositoryInfo gitRepositoryInfo in this.m_gitRepositoriesById.Values)
        gitRepositoryInfo.GitProvider.Repository.Dispose();
      this.m_gitRepositoriesByName.Clear();
      this.m_gitRepositoriesById.Clear();
    }

    public IVssRequestContext RequestContext { get; private set; }

    public ProjectInfo Project
    {
      get
      {
        if (this.m_project == null)
        {
          IVssRequestContext requestContext = this.RequestContext;
          this.m_project = (requestContext != null ? requestContext.GetService<IRequestProjectService>() : (IRequestProjectService) null)?.GetProject(this.RequestContext);
        }
        return this.m_project;
      }
    }

    public IEnumerable<TfsGitRepositoryInfo> GetCachedRepositoriesInfo() => this.m_gitRepositoriesById.Values.Select<GitRepositoryInfo, TfsGitRepositoryInfo>((Func<GitRepositoryInfo, TfsGitRepositoryInfo>) (gitRepoInfo =>
    {
      string repositoryName = gitRepoInfo.GitProvider.RepositoryName;
      RepoKey key = gitRepoInfo.GitProvider.Repository.Key;
      bool isInMaintenance = gitRepoInfo.GitProvider.Repository.IsInMaintenance;
      DateTime createdDate = new DateTime();
      DateTime lastMetadataUpdate = new DateTime();
      int num = isInMaintenance ? 1 : 0;
      return new TfsGitRepositoryInfo(repositoryName, key, createdDate: createdDate, lastMetadataUpdate: lastMetadataUpdate, isInMaintenance: num != 0);
    }));

    public void CacheRepositories(IEnumerable<ITfsGitRepository> cachedRepositories)
    {
      foreach (ITfsGitRepository cachedRepository in cachedRepositories)
        this.CacheRepository(cachedRepository);
    }

    public GitRepositoryInfo CacheRepository(ITfsGitRepository repository)
    {
      GitRepositoryInfo gitRepositoryInfo = new GitRepositoryInfo(this.RequestContext, repository);
      string key1 = string.Format("{0}.{1}", (object) repository.Key.ProjectId, (object) repository.Name);
      string key2 = string.Format("{0}.{1}", (object) repository.Key.ProjectId, (object) repository.Key.RepoId);
      if (!this.m_gitRepositoriesById.ContainsKey(repository.Key.RepoId) || !this.m_gitRepositoriesByName.ContainsKey(key1))
      {
        this.m_gitRepositoriesByName[key1] = gitRepositoryInfo;
        this.m_gitRepositoriesById[repository.Key.RepoId] = gitRepositoryInfo;
        this.m_gitRepositoriesByName[key2] = gitRepositoryInfo;
      }
      return gitRepositoryInfo;
    }

    public bool HasTfsVersionControl => this.ProjectVersionControlInfo == null || this.ProjectVersionControlInfo.SupportsTFVC;

    public bool HasTfvcReadPermissions
    {
      get
      {
        if (!this.m_hasTfvcReadPermissions.HasValue)
          this.m_hasTfvcReadPermissions = this.RequestContext == null ? new bool?(false) : new bool?(VersionControlRepositoryInfoFactory.HasTfvcReadPermission(this.RequestContext));
        return this.m_hasTfvcReadPermissions.Value;
      }
    }

    private static bool HasTfvcReadPermission(IVssRequestContext requestContext) => requestContext.GetService<IContributionClaimService>().HasClaim(requestContext, "member");

    public string DefaultGitRepositoryName
    {
      get
      {
        if (!VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(this.DefaultRepositoryName))
          return this.DefaultRepositoryName;
        GitRepository repositoryOrDefault = this.GetUserLastGitRepositoryOrDefault(this.DefaultRepositoryName);
        return repositoryOrDefault == null ? this.Project.Name : repositoryOrDefault.Name;
      }
    }

    public bool DefaultRepoCanFork => this.m_allowLoadingRepoSettings && !VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(this.DefaultRepositoryName) && this.GetGitRepositoryInfoOrNull(this.DefaultRepositoryName)?.GitProvider?.Repository?.Settings?.AllowedForkTargets.GetValueOrDefault() != 0;

    public bool DefaultRepoIsFork
    {
      get
      {
        if (VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(this.DefaultRepositoryName))
          return false;
        bool? isFork = this.GetGitRepositoryInfoOrNull(this.DefaultRepositoryName)?.GitProvider?.Repository?.IsFork;
        return isFork.HasValue && isFork.Value;
      }
    }

    public string DefaultRepositoryName
    {
      get
      {
        if (this.m_defaultRepositoryName == null)
        {
          string str = (string) null;
          if (this.ProjectVersionControlInfo != null && this.ProjectVersionControlInfo.SupportsGit)
          {
            if (this.ProjectVersionControlInfo.SupportsTFVC)
            {
              str = VersionControlRepositoryInfoFactory.GetUserLastRepositoryName(this.RequestContext);
              if (string.IsNullOrEmpty(str) && this.HasTfvcReadPermissions)
                str = "$/";
            }
          }
          else
            str = "$/";
          if (VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(str))
          {
            this.m_defaultRepositoryName = this.GetTfvcRepositoryName();
          }
          else
          {
            GitRepository repositoryOrDefault = this.GetUserLastGitRepositoryOrDefault(str);
            if (repositoryOrDefault != null)
            {
              this.m_defaultRepositoryName = repositoryOrDefault.Name;
              this.m_defaultRepositoryId = repositoryOrDefault.Id;
            }
            else
              this.m_defaultRepositoryName = this.Project.Name;
          }
        }
        this.m_defaultRepositoryName = this.m_defaultRepositoryName ?? "";
        return this.m_defaultRepositoryName;
      }
    }

    public bool DefaultRepositoryIsGit => !this.DefaultRepositoryIsTfvc;

    public bool DefaultRepositoryIsTfvc => VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(this.DefaultRepositoryName);

    public Guid DefaultRepositoryId
    {
      get
      {
        string defaultRepositoryName = this.DefaultRepositoryName;
        return this.m_defaultRepositoryId;
      }
    }

    public string GetTfvcRepositoryName() => "$/" + (this.Project == null ? string.Empty : this.Project.Name);

    private void SetDefaultRepositoryNameAndId(string name, Guid id)
    {
      this.m_defaultRepositoryName = name;
      this.m_defaultRepositoryId = id;
    }

    private static string GetUserLastRepositoryName(
      IVssRequestContext requestContext,
      ProjectInfo project = null)
    {
      if (project == null)
        project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      string lastRepositoryName = (string) null;
      if (project != null)
        lastRepositoryName = requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.User, "Project", project.Id.ToString(), "Git/DefaultRepository", (string) null, false);
      return lastRepositoryName;
    }

    internal static GitRepository GetUserDefaultGitRepository(
      IVssRequestContext requestContext,
      IEnumerable<GitRepository> gitRepositories,
      Guid teamProjectId,
      string teamProjectName,
      string defaultRepositoryName = null)
    {
      if (!gitRepositories.Any<GitRepository>())
        return (GitRepository) null;
      if (gitRepositories.Count<GitRepository>() == 1)
        return gitRepositories.First<GitRepository>();
      string userLastRepositoryName = defaultRepositoryName ?? VersionControlRepositoryInfoFactory.GetUserLastRepositoryName(requestContext);
      if (!string.IsNullOrEmpty(userLastRepositoryName))
      {
        GitRepository defaultGitRepository = gitRepositories.FirstOrDefault<GitRepository>((Func<GitRepository, bool>) (repo => string.Equals(userLastRepositoryName, repo.Name, StringComparison.CurrentCultureIgnoreCase) || string.Equals(userLastRepositoryName, repo.Id.ToString(), StringComparison.CurrentCultureIgnoreCase)));
        if (defaultGitRepository != null)
          return defaultGitRepository;
      }
      return gitRepositories.FirstOrDefault<GitRepository>((Func<GitRepository, bool>) (repo => string.Equals(repo.Name, teamProjectName, StringComparison.CurrentCultureIgnoreCase))) ?? gitRepositories.First<GitRepository>();
    }

    internal GitRepository GetUserLastGitRepositoryOrDefault(string defaultRepositoryName)
    {
      GitRepositoryInfo gitRepositoryInfo = (GitRepositoryInfo) null;
      string str = defaultRepositoryName ?? VersionControlRepositoryInfoFactory.GetUserLastRepositoryName(this.RequestContext);
      if (!string.IsNullOrEmpty(str))
        gitRepositoryInfo = this.GetGitRepositoryInfoOrNull(str);
      if (gitRepositoryInfo == null && (string.IsNullOrEmpty(str) || !TFStringComparer.TeamProjectName.Equals(this.Project.Name, str)))
        gitRepositoryInfo = this.GetGitRepositoryInfoOrNull(this.Project.Name);
      GitRepository repositoryOrDefault;
      if (gitRepositoryInfo == null)
      {
        repositoryOrDefault = this.GitRepositories.FirstOrDefault<GitRepository>();
        if (repositoryOrDefault != null)
          VersionControlRepositoryInfoFactory.SaveDefaultRepository(this.RequestContext, repositoryOrDefault.Name);
      }
      else
        repositoryOrDefault = gitRepositoryInfo.GitProvider.GetRepository(false);
      return repositoryOrDefault;
    }

    public TfsRepositoryInfo TfsRepositoryInfo
    {
      get
      {
        if (this.m_tfsRepositoryInfo == null)
          this.m_tfsRepositoryInfo = new TfsRepositoryInfo(this.RequestContext);
        return this.m_tfsRepositoryInfo;
      }
    }

    public VersionControlProjectInfo ProjectVersionControlInfo
    {
      get
      {
        if (this.m_versionControlProjectInfo == null && this.Project != null)
        {
          this.m_versionControlProjectInfo = TeamProjectVersionControlInfoUtil.GetProjectInfo(this.RequestContext, this.Project.Id);
          this.m_versionControlProjectInfo.SupportsTFVC = this.m_versionControlProjectInfo.SupportsTFVC && this.HasTfvcReadPermissions;
        }
        return this.m_versionControlProjectInfo;
      }
    }

    public IEnumerable<GitRepository> GitRepositories
    {
      get
      {
        if (this.m_gitRepositories == null)
          this.m_gitRepositories = this.ProjectVersionControlInfo == null || !this.ProjectVersionControlInfo.SupportsGit ? (IList<GitRepository>) new List<GitRepository>() : (IList<GitRepository>) this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(this.RequestContext, this.Project.Uri, true).Select<TfsGitRepositoryInfo, GitRepository>((Func<TfsGitRepositoryInfo, GitRepository>) (x => x.ToWebApiItem(this.RequestContext, this.Project))).ToList<GitRepository>();
        return (IEnumerable<GitRepository>) this.m_gitRepositories;
      }
    }

    public VersionControlRepositoryInfo GetRepositoryInfo(
      string repositoryName = null,
      VersionControlRepositoryType? requiredType = null)
    {
      VersionControlRepositoryInfo repositoryInfo = (VersionControlRepositoryInfo) null;
      if (string.IsNullOrEmpty(repositoryName))
      {
        if (requiredType.HasValue && requiredType.Value == VersionControlRepositoryType.TFS)
          return (VersionControlRepositoryInfo) this.TfsRepositoryInfo;
        repositoryInfo = !this.ProjectVersionControlInfo.SupportsGit ? (VersionControlRepositoryInfo) this.TfsRepositoryInfo : (VersionControlRepositoryInfo) this.GetGitRepositoryInfo(this.DefaultGitRepositoryName, true);
      }
      if (repositoryInfo == null)
        repositoryInfo = !VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(repositoryName) ? (VersionControlRepositoryInfo) this.GetGitRepositoryInfo(repositoryName, true) : (VersionControlRepositoryInfo) this.TfsRepositoryInfo;
      if (requiredType.HasValue && repositoryInfo.RepositoryType != requiredType.Value)
        throw new ArgumentException(string.Format(VCServerResources.ErrorRepositoryWithNameIsNotOfExpectedType, (object) repositoryName, (object) this.Project.Name, (object) requiredType.Value, (object) repositoryInfo.RepositoryType)).Expected(this.RequestContext.ServiceName);
      return repositoryInfo;
    }

    public GitRepositoryInfo GetGitRepositoryById(Guid repositoryId)
    {
      GitRepositoryInfo gitRepositoryById;
      if (!this.m_gitRepositoriesById.TryGetValue(repositoryId, out gitRepositoryById))
      {
        ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repository = (ITfsGitRepository) null;
        try
        {
          repository = service.FindRepositoryById(this.RequestContext, repositoryId);
          gitRepositoryById = this.CacheRepository(repository);
          repository = (ITfsGitRepository) null;
        }
        finally
        {
          repository?.Dispose();
        }
      }
      return gitRepositoryById;
    }

    private GitRepositoryInfo GetGitRepositoryInfo(string repositoryName, bool includeDisabled = false)
    {
      GitRepositoryInfo gitRepositoryInfo = (GitRepositoryInfo) null;
      ProjectInfo project = this.Project;
      Guid guid = project != null ? project.Id : Guid.Empty;
      if (guid == Guid.Empty || !this.m_gitRepositoriesByName.TryGetValue(string.Format("{0}.{1}", (object) guid, (object) repositoryName), out gitRepositoryInfo))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(this.RequestContext, "RequestContext");
        ITeamFoundationGitRepositoryService service = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repository = (ITfsGitRepository) null;
        try
        {
          repository = service.FindRepositoryByName(this.RequestContext, this.Project.Name, repositoryName, includeDisabled);
          gitRepositoryInfo = this.CacheRepository(repository);
          repository = (ITfsGitRepository) null;
        }
        finally
        {
          repository?.Dispose();
        }
      }
      return gitRepositoryInfo;
    }

    private GitRepositoryInfo GetGitRepositoryInfoOrNull(
      string repositoryName,
      bool includeDisabled = false)
    {
      GitRepositoryInfo repositoryInfoOrNull = (GitRepositoryInfo) null;
      try
      {
        repositoryInfoOrNull = this.GetGitRepositoryInfo(repositoryName, includeDisabled);
      }
      catch (GitRepositoryNotFoundException ex)
      {
      }
      catch (GitNeedsPermissionException ex)
      {
      }
      return repositoryInfoOrNull;
    }

    private static bool IsTfvcRepositoryName(string repositoryName) => repositoryName != null && repositoryName.StartsWith("$/", StringComparison.OrdinalIgnoreCase);

    public string GetRequestGitRepositoryName(IVssRequestContext requestContext)
    {
      string repositoryName = (string) null;
      string routeValue = requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "vctype");
      if (routeValue != "git" && routeValue != "tfvc")
        repositoryName = VersionControlRepositoryInfoFactory.GetUserLastRepositoryName(requestContext);
      if (string.IsNullOrEmpty(repositoryName))
      {
        DefaultRepositoryInformation repositoryInformation = this.GetDefaultRepositoryInformation(requestContext);
        if (repositoryInformation.DefaultRepoIsGit)
          repositoryName = repositoryInformation.DefaultGitRepoName;
      }
      if (VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(repositoryName))
        repositoryName = (string) null;
      return repositoryName;
    }

    public DefaultRepositoryInformation GetDefaultRepositoryInformation(
      IVssRequestContext requestContext)
    {
      DefaultRepositoryInformation repositoryInformation;
      requestContext.RootContext.Items.TryGetValue<DefaultRepositoryInformation>("_GitDefaultRepositoryInformation", out repositoryInformation);
      if (repositoryInformation == null)
      {
        repositoryInformation = (DefaultRepositoryInformation) null;
        if (requestContext.GetService<IRequestProjectService>()?.GetProject(requestContext) == null)
        {
          repositoryInformation = new DefaultRepositoryInformation(requestContext);
        }
        else
        {
          using (PerformanceTimer.StartMeasure(requestContext, "VersionControlRepositoryInfo.GetDefaultRepositoryInformation"))
            repositoryInformation = this.GetUpdatedRepositoryInformation(requestContext);
        }
        if (repositoryInformation != null)
          requestContext.RootContext.Items["_GitDefaultRepositoryInformation"] = (object) repositoryInformation;
      }
      return repositoryInformation;
    }

    public static bool GetDefaultRepositoryIsGit(IVssRequestContext requestContext)
    {
      DefaultRepositoryInformation repositoryInformation;
      if (requestContext.RootContext.Items.TryGetValue<DefaultRepositoryInformation>("_DefaultRepositoryIsGit", out repositoryInformation))
        return repositoryInformation.DefaultRepoIsGit;
      bool defaultRepositoryIsGit;
      if (requestContext.RootContext.Items.TryGetValue<bool>("_DefaultRepositoryIsGit", out defaultRepositoryIsGit))
        return defaultRepositoryIsGit;
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project != null)
      {
        VersionControlProjectInfo projectInfo = TeamProjectVersionControlInfoUtil.GetProjectInfo(requestContext, project.Id);
        if (projectInfo.SupportsGit)
        {
          defaultRepositoryIsGit = true;
          if (projectInfo.SupportsTFVC && VersionControlRepositoryInfoFactory.HasTfvcReadPermission(requestContext))
          {
            string lastRepositoryName = VersionControlRepositoryInfoFactory.GetUserLastRepositoryName(requestContext, project);
            if (string.IsNullOrEmpty(lastRepositoryName) || VersionControlRepositoryInfoFactory.IsTfvcRepositoryName(lastRepositoryName))
              defaultRepositoryIsGit = false;
          }
        }
      }
      requestContext.RootContext.Items["_DefaultRepositoryIsGit"] = (object) defaultRepositoryIsGit;
      return defaultRepositoryIsGit;
    }

    private DefaultRepositoryInformation GetUpdatedRepositoryInformation(
      IVssRequestContext requestContext)
    {
      string str = (string) null;
      string repositoryName = (string) null;
      DefaultRepositoryInformation repositoryInfo = new DefaultRepositoryInformation(requestContext);
      string routeValue = requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "vctype");
      if ((routeValue == "git" || routeValue == "tfvc") && this.ProjectVersionControlInfo != null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "VersionControlRepositoryInfo.UpdateDefaultRepository"))
        {
          WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
          if (routeValue == "git" && this.ProjectVersionControlInfo.SupportsGit)
          {
            repositoryName = GitItemUtility.GetGitRepositoryNameFromUri(pageSource.Uri);
            if (string.IsNullOrEmpty(repositoryName) && pageSource.Navigation?.RouteValues != null)
              pageSource.Navigation.RouteValues.TryGetValue("GitRepositoryName", out repositoryName);
            if (!string.IsNullOrEmpty(repositoryName))
            {
              GitRepositoryInfo repositoryInfoOrNull = this.GetGitRepositoryInfoOrNull(repositoryName, true);
              if (repositoryInfoOrNull != null)
              {
                GitRepository repository = repositoryInfoOrNull.GitProvider.GetRepository(false);
                bool valueOrDefault = repository.IsDisabled.GetValueOrDefault();
                if (!valueOrDefault)
                {
                  str = repository.Name;
                  this.SetDefaultRepositoryNameAndId(str, repository.Id);
                }
                else if (valueOrDefault)
                {
                  repositoryInfo.DisabledRepoName = repositoryName;
                  this.SetDefaultRepositoryNameAndId(repository.Name, repository.Id);
                }
              }
            }
          }
          if (routeValue == "tfvc" && VersionControlRepositoryInfoFactory.IsTfvcUri(pageSource.Uri))
          {
            repositoryName = "$/" + pageSource.Project.Name;
            if (this.ProjectVersionControlInfo.SupportsTFVC)
            {
              str = repositoryName;
              this.SetDefaultRepositoryNameAndId(str, Guid.Empty);
            }
          }
          if (!string.IsNullOrEmpty(str))
            VersionControlRepositoryInfoFactory.SaveDefaultRepository(requestContext, str);
          else if (!string.IsNullOrEmpty(repositoryInfo.DisabledRepoName))
          {
            VersionControlRepositoryInfoFactory.SaveDefaultRepository(requestContext, repositoryInfo.DisabledRepoName);
            repositoryInfo.NotFoundRepoName = repositoryName;
          }
          else if (!string.IsNullOrEmpty(repositoryName))
            repositoryInfo.NotFoundRepoName = repositoryName;
        }
      }
      repositoryInfo.DefaultRepoIsGit = this.DefaultRepositoryIsGit;
      repositoryInfo.SupportsTfvc = this.ProjectVersionControlInfo.SupportsTFVC;
      repositoryInfo.DefaultRepoId = this.DefaultRepositoryId;
      if (repositoryInfo.DefaultRepoIsGit)
      {
        repositoryInfo.DefaultGitRepoName = this.DefaultGitRepositoryName;
        repositoryInfo.DefaultRepoCanFork = this.DefaultRepoCanFork;
        repositoryInfo.DefaultRepoIsFork = this.DefaultRepoIsFork;
        this.SetSecuredObject(repositoryInfo);
      }
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project != null)
        repositoryInfo.DefaultGitBranchName = VersionControlSettings.ReadDefaultBranchName(this.RequestContext, new Guid?(project.Id));
      if (string.IsNullOrEmpty(repositoryInfo.DefaultGitBranchName))
        repositoryInfo.DefaultGitBranchName = VersionControlSettings.ReadDefaultBranchName(this.RequestContext, new Guid?());
      if (string.IsNullOrEmpty(repositoryInfo.DefaultGitBranchName))
        repositoryInfo.DefaultGitBranchName = this.RequestContext.IsFeatureEnabled("Git.DefaultBranchIsMain") ? "main" : "master";
      return repositoryInfo;
    }

    private static void SaveDefaultRepository(
      IVssRequestContext requestContext,
      string repositoryName)
    {
      IRequestProjectService service1 = requestContext.GetService<IRequestProjectService>();
      ISettingsService service2 = requestContext.GetService<ISettingsService>();
      IVssRequestContext requestContext1 = requestContext;
      ProjectInfo project = service1.GetProject(requestContext1);
      string str = service2.GetValue<string>(requestContext, SettingsUserScope.User, "Project", project.Id.ToString(), "Git/DefaultRepository", (string) null, false);
      if (str != null && str.Equals(repositoryName))
        return;
      service2.SetValue(requestContext, SettingsUserScope.User, "Project", project.Id.ToString(), "Git/DefaultRepository", (object) repositoryName, false);
    }

    private static bool IsTfvcUri(Uri pageUri)
    {
      char[] separators = "/".ToCharArray();
      return ((IEnumerable<string>) pageUri.Segments).Select<string, string>((Func<string, string>) (x => x.Trim(separators))).Any<string>((Func<string, bool>) (x => string.Equals(x, "_versionControl", StringComparison.OrdinalIgnoreCase)));
    }

    private void SetSecuredObject(DefaultRepositoryInformation repositoryInfo)
    {
      ISecuredObject securedObject = !repositoryInfo.DefaultRepoId.Equals(Guid.Empty) ? GitSecuredObjectFactory.CreateRepositoryReadOnly(this.ProjectVersionControlInfo.Project.Id, repositoryInfo.DefaultRepoId) : GitSecuredObjectFactory.CreateProjectReadOnly(this.ProjectVersionControlInfo.Project.Id);
      repositoryInfo.SetSecuredObject(securedObject);
    }
  }
}
