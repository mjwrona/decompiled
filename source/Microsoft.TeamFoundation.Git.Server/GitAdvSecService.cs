// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAdvSecService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitAdvSecService : IGitAdvSecService, IVssFrameworkService
  {
    public static readonly Guid AdvSecEnabledAllRepos = new Guid("{FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF}");
    private const string c_layer = "TeamFoundationGitRepositoryService";
    private const string AdvancedSecurityServiceInstanceString = "00000071-0000-8888-8000-000000000000";
    private const string ExternalSecretScanPolicyType = "90F9629B-664B-4804-A560-DD79B0C628F8";
    private static readonly Guid AdvancedSecurityServiceInstance = new Guid("00000071-0000-8888-8000-000000000000");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool CheckS2SCall(IVssRequestContext rc)
    {
      if (rc.ExecutionEnvironment.IsDevFabricDeployment)
        return true;
      IdentityDescriptor authenticatedDescriptor = rc.GetAuthenticatedDescriptor();
      Guid spId;
      return ServicePrincipals.IsServicePrincipal(rc, authenticatedDescriptor, true, out spId) && spId == GitAdvSecService.AdvancedSecurityServiceInstance;
    }

    public void DeleteEnablementData(
      IVssRequestContext rc,
      bool allProjects,
      bool includeBillableCommitters,
      IEnumerable<Guid> projectIds)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.AdvSecDeleteEnablementData(allProjects, includeBillableCommitters, projectIds);
      foreach (Guid projectId in projectIds)
        this.RemoveEnableOnCreateProjectRegKey(rc, projectId);
    }

    public void DeleteRepositoryEnablementData(
      IVssRequestContext rc,
      Guid projectId,
      Guid repositoryId,
      bool includeBillableCommitters)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.AdvSecDeleteRepositoryEnablementData(projectId, repositoryId, includeBillableCommitters);
    }

    public List<GitBillableCommitter> EstimateBillableCommitters(
      IVssRequestContext rc,
      Guid projectId,
      Guid? repositoryId)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecEstimateBillableCommitters(projectId, repositoryId);
    }

    public List<GitBillablePusher> EstimateBillablePushers(IVssRequestContext rc, Guid? projectId)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecEstimateBillablePushers(projectId);
    }

    public bool IsEnabledForRepository(
      IVssRequestContext rc,
      string teamProjectUri,
      Guid repositoryId,
      DateTime? billingDate)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecQueryEnablementStatusForRepository(teamProjectUri, repositoryId, new DateTime?());
    }

    public bool IsEnabledForAnyRepository(IVssRequestContext rc, DateTime? billingDate)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecQueryEnablementStatus((IEnumerable<Guid>) new List<Guid>(), billingDate, false, new int?(), new int?()).Any<GitAdvSecEnablementStatus>((Func<GitAdvSecEnablementStatus, bool>) (status => status.Enabled.HasValue && status.Enabled.Value));
    }

    public void OnProjectCreate(IVssRequestContext rc, Guid projectId, bool isRecovered)
    {
      if (isRecovered || !this.QueryEnableOnCreateHostRegKey(rc))
        return;
      this.UpdateEnableOnCreateProjectRegKey(rc, projectId, true);
      IEnumerable<Guid> projectIds = (IEnumerable<Guid>) new List<Guid>()
      {
        projectId
      };
      IEnumerable<GitAdvSecEnablementUpdate> updates = this.QueryEnablementStatus(rc, projectIds, false, new DateTime?(), new int?(), new int?()).Where<GitAdvSecEnablementStatus>((Func<GitAdvSecEnablementStatus, bool>) (x =>
      {
        bool? enabled = x.Enabled;
        bool flag = true;
        return !(enabled.GetValueOrDefault() == flag & enabled.HasValue);
      })).Select<GitAdvSecEnablementStatus, GitAdvSecEnablementUpdate>((Func<GitAdvSecEnablementStatus, GitAdvSecEnablementUpdate>) (x => new GitAdvSecEnablementUpdate(x.ProjectId, x.RepositoryId, true)));
      this.UpdateEnablementStatus(rc, updates);
      foreach (GitAdvSecEnablementUpdate enablementUpdate in updates)
        this.CreateSecretScanPolicy(rc, enablementUpdate.ProjectId, enablementUpdate.RepositoryId);
    }

    public void OnProjectDisable(IVssRequestContext rc, Guid projectId)
    {
      this.UpdateEnableOnCreateProjectRegKey(rc, projectId, false);
      IEnumerable<Guid> projectIds = (IEnumerable<Guid>) new List<Guid>()
      {
        projectId
      };
      IEnumerable<GitAdvSecEnablementUpdate> updates = this.QueryEnablementStatus(rc, projectIds, true, new DateTime?(), new int?(), new int?()).Where<GitAdvSecEnablementStatus>((Func<GitAdvSecEnablementStatus, bool>) (x =>
      {
        bool? enabled = x.Enabled;
        bool flag = true;
        return enabled.GetValueOrDefault() == flag & enabled.HasValue;
      })).Select<GitAdvSecEnablementStatus, GitAdvSecEnablementUpdate>((Func<GitAdvSecEnablementStatus, GitAdvSecEnablementUpdate>) (x => new GitAdvSecEnablementUpdate(x.ProjectId, x.RepositoryId, false)));
      this.UpdateEnablementStatus(rc, updates);
      this.DeleteSecretScanPolicy(rc, projectId, new Guid?(), new Guid("90F9629B-664B-4804-A560-DD79B0C628F8"));
    }

    public void OnRepositoryCreate(IVssRequestContext rc, RepoKey repoKey)
    {
      if (!this.QueryEnableOnCreateProjectRegKey(rc, repoKey.ProjectId))
        return;
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
      {
        if (gitCoreComponent.RepositoryNameFromId(repoKey.RepoId, true, out RepoKey _, out bool _, out long _, out bool _, out bool _).EndsWith(".wiki", StringComparison.OrdinalIgnoreCase))
        {
          rc.Trace(1013951, TraceLevel.Verbose, GitServerUtils.TraceArea, "TeamFoundationGitRepositoryService", string.Format("Repository {0} was Created.  It ends with .wiki, skipping AdvSec Enablement.", (object) repoKey.RepoId));
          return;
        }
      }
      IList<GitAdvSecEnablementUpdate> updates = (IList<GitAdvSecEnablementUpdate>) new List<GitAdvSecEnablementUpdate>()
      {
        new GitAdvSecEnablementUpdate(repoKey.ProjectId, repoKey.RepoId, true)
      };
      this.UpdateEnablementStatus(rc, (IEnumerable<GitAdvSecEnablementUpdate>) updates);
      this.CreateSecretScanPolicy(rc, repoKey.ProjectId, repoKey.RepoId);
      rc.Trace(1013951, TraceLevel.Verbose, GitServerUtils.TraceArea, "TeamFoundationGitRepositoryService", string.Format("Repository {0} was Created.  AdvSec Created.", (object) repoKey.RepoId));
    }

    public void OnRepositoryDestroy(IVssRequestContext rc, RepoKey repoKey)
    {
      rc.GetService<IGitAdvSecService>().DeleteRepositoryEnablementData(rc, repoKey.ProjectId, repoKey.RepoId, true);
      this.DeleteSecretScanPolicy(rc, repoKey.ProjectId, new Guid?(repoKey.RepoId), new Guid("90F9629B-664B-4804-A560-DD79B0C628F8"));
      rc.Trace(1013951, TraceLevel.Verbose, GitServerUtils.TraceArea, "TeamFoundationGitRepositoryService", string.Format("Repository {0} was Destroyed.  AdvSec Destroyed.", (object) repoKey.RepoId));
    }

    public void OnRepositoryDisable(IVssRequestContext rc, RepoKey repoKey)
    {
      string projectUri = ProjectInfo.GetProjectUri(repoKey.ProjectId);
      if (!this.IsEnabledForRepository(rc, projectUri, repoKey.RepoId, new DateTime?()))
        return;
      IList<GitAdvSecEnablementUpdate> updates = (IList<GitAdvSecEnablementUpdate>) new List<GitAdvSecEnablementUpdate>()
      {
        new GitAdvSecEnablementUpdate(repoKey.ProjectId, repoKey.RepoId, false)
      };
      this.UpdateEnablementStatus(rc, (IEnumerable<GitAdvSecEnablementUpdate>) updates);
      this.DeleteSecretScanPolicy(rc, repoKey.ProjectId, new Guid?(repoKey.RepoId), new Guid("90F9629B-664B-4804-A560-DD79B0C628F8"));
      rc.Trace(1013951, TraceLevel.Verbose, GitServerUtils.TraceArea, "TeamFoundationGitRepositoryService", string.Format("Repository {0} was Disabled.  AdvSec Disabled.", (object) repoKey.RepoId));
    }

    public List<GitBillableCommitter> QueryBillableCommitters(
      IVssRequestContext rc,
      string teamProjectUri,
      DateTime? billingDate,
      int? skip,
      int? take)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecQueryBillableCommitters(teamProjectUri, billingDate, skip, take);
    }

    public List<GitBillableCommitterDetail> QueryBillableCommittersDetailed(
      IVssRequestContext rc,
      string teamProjectUri,
      DateTime? billingDate)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecQueryBillableCommittersDetailed(teamProjectUri, billingDate);
    }

    public List<GitAdvSecEnablementStatus> QueryEnablementStatus(
      IVssRequestContext rc,
      IEnumerable<Guid> projectIds,
      bool includeDeleted,
      DateTime? billingDate,
      int? skip,
      int? take)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        return gitCoreComponent.AdvSecQueryEnablementStatus(projectIds, billingDate, includeDeleted, skip, take);
    }

    public bool QueryEnableOnCreateHostRegKey(IVssRequestContext rc)
    {
      bool result;
      bool.TryParse(rc.GetService<IVssRegistryService>().GetValue(rc, (RegistryQuery) "/Service/Git/Settings/AdvSec/EnableOnCreateHost", "false"), out result);
      return result;
    }

    public bool QueryEnableOnCreateProjectRegKey(IVssRequestContext rc, Guid projectId)
    {
      bool result;
      bool.TryParse(rc.GetService<IVssRegistryService>().GetValue(rc, (RegistryQuery) ("/Service/Git/Settings/AdvSec/EnableOnCreateProject/" + projectId.ToString()), "false"), out result);
      return result;
    }

    public void RemoveEnableOnCreateHostRegKey(IVssRequestContext rc) => rc.GetService<IVssRegistryService>().SetValue(rc, "/Service/Git/Settings/AdvSec/EnableOnCreateHost", (object) null);

    public void RemoveEnableOnCreateProjectRegKey(IVssRequestContext rc, Guid projectId) => rc.GetService<IVssRegistryService>().SetValue(rc, "/Service/Git/Settings/AdvSec/EnableOnCreateProject/" + projectId.ToString(), (object) null);

    public void UpdateEnablementStatus(
      IVssRequestContext rc,
      IEnumerable<GitAdvSecEnablementUpdate> updates)
    {
      int num = !rc.ExecutionEnvironment.IsDevFabricDeployment ? 14400 : 0;
      foreach (GitAdvSecEnablementUpdate update in updates)
      {
        if (update.NewStatus)
        {
          this.UpdatePluginWatermarkToLastCommit(rc, new RepoKey(update.ProjectId, update.RepositoryId), "d93e72b8-37b6-4b95-bc1d-c7619933fd52");
          IVssRequestContext requestContext = rc;
          RepoJobKey targetKey = new RepoJobKey();
          targetKey.RepositoryId = update.RepositoryId;
          int maxDelaySeconds = num;
          KeyScopedJobUtil.QueueFor<RepoJobKey>(requestContext, targetKey, "GitAdvSecBillableCommittersBackfillJob", "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitAdvSecBillableCommittersBackfillJob", JobPriorityLevel.Normal, JobPriorityClass.Normal, maxDelaySeconds);
        }
      }
      this.UpdateEnablementStatusInDB(rc, updates);
      foreach (GitAdvSecEnablementUpdate update in updates)
      {
        if (update.NewStatus)
        {
          KeyScopedJobUtil.QueueFor<RepoWithProjectJobKey>(rc, new RepoWithProjectJobKey()
          {
            ProjectId = update.ProjectId,
            RepositoryId = update.RepositoryId
          }, "GitAdvSecInitializePermissionsJob", "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitAdvSecInitializePermissionsJob", JobPriorityLevel.Normal, JobPriorityClass.Normal);
          KeyScopedJobUtil.QueueFor<RepoJobKey>(rc, new RepoJobKey()
          {
            RepositoryId = update.RepositoryId
          }, "GitAdvSecCommitScanningJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitAdvSecCommitScanningJob", JobPriorityLevel.BelowNormal, JobPriorityClass.Normal);
        }
      }
    }

    public void UpdateEnableOnCreateHostRegKey(IVssRequestContext rc, bool value) => rc.GetService<IVssRegistryService>().SetValue<string>(rc, "/Service/Git/Settings/AdvSec/EnableOnCreateHost", value.ToString());

    public void UpdateEnableOnCreateProjectRegKey(
      IVssRequestContext rc,
      Guid projectId,
      bool value)
    {
      rc.GetService<IVssRegistryService>().SetValue<string>(rc, "/Service/Git/Settings/AdvSec/EnableOnCreateProject/" + projectId.ToString(), value.ToString());
    }

    internal virtual OdbId GetOdb(ITfsGitRepository gitRepository) => GitServerUtils.GetOdb(gitRepository).Id;

    internal virtual void UpdateEnablementStatusInDB(
      IVssRequestContext rc,
      IEnumerable<GitAdvSecEnablementUpdate> updates)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.AdvSecUpdateEnablementStatus(rc.GetUserId(), updates);
    }

    internal virtual void UpdatePluginWatermarkToLastCommit(
      IVssRequestContext rc,
      RepoKey repoKey,
      string jobIdString)
    {
      Guid jobId = Guid.Parse(jobIdString);
      TfsGitProcessedCommit gitProcessedCommit = (TfsGitProcessedCommit) null;
      using (GitCoreComponent gitCoreComponent = rc.CreateReadOnlyGitCoreComponent())
        gitProcessedCommit = gitCoreComponent.AdvSecQueryLastCommitForPluginWatermark(repoKey, jobId, 1).LastOrDefault<TfsGitProcessedCommit>();
      if (gitProcessedCommit == null)
        return;
      List<UnprocessedCommit> commits = new List<UnprocessedCommit>();
      using (ITfsGitRepository repositoryById = rc.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(rc.Elevate(), repoKey.RepoId))
      {
        if (repositoryById.TryLookupObject(gitProcessedCommit.CommitId) is TfsGitCommit commit)
          commits.Add(UnprocessedCommit.FromTfsGitCommit(repositoryById.Key, commit, gitProcessedCommit.PushId));
      }
      if (commits.Count == 0)
        return;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.MarkProcessedCommits(repoKey, jobId, (IEnumerable<UnprocessedCommit>) commits);
    }

    private void CreateSecretScanPolicy(IVssRequestContext rc, Guid projectId, Guid repoId)
    {
      IVssRequestContext vssRequestContext = rc.Elevate();
      vssRequestContext.GetService<ITeamFoundationPolicyService>().CreatePolicyConfiguration(vssRequestContext, new Guid("90F9629B-664B-4804-A560-DD79B0C628F8"), projectId, true, true, false, JObject.Parse("{\"suppressionFile\":null,\"avoidThrowingErrors\":null,\"scope\":[{\"repositoryId\":\"" + repoId.ToString() + "\"}]}").ToString());
    }

    private void DeleteSecretScanPolicy(
      IVssRequestContext rc,
      Guid projectId,
      Guid? repoId,
      Guid secretScanPolicyType)
    {
      IList<PolicyConfigurationRecord> configurationRecordList = !repoId.HasValue ? (IList<PolicyConfigurationRecord>) this.GetSecretScanPoliciesForProject(rc, projectId, secretScanPolicyType).ToList<PolicyConfigurationRecord>() : (IList<PolicyConfigurationRecord>) this.GetSecretScanPolicyForRepo(rc, projectId, repoId.Value, secretScanPolicyType).ToList<PolicyConfigurationRecord>();
      if (configurationRecordList == null || configurationRecordList.Count <= 0)
        return;
      ITeamFoundationPolicyService service = rc.GetService<ITeamFoundationPolicyService>();
      foreach (PolicyConfigurationRecord configurationRecord in (IEnumerable<PolicyConfigurationRecord>) configurationRecordList)
        service.DeletePolicyConfiguration(rc, configurationRecord.ProjectId, configurationRecord.ConfigurationId);
    }

    private IEnumerable<PolicyConfigurationRecord> GetSecretScanPoliciesForProject(
      IVssRequestContext rc,
      Guid projectId,
      Guid secretScanPolicyType)
    {
      return rc.GetService<ITeamFoundationPolicyService>().GetLatestPolicyConfigurationRecords(rc, projectId, int.MaxValue, 1, out int? _, new Guid?(secretScanPolicyType));
    }

    private IEnumerable<PolicyConfigurationRecord> GetSecretScanPolicyForRepo(
      IVssRequestContext rc,
      Guid projectId,
      Guid repoId,
      Guid secretScanPolicyType)
    {
      ITeamFoundationPolicyService service = rc.GetService<ITeamFoundationPolicyService>();
      IEnumerable<string> strings = (IEnumerable<string>) new List<string>()
      {
        repoId.ToString().Replace("-", string.Empty)
      };
      IVssRequestContext requestContext = rc;
      Guid projectId1 = projectId;
      IEnumerable<string> scopes = strings;
      int? nullable;
      ref int? local = ref nullable;
      Guid? policyType = new Guid?(secretScanPolicyType);
      return service.GetLatestPolicyConfigurationRecordsByScope(requestContext, projectId1, scopes, int.MaxValue, 1, out local, policyType);
    }
  }
}
