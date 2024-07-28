// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Forks.GitForkService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server.Forks
{
  internal sealed class GitForkService : 
    TeamFoundationGitAsyncOperationService<ForkFetchAsyncOp, ForkFetchParams, GitForkOperationStatusDetail>,
    IGitForkService,
    IVssFrameworkService
  {
    private static readonly GitForkOperationStatusDetail s_defaultStatusDetail = new GitForkOperationStatusDetail()
    {
      CurrentStep = 0,
      AllSteps = (IEnumerable<string>) Enum.GetNames(typeof (ForkUpdateStep))
    };
    private const string c_layer = "GitForkService";
    private const string c_initialJobIntervalInSecondsRegistryPath = "/Service/Git/Settings/Forks/InitialJobIntervalInSeconds";
    private const int c_defaultInitialJobIntervalInSeconds = 15;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<GitRepositoryRef> QueryChildren(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      Guid collectionId)
    {
      requestContext.CheckProjectCollectionRequestContext();
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      IReadOnlyList<MinimalGlobalRepoKey> inDb;
      using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(repoKey))
        inDb = gitOdbComponent.QueryChildren(new MinimalGlobalRepoKey(Guid.Empty, repoKey.RepoId), collectionId);
      return GitForkService.BuildGitRepositoryRefs(requestContext, (IEnumerable<MinimalGlobalRepoKey>) inDb);
    }

    public GitRepositoryRef GetParent(IVssRequestContext requestContext, RepoKey repoKey)
    {
      requestContext.CheckProjectCollectionRequestContext();
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      MinimalGlobalRepoKey? parent;
      using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(repoKey))
        parent = gitOdbComponent.GetParent(new MinimalGlobalRepoKey(Guid.Empty, repoKey.RepoId));
      if (!parent.HasValue)
        return (GitRepositoryRef) null;
      return GitForkService.BuildGitRepositoryRefs(requestContext, (IEnumerable<MinimalGlobalRepoKey>) new MinimalGlobalRepoKey[1]
      {
        parent.Value
      }).FirstOrDefault<GitRepositoryRef>();
    }

    public ITfsGitRepository CreateFork(
      IVssRequestContext requestContext,
      GlobalGitRepositoryKey source,
      Guid targetProjectId,
      string newRepositoryName)
    {
      requestContext.CheckProjectCollectionRequestContext();
      if ((source.CollectionId == requestContext.ServiceHost.InstanceId ? 1 : (source.CollectionId == Guid.Empty ? 1 : 0)) == 0)
        throw new ArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Get("CrossCollectionForkingUnsupported"));
      Guid empty = Guid.Empty;
      OdbId odbId;
      string projectUri;
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, source.RepositoryId))
      {
        if (repositoryById.Settings.AllowedForkTargets == AllowedForkTargets.Nowhere)
          throw new ArgumentException(Microsoft.TeamFoundation.Git.Server.Resources.Format("ForkingDisabledForRepository", (object) source.RepositoryId, (object) source.CollectionId));
        odbId = repositoryById.Key.OdbId;
        projectUri = ProjectInfo.GetProjectUri(repositoryById.Key.ProjectId);
      }
      ITfsGitRepository repositoryWithExistingOdb = requestContext.GetService<IInternalGitRepoService>().CreateRepositoryWithExistingOdb(requestContext, targetProjectId, newRepositoryName, odbId, new MinimalGlobalRepoKey(empty, source.RepositoryId));
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new RepositoryForkedNotification(projectUri, source.RepositoryId, repositoryWithExistingOdb.Key.RepoId, repositoryWithExistingOdb.Name, requestContext.UserContext));
      bool flag = true;
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        service.SetValue<bool>(requestContext, "/WebAccess" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/VersionControl/Repositories/{0}/WitMentionsEnabled", (object) repositoryWithExistingOdb.Key.RepoId), false);
        service.SetValue<bool>(requestContext, "/WebAccess" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled", (object) repositoryWithExistingOdb.Key.RepoId), false);
        flag = false;
        return repositoryWithExistingOdb;
      }
      finally
      {
        if (flag && repositoryWithExistingOdb != null)
          repositoryWithExistingOdb.Dispose();
      }
    }

    public bool TryCalculateMergeBases(
      ITfsGitRepository repo,
      Sha1Id commit,
      ITfsGitRepository otherRepo,
      Sha1Id otherCommit,
      out IEnumerable<Sha1Id> maybeMergeBases)
    {
      if (repo.Key.OdbId != otherRepo.Key.OdbId)
        throw new ArgumentException("repo and otherRepo do not share an ODB.");
      repo.LookupObject<TfsGitCommit>(commit);
      otherRepo.LookupObject<TfsGitCommit>(otherCommit);
      List<Sha1Id> mergeBases;
      int num = new AncestralGraphAlgorithm<int, Sha1Id>().TryGetMergeBases((IDirectedGraph<int, Sha1Id>) GitServerUtils.GetOdb(repo).GraphProvider.Get((IEnumerable<Sha1Id>) new Sha1Id[2]
      {
        commit,
        otherCommit
      }), commit, otherCommit, out mergeBases) ? 1 : 0;
      maybeMergeBases = (IEnumerable<Sha1Id>) mergeBases;
      return num != 0;
    }

    public bool TryCalculateMergeBase(
      ITfsGitRepository repo,
      Sha1Id commit,
      ITfsGitRepository otherRepo,
      Sha1Id otherCommit,
      out Sha1Id maybeMergeBase)
    {
      IEnumerable<Sha1Id> maybeMergeBases;
      int num = this.TryCalculateMergeBases(repo, commit, otherRepo, otherCommit, out maybeMergeBases) ? 1 : 0;
      maybeMergeBase = maybeMergeBases != null ? maybeMergeBases.FirstOrDefault<Sha1Id>() : Sha1Id.Empty;
      return num != 0;
    }

    public ForkFetchAsyncOp SyncFork(
      IVssRequestContext requestContext,
      GitForkSyncRequestParameters sourceParams,
      RepoKey targetRepository,
      bool copySourceRepoDefaults)
    {
      SecurityHelper.Instance.CheckWritePermission(requestContext, (RepoScope) targetRepository, (string) null);
      RepoKey repoKey = new RepoKey(sourceParams.Source.ProjectId, sourceParams.Source.RepositoryId);
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      ForkFetchAsyncOp asyncOperation = this.CreateAsyncOperation(requestContext, targetRepository, GitAsyncOperationType.Fork, new ForkFetchParams(sourceParams.Source, targetRepository.RepoId, sourceParams.SourceToTargetRefs, copySourceRepoDefaults));
      this.QueueForkFetchJob(requestContext, targetRepository, asyncOperation.OperationId);
      return this.FillDefaultDetailedStatus(asyncOperation);
    }

    public ForkFetchAsyncOp GetFetchRequestById(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int createForkOperationId)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      return this.FillDefaultDetailedStatus(this.GetAsyncOperationById(requestContext, repoKey, createForkOperationId, true));
    }

    public IEnumerable<ForkFetchAsyncOp> QueryFetchRequests(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      bool includeAbandoned)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      IEnumerable<ForkFetchAsyncOp> source = this.QueryAsyncOperationsByType(requestContext, repoKey, GitAsyncOperationType.Fork);
      return (includeAbandoned ? source : source.Where<ForkFetchAsyncOp>((Func<ForkFetchAsyncOp, bool>) (x => x.Status != GitAsyncOperationStatus.Abandoned))).Select<ForkFetchAsyncOp, ForkFetchAsyncOp>((Func<ForkFetchAsyncOp, ForkFetchAsyncOp>) (x => this.FillDefaultDetailedStatus(x)));
    }

    public ForkFetchAsyncOp UpdateFetchProgress(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int asyncOpId,
      GitAsyncOperationStatus status,
      ForkUpdateStep step,
      string errorMessage)
    {
      GitForkOperationStatusDetail detailedStatus = new GitForkOperationStatusDetail()
      {
        CurrentStep = (int) step,
        AllSteps = GitForkService.s_defaultStatusDetail.AllSteps,
        ErrorMessage = errorMessage
      };
      this.UpdateAsyncOperationStatus(requestContext, repoKey, asyncOpId, status, detailedStatus, new GitAsyncOperationStatus?());
      return this.GetAsyncOperationById(requestContext, repoKey, asyncOpId, true);
    }

    private void QueueForkFetchJob(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int createForkOperationId)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/Forks/InitialJobIntervalInSeconds", 15);
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
      {
        Interval = num,
        ScheduledTime = DateTime.UtcNow
      };
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AsyncGitOperationJobData()
      {
        RepositoryId = repoKey.RepoId,
        OperationId = createForkOperationId
      });
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(Guid.NewGuid(), "GitForkFetchJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitForkFetchJob", xml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal);
      foundationJobDefinition.Schedule.Add(foundationJobSchedule);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        foundationJobDefinition.JobId
      });
    }

    private static IEnumerable<GitRepositoryRef> BuildGitRepositoryRefs(
      IVssRequestContext requestContext,
      IEnumerable<MinimalGlobalRepoKey> inDb)
    {
      Dictionary<Guid, TfsGitRepositoryInfo> dictionary = requestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositoriesAcrossProjects(requestContext, inDb.Select<MinimalGlobalRepoKey, Guid>((Func<MinimalGlobalRepoKey, Guid>) (c => c.RepositoryId))).ToDictionary<TfsGitRepositoryInfo, Guid>((Func<TfsGitRepositoryInfo, Guid>) (info => info.Key.RepoId));
      ITeamProjectCollectionPropertiesService service1 = requestContext.To(TeamFoundationHostType.Application).GetService<ITeamProjectCollectionPropertiesService>();
      IProjectService service2 = requestContext.GetService<IProjectService>();
      ILocationService service3 = requestContext.GetService<ILocationService>();
      GitRepositoryRefBuilder repositoryRefBuilder = new GitRepositoryRefBuilder(requestContext, (IReadOnlyDictionary<Guid, TfsGitRepositoryInfo>) dictionary, service1, service2, service3);
      List<GitRepositoryRef> gitRepositoryRefList = new List<GitRepositoryRef>();
      foreach (MinimalGlobalRepoKey repoKey in inDb)
      {
        GitRepositoryRef result;
        if (repositoryRefBuilder.TryGetFromMinimalGlobalKey(repoKey, out result))
          gitRepositoryRefList.Add(result);
      }
      return (IEnumerable<GitRepositoryRef>) gitRepositoryRefList;
    }

    private ForkFetchAsyncOp FillDefaultDetailedStatus(ForkFetchAsyncOp forkRequest)
    {
      if (forkRequest.DetailedStatus == null)
        forkRequest.DetailedStatus = GitForkService.s_defaultStatusDetail;
      return forkRequest;
    }
  }
}
