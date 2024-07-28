// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.InternalGitCommitService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class InternalGitCommitService : IInternalGitCommitService, IVssFrameworkService
  {
    private TimeSpan m_processingJobTimeout = InternalGitCommitService.c_defaultProcessingJobTimeout;
    private int m_commitFlushFrequency = 250;
    private static readonly TimeSpan c_defaultProcessingJobTimeout = TimeSpan.FromMinutes(5.0);
    private const int c_defaultCommitFlushFrequency = 250;
    private const string c_maxCommitsToProcessAtATimeRegistryPath = "/Service/Git/Settings/MaxCommitMetadataPerBatch";
    private const string c_commitProcessingRegistryKeyPrefix = "/Service/Legacy/Settings/CommitProcessing";
    private const string c_commitProcessingRegistryFilter = "/Service/Legacy/Settings/CommitProcessing/*";
    private const string c_jobTimeoutRegistryKey = "/Service/Legacy/Settings/CommitProcessing/JobTimeout";
    private const string c_commitFlushFrequencyRegistryKey = "/Service/Legacy/Settings/CommitProcessing/CommitFlushFrequency";
    private const string c_layer = "InternalGitCommitService";

    public TimeSpan ProcessingJobTimeout => this.m_processingJobTimeout;

    public bool ComputeAndPersistCommitMetadata(IVssRequestContext rc, OdbId odbId)
    {
      List<Sha1Id> source;
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
        source = gitOdbComponent.QueryUncalculatedCommitMetadata();
      IEnumerable<CommitMetadataKey> commits = source.Select<Sha1Id, CommitMetadataKey>((Func<Sha1Id, CommitMetadataKey>) (commitId => new CommitMetadataKey(commitId, new int?(), Guid.Empty)));
      return this.ComputeAndPersistCommitMetadata(rc, odbId, commits, (Func<CommitMetadataKey, CommitMetadataUpdate>) null);
    }

    public bool ComputeAndPersistCommitMetadata(
      IVssRequestContext requestContext,
      OdbId odbId,
      IEnumerable<CommitMetadataKey> commits,
      Func<CommitMetadataKey, CommitMetadataUpdate> metadataUpdateBuilder)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      odbId.CheckValid();
      ArgumentUtility.CheckForNull<IEnumerable<CommitMetadataKey>>(commits, nameof (commits));
      if (requestContext.IsTracing(1013528, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (InternalGitCommitService)))
        requestContext.Trace(1013528, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "QueuePendingCommitMetadata ODB:{0} commitsCount:{1}", (object) odbId, (object) commits.Count<CommitMetadataKey>());
      Stopwatch stopwatch = Stopwatch.StartNew();
      bool persistCommitMetadata = false;
      int num = 0;
      List<CommitMetadataUpdate> commits1 = new List<CommitMetadataUpdate>(this.m_commitFlushFrequency);
      using (Odb odb = DefaultGitDependencyRoot.Instance.CreateOdb(requestContext, odbId))
      {
        metadataUpdateBuilder = metadataUpdateBuilder ?? (Func<CommitMetadataKey, CommitMetadataUpdate>) (key => new CommitMetadataUpdate(odb, odb.ObjectSet.LookupObject<TfsGitCommit>(key.CommitId), (Action<TfsGitDiffEntry>) null));
        foreach (CommitMetadataKey commitMetadataKey in (IEnumerable<CommitMetadataKey>) commits.OrderBy<CommitMetadataKey, int>((Func<CommitMetadataKey, int>) (x => odb.ContentDB.Index.ObjectIds.GetIndex<Sha1Id>(x.CommitId))))
        {
          requestContext.RequestContextInternal().CheckCanceled();
          CommitMetadataUpdate commitMetadataUpdate = metadataUpdateBuilder(commitMetadataKey);
          commits1.Add(commitMetadataUpdate);
          ++num;
          if (commits1.Count >= commits1.Capacity)
          {
            requestContext.Trace(1013531, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "Flushing processed commits. Running processed commit count:{0}, Commits in current batch:{1}.", (object) num, (object) commits1.Count);
            using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(odbId))
              gitOdbComponent.UpdateCommitMetadata((IEnumerable<CommitMetadataUpdate>) commits1);
            commits1.Clear();
          }
          if (stopwatch.Elapsed > this.ProcessingJobTimeout)
          {
            persistCommitMetadata = true;
            break;
          }
        }
        if (commits1.Count > 0)
        {
          requestContext.Trace(1013532, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "Flushing processed commits. Running processed commit count:{0}, Commits in current batch:{1}.", (object) num, (object) commits1.Count);
          using (GitOdbComponent gitOdbComponent = requestContext.CreateGitOdbComponent(odbId))
            gitOdbComponent.UpdateCommitMetadata((IEnumerable<CommitMetadataUpdate>) commits1);
          commits1.Clear();
        }
      }
      requestContext.Trace(1013512, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "ComputeChanges completed. ProcessedCommitCount:{0}", (object) num);
      if (persistCommitMetadata)
        this.QueueCommitMetadataCatchupJob(requestContext, odbId, true, false);
      return persistCommitMetadata;
    }

    public List<UnprocessedCommit> QueryUnprocessedCommits(
      IVssRequestContext rc,
      RepoKey repoKey,
      Guid jobId,
      int take)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      this.CheckForSystemContext(rc);
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      ITeamFoundationGitRepositoryService service = rc.GetService<ITeamFoundationGitRepositoryService>();
      List<TfsGitProcessedCommit> gitProcessedCommitList;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitProcessedCommitList = gitCoreComponent.QueryUnprocessedCommits(repoKey, jobId, take);
      List<UnprocessedCommit> unprocessedCommitList = new List<UnprocessedCommit>(gitProcessedCommitList.Count);
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(rc, repoKey.RepoId))
      {
        foreach (TfsGitProcessedCommit gitProcessedCommit in gitProcessedCommitList)
        {
          if (repositoryById.TryLookupObject(gitProcessedCommit.CommitId) is TfsGitCommit commit)
            unprocessedCommitList.Add(UnprocessedCommit.FromTfsGitCommit(repositoryById.Key, commit, gitProcessedCommit.PushId));
        }
      }
      return unprocessedCommitList;
    }

    public void MarkProcessedCommits(
      IVssRequestContext rc,
      RepoKey repoKey,
      Guid jobId,
      IEnumerable<UnprocessedCommit> commits)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      this.CheckForSystemContext(rc);
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      ArgumentUtility.CheckForNull<IEnumerable<UnprocessedCommit>>(commits, nameof (commits));
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.MarkProcessedCommits(repoKey, jobId, commits);
    }

    public void QueueCommitMetadataCatchupJob(
      IVssRequestContext requestContext,
      OdbId odbId,
      bool queueNow,
      bool idlePriority)
    {
      KeyScopedJobUtil.QueueFor(requestContext, odbId, "GitCommitMetadataCatchupJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitCommitMetadataCatchupJob", idlePriority ? JobPriorityLevel.Idle : JobPriorityLevel.Normal, JobPriorityClass.AboveNormal, queueNow ? 0 : 300);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1013501, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "ServiceStart");
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/Service/Legacy/Settings/CommitProcessing/*");
      this.ReadRegistryValues(service.ReadEntriesFallThru(systemRequestContext, (RegistryQuery) "/Service/Legacy/Settings/CommitProcessing/*"));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1013505, GitServerUtils.TraceArea, nameof (InternalGitCommitService), "ServiceEnd");
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void CheckForSystemContext(IVssRequestContext requestContext)
    {
      if (!requestContext.IsSystemContext)
        throw new InvalidOperationException();
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadRegistryValues(requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/Legacy/Settings/CommitProcessing/*"));
    }

    private void ReadRegistryValues(RegistryEntryCollection registryEntries)
    {
      if (registryEntries.ContainsPath("/Service/Legacy/Settings/CommitProcessing/JobTimeout"))
        this.m_processingJobTimeout = registryEntries.GetValueFromPath<TimeSpan>("/Service/Legacy/Settings/CommitProcessing/JobTimeout", InternalGitCommitService.c_defaultProcessingJobTimeout);
      if (!registryEntries.ContainsPath("/Service/Legacy/Settings/CommitProcessing/CommitFlushFrequency"))
        return;
      this.m_commitFlushFrequency = registryEntries.GetValueFromPath<int>("/Service/Legacy/Settings/CommitProcessing/CommitFlushFrequency", 250);
    }
  }
}
